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

    // Рендер доступных жанров в блоке фильтров (строка над товарами)
    renderGenreFilters() {
        const container = document.getElementById('filterTags');
        if (!container) return;
        container.innerHTML = '';
        (this.allGenres || []).forEach(g => {
            const title = g || (g.title || g.Title);
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'filter-tag';
            btn.textContent = title;
            btn.dataset.genre = title;
            btn.addEventListener('click', async () => {
                const idx = this.selectedGenres.indexOf(title);
                if (idx === -1) this.selectedGenres.push(title);
                else this.selectedGenres.splice(idx, 1);
                this.updateGenreButtons();
                this.currentPage = 1;
                await this.loadGames();
            });
            container.appendChild(btn);
        });
    }

    // Обновление состояния кнопок жанров и информации о фильтре
    updateGenreButtons() {
        // Update small tags
        document.querySelectorAll('#filterTags .filter-tag').forEach(btn => {
            const g = btn.dataset.genre;
            if (this.selectedGenres.includes(g)) btn.classList.add('selected');
            else btn.classList.remove('selected');
        });

        // Update apply button text and clear button
        const applyBtn = document.getElementById('applyGenresBtn');
        const clearBtn = document.getElementById('clearGenresBtn');
        if (applyBtn) applyBtn.textContent = `Применить (${this.selectedGenres.length})`;
        if (clearBtn) clearBtn.disabled = this.selectedGenres.length === 0;
    }

    // Обновление информации о фильтре и пагинации
    updateFilterInfo(response) {
        if (!response) return;
        const total = response.totalElements || response.TotalElements || response.total || 0;
        const pageNumber = response.pageNumber || response.PageNumber || response.page || response.Page || this.currentPage;
        const totalPages = response.totalPages || response.TotalPages || response.TotalPages || 1;

        const filterCountText = document.getElementById('filterCountText');
        if (filterCountText) filterCountText.textContent = `(найдено ${total} товаров)`;

        const currentPageEl = document.getElementById('current-page');
        const totalPagesEl = document.getElementById('total-pages');
        if (currentPageEl) currentPageEl.textContent = pageNumber;
        if (totalPagesEl) totalPagesEl.textContent = totalPages;
    }

    async loadGames() {
        const loadingSpinner = document.getElementById('loading-spinner');
        const noResults = document.getElementById('no-results');
        const productsGrid = document.getElementById('products-grid');

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
                this.renderPagination(response);
            } else {
                if (noResults) noResults.style.display = 'block';
                this.hidePagination();
            }
        } catch (error) {
            console.error('Failed to load games:', error);
            if (noResults) {
                noResults.textContent = 'Ошибка загрузки товаров';
                noResults.style.display = 'block';
            }
        } finally {
            if (loadingSpinner) loadingSpinner.style.display = 'none';
        }
    }


    // Рендер карточки игры в сетке
    renderGameCard(game) {
        const productsGrid = document.getElementById('products-grid');
        if (!productsGrid) return;

        const card = document.createElement('div');
        card.className = 'product-card';

        const title = game.title || game.Title || '';
        const price = (game.price !== undefined && game.price !== null) ? game.price : (game.Price || 0);
        const img = game.imageUrl || game.ImageUrl || game.image || '';
        const desc = game.description || game.Description || '';

        card.innerHTML = `
            <div class="product-image">
                ${img ? `<img src="${img}" alt="${title}">` : '<div class="no-image">Нет изображения</div>'}
            </div>
            <div class="product-info">
                <div class="product-title">${title}</div>
                <div class="product-price">${price} ₽</div>
            </div>
        `;

        card.addEventListener('click', () => {
            // Открыть модальное окно просмотра товара и заполнить данные
            const overlay = document.getElementById('productViewOverlay');
            if (!overlay) return;

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
                (genres || []).forEach(g => {
                    const span = document.createElement('span');
                    span.className = 'view-genre';
                    span.textContent = (g.title || g.Title || g);
                    viewGenresContainer.appendChild(span);
                });
            }

            if (viewMainImage) {
                if (img) {
                    viewMainImage.src = img;
                    viewMainImage.style.display = 'block';
                } else {
                    viewMainImage.style.display = 'none';
                }
            }

            // Open overlay
            overlay.style.display = 'flex';

            const addBtn = document.getElementById('addToCartBtn');
            if (addBtn) {
                addBtn.onclick = async () => {
                    await this.addToCart(game.id || game.Id || game.gameId || game.GameId);
                };
            }

            const closeBtn = document.getElementById('closeProductViewBtn1');
            closeBtn.onclick = () => {
                overlay.style.display = 'none';
            };

            const closeBtn2 = document.getElementById('closeProductViewBtn2');
            closeBtn2.onclick = () => {
                overlay.style.display = 'none';
            };
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
        const prev = document.getElementById('prev-page');
        const next = document.getElementById('next-page');
        const pageNumber = response.pageNumber || response.PageNumber || response.page || this.currentPage;
        const totalPages = response.totalPages || response.TotalPages || 1;

        if (prev) {
            prev.disabled = pageNumber <= 1;
            prev.onclick = async () => { if (pageNumber > 1) { this.currentPage = pageNumber - 1; await this.loadGames(); } };
        }
        if (next) {
            next.disabled = pageNumber >= totalPages;
            next.onclick = async () => { if (pageNumber < totalPages) { this.currentPage = pageNumber + 1; await this.loadGames(); } };
        }
    }

    hidePagination() {
        const prev = document.getElementById('prev-page');
        const next = document.getElementById('next-page');
        if (prev) prev.disabled = true;
        if (next) next.disabled = true;
        const currentPageEl = document.getElementById('current-page');
        const totalPagesEl = document.getElementById('total-pages');
        if (currentPageEl) currentPageEl.textContent = '1';
        if (totalPagesEl) totalPagesEl.textContent = '1';
    }

}

document.addEventListener('DOMContentLoaded', () => {
    window.gameStore = new GameStore();
});