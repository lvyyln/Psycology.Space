<template>
  <div class="auth-wrap">
    <div class="auth-card">
      <div class="logo">⚕ Psycology Space</div>
      <h2>Ласкаво просимо</h2>
      <form @submit.prevent="submit">
        <label>
          Електронна пошта
          <input v-model="form.email" type="email" required autocomplete="email" placeholder="you@example.com" />
          <span v-if="fieldErrors.email" class="field-error">{{ fieldErrors.email }}</span>
        </label>
        <label>
          Пароль
          <input v-model="form.password" type="password" required autocomplete="current-password" placeholder="••••••••" />
        </label>
        <p v-if="error" class="error">{{ error }}</p>
        <button type="submit" class="btn-primary" :disabled="loading">{{ loading ? 'Вхід…' : 'Увійти' }}</button>
      </form>
      <p class="footer-link">Немає акаунту? <RouterLink to="/register">Зареєструватись</RouterLink></p>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'
import { useApi } from '../composables/useApi.js'

const form = reactive({ email: '', password: '' })
const error = ref(null)
const fieldErrors = reactive({ email: '' })
const loading = ref(false)
const { login } = useAuth()
const { post } = useApi()
const router = useRouter()

async function submit() {
  error.value = null
  fieldErrors.email = ''

  if (!/\S+@\S+\.\S+/.test(form.email)) {
    fieldErrors.email = 'Введіть коректну електронну адресу.'
    return
  }

  loading.value = true
  try {
    const res = await post('/api/auth/login', { email: form.email, password: form.password })
    login(res.token, res.user)
    router.push(res.user.role === 'Psychologist' ? '/manage-appointments' : '/questionnaire')
  } catch (e) {
    error.value = e.status === 401 ? 'Невірна пошта або пароль.' : 'Помилка входу.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-wrap {
  min-height: calc(100vh - 60px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
}
.auth-card {
  background: var(--card);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(74, 144, 164, 0.13);
  padding: 2.5rem 2rem;
  width: 100%;
  max-width: 400px;
}
.logo {
  text-align: center;
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--primary);
  margin-bottom: 0.5rem;
  letter-spacing: 0.01em;
}
h2 {
  text-align: center;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text);
  margin-bottom: 1.5rem;
  padding-left: 0;
  border-left: none;
}
form { display: flex; flex-direction: column; gap: 1rem; }
label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.9rem; color: var(--muted); font-weight: 500; }
.field-error { font-size: 0.8rem; color: var(--danger); margin-top: 0.1rem; }
.btn-primary {
  background: var(--primary);
  color: white;
  padding: 0.65rem;
  font-size: 1rem;
  font-weight: 600;
  border-radius: 8px;
  margin-top: 0.5rem;
}
.btn-primary:hover:not(:disabled) { background: var(--primary-dark); }
.error { color: var(--danger); font-size: 0.9rem; margin: 0; }
.footer-link { text-align: center; margin-top: 1.25rem; font-size: 0.9rem; color: var(--muted); }
</style>
