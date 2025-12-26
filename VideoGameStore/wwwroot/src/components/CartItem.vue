<template>
  <div class="cart-item">
    <CheckBox v-model="checked" class="cart-item-checkbox" />
    
    <div class="cart-item-image">
      <img :src="item.image" :alt="item.title" />
    </div>
    
    <div class="cart-item-content">
      <div class="cart-item-info">
        <h3 class="cart-item-title">{{ item.title }}</h3>
        <div class="cart-item-platform">{{ item.platform }}</div>
      </div>
      
      <div class="cart-item-controls">
        <div class="quantity-control">
          <button 
            class="quantity-btn minus"
            @click="decreaseQuantity"
            :disabled="item.quantity <= 1"
          >
            −
          </button>
          <span class="quantity-value">{{ item.quantity }}</span>
          <button 
            class="quantity-btn plus"
            @click="increaseQuantity"
          >
            +
          </button>
        </div>
        
        <div class="cart-item-price">{{ item.price * item.quantity }} ₽</div>
      </div>
    </div>
    
    <div class="cart-item-actions">
      <router-link 
        :to="item.link || '/product/' + item.id" 
        class="action-btn details-btn"
        title="Полное описание"
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M12 16V12M12 8H12.01M21 12C21 13.1819 20.7672 14.3522 20.3149 15.4442C19.8626 16.5361 19.1997 17.5282 18.364 18.364C17.5282 19.1997 16.5361 19.8626 15.4442 20.3149C14.3522 20.7672 13.1819 21 12 21C10.8181 21 9.64778 20.7672 8.55585 20.3149C7.46392 19.8626 6.47177 19.1997 5.63604 18.364C4.80031 17.5282 4.13738 16.5361 3.68508 15.4442C3.23279 14.3522 3 13.1819 3 12C3 9.61305 3.94821 7.32387 5.63604 5.63604C7.32387 3.94821 9.61305 3 12 3C14.3869 3 16.6761 3.94821 18.364 5.63604C20.0518 7.32387 21 9.61305 21 12Z" 
                stroke="currentColor" 
                stroke-width="2" 
                stroke-linecap="round" 
                stroke-linejoin="round"/>
        </svg>
      </router-link>
      
      <button class="action-btn delete-btn" @click="handleDelete" title="Удалить">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M4 7H20M10 11V17M14 11V17M5 7L6 19C6 19.5304 6.21071 20.0391 6.58579 20.4142C6.96086 20.7893 7.46957 21 8 21H16C16.5304 21 17.0391 20.7893 17.4142 20.4142C17.7893 20.0391 18 19.5304 18 19L19 7M9 7V4C9 3.73478 9.10536 3.48043 9.29289 3.29289C9.48043 3.10536 9.73478 3 10 3H14C14.2652 3 14.5196 3.10536 14.7071 3.29289C14.8946 3.48043 15 3.73478 15 4V7" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import CheckBox from './CheckBox.vue'

const router = useRouter()

const props = defineProps({
  item: {
    type: Object,
    required: true,
    default: () => ({
      id: 1,
      title: 'Plants vs. Zombies',
      platform: 'Steam ключ',
      price: 1800,
      quantity: 1,
      image: 'https://via.placeholder.com/105x135',
      link: '/product/1'
    })
  }
})

const emit = defineEmits(['update:item', 'delete', 'details'])

const checked = ref(false)

const itemModel = computed({
  get: () => props.item,
  set: (value) => emit('update:item', value)
})

const increaseQuantity = () => {
  itemModel.value = {
    ...itemModel.value,
    quantity: itemModel.value.quantity + 1
  }
}

const decreaseQuantity = () => {
  if (itemModel.value.quantity > 1) {
    itemModel.value = {
      ...itemModel.value,
      quantity: itemModel.value.quantity - 1
    }
  }
}

const handleDelete = () => {
  emit('delete', itemModel.value.id)
}

const handleDetails = (event) => {
  // Если нужно дополнительное действие при клике
  emit('details', itemModel.value.id)
}
</script>

<style scoped>
.cart-item {
  display: flex;
  align-items: center;
  padding: 15px 0;
  border-bottom: 1px solid #E0E0E0;
  gap: 15px;
}

.cart-item:last-child {
  border-bottom: none;
}

.cart-item-checkbox {
  flex-shrink: 0;
}

.cart-item-image {
  width: 105px;
  height: 135px;
  flex-shrink: 0;
  border-radius: 8px;
  overflow: hidden;
  background: #F5F5F5;
  border: 1px solid #E0E0E0;
}

.cart-item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.cart-item-content {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  min-width: 0;
}

.cart-item-info {
  flex: 1;
  min-width: 0;
  margin-right: 20px;
}

.cart-item-title {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 5px 0;
  color: #333;
  font-family: 'Montserrat Alternates', sans-serif;
  line-height: 1.3;
}

.cart-item-platform {
  font-size: 14px;
  color: #666;
  margin: 0;
}

.cart-item-controls {
  display: flex;
  align-items: center;
  gap: 25px;
  flex-shrink: 0;
}

.quantity-control {
  display: flex;
  align-items: center;
  gap: 10px;
}

.quantity-btn {
  width: 30px;
  height: 30px;
  border: 1px solid #A53DFF;
  background: #FFFFFF;
  border-radius: 4px;
  color: #A53DFF;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  line-height: 1;
  padding: 0;
}

.quantity-btn:hover:not(:disabled) {
  background: #A53DFF;
  color: white;
}

.quantity-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
  border-color: #999;
  color: #999;
}

.quantity-value {
  font-size: 16px;
  font-weight: 500;
  min-width: 24px;
  text-align: center;
  font-family: 'Montserrat Alternates', sans-serif;
}

.cart-item-price {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  font-family: 'Montserrat Alternates', sans-serif;
  min-width: 100px;
  text-align: right;
}

.cart-item-actions {
  display: flex;
  gap: 10px;
  flex-shrink: 0;
}

.action-btn {
  width: 40px;
  height: 40px;
  border: none;
  background: #F5F5F5;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  color: #666;
  text-decoration: none;
}

.action-btn:hover {
  background: #E0E0E0;
  color: #333;
}

.details-btn:hover {
  background: #E8F5E9;
  color: #4CAF50;
}

.delete-btn:hover {
  background: #FFEBEE;
  color: #F44336;
}

@media (max-width: 900px) {
  .cart-item {
    flex-wrap: wrap;
    gap: 12px;
  }
  
  .cart-item-image {
    width: 90px;
    height: 115px;
    order: 2;
    margin-left: 30px;
  }
  
  .cart-item-content {
    order: 3;
    width: 100%;
    margin-top: 10px;
  }
  
  .cart-item-actions {
    order: 1;
    margin-left: auto;
  }
  
  .cart-item-checkbox {
    order: 0;
  }
}

@media (max-width: 768px) {
  .cart-item {
    padding: 12px 0;
  }
  
  .cart-item-image {
    width: 80px;
    height: 105px;
  }
  
  .cart-item-controls {
    flex-direction: column;
    gap: 15px;
    align-items: flex-start;
  }
  
  .cart-item-price {
    text-align: left;
  }
}

@media (max-width: 480px) {
  .cart-item {
    gap: 10px;
  }
  
  .cart-item-image {
    width: 70px;
    height: 90px;
    margin-left: 25px;
  }
  
  .cart-item-title {
    font-size: 15px;
  }
  
  .cart-item-platform {
    font-size: 13px;
  }
  
  .cart-item-price {
    font-size: 16px;
  }
  
  .action-btn {
    width: 36px;
    height: 36px;
  }
}
</style>