<template>
  <nav class="navbar">
    <RouterLink to="/" class="brand">⚕ Psycology Space</RouterLink>
    <template v-if="isLoggedIn">
      <template v-if="role === 'Client'">
        <RouterLink to="/book" class="nav-link">Записатись</RouterLink>
        <RouterLink to="/my-appointments" class="nav-link">Мої записи</RouterLink>
      </template>
      <template v-else-if="role === 'Psychologist'">
        <RouterLink to="/manage-slots" class="nav-link">Слоти</RouterLink>
        <RouterLink to="/manage-appointments" class="nav-link">
          Записи
          <span v-if="pendingCount > 0" class="badge">{{ pendingCount }}</span>
        </RouterLink>
      </template>
      <span class="user-name">{{ user?.fullName }}</span>
      <button @click="handleLogout" class="nav-btn">Вийти</button>
    </template>
    <template v-else>
      <RouterLink to="/login" class="nav-link ml-auto">Увійти</RouterLink>
      <RouterLink to="/register" class="nav-link">Реєстрація</RouterLink>
    </template>
  </nav>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useAuth } from '../composables/useAuth.js'
import { useApi } from '../composables/useApi.js'
import { useRouter } from 'vue-router'

const { isLoggedIn, role, user, logout } = useAuth()
const { get } = useApi()
const router = useRouter()
const pendingCount = ref(0)

async function loadPending() {
  if (role.value !== 'Psychologist') return
  try {
    const list = await get('/api/appointments')
    pendingCount.value = list.filter(a => a.status === 0).length
  } catch {
    // silently ignore
  }
}

watch([isLoggedIn, role], ([loggedIn]) => {
  if (loggedIn) loadPending()
  else pendingCount.value = 0
}, { immediate: true })

function handleLogout() {
  logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 0 1.75rem;
  height: 56px;
  background: linear-gradient(135deg, #2c3e50 0%, #3d6b78 100%);
  box-shadow: 0 2px 8px rgba(0,0,0,.2);
}
.brand {
  color: #ecf0f1;
  font-weight: 700;
  font-size: 1.05rem;
  text-decoration: none;
  letter-spacing: 0.02em;
  margin-right: 0.5rem;
}
.nav-link {
  color: rgba(236,240,241,.85);
  text-decoration: none;
  font-size: 0.92rem;
  padding: 0.2rem 0;
  border-bottom: 2px solid transparent;
  transition: color 0.15s, border-color 0.15s;
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
}
.nav-link:hover,
.nav-link.router-link-active {
  color: #ecf0f1;
  border-bottom-color: #5ba08a;
}
.ml-auto { margin-left: auto; }
.badge {
  background: #e74c3c;
  color: white;
  border-radius: 999px;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.1rem 0.45rem;
  line-height: 1.4;
}
.user-name {
  margin-left: auto;
  color: rgba(236,240,241,.7);
  font-size: 0.88rem;
}
.nav-btn {
  background: rgba(231,76,60,.85);
  color: white;
  border: none;
  padding: 0.3rem 0.85rem;
  border-radius: 6px;
  font-size: 0.88rem;
  cursor: pointer;
  transition: background 0.15s;
}
.nav-btn:hover { background: #c0392b; }
</style>
