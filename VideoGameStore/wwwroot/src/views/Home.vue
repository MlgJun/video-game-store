<template>
  <div class="app-root">
    <SiteHeader />

    <main class="home-canvas" role="main" aria-label="Главная страница">
      <section class="three-blocks">
        <aside class="left-block">
          <h2 class="left-title">Новинки!</h2>
          <ol class="new-list">
            <li>Solo Leveling</li>
            <li>Night Swarm</li>
            <li>Escape from Tarkov</li>
            <li>Arc Raiders</li>
            <li>Dispatch</li>
            <li>Hades II</li>
            <li>Megabonk</li>
            <li>No, I'm not a Human</li>
            <li>CloverPit</li>
            <li>Hollow Knight: Silksong</li>
          </ol>
        </aside>

        <div class="center-block">
          <img src="/src/assets/photos/baner.png" alt="baner" />
        </div>

        <div class="right-block">
          <div class="promo-text">Новинка!</div>
          <img src="/src/assets/photos/new.png" alt="new" />
        </div>
      </section>
      
      <div class="price-filter" role="region" aria-label="Фильтр по цене">
        <div class="pf-fields">
          <label class="pf-field">
            <span class="pf-label">Цена от:</span>
            <input 
              class="pf-input" 
              type="number" 
              inputmode="numeric" 
              pattern="\\d*" 
              aria-label="Цена от" 
              min="0" 
              step="1" 
              v-model="minPriceInput"
              @input="validateInput($event, 'min')"
              @keyup.enter="applyFilter"
              @blur="handleInputBlur('min')"
            />
          </label>
          <label class="pf-field">
            <span class="pf-label">До:</span>
            <input 
              class="pf-input" 
              type="number" 
              inputmode="numeric" 
              pattern="\\d*" 
              aria-label="Цена до" 
              min="0" 
              step="1" 
              v-model="maxPriceInput"
              @input="validateInput($event, 'max')"
              @keyup.enter="applyFilter"
              @blur="handleInputBlur('max')"
            />
          </label>
          
          <!-- Кнопка сброса фильтра -->
          <button 
            v-if="filterApplied" 
            class="reset-filter-btn"
            @click="resetFilter"
            aria-label="Сбросить фильтр"
            title="Сбросить фильтр"
          >
            ×
          </button>
        </div>

        <button 
          class="pf-btn" 
          :class="{ 'btn-loading': isLoading, 'btn-success': isSuccess }"
          :disabled="isLoading"
          @click="applyFilter"
          @mouseenter="hoverButton"
          @mouseleave="resetButton"
          aria-label="Готово"
        >
          <span v-if="!isLoading && !isSuccess">Готово</span>
          <span v-else-if="isSuccess" class="success-icon">✓</span>
          <span v-else class="loader"></span>
        </button>
      </div>

      <!-- Информация о фильтре -->
      <div v-if="filterApplied && (minPrice || maxPrice)" class="filter-info">
        <span class="filter-text">Фильтр: </span>
        <span v-if="minPrice && maxPrice" class="filter-range">
          от {{ formatPrice(minPrice) }} до {{ formatPrice(maxPrice) }} ₽
        </span>
        <span v-else-if="minPrice" class="filter-range">
          от {{ formatPrice(minPrice) }} ₽
        </span>
        <span v-else-if="maxPrice" class="filter-range">
          до {{ formatPrice(maxPrice) }} ₽
        </span>
        <span class="filter-count">
          (найдено {{ displayedProducts.length }} из {{ products.length }} товаров)
        </span>
      </div>

      <div class="products-grid">
        <!-- Карточки товаров -->
        <ProductCard 
          v-for="product in paginatedProducts" 
          :key="product.id"
          :product="{
            id: product.id,
            title: product.name,
            price: product.price,
            imageUrl: product.image,
            count: product.count || 10
          }"
          @open-details="openProductModal"
          @add-to-cart="handleAddToCart"
        />
        
        <div v-if="displayedProducts.length === 0 && filterApplied" class="no-results">
          {{ noResultsMessage }}
        </div>
      </div>

      <!-- Пагинация -->
      <div v-if="totalPages > 1 && displayedProducts.length > 0" class="pagination">
        <button 
          class="page-btn prev-btn" 
          @click="prevPage"
          :disabled="currentPage === 1"
          aria-label="Предыдущая страница"
        >
          ←
        </button>
        
        <div class="page-numbers">
          <button 
            v-for="page in visiblePages" 
            :key="page"
            class="page-number" 
            :class="{ active: page === currentPage }"
            @click="goToPage(page)"
            :aria-label="`Страница ${page}`"
            :aria-current="page === currentPage ? 'page' : null"
          >
            {{ page }}
          </button>
          
          <span v-if="hasEllipsisEnd" class="page-ellipsis">...</span>
        </div>
        
        <button 
          class="page-btn next-btn" 
          @click="nextPage"
          :disabled="currentPage === totalPages"
          aria-label="Следующая страница"
        >
          →
        </button>
        
        <div class="page-info">
          Страница {{ currentPage }} из {{ totalPages }}
        </div>
      </div>
      
      <!-- Модальное окно деталей товара -->
      <ProductDetails
        v-if="selectedProductId"
        :game-id="selectedProductId"
        :is-visible="!!selectedProductId"
        @close="closeProductModal"
        @add-to-cart="handleAddToCartFromModal"
        @buy-now="handleBuyNow"
      />
      
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import SiteHeader from '../components/Header.vue'
import Footer from '../components/Footer.vue'
import ProductCard from '../components/ProductCard.vue'
import ProductDetails from '../components/ProductDetails.vue'


// Реактивные данные для фильтров
const minPriceInput = ref('')
const maxPriceInput = ref('')
const minPrice = ref(null)
const maxPrice = ref(null)
const isLoading = ref(false)
const isSuccess = ref(false)
const filterApplied = ref(false)

// Модальное окно
const selectedProductId = ref(null)

// Пагинация
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Массив товаров
const products = ref([
  {
    id: 1,
    name: 'The Witcher 3: Wild Hunt',
    price: 1499,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/292030/capsule_616x353.jpg',
    description: 'Эпическая RPG игра от CD Projekt Red',
    count: 10,
    developerTitle: 'CD Projekt Red',
    publisherTitle: 'CD Projekt',
    genres: ['RPG', 'Action', 'Open World'],
    createdAt: '2023-01-01T00:00:00'
  },
  {
    id: 2,
    name: 'Cyberpunk 2077',
    price: 1999,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/capsule_616x353.jpg',
    description: 'Научно-фантастическая RPG в мире будущего',
    count: 8,
    developerTitle: 'CD Projekt Red',
    publisherTitle: 'CD Projekt',
    genres: ['RPG', 'Action', 'Sci-Fi'],
    createdAt: '2023-02-01T00:00:00'
  },
  {
    id: 3,
    name: 'Elden Ring',
    price: 2499,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/capsule_616x353.jpg',
    description: 'Эпическая action-RPG от создателей Dark Souls',
    count: 5,
    developerTitle: 'FromSoftware',
    publisherTitle: 'Bandai Namco',
    genres: ['RPG', 'Action', 'Fantasy'],
    createdAt: '2023-03-01T00:00:00'
  },
  {
    id: 4,
    name: 'Red Dead Redemption 2',
    price: 1799,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/capsule_616x353.jpg',
    description: 'Приключенческий вестерн от Rockstar Games',
    count: 12,
    developerTitle: 'Rockstar Games',
    publisherTitle: 'Rockstar Games',
    genres: ['Action', 'Adventure', 'Western'],
    createdAt: '2023-04-01T00:00:00'
  },
  {
    id: 5,
    name: 'Baldur\'s Gate 3',
    price: 2999,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/capsule_616x353.jpg',
    description: 'Эпическая RPG на базе Dungeons & Dragons',
    count: 3,
    developerTitle: 'Larian Studios',
    publisherTitle: 'Larian Studios',
    genres: ['RPG', 'Adventure', 'Strategy'],
    createdAt: '2023-05-01T00:00:00'
  },
  {
    id: 6,
    name: 'Counter-Strike 2',
    price: 0,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/730/capsule_616x353.jpg',
    description: 'Командный шутер от первого лица',
    count: 999,
    developerTitle: 'Valve',
    publisherTitle: 'Valve',
    genres: ['FPS', 'Action', 'Multiplayer'],
    createdAt: '2023-06-01T00:00:00'
  },
  {
    id: 7,
    name: 'Dota 2',
    price: 0,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/570/capsule_616x353.jpg',
    description: 'MOBA игра от Valve',
    count: 999,
    developerTitle: 'Valve',
    publisherTitle: 'Valve',
    genres: ['MOBA', 'Strategy', 'Multiplayer'],
    createdAt: '2023-07-01T00:00:00'
  },
  {
    id: 8,
    name: 'Apex Legends',
    price: 0,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1172470/capsule_616x353.jpg',
    description: 'Бесплатный королевский бой от Respawn',
    count: 999,
    developerTitle: 'Respawn Entertainment',
    publisherTitle: 'Electronic Arts',
    genres: ['Battle Royale', 'FPS', 'Action'],
    createdAt: '2023-08-01T00:00:00'
  },
  {
    id: 9,
    name: 'Grand Theft Auto V',
    price: 899,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/271590/capsule_616x353.jpg',
    description: 'Культовая игра в жанре action-adventure',
    count: 20,
    developerTitle: 'Rockstar North',
    publisherTitle: 'Rockstar Games',
    genres: ['Action', 'Adventure', 'Open World'],
    createdAt: '2023-09-01T00:00:00'
  },
  {
    id: 10,
    name: 'The Sims 4',
    price: 0,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1222670/capsule_616x353.jpg',
    description: 'Симулятор жизни от Maxis',
    count: 999,
    developerTitle: 'Maxis',
    publisherTitle: 'Electronic Arts',
    genres: ['Simulation', 'Life', 'Sandbox'],
    createdAt: '2023-10-01T00:00:00'
  },
  {
    id: 11,
    name: 'Call of Duty: Modern Warfare III',
    price: 3499,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1938090/capsule_616x353.jpg',
    description: 'Шутер от первого лица от Activision',
    count: 7,
    developerTitle: 'Sledgehammer Games',
    publisherTitle: 'Activision',
    genres: ['FPS', 'Action', 'War'],
    createdAt: '2023-11-01T00:00:00'
  },
  {
    id: 12,
    name: 'Starfield',
    price: 2799,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1716740/capsule_616x353.jpg',
    description: 'Научно-фантастическая RPG от Bethesda',
    count: 9,
    developerTitle: 'Bethesda Game Studios',
    publisherTitle: 'Bethesda Softworks',
    genres: ['RPG', 'Action', 'Sci-Fi'],
    createdAt: '2023-12-01T00:00:00'
  },
  {
    id: 13,
    name: 'Forza Horizon 5',
    price: 2199,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/capsule_616x353.jpg',
    description: 'Аркадный гоночный симулятор',
    count: 15,
    developerTitle: 'Playground Games',
    publisherTitle: 'Xbox Game Studios',
    genres: ['Racing', 'Simulation', 'Open World'],
    createdAt: '2024-01-01T00:00:00'
  },
  {
    id: 14,
    name: 'Hogwarts Legacy',
    price: 2599,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/990080/capsule_616x353.jpg',
    description: 'Игра по вселенной Гарри Поттера',
    count: 6,
    developerTitle: 'Avalanche Software',
    publisherTitle: 'Warner Bros. Games',
    genres: ['Action', 'RPG', 'Adventure'],
    createdAt: '2024-02-01T00:00:00'
  },
  {
    id: 15,
    name: 'Marvel\'s Spider-Man Remastered',
    price: 2399,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1817070/capsule_616x353.jpg',
    description: 'Экшен-игра о Человеке-пауке',
    count: 11,
    developerTitle: 'Insomniac Games',
    publisherTitle: 'PlayStation PC LLC',
    genres: ['Action', 'Adventure', 'Superhero'],
    createdAt: '2024-03-01T00:00:00'
  },
  {
    id: 16,
    name: 'God of War',
    price: 2299,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1593500/capsule_616x353.jpg',
    description: 'Эпическая action-adventure игра',
    count: 8,
    developerTitle: 'Santa Monica Studio',
    publisherTitle: 'PlayStation PC LLC',
    genres: ['Action', 'Adventure', 'Mythology'],
    createdAt: '2024-04-01T00:00:00'
  },
  {
    id: 17,
    name: 'Resident Evil 4',
    price: 2499,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/2050650/capsule_616x353.jpg',
    description: 'Хоррор выживания от Capcom',
    count: 4,
    developerTitle: 'Capcom',
    publisherTitle: 'Capcom',
    genres: ['Horror', 'Action', 'Survival'],
    createdAt: '2024-05-01T00:00:00'
  },
  {
    id: 18,
    name: 'Dead Space',
    price: 2099,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1693980/capsule_616x353.jpg',
    description: 'Ремейк культового хоррора',
    count: 7,
    developerTitle: 'Motive Studio',
    publisherTitle: 'Electronic Arts',
    genres: ['Horror', 'Action', 'Sci-Fi'],
    createdAt: '2024-06-01T00:00:00'
  },
  {
    id: 19,
    name: 'Sea of Thieves',
    price: 899,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1172620/capsule_616x353.jpg',
    description: 'Пиратская приключенческая игра',
    count: 999,
    developerTitle: 'Rare',
    publisherTitle: 'Xbox Game Studios',
    genres: ['Adventure', 'Action', 'Multiplayer'],
    createdAt: '2024-07-01T00:00:00'
  },
  {
    id: 20,
    name: 'Fallout 4',
    price: 999,
    image: 'https://cdn.cloudflare.steamstatic.com/steam/apps/377160/capsule_616x353.jpg',
    description: 'Постапокалиптическая RPG',
    count: 14,
    developerTitle: 'Bethesda Game Studios',
    publisherTitle: 'Bethesda Softworks',
    genres: ['RPG', 'Action', 'Open World'],
    createdAt: '2024-08-01T00:00:00'
  }
])

// Форматирование цены
const formatPrice = (price) => {
  if (!price && price !== 0) return '0'
  return new Intl.NumberFormat('ru-RU').format(price)
}

// Валидация ввода
const validateInput = (event, type) => {
  const input = event.target
  let value = input.value.replace(/[^0-9]/g, '')
  
  if (value.length > 7) {
    value = value.slice(0, 7)
  }
  
  input.value = value
  
  if (type === 'min') {
    minPriceInput.value = value
  } else {
    maxPriceInput.value = value
  }
}

// Обработка потери фокуса
const handleInputBlur = (type) => {
  if (type === 'min' && minPriceInput.value) {
    const value = parseInt(minPriceInput.value)
    if (value < 0) minPriceInput.value = '0'
    if (value > 9999999) minPriceInput.value = '9999999'
    
    if (maxPriceInput.value && value > parseInt(maxPriceInput.value)) {
      maxPriceInput.value = minPriceInput.value
    }
  }
  
  if (type === 'max' && maxPriceInput.value) {
    const value = parseInt(maxPriceInput.value)
    if (value < 0) maxPriceInput.value = '0'
    if (value > 9999999) maxPriceInput.value = '9999999'
    
    if (minPriceInput.value && value < parseInt(minPriceInput.value)) {
      minPriceInput.value = maxPriceInput.value
    }
  }
}

// Применение фильтра
const applyFilter = async () => {
  if (isLoading.value) return
  
  isLoading.value = true
  
  minPrice.value = minPriceInput.value ? parseInt(minPriceInput.value) : null
  maxPrice.value = maxPriceInput.value ? parseInt(maxPriceInput.value) : null
  
  if (minPrice.value !== null && maxPrice.value !== null && minPrice.value > maxPrice.value) {
    maxPrice.value = minPrice.value
    maxPriceInput.value = minPriceInput.value
  }
  
  filterApplied.value = true
  currentPage.value = 1
  
  // Имитация загрузки данных
  await new Promise(resolve => setTimeout(resolve, 600))
  
  isLoading.value = false
  isSuccess.value = true
  
  setTimeout(() => {
    isSuccess.value = false
  }, 2000)
}

// Сброс фильтра
const resetFilter = () => {
  minPriceInput.value = ''
  maxPriceInput.value = ''
  minPrice.value = null
  maxPrice.value = null
  filterApplied.value = false
  currentPage.value = 1
}

// Открытие модалки
const openProductModal = (productId) => {
  selectedProductId.value = productId
  document.body.style.overflow = 'hidden'
}

// Закрытие модалки
const closeProductModal = () => {
  selectedProductId.value = null
  document.body.style.overflow = ''
}

// Обработка добавления в корзину из модалки
const handleAddToCartFromModal = (productId) => {
  console.log('Товар добавлен в корзину из модалки:', productId)
  const product = products.value.find(p => p.id === productId)
  if (product && product.count > 0) {
    product.count -= 1
  }
  alert('Товар добавлен в корзину!')
}

// Обработка добавления в корзину из карточки
const handleAddToCart = (productId) => {
  console.log('Товар добавлен в корзину из карточки:', productId)
  const product = products.value.find(p => p.id === productId)
  if (product && product.count > 0) {
    product.count -= 1
  }
}

// Обработка покупки
const handleBuyNow = (order) => {
  console.log('Заказ создан:', order)
  alert('Заказ успешно оформлен!')
  closeProductModal()
}

// Закрытие по Escape
const handleEscape = (e) => {
  if (e.key === 'Escape' && selectedProductId.value) {
    closeProductModal()
  }
}

// При монтировании
onMounted(() => {
  document.addEventListener('keydown', handleEscape)
})

// При размонтировании
onUnmounted(() => {
  document.removeEventListener('keydown', handleEscape)
  document.body.style.overflow = ''
})

// Отфильтрованные товары
const displayedProducts = computed(() => {
  if (!filterApplied.value) {
    return products.value
  }
  
  return products.value.filter(product => {
    const price = product.price
    
    if (minPrice.value === null && maxPrice.value === null) {
      return true
    }
    
    const minValid = minPrice.value === null || price >= minPrice.value
    const maxValid = maxPrice.value === null || price <= maxPrice.value
    
    return minValid && maxValid
  })
})

// Пагинация
const totalPages = computed(() => {
  return Math.ceil(displayedProducts.value.length / itemsPerPage.value)
})

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return displayedProducts.value.slice(start, end)
})

// Отображаемые номера страниц
const visiblePages = computed(() => {
  const pages = []
  const maxVisible = 5
  
  if (totalPages.value <= maxVisible) {
    for (let i = 1; i <= totalPages.value; i++) {
      pages.push(i)
    }
  } else {
    let start = Math.max(1, currentPage.value - 2)
    let end = Math.min(totalPages.value, start + maxVisible - 1)
    
    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1)
    }
    
    for (let i = start; i <= end; i++) {
      pages.push(i)
    }
  }
  
  return pages
})

const hasEllipsisEnd = computed(() => {
  return visiblePages.value[visiblePages.value.length - 1] < totalPages.value
})

// Сообщение при отсутствии результатов
const noResultsMessage = computed(() => {
  if (!filterApplied.value) return ''
  
  if (minPrice.value !== null && maxPrice.value !== null) {
    return `Товары в диапазоне от ${formatPrice(minPrice.value)} до ${formatPrice(maxPrice.value)} ₽ не найдены`
  } else if (minPrice.value !== null) {
    return `Товары от ${formatPrice(minPrice.value)} ₽ не найдены`
  } else if (maxPrice.value !== null) {
    return `Товары до ${formatPrice(maxPrice.value)} ₽ не найдены`
  }
  
  return 'Товары не найдены'
})

// Функции пагинации
const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
    scrollToTop()
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    scrollToTop()
  }
}

const goToPage = (page) => {
  currentPage.value = page
  scrollToTop()
}

const scrollToTop = () => {
  window.scrollTo({
    top: document.querySelector('.products-grid').offsetTop - 100,
    behavior: 'smooth'
  })
}

// Анимация при наведении
const hoverButton = (event) => {
  if (!isLoading.value && !isSuccess.value) {
    event.target.style.transform = 'translateY(-2px)'
  }
}

const resetButton = (event) => {
  if (!isLoading.value && !isSuccess.value) {
    event.target.style.transform = 'translateY(0)'
  }
}
</script>

<style> 
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');

html, body, #app {
  height: 100%;
  margin: 0;
}

body {
  font-family: Inter, system-ui, Arial, sans-serif;
  background: #F5F5F5;
}

.app-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.home-canvas {
  max-width: 1920px;
  width: 100%;
  min-height: calc(100vh - 164px);
  background: #F5F5F5;
}

/* ========== АДАПТИВ ДЛЯ БЛОКОВ ВВЕРХУ ========== */
.three-blocks {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 110px;
  margin-top: 36px;
  padding: 0 64px;
  box-sizing: border-box;
}

.left-block {
  width: 370px;
  height: 400px;
  background: #EFEFEF;
  border-radius: 30px;
  padding: 22px;
  box-sizing: border-box;
}

.left-title {
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 36px;
  margin: 0 0 12px;
  color: #111;
  padding-left: 8px;
}

.new-list {
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 18px;
  line-height: 1.25;
  margin: 0;
  padding-left: 32px;
  color: #111;
  overflow-wrap: break-word;
  hyphens: auto;
}

.new-list li {
  margin: 6px 0;
}

.center-block {
  width: 780px;
  height: 440px;
  border-radius: 30px;
  overflow: hidden;
  align-self: center;
}

.center-block img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.right-block {
  width: 370px;
  height: 400px;
  border-radius: 30px;
  overflow: hidden;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  align-self: center;
}

.right-block img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.promo-text {
  position: absolute;
  top: 18px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 36px;
  color: #ffffff;
  text-align: center;
  pointer-events: none;
}

/* ========== АДАПТИВ ДЛЯ ФИЛЬТРА ЦЕНЫ ========== */
.price-filter {
  width: 600px;
  max-width: 90%;
  margin: 35px auto 0;
  background: #E2E2E2;
  border-radius: 30px;
  height: 60px;
  padding: 10px 18px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  box-sizing: border-box;
  font-size: 20px;
}

.pf-label {
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 20px;
  color: #A53DFF;
  margin-right: 6px;
}

.pf-fields {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
}

.pf-field {
  display: flex;
  align-items: center;
  gap: 6px;
}

.reset-filter-btn {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: #A53DFF;
  color: white;
  border: none;
  font-size: 20px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  margin-left: 8px;
  flex-shrink: 0;
}

.reset-filter-btn:hover {
  background: #8C2BD9;
  transform: rotate(90deg) scale(1.1);
}

.reset-filter-btn:active {
  transform: rotate(90deg) scale(0.95);
}

.filter-info {
  text-align: center;
  margin: 20px auto 0;
  padding: 12px 24px;
  background: #E2E2E2;
  border-radius: 20px;
  max-width: 800px;
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 16px;
  color: #333;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
  gap: 8px;
  animation: fadeIn 0.5s ease-out;
}

.filter-text {
  font-weight: 600;
  color: #A53DFF;
}

.filter-range {
  font-weight: 600;
  color: #222;
}

.filter-count {
  color: #666;
  font-size: 14px;
}

.pf-input {
  width: 90px;
  height: 28px;
  background: #F4F4F4;
  border-radius: 30px;
  border: 0;
  padding: 4px 8px;
  font-size: 20px;
  font-family: 'Montserrat Alternates', sans-serif;
  box-sizing: border-box;
  text-align: left;
}

.pf-input[type="number"]::-webkit-outer-spin-button,
.pf-input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  appearance: none;
  margin: 0;
}

.pf-input[type="number"] {
  -moz-appearance: textfield;
  -webkit-appearance: none;
  appearance: none;
}

.pf-btn {
  width: 130px;
  height: 40px;
  background: #222222;
  color: #fff;
  border-radius: 30px;
  border: 0;
  cursor: pointer;
  font-size: 20px;
  font-family: 'Montserrat Alternates', sans-serif;
  box-shadow: 0 6px 16px rgba(0,0,0,0.25);
  align-self: center;
  margin-left: 10px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pf-btn:not(.btn-loading):not(.btn-success):hover {
  background: #A53DFF;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(165, 61, 255, 0.3);
}

.pf-btn:not(.btn-loading):not(.btn-success):active {
  transform: translateY(0);
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
}

.pf-btn.btn-loading {
  background: #444;
  cursor: not-allowed;
}

.pf-btn.btn-success {
  background: #4CAF50;
  animation: successPulse 0.5s ease-in-out;
}

@keyframes successPulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
}

.loader {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255,255,255,0.3);
  border-radius: 50%;
  border-top-color: #fff;
  animation: spin 1s ease-in-out infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.success-icon {
  font-size: 24px;
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* ========== АДАПТИВ ДЛЯ СЕТКИ ТОВАРОВ ========== */
.products-grid {
  max-width: 1600px;
  width: 100%;
  margin: 40px auto 0;
  padding: 0 40px;
  box-sizing: border-box;
  
  /* Grid разметка - максимум 5 карточек в строку */
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 30px;
  justify-content: center;
  justify-items: center;
}

/* Адаптивные точки */
@media (max-width: 1600px) {
  .products-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

@media (max-width: 1200px) {
  .products-grid {
    grid-template-columns: repeat(3, 1fr);
    padding: 0 30px;
  }
  .left-block,
  .right-block {
    display: none !important;
  }
}

@media (max-width: 992px) {
  .products-grid {
    grid-template-columns: repeat(2, 1fr);
    padding: 0 20px;
  }
}

@media (max-width: 576px) {
  .products-grid {
    grid-template-columns: 1fr;
    padding: 0 15px;
    gap: 20px;
    max-width: 400px;
  }
}


/* ========== АДАПТИВ ДЛЯ ПАГИНАЦИИ ========== */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin: 40px 0;
  padding: 0 64px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.page-btn {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  border: 2px solid #A53DFF;
  background: white;
  color: #A53DFF;
  font-size: 20px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.page-btn:hover:not(:disabled) {
  background: #A53DFF;
  color: white;
}

.page-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.page-numbers {
  display: flex;
  gap: 8px;
}

.page-number {
  min-width: 40px;
  height: 40px;
  border-radius: 8px;
  border: 2px solid #E2E2E2;
  background: white;
  color: #333;
  font-size: 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.page-number:hover {
  border-color: #A53DFF;
}

.page-number.active {
  background: #A53DFF;
  color: white;
  border-color: #A53DFF;
}

.page-ellipsis {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #666;
  padding: 0 8px;
}

.page-info {
  font-size: 14px;
  color: #666;
  margin-left: 16px;
}

.no-results {
  width: 100%;
  text-align: center;
  padding: 40px;
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 18px;
  color: #666;
  animation: fadeInUp 0.5s ease-out;
  grid-column: 1 / -1;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* ========== МЕДИА-ЗАПРОСЫ ДЛЯ АДАПТИВНОСТИ ========== */

/* Большие планшеты и маленькие десктопы (1200px - 1400px) */
@media (max-width: 1400px) {
  .three-blocks {
    gap: 60px;
    padding: 0 40px;
  }
  
  .left-block,
  .right-block {
    width: 320px;
    height: 360px;
  }
  
  .center-block {
    width: 650px;
    height: 380px;
  }
  
  .left-title {
    font-size: 32px;
  }
  
  .new-list {
    font-size: 16px;
  }
  
  .promo-text {
    font-size: 32px;
  }
  
  .products-grid {
    padding: 0 40px;
    gap: 30px;
    grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  }
}

/* Планшеты (768px - 1200px) */
@media (max-width: 1200px) {
  .three-blocks {
    flex-direction: column;
    gap: 30px;
    padding: 0 30px;
  }
  
  .left-block,
  .center-block,
  .right-block {
    width: 100%;
    max-width: 780px;
  }
  
  .left-block,
  .right-block {
    height: 200px;
  }
  
  .center-block {
    height: 300px;
  }
  
  .left-title {
    font-size: 28px;
    text-align: center;
  }
  
  .new-list {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    padding-left: 20px;
  }
  
  .new-list li {
    width: calc(50% - 10px);
    margin: 3px 0;
  }
  
  .promo-text {
    font-size: 28px;
  }
  
  .price-filter {
    width: 500px;
    padding: 10px 15px;
  }
  
  .pf-label {
    font-size: 18px;
  }
  
  .pf-input {
    width: 80px;
    font-size: 18px;
  }
  
  .pf-btn {
    width: 110px;
    height: 36px;
    font-size: 18px;
  }
  
  .products-grid {
    padding: 0 30px;
    gap: 25px;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  }
  
  .pagination {
    padding: 0 30px;
    gap: 12px;
  }
  
  .page-btn,
  .page-number {
    width: 36px;
    height: 36px;
    font-size: 16px;
  }
  
  .page-info {
    font-size: 13px;
  }
}

/* Маленькие планшеты и большие телефоны (576px - 768px) */
@media (max-width: 768px) {
  .home-canvas {
    min-height: calc(100vh - 140px);
  }
  
  .three-blocks {
    margin-top: 20px;
    padding: 0 20px;
    gap: 20px;
  }
  
  .left-block,
  .right-block {
    height: 180px;
    padding: 15px;
  }
  
  .center-block {
    height: 250px;
  }
  
  .left-title {
    font-size: 24px;
    margin-bottom: 8px;
  }
  
  .new-list {
    font-size: 14px;
    padding-left: 15px;
  }
  
  .promo-text {
    font-size: 24px;
    top: 12px;
  }
  
  .price-filter {
    width: 90%;
    height: 50px;
    padding: 8px 12px;
    margin-top: 25px;
  }
  
  .pf-fields {
    gap: 8px;
  }
  
  .pf-label {
    font-size: 16px;
    margin-right: 4px;
  }
  
  .pf-input {
    width: 70px;
    height: 24px;
    font-size: 16px;
    padding: 2px 6px;
  }
  
  .pf-btn {
    width: 100px;
    height: 32px;
    font-size: 16px;
    margin-left: 8px;
  }
  
  .filter-info {
    padding: 10px 15px;
    font-size: 14px;
    max-width: 90%;
  }
  
  .filter-count {
    font-size: 12px;
  }
  
  .products-grid {
    padding: 0 20px;
    margin-top: 30px;
    gap: 20px;
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  }
  
  .pagination {
    flex-wrap: wrap;
    padding: 0 20px;
    margin: 30px 0;
    gap: 10px;
  }
  
  .page-btn,
  .page-number {
    width: 32px;
    height: 32px;
    font-size: 14px;
  }
  
  .page-numbers {
    order: 3;
    width: 100%;
    justify-content: center;
    margin-top: 10px;
  }
  
  .page-info {
    order: 2;
    margin-left: 0;
    font-size: 12px;
  }
  
  .no-results {
    padding: 30px 20px;
    font-size: 16px;
  }
}

/* Телефоны (до 576px) */
@media (max-width: 576px) {
  .three-blocks {
    padding: 0 15px;
  }
  
  .left-block,
  .right-block {
    height: 160px;
    padding: 12px;
  }
  
  .center-block {
    height: 200px;
  }
  
  .left-title {
    font-size: 20px;
  }
  
  .new-list {
    font-size: 12px;
    padding-left: 10px;
  }
  
  .new-list li {
    width: 100%;
  }
  
  .promo-text {
    font-size: 20px;
    top: 10px;
  }
  
  .price-filter {
    flex-direction: column;
    height: auto;
    padding: 15px;
    gap: 12px;
    border-radius: 20px;
  }
  
  .pf-fields {
    width: 100%;
    justify-content: center;
  }
  
  .pf-btn {
    width: 100%;
    margin-left: 0;
  }
  
  .products-grid {
    padding: 0 15px;
    gap: 15px;
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  }
  
  .pagination {
    padding: 0 15px;
  }
  
  .page-btn,
  .page-number {
    width: 30px;
    height: 30px;
    font-size: 13px;
  }
  
  .page-numbers {
    gap: 5px;
  }
  
  .no-results {
    padding: 20px 15px;
    font-size: 14px;
  }
  
  .filter-info {
    font-size: 12px;
    padding: 8px 12px;
  }
  
  .filter-count {
    font-size: 11px;
  }
}

/* Очень маленькие телефоны (до 400px) */
@media (max-width: 400px) {
  .three-blocks {
    padding: 0 10px;
  }
  
  .left-block,
  .right-block {
    height: 140px;
    padding: 10px;
    border-radius: 20px;
  }
  
  .center-block {
    height: 180px;
    border-radius: 20px;
  }
  
  .left-title {
    font-size: 18px;
  }
  
  .new-list {
    font-size: 11px;
  }
  
  .promo-text {
    font-size: 18px;
  }
  
  .price-filter {
    padding: 12px;
    border-radius: 15px;
  }
  
  .pf-input {
    width: 60px;
    font-size: 14px;
  }
  
  .pf-label {
    font-size: 14px;
  }
  
  .products-grid {
    padding: 0 10px;
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }
  
  .pagination {
    padding: 0 10px;
  }
  
  .page-btn,
  .page-number {
    width: 28px;
    height: 28px;
    font-size: 12px;
  }
}
</style>