// js/orders.js
class OrdersPage {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 10;
        this.init();
    }

    async init() {
        await auth.init();
        if (auth.currentUser?.role !== 'CUSTOMER') {
            window.location.href = 'index.html';
            return;
        }
        this.setupEventListeners();
        await this.loadOrders();
    }

    async loadOrders() {
        try {
            const response = await api.getOrders(this.currentPage, this.pageSize);
            this.renderOrders(response);
            console.log(response);
        } catch (error) {
            console.error('Failed to load orders:', error);
        }
    }

    renderOrders(response) {
        const ordersContainer = document.getElementById('purchasesList');
        const totalItems = document.getElementById('totalItems');

        if (!ordersContainer) return console.error('purchasesList не найден!');

        ordersContainer.innerHTML = '';

        if (!response.content?.length) {
            ordersContainer.innerHTML = `
        <div class="empty-state" style="text-align:center; padding:60px 20px;">
            <div style="font-size:64px; margin-bottom:20px;">📦</div>
            <h3 style="margin:0 0 12px 0; font-size:24px;">Пока нет заказов</h3>
            <p style="color:#666; margin:0 0 24px 0;">Совершите первую покупку!</p>
            <a href="index.html" class="btn btn-primary" style="padding:12px 24px;">Начать покупки</a>
        </div>
    `;
            if (totalItems) totalItems.textContent = '0 шт.';
            return;
        }

        if (totalItems) totalItems.textContent = `${response.content.length} шт.`;

        response.content.forEach((order, index) => {
            const orderElement = document.createElement('div');
            orderElement.className = 'order-card';
            orderElement.style.cssText = `
            background: white; border-radius: 12px; 
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            margin-bottom: 20px; overflow: hidden;
            border-left: 4px solid #667eea;
            transition: all 0.3s ease;
        `;
            orderElement.onmouseenter = () => orderElement.style.boxShadow = '0 8px 24px rgba(0,0,0,0.15)';
            orderElement.onmouseleave = () => orderElement.style.boxShadow = '0 4px 12px rgba(0,0,0,0.08)';

            // Обработка ключей (поддержка строки JSON и массива)
            const processedOrderItems = order.orderItems.map(item => {
                let keys = [];
                try {
                    if (typeof item.keys === 'string') {
                        keys = JSON.parse(item.keys);
                    } else if (Array.isArray(item.keys)) {
                        keys = item.keys;
                    }
                } catch (e) {
                    console.warn('Failed to parse keys:', item.keys, e);
                    keys = [];
                }
                return { ...item, keys };
            });

            orderElement.innerHTML = `
        <div class="order-header" style="
            padding: 20px 24px; 
            color: white;
        ">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <div class="order-id" style="font-size: 18px; font-weight: 600; margin-bottom: 4px;">
                        Заказ #${order.id || index + 1}
                    </div>
                    <div class="order-date" style="font-size: 14px; opacity: 0.9;">
                        ${new Date(order.createdAt).toLocaleString('ru-RU', {
                year: 'numeric', month: 'long', day: 'numeric',
                hour: '2-digit', minute: '2-digit'
            })}
                    </div>
                </div>
            </div>
        </div>
        
        <div class="order-items" style="padding: 24px;">
            ${processedOrderItems.map(item => `
                <div class="order-game-item" style="
                    display: flex;
                    flex-direction: column;
                    border-bottom: 1px solid #eee;
                    padding: 12px 0;
                ">
                    <!-- Основная строка товара -->
                    <div style="
                        display: flex;
                        align-items: center;
                        height: 50px;
                        justify-content: space-between;
                    ">
                        <div style="
                            display: flex;
                            align-items: center;
                            flex: 1;
                            gap: 12px;
                        ">
                            <div style="
                                font-weight: 500;
                                font-size: 16px;
                                color: black;
                                max-width: 300px;
                                white-space: nowrap;
                                overflow: hidden;
                                text-overflow: ellipsis;
                            ">${item.gameTitle || item.title || 'Без названия'}</div>
                            <span style="
                                background: #f0f0f0;
                                color: #666;
                                padding: 4px 8px;
                                border-radius: 12px;
                                font-size: 14px;
                                flex-shrink: 0;
                            ">×${item.quantity}</span>
                        </div>
                        <div style="
                            font-weight: 600;
                            font-size: 16px;
                            color: #333;
                            min-width: 80px;
                            text-align: right;
                        ">${(item.price || 0).toFixed(2)} ₽</div>
                    </div>

                    <!-- Ключи (если есть) -->
                    ${item.keys && item.keys.length > 0 ? `
                        <div style="margin-top: 10px; padding-top: 8px;">
                            <div style="font-size: 13px; color: #555; margin-bottom: 4px; font-weight: 500;">
                                Ключи активации:
                            </div>
                            <div style="display: flex; flex-wrap: wrap; gap: 8px;">
                                ${item.keys.map(key => `
                                    <code style="
                                        background: #f8f9fa;
                                        color: #2c7be5;
                                        border: 1px solid #e0e6ed;
                                        padding: 4px 10px;
                                        border-radius: 6px;
                                        font-family: monospace;
                                        font-size: 13px;
                                        word-break: break-all;
                                    ">${key}</code>
                                `).join('')}
                            </div>
                        </div>
                    ` : ''}
                </div>
            `).join('')}
        </div>
        
        <div class="order-footer" style="
            padding: 20px 24px; 
            background: #f8f9ff; border-top: 1px solid #eee;
            font-size: 18px; font-weight: 700; color: #333;
            text-align: right;
        ">
            Итого: <span style="color: #667eea;">${(order.totalAmount || 0).toFixed(2)} ₽</span>
        </div>
    `;

            ordersContainer.appendChild(orderElement);
        });
    }


    setupEventListeners() {
        // Пагинация
        const prevBtn = document.getElementById('prevPage');
        const nextBtn = document.getElementById('nextPage');

        if (prevBtn) {
            prevBtn.addEventListener('click', () => {
                if (this.currentPage > 1) {
                    this.currentPage--;
                    this.loadOrders();
                }
            });
        }

        if (nextBtn) {
            nextBtn.addEventListener('click', () => {
                this.currentPage++;
                this.loadOrders();
            });
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new OrdersPage();
});