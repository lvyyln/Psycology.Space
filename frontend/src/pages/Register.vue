<template>
  <div class="auth-wrap">
    <div class="auth-card">
      <div class="logo">⚕ Psycology Space</div>
      <h2>Створити акаунт</h2>
      <form @submit.prevent="submit">
        <label>
          Повне ім'я
          <input v-model="form.fullName" type="text" required autocomplete="name" placeholder="Іван Іваненко" />
          <span v-if="fieldErrors.fullName" class="field-error">{{ fieldErrors.fullName }}</span>
        </label>
        <label>
          Електронна пошта
          <input v-model="form.email" type="email" required autocomplete="email" placeholder="you@example.com" />
          <span v-if="fieldErrors.email" class="field-error">{{ fieldErrors.email }}</span>
        </label>
        <label>
          Пароль
          <input v-model="form.password" type="password" required autocomplete="new-password" placeholder="Мінімум 8 символів" />
          <span v-if="fieldErrors.password" class="field-error">{{ fieldErrors.password }}</span>
        </label>
        <p v-if="errors.length" class="error">
          <span v-for="e in errors" :key="e">{{ e }}<br /></span>
        </p>
        <button type="submit" class="btn-primary" :disabled="loading">{{ loading ? 'Реєстрація…' : 'Зареєструватись' }}</button>
      </form>
      <p class="footer-link">Вже є акаунт? <RouterLink to="/login">Увійти</RouterLink></p>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'
import { useApi } from '../composables/useApi.js'

const form = reactive({ fullName: '', email: '', password: '' })
const errors = ref([])
const fieldErrors = reactive({ fullName: '', email: '', password: '' })
const loading = ref(false)
const { login } = useAuth()
const { post } = useApi()
const router = useRouter()

function validate() {
  let valid = true
  fieldErrors.fullName = ''
  fieldErrors.email = ''
  fieldErrors.password = ''

  if (form.fullName.trim().length < 2) {
    fieldErrors.fullName = 'Ім\'я має містити щонайменше 2 символи.'
    valid = false
  }
  if (!/\S+@\S+\.\S+/.test(form.email)) {
    fieldErrors.email = 'Введіть коректну електронну адресу.'
    valid = false
  }
  if (form.password.length < 8) {
    fieldErrors.password = 'Пароль має містити щонайменше 8 символів.'
    valid = false
  }
  return valid
}

async function submit() {
  errors.value = []
  if (!validate()) return

  loading.value = true
  try {
    const res = await post('/api/auth/register', form)
    login(res.token, res.user)
    router.push(res.user.role === 'Psychologist' ? '/manage-appointments' : '/questionnaire')
  } catch (e) {
    errors.value = Array.isArray(e.data) ? e.data : ['Помилка реєстрації.']
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
.field-error { font-size: 0.8rem; color: var(--danger); }
.btn-primary {
  background: var(--accent);
  color: white;
  padding: 0.65rem;
  font-size: 1rem;
  font-weight: 600;
  border-radius: 8px;
  margin-top: 0.5rem;
}
.btn-primary:hover:not(:disabled) { background: #4d8f7a; }
.error { color: var(--danger); font-size: 0.9rem; margin: 0; }
.footer-link { text-align: center; margin-top: 1.25rem; font-size: 0.9rem; color: var(--muted); }
</style>
