<template>
  <footer class="site-footer" role="contentinfo" aria-label="Футер">
    <div class="footer-inner">
      <div class="footer-left">
        <a href="/" class="footer-logo-link">
          <div class="footer-logo-wrap">
            <div class="footer-logo-text">Flex-Shop</div>
            <img src="/src/assets/icons/logo.svg" alt="logo" class="footer-logo-icon" />
          </div>
        </a>
        <div class="footer-sub">© Flex-Shop 2025. Все права защищены.</div>
      </div>

      <div class="footer-right">
        <div class="footer-title">Мы в соцсетях</div>
        <div class="footer-icons">
          <a class="social-link" href="#" aria-label="Telegram">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="58 112 29 26" class="social-svg" fill="currentColor">
              <path d="M86.9202 115.276L82.6948 136.491C82.3761 137.988 81.5447 138.361 80.3634 137.655L73.9253 132.605L70.8188 135.785C70.475 136.152 70.1875 136.458 69.5249 136.458L69.9875 129.477L81.9198 117.998C82.4386 117.505 81.8073 117.232 81.1134 117.725L66.3621 127.614L60.0116 125.497C58.6302 125.038 58.6052 124.027 60.2991 123.321L85.1388 113.133C86.2889 112.674 87.2952 113.406 86.9202 115.276Z" />
            </svg>
            <span class="social-label">Telegram</span>
          </a>

          <a class="social-link" href="#" aria-label="Вконтакте">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="107 116 28 18" class="social-svg" fill="currentColor">
              <path d="M134.362 117.221C134.554 116.517 134.362 116 133.437 116H130.375C129.595 116 129.237 116.444 129.044 116.939C129.044 116.939 127.485 121.051 125.281 123.717C124.569 124.488 124.242 124.736 123.852 124.736C123.659 124.736 123.363 124.488 123.363 123.785V117.221C123.363 116.377 123.145 116 122.5 116H117.687C117.198 116 116.907 116.394 116.907 116.759C116.907 117.558 118.009 117.744 118.124 119.994V124.876C118.124 125.945 117.947 126.142 117.557 126.142C116.517 126.142 113.991 122.013 112.494 117.288C112.193 116.371 111.896 116 111.112 116H108.05C107.177 116 107 116.444 107 116.939C107 117.817 108.04 122.176 111.839 127.936C114.371 131.868 117.936 134 121.18 134C123.129 134 123.368 133.527 123.368 132.712C123.368 128.954 123.192 128.6 124.169 128.6C124.621 128.6 125.401 128.848 127.22 130.743C129.299 132.993 129.642 134 130.806 134H133.868C134.741 134 135.183 133.528 134.928 132.594C134.346 130.631 130.411 126.592 130.235 126.322C129.782 125.692 129.912 125.411 130.235 124.848C130.24 124.843 133.977 119.15 134.362 117.221Z" />
            </svg>
            <span class="social-label">ВКонтакте</span>
          </a>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Jeju+Myeongjo&display=swap');

.site-footer {
  margin-top: auto;
  width: 100%;
  background-color: #222222;
  height: auto;
  color: #fff;
  transition: all 0.3s ease;
}

.footer-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 60px 150px;
  box-sizing: border-box;
  max-width: 1920px;
  margin: 0 auto;
  transition: all 0.3s ease;
}

.footer-left {
  display: flex;
  flex-direction: column;
  gap: 18px;
  transition: all 0.3s ease;
}

.footer-logo-link {
  text-decoration: none;
  cursor: pointer;
  transition: opacity 0.18s ease;
}

.footer-logo-link:hover {
  opacity: 0.8;
}

.footer-logo-wrap {
  display: flex;
  align-items: center;
  gap: 18px;
  transition: all 0.3s ease;
}

.footer-logo-text {
  font-family: 'Jeju Myeongjo', serif;
  font-size: 96px;
  color: #FFFFFF;
  line-height: 1;
  transition: font-size 0.3s ease;
}

.footer-logo-icon {
  width: 77px;
  height: 70px;
  margin-top: 20px;
  transition: all 0.3s ease;
}

.footer-sub {
  font-family: 'Jeju Myeongjo', serif;
  font-size: 20px;
  color: #DADADA;
  transition: font-size 0.3s ease;
}

.footer-right {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.footer-title {
  font-family: 'Jeju Myeongjo', serif;
  font-size: 36px;
  transition: font-size 0.3s ease;
}

.footer-icons {
  display: flex;
  gap: 16px;
  align-items: center;
  transition: all 0.3s ease;
}

.social-link {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #fff;
  text-decoration: none;
  transition: color 0.18s ease;
}

.social-svg {
  width: 28px;
  height: 28px;
  flex: 0 0 28px;
  transition: all 0.3s ease;
}

.social-label {
  font-family: 'Jeju Myeongjo', serif;
  font-size: 18px;
  color: inherit;
  transition: font-size 0.3s ease;
}

.social-link:hover {
  color: #A53DFF;
}

@media (max-width: 1600px) {
  .footer-inner {
    padding: 50px 120px;
  }
  
  .footer-logo-text {
    font-size: 86px;
  }
  
  .footer-logo-icon {
    width: 70px;
    height: 64px;
    margin-top: 18px;
  }
  
  .footer-title {
    font-size: 32px;
  }
  
  .social-label {
    font-size: 17px;
  }
}

@media (max-width: 1400px) {
  .footer-inner {
    padding: 45px 100px;
  }
  
  .footer-logo-text {
    font-size: 78px;
  }
  
  .footer-logo-icon {
    width: 65px;
    height: 59px;
    margin-top: 16px;
  }
  
  .footer-sub {
    font-size: 19px;
  }
  
  .footer-title {
    font-size: 30px;
  }
  
  .social-svg {
    width: 26px;
    height: 26px;
  }
  
  .social-label {
    font-size: 16px;
  }
}

@media (max-width: 1200px) {
  .footer-inner {
    padding: 40px 80px;
  }
  
  .footer-logo-text {
    font-size: 72px;
  }
  
  .footer-logo-icon {
    width: 60px;
    height: 55px;
    margin-top: 15px;
  }
  
  .footer-sub {
    font-size: 18px;
  }
  
  .footer-title {
    font-size: 28px;
  }
  
  .social-svg {
    width: 25px;
    height: 25px;
  }
  
  .social-label {
    font-size: 16px;
  }
}

@media (max-width: 1100px) {
  .footer-inner {
    padding: 35px 60px;
  }
  
  .footer-logo-text {
    font-size: 66px;
  }
  
  .footer-logo-icon {
    width: 55px;
    height: 50px;
    margin-top: 14px;
  }
  
  .footer-sub {
    font-size: 17px;
  }
  
  .footer-title {
    font-size: 26px;
  }
  
  .social-svg {
    width: 24px;
    height: 24px;
  }
  
  .social-label {
    font-size: 15px;
  }
}

@media (max-width: 1000px) {
  .footer-inner {
    padding: 35px 50px;
    flex-direction: column;
    align-items: flex-start;
    gap: 30px;
  }
  
  .footer-logo-text {
    font-size: 64px;
  }
  
  .footer-logo-icon {
    width: 54px;
    height: 49px;
    margin-top: 13px;
  }
  
  .footer-sub {
    font-size: 17px;
  }
  
  .footer-right {
    align-items: flex-start;
    width: 100%;
  }
  
  .footer-title {
    font-size: 26px;
  }
  
  .footer-icons {
    gap: 20px;
  }
}

@media (max-width: 850px) {
  .footer-inner {
    padding: 30px 40px;
    gap: 25px;
  }
  
  .footer-logo-text {
    font-size: 60px;
  }
  
  .footer-logo-icon {
    width: 50px;
    height: 46px;
    margin-top: 12px;
  }
  
  .footer-sub {
    font-size: 16px;
  }
  
  .footer-title {
    font-size: 24px;
  }
  
  .footer-icons {
    gap: 18px;
  }
  
  .social-svg {
    width: 23px;
    height: 23px;
  }
  
  .social-label {
    font-size: 15px;
  }
}

@media (max-width: 768px) {
  .footer-inner {
    padding: 25px 35px;
    gap: 22px;
  }
  
  .footer-logo-text {
    font-size: 56px;
  }
  
  .footer-logo-icon {
    width: 46px;
    height: 42px;
    margin-top: 11px;
  }
  
  .footer-sub {
    font-size: 15px;
  }
  
  .footer-title {
    font-size: 22px;
  }
  
  .footer-icons {
    gap: 16px;
  }
  
  .social-svg {
    width: 22px;
    height: 22px;
  }
  
  .social-label {
    font-size: 14px;
  }
}

@media (max-width: 600px) {
  .footer-inner {
    padding: 22px 25px;
    gap: 20px;
  }
  
  .footer-logo-text {
    font-size: 48px;
  }
  
  .footer-logo-icon {
    width: 40px;
    height: 36px;
    margin-top: 10px;
  }
  
  .footer-sub {
    font-size: 14px;
  }
  
  .footer-title {
    font-size: 20px;
  }
  
  .footer-left {
    gap: 12px;
  }
  
  .footer-logo-wrap {
    gap: 12px;
  }
  
  .footer-icons {
    gap: 14px;
  }
  
  .social-link {
    gap: 8px;
  }
  
  .social-svg {
    width: 20px;
    height: 20px;
  }
  
  .social-label {
    font-size: 13px;
  }
}

@media (max-width: 480px) {
  .footer-inner {
    padding: 18px 20px;
    gap: 18px;
  }
  
  .footer-logo-text {
    font-size: 42px;
  }
  
  .footer-logo-icon {
    width: 36px;
    height: 33px;
    margin-top: 9px;
  }
  
  .footer-sub {
    font-size: 13px;
  }
  
  .footer-title {
    font-size: 18px;
  }
  
  .footer-left {
    gap: 10px;
  }
  
  .footer-logo-wrap {
    gap: 10px;
  }
  
  .footer-icons {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  
  .social-link {
    gap: 8px;
  }
  
  .social-svg {
    width: 18px;
    height: 18px;
  }
  
  .social-label {
    font-size: 12px;
  }
}

@media (max-width: 360px) {
  .footer-inner {
    padding: 15px 16px;
    gap: 15px;
  }
  
  .footer-logo-text {
    font-size: 36px;
  }
  
  .footer-logo-icon {
    width: 32px;
    height: 29px;
    margin-top: 8px;
  }
  
  .footer-sub {
    font-size: 12px;
  }
  
  .footer-title {
    font-size: 16px;
  }
  
  .footer-left {
    gap: 8px;
  }
  
  .footer-logo-wrap {
    gap: 8px;
  }
  
  .footer-icons {
    gap: 10px;
  }
  
  .social-svg {
    width: 16px;
    height: 16px;
  }
  
  .social-label {
    font-size: 11px;
  }
}

@media (max-width: 320px) {
  .footer-inner {
    padding: 12px 14px;
  }
  
  .footer-logo-text {
    font-size: 32px;
  }
  
  .footer-logo-icon {
    width: 28px;
    height: 26px;
    margin-top: 7px;
  }
  
  .footer-sub {
    font-size: 11px;
  }
  
  .footer-title {
    font-size: 15px;
  }
  
  .social-svg {
    width: 15px;
    height: 15px;
  }
  
  .social-label {
    font-size: 10px;
  }
}
</style>
