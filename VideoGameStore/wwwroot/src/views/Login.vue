<template>
  <div class="login-root">
    <SiteHeader />

    <main class="login-canvas" role="main" aria-label="Вход">
      <div class="login-layout">
        <div class="login-image" aria-hidden="true"></div>

        <div class="login-form">
          <h1 class="login-title">Вход в аккаунт</h1>
          <p class="login-sub">Введите детали входа</p>

          <label class="login-field"><input placeholder="Почта или Логин" /></label>
          <label class="login-field"><input placeholder="Пароль" type="password" /></label>

          <div class="login-actions">
            <button 
              class="login-submit" 
              @click="handleLogin"
              :disabled="isLoading"
            >
              {{ isLoading ? 'Вход...' : 'Войти' }}
            </button>
            <a class="login-forgot" href="#" @click.prevent="noop">Забыли пароль?</a>
          </div>

        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SiteHeader from '../components/Header.vue'
import Footer from '../components/Footer.vue'

const isLoading = ref(false)

const handleLogin = () => {
  if (isLoading.value) return
  
  isLoading.value = true
  
  // Имитация запроса на сервер
  setTimeout(() => {
    isLoading.value = false
    // Здесь будет логика входа
    console.log('Вход выполнен')
  }, 1000)
}

const noop = () => {}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');

.login-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: #F5F5F5;
}

.login-canvas {
  width: 100%;
  display: flex;
  justify-content: center;
  background: #F5F5F5;
  flex: 1;
  padding: 150px 70px;
  box-sizing: border-box;
}

.login-layout {
  display: flex;
  align-items: center;
  gap: 160px;
  width: 100%;
  max-width: 1480px; /* 900px + 420px + 160px gap */
  margin: 0 auto;
}

.login-image {
  width: 900px;
  height: 510px;
  background-image: url('/src/assets/photos/login.png');
  background-size: cover;
  background-position: center;
  border-radius: 30px;
  flex: 0 0 900px;
}

.login-form {
  width: 420px;
  display: flex;
  flex-direction: column;
  gap: 18px;
  flex-shrink: 0;
}

.login-title {
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 36px;
  margin: 0 0 6px;
  line-height: 1.2;
}

.login-sub {
  margin: 0;
  color: #333;
  font-size: 16px;
  line-height: 1.4;
}

.login-field {
  display: block;
}

.login-field input {
  width: 100%;
  border: 0;
  border-bottom: 1px solid #cfcfcf;
  padding: 12px 6px;
  font-size: 16px;
  background: transparent;
  outline: none;
  transition: border-color 0.2s ease;
}

.login-field input:focus {
  border-bottom-color: #222;
}

.login-submit {
  margin-top: 8px;
  background: #222;
  color: #fff;
  border: 0;
  border-radius: 30px;
  padding: 12px 18px;
  font-size: 16px;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
  min-width: 120px;
  transition: background-color 0.2s ease;
}

.login-submit:active {
  background: #111;
}

.login-submit:disabled {
  background: #666;
  cursor: not-allowed;
}

.login-forgot {
  margin-top: 12px;
  color: #666;
  text-decoration: none;
  margin-left: 12px;
  transition: color 0.2s ease;
  font-size: 14px;
  white-space: nowrap;
}

.login-forgot:hover {
  color: #222;
}

.login-actions {
  display: flex;
  align-items: center;
  gap: 24px;
  flex-wrap: wrap;
}

/* Плавная адаптация для больших экранов */
@media (max-width: 1600px) {
  .login-canvas {
    padding: 120px 50px;
  }
  
  .login-layout {
    gap: 120px;
    max-width: 1340px; /* 800px + 420px + 120px */
  }
  
  .login-image {
    width: 800px;
    height: 450px;
    flex: 0 0 800px;
  }
}

@media (max-width: 1400px) {
  .login-canvas {
    padding: 100px 40px;
  }
  
  .login-layout {
    gap: 80px;
    max-width: 1200px;
  }
  
  .login-image {
    width: 700px;
    height: 400px;
    flex: 0 0 700px;
  }
}

/* Средние экраны - начинаем уменьшать изображение */
@media (max-width: 1200px) {
  .login-canvas {
    padding: 80px 30px;
  }
  
  .login-layout {
    gap: 60px;
    max-width: 1000px;
  }
  
  .login-image {
    width: 600px;
    height: 340px;
    flex: 0 0 600px;
  }
  
  .login-form {
    width: 340px;
  }
}

/* Переходный этап - уменьшаем дальше */
@media (max-width: 1024px) {
  .login-canvas {
    padding: 60px 25px;
  }
  
  .login-layout {
    gap: 40px;
    max-width: 900px;
  }
  
  .login-image {
    width: 500px;
    height: 284px;
    flex: 0 0 500px;
  }
  
  .login-form {
    width: 320px;
  }
  
  .login-title {
    font-size: 32px;
  }
}

/* Планшеты - скрываем изображение, центрируем форму */
@media (max-width: 900px) {
  .login-canvas {
    padding: 50px 20px;
  }
  
  .login-layout {
    justify-content: center;
  }
  
  .login-image {
    display: none;
  }
  
  .login-form {
    width: 100%;
    max-width: 500px;
    padding: 40px;
    background: #FDFDFD;
    border-radius: 25px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  }
  
  .login-title {
    text-align: center;
    font-size: 30px;
  }
  
  .login-sub {
    text-align: center;
  }
  
  .login-actions {
    justify-content: center;
  }
}

/* Мобильные устройства среднего размера */
@media (max-width: 600px) {
  .login-canvas {
    padding: 40px 15px;
  }
  
  .login-form {
    padding: 30px 25px;
    border-radius: 20px;
  }
  
  .login-title {
    font-size: 28px;
  }
  
  .login-sub {
    font-size: 15px;
  }
  
  .login-field input {
    padding: 10px 6px;
    font-size: 15px;
  }
  
  .login-submit {
    padding: 10px 16px;
    font-size: 15px;
  }
  
  .login-forgot {
    font-size: 13px;
    margin-left: 0;
  }
  
  .login-actions {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .login-submit {
    width: 100%;
  }
  
  .login-forgot {
    text-align: center;
    margin-top: 5px;
  }
}

/* Маленькие мобильные устройства */
@media (max-width: 400px) {
  .login-canvas {
    padding: 30px 10px;
  }
  
  .login-form {
    padding: 25px 20px;
    border-radius: 18px;
  }
  
  .login-title {
    font-size: 26px;
  }
  
  .login-sub {
    font-size: 14px;
  }
  
  .login-field input {
    padding: 8px 6px;
    font-size: 14px;
  }
  
  .login-submit {
    padding: 9px 14px;
    font-size: 14px;
  }
  
  .login-forgot {
    font-size: 12px;
  }
}

/* Очень маленькие экраны */
@media (max-width: 320px) {
  .login-form {
    padding: 20px 15px;
    border-radius: 15px;
  }
  
  .login-title {
    font-size: 24px;
  }
  
  .login-sub {
    font-size: 13px;
  }
  
  .login-field input {
    padding: 7px 5px;
    font-size: 13px;
  }
}
</style>
