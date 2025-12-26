<template>
  <div v-if="visible" class="guest-menu-overlay" :style="overlayStyle" @click.self="close">
    <div class="guest-menu" :style="menuStyle" role="dialog" aria-label="Меню продавца">
      <router-link to="/seller/dashboard" class="menu-button" @click="close">
        <img src="/src/assets/icons/dashboard.svg" alt="dashboard" class="menu-icon" />
        <span>Панель продавца</span>
      </router-link>
      <router-link to="/seller/products" class="menu-button" @click="close">
        <img src="/src/assets/icons/product.svg" alt="products" class="menu-icon" />
        <span>Товары</span>
      </router-link>
      <router-link to="/seller/orders" class="menu-button" @click="close">
        <img src="/src/assets/icons/history.svg" alt="orders" class="menu-icon" />
        <span>Заказы</span>
      </router-link>
      <router-link to="/seller/logout" class="menu-button" @click="close">
        <img src="/src/assets/icons/logout.svg" alt="logout" class="menu-icon" />
        <span>Выйти</span>
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits, watch, ref, nextTick } from 'vue'

const props = defineProps({
  visible: { type: Boolean, default: false }
})
const emits = defineEmits(['update:visible', 'close'])

const menuStyle = ref({})
const overlayStyle = ref({})
const ignoreClicksUntil = ref(0)

function close(){
  const now = Date.now()
  if(now < ignoreClicksUntil.value) return
  emits('update:visible', false)
  emits('close')
}

function positionMenu(){
  const menuHeight = 240
  const viewportPad = 40
  let menuWidth = 400
  if(window.innerWidth - viewportPad < menuWidth) menuWidth = Math.max(280, window.innerWidth - viewportPad)
  const cartBtn = document.querySelector('.site-header .header-inner .cart-btn')
  const userBtn = document.querySelector('.site-header .header-inner .user-btn')
  const headerEl = document.querySelector('.site-header')
  if(cartBtn && userBtn && headerEl){
    const r1 = cartBtn.getBoundingClientRect()
    const r2 = userBtn.getBoundingClientRect()
    const leftEdge = Math.min(r1.left, r2.left)
    const rightEdge = Math.max(r1.right, r2.right)
    const center = (leftEdge + rightEdge) / 2
    let left = center - menuWidth / 2
    if(left + menuWidth > window.innerWidth) left = window.innerWidth - menuWidth
    if(left < 0) left = 0
    const headerRect = headerEl.getBoundingClientRect()
    const overlayTop = headerRect.bottom
    overlayStyle.value = { position: 'fixed', left: 0, right: 0, top: overlayTop + 'px', pointerEvents: 'auto', zIndex: 1100 }
    menuStyle.value = { position: 'absolute', width: menuWidth + 'px', height: menuHeight + 'px', top: '0px', left: left + 'px', zIndex: 1101 }
    return
  }
  const anchor = document.querySelector('.site-header .header-inner .header-right') || document.querySelector('.site-header .header-inner')
  if(!anchor){
    const left = Math.max((window.innerWidth - menuWidth)/2, 0)
    const top = 164
    overlayStyle.value = { position: 'fixed', left: 0, right: 0, top: top + 'px', pointerEvents: 'auto', zIndex: 900 }
    menuStyle.value = { position: 'absolute', width: menuWidth + 'px', height: menuHeight + 'px', top: 0 + 'px', left: left + 'px' }
    return
  }
  const rect = anchor.getBoundingClientRect()
  const headerEl2 = document.querySelector('.site-header')
  const headerRect2 = headerEl2 ? headerEl2.getBoundingClientRect() : rect
  const overlayTop = headerRect2.bottom
  let left = rect.left + (rect.width - menuWidth)/2
  if(left + menuWidth > window.innerWidth) left = window.innerWidth - menuWidth
  if(left < 0) left = 0
  overlayStyle.value = { position: 'fixed', left: 0, right: 0, top: overlayTop + 'px', pointerEvents: 'auto', zIndex: 1100 }
  menuStyle.value = { position: 'absolute', width: menuWidth + 'px', height: menuHeight + 'px', top: '0px', left: left + 'px', zIndex: 1101 }
}

watch(()=>props.visible, async (v)=>{
  if(v){
    await nextTick()
    positionMenu()
    ignoreClicksUntil.value = Date.now() + 200
    window.addEventListener('resize', positionMenu)
  } else {
    window.removeEventListener('resize', positionMenu)
    overlayStyle.value = {}
    menuStyle.value = {}
  }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');
.guest-menu-overlay{position:fixed;left:0;right:0;display:block;pointer-events:auto}
.guest-menu{position:absolute;top:0;width:400px;height:240px;background:rgba(34,34,34,0.95);border-top-left-radius:0;border-top-right-radius:0;border-bottom-left-radius:30px;border-bottom-right-radius:30px;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:12px;box-sizing:border-box;padding:20px;font-family:'Montserrat Alternates',sans-serif;z-index:900;transform-origin:top center;animation:menuEnter .22s cubic-bezier(.2,.9,.2,1) both}
.menu-button{width:360px;height:50px;background-color:rgba(39,39,39,0.95);border-radius:30px;border:none;display:flex;align-items:center;justify-content:center;gap:12px;padding:0 12px;box-sizing:border-box;color:#FFFFFF;font-family:'Montserrat Alternates',sans-serif;font-size:18px;text-decoration:none;position:relative;padding-left:72px;transition:transform .12s ease,box-shadow .12s ease,background-color .12s ease}
.menu-button.router-link-active{outline: none}
.menu-button:hover{transform:translateY(-3px)}
.menu-button:active{transform:translateY(0) scale(.985);box-shadow:inset 0 3px 8px rgba(0,0,0,0.35)}
.menu-icon{width:28px;height:30px;filter:brightness(0) invert(1);position:absolute;left:16px;top:50%;transform:translateY(-50%)}
.menu-button span{line-height:1;display:block;position:absolute;left:0;right:0;text-align:center}

@keyframes menuEnter{
  from{opacity:0;transform:translateY(-8px) scale(.985)}
  to{opacity:1;transform:translateY(0) scale(1)}
}

@media (prefers-reduced-motion: reduce){
  .guest-menu{animation:none !important;transform:none !important}
  .menu-button{transition:none !important}
}
</style>