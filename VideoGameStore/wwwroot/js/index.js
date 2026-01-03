// js/index.js (упрощенная версия)
class GameStore {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 20;
        this.filters = {
            minPrice: null,
            maxPrice: null,
            gameTitle: '',
            genres: []
        };
        this.selectedGenres = [];
        this.allGenres = [];

        this.init();
    }

    async init() {
        await this.loadGenres();
        this.setupEventListeners();
        await this.loadGames();
    }

    // Настройка слушателей событий для поиска и очистки

    setupEventListeners() {
        const searchInput = document.getElementById('searchInput');
        const clearBtn = document.getElementById('clearSearchBtn');

        if (searchInput) {
            let debounce;
            searchInput.addEventListener('input', (e) => {
                clearTimeout(debounce);
                debounce = setTimeout(async () => {
                    this.filters.gameTitle = e.target.value.trim();
                    this.currentPage = 1;
                    await this.loadGames();
                }, 350);
            });
        }

        if (clearBtn) {
            clearBtn.addEventListener('click', async () => {
                if (searchInput) searchInput.value = '';
                this.filters.gameTitle = '';
                this.currentPage = 1;
                await this.loadGames();
            });
        }

        const applyFilterBtn = document.getElementById('apply-filter');
        const resetFilterBtn = document.getElementById('reset-filter');
        const minPriceInput = document.getElementById('min-price');
        const maxPriceInput = document.getElementById('max-price');

        if (applyFilterBtn) {
            applyFilterBtn.addEventListener('click', async () => {
                this.applyPriceFilter();
            });
        }

        if (resetFilterBtn) {
            resetFilterBtn.addEventListener('click', async () => {
                if (minPriceInput) minPriceInput.value = '';
                if (maxPriceInput) maxPriceInput.value = '';
                this.filters.minPrice = null;
                this.filters.maxPrice = null;
                this.currentPage = 1;
                await this.loadGames();
                this.updateFilterInfoText();
            });
        }

        // Обработка Enter в полях цены
        if (minPriceInput) {
            minPriceInput.addEventListener('keypress', async (e) => {
                if (e.key === 'Enter') this.applyPriceFilter();
            });
        }
        if (maxPriceInput) {
            maxPriceInput.addEventListener('keypress', async (e) => {
                if (e.key === 'Enter') this.applyPriceFilter();
            });
        }

        // Сброс всех фильтров
        const clearAllFiltersBtn = document.getElementById('clearAllFiltersBtn');
        if (clearAllFiltersBtn) {
            clearAllFiltersBtn.addEventListener('click', async () => {
                // Сброс цен
                if (minPriceInput) minPriceInput.value = '';
                if (maxPriceInput) maxPriceInput.value = '';
                this.filters.minPrice = null;
                this.filters.maxPrice = null;

                // Сброс жанров
                this.selectedGenres = [];
                this.updateGenreButtons();

                this.currentPage = 1;
                await this.loadGames();
                this.updateFilterInfoText();
            });
        }

        // Обработчик кнопки применения жанров
        const applyGenresBtn = document.getElementById('applyGenresBtn');
        const closeGenresBtn = document.getElementById('closeGenresBtn');
        const clearGenresBtn = document.getElementById('clearGenresBtn');
        const openGenresBtn = document.getElementById('openGenresBtn');
        const genresOverlay = document.getElementById('genresOverlay');

        if (applyGenresBtn) {
            applyGenresBtn.addEventListener('click', async () => {
                // Скрыть оверлей жанров
                if (genresOverlay) genresOverlay.style.display = 'none';
                // Загрузить игры с выбранными жанрами
                this.currentPage = 1;
                await this.loadGames();
                // Обновить отображение активных фильтров
                this.updateActiveGenresDisplay();
            });
        }

        // Закрытие меню жанров без применения
        if (closeGenresBtn) {
            closeGenresBtn.addEventListener('click', () => {
                if (genresOverlay) genresOverlay.style.display = 'none';
            });
        }

        // Открытие меню жанров
        if (openGenresBtn) {
            openGenresBtn.addEventListener('click', () => {
                if (genresOverlay) genresOverlay.style.display = 'flex';
            });
        }

        // Сброс выбранных жанров в меню
        if (clearGenresBtn) {
            clearGenresBtn.addEventListener('click', () => {
                this.selectedGenres = [];
                this.updateGenreButtons();
            });
        }
    }

    // Обновление отображения активных жанров под фильтрами
    updateActiveGenresDisplay() {
        const activeFiltersContainer = document.getElementById('activeFilters');
        const filterTagsContainer = document.getElementById('filterTags');

        if (!activeFiltersContainer || !filterTagsContainer) return;

        // Показываем/скрываем блок активных фильтров
        if (this.selectedGenres.length > 0) {
            activeFiltersContainer.classList.remove('hidden');
            // Очищаем и пересоздаем теги
            filterTagsContainer.innerHTML = '';
            this.selectedGenres.forEach(genre => {
                const tag = document.createElement('span');
                tag.className = 'filter-tag selected';
                tag.textContent = genre;

                const removeBtn = document.createElement('button');
                removeBtn.className = 'remove-filter';
                removeBtn.textContent = '×';
                removeBtn.addEventListener('click', async (e) => {
                    e.stopPropagation();
                    const index = this.selectedGenres.indexOf(genre);
                    if (index > -1) {
                        this.selectedGenres.splice(index, 1);
                        this.updateActiveGenresDisplay();
                        this.updateGenreButtons();
                        this.currentPage = 1;
                        await this.loadGames();
                    }
                });

                tag.appendChild(removeBtn);
                filterTagsContainer.appendChild(tag);
            });
        } else {
            activeFiltersContainer.classList.add('hidden');
        }
    }

    async applyPriceFilter() {
        const minPriceInput = document.getElementById('min-price');
        const maxPriceInput = document.getElementById('max-price');
        const applyFilterBtn = document.getElementById('apply-filter');

        if (!minPriceInput || !maxPriceInput) return;

        try {
            let minVal = minPriceInput.value.trim();
            let maxVal = maxPriceInput.value.trim();

            this.filters.minPrice = minVal ? parseFloat(minVal) : null;
            this.filters.maxPrice = maxVal ? parseFloat(maxVal) : null;

            // Валидация
            if (this.filters.minPrice !== null && isNaN(this.filters.minPrice)) {
                alert('Минимальная цена должна быть числом');
                return;
            }

            if (this.filters.maxPrice !== null && isNaN(this.filters.maxPrice)) {
                alert('Максимальная цена должна быть числом');
                return;
            }

            if (this.filters.minPrice !== null && this.filters.maxPrice !== null &&
                this.filters.minPrice > this.filters.maxPrice) {
                alert('Минимальная цена не может быть больше максимальной');
                return;
            }

            this.currentPage = 1;
            await this.loadGames();
            this.updateFilterInfoText();
        } catch (error) {
            console.error('Error applying price filter:', error);
            alert('Ошибка при применении фильтра цены');
        }
    }

    // ДОБАВИТЬ ЭТОТ МЕТОД:
    updateFilterInfoText() {
        const filterRangeText = document.getElementById('filterRangeText');
        if (!filterRangeText) return;

        let rangeText = 'от 0 до ∞ ₽';

        if (this.filters.minPrice !== null || this.filters.maxPrice !== null) {
            const min = this.filters.minPrice !== null ? this.filters.minPrice.toLocaleString('ru-RU') : '0';
            const max = this.filters.maxPrice !== null ? this.filters.maxPrice.toLocaleString('ru-RU') : '∞';
            rangeText = `от ${min} до ${max} ₽`;
        }

        filterRangeText.textContent = rangeText;
    }

    async loadGenres() {
        try {
            const genres = await api.getGenres();
            this.allGenres = genres.map(g => g.title);
            this.renderGenreFilters();
            localStorage.setItem("genres", JSON.stringify(this.allGenres));
        } catch (error) {
            console.error('Failed to load genres:', error);
        }
    }

    renderGenreFilters() {
        const container = document.getElementById('genresList');
        if (!container) return;

        container.innerHTML = '';
        (this.allGenres || []).forEach(genre => {
            const label = document.createElement('label');
            label.className = 'genre-checkbox';

            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.value = genre;
            checkbox.checked = this.selectedGenres.includes(genre);
            checkbox.addEventListener('change', () => {
                if (checkbox.checked) {
                    if (!this.selectedGenres.includes(genre)) {
                        this.selectedGenres.push(genre);
                    }
                } else {
                    const index = this.selectedGenres.indexOf(genre);
                    if (index > -1) {
                        this.selectedGenres.splice(index, 1);
                    }
                }
                this.updateGenreButtons();
            });

            const span = document.createElement('span');
            span.textContent = genre;

            label.appendChild(checkbox);
            label.appendChild(span);
            container.appendChild(label);
        });
    }

    updateGenreButtons() {
        // Обновляем кнопку применения в меню жанров
        const applyBtn = document.getElementById('applyGenresBtn');
        const clearBtn = document.getElementById('clearGenresBtn');
        if (applyBtn) applyBtn.textContent = `Применить (${this.selectedGenres.length})`;
        if (clearBtn) clearBtn.disabled = this.selectedGenres.length === 0;

        // Обновляем чекбоксы в меню жанров
        const checkboxes = document.querySelectorAll('#genresList input[type="checkbox"]');
        checkboxes.forEach(checkbox => {
            checkbox.checked = this.selectedGenres.includes(checkbox.value);
        });

        // Обновляем отображение активных фильтров
        this.updateActiveGenresDisplay();
    }

    // Обновление информации о фильтре и пагинации
    updateFilterInfo(response) {
        if (!response) return;
        const total = response.totalElements || response.TotalElements || response.total || 0;
        const pageNumber = response.pageNumber || response.PageNumber || response.page || response.Page || this.currentPage;
        const totalPages = response.totalPages || response.TotalPages || response.TotalPages || 1;

        const filterCountText = document.getElementById('filterCountText');
        if (filterCountText) filterCountText.textContent = `(найдено ${total} товаров)`;

        this.updateFilterInfoText();
    }

    async loadGames() {
        const loadingSpinner = document.getElementById('loading-spinner');
        const noResults = document.getElementById('no-results');
        const productsGrid = document.getElementById('products-grid');
        const pageNumbersContainer = document.getElementById('page-numbers');

        if (productsGrid) productsGrid.innerHTML = '';
        if (pageNumbersContainer) pageNumbersContainer.innerHTML = '';

        if (loadingSpinner) loadingSpinner.style.display = 'flex';
        if (noResults) noResults.style.display = 'none';

        try {
            const filters = {
                minPrice: this.filters.minPrice ? parseFloat(this.filters.minPrice) : null,
                maxPrice: this.filters.maxPrice ? parseFloat(this.filters.maxPrice) : null,
                gameTitle: this.filters.gameTitle || null,
                genres: this.selectedGenres.length > 0 ? this.selectedGenres : null
            };

            const response = await api.getGames(filters, this.currentPage, this.pageSize);

            this.updateFilterInfo(response);

            if (productsGrid) productsGrid.innerHTML = '';

            if (response.content && response.content.length > 0) {
                response.content.forEach(game => this.renderGameCard(game));
            } else {
                if (noResults) noResults.style.display = 'block';
            }

            this.renderPagination(response);
        } catch (error) {
            console.error('Failed to load games:', error);
            if (noResults) {
                noResults.textContent = 'Ошибка загрузки товаров';
                noResults.style.display = 'block';
            }

            this.renderPagination({totalPages: 0, pageNumber: 1 });
        } finally {
            if (loadingSpinner) loadingSpinner.style.display = 'none';
        }
    }


    renderGameCard(game) {
        const productsGrid = document.getElementById('products-grid');
        if (!productsGrid) return;

        const card = document.createElement('div');
        card.className = 'game-card';

        const title = game.title || game.Title || 'Без названия';
        const price = (game.price !== undefined && game.price !== null) ? game.price : (game.Price || 0);
        const img = game.imageUrl || game.ImageUrl || '/api/placeholder/game';
        const dev = game.developerTitle || game.DeveloperTitle || '';
        const desc = game.description;

        card.innerHTML = `
            <div class="game-image">
                <img src="${img}" alt="${title}" loading="lazy">
            </div>
            <div class="game-info">
                <h3 class="game-title">${title}</h3>
                <div class="game-price">${price.toLocaleString('ru-RU')} ₽</div>
                <div class="game-dev">${dev}</div>
            </div>
        `;

        // Клик открывает модалку просмотра
        card.addEventListener('click', () => {
            const overlay = document.getElementById('productViewOverlay');
            if (!overlay) return;

            // Заполнение модалки (ваш код без изменений)
            const viewTitle = document.getElementById('viewTitle');
            const viewDescription = document.getElementById('viewDescription');
            const viewPrice = document.getElementById('viewPrice');
            const viewDeveloper = document.getElementById('viewDeveloper');
            const viewPublisher = document.getElementById('viewPublisher');
            const viewGenresContainer = document.getElementById('viewGenresContainer');
            const viewMainImage = document.getElementById('viewMainImage');
            const viewCount = document.getElementById('viewCount');

            if (viewTitle) viewTitle.textContent = title;
            if (viewDescription) viewDescription.textContent = desc || 'Описание отсутствует';
            if (viewPrice) viewPrice.textContent = price;
            if (viewDeveloper) viewDeveloper.textContent = game.developerTitle || game.DeveloperTitle || '';
            if (viewPublisher) viewPublisher.textContent = game.publisherTitle || game.PublisherTitle || '';
            if (viewCount) viewCount.textContent = game.count || game.Count || 0;

            if (viewGenresContainer) {
                viewGenresContainer.innerHTML = '';
                const genres = game.genres || game.Genres || [];
                genres.forEach(g => {
                    const span = document.createElement('span');
                    span.className = 'view-genre';
                    span.textContent = g.title || g.Title || g;
                    viewGenresContainer.appendChild(span);
                });
            }

            if (viewMainImage) {
                if (img && img !== '/placeholder.jpg') {
                    viewMainImage.src = img;
                    viewMainImage.style.display = 'block';
                } else {
                    viewMainImage.style.display = 'none';
                }
            }

            overlay.style.display = 'flex';

            // Кнопки модалки
            const addBtn = document.getElementById('addToCartBtn');
            if (addBtn) {
                addBtn.onclick = async () => {
                    await this.addToCart(game.id || game.Id || game.gameId || game.GameId);
                };
            }

            document.querySelectorAll('[id^="closeProductViewBtn"]').forEach(btn => {
                btn.onclick = () => overlay.style.display = 'none';
            });
        });

        productsGrid.appendChild(card);
    }


    async addToCart(gameId) {
        if (!auth.currentUser || auth.currentUser.role !== 'CUSTOMER') {
            alert('Пожалуйста, войдите как покупатель для добавления в корзину');
            return;
        }

        try {
            await api.addToCart(gameId, 1);
            await auth.updateCartBadge();
            alert('Товар добавлен в корзину!');
        } catch (error) {
            console.error('Failed to add to cart:', error);
            alert('Не удалось добавить товар в корзину');
        }
    }

    renderPagination(response) {
        const pageNumbersContainer = document.getElementById('page-numbers');
        if (!pageNumbersContainer) return;

        // Очистка — обязательно!
        pageNumbersContainer.innerHTML = '';

        const pageNumber = response.pageNumber || response.PageNumber || response.page || this.currentPage;
        const totalPages = response.totalPages || response.TotalPages || response.totalPages || 1;

        // Если страниц 0 или 1 — не показываем пагинацию
        if (!totalPages || totalPages <= 1) {
            pageNumbersContainer.style.display = 'none';
            return;
        }

        pageNumbersContainer.style.display = 'flex'; // или 'block', как у вас

        // Создаём кнопки
        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.className = 'page-number';
            pageBtn.textContent = i;
            if (i === pageNumber) {
                pageBtn.classList.add('active');
            }

            pageBtn.addEventListener('click', async () => {
                if (this.currentPage !== i) {
                    this.currentPage = i;
                    await this.loadGames();
                }
            });

            pageNumbersContainer.appendChild(pageBtn);
        }
    }

    hidePagination() {
        const prev = document.getElementById('prev-page');
        const next = document.getElementById('next-page');
        if (prev) prev.disabled = true;
        if (next) next.disabled = true;
    }

}

document.addEventListener('DOMContentLoaded', () => {
    window.gameStore = new GameStore();
});