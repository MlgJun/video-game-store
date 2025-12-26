import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { auth, cart, games, genres as genresApi, orders } from '../services/api'

export const useAppStore = defineStore('app', () => {
  // User state
  const user = ref(null)
  const userRole = ref(null)
  const isAuthenticated = computed(() => !!user.value)

  // Cart state
  const cartItems = ref([])
  const cartCount = computed(() => 
    cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
  )
  const cartTotal = computed(() =>
    cartItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
  )

  // Games state
  const games = ref([])
  const currentGame = ref(null)
  const sellerGames = ref([])
  const genres = ref([])

  // Pagination
  const pagination = ref({
    page: 1,
    pageSize: 20,
    totalPages: 0,
    totalElements: 0
  })

  // Auth methods
  const register = async (email, username, password, role) => {
    try {
      const response = await auth.register(email, username, password, role)
      if (response && !response.statusCode) {
        user.value = response
        userRole.value = role
        return { success: true, data: response }
      }
      return { success: false, error: response?.message || 'Ошибка регистрации' }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const login = async (email, username, password, role) => {
    try {
      const response = await auth.login(email, username, password, role)
      if (response && !response.statusCode) {
        user.value = response
        userRole.value = role
        return { success: true, data: response }
      }
      return { success: false, error: response?.message || 'Ошибка входа' }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const logout = async () => {
    try {
      await auth.logout()
      user.value = null
      userRole.value = null
      cartItems.value = []
      return { success: true }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  // Cart methods
  const loadCart = async () => {
    try {
      const response = await cart.getCart()
      if (response && !response.statusCode) {
        cartItems.value = response.cartItems || []
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const addToCart = async (gameId, quantity = 1) => {
    try {
      const response = await cart.addItem(gameId, quantity)
      if (response && !response.statusCode) {
        cartItems.value = response.cartItems || []
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const removeFromCart = async (gameId, quantity = 1) => {
    try {
      const response = await cart.removeItem(gameId, quantity)
      if (response && !response.statusCode) {
        cartItems.value = response.cartItems || []
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  // Games methods
  const loadGames = async (filters = {}, page = 1) => {
    try {
      const response = await games.getAll(
        filters.minPrice,
        filters.maxPrice,
        filters.gameTitle,
        filters.genres,
        page,
        pagination.value.pageSize
      )
      if (response && !response.statusCode) {
        games.value = response.content || []
        pagination.value = {
          page: response.pageNumber,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
          totalElements: response.totalElements
        }
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const loadSellerGames = async (page = 1) => {
    try {
      const response = await games.getMySeller(page, pagination.value.pageSize)
      if (response && !response.statusCode) {
        sellerGames.value = response.content || []
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const createGame = async (formData) => {
    try {
      const response = await games.create(formData)
      if (response && !response.statusCode) {
        return { success: true, data: response }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const updateGame = async (id, formData) => {
    try {
      const response = await games.update(id, formData)
      if (response && !response.statusCode) {
        return { success: true, data: response }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const deleteGame = async (id) => {
    try {
      const response = await games.delete(id)
      if (!response.statusCode || response.statusCode === 200) {
        sellerGames.value = sellerGames.value.filter(g => g.id !== id)
        return { success: true }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  // Orders methods
  const loadOrders = async (page = 1) => {
    try {
      const response = await orders.getOrders(page, pagination.value.pageSize)
      if (response && !response.statusCode) {
        return { success: true, data: response }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  const createOrder = async (orderItems) => {
    try {
      const response = await orders.createOrder(orderItems)
      if (response && !response.statusCode) {
        cartItems.value = []
        return { success: true, data: response }
      }
      return { success: false, error: response?.message }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  // Genres methods
  const loadGenres = async () => {
    try {
      const response = await genresApi.getAll()
      if (Array.isArray(response)) {
        genres.value = response
        return { success: true }
      }
      return { success: false, error: 'Ошибка загрузки жанров' }
    } catch (error) {
      return { success: false, error: error.message }
    }
  }

  return {
    // User
    user,
    userRole,
    isAuthenticated,
    register,
    login,
    logout,

    // Cart
    cartItems,
    cartCount,
    cartTotal,
    loadCart,
    addToCart,
    removeFromCart,

    // Games
    games,
    currentGame,
    sellerGames,
    genres,
    loadGames,
    loadSellerGames,
    createGame,
    updateGame,
    deleteGame,

    // Orders
    loadOrders,
    createOrder,

    // Genres
    loadGenres,

    // Pagination
    pagination
  }
})
