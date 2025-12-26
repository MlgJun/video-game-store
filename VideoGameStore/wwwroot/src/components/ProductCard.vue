<template>
  <div class="product-card">
    <div class="product-image-wrapper">
      <!-- Используем product.imageUrl, если есть, иначе product.image -->
      <img :src="product.imageUrl || product.image" :alt="product.name || product.title" class="product-image" />
    </div>
    
    <div class="product-info">
      <!-- Используем product.name, если есть, иначе product.title -->
      <h3 class="product-name">{{ product.name || product.title }}</h3>
      <div class="product-price">{{ formatPrice(product.price) }} Руб.</div>
    </div>

    <button 
      class="add-to-cart-btn" 
      aria-label="Добавить в корзину"
      @click="addToCart"
      :class="{ 'adding': isAdding, 'added': isAdded || isInCart }"
      :disabled="isAdding || isInCart || product.count === 0"
    >
      <span class="btn-text">{{ buttonText }}</span>
      <span class="btn-icon">✓</span>
    </button>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  product: {
    type: Object,
    required: true,
    default: () => ({
      id: null,
      name: '',        // поддерживаем name
      title: '',       // поддерживаем title
      price: 0,
      image: '',       // поддерживаем image
      imageUrl: '',    // поддерживаем imageUrl
      count: 0
    })
  }
})

const emits = defineEmits(['add-to-cart'])

const isAdding = ref(false)
const isAdded = ref(false)
const isInCart = ref(false)

// Проверяем, есть ли товар в гостевой корзине
const checkIfInCart = () => {
  try {
    const guestCart = JSON.parse(localStorage.getItem('guestCart') || '[]')
    isInCart.value = guestCart.some(item => item.id === props.product.id)
  } catch (e) {
    console.error('Ошибка проверки корзины:', e)
    isInCart.value = false
  }
}

// Форматирование цены
const formatPrice = (price) => {
  if (!price && price !== 0) return '0'
  const numPrice = typeof price === 'string' ? parseInt(price.replace(/\D/g, '')) || 0 : price
  return new Intl.NumberFormat('ru-RU').format(numPrice)
}

// Получаем имя товара (из name или title)
const productName = computed(() => {
  return props.product.name || props.product.title || 'Название товара'
})

// Получаем URL изображения (из imageUrl или image)
const productImage = computed(() => {
  return props.product.imageUrl || props.product.image || ''
})

const buttonText = computed(() => {
  if (isAdding.value) return 'Добавляется...'
  if (isAdded.value || isInCart.value) return 'В корзине'
  if (props.product.count === 0) return 'Нет в наличии'
  return 'В корзину'
})

const addToCart = () => {
  if (isAdding.value || isAdded.value || isInCart.value || props.product.count === 0) return
  
  isAdding.value = true
  
  // Сохраняем в гостевую корзину
  saveToGuestCart()
  
  // Имитация запроса на сервер
  setTimeout(() => {
    isAdding.value = false
    isAdded.value = true
    
    // Отправляем событие родителю (Home.vue)
    emits('add-to-cart', props.product.id)
    
    // Отправляем глобальное событие для обновления счетчика в Header
    window.dispatchEvent(new CustomEvent('cart-updated'))
    
    // Сброс состояния через 2 секунды
    setTimeout(() => {
      isAdded.value = false
      checkIfInCart() // Обновляем статус после сброса анимации
    }, 2000)
    
    console.log('Товар добавлен в корзину:', props.product)
  }, 800)
}

// Сохранение в гостевую корзину
const saveToGuestCart = () => {
  try {
    let guestCart = JSON.parse(localStorage.getItem('guestCart') || '[]')
    
    // Проверяем, есть ли уже такой товар
    const existingIndex = guestCart.findIndex(item => item.id === props.product.id)
    
    if (existingIndex !== -1) {
      // Увеличиваем количество
      guestCart[existingIndex].quantity += 1
    } else {
      // Добавляем новый товар
      guestCart.push({
        id: props.product.id,
        name: productName.value,
        title: productName.value,
        price: props.product.price,
        quantity: 1,
        image: productImage.value,
        imageUrl: productImage.value,
        platform: 'Steam ключ',
        link: `/product/${props.product.id}`
      })
    }
    
    localStorage.setItem('guestCart', JSON.stringify(guestCart))
    
  } catch (e) {
    console.error('Ошибка сохранения в корзину:', e)
  }
}

// Слушаем события обновления корзины
const handleCartUpdate = () => {
  checkIfInCart()
}

// При монтировании проверяем статус товара в корзине
onMounted(() => {
  checkIfInCart()
  
  // Слушаем события обновления корзины
  window.addEventListener('cart-updated', handleCartUpdate)
})

onUnmounted(() => {
  window.removeEventListener('cart-updated', handleCartUpdate)
})
</script>

<style scoped>
/* ВАШИ СТИЛИ ОСТАЮТСЯ БЕЗ ИЗМЕНЕНИЙ */
.product-card {
  width: 240px;
  height: 400px;
  background: #E2E2E2;
  border: 8px solid #CFCFCF;
  border-radius: 30px;
  display: flex;
  flex-direction: column;
  align-items: center;
  box-sizing: border-box;
  padding: 20px 12px 12px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.product-image-wrapper {
  width: 180px;
  height: 245px;
  background: #F5F5F5;
  border-radius: 20px;
  overflow: hidden;
  margin-bottom: 12px;
  transition: all 0.3s ease;
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  transition: transform 0.5s ease;
}

.product-card:hover .product-image {
  transform: scale(1.05);
}

.product-info {
  width: 100%;
  text-align: center;
  margin-bottom: 12px;
  transition: all 0.3s ease;
}

.product-name {
  font-size: 20px;
  margin: 0 0 6px;
  color: #A53DFF;
  font-weight: 600;
  font-family: 'Montserrat Alternates', sans-serif;
  line-height: 1.2;
  overflow-wrap: break-word;
  hyphens: auto;
  transition: font-size 0.3s ease;
}

.product-price {
  font-size: 20px;
  color: #A53DFF;
  font-weight: 600;
  font-family: 'Montserrat Alternates', sans-serif;
  transition: font-size 0.3s ease;
}

.add-to-cart-btn {
  width: 204px;
  height: 40px;
  background: #222222;
  color: #FFFFFF;
  border: none;
  border-radius: 30px;
  font-size: 18px;
  font-family: 'Montserrat Alternates', sans-serif;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  text-align: center;
  justify-content: center;
  gap: 8px;
  padding: 0 20px;
}

.add-to-cart-btn:hover:not(:disabled) {
  background: #111111;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.3);
  transform: translateY(-2px);
}

.add-to-cart-btn:active:not(:disabled) {
  transform: scale(0.98);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.add-to-cart-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.add-to-cart-btn.adding {
  background: #444;
  animation: pulse 1.5s ease-in-out infinite;
}

.add-to-cart-btn.added {
  background: #4CAF50;
  animation: successPop 0.5s ease;
}

@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.7; }
  100% { opacity: 1; }
}

@keyframes successPop {
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
}

.add-to-cart-btn::after {
  content: '';
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  pointer-events: none;
  background-image: radial-gradient(circle, #fff 10%, transparent 10.01%);
  background-repeat: no-repeat;
  background-position: 50%;
  transform: scale(10, 10);
  opacity: 0;
  transition: transform 0.5s, opacity 1s;
}

.add-to-cart-btn:active:not(:disabled)::after {
  transform: scale(0, 0);
  opacity: 0.3;
  transition: 0s;
}

.btn-icon {
  opacity: 0;
  transform: scale(0);
  transition: all 0.3s ease;
  font-size: 20px;
  line-height: 1;
}

.add-to-cart-btn.added .btn-icon {
  opacity: 1;
  transform: scale(1);
  animation: iconPop 0.4s ease;
}

@keyframes iconPop {
  0% { transform: scale(0); opacity: 0; }
  70% { transform: scale(1.2); }
  100% { transform: scale(1); opacity: 1; }
}

@media (max-width: 1600px) {
  .product-card {
    width: 230px;
    height: 390px;
  }
  
  .product-image-wrapper {
    width: 170px;
    height: 235px;
  }
  
  .add-to-cart-btn {
    width: 190px;
  }
}

@media (max-width: 1400px) {
  .product-card {
    width: 220px;
    height: 380px;
    border-width: 7px;
  }
  
  .product-image-wrapper {
    width: 165px;
    height: 225px;
  }
  
  .product-name,
  .product-price {
    font-size: 19px;
  }
  
  .add-to-cart-btn {
    width: 180px;
    height: 38px;
    font-size: 17px;
  }
}

@media (max-width: 1200px) {
  .product-card {
    width: 210px;
    height: 370px;
    border-width: 6px;
    border-radius: 28px;
    padding: 18px 10px 10px;
  }
  
  .product-image-wrapper {
    width: 160px;
    height: 215px;
    border-radius: 18px;
  }
  
  .product-name,
  .product-price {
    font-size: 18px;
  }
  
  .add-to-cart-btn {
    width: 170px;
    height: 36px;
    font-size: 16px;
  }
}

@media (max-width: 1024px) {
  .product-card {
    width: 200px;
    height: 360px;
  }
  
  .product-image-wrapper {
    width: 155px;
    height: 205px;
  }
  
  .product-name,
  .product-price {
    font-size: 17px;
  }
  
  .add-to-cart-btn {
    width: 160px;
    height: 34px;
    font-size: 15px;
  }
}

@media (max-width: 900px) {
  .product-card {
    width: 190px;
    height: 340px;
    border-width: 5px;
    border-radius: 26px;
  }
  
  .product-image-wrapper {
    width: 145px;
    height: 190px;
    border-radius: 16px;
  }
  
  .product-name,
  .product-price {
    font-size: 16px;
  }
  
  .add-to-cart-btn {
    width: 150px;
    height: 32px;
    font-size: 14px;
  }
}

@media (max-width: 768px) {
  .product-card {
    width: 180px;
    height: 320px;
    padding: 15px 8px 8px;
  }
  
  .product-image-wrapper {
    width: 140px;
    height: 180px;
    margin-bottom: 10px;
  }
  
  .product-info {
    margin-bottom: 10px;
  }
  
  .product-name,
  .product-price {
    font-size: 15px;
  }
  
  .add-to-cart-btn {
    width: 140px;
    height: 30px;
    font-size: 13px;
    gap: 6px;
    padding: 0 16px;
  }
  
  .btn-icon {
    font-size: 16px;
  }
}

@media (max-width: 600px) {
  .product-card {
    width: 170px;
    height: 310px;
    border-radius: 24px;
  }
  
  .product-image-wrapper {
    width: 130px;
    height: 170px;
    border-radius: 14px;
  }
  
  .product-name,
  .product-price {
    font-size: 14px;
  }
  
  .add-to-cart-btn {
    width: 130px;
    height: 28px;
    font-size: 12px;
  }
  
  .btn-icon {
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .product-card {
    width: 160px;
    height: 300px;
    border-width: 4px;
    border-radius: 22px;
  }
  
  .product-image-wrapper {
    width: 120px;
    height: 160px;
    border-radius: 12px;
  }
  
  .product-name {
    font-size: 13px;
    margin-bottom: 4px;
  }
  
  .product-price {
    font-size: 13px;
  }
  
  .add-to-cart-btn {
    width: 120px;
    height: 26px;
    font-size: 11px;
    gap: 4px;
    padding: 0 12px;
  }
}

@media (max-width: 360px) {
  .product-card {
    width: 150px;
    height: 290px;
    border-radius: 20px;
  }
  
  .product-image-wrapper {
    width: 110px;
    height: 150px;
  }
  
  .product-name,
  .product-price {
    font-size: 12px;
  }
  
  .add-to-cart-btn {
    width: 110px;
    height: 24px;
    font-size: 10px;
  }
}
</style>