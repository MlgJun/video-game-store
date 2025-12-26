<template>
  <button class="scroll-top" @click="scrollToTop" aria-label="Наверх">
    <svg class="scroll-icon" viewBox="110 170 28 28" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
      <path d="M124 198V170M110 182.25L124 170L138 182.25" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>
  </button>
</template>

<script setup>
const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
</script>

<style scoped>
.scroll-top{position:fixed;right:24px;bottom:24px;width:50px;height:50px;border-radius:50%;background:#181818;border:none;display:flex;align-items:center;justify-content:center;cursor:pointer;padding:0;box-shadow:0 6px 16px rgba(0,0,0,0.12);z-index:999}
.scroll-top:active{transform:translateY(1px)}
.scroll-icon{width:15.22px;height:17.39px;color:#ffffff}
.scroll-icon path{stroke:currentColor}
.scroll-top:hover{opacity:0.9}
</style>
