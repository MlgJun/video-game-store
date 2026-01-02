// js/cart.js
class CartPage {
    constructor() {
        this.init();
    }

    async init() {
        await auth.init();
        if (auth.currentUser?.role !== 'CUSTOMER') {
            window.location.href = 'index.html';
            return;
        }

        document.querySelector('.cart-root').style.opacity = '1';
        this.setupEventListeners();
        await this.loadCart();
    }

    async loadCart() {
        try {
            const cart = await api.getCart();
            this.renderCart(cart);
        } catch (error) {
            console.error('Failed to load cart:', error);
        }
    }

    renderCart(cart) {
        // Элементы из HTML
        const cartContent = document.getElementById('cartContent');
        const emptyCart = document.getElementById('emptyCart');
        const cartWithItems = document.getElementById('cartWithItems');
        const cartBody = document.getElementById('cartBody');
        const cartSidebar = document.getElementById('cartSidebar');
        const cartCount = document.getElementById('cartCount');
        const totalPrice = document.getElementById('totalPrice');
        const summaryItems = document.getElementById('summaryItems');

        if (!cartBody) return console.error('cartBody не найден!');

        const items = cart?.CartItems || cart?.cartItems || [];
        let totalAmount = 0;
        let totalItems = 0;

        if (!items || items.length === 0) {
            cartContent.style.display = 'block';
            emptyCart.style.display = 'flex';
            cartWithItems.style.display = 'none';
            cartSidebar.style.display = 'none';
            cartCount.style.display = 'none';
            if (totalPrice) totalPrice.textContent = '0';
            return;
        }

        cartContent.style.display = 'block';
        emptyCart.style.display = 'none';
        cartWithItems.style.display = 'block';
        cartSidebar.style.display = 'block';
        cartCount.style.display = 'block';

        cartBody.innerHTML = '';

        items.forEach(item => {
            const template = document.getElementById('cartItemTemplate');
            if (!template) return console.error('template#cartItemTemplate не найден!');

            const clone = template.content.cloneNode(true);
            const cartItem = clone.querySelector('.cart-item');

            // Заполнить данные
            cartItem.dataset.id = item.GameId || item.gameId;
            cartItem.querySelector('.item-title').textContent = item.GameTitle || item.gameTitle || 'Без названия';
            cartItem.querySelector('.item-platform').textContent = item.Platform || 'Steam ключ';
            cartItem.querySelector('.item-price').textContent = `${(item.Price || item.price || 0).toFixed(0)} ₽`;
            cartItem.querySelector('.quantity-value').textContent = item.Quantity || item.quantity || 1;
            cartItem.querySelector('img').src = item.ImageUrl;
            cartItem.querySelector('img').alt = item.GameTitle || 'Игра';

            cartItem.querySelectorAll('.quantity-btn, .delete-btn').forEach(btn => {
                btn.dataset.gameId = item.GameId || item.gameId;
            });

            cartBody.appendChild(clone);

            // Подсчёт суммы
            const price = item.Price || item.price || 0;
            const qty = item.Quantity || item.quantity || 1;
            totalAmount += price * qty;
            totalItems += qty;
        });

        // Обновить итоги
        if (totalPrice) totalPrice.textContent = totalAmount.toFixed(0);
        if (summaryItems) summaryItems.textContent = totalItems;
        document.querySelector('.cart-items-number').textContent = totalItems;
    }


    setupEventListeners() {
        // Делегирование событий для динамических элементов
        document.addEventListener('click', async (e) => {
            const gameId = e.target.dataset?.gameId;
            if (!gameId) return;

            if (e.target.classList.contains('plus')) {
                await this.updateCartItem(gameId, 1);
            } else if (e.target.classList.contains('minus')) {
                await this.updateCartItem(gameId, -1);
            } else if (e.target.classList.contains('delete-btn')) {
                await this.removeCartItem(gameId);
            }
        });
        
        document.getElementById('orderBtn').onclick = () => this.checkout();

        document.getElementById('selectAllCheckbox').onchange = (e) => {
            document.querySelectorAll('.item-checkbox').forEach(cb => cb.checked = e.target.checked);
        };
    }

    async updateCartItem(gameId, delta) {
        try {
            const cart = await api.getCart();
            const items = cart?.CartItems || cart?.cartItems || [];
            const item = items.find(i => (i.GameId || i.gameId) == gameId);

            if (item) {
                const currentQty = item.Quantity ?? item.quantity ?? 0;
                const newQuantity = currentQty + delta;
                if (newQuantity <= 0) {
                    await api.removeFromCart(gameId, currentQty);
                } else {
                    // addToCart increases by quantity
                    await api.addToCart(gameId, delta);
                }
                await this.loadCart();
            }
        } catch (error) {
            console.error('Failed to update cart:', error);
        }
    }

    async removeCartItem(gameId) {
        try {
            const cart = await api.getCart();
            const items = cart?.CartItems || cart?.cartItems || [];
            const item = items.find(i => (i.GameId || i.gameId) == gameId);

            if (item) {
                const qty = item.Quantity ?? item.quantity ?? 0;
                await api.removeFromCart(gameId, qty);
                await this.loadCart();
            }
        } catch (error) {
            console.error('Failed to remove item:', error);
        }
    }

    async checkout() {
        try {
            const cart = await api.getCart();
            const items = cart?.CartItems || cart?.cartItems || [];
            if (!items || items.length === 0) {
                alert('Корзина пуста');
                return;
            }
            const orderItems = items.map(item => ({
                GameId: item.GameId ?? item.gameId,
                Quantity: item.Quantity ?? item.quantity
            }));

            await api.createOrder(orderItems);
            alert('Заказ успешно оформлен!');
            await this.loadCart(); 
        } catch (error) {
            console.error('Failed to checkout:', error);
            alert('Не удалось оформить заказ');
        }
    }
}

// Инициализация
document.addEventListener('DOMContentLoaded', () => {
    new CartPage();
});