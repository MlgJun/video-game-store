// js/auth.js
class AuthService {
    constructor() {
        this.currentUser = {
            username: null,
            email: null,
            role: null,
            isAuthenticated: false
        };
        this.cartItemsCount = 0;
    }

    async init() {
        let user = localStorage.getItem("currentUser");

        if(user) this.currentUser = JSON.parse(user);
    }

    async login(email, username, password, role = 'CUSTOMER') {
        try {
            // identifier can be email or username
            const payload = { Email: email, Username: username, Password: password, UserRole: role };
            const data = await api.login(payload);
            this.currentUser.email = email;
            this.currentUser.username = username;
            this.currentUser.role = role;
            this.currentUser.isAuthenticated = true;
            localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
            this.updateUI();
            menuManager.updateMenu();
            return data;
        } catch (error) {
            throw error;
        }
    }

    async register(email, username, password, role = 'CUSTOMER') {
        try {
            const payload = { Email: email, Username: username, Password: password, UserRole: role };
            const data = await api.register(payload);
            //this.updateUI();
            //menuManager.updateMenu();
            return data;
        } catch (error) {
            throw error;
        }
    }

    async logout() {
        try {
            await api.logout();
            this.currentUser = null;
            localStorage.removeItem("currentUser");
            this.cartItemsCount = 0;
            this.updateUI();
            menuManager.updateMenu();
            menuManager.closeMenu();
        } catch (error) {
            console.error('Logout failed:', error);
        }
    }

    updateUI() {
        const userMenuLabel = document.getElementById('userMenuLabel');
        if (userMenuLabel) {
            if (this.currentUser) {
                if (this.currentUser.role === 'SELLER') {
                    userMenuLabel.textContent = 'Продавец';
                } else {
                    userMenuLabel.textContent = 'Профиль';
                }
            } else {
                userMenuLabel.textContent = 'Войти';
            }
        }

        // Обновляем данные пользователя в меню
        if (this.currentUser && this.currentUser.username) {
            const userName = document.getElementById('userName');
            const userEmail = document.getElementById('userEmail');
            const userAvatar = document.getElementById('userAvatar');
            const sellerName = document.getElementById('sellerName');
            const sellerEmail = document.getElementById('sellerEmail');
            const sellerAvatar = document.getElementById('sellerAvatar');

            if (userName) userName.textContent = this.currentUser.username;
            if (userEmail) userEmail.textContent = this.currentUser.email;
            if (userAvatar) userAvatar.textContent = this.currentUser.username.charAt(0).toUpperCase();
            if (sellerName) sellerName.textContent = this.currentUser.username;
            if (sellerEmail) sellerEmail.textContent = this.currentUser.email;
            if (sellerAvatar) sellerAvatar.textContent = this.currentUser.username.charAt(0).toUpperCase();
        }

        // Обновляем меню
        if (window.menuManager) {
            window.menuManager.updateMenuContent();
        }

        // Обновляем счетчик корзины
        this.updateCartBadge();
    }

    async updateCartBadge() {
        if (this.currentUser?.role === 'CUSTOMER') {
            try {
                const cart = await api.getCart();
                if (cart && (cart.CartItems || cart.cartItems)) {
                    const items = cart.CartItems || cart.cartItems;
                    this.cartItemsCount = items.reduce((sum, item) => sum + (item.Quantity ?? item.quantity ?? 0), 0);

                    const cartBadge = document.getElementById('cartItemCount');
                    const cartBadgeMenu = document.getElementById('cartItemCountMenu');

                    if (cartBadge) {
                        cartBadge.textContent = this.cartItemsCount;
                        cartBadge.style.display = this.cartItemsCount > 0 ? 'block' : 'none';
                    }
                    if (cartBadgeMenu) {
                        cartBadgeMenu.textContent = this.cartItemsCount;
                        cartBadgeMenu.style.display = this.cartItemsCount > 0 ? 'block' : 'none';
                    }
                }
            } catch (error) {
                console.error('Failed to load cart:', error);
            }
        }
    }

    updateCartCount(count) {
        this.cartItemsCount = count;
        this.updateCartBadge();
    }
}

const auth = new AuthService();

// Инициализация авторизации при загрузке
document.addEventListener('DOMContentLoaded', async () => {
    await auth.init();
    menuManager.updateMenu();
});