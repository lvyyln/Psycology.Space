import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'
import Home from '../pages/Home.vue'
import Login from '../pages/Login.vue'
import Register from '../pages/Register.vue'
import Book from '../pages/Book.vue'
import MyAppointments from '../pages/MyAppointments.vue'
import ManageSlots from '../pages/ManageSlots.vue'
import ManageAppointments from '../pages/ManageAppointments.vue'
import Questionnaire from '../pages/Questionnaire.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/login', component: Login, meta: { guest: true } },
  { path: '/register', component: Register, meta: { guest: true } },
  { path: '/questionnaire', component: Questionnaire, meta: { auth: true, role: 'Client' } },
  { path: '/book', component: Book, meta: { auth: true, role: 'Client' } },
  { path: '/my-appointments', component: MyAppointments, meta: { auth: true, role: 'Client' } },
  { path: '/manage-slots', component: ManageSlots, meta: { auth: true, role: 'Psychologist' } },
  { path: '/manage-appointments', component: ManageAppointments, meta: { auth: true, role: 'Psychologist' } },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const { isLoggedIn, role } = useAuth()

  if (to.meta.auth && !isLoggedIn.value) return '/login'
  if (to.meta.guest && isLoggedIn.value) {
    return role.value === 'Psychologist' ? '/manage-appointments' : '/book'
  }
  if (to.meta.role && isLoggedIn.value && role.value !== to.meta.role) {
    return role.value === 'Psychologist' ? '/manage-appointments' : '/book'
  }
  return true
})

export default router
