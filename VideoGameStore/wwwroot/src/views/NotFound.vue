<!-- src/views/NotFound.vue -->
<template>
  <div class="not-found">
    <h1>404</h1>
    <p>Страница не найдена</p>
    <router-link to="/" class="home-link">На главную</router-link>
  </div>
</template>

<style scoped>
.not-found {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  text-align: center;
}

.not-found h1 {
  font-size: 120px;
  color: #A53DFF;
  margin: 0;
}

.not-found p {
  font-size: 24px;
  color: #666;
  margin: 20px 0;
}

.home-link {
  display: inline-block;
  padding: 12px 24px;
  background: #A53DFF;
  color: white;
  text-decoration: none;
  border-radius: 8px;
  font-size: 18px;
  transition: background 0.3s;
}

.home-link:hover {
  background: #8B1FD9;
}
</style>