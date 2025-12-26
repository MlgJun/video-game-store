<template>
  <div class="cart-root">
    <SiteHeader />

    <main class="cart-canvas">
      <div class="cart-inner">
        <section class="cart-container">
          <div class="cart-header">
            <h1 class="cart-title">Корзина</h1>
            <div class="cart-count">
              <span class="cart-items-number">{{ totalItems }}</span>
              <span class="cart-items-label">товаров</span>
            </div>
          </div>

          <!-- Уведомление для гостей -->
          <div v-if="!isLoggedIn" class="guest-notice">
            <div class="guest-icon">👤</div>
            <div class="guest-text">
              <p>Вы просматриваете корзину как гость.</p>
              <p>
                <router-link to="/login" class="guest-login-link">Войдите</router-link>
                или 
                <router-link to="/register" class="guest-register-link">зарегистрируйтесь</router-link>
                для сохранения корзины.
              </p>
            </div>
          </div>

          <div class="cart-controls">
            <CheckBox 
              v-model="allChecked" 
              class="select-all-checkbox" 
              @update:modelValue="toggleAllItems"
            />
            <div class="cart-select-all">Выбрать всё</div>
          </div>

          <div class="cart-divider"></div>

          <div class="cart-body">
            <CartItem 
              v-for="item in cartItems" 
              :key="item.id"
              :item="item"
              :checked="isItemChecked(item.id)"
              @delete="removeItem"
              @download="downloadItem"
              @update:item="updateItem"
              @update:checked="updateItemChecked"
            />
            
            <div v-if="cartItems.length === 0" class="cart-empty">
              <div class="empty-icon">🛒</div>
              <h3 class="empty-title">Корзина пуста</h3>
              <p class="empty-text">Добавьте товары, чтобы сделать заказ</p>
              <router-link to="/" class="empty-link">
                Перейти к покупкам
              </router-link>
            </div>
          </div>
        </section>

        <aside class="cart-sidebar">
          <div class="summary-box">
            <div class="summary-title">Итого:</div>
            <div class="summary-price">{{ totalPrice }} ₽</div>
            
            <!-- Разные состояния кнопки заказа -->
            <div v-if="!isLoggedIn" class="order-guest-section">
              <button 
                class="order-btn guest-order-btn"
                @click="handleGuestOrder"
                :disabled="isOrdering || cartItems.length === 0"
              >
                Оформить как гость
              </button>
              <p class="guest-order-note">
                Для оформления заказа потребуется указать email
              </p>
            </div>
            
            <div v-else>
              <button 
                class="order-btn"
                @click="handleOrder"
                :disabled="isOrdering || cartItems.length === 0"
              >
                Заказать
              </button>
            </div>
            
            <div class="summary-note">{{ totalItems }} товаров</div>
          </div>
        </aside>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import SiteHeader from '../components/Header.vue'
import Footer from '../components/Footer.vue'
import CheckBox from '../components/CheckBox.vue'
import CartItem from '../components/CartItem.vue'

const router = useRouter()
const allChecked = ref(false)
const isOrdering = ref(false)
const checkedItems = ref(new Set()) // Храним ID выбранных товаров

// Симуляция статуса авторизации (в реальном приложении используйте store)
const isLoggedIn = ref(false)

// Загрузка корзины из localStorage для гостей
const loadGuestCart = () => {
  try {
    const savedCart = localStorage.getItem('guestCart')
    if (savedCart) {
      const parsed = JSON.parse(savedCart)
      return Array.isArray(parsed) ? parsed : []
    }
  } catch (e) {
    console.error('Ошибка загрузки корзины гостя:', e)
  }
  return []
}

// Сохранение корзины в localStorage
const saveGuestCart = (cart) => {
  try {
    localStorage.setItem('guestCart', JSON.stringify(cart))
  } catch (e) {
    console.error('Ошибка сохранения корзины:', e)
  }
}

// Загрузка статуса авторизации (пример)
const loadAuthStatus = () => {
  // В реальном приложении здесь будет проверка токена/сессии
  const token = localStorage.getItem('authToken')
  isLoggedIn.value = !!token
}

// Инициализация корзины
const cartItems = ref(loadGuestCart())

// Наблюдатель для автосохранения корзины
watch(cartItems, (newCart) => {
  saveGuestCart(newCart)
}, { deep: true })

// Следим за состоянием чекбоксов
watch(() => cartItems.value.length, () => {
  updateAllCheckedState()
})

// Загрузка при монтировании
onMounted(() => {
  loadAuthStatus()
  
  // Слушаем события обновления корзины
  window.addEventListener('add-to-cart', handleExternalAddToCart)
  
  // Инициализируем состояние выбранных товаров
  updateAllCheckedState()
})

onBeforeUnmount(() => {
  window.removeEventListener('add-to-cart', handleExternalAddToCart)
})

// Обновление состояния чекбокса "Выбрать всё"
const updateAllCheckedState = () => {
  if (cartItems.value.length === 0) {
    allChecked.value = false
    return
  }
  
  allChecked.value = cartItems.value.every(item => checkedItems.value.has(item.id))
}

// Обработчик внешних событий добавления в корзину
const handleExternalAddToCart = (event) => {
  if (event.detail && event.detail.product) {
    addToCart(event.detail.product)
  }
}

// Функция добавления товара в корзину
const addToCart = (product) => {
  const existingItem = cartItems.value.find(item => item.id === product.id)
  
  if (existingItem) {
    // Увеличиваем количество если товар уже есть
    existingItem.quantity += 1
    cartItems.value = [...cartItems.value] // триггерим реактивность
  } else {
    // Добавляем новый товар
    cartItems.value.push({
      id: product.id,
      title: product.title || product.name,
      platform: 'Steam ключ',
      price: product.price,
      quantity: 1,
      image: product.imageUrl || product.image,
      link: `/product/${product.id}`,
      checked: false
    })
  }
  
  // Автоматически выбираем добавленный товар
  checkedItems.value.add(product.id)
  
  // Показываем уведомление
  showNotification(`Товар "${product.title || product.name}" добавлен в корзину!`)
  
  updateAllCheckedState()
}

// Уведомление
const showNotification = (message) => {
  // В реальном приложении можно использовать toast-библиотеку
  alert(message)
}

const totalItems = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
})

const totalPrice = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
})

// Проверка, выбран ли товар
const isItemChecked = (itemId) => {
  return checkedItems.value.has(itemId)
}

// Обновление состояния выбора товара
const updateItemChecked = ({ itemId, checked }) => {
  if (checked) {
    checkedItems.value.add(itemId)
  } else {
    checkedItems.value.delete(itemId)
  }
  updateAllCheckedState()
}

// Выбрать/снять все товары
const toggleAllItems = (checked) => {
  if (checked) {
    // Выбрать все
    cartItems.value.forEach(item => {
      checkedItems.value.add(item.id)
    })
  } else {
    // Снять все
    checkedItems.value.clear()
  }
}

// Оформление заказа для авторизованного пользователя
const handleOrder = () => {
  if (isOrdering.value || cartItems.value.length === 0) return
  
  isOrdering.value = true
  
  setTimeout(() => {
    isOrdering.value = false
    alert(`Заказ оформлен на сумму ${totalPrice.value} ₽!`)
    // Очищаем корзину после успешного заказа
    cartItems.value = []
    checkedItems.value.clear()
  }, 1500)
}

// Оформление заказа для гостя
const handleGuestOrder = () => {
  if (isOrdering.value || cartItems.value.length === 0) return
  
  isOrdering.value = true
  
  // Перенаправляем на страницу оформления заказа для гостей
  setTimeout(() => {
    isOrdering.value = false
    router.push({
      path: '/checkout/guest',
      query: { 
        items: JSON.stringify(cartItems.value),
        total: totalPrice.value
      }
    })
  }, 500)
}

const removeItem = (itemId) => {
  cartItems.value = cartItems.value.filter(item => item.id !== itemId)
  checkedItems.value.delete(itemId)
  updateAllCheckedState()
}

const downloadItem = (itemId) => {
  const item = cartItems.value.find(item => item.id === itemId)
  if (item) {
    alert(`Скачивание товара: ${item.title}`)
  }
}

const updateItem = (updatedItem) => {
  const index = cartItems.value.findIndex(item => item.id === updatedItem.id)
  if (index !== -1) {
    cartItems.value[index] = updatedItem
  }
}

// Экспортируем функцию для использования в других компонентах
defineExpose({
  addToCart
})
</script>

<style scoped>
/* Существующие стили остаются без изменений */
.cart-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #F5F5F5;
}

.cart-canvas {
  flex: 1;
  display: flex;
  justify-content: center;
  min-height: calc(100vh - 164px);
  padding: 30px 20px; 
  box-sizing: border-box;
}

.cart-inner {
  width: 100%;
  max-width: 1200px; 
  display: flex;
  gap: 30px;
  padding: 0; 
  box-sizing: border-box;
  align-items: flex-start;
}

.cart-container {
  flex: 1;
  background: #FFFFFF;
  border-radius: 12px; 
  padding: 25px 30px;
  box-sizing: border-box;
  min-height: 100%;
  box-shadow: 0 2px 10px rgba(0,0,0,0.05);
}

.cart-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
}

.cart-title {
  font-size: 28px;
  margin: 0;
  font-weight: 600;
  font-family: 'Montserrat Alternates', sans-serif;
  color: #333;
}

.cart-count {
  font-size: 18px;
  color: #666;
  display: flex;
  align-items: center;
  gap: 4px;
}

.cart-items-number {
  font-weight: 600;
  color: #333;
}

.cart-items-label {
  color: #666;
}

/* Стили для уведомления гостя */
.guest-notice {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 10px;
  padding: 15px 20px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 15px;
  color: white;
  animation: fadeIn 0.5s ease;
  font-family: 'Montserrat', sans-serif; /* Изменено на Montserrat */
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.guest-icon {
  font-size: 32px;
  flex-shrink: 0;
}

.guest-text {
  flex: 1;
  font-size: 14px;
  line-height: 1.4;
}

.guest-text p {
  margin: 0 0 5px 0;
}

.guest-text p:last-child {
  margin-bottom: 0;
}

.guest-login-link,
.guest-register-link {
  color: #03c3e6; 
  text-decoration: none;
  font-weight: 600;
  transition: opacity 0.2s;
}

.guest-login-link:hover,
.guest-register-link:hover {
  opacity: 0.8;
  text-decoration: underline;
}

.cart-controls {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
}

.select-all-checkbox {
  transform: scale(1.1);
}

.cart-select-all {
  font-size: 16px;
  font-family: 'Montserrat Alternates', sans-serif;
  color: #333;
  font-weight: 500;
}

.cart-divider {
  height: 1px;
  background: #E0E0E0;
  margin-bottom: 10px;
}

.cart-body {
  margin-top: 10px;
}

.cart-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
  border-radius: 10px;
  background: #F8F8F8;
  margin-top: 20px;
}

.empty-icon {
  font-size: 60px;
  margin-bottom: 20px;
  opacity: 0.7;
}

.empty-title {
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 10px 0;
  color: #333;
  font-family: 'Montserrat Alternates', sans-serif;
}

.empty-text {
  font-size: 15px;
  color: #666;
  margin: 0 0 15px 0;
  max-width: 250px;
}

.empty-link {
  background: #A53DFF;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 10px 20px;
  font-size: 14px;
  text-decoration: none;
  font-family: 'Montserrat Alternates', sans-serif;
  transition: background 0.2s;
}

.empty-link:hover {
  background: #8C2BD9;
}

.cart-sidebar {
  width: 300px;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  position: sticky;
  top: 30px;
}

.summary-box {
  width: 300px;
  min-height: 240px;
  border: 2px solid #A53DFF;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  background: transparent;
  box-sizing: border-box;
  padding: 24px;
  font-family: 'Montserrat Alternates', sans-serif;
  box-shadow: 0 3px 15px rgba(165, 61, 255, 0.08);
}

.summary-title {
  font-size: 22px;
  font-weight: 600;
  color: #333;
}

.summary-price {
  font-size: 28px;
  color: #A53DFF;
  font-weight: 700;
  margin: 5px 0 10px 0;
}

.order-guest-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
}

.guest-order-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border: 0;
  border-radius: 8px;
  padding: 12px 40px;
  font-size: 16px;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
  font-weight: 600;
  text-align: center;
  transition: all 0.2s;
  margin: 5px 0;
  width: 100%;
  max-width: 200px;
}

.guest-order-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.guest-order-note {
  font-size: 12px;
  color: #666;
  text-align: center;
  margin: 8px 0 0 0;
  max-width: 200px;
}

.order-btn {
  background: #A53DFF;
  color: #fff;
  border: 0;
  border-radius: 8px;
  padding: 12px 40px;
  font-size: 16px;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
  font-weight: 600;
  text-align: center;
  transition: all 0.2s;
  margin: 5px 0;
  width: 100%;
  max-width: 200px;
}

.order-btn:hover:not(:disabled) {
  background: #8C2BD9;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(165, 61, 255, 0.2);
}

.order-btn:active:not(:disabled) {
  transform: translateY(0);
}

.order-btn:disabled,
.guest-order-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.summary-note {
  font-size: 14px;
  color: #666;
  margin-top: 5px;
}

@media (max-width: 1100px) {
  .cart-inner {
    gap: 25px;
  }
  
  .cart-sidebar {
    width: 280px;
  }
  
  .summary-box {
    width: 280px;
    min-height: 220px;
  }
}

@media (max-width: 975px) {
  .cart-canvas {
    padding: 20px 15px;
  }
  
  .cart-inner {
    flex-direction: column;
    gap: 20px;
    align-items: stretch;
  }
  
  .cart-sidebar {
    width: 100%;
    order: -1;
    justify-content: center;
    position: static;
  }
  
  .summary-box {
    width: 100%;
    max-width: 100%;
    min-height: 200px;
  }
  
  .cart-container {
    order: 0;
    width: 100%;
    padding: 20px;
  }
}

@media (max-width: 768px) {
  .cart-canvas {
    padding: 15px 10px;
  }
  
  .cart-inner {
    gap: 15px;
  }
  
  .cart-container {
    padding: 16px;
  }
  
  .cart-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .cart-title {
    font-size: 24px;
  }
  
  .cart-count {
    font-size: 16px;
  }
  
  .guest-notice {
    flex-direction: column;
    text-align: center;
    padding: 15px;
  }
  
  .guest-icon {
    font-size: 28px;
  }
  
  .summary-box {
    min-height: 180px;
    padding: 20px;
  }
  
  .summary-title {
    font-size: 20px;
  }
  
  .summary-price {
    font-size: 24px;
  }
  
  .order-btn,
  .guest-order-btn {
    padding: 10px 30px;
    font-size: 15px;
    max-width: 180px;
  }
}

@media (max-width: 480px) {
  .cart-canvas {
    padding: 12px 8px;
  }
  
  .cart-container {
    padding: 14px;
  }
  
  .cart-title {
    font-size: 22px;
  }
  
  .cart-select-all {
    font-size: 15px;
  }
  
  .summary-box {
    min-height: 170px;
    padding: 16px;
    border-radius: 10px;
  }
  
  .summary-title {
    font-size: 18px;
  }
  
  .summary-price {
    font-size: 22px;
  }
  
  .order-btn,
  .guest-order-btn {
    padding: 10px 25px;
  }
}
</style>