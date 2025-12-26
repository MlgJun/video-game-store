<template>
  <RouterView />
  <ScrollTop />
</template>

<script setup>
import { RouterView } from 'vue-router'
import ScrollTop from './components/ScrollTop.vue'
</script>

<style>
html,body,#app{height:100%;margin:0}
body{font-family:Inter, system-ui, Arial, sans-serif;background:#F5F5F5}
</style>
