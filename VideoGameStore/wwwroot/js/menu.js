// js/menu.js
class MenuManager {
    constructor() {
        this.isMenuOpen = false;
        this.currentMenuType = null;
        this.init();
    }

    init() {
        // Сначала скрываем меню полностью при инициализации
        this.hideMenu();

        // Инициализируем состояние меню (скрыто)
        this.updateMenuVisibility();

        // Затем настраиваем слушатели событий
        this.setupEventListeners();
    }

    hideMenu() {
        const menuContainer = document.getElementById('menuContainer');
        if (menuContainer) {
            // Скрываем контейнер меню
            menuContainer.style.display = 'none';
            menuContainer.style.opacity = '0';
            menuContainer.style.pointerEvents = 'none';
            menuContainer.classList.remove('active');

            // Скрываем все внутренние меню
            const menus = document.querySelectorAll('.menu-overlay');
            menus.forEach(menu => {
                menu.style.display = 'none';
                menu.classList.remove('show');
            });
        }
    }

    setupEventListeners() {
        // Кнопка пользователя в хедере
        const userMenuToggle = document.getElementById('userMenuToggle') || document.querySelector('.user-btn');
        if (userMenuToggle) {
            userMenuToggle.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.toggleMenu();
            });
        }

        // Закрытие меню по клику вне его
        document.addEventListener('click', (e) => {
            if (!this.isMenuOpen) return;

            const menuContainer = document.getElementById('menuContainer');
            const userBtn = document.getElementById('userMenuToggle') || document.querySelector('.user-btn');

            // Если кликнули не по меню и не по кнопке открытия меню
            if (menuContainer &&
                !menuContainer.contains(e.target) &&
                !(userBtn && userBtn.contains(e.target))) {
                this.closeMenu();
            }
        });

        // Кнопка Escape для закрытия меню
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isMenuOpen) {
                this.closeMenu();
            }
        });

        // Кнопки в самом меню
        setTimeout(() => this.setupMenuButtons(), 100);
    }

    toggleMenu() {
        if (this.isMenuOpen) {
            this.closeMenu();
        } else {
            this.openMenu();
        }
    }

    openMenu() {
        this.isMenuOpen = true;
        const menuContainer = document.getElementById('menuContainer');
        if (menuContainer) {
            // Показываем контейнер меню
            menuContainer.style.display = 'block';

            // Даем время для отображения, затем добавляем анимацию
            setTimeout(() => {
                menuContainer.style.opacity = '1';
                menuContainer.style.pointerEvents = 'auto';
                menuContainer.classList.add('active');

                // Показываем нужное меню
                this.updateMenuContent();
            }, 10);
        }
    }

    closeMenu() {
        this.isMenuOpen = false;
        const menuContainer = document.getElementById('menuContainer');
        if (menuContainer) {
            // Убираем активный класс и начинаем скрывать
            menuContainer.classList.remove('active');
            menuContainer.style.opacity = '0';
            menuContainer.style.pointerEvents = 'none';

            // После завершения анимации полностью скрываем
            setTimeout(() => {
                menuContainer.style.display = 'none';

                // Скрываем все меню
                const menus = document.querySelectorAll('.menu-overlay');
                menus.forEach(menu => {
                    menu.classList.remove('show');
                    menu.style.display = 'none';
                });
            }, 300);
        }
    }

    updateMenuVisibility() {
        const menuContainer = document.getElementById('menuContainer');
        if (menuContainer) {
            if (this.isMenuOpen) {
                menuContainer.style.display = 'block';
                menuContainer.style.opacity = '1';
                menuContainer.style.pointerEvents = 'auto';
                menuContainer.classList.add('active');
            } else {
                menuContainer.style.display = 'none';
                menuContainer.style.opacity = '0';
                menuContainer.style.pointerEvents = 'none';
                menuContainer.classList.remove('active');
            }
        }
    }

    setupMenuButtons() {
        // Вход в аккаунт (для гостей)
        const loginMenuBtn = document.getElementById('loginMenuBtn');
        if (loginMenuBtn) {
            loginMenuBtn.addEventListener('click', () => {
                this.closeMenu();
                window.location.href = 'login.html';
            });
        }

        // Регистрация (для гостей)
        const registerMenuBtn = document.getElementById('registerMenuBtn');
        if (registerMenuBtn) {
            registerMenuBtn.addEventListener('click', () => {
                this.closeMenu();
                window.location.href = 'register.html';
            });
        }

        // История заказов (для покупателей)
        const ordersMenuBtn = document.getElementById('ordersMenuBtn');
        if (ordersMenuBtn) {
            ordersMenuBtn.addEventListener('click', () => {
                this.closeMenu();
                if (auth.currentUser?.role === 'CUSTOMER') {
                    window.location.href = 'orders.html';
                } else {
                    alert('Эта функция доступна только для покупателей');
                }
            });
        }

        // Корзина (для покупателей)
        const cartMenuBtn = document.getElementById('cartMenuBtn');
        if (cartMenuBtn) {
            cartMenuBtn.addEventListener('click', () => {
                this.closeMenu();
                if (auth.currentUser?.role === 'CUSTOMER') {
                    window.location.href = 'cart.html';
                } else {
                    alert('Для доступа к корзине необходимо войти как покупатель');
                }
            });
        }

        // Мои товары (для продавцов)
        const sellerGamesMenuBtn = document.getElementById('sellerGamesMenuBtn');
        if (sellerGamesMenuBtn) {
            sellerGamesMenuBtn.addEventListener('click', () => {
                this.closeMenu();
                if (auth.currentUser?.role === 'SELLER') {
                    window.location.href = 'seller.html';
                } else {
                    alert('Эта функция доступна только для продавцов');
                }
            });
        }

        // Выйти (для авторизованных пользователей)
        const logoutButtons = document.querySelectorAll('[onclick*="logout"], .menu-button:last-child');
        logoutButtons.forEach(btn => {
            if (btn.textContent.includes('Выйти')) {
                btn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.closeMenu();
                    auth.logout();
                });
            }
        });

        // Главная страница
        const mainButtons = document.querySelectorAll('.menu-button');
        mainButtons.forEach(btn => {
            if (btn.textContent.includes('Главная') && !btn.hasAttribute('onclick')) {
                btn.addEventListener('click', () => {
                    this.closeMenu();
                    window.location.href = 'index.html';
                });
            }
        });
    }

    updateUserInfo() {
        const user = auth.currentUser;
        if (!user) return;

        document.getElementById('userName').textContent = user.username || 'Пользователь';
        document.getElementById('userEmail').textContent = user.email || '';
        document.getElementById('userAvatar').textContent = user.username?.[0]?.toUpperCase() || 'П';
    }

    // ← updateMenuContent() ИСПРАВЛЕН ↓
    updateMenuContent() {
        if (!this.isMenuOpen) return;

        // Скрыть все
        ['guestMenu', 'userMenu', 'sellerMenu'].forEach(id => {
            const menu = document.getElementById(id);
            if (menu) {
                menu.style.display = 'none';
                menu.classList.remove('show');
            }
        });

        if (!auth.currentUser?.role) {
            console.log("👤 Гость");  // ← console вместо alert
            document.getElementById('guestMenu').style.display = 'block';
            document.getElementById('guestMenu').classList.add('show');
            this.currentMenuType = 'guest';
        } else if (auth.currentUser.role === 'SELLER' && auth.currentUser.isAuthenticated) {
            console.log("🏪 Продавец");
            const sellerMenu = document.getElementById('sellerMenu');
            sellerMenu.style.display = 'block';
            sellerMenu.classList.add('show');
            this.updateSellerInfo();
            this.currentMenuType = 'seller';
        } else if (auth.currentUser.role === 'CUSTOMER' && auth.currentUser.isAuthenticated) {
            console.log("🛒 Покупатель");
            const userMenu = document.getElementById('userMenu');
            userMenu.style.display = 'block';
            userMenu.classList.add('show');
            this.updateUserInfo();
            this.currentMenuType = 'user';
        }
    }

    async updateSellerInfo() {
        if (auth.currentUser?.role === 'SELLER') {
            try {
                const response = await api.getMyGames(1, 1);
                const sellerGamesCount = document.getElementById('sellerGamesCount');
                if (sellerGamesCount && response.totalElements !== undefined) {
                    sellerGamesCount.textContent = response.totalElements;
                }
            } catch (error) {
                console.error('Failed to load seller games:', error);
            }
        }
    }

    updateMenu() {
        this.updateMenuContent();
    }
}

// Инициализация меню при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    // Ensure genre manager exists so header "Жанры" button works on pages with header
    if (!window.genreManager) {
        try {
            window.genreManager = new GenreManager(window.gameStore);
        } catch (e) {
            console.warn('GenreManager init skipped (missing script or dependency):', e);
        }
    }
});

// Глобальная функция для обновления меню
//function updateMenu() {
//    if (window.menuManager) {
//        window.menuManager.updateMenuContent();
//    }
//}

// Глобальная функция для logout
function logout() {
    if (window.menuManager) {
        window.menuManager.closeMenu();
    }
    auth.logout();
}

const menuManager = new MenuManager();