<template>
  <div class="register-root">
    <SiteHeader />

    <main class="register-canvas" role="main" aria-label="Регистрация">
      <div class="register-layout">
        <div class="register-image" aria-hidden="true"></div>

        <div class="register-form">
          <h1 class="reg-title">Создание аккаунта</h1>
          <p class="reg-sub">Введите детали аккаунта</p>

          <label class="reg-field"><input placeholder="Логин" /></label>
          <label class="reg-field"><input placeholder="Почта" type="email" /></label>
          <label class="reg-field"><input placeholder="Пароль" type="password" /></label>
          <label class="reg-field"><input placeholder="Повторите пароль" type="password" /></label>

          <button class="reg-submit">Создать аккаунт</button>

          <div class="reg-footer">Уже имеете аккаунт? <a href="/login">Войти</a></div>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import SiteHeader from '../components/Header.vue'
import Footer from '../components/Footer.vue'
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');

.register-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: #F5F5F5;
}

.register-canvas {
  width: 100%;
  display: flex;
  justify-content: center;
  background: #F5F5F5;
  flex: 1;
  padding: 150px 70px;
  box-sizing: border-box;
  transition: all 0.3s ease;
}

.register-layout {
  display: flex;
  align-items: center;
  gap: 160px;
  width: 100%;
  max-width: 1480px;
  margin: 0 auto;
  transition: all 0.3s ease;
}

.register-image {
  width: 900px;
  height: 510px;
  background-image: url('/src/assets/photos/registration.png');
  background-size: cover;
  background-position: center;
  border-radius: 30px;
  flex: 0 0 900px;
  transition: all 0.3s ease;
}

.register-form {
  width: 420px;
  display: flex;
  flex-direction: column;
  gap: 18px;
  flex-shrink: 0;
  transition: all 0.3s ease;
}

.reg-title {
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 36px;
  margin: 0 0 6px;
  line-height: 1.2;
  transition: font-size 0.3s ease;
}

.reg-sub {
  margin: 0;
  color: #333;
  font-size: 16px;
  line-height: 1.4;
  transition: font-size 0.3s ease;
}

.reg-field {
  display: block;
}

.reg-field input {
  width: 100%;
  border: 0;
  border-bottom: 1px solid #cfcfcf;
  padding: 12px 6px;
  font-size: 16px;
  background: transparent;
  outline: none;
  transition: all 0.3s ease;
}

.reg-field input:focus {
  border-bottom-color: #222;
}

.reg-submit {
  margin-top: 8px;
  background: #222;
  color: #fff;
  border: 0;
  border-radius: 30px;
  padding: 12px 18px;
  font-size: 16px;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
  min-width: 180px;
  transition: all 0.3s ease;
}

.reg-submit:active {
  background: #111;
}

.reg-submit:disabled {
  background: #666;
  cursor: not-allowed;
}

.reg-footer {
  margin-top: 12px;
  color: #666;
  font-size: 15px;
  transition: all 0.3s ease;
}

.reg-footer a {
  color: #222;
  text-decoration: none;
  margin-left: 6px;
  font-weight: 600;
  transition: color 0.2s ease;
}

.reg-footer a:hover {
  color: #A53DFF;
}

@media (max-width: 1600px) {
  .register-canvas {
    padding: 120px 50px;
  }
  
  .register-layout {
    gap: 120px;
    max-width: 1340px;
  }
  
  .register-image {
    width: 800px;
    height: 450px;
    flex: 0 0 800px;
  }
}

@media (max-width: 1400px) {
  .register-canvas {
    padding: 100px 40px;
  }
  
  .register-layout {
    gap: 80px;
    max-width: 1200px;
  }
  
  .register-image {
    width: 700px;
    height: 400px;
    flex: 0 0 700px;
  }
  
  .register-form {
    width: 380px;
  }
}

@media (max-width: 1200px) {
  .register-canvas {
    padding: 80px 30px;
  }
  
  .register-layout {
    gap: 60px;
    max-width: 1000px;
  }
  
  .register-image {
    width: 600px;
    height: 340px;
    flex: 0 0 600px;
  }
  
  .register-form {
    width: 340px;
  }
  
  .reg-title {
    font-size: 32px;
  }
}

@media (max-width: 1100px) {
  .register-canvas {
    padding: 60px 25px;
  }
  
  .register-layout {
    gap: 40px;
    max-width: 900px;
  }
  
  .register-image {
    width: 500px;
    height: 284px;
    flex: 0 0 500px;
  }
  
  .register-form {
    width: 320px;
  }
  
  .reg-title {
    font-size: 30px;
  }
  
  .reg-sub {
    font-size: 15px;
  }
}

@media (max-width: 1024px) {
  .register-canvas {
    padding: 50px 20px;
  }
  
  .register-layout {
    justify-content: center;
  }
  
  .register-image {
    display: none;
  }
  
  .register-form {
    width: 100%;
    max-width: 500px;
    padding: 40px;
    background: #FDFDFD;
    border-radius: 25px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  }
  
  .reg-title {
    text-align: center;
    font-size: 30px;
  }
  
  .reg-sub {
    text-align: center;
  }
  
  .reg-submit {
    align-self: center;
    min-width: 200px;
  }
  
  .reg-footer {
    text-align: center;
  }
}

@media (max-width: 900px) {
  .register-canvas {
    padding: 40px 20px;
  }
  
  .register-form {
    padding: 35px 30px;
  }
  
  .reg-title {
    font-size: 28px;
  }
  
  .reg-sub {
    font-size: 15px;
  }
  
  .reg-field input {
    padding: 10px 6px;
    font-size: 15px;
  }
  
  .reg-submit {
    padding: 10px 16px;
    font-size: 15px;
  }
  
  .reg-footer {
    font-size: 14px;
  }
}

@media (max-width: 600px) {
  .register-canvas {
    padding: 30px 15px;
  }
  
  .register-form {
    padding: 30px 25px;
    border-radius: 20px;
  }
  
  .reg-title {
    font-size: 26px;
  }
  
  .reg-sub {
    font-size: 14px;
  }
  
  .reg-field input {
    padding: 9px 6px;
    font-size: 14px;
  }
  
  .reg-submit {
    padding: 9px 14px;
    font-size: 14px;
    min-width: 100%;
  }
  
  .reg-footer {
    font-size: 13px;
  }
}

@media (max-width: 480px) {
  .register-canvas {
    padding: 25px 12px;
  }
  
  .register-form {
    padding: 25px 20px;
    border-radius: 18px;
    gap: 15px;
  }
  
  .reg-title {
    font-size: 24px;
  }
  
  .reg-sub {
    font-size: 13px;
  }
  
  .reg-field input {
    padding: 8px 6px;
    font-size: 13px;
  }
  
  .reg-submit {
    padding: 8px 12px;
    font-size: 13px;
  }
  
  .reg-footer {
    font-size: 12px;
  }
}

@media (max-width: 360px) {
  .register-canvas {
    padding: 20px 10px;
  }
  
  .register-form {
    padding: 20px 18px;
    border-radius: 16px;
    gap: 12px;
  }
  
  .reg-title {
    font-size: 22px;
  }
  
  .reg-sub {
    font-size: 12px;
  }
  
  .reg-field input {
    padding: 7px 5px;
    font-size: 12px;
  }
  
  .reg-submit {
    padding: 7px 10px;
    font-size: 12px;
  }
  
  .reg-footer {
    font-size: 11px;
  }
}

@media (max-width: 320px) {
  .register-canvas {
    padding: 18px 8px;
  }
  
  .register-form {
    padding: 18px 16px;
    border-radius: 14px;
  }
  
  .reg-title {
    font-size: 20px;
  }
  
  .reg-sub {
    font-size: 11px;
  }
  
  .reg-field input {
    padding: 6px 4px;
    font-size: 11px;
  }
  
  .reg-submit {
    padding: 6px 8px;
    font-size: 11px;
  }
  
  .reg-footer {
    font-size: 10px;
  }
}
</style>
