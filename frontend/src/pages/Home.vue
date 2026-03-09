<template>
  <!-- Hero -->
  <section class="hero">
    <div class="hero-inner">
      <h1 class="hero-title">Ваше психічне здоров'я —<br>крок за кроком.</h1>
      <p class="hero-tagline">Зв'яжіться з кваліфікованим психологом, оберіть зручний час і отримайте підтримку, на яку ви заслуговуєте.</p>
      <div class="hero-cta">
        <RouterLink v-if="isLoggedIn" :to="dashboardPath" class="btn-primary">Перейти до кабінету</RouterLink>
        <RouterLink v-else to="/login" class="btn-primary">Записатись на сеанс</RouterLink>
        <RouterLink v-if="!isLoggedIn" to="/register" class="btn-outline">Створити акаунт</RouterLink>
      </div>
    </div>
  </section>

  <!-- About -->
  <section class="section">
    <div class="section-inner two-col">
      <div class="about-text">
        <h2>Про Psycology Space</h2>
        <p>Ми переконані, що допомога психолога має бути доступною, комфортною та особистою. Psycology Space з'єднує вас із ліцензованими психологами, яким справді важливий ваш прогрес.</p>
        <p>Незалежно від того, чи переживаєте ви тривогу, труднощі у стосунках, або просто шукаєте когось, з ким можна поговорити — наші спеціалісти готові допомогти рухатися вперед у вашому темпі.</p>
        <p>Наша платформа робить планування простим: переглядайте доступні слоти, записуйтесь за кілька секунд і отримайте підтвердження без зайвого листування.</p>
      </div>
      <div class="about-illustration" aria-hidden="true">
        <div class="illustration-placeholder">🧠</div>
      </div>
    </div>
  </section>

  <!-- How it works -->
  <section class="section section-alt">
    <div class="section-inner">
      <h2 class="centered">Як це працює</h2>
      <div class="steps">
        <div class="step">
          <span class="step-icon">🗓</span>
          <h3>Оберіть слот</h3>
          <p>Переглядайте доступні слоти вашого психолога та обирайте зручний для вас час.</p>
        </div>
        <div class="step">
          <span class="step-icon">✍</span>
          <h3>Запишіться</h3>
          <p>Підтвердіть запис одним натисканням. Без дзвінків і очікування — ваш запит надсилається миттєво.</p>
        </div>
        <div class="step">
          <span class="step-icon">✅</span>
          <h3>Отримайте підтвердження</h3>
          <p>Психолог розгляне та підтвердить ваш запис. Оновлений статус ви побачите прямо у своєму кабінеті.</p>
        </div>
      </div>
    </div>
  </section>

  <!-- Testimonials -->
  <section class="section">
    <div class="section-inner">
      <h2 class="centered">Що кажуть наші клієнти</h2>
      <div class="testimonials">
        <div class="testimonial-card">
          <p class="quote">«Записатись було дуже просто — я знайшла вільний слот і підтвердила за хвилину. Платформа відчувається теплою та професійною.»</p>
          <span class="author">— Анна К., клієнтка</span>
        </div>
        <div class="testimonial-card">
          <p class="quote">«Я хвилювався, починаючи онлайн-терапію, але процес виявився бездоганним. Бачити оновлення статусу запису в реальному часі — дуже заспокоює.»</p>
          <span class="author">— Дмитро Л., клієнт</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue'
import { useAuth } from '../composables/useAuth.js'

const { isLoggedIn, role } = useAuth()
const dashboardPath = computed(() =>
  role.value === 'Psychologist' ? '/manage-appointments' : '/book'
)
</script>

<style scoped>
/* Hero */
.hero {
  background: linear-gradient(135deg, #2c3e50 0%, #3d6b78 100%);
  color: #ecf0f1;
  padding: 5rem 1.5rem;
  text-align: center;
}
.hero-inner { max-width: 680px; margin: 0 auto; }
.hero-title {
  font-size: clamp(1.8rem, 4vw, 2.8rem);
  font-weight: 800;
  line-height: 1.2;
  margin: 0 0 1rem;
  color: #ecf0f1;
}
.hero-tagline {
  font-size: 1.1rem;
  color: rgba(236,240,241,.8);
  margin-bottom: 2rem;
  line-height: 1.7;
}
.hero-cta { display: flex; gap: 1rem; justify-content: center; flex-wrap: wrap; }

.btn-primary {
  background: var(--accent);
  color: #fff;
  padding: 0.7rem 1.7rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.15s, transform 0.1s;
}
.btn-primary:hover { background: #4d8f7a; transform: translateY(-1px); }

.btn-outline {
  background: transparent;
  color: #ecf0f1;
  border: 2px solid rgba(236,240,241,.55);
  padding: 0.7rem 1.7rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  text-decoration: none;
  transition: border-color 0.15s, background 0.15s;
}
.btn-outline:hover { border-color: #ecf0f1; background: rgba(255,255,255,.07); }

/* Sections */
.section { padding: 4rem 1.5rem; }
.section-alt { background: var(--card); }
.section-inner { max-width: 900px; margin: 0 auto; }
.section-inner h2 { border-left: 4px solid var(--primary); padding-left: 0.75rem; }
h2.centered { border-left: none; padding-left: 0; text-align: center; margin-bottom: 2.5rem; }

/* About */
.two-col { display: flex; gap: 3rem; align-items: center; }
.about-text { flex: 1; }
.about-text p { color: var(--muted); line-height: 1.75; margin-bottom: 0.9rem; }
.about-illustration { flex-shrink: 0; }
.illustration-placeholder {
  width: 180px;
  height: 180px;
  background: linear-gradient(135deg, #e8f4f8 0%, #d1eae4 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 5rem;
}

/* Steps */
.steps { display: flex; gap: 2rem; flex-wrap: wrap; }
.step {
  flex: 1;
  min-width: 200px;
  text-align: center;
  padding: 1.5rem 1rem;
  background: var(--bg);
  border-radius: 12px;
}
.step-icon { font-size: 2.5rem; display: block; margin-bottom: 0.75rem; }
.step h3 { margin: 0 0 0.5rem; font-size: 1.05rem; color: var(--text); }
.step p { color: var(--muted); font-size: 0.92rem; line-height: 1.6; margin: 0; }

/* Testimonials */
.testimonials { display: flex; gap: 1.5rem; flex-wrap: wrap; }
.testimonial-card {
  flex: 1;
  min-width: 260px;
  background: var(--card);
  border: 1.5px solid var(--border);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 10px rgba(74,144,164,.07);
}
.quote {
  font-style: italic;
  color: var(--text);
  line-height: 1.7;
  margin: 0 0 1rem;
}
.author { font-size: 0.88rem; color: var(--muted); font-weight: 600; }

@media (max-width: 600px) {
  .two-col { flex-direction: column; }
  .about-illustration { align-self: center; }
}
</style>
