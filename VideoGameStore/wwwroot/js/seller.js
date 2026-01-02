class SellerDashboard {
    constructor() {
        this.editingId = null;
        this.editingGame = null;
        this.init();
    }

    async init() {
        await auth.init();
        if (auth.currentUser?.role !== 'SELLER') {
            window.location.href = 'index.html';
            return;
        }

        // UI элементы
        this.ui = {
            menuToggle: document.getElementById('sellerMenuToggle'),
            menu: document.getElementById('sellerMenu'),
            modal: document.getElementById('gameModal'),
            form: document.getElementById('gameForm'),
            products: document.getElementById('productsGrid'),
            gamesCount: document.getElementById('gamesCount'),
            addBtn: document.getElementById('addGameBtn')
        };

        // Event listeners
        this.ui.menuToggle.onclick = () => this.toggleMenu();
        document.getElementById('closeModal').onclick = () => this.closeModal();
        document.getElementById('cancelGame').onclick = () => this.closeModal();
        this.ui.form.onsubmit = (e) => this.saveGame(e);
        document.getElementById('logoutBtn').onclick = () => auth.logout();
        document.getElementById('addGameBtn').onclick = () => this.openModal();

        // Инициализация пользователя
        this.updateUserInfo();

        // Загрузка данных
        this.loadGames();
    }

    updateUserInfo() {
        const user = auth.currentUser;
        if (user) {
            document.getElementById('sellerName').textContent = user.name || 'Продавец';
            document.getElementById('sellerEmail').textContent = user.email || '-';
        }
    }

    toggleMenu() {
        this.ui.menu.classList.toggle('active');
    }

    // ===== ЖАНРЫ =====
    getGenres() {
        try {
            const genres = localStorage.getItem('genres');
            if (!genres) {
                const fallback = [
                    "Шутер от первого лица", "Шутер от третьего лица", "Стратегии и тактические ролевые",
                    "Симуляторы строительства и автоматизации", "Симуляторы хобби и работы",
                    "Казуальные", "Рогалики", "Карточные и настольные", "Пошаговые стратегии",
                    "Научная фантастика", "Головоломки", "Спортивные симуляторы",
                    "Хорроры", "Гонки", "Выживание", "Башенная защита"
                ];
                localStorage.setItem('genres', JSON.stringify(fallback));
                return fallback;
            }
            return JSON.parse(genres);
        } catch (e) {
            console.error('Ошибка жанров:', e);
            return [];
        }
    }

    initGenresEditor() {
        const genres = this.getGenres();
        const genresList = document.getElementById('genresList');

        genresList.innerHTML = genres.map(genreTitle => `
            <label class="genre-checkbox">
                <input type="checkbox" name="genres[]" value="${genreTitle}">
                <span>${genreTitle}</span>
            </label>
        `).join('');

        // Восстановить выбранные жанры
        const savedGenres = this.editingGame?.genres || [];
        document.querySelectorAll('input[name="genres[]"]').forEach(checkbox => {
            if (savedGenres.includes(checkbox.value)) {
                checkbox.checked = true;
            }
        });
    }

    async loadGames() {
        try {
            this.ui.products.innerHTML = '<div class="loader">⏳ Загрузка...</div>';
            const games = await api.getMyGames();

            console.log(games);

            if (!games?.content.length) {
                this.ui.products.innerHTML = `
                    <div class="empty-state">
                        <div class="emoji">🎮</div>
                        <h3>Нет игр</h3>
                        <p>Добавьте первую игру!</p>
                    </div>
                `;
                this.ui.gamesCount.textContent = '0 игр';
                return;
            }

            this.ui.products.innerHTML = games.content.map(game => `
                <div class="game-card" data-id="${game.id}">
                    <div class="card-image">
                        <img src="${game.imageUrl || '/placeholder.jpg'}" alt="${game.title}" loading="lazy">
                        <div class="card-actions">
                            <button class="btn-edit" onclick="seller.editGame('${game.id}')">✏️</button>
                            <button class="btn-delete" onclick="seller.deleteGame('${game.id}')">🗑️</button>
                        </div>
                    </div>
                    <div class="card-info">
                        <h3>${this.escapeHtml(game.title)}</h3>
                        <div class="price">${parseFloat(game.price || 0).toLocaleString('ru-RU')} ₽</div>
                        <div class="dev">${this.escapeHtml(game.developerTitle || '')}</div>
                    </div>
                </div>
            `).join('');

            this.ui.gamesCount.textContent = `${games.content.length} ${this.pluralize(games.length, ['игра', 'игры', 'игр'])}`;
        } catch (error) {
            this.ui.products.innerHTML = '<div class="error">❌ Ошибка загрузки</div>';
            console.error('Load games error:', error);
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    pluralize(count, forms) {
        const n = Math.abs(count) % 100;
        const n1 = n % 10;
        if (n > 10 && n < 20) return forms[2];
        if (n1 > 1 && n1 < 5) return forms[1];
        if (n1 === 1) return forms[0];
        return forms[2];
    }

    openModal(title = '➕ Добавить игру') {
        this.editingId = null;
        this.editingGame = null;
        document.getElementById('modalTitle').textContent = title;
        this.ui.modal.classList.add('show');
        this.ui.form.reset();
        document.getElementById('imagePreview').innerHTML = '';
        this.initGenresEditor();
    }

    closeModal() {
        this.ui.modal.classList.remove('show');
        this.ui.menu.classList.remove('active');
    }

    async editGame(id) {
        try {
            const game = await api.getGame(id);
            this.editingId = id;
            this.editingGame = game;
            document.getElementById('modalTitle').textContent = '✏️ Редактировать игру';

            const fields = ['title', 'price', 'developerTitle', 'publisher', 'description'];
            fields.forEach(key => {
                const el = document.getElementById(key);
                if (el) el.value = game[key] || '';
            });

            if (game.imageUrl) {
                document.getElementById('imagePreview').innerHTML =
                    `<img src="${game.imageUrl}" alt="Текущее изображение">`;
            }

            this.openModal();
        } catch (error) {
            alert('Ошибка загрузки: ' + (error.message || 'Неизвестная ошибка'));
            console.error('Edit game error:', error);
        }
    }

    async deleteGame(id) {
        if (!confirm('Удалить игру навсегда? Это действие нельзя отменить.')) return;
        try {
            await api.deleteGame(id);
            this.loadGames();
        } catch (error) {
            alert('Ошибка удаления: ' + (error.message || 'Неизвестная ошибка'));
            console.error('Delete game error:', error);
        }
    }

    async saveGame(e) {
        e.preventDefault();
        const btn = document.getElementById('saveGame');
        const originalText = btn.textContent;
        btn.disabled = true;
        btn.textContent = '💾 Сохранение...';

        try {
            const formData = new FormData(this.ui.form);

            // ✅ УДАЛЯЕМ ЛИШНИЕ genres[]
            while (formData.getAll('genres[]').length > 0) {
                formData.delete('genres[]');
            }

            // ✅ ТОЛЬКО Genres[0].Title
            const selectedGenres = Array.from(document.querySelectorAll('input[name="genres[]"]:checked'))
                .map(cb => cb.value);

            selectedGenres.forEach((genre, index) => {
                formData.append(`Genres[${index}].Title`, genre);
            });

            // 🕵️‍♂️ ПРОВЕРКА (чистый результат):
            console.clear();
            console.log('🎮 FORM DATA ЧИСТЫЙ:');
            for (let [key, value] of formData.entries()) {
                console.log(`${key}: ${value}`);
            }

            if (this.editingId) {
                await api.updateGame(this.editingId, formData);
                alert('✅ Обновлено!');
            } else {
                await api.createGame(formData);
                alert('✅ Добавлено!');
            }

            this.closeModal();
            this.loadGames();
        } catch (error) {
            alert('❌ ' + error.message);
        } finally {
            btn.disabled = false;
            btn.textContent = originalText;
        }
    }


}

// Глобальный экземпляр
const seller = new SellerDashboard();
window.seller = seller;
