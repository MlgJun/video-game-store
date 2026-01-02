// js/register.js
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('registerForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('username-field').value;
        const email = document.getElementById('email-field').value;
        const password = document.getElementById('password-field').value;
        const confirmPassword = document.getElementById('confirmPassword-field').value;
        const roleInput = document.getElementById('userRole-field');

        const role = roleInput.value;

        if (!username || !email || !password || !confirmPassword) {
            alert('Заполните все поля');
            return;
        }
        if (password !== confirmPassword) {
            alert('Пароли не совпадают');
            return;
        }

        try {
            await auth.register(email, username, password, role);
            window.location.href = 'index.html';
        } catch (err) {
            console.error('Register failed', err);
            alert(err.message || 'Ошибка при регистрации');
        }
    });
});