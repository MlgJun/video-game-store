<!-- src/components/ProductDetails.vue -->
<template>
  <transition name="modal">
    <div v-if="isVisible" class="modal-overlay" @click.self="emitClose">
      <div class="modal-content">

        <button class="modal-close" @click="emitClose">×</button>
        
        <div v-if="loading" class="loading">
          Загрузка игры...
        </div>
        
        <div v-else-if="error" class="error">
          {{ error }}
        </div>
        
        <div v-else-if="game" class="product-details-content">

          <div class="left-section">
            <div class="vertical-photo-wrapper">
              <img :src="game.imageUrl" :alt="game.title" class="vertical-photo" />
            </div>
            <p class="photo-label">Изображение товара</p>
            
            <div class="stock-info" v-if="game.count !== undefined">
              <span class="stock-label">В наличии:</span>
              <span class="stock-count" :class="{ 'low-stock': game.count < 5 }">
                {{ game.count }} шт.
              </span>
            </div>
          </div>

          <div class="center-section">
            <div class="product-image-wrapper">
              <img :src="game.imageUrl" :alt="game.title" class="product-image" />
            </div>
            
            <div class="product-description-box">
              <p class="product-description-label">Описание:</p>
              <p class="product-description-text">{{ game.description || 'Нет описания' }}</p>
            </div>

            <div class="price-section">
              <span class="price">{{ formatPrice(game.price) }} ₽</span>
            </div>
          </div>

          <div class="right-section">
            <h1 class="product-title">{{ game.title }}</h1>

            <div class="seller-info">
              <img src="/src/assets/icons/saleman.svg" alt="Продавец" class="seller-icon" />
              <span class="seller-name">{{ game.seller?.username || 'Продавец' }}</span>
            </div>

            <div class="specs">
              <div class="spec-row">
                <span class="spec-label">Разработчик:</span>
                <span class="spec-value">{{ game.developerTitle }}</span>
              </div>
              <div class="spec-row">
                <span class="spec-label">Издатель:</span>
                <span class="spec-value">{{ game.publisherTitle }}</span>
              </div>
              <div class="spec-row">
                <span class="spec-label">Дата добавления:</span>
                <span class="spec-value">{{ formatDate(game.createdAt) }}</span>
              </div>
              <div class="spec-row">
                <span class="spec-label">Жанры:</span>
                <span class="spec-value">
                  <span v-for="genre in game.genres" :key="genre.title" class="genre-tag">
                    {{ genre.title }}
                  </span>
                </span>
              </div>
            </div>

            <div class="buttons-section">
              <button 
                class="btn btn-primary" 
                @click="addToCart"
                :disabled="game.count === 0 || cartLoading"
              >
                <span v-if="cartLoading">Добавление...</span>
                <span v-else>{{ game.count > 0 ? 'В корзину' : 'Нет в наличии' }}</span>
              </button>
              
              <button 
                class="btn btn-success" 
                @click="buyNow"
                :disabled="game.count === 0"
              >
                Купить сейчас
              </button>
            </div>
          </div>
        </div>
        
        <!-- Игра не найдена -->
        <div v-else class="not-found">
          Игра не найдена
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '/src/stores/auth.js'
import { useRouter } from 'vue-router'

const props = defineProps({
  gameId: {
    type: Number,
    required: true
  },
  isVisible: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'add-to-cart', 'buy-now'])

const router = useRouter()
const authStore = useAuthStore?.()

// Состояние
const game = ref(null)
const loading = ref(false)
const error = ref('')
const cartLoading = ref(false)

// Форматирование цены
const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price)
}

// Форматирование даты
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('ru-RU')
}

// Загрузка игры
const loadGame = async () => {
  if (!props.gameId) return
  
  try {
    loading.value = true
    error.value = ''
    
    const response = await fetch(`/api/games/${props.gameId}`)
    
    if (!response.ok) { 
      if (response.status === 404) {
        throw new Error('Игра не найдена')
      }
      throw new Error('Ошибка загрузки')
    }
    
    game.value = await response.json()
    
  } catch (err) {
    error.value = err.message
    console.error('Ошибка загрузки игры:', err)
  } finally {
    loading.value = false
  }
}

// Добавление в корзину
const addToCart = async () => {
  if (!authStore?.isAuthenticated) {
    router.push('/login')
    emit('close')
    return
  }
  
  if (!game.value || game.value.count === 0) return
  
  cartLoading.value = true
  
  try {
    const response = await fetch('/api/carts/items', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        gameId: game.value.id,
        quantity: 1
      }),
      credentials: 'include'
    })
    
    if (response.ok) {
      emit('add-to-cart', game.value.id)
      alert('Товар добавлен в корзину!')
      
      // Обновляем количество
      if (game.value.count > 0) {
        game.value.count -= 1
      }
    } else {
      const errorData = await response.json()
      alert(errorData.message || 'Ошибка добавления в корзину')
    }
  } catch (err) {
    alert('Ошибка сети')
  } finally {
    cartLoading.value = false
  }
}

// Покупка сейчас
const buyNow = async () => {
  if (!authStore?.isAuthenticated) {
    router.push('/login')
    emit('close')
    return
  }
  
  if (!game.value || game.value.count === 0) return
  
  try {
    const response = await fetch('/api/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        orderItems: [{
          gameId: game.value.id,
          quantity: 1
        }]
      }),
      credentials: 'include'
    })
    
    if (response.ok) {
      const order = await response.json()
      emit('buy-now', order)
      emit('close')
      router.push(`/order/${order.id}`)
    } else {
      const errorData = await response.json()
      alert(errorData.message || 'Ошибка оформления заказа')
    }
  } catch (err) {
    alert('Ошибка сети')
  }
}

// Закрытие
const emitClose = () => {
  emit('close')
}

// Закрытие по Escape
const handleEscape = (e) => {
  if (e.key === 'Escape' && props.isVisible) {
    emitClose()
  }
}

// При монтировании
onMounted(() => {
  document.addEventListener('keydown', handleEscape)
  if (props.isVisible) {
    document.body.style.overflow = 'hidden'
  }
})

// При размонтировании
onUnmounted(() => {
  document.removeEventListener('keydown', handleEscape)
  document.body.style.overflow = ''
})

// При изменении видимости
watch(() => props.isVisible, (visible) => {
  if (visible) {
    document.body.style.overflow = 'hidden'
    loadGame()
  } else {
    document.body.style.overflow = ''
  }
})

// При изменении gameId
watch(() => props.gameId, () => {
  if (props.isVisible) {
    loadGame()
  }
})
</script>

<style scoped>
/* Модальные стили */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  position: relative;
  background: #EFEFEF;
  border: 8px solid #A53DFF;
  border-radius: 30px;
  padding: 30px;
  max-width: 1400px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-sizing: border-box;
  font-family: 'Montserrat Alternates', sans-serif;
}

.modal-close {
  position: absolute;
  top: 20px;
  right: 20px;
  width: 40px;
  height: 40px;
  background: rgba(0, 0, 0, 0.5);
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 24px;
  cursor: pointer;
  z-index: 100;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s;
}

.modal-close:hover {
  background: rgba(0, 0, 0, 0.8);
}

/* Анимация */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

/* Содержимое */
.product-details-content {
  display: flex;
  gap: 30px;
}

/* Ваши существующие стили из компонента ProductDetails остаются, но адаптируем: */
.left-section {
  flex: 0 0 280px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  align-items: flex-start;
}

.vertical-photo-wrapper {
  width: 280px;
  height: 305px;
  background: #E6E6E6;
  border-radius: 30px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.vertical-photo {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.center-section {
  flex: 0 0 612px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.product-image-wrapper {
  width: 612px;
  height: 428px;
  background: #E6E6E6;
  border-radius: 30px;
  overflow: hidden;
  position: relative;
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.product-description-box {
  width: 612px;
  height: 115px;
  background: #E6E6E6;
  border-radius: 20px;
  padding: 16px;
  box-sizing: border-box;
  overflow: hidden;
}

.product-description-label {
  margin: 0 0 8px 0;
  font-size: 20px;
  color: #666;
  font-family: 'Montserrat Alternates', sans-serif;
}

.product-description-text {
  margin: 0;
  font-size: 16px;
  color: #333;
  line-height: 1.6;
  max-height: 80px;
  overflow-y: auto;
  padding-right: 5px;
}

.price-section {
  margin-top: 10px;
  background: #E6E6E6;
  padding: 20px;
  border-radius: 30px;
  width: fit-content;
}

.price {
  font-size: 64px;
  color: #A53DFF;
  font-weight: 700;
  font-family: 'Montserrat Alternates', sans-serif;
}

.right-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.product-title {
  margin: 0;
  font-size: 36px;
  background: #E6E6E6;
  padding: 20px;
  border-radius: 30px;
  color: #111;
  font-weight: 600;
  line-height: 1.3;
  word-break: break-word;
  text-align: center;
}

.seller-info {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 12px 0;
  margin-top: -15px;
}

.seller-icon {
  width: 21px;
  height: 21px;
  flex-shrink: 0;
}

.seller-name {
  font-size: 24px;
  color: #454545;
  font-weight: 500;
}

.specs {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 20px;
  background: #E6E6E6;
  border-radius: 30px;
}

.spec-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;
  font-size: 24px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.spec-label {
  color: #666;
  flex-shrink: 0;
  font-weight: 500;
}

.spec-value {
  display: block;
  color: #111;
  text-align: right;
  max-width: 200px;
  word-break: break-word;
}

.genre-tag {
  background: #A53DFF;
  color: white;
  padding: 4px 12px;
  border-radius: 15px;
  font-size: 14px;
  margin: 0 4px 4px 0;
  display: inline-block;
}

.stock-info {
  background: #E6E6E6;
  padding: 12px 20px;
  border-radius: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 10px;
  width: 100%;
}

.stock-label {
  font-size: 16px;
  color: #666;
}

.stock-count {
  font-size: 18px;
  font-weight: 600;
  color: #52C41A;
}

.stock-count.low-stock {
  color: #F5222D;
}

.buttons-section {
  display: flex;
  flex-direction: column;
  gap: 16px;
  align-items: center;
  margin-top: auto;
}

.btn {
  width: 435px;
  height: 80px;
  border: none;
  border-radius: 30px;
  font-size: 32px;
  font-family: 'Montserrat Alternates', sans-serif;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background: #A53DFF;
  color: #FFFFFF;
  box-shadow: 0 8px 20px rgba(165, 61, 255, 0.25);
}

.btn-primary:hover:not(:disabled) {
  background: #8B1FD9;
  box-shadow: 0 10px 28px rgba(165, 61, 255, 0.35);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-success {
  background: #52C41A;
  color: #FFFFFF;
  box-shadow: 0 8px 20px rgba(82, 196, 26, 0.25);
}

.btn-success:hover:not(:disabled) {
  background: #45A517;
  box-shadow: 0 10px 28px rgba(82, 196, 26, 0.35);
}

.btn-success:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Сообщения */
.loading, .error, .not-found {
  text-align: center;
  padding: 60px 20px;
  font-size: 24px;
  color: #666;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
}

/* Адаптивность */
@media (max-width: 1400px) {
  .modal-content {
    max-width: 95%;
  }
  
  .product-details-content {
    flex-direction: column;
  }
  
  .left-section,
  .center-section,
  .right-section {
    width: 100%;
  }
  
  .vertical-photo-wrapper,
  .product-image-wrapper {
    width: 100%;
    height: auto;
    aspect-ratio: 4/3;
  }
  
  .product-description-box {
    width: 100%;
    height: auto;
    min-height: 120px;
  }
  
  .btn {
    width: 100%;
    max-width: 400px;
  }
}

@media (max-width: 768px) {
  .modal-content {
    padding: 20px;
  }
  
  .spec-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .spec-value {
    text-align: left;
    max-width: 100%;
  }
}
</style>