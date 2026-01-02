// js/login.js
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('loginForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const emailInput = document.getElementById('email-field').value;
        const usernameInput = document.getElementById('username-field').value;
        const passwordInput = document.getElementById('password-field').value;
        const roleSelect = document.getElementById('userRole-field');

        const role = roleSelect.value;

        try {
            await auth.login(emailInput, usernameInput, passwordInput, role);
            window.location.href = 'index.html';
        } catch (err) {
            console.error('Login failed', err);
            alert(err.message || 'Ошибка входа');
        }
    });
});
