import { ref, computed } from 'vue'

const TOKEN_KEY = 'authToken'
const USER_KEY = 'authUser'

function parseUser(raw) {
  try { return raw ? JSON.parse(raw) : null } catch { return null }
}

const token = ref(localStorage.getItem(TOKEN_KEY) ?? null)
const user = ref(parseUser(localStorage.getItem(USER_KEY)))

export function useAuth() {
  const isLoggedIn = computed(() => token.value !== null)
  const role = computed(() => user.value?.role ?? null)

  function login(newToken, newUser) {
    token.value = newToken
    user.value = newUser
    localStorage.setItem(TOKEN_KEY, newToken)
    localStorage.setItem(USER_KEY, JSON.stringify(newUser))
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
  }

  return { token, user, isLoggedIn, role, login, logout }
}
