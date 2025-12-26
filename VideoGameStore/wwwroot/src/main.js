import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'

// Статические импорты
import Home from './views/Home.vue'
import Register from './views/Register.vue'
import Login from './views/Login.vue'
import Cart from './views/Cart.vue'
import PurchaseHistory from './views/BuyHistory.vue'
import SellerPage from './views/SellerPage.vue'
import NotFound from './views/NotFound.vue'

// Создаем Pinia до роутера
const pinia = createPinia()

// Импортируем store для использования в хуках
import { useAuthStore } from './stores/auth.js'

const routes = [
  { path: '/', component: Home, name: 'Home' },
  { path: '/register', component: Register, name: 'Register' },
  { path: '/login', component: Login, name: 'Login' },
  { 
    path: '/cart', 
    component: Cart, 
    name: 'Cart', 
    meta: { requiresAuth: false } // ИЗМЕНЕНИЕ ЗДЕСЬ: убираем requiresAuth
  },
  { 
    path: '/history', 
    component: PurchaseHistory, 
    name: 'BuyHistory', 
    meta: { requiresAuth: true, requiresCustomer: true } 
  },
  { 
    path: '/sellerpage', 
    component: SellerPage, 
    name: 'SellerPage', 
    meta: { requiresAuth: true, requiresSeller: true } 
  },
  { path: '/seller', redirect: '/sellerpage' },
  
  // ВАЖНО: исправленный путь
  {
    path: '/seller/games/new',
    name: 'GameCreate',
    component: () => import('./components/SellerProductEdit.vue'), 
    meta: { requiresAuth: true, requiresSeller: true }
  },
  {
    path: '/seller/games/edit/:id',
    name: 'GameEdit',
    component: () => import('./components/SellerProductEdit.vue'),
    props: true,
    meta: { requiresAuth: true, requiresSeller: true }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFound
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Настройка хука авторизации
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore(pinia)
  
  // Для корзины пропускаем проверку авторизации
  if (to.name === 'Cart') {
    next()
    return
  }
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  if (to.meta.requiresSeller && authStore.userRole !== 'SELLER') {
    next('/')
    return
  }
  
  if (to.meta.requiresCustomer && authStore.userRole !== 'CUSTOMER') {
    next('/')
    return
  }
  
  next()
})

// Создаем приложение
const app = createApp(App)
app.use(pinia)
app.use(router)
app.mount('#app')

console.log('App initialized')