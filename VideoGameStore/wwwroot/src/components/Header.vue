<template>
  <header class="site-header" role="banner" aria-label="Главная шапка">
    <div class="header-inner">
      <div class="header-left">
        <router-link to="/" class="logo-wrap">
          <div class="logo-text">Flex-Shop</div>
          <img src="/src/assets/icons/logo.svg" alt="logo" class="logo-icon" />
        </router-link>
      </div>

      <div class="header-center">
        <button class="genres-btn" aria-label="Жанры" @click="toggleGenresMenu()">
          <svg class="icon genres-icon" viewBox="0 0 35 35" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
            <path d="M1.69355 0C0.75504 0 0 0.90024 0 2.01923V6.05769C0 7.17668 0.75504 8.07692 1.69355 8.07692H5.08065C6.01915 8.07692 6.77419 7.17668 6.77419 6.05769V2.01923C6.77419 0.90024 6.01915 0 5.08065 0H1.69355ZM12.4194 1.34615C11.1704 1.34615 10.1613 2.54928 10.1613 4.03846C10.1613 5.52764 11.1704 6.73077 12.4194 6.73077H32.7419C33.9909 6.73077 35 5.52764 35 4.03846C35 2.54928 33.9909 1.34615 32.7419 1.34615H12.4194ZM12.4194 14.8077C11.1704 14.8077 10.1613 16.0108 10.1613 17.5C10.1613 18.9892 11.1704 20.1923 12.4194 20.1923H32.7419C33.9909 20.1923 35 18.9892 35 17.5C35 16.0108 33.9909 14.8077 32.7419 14.8077H12.4194ZM12.4194 28.2692C11.1704 28.2692 10.1613 29.4724 10.1613 30.9615C10.1613 32.4507 11.1704 33.6539 12.4194 33.6539H32.7419C33.9909 33.6539 35 32.4507 35 30.9615C35 29.4724 33.9909 28.2692 32.7419 28.2692H12.4194ZM0 15.4808V19.5192C0 20.6382 0.75504 21.5385 1.69355 21.5385H5.08065C6.01915 21.5385 6.77419 20.6382 6.77419 19.5192V15.4808C6.77419 14.3618 6.01915 13.4615 5.08065 13.4615H1.69355C0.75504 13.4615 0 14.3618 0 15.4808ZM1.69355 26.9231C0.75504 26.9231 0 27.8233 0 28.9423V32.9808C0 34.0998 0.75504 35 1.69355 35H5.08065C6.01915 35 6.77419 34.0998 6.77419 32.9808V28.9423C6.77419 27.8233 6.01915 26.9231 5.08065 26.9231H1.69355Z" fill="currentColor"/>
          </svg>
        </button>

        <div class="search-wrap">
          <img src="/src/assets/icons/search.svg" alt="search" class="search-inline" />
          <input class="search-input" placeholder="Поиск" aria-label="Поиск" />
        </div>
      </div>

      <div class="header-right">
        <router-link to="/cart" class="action-btn cart-btn">
          <svg class="icon cart-icon" viewBox="59 55 28 29" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
            <path d="M87 67.5001V68.5001C87 69.3285 86.4777 70.0001 85.8333 70.0001H85.4444L84.1751 81.4243C84.0109 82.9022 83.0264 84 81.8652 84H64.1348C62.9736 84 61.9891 82.9022 61.8249 81.4243L60.5556 70.0001H60.1667C59.5223 70.0001 59 69.3285 59 68.5001V67.5001C59 66.6717 59.5223 66.0001 60.1667 66.0001H63.4402L68.6309 56.8239C69.1362 55.9306 70.109 55.7331 70.8038 56.3827C71.4986 57.0324 71.6523 58.2832 71.1469 59.1765L67.2871 66.0001H78.7129L74.8531 59.1765C74.3477 58.2832 74.5014 57.0324 75.1962 56.3827C75.8909 55.733 76.8639 55.9305 77.3692 56.8238L82.5598 66.0001H85.8333C86.4777 66.0001 87 66.6717 87 67.5001ZM74.1667 78.5V71.5001C74.1667 70.6716 73.6443 70.0001 73 70.0001C72.3557 70.0001 71.8333 70.6716 71.8333 71.5001V78.5C71.8333 79.3285 72.3557 80 73 80C73.6443 80 74.1667 79.3285 74.1667 78.5ZM79.6111 78.5V71.5001C79.6111 70.6716 79.0888 70.0001 78.4444 70.0001C77.8001 70.0001 77.2778 70.6716 77.2778 71.5001V78.5C77.2778 79.3285 77.8001 80 78.4444 80C79.0888 80 79.6111 79.3285 79.6111 78.5ZM68.7222 78.5V71.5001C68.7222 70.6716 68.1999 70.0001 67.5556 70.0001C66.9112 70.0001 66.3889 70.6716 66.3889 71.5001V78.5C66.3889 79.3285 66.9112 80 67.5556 80C68.1999 80 68.7222 79.3285 68.7222 78.5Z" fill="currentColor"/>
          </svg>
          <span class="action-label">Корзина</span>
        </router-link>
        <button type="button" class="action-btn user-btn" @click="toggleGuestMenu()">
          <svg class="icon user-icon" viewBox="107 56 26 28" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
            <path d="M120 70C121.97 70 123.86 69.2625 125.253 67.9497C126.646 66.637 127.429 64.8565 127.429 63C127.429 61.1435 126.646 59.363 125.253 58.0503C123.86 56.7375 121.97 56 120 56C118.03 56 116.14 56.7375 114.747 58.0503C113.354 59.363 112.571 61.1435 112.571 63C112.571 64.8565 113.354 66.637 114.747 67.9497C116.14 69.2625 118.03 70 120 70ZM117.348 72.625C111.631 72.625 107 76.9891 107 82.3758C107 83.2727 107.772 84 108.724 84H131.276C132.228 84 133 83.2727 133 82.3758C133 76.9891 128.369 72.625 122.652 72.625H117.348Z" fill="currentColor"/>
          </svg>
          <span class="action-label">Войти</span>
        </button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { showGuestMenu, showUserMenu, showGenresMenu } from '../stores/ui'

function toggleGuestMenu(){
  showGuestMenu.value = !showGuestMenu.value
}

function toggleUserMenu(){
  showUserMenu.value = !showUserMenu.value
}

function toggleGenresMenu(){
  showGenresMenu.value = !showGenresMenu.value
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');

.site-header {
  width: 100%;
  display: flex;
  justify-content: center;
  background: #222222;
  z-index: 1000;
  transition: all 0.3s ease;
}

.header-inner {
  white-space: nowrap;
  max-width: 1920px;
  width: 100%;
  height: 164px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 150px;
  box-sizing: border-box;
  color: #fff;
  margin: 0 auto;
  background: transparent;
  transition: all 0.3s ease;
}

.header-left {
  display: flex;
  align-items: center;
}

.logo-wrap {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: opacity 0.18s ease;
  text-decoration: none;
  color: inherit;
}

.logo-text {
  font-family: 'Jersey 10', Georgia, serif;
  font-size: 64px;
  color: inherit;
  line-height: 1;
  transition: font-size 0.3s ease;
}

.logo-icon {
  width: 45px;
  height: 40px;
  margin-top: 10px;
  transition: all 0.3s ease;
}

.logo-wrap:hover {
  opacity: 0.8;
}

.header-center {
  position: relative;
  display: flex;
  align-items: center;
  padding-right: 60px;
}

.genres-btn {
  width: 45px;
  height: 45px;
  border-radius: 30px;
  background: transparent;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.search-wrap {
  width: 420px;
  height: 25px;
  background: #FFFFFF;
  border-radius: 30px;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px;
  box-shadow: 0 4px 12px rgba(2, 6, 23, 0.12);
  margin: 0 24px;
  transition: all 0.3s ease;
}

.search-inline {
  width: 20px;
  height: 20px;
  opacity: 0.8;
  transition: all 0.3s ease;
}

.search-input {
  flex: 1;
  border: 0;
  outline: none;
  font-size: 16px;
  background: transparent;
  transition: all 0.3s ease;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 40px;
  transition: all 0.3s ease;
}

.action-btn {
  background: transparent;
  border: none;
  color: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
  text-decoration: none;
  transition: all 0.3s ease;
}

.action-label {
  font-size: 14px;
  margin-top: 6px;
  color: inherit;
  transition: all 0.3s ease;
}

.icon {
  display: block;
  fill: currentColor;
  transition: all 0.3s ease;
}

.genres-icon {
  width: 35px;
  height: 35px;
  transition: all 0.3s ease;
}

.cart-icon {
  width: 50px;
  height: 50px;
  transition: all 0.3s ease;
}

.user-icon {
  width: 45px;
  height: 50px;
  transition: all 0.3s ease;
}

.genres-btn,
.action-btn {
  color: #ffffff;
  transition: color 0.18s ease, transform 0.18s ease;
}

.genres-btn .icon,
.action-btn .icon {
  transition: transform 0.18s ease;
}

.genres-btn:hover,
.action-btn:hover {
  color: #A53DFF;
}

.action-btn:hover .icon,
.genres-btn:hover .icon {
  transform: translateY(-2px);
}

/* Плавная адаптация для больших экранов */
@media (max-width: 1600px) {
  .header-inner {
    padding: 0 100px;
  }
  
  .logo-text {
    font-size: 58px;
  }
  
  .search-wrap {
    width: 380px;
  }
  
  .header-right {
    gap: 30px;
  }
}

@media (max-width: 1440px) {
  .header-inner {
    padding: 0 70px;
    height: 140px;
  }
  
  .logo-text {
    font-size: 52px;
  }
  
  .logo-icon {
    width: 40px;
    height: 36px;
    margin-top: 8px;
  }
  
  .search-wrap {
    width: 360px;
    margin: 0 20px;
  }
  
  .genres-icon {
    width: 32px;
    height: 32px;
  }
  
  .cart-icon {
    width: 46px;
    height: 46px;
  }
  
  .user-icon {
    width: 42px;
    height: 46px;
  }
  
  .header-right {
    gap: 25px;
  }
}

@media (max-width: 1280px) {
  .header-inner {
    padding: 0 50px;
    height: 130px;
  }
  
  .logo-text {
    font-size: 48px;
  }
  
  .logo-icon {
    width: 38px;
    height: 34px;
    margin-top: 6px;
  }
  
  .search-wrap {
    width: 340px;
    margin: 0 18px;
    padding: 10px 14px;
  }
  
  .genres-icon {
    width: 30px;
    height: 30px;
  }
  
  .genres-btn {
    width: 42px;
    height: 42px;
  }
  
  .cart-icon {
    width: 44px;
    height: 44px;
  }
  
  .user-icon {
    width: 40px;
    height: 44px;
  }
  
  .action-label {
    font-size: 13px;
  }
}

/* Планшеты горизонтальные */
@media (max-width: 1100px) {
  .header-inner {
    padding: 0 40px;
    height: 120px;
  }
  
  .logo-text {
    font-size: 44px;
  }
  
  .logo-icon {
    width: 35px;
    height: 32px;
  }
  
  .search-wrap {
    width: 300px;
    margin: 0 16px;
  }
  
  .search-input {
    font-size: 15px;
  }
  
  .header-right {
    gap: 20px;
  }
}

/* Переход к мобильному виду */
@media (max-width: 1024px) {
  .header-inner {
    height: auto;
    padding: 15px 30px;
    flex-wrap: wrap;
    align-items: center;
  }
  
  .header-left {
    order: 1;
    width: 100%;
    display: flex;
    justify-content: center;
    margin-bottom: 10px;
  }
  
  .header-center {
    order: 3;
    width: 100%;
    display: flex;
    justify-content: center;
    padding: 15px 0 0;
    padding-right: 0;
  }
  
  .header-right {
    order: 2;
    width: 100%;
    display: flex;
    justify-content: center;
    margin-bottom: 10px;
  }
  
  .logo-text {
    font-size: 40px;
  }
  
  .search-wrap {
    width: calc(100% - 32px);
    max-width: 600px;
    margin: 0 auto;
  }
}

/* Планшеты вертикальные */
@media (max-width: 768px) {
  .header-inner {
    padding: 12px 20px;
  }
  
  .logo-text {
    font-size: 36px;
  }
  
  .logo-icon {
    width: 32px;
    height: 28px;
  }
  
  .genres-btn {
    width: 40px;
    height: 40px;
  }
  
  .genres-icon {
    width: 28px;
    height: 28px;
  }
  
  .cart-icon {
    width: 40px;
    height: 40px;
  }
  
  .user-icon {
    width: 36px;
    height: 40px;
  }
  
  .action-label {
    font-size: 12px;
  }
  
  .header-right {
    gap: 25px;
  }
}

/* Большие мобильные */
@media (max-width: 600px) {
  .header-inner {
    padding: 10px 15px;
  }
  
  .header-left,
  .header-right {
    margin-bottom: 8px;
  }
  
  .header-center {
    padding-top: 10px;
  }
  
  .logo-text {
    font-size: 32px;
  }
  
  .logo-icon {
    width: 28px;
    height: 25px;
  }
  
  .search-wrap {
    width: calc(100% - 20px);
    padding: 8px 12px;
  }
  
  .search-input {
    font-size: 14px;
  }
  
  .search-inline {
    width: 18px;
    height: 18px;
  }
  
  .header-right {
    gap: 20px;
  }
}

/* Средние мобильные */
@media (max-width: 480px) {
  .header-inner {
    padding: 8px 12px;
  }
  
  .logo-text {
    font-size: 28px;
  }
  
  .logo-icon {
    width: 25px;
    height: 22px;
    margin-top: 4px;
  }
  
  .genres-btn {
    width: 38px;
    height: 38px;
  }
  
  .genres-icon {
    width: 26px;
    height: 26px;
  }
  
  .search-wrap {
    padding: 7px 10px;
    gap: 8px;
  }
  
  .search-input::placeholder {
    font-size: 13px;
  }
  
  .cart-icon,
  .user-icon {
    width: 36px;
    height: 36px;
  }
  
  .action-label {
    font-size: 11px;
    margin-top: 4px;
  }
  
  .header-right {
    gap: 18px;
  }
}

/* Маленькие мобильные */
@media (max-width: 360px) {
  .header-inner {
    padding: 6px 10px;
  }
  
  .logo-text {
    font-size: 26px;
  }
  
  .logo-icon {
    width: 22px;
    height: 20px;
  }
  
  .logo-wrap {
    gap: 6px;
  }
  
  .search-wrap {
    width: calc(100% - 16px);
    padding: 6px 10px;
  }
  
  .search-input {
    font-size: 13px;
  }
  
  .search-inline {
    width: 16px;
    height: 16px;
  }
  
  .header-right {
    gap: 15px;
  }
  
  .cart-icon,
  .user-icon {
    width: 32px;
    height: 32px;
  }
  
  .genres-btn {
    width: 36px;
    height: 36px;
  }
  
  .genres-icon {
    width: 24px;
    height: 24px;
  }
  
  .action-label {
    font-size: 10px;
  }
}
</style>
