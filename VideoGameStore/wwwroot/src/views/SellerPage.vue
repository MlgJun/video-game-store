<template>
  <div class="app-root">
    <SiteHeader />

    <main class="seller-canvas" role="main" aria-label="Страница продавца">
      <div class="products-section">
        <div class="products-header">
          <h2 class="products-title">Товары</h2>
          <span class="products-count">шт.</span>
          <button class="add-product-btn" @click="openAddModal" aria-label="Добавить товар">Добавить товар</button>
        </div>

        <div class="products-container">
          <!-- Здесь будут товары -->
        </div>
      </div>
    </main>

    <Footer />

    <div v-if="showAddModal" class="modal-overlay" @click.self="closeAddModal">
      <div class="modal-window">
        <SellerProductEditor :product="newProduct" @save="onModalSave" @cancel="closeAddModal" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SiteHeader from '../components/Header.vue'
import Footer from '../components/Footer.vue'
import SellerProductEditor from '../components/SellerProductEdit.vue'

const showAddModal = ref(false)
const newProduct = ref({ title: '', description: '', price: 0, image: '', cardImage: '' })

function openAddModal() { showAddModal.value = true }
function closeAddModal() { showAddModal.value = false }

function onModalSave(payload) {
  // Здесь можно отправить payload на сервер или добавить в список
  console.log('Saved from modal', payload)
  closeAddModal()
}
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Montserrat+Alternates:wght@400;600&display=swap');
html,body,#app{height:100%;margin:0}
body{font-family:Inter, system-ui, Arial, sans-serif;background:#F5F5F5}

.app-root{min-height:100vh;display:flex;flex-direction:column;align-items:center}

.seller-canvas{max-width:1920px;width:100%;min-height:calc(100vh - 164px);background:#F5F5F5;padding:40px 64px;box-sizing:border-box}

.products-section{width:100%;background:#FFFFFF;border-radius:30px;padding:30px;box-sizing:border-box}

.products-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:20px;padding-bottom:16px;border-bottom:1px solid #E0E0E0;gap:20px}

.products-title{font-family:'Montserrat Alternates', sans-serif;font-size:32px;margin:0;color:#111;font-weight:600}

.products-count{font-size:16px;color:#666;font-family:'Montserrat Alternates', sans-serif}

.add-product-btn{background:#52C41A;color:#FFFFFF;border:none;border-radius:30px;padding:12px 30px;font-size:16px;font-family:'Montserrat Alternates', sans-serif;font-weight:600;cursor:pointer;box-shadow:0 6px 16px rgba(82, 196, 26, 0.25);transition:all 0.3s ease;margin-left:auto}

.add-product-btn:hover{background:#45A517;box-shadow:0 8px 20px rgba(82, 196, 26, 0.35)}

.add-product-btn:active{transform:scale(0.98)}

.products-container{display:flex;flex-direction:row;gap:20px;flex-wrap:wrap;min-height:200px;align-items:flex-start;justify-content:flex-start;color:#999;padding-top:20px}

@media (max-width:768px){
  .seller-canvas{padding:20px 16px}
  .products-section{padding:20px}
  .products-header{flex-direction:column;gap:15px;align-items:flex-start}
  .add-product-btn{width:100%}
  .products-title{font-size:24px}
}

.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display:flex; align-items:center; justify-content:center; z-index:9999 }
.modal-window { width: min(1505px, 95vw); height: min(865px, 90vh); overflow:auto; background: transparent; border-radius: 18px; padding: 0; box-sizing: border-box; position: relative }
.modal-window :deep(.product-details) { width: 100%; height: 100%; margin: 0; }
</style>
