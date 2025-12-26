import { ref, computed } from 'vue'

export const useCartStore = () => {
  const cart = ref([])
  
  const addToCart = (product) => {
    // Логика добавления
  }
  
  const removeFromCart = (productId) => {
    // Логика удаления
  }
  
  const clearCart = () => {
    cart.value = []
  }
  
  return {
    cart: computed(() => cart.value),
    addToCart,
    removeFromCart,
    clearCart
  }
}