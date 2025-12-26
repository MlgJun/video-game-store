<template>
  <div class="history-root">
    <SiteHeader />

    <main class="history-canvas">
      <div class="history-inner">
        <section class="history-container">
          <div class="history-header">
            <h1 class="history-title">История покупок</h1>
            <div class="history-count">
              <span class="history-items-label">товаров</span>
              <span class="history-items-number"></span>
            </div>
          </div>

          <div class="history-body">
            <!-- Товары будут добавлены позже -->
          </div>
        </section>

        <aside class="history-sidebar">
          <img src="/src/assets/photos/history.png" alt="История покупок" class="history-image" />
        </aside>
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

.history-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #F5F5F5;
  transition: all 0.3s ease;
}

.history-canvas {
  flex: 1;
  display: flex;
  justify-content: center;
  min-height: calc(100vh - 164px);
  padding: 40px 20px;
  box-sizing: border-box;
  transition: all 0.3s ease;
}

.history-inner {
  width: 100%;
  max-width: 1400px;
  display: flex;
  gap: 140px;
  padding: 0;
  box-sizing: border-box;
  align-items: stretch;
  transition: all 0.3s ease;
}

.history-container {
  flex: 1;
  background: #FDFDFD;
  border-radius: 30px;
  padding: 30px 36px;
  box-sizing: border-box;
  min-height: 100%;
  transition: all 0.3s ease;
}

.history-header {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
  padding-bottom: 16px;
  border-bottom: 1px solid #E0E0E0;
  transition: all 0.3s ease;
}

.history-title {
  font-size: 36px;
  margin: 0;
  font-weight: 500;
  transition: font-size 0.3s ease;
}

.history-count {
  font-size: 20px;
  color: #676767;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.history-body {
  margin-top: 24px;
  transition: all 0.3s ease;
}

.history-sidebar {
  width: 340px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: auto;
  transition: all 0.3s ease;
}

.history-image {
  width: 423px;
  height: 624px;
  border-radius: 30px;
  object-fit: cover;
  transition: all 0.3s ease;
}

.history-items-label {
  color: #676767;
  font-family: 'Montserrat Alternates', sans-serif;
  font-size: 20px;
  transition: font-size 0.3s ease;
}

.history-items-number {
  min-width: 56px;
  display: inline-block;
  transition: all 0.3s ease;
}

 @media (max-width: 1600px) {
  .history-inner {
    max-width: 1300px;
    gap: 120px;
  }
  
  .history-sidebar {
    width: 320px;
  }
  
  .history-image {
    width: 400px;
    height: 590px;
  }
}

@media (max-width: 1500px) {
  .history-inner {
    max-width: 1200px;
    gap: 100px;
  }
  
  .history-sidebar {
    width: 300px;
  }
  
  .history-image {
    width: 380px;
    height: 560px;
  }
}

@media (max-width: 1400px) {
  .history-inner {
    max-width: 1100px;
    gap: 80px;
  }
  
  .history-sidebar {
    width: 280px;
  }
  
  .history-image {
    width: 350px;
    height: 520px;
    opacity: 0.95;
  }
  
  .history-title {
    font-size: 34px;
  }
  
  .history-container {
    padding: 28px 32px;
  }
}

@media (max-width: 1300px) {
  .history-inner {
    gap: 60px;
    max-width: 1000px;
  }
  
  .history-sidebar {
    width: 260px;
  }
  
  .history-image {
    width: 320px;
    height: 480px;
    opacity: 0.9;
  }
  
  .history-title {
    font-size: 32px;
  }
  
  .history-items-label {
    font-size: 19px;
  }
}

@media (max-width: 1200px) {
  .history-canvas {
    padding: 35px 20px;
  }
  
  .history-inner {
    gap: 40px;
    max-width: 900px;
  }
  
  .history-sidebar {
    width: 200px;
  }
  
  .history-image {
    width: 250px;
    height: 370px;
    opacity: 0.8;
  }
  
  .history-title {
    font-size: 30px;
  }
  
  .history-container {
    padding: 25px 30px;
  }
}

@media (max-width: 1100px) {
  .history-canvas {
    padding: 30px 18px;
  }
  
  .history-inner {
    flex-direction: column;
    gap: 30px;
    max-width: 800px;
  }
  
  .history-image {
    display: none;
  }
  
  .history-sidebar {
    display: none;
  }
  
  .history-container {
    width: 100%;
    padding: 24px 28px;
  }
  
  .history-title {
    font-size: 28px;
  }
  
  .history-items-label {
    font-size: 18px;
  }
  
  .history-count {
    font-size: 19px;
  }
}

@media (max-width: 900px) {
  .history-canvas {
    padding: 20px 15px;
  }
  
  .history-inner {
    gap: 20px;
  }
  
  .history-container {
    padding: 22px 24px;
    border-radius: 25px;
  }
  
  .history-title {
    font-size: 26px;
  }
  
  .history-header {
    flex-direction: column;
    gap: 10px;
  }
  
  .history-count {
    font-size: 18px;
  }
  
  .history-items-label {
    font-size: 17px;
  }
}

@media (max-width: 768px) {
  .history-canvas {
    padding: 18px 12px;
    min-height: auto;
  }
  
  .history-container {
    padding: 20px 22px;
    border-radius: 22px;
  }
  
  .history-title {
    font-size: 24px;
  }
  
  .history-count {
    font-size: 17px;
  }
}

@media (max-width: 600px) {
  .history-canvas {
    padding: 15px 10px;
  }
  
  .history-container {
    padding: 18px 20px;
    border-radius: 20px;
  }
  
  .history-title {
    font-size: 22px;
  }
  
  .history-header {
    gap: 8px;
    padding-bottom: 14px;
  }
  
  .history-count {
    font-size: 16px;
  }
  
  .history-items-label {
    font-size: 16px;
  }
  
  .history-body {
    margin-top: 20px;
  }
}

@media (max-width: 480px) {
  .history-canvas {
    padding: 12px 8px;
  }
  
  .history-container {
    padding: 16px 18px;
    border-radius: 18px;
  }
  
  .history-title {
    font-size: 20px;
  }
  
  .history-header {
    gap: 6px;
    padding-bottom: 12px;
  }
  
  .history-count {
    font-size: 15px;
  }
  
  .history-items-label {
    font-size: 15px;
  }
  
  .history-body {
    margin-top: 18px;
  }
}

@media (max-width: 360px) {
  .history-canvas {
    padding: 10px 6px;
  }
  
  .history-container {
    padding: 14px 16px;
    border-radius: 16px;
  }
  
  .history-title {
    font-size: 18px;
  }
  
  .history-header {
    gap: 5px;
    padding-bottom: 10px;
  }
  
  .history-count {
    font-size: 14px;
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }
  
  .history-items-label {
    font-size: 14px;
  }
  
  .history-body {
    margin-top: 16px;
  }
}
</style>
