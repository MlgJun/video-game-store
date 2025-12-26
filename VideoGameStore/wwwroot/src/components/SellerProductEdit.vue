<template>
  <div class="product-edit">
    <div v-if="loading" class="loading">
      Загрузка данных...
    </div>
    
    <form v-else @submit.prevent="handleSubmit" class="product-details" enctype="multipart/form-data">
      <div class="left-section">
        <!-- Карточка игры -->
        <div class="vertical-photo-wrapper card-size">
          <img 
            v-if="previewCardImage" 
            :src="previewCardImage" 
            alt="Превью карточки" 
            class="vertical-photo" 
          />
          <img 
            v-else-if="form.imageUrl" 
            :src="form.imageUrl" 
            alt="Текущее изображение" 
            class="vertical-photo" 
          />
          <div v-else class="no-image">Нет изображения</div>
          
          <div class="photo-overlay">
            <label class="photo-add-btn" for="cardImageUpload">
              <span>+</span>
              <input 
                id="cardImageUpload"
                type="file" 
                accept="image/*" 
                @change="handleCardImageChange"
                style="display: none;"
              />
            </label>
          </div>
        </div>

        <p class="photo-label">Изображение карточки товара</p>

        <!-- Основное изображение -->
        <div class="photo-upload-placeholder">
          <label class="upload-label">
            Основное изображение:
            <input 
              type="file" 
              accept="image/*" 
              @change="handleMainImageChange"
              class="file-input"
            />
          </label>
          <div v-if="mainImageFile" class="file-name">
            {{ mainImageFile.name }}
          </div>
        </div>

        <!-- Файл с ключами (для создания/добавления) -->
        <div class="photo-upload-placeholder" v-if="!isEditMode">
          <label class="upload-label">
            Файл с ключами (JSON):
            <input 
              type="file" 
              accept=".json" 
              @change="handleKeysFileChange"
              class="file-input"
            />
          </label>
          <div v-if="keysFile" class="file-name">
            {{ keysFile.name }}
          </div>
        </div>

        <div class="seller-note-plain">
          Примечание для продавцов:<br />
          Выставляя товар, вы обязаны заполнить все поля товара, в противном случае ваш товар будет удален!
        </div>
        
        <!-- Количество в наличии -->
        <div class="stock-info" v-if="form.count !== undefined">
          <div class="field-label">Ключей в наличии:</div>
          <div class="stock-count">{{ form.count }}</div>
          <div class="add-keys-section" v-if="isEditMode">
            <label class="upload-label small">
              Добавить ключи:
              <input 
                type="file" 
                accept=".json" 
                @change="handleAdditionalKeysChange"
                class="file-input"
              />
            </label>
          </div>
        </div>
      </div>

      <div class="center-section">
        <!-- Основное изображение -->
        <div class="product-image-wrapper">
          <img 
            v-if="previewMainImage" 
            :src="previewMainImage" 
            alt="Превью основного изображения" 
            class="product-image" 
          />
          <img 
            v-else-if="form.imageUrl" 
            :src="form.imageUrl" 
            alt="Текущее изображение" 
            class="product-image" 
          />
          <div v-else class="no-image large">Нет изображения</div>
          
          <div class="photo-overlay large">
            <label class="photo-add-btn" for="mainImageUpload">
              <span>+</span>
              <input 
                id="mainImageUpload"
                type="file" 
                accept="image/*" 
                @change="handleMainImageChange"
                style="display: none;"
              />
            </label>
          </div>
        </div>
        
        <!-- Описание -->
        <div class="product-description-box">
          <label class="field-label">Описание:</label>
          <textarea 
            v-model="form.description" 
            class="product-description-input" 
            rows="4"
            placeholder="Подробное описание игры..."
            :maxlength="1000"
          ></textarea>
          <div class="char-counter">{{ form.description?.length || 0 }}/1000</div>
        </div>

        <!-- Цена -->
        <div class="price-section">
          <label class="field-label">Цена (₽):</label>
          <input 
            type="number" 
            v-model.number="form.price" 
            class="price-input" 
            min="0.1" 
            step="0.01"
            required
          />
          <div class="price-hint">Минимальная цена: 0.1 ₽</div>
        </div>
      </div>

      <div class="right-section">
        <!-- Название -->
        <input 
          v-model="form.title" 
          class="product-title-input" 
          placeholder="Название игры" 
          required
          minlength="2"
          maxlength="100"
        />

        <!-- Информация о продавце -->
        <div class="seller-info">
          <img src="/src/assets/icons/saleman.svg" alt="Продавец" class="seller-icon" />
          <span class="seller-name">{{ currentUser?.username || 'Продавец' }}</span>
        </div>

        <!-- Технические характеристики -->
        <div class="specs">
          <div class="spec-row">
            <div class="spec-label">Разработчик:</div>
            <input 
              class="spec-input" 
              v-model="form.developerTitle" 
              placeholder="Название разработчика"
              required
              minlength="2"
              maxlength="100"
            />
          </div>
          
          <div class="spec-row">
            <div class="spec-label">Издатель:</div>
            <input 
              class="spec-input" 
              v-model="form.publisherTitle" 
              placeholder="Название издателя"
              required
              minlength="2"
              maxlength="100"
            />
          </div>
          
          <div class="spec-row">
            <div class="spec-label">Жанры:</div>
            <div class="genres-input-container">
              <input 
                class="spec-input" 
                v-model="genreInput"
                placeholder="Введите жанры через запятую"
                @keydown.enter.prevent="addGenre"
              />
              <button type="button" class="add-genre-btn" @click="addGenre">+</button>
            </div>
          </div>
          
          <div class="genres-list" v-if="form.genres && form.genres.length > 0">
            <div v-for="(genre, index) in form.genres" :key="index" class="genre-tag">
              {{ genre.title }}
              <button type="button" @click="removeGenre(index)" class="remove-genre">×</button>
            </div>
          </div>
          
          <!-- Дополнительные поля -->
          <div class="spec-row" v-if="!isEditMode">
            <div class="spec-label">Платформа:</div>
            <select v-model="form.platform" class="spec-select">
              <option value="">Выберите платформу</option>
              <option value="PC">PC</option>
              <option value="PlayStation">PlayStation</option>
              <option value="Xbox">Xbox</option>
              <option value="Nintendo Switch">Nintendo Switch</option>
              <option value="Mobile">Mobile</option>
            </select>
          </div>
          
          <div class="spec-row">
            <div class="spec-label">Язык интерфейса:</div>
            <input 
              class="spec-input" 
              v-model="form.language" 
              placeholder="Русский, Английский и т.д."
            />
          </div>
        </div>

        <!-- Сообщения об ошибках/успехе -->
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
        
        <div v-if="successMessage" class="success-message">
          {{ successMessage }}
        </div>

        <!-- Кнопки действий -->
        <div class="buttons-section">
          <button type="submit" class="btn btn-save" :disabled="submitting">
            <span v-if="submitting">Сохранение...</span>
            <span v-else>{{ isEditMode ? 'Обновить игру' : 'Создать игру' }}</span>
          </button>
          
          <button type="button" class="btn btn-cancel" @click="handleCancel" :disabled="submitting">
            Отменить
          </button>
        </div>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
// Используем правильный путь к вашему store
import { useAuthStore } from '../stores/auth.js' // Или '@/stores/auth' если настроен алиас

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore?.() || { // Заглушка если store не найден
  isAuthenticated: false,
  userRole: null,
  user: { username: 'Продавец' }
}

const props = defineProps({
  gameId: {
    type: [Number, String],
    default: null
  }
})

const emit = defineEmits(['saved', 'canceled'])

// Режим редактирования или создания
const isEditMode = computed(() => props.gameId !== null)

// Состояние формы
const form = reactive({
  title: '',
  developerTitle: '',
  publisherTitle: '',
  price: 0.1,
  description: '',
  genres: [],
  language: '',
  platform: '',
  imageUrl: ''
})

// Файлы
const mainImageFile = ref(null)
const cardImageFile = ref(null)
const keysFile = ref(null)
const additionalKeysFile = ref(null)

// Превью изображений
const previewMainImage = ref('')
const previewCardImage = ref('')

// UI состояние
const loading = ref(false)
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const genreInput = ref('')

// Текущий пользователь
const currentUser = computed(() => {
  return authStore.user || { username: 'Продавец' }
})

// Загрузка данных игры для редактирования
const loadGame = async () => {
  if (!isEditMode.value) return
  
  loading.value = true
  try {
    // Заглушка для тестирования - используйте реальный API
    console.log('Загрузка игры ID:', props.gameId)
    
    // Если API нет, создаем мок-данные
    if (!import.meta.env.VITE_API_URL) {
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      // Мок-данные для тестирования
      const mockGame = {
        id: props.gameId,
        title: 'Пример игры',
        developerTitle: 'Пример разработчика',
        publisherTitle: 'Пример издателя',
        price: 1999,
        description: 'Пример описания игры',
        genres: [{ title: 'Экшен' }, { title: 'Приключения' }],
        language: 'Русский',
        platform: 'PC',
        imageUrl: 'https://via.placeholder.com/600x400',
        count: 10
      }
      
      Object.assign(form, mockGame)
      successMessage.value = 'Мок-данные загружены (API не настроен)'
      
    } else {
      // Реальный API запрос
      const response = await fetch(`/api/games/${props.gameId}`, {
        credentials: 'include'
      })
      
      if (!response.ok) throw new Error('Не удалось загрузить игру')
      
      const game = await response.json()
      
      // Заполняем форму
      Object.assign(form, {
        title: game.title,
        developerTitle: game.developerTitle,
        publisherTitle: game.publisherTitle,
        price: game.price,
        description: game.description || '',
        genres: game.genres || [],
        language: game.language || '',
        platform: game.platform || '',
        imageUrl: game.imageUrl,
        count: game.count
      })
    }
    
  } catch (error) {
    console.error('Ошибка загрузки:', error)
    errorMessage.value = 'Ошибка загрузки: ' + error.message
  } finally {
    loading.value = false
  }
}

// Обработчики файлов
const handleMainImageChange = (event) => {
  const file = event.target.files[0]
  if (!file) return
  
  // Проверка типа файла
  if (!file.type.startsWith('image/')) {
    errorMessage.value = 'Пожалуйста, выберите изображение'
    return
  }
  
  // Проверка размера (максимум 5MB)
  if (file.size > 5 * 1024 * 1024) {
    errorMessage.value = 'Изображение должно быть меньше 5MB'
    return
  }
  
  mainImageFile.value = file
  
  // Создаем превью
  const reader = new FileReader()
  reader.onload = (e) => {
    previewMainImage.value = e.target.result
  }
  reader.readAsDataURL(file)
}

const handleCardImageChange = (event) => {
  const file = event.target.files[0]
  if (!file) return
  
  if (!file.type.startsWith('image/')) {
    errorMessage.value = 'Пожалуйста, выберите изображение'
    return
  }
  
  if (file.size > 5 * 1024 * 1024) {
    errorMessage.value = 'Изображение должно быть меньше 5MB'
    return
  }
  
  cardImageFile.value = file
  
  const reader = new FileReader()
  reader.onload = (e) => {
    previewCardImage.value = e.target.result
  }
  reader.readAsDataURL(file)
}

const handleKeysFileChange = (event) => {
  const file = event.target.files[0]
  if (!file) return
  
  if (file.type !== 'application/json' && !file.name.endsWith('.json')) {
    errorMessage.value = 'Пожалуйста, выберите JSON файл'
    return
  }
  
  keysFile.value = file
}

const handleAdditionalKeysChange = (event) => {
  const file = event.target.files[0]
  if (!file) return
  
  if (file.type !== 'application/json' && !file.name.endsWith('.json')) {
    errorMessage.value = 'Пожалуйста, выберите JSON файл'
    return
  }
  
  additionalKeysFile.value = file
}

// Управление жанрами
const addGenre = () => {
  const genreTitle = genreInput.value.trim()
  if (!genreTitle) return
  
  // Разделяем по запятым если введено несколько
  const genres = genreTitle.split(',').map(g => g.trim()).filter(g => g)
  
  genres.forEach(genre => {
    if (!form.genres.some(g => g.title.toLowerCase() === genre.toLowerCase())) {
      form.genres.push({ title: genre })
    }
  })
  
  genreInput.value = ''
  errorMessage.value = ''
}

const removeGenre = (index) => {
  form.genres.splice(index, 1)
}

// Отправка формы
const handleSubmit = async () => {
  // Валидация
  if (!validateForm()) return
  
  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    const formData = new FormData()
    
    // Добавляем текстовые поля
    formData.append('Title', form.title)
    formData.append('DeveloperTitle', form.developerTitle)
    formData.append('PublisherTitle', form.publisherTitle)
    formData.append('Price', form.price.toString())
    formData.append('Description', form.description || '')
    
    // Жанры
    form.genres.forEach((genre, index) => {
      formData.append(`Genres[${index}].Title`, genre.title)
    })
    
    // Файлы
    if (mainImageFile.value) {
      formData.append('Image', mainImageFile.value)
    }
    
    if (cardImageFile.value) {
      formData.append('CardImage', cardImageFile.value)
    }
    
    // Для создания новой игры - ключи обязательны
    if (!isEditMode.value) {
      if (!keysFile.value) {
        throw new Error('Файл с ключами обязателен для создания игры')
      }
      formData.append('Keys', keysFile.value)
    }
    
    // Определяем URL и метод
    const url = isEditMode.value 
      ? `/api/games/${props.gameId}`
      : '/api/games'
    
    const method = isEditMode.value ? 'PUT' : 'POST'
    
    console.log('Отправка формы:', {
      url,
      method,
      title: form.title,
      genres: form.genres
    })
    
    // Имитация отправки если API нет
    if (!import.meta.env.VITE_API_URL) {
      await new Promise(resolve => setTimeout(resolve, 1500))
      
      const mockSavedGame = {
        id: isEditMode.value ? props.gameId : Math.floor(Math.random() * 1000) + 20,
        ...form
      }
      
      successMessage.value = isEditMode.value 
        ? 'Игра успешно обновлена! (тестовый режим)' 
        : 'Игра успешно создана! (тестовый режим)'
      
      // Перенаправление через 2 секунды
      setTimeout(() => {
        emit('saved', mockSavedGame)
        alert(successMessage.value)
        router.push('/sellerpage')
      }, 2000)
      
    } else {
      // Реальный API запрос
      const response = await fetch(url, {
        method,
        body: formData,
        credentials: 'include'
      })
      
      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Ошибка сохранения')
      }
      
      const savedGame = await response.json()
      
      // Если есть дополнительные ключи - добавляем их
      if (isEditMode.value && additionalKeysFile.value) {
        await addAdditionalKeys(savedGame.id)
      }
      
      successMessage.value = isEditMode.value 
        ? 'Игра успешно обновлена!' 
        : 'Игра успешно создана!'
      
      // Перенаправление через 2 секунды
      setTimeout(() => {
        emit('saved', savedGame)
        router.push(`/game/${savedGame.id}`)
      }, 2000)
    }
    
  } catch (error) {
    console.error('Ошибка отправки:', error)
    errorMessage.value = error.message
  } finally {
    submitting.value = false
  }
}

// Добавление дополнительных ключей
const addAdditionalKeys = async (gameId) => {
  const keysFormData = new FormData()
  keysFormData.append('Keys', additionalKeysFile.value)
  
  const response = await fetch(`/api/games/${gameId}/keys`, {
    method: 'POST',
    body: keysFormData,
    credentials: 'include'
  })
  
  if (!response.ok) {
    throw new Error('Не удалось добавить ключи')
  }
}

// Валидация формы
const validateForm = () => {
  errorMessage.value = ''
  
  if (!form.title || form.title.length < 2) {
    errorMessage.value = 'Название должно быть не менее 2 символов'
    return false
  }
  
  if (!form.developerTitle || form.developerTitle.length < 2) {
    errorMessage.value = 'Название разработчика должно быть не менее 2 символов'
    return false
  }
  
  if (!form.publisherTitle || form.publisherTitle.length < 2) {
    errorMessage.value = 'Название издателя должно быть не менее 2 символов'
    return false
  }
  
  if (form.price < 0.1) {
    errorMessage.value = 'Цена должна быть не менее 0.1 ₽'
    return false
  }
  
  if (form.genres.length === 0) {
    errorMessage.value = 'Добавьте хотя бы один жанр'
    return false
  }
  
  if (!isEditMode.value && !mainImageFile.value) {
    errorMessage.value = 'Загрузите основное изображение'
    return false
  }
  
  if (!isEditMode.value && !keysFile.value) {
    errorMessage.value = 'Загрузите файл с ключами'
    return false
  }
  
  return true
}

// Отмена
const handleCancel = () => {
  emit('canceled')
  if (isEditMode.value && props.gameId) {
    router.push(`/game/${props.gameId}`)
  } else {
    router.push('/sellerpage')
  }
}

// Проверка авторизации при монтировании
const checkAuth = () => {
  if (!authStore.isAuthenticated || authStore.userRole !== 'SELLER') {
    console.warn('Требуется авторизация продавца')
    // router.push('/login') // Раскомментируйте когда будет готова авторизация
    return false
  }
  return true
}

// Инициализация
onMounted(() => {
  // Временно отключаем проверку для тестирования
  // if (!checkAuth()) return
  
  loadGame()
})

// При изменении gameId
watch(() => props.gameId, () => {
  loadGame()
})
</script>

<style scoped> 

.product-edit {
  width: 100%;
  max-width: 1505px;
  margin: 0 auto;
  padding: 20px;
}

.loading {
  text-align: center;
  padding: 100px;
  font-size: 24px;
  color: #666;
  font-family: 'Montserrat Alternates', sans-serif;
}

.no-image {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  background: #f0f0f0;
  color: #999;
  font-size: 16px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.no-image.large {
  font-size: 24px;
}

.upload-label {
  display: block;
  margin-bottom: 8px;
  color: #666;
  font-size: 14px;
  cursor: pointer;
  font-family: 'Montserrat Alternates', sans-serif;
}

.upload-label.small {
  font-size: 12px;
}

.file-input {
  display: block;
  margin-top: 4px;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 6px;
  width: 100%;
  font-family: 'Montserrat Alternates', sans-serif;
}

.file-name {
  font-size: 12px;
  color: #666;
  margin-top: 4px;
  word-break: break-all;
  font-family: 'Montserrat Alternates', sans-serif;
}

.stock-info {
  background: #E6E6E6;
  padding: 12px;
  border-radius: 15px;
  width: 100%;
  margin-top: 10px;
}

.stock-count {
  font-size: 24px;
  font-weight: bold;
  color: #52C41A;
  text-align: center;
  margin: 8px 0;
  font-family: 'Montserrat Alternates', sans-serif;
}

.add-keys-section {
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px solid #ddd;
}

.char-counter {
  text-align: right;
  font-size: 12px;
  color: #999;
  margin-top: 4px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.price-hint {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.genres-input-container {
  display: flex;
  gap: 8px;
  width: 100%;
}

.add-genre-btn {
  background: #A53DFF;
  color: white;
  border: none;
  border-radius: 8px;
  width: 40px;
  height: 40px;
  cursor: pointer;
  font-size: 20px;
  flex-shrink: 0;
  font-family: 'Montserrat Alternates', sans-serif;
  transition: background 0.3s;
}

.add-genre-btn:hover {
  background: #8B1FD9;
}

.genres-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 10px;
}

.genre-tag {
  background: #A53DFF;
  color: white;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 6px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.remove-genre {
  background: none;
  border: none;
  color: white;
  cursor: pointer;
  font-size: 16px;
  padding: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: 'Montserrat Alternates', sans-serif;
}

.remove-genre:hover {
  color: #ffcccc;
}

.spec-select {
  width: 220px;
  padding: 12px 14px;
  border-radius: 12px;
  border: 1px solid rgba(0,0,0,0.08);
  background: #E6E6E6;
  font-size: 16px;
  font-family: 'Montserrat Alternates', sans-serif;
}

.error-message {
  background: #FFF2E8;
  border: 1px solid #FFBB96;
  color: #FA541C;
  padding: 12px;
  border-radius: 10px;
  margin: 10px 0;
  text-align: center;
  font-family: 'Montserrat Alternates', sans-serif;
}

.success-message {
  background: #F6FFED;
  border: 1px solid #B7EB8F;
  color: #52C41A;
  padding: 12px;
  border-radius: 10px;
  margin: 10px 0;
  text-align: center;
  font-family: 'Montserrat Alternates', sans-serif;
  animation: fadeIn 0.5s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Стили из оригинального компонента */
.product-details {
  width: 1505px;
  height: 865px;
  background: #EFEFEF;
  border: 8px solid #A53DFF;
  border-radius: 30px;
  display: flex;
  gap: 50px;
  padding: 30px;
  box-sizing: border-box;
  font-family: 'Montserrat Alternates', sans-serif;
}

.left-section {
  flex: 0 0 280px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  align-items: flex-start;
}

.vertical-photo-wrapper.card-size {
  width: 280px;
  height: 350px;
  background: #E6E6E6;
  border-radius: 30px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.vertical-photo {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.photo-upload-placeholder { 
  height:40px; 
  width:100%; 
  box-sizing:border-box 
}

.photo-overlay { 
  position: absolute; 
  inset: 0; 
  display:flex; 
  align-items:center; 
  justify-content:center; 
  background: rgba(0,0,0,0.04); 
  pointer-events: none 
}

.photo-add-btn { 
  pointer-events: auto; 
  background: transparent; 
  border: none; 
  color: #A53DFF; 
  font-size: 38px; 
  line-height:1; 
  cursor: pointer; 
  padding: 4px 8px; 
  transition: transform 0.12s ease, color 0.12s ease 
}

.photo-add-btn:hover { 
  transform: scale(1.06); 
  color: #5c00c5 
}

.photo-label { 
  margin: 0; 
  font-size: 16px; 
  color: #666; 
  align-self: center; 
  text-align: center; 
}

.seller-note-plain { 
  margin-top: 100px; 
  color: #666; 
  font-size: 24px; 
  line-height:1.3 
}

.center-section {
  flex: 0 0 612px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.product-image-wrapper {
  width: 612px;
  height: 428px;
  background: #E6E6E6;
  border-radius: 30px;
  overflow: hidden;
  position: relative;
}

.product-image { 
  width:620; 
  height:430; 
}

.product-description-box { 
  width:100%; 
  background: transparent; 
}

.field-label { 
  display:block; 
  margin-bottom:6px; 
  color:#666;
  font-family: 'Montserrat Alternates', sans-serif;
}

.product-description-input { 
  width:590px; 
  height: 115px; 
  padding:12px; 
  border-radius:15px;
  background:#E6E6E6; 
  font-size: 20px; 
  font-family: 'Montserrat Alternates', sans-serif;
  border: none;
  resize: vertical;
}

.price-section { 
  margin-top: 10px; 
  background: transparent; 
  padding:0 
}

.price-input { 
  width:288px; 
  height: 113px; 
  padding:8px 12px; 
  border-radius:15px; 
  font-size: 64px ;
  color:#A53DFF;
  background:#E6E6E6;
  border: none;
  font-family: 'Montserrat Alternates', sans-serif;
}

.right-section { 
  flex: 0 0 372px; 
  display: flex; 
  flex-direction: column; 
  gap:20px; 
  padding:0; 
  box-sizing: border-box; 
  align-items: flex-start 
}

.product-title-input { 
  width:100%; 
  height:73px; 
  border-radius:15px; 
  background:#E6E6E6; 
  font-size:28px; 
  color:#A53DFF; 
  box-sizing: border-box; 
  text-align: center; 
  font-family: 'Montserrat Alternates', sans-serif ; 
  font-weight: bold;
  border: none;
  padding: 0 20px;
}

.seller-info { 
  display:flex; 
  width:100%; 
  justify-content:center; 
  align-items:center; 
  gap:8px; 
  padding:6px 0; 
  margin-top:8px 
}

.seller-icon { 
  width:21px; 
  height:21px 
}

.specs { 
  flex:1; 
  display:flex; 
  flex-direction:column; 
  gap:18px; 
  padding:0; 
  border-radius:15px 
}

.spec-row { 
  display:flex; 
  justify-content:space-between; 
  align-items:center; 
  gap:12px; 
  font-size:18px 
}

.spec-label { 
  padding:0; 
  min-width:140px; 
  color:#666; 
  font-weight:600; 
  text-align:left 
}

.spec-input { 
  width:220px; 
  flex: 0 0 220px; 
  padding:12px 14px; 
  border-radius:12px; 
  border:1px solid rgba(0,0,0,0.08); 
  background:#E6E6E6; 
  font-size:16px; 
  font-family: 'Montserrat Alternates', sans-serif;
  border: none;
}

.buttons-section { 
  display:flex; 
  flex-direction:column; 
  gap:16px; 
  align-items:center; 
  margin-top:auto 
}

.btn { 
  width:435px; 
  height:80px; 
  border:none; 
  border-radius:15px; 
  font-size:32px; 
  font-family:'Montserrat Alternates', sans-serif; 
  font-weight:600; 
  cursor:pointer; 
  transition: transform 0.12s ease, box-shadow 0.12s ease 
}

.btn-save { 
  background:#A53DFF; 
  color:#fff; 
  box-shadow:0 8px 20px rgba(165,61,255,0.25) 
}

.btn-cancel { 
  background:#0962C9; 
  color:#fff; 
  box-shadow:0 8px 20px rgba(9,98,201,0.25) 
}

.btn:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 12px 24px rgba(0,0,0,0.2);
}

.btn:active { 
  transform: translateY(3px) scale(0.995); 
  box-shadow: 0 6px 14px rgba(0,0,0,0.12) 
}

/* Адаптивность */
@media (max-width: 1400px) {
  .product-edit {
    max-width: 100%;
  }
  
  .product-details {
    flex-direction: column;
    height: auto;
    gap: 30px;
    width: 100%;
    max-width: 100%;
    padding: 20px;
  }
  
  .left-section,
  .center-section,
  .right-section {
    width: 100%;
    max-width: 100%;
  }
  
  .vertical-photo-wrapper.card-size {
    width: 100%;
    max-width: 280px;
    margin: 0 auto;
  }
  
  .product-image-wrapper {
    width: 100%;
    height: auto;
    aspect-ratio: 4/3;
  }
  
  .product-description-input {
    width: 100%;
  }
  
  .price-input {
    width: 100%;
    max-width: 288px;
  }
  
  .btn {
    width: 100%;
    max-width: 435px;
  }
}

@media (max-width: 768px) {
  .product-details {
    padding: 20px;
    gap: 20px;
  }
  
  .spec-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .spec-input,
  .spec-select {
    width: 100%;
  }
  
  .genres-input-container {
    width: 100%;
  }
}
</style>