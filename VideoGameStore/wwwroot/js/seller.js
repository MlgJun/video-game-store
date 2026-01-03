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

        this.ui = {
            createModal: document.getElementById('gameModal'),
            editModal: document.getElementById('editModal'),
            keysModal: document.getElementById('keysModal'),
            createForm: document.getElementById('gameForm'),
            editForm: document.getElementById('editForm'),
            keysForm: document.getElementById('keysForm'),
            products: document.getElementById('productsGrid'),
            gamesCount: document.getElementById('gamesCount')
        };

        // Event listeners
        document.getElementById('closeModal').onclick = () => this.closeCreateModal();
        document.getElementById('cancelGame').onclick = () => this.closeCreateModal();
        document.getElementById('closeEditModal').onclick = () => this.closeEditModal();
        document.getElementById('cancelEditGame').onclick = () => this.closeEditModal();
        document.getElementById('closeKeysModal').onclick = () => this.closeKeysModal();
        document.getElementById('cancelKeysBtn').onclick = () => this.closeKeysModal();
        this.ui.createForm.onsubmit = (e) => this.saveNewGame(e);
        this.ui.editForm.onsubmit = (e) => this.saveEditGame(e);
        this.ui.keysForm.onsubmit = (e) => this.saveKeys(e);
        document.getElementById('addGameBtn').onclick = () => this.openCreateModal();

        this.updateUserInfo();
        this.loadGames();
    }

    updateUserInfo() {
        const user = auth.currentUser;
        if (user) {
            document.getElementById('sellerName').textContent = user.name || 'Продавец';
            document.getElementById('sellerEmail').textContent = user.email || '-';
        }
    }

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

    initGenresEditor(containerId) {
        const genres = this.getGenres();
        const container = document.getElementById(containerId);

        container.innerHTML = genres.map(genre => `
            <label class="genre-checkbox">
                <input type="checkbox" name="genres[]" value="${genre}">
                <span>${genre}</span>
            </label>
        `).join('');

        // Восстанавливаем жанры для редактирования
        if (containerId === 'editGenresList' && this.editingGame?.genres) {
            setTimeout(() => {
                this.editingGame.genres.forEach(genre => {
                    const checkbox = document.querySelector(`#${containerId} input[value="${genre}"]`);
                    if (checkbox) checkbox.checked = true;
                });
            }, 100);
        }
    }

    // === CREATE MODAL ===
    openCreateModal() {
        document.getElementById('modalTitle').textContent = '➕ Добавить игру';
        this.ui.createModal.classList.add('show');
        this.ui.createForm.reset();
        document.getElementById('imagePreview').innerHTML = '';
        this.initGenresEditor('genresList');
    }

    closeCreateModal() {
        this.ui.createModal.classList.remove('show');
    }

    async saveNewGame(e) {
        e.preventDefault();
        const btn = document.getElementById('saveGame');
        const originalText = btn.textContent;
        btn.disabled = true;
        btn.textContent = '💾 Сохранение...';

        try {
            const formData = new FormData(this.ui.createForm);

            // Удаляем лишние genres[]
            while (formData.getAll('genres[]').length > 0) {
                formData.delete('genres[]');
            }

            const genres = Array.from(document.querySelectorAll('#genresList input[name="genres[]"]:checked'))
                .map(cb => cb.value);
            genres.forEach((genre, i) => formData.append(`Genres[${i}].Title`, genre));

            await api.createGame(formData);
            alert('✅ Игра добавлена!');
            this.closeCreateModal();
            this.loadGames();
        } catch (error) {
            alert('❌ ' + error.message);
        } finally {
            btn.disabled = false;
            btn.textContent = originalText;
        }
    }

    // === EDIT MODAL ===
    openEditModal() {
        document.getElementById('editModalTitle').textContent = '✏️ Редактировать игру';
        this.ui.editModal.classList.add('show');
        this.initGenresEditor('editGenresList');
    }

    closeEditModal() {
        this.ui.editModal.classList.remove('show');
    }

    async editGame(id) {
        try {
            const game = await api.getGame(id);
            this.editingId = id;
            this.editingGame = game;

            // Заполняем форму редактирования
            document.getElementById('editTitle').value = game.title || '';
            document.getElementById('editPrice').value = game.price || '';
            document.getElementById('editDeveloperTitle').value = game.developerTitle || '';
            document.getElementById('editPublisherTitle').value = game.publisherTitle || '';
            document.getElementById('editDescription').value = game.description || '';

            if (game.imageUrl) {
                document.getElementById('editImagePreview').innerHTML =
                    `<img src="${game.imageUrl}" alt="Текущее изображение">`;
            }

            this.openEditModal();
        } catch (error) {
            alert('Ошибка загрузки: ' + error.message);
        }
    }

    async saveEditGame(e) {
        e.preventDefault();
        const btn = document.getElementById('saveEditGame');
        const originalText = btn.textContent;
        btn.disabled = true;
        btn.textContent = '💾 Сохранение...';

        try {
            const formData = new FormData(this.ui.editForm);

            while (formData.getAll('genres[]').length > 0) {
                formData.delete('genres[]');
            }

            const genres = Array.from(document.querySelectorAll('#editGenresList input[name="genres[]"]:checked'))
                .map(cb => cb.value);
            genres.forEach((genre, i) => formData.append(`Genres[${i}].Title`, genre));

            await api.updateGame(this.editingId, formData);
            alert('✅ Игра обновлена!');
            this.closeEditModal();
            this.loadGames();
        } catch (error) {
            alert('❌ ' + error.message);
        } finally {
            btn.disabled = false;
            btn.textContent = originalText;
        }
    }

    // === KEYS MODAL ===
    openKeysModal(gameId, gameTitle) {
        this.editingId = gameId;
        document.getElementById('keysModalTitle').textContent = `🔑 Ключи для: ${gameTitle}`;
        this.ui.keysModal.classList.add('show');
        this.ui.keysForm.reset();
        document.getElementById('keysPreview').innerHTML = '';
    }

    closeKeysModal() {
        this.ui.keysModal.classList.remove('show');
        this.editingId = null;
    }

    async saveKeys(e) {
        e.preventDefault();
        const btn = document.getElementById('saveKeysBtn');
        const originalText = btn.textContent;
        btn.disabled = true;
        btn.textContent = '💾 Загрузка...';

        try {
            const keysFile = document.getElementById('keysFileInput').files[0];
            if (!keysFile) {
                alert('❌ Выберите JSON файл с ключами');
                return;
            }

            await api.addGameKeys(this.editingId, keysFile);
            alert('✅ Ключи добавлены!');
            this.closeKeysModal();
            this.loadGames();
        } catch (error) {
            alert('❌ ' + error.message);
        } finally {
            btn.disabled = false;
            btn.textContent = originalText;
        }
    }

    // === COMMON ===
    async loadGames() {
        try {
            this.ui.products.innerHTML = '<div class="loader">⏳ Загрузка...</div>';
            const games = await api.getMyGames();

            if (!games?.content?.length) {
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

            this.ui.products.innerHTML = games.content.map(game => {
                // Считаем количество ключей
                const keysCount = Array.isArray(game.keys) ? game.keys.length : 0;

                return `
        <div class="game-card" data-id="${game.id}">
            <div class="card-image">
                <img src="${game.imageUrl || '/placeholder.jpg'}" alt="${game.title}" loading="lazy">
                <div class="card-actions">
                    <button class="btn-edit" onclick="seller.editGame('${game.id}')" title="Редактировать">✏️</button>
                    <button class="btn-keys" onclick="seller.openKeysModal('${game.id}', '${game.title.replace(/'/g, "\\'")}')" title="Добавить ключи">🔑</button>
                    <button class="btn-delete" onclick="seller.deleteGame('${game.id}')" title="Удалить">🗑</button>
                </div>
            </div>
            <div class="card-info">
                <h3>${this.escapeHtml(game.title)}</h3>
                <div class="price">${parseFloat(game.price || 0).toLocaleString('ru-RU')} ₽</div>
                <div class="dev">${this.escapeHtml(game.developerTitle || '')}</div>
                <div class="keys-info" style="
                    margin-top: 8px;
                    font-size: 14px;
                    color: ${keysCount > 0 ? '#28a745' : '#dc3545'};
                    font-weight: 500;
                    display: flex;
                    align-items: center;
                    gap: 4px;
                ">
                     ${keysCount} ${this.pluralize(keysCount, ['ключ', 'ключа', 'ключей'])}
                </div>
            </div>
        </div>
    `;
            }).join('');

            this.ui.gamesCount.textContent = `${games.content.length} ${this.pluralize(games.content.length, ['игра', 'игры', 'игр'])}`;
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

    async deleteGame(id) {
        if (confirm('Удалить игру навсегда?')) {
            try {
                await api.deleteGame(id);
                this.loadGames();
            } catch (error) {
                alert('Ошибка удаления: ' + error.message);
            }
        }
    }
}

const seller = new SellerDashboard();
window.seller = seller;
