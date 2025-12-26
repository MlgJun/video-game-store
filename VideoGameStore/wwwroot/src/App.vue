<template>
	<AppRouter />
	<GuestMenu v-model:visible="showGuestMenu" />
	<UserMenu v-model:visible="showUserMenu" />
	<GenresMenu v-model:visible="showGenresMenu" />
	<ScrollTop />
</template>

<script setup>
import { onMounted, onBeforeUnmount, watch } from 'vue'
import AppRouter from './AppRouter.vue'
import ScrollTop from './components/ScrollTop.vue'
import GuestMenu from './components/GuestMenu.vue'
import UserMenu from './components/UserMenu.vue'
import GenresMenu from './components/GenresMenu.vue'
import { showGuestMenu } from './stores/ui'
import { showUserMenu, showGenresMenu } from './stores/ui'


onMounted(()=>{
	window.addEventListener('keydown', handleKey)
	try{ window.__showGuestMenuRef = showGuestMenu }catch(e){}
})

onBeforeUnmount(()=> window.removeEventListener('keydown', handleKey))

watch(showGuestMenu, ()=>{})
watch(showUserMenu, ()=>{})
watch(showGenresMenu, ()=>{})
</script>



