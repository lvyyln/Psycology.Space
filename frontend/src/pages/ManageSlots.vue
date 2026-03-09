<template>
  <div class="page-card">
    <h2>Управління слотами</h2>

    <section class="add-section">
      <h3>Додати новий слот</h3>
      <form @submit.prevent="addSlot" class="add-form">
        <label>Дата та час
          <input v-model="newSlot.startsAt" type="datetime-local" required />
          <span v-if="addFieldError" class="field-error">{{ addFieldError }}</span>
        </label>
        <label>Тривалість (хвилини)
          <input v-model.number="newSlot.durationMinutes" type="number" min="15" max="480" step="15" required />
        </label>
        <button type="submit" :disabled="addLoading" class="btn-primary">
          {{ addLoading ? 'Додавання…' : '+ Додати слот' }}
        </button>
      </form>
      <p v-if="addError" class="error">{{ addError }}</p>
    </section>

    <section>
      <h3 class="section-title">Всі слоти</h3>
      <p v-if="loading">Завантаження…</p>
      <p v-else-if="loadError" class="error">{{ loadError }}</p>
      <p v-else-if="slots.length === 0" class="empty">Слотів ще немає.</p>
      <ul v-else class="list">
        <li v-for="slot in slots" :key="slot.id" class="item">
          <div>
            <strong>{{ formatDate(slot.startsAt) }}</strong>
            <span class="duration">{{ slot.durationMinutes }} хв</span>
            <span :class="['status-badge', slot.isBooked ? 'booked' : 'free']">
              {{ slot.isBooked ? 'Заброньовано' : 'Вільний' }}
            </span>
          </div>
          <button @click="deleteSlot(slot.id)" class="btn-danger" :disabled="deleting === slot.id || slot.isBooked" :title="slot.isBooked ? 'Не можна видалити заброньований слот' : ''">
            {{ deleting === slot.id ? '…' : '✕ Видалити' }}
          </button>
        </li>
      </ul>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useApi } from '../composables/useApi.js'

const { get, post, del } = useApi()
const slots = ref([])
const loading = ref(true)
const loadError = ref(null)
const newSlot = ref({ startsAt: '', durationMinutes: 60 })
const addLoading = ref(false)
const addError = ref(null)
const addFieldError = ref('')
const deleting = ref(null)

onMounted(async () => {
  try {
    slots.value = await get('/api/slots/all')
  } catch {
    loadError.value = 'Не вдалося завантажити слоти.'
  } finally {
    loading.value = false
  }
})

function formatDate(iso) {
  return new Date(iso).toLocaleString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

async function addSlot() {
  addError.value = null
  addFieldError.value = ''

  const chosen = new Date(newSlot.value.startsAt)
  if (isNaN(chosen.getTime()) || chosen <= new Date()) {
    addFieldError.value = 'Слот має бути в майбутньому.'
    return
  }

  addLoading.value = true
  try {
    const slot = await post('/api/slots', {
      startsAt: chosen.toISOString(),
      durationMinutes: newSlot.value.durationMinutes
    })
    slots.value.push(slot)
    slots.value.sort((a, b) => new Date(a.startsAt) - new Date(b.startsAt))
    newSlot.value = { startsAt: '', durationMinutes: 60 }
  } catch {
    addError.value = 'Не вдалося додати слот.'
  } finally {
    addLoading.value = false
  }
}

async function deleteSlot(id) {
  deleting.value = id
  try {
    await del(`/api/slots/${id}`)
    slots.value = slots.value.filter(s => s.id !== id)
  } catch (e) {
    alert(e.data ?? 'Не вдалося видалити слот.')
  } finally {
    deleting.value = null
  }
}
</script>

<style scoped>
h2 { margin-bottom: 1.5rem; }
h3 { color: var(--text); margin-bottom: 0.75rem; }
.section-title { margin: 1.75rem 0 1rem; }
.add-section {
  background: #f7fbfc;
  border: 1.5px solid var(--border);
  border-radius: 10px;
  padding: 1.25rem 1.5rem;
  margin-bottom: 0.5rem;
}
.add-form { display: flex; flex-wrap: wrap; gap: 1rem; align-items: flex-end; margin-top: 0.75rem; }
.add-form label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.9rem; color: var(--muted); font-weight: 500; min-width: 180px; }
.field-error { font-size: 0.8rem; color: var(--danger); }
.list { list-style: none; display: flex; flex-direction: column; gap: 0.6rem; }
.item {
  background: var(--card);
  padding: 0.9rem 1.25rem;
  border-radius: 10px;
  box-shadow: 0 1px 5px rgba(0,0,0,.07);
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: transform 0.15s;
}
.item:hover { transform: translateY(-1px); }
.duration { margin-left: 0.75rem; color: var(--muted); font-size: 0.9rem; }
.status-badge { margin-left: 0.75rem; padding: 0.1rem 0.5rem; border-radius: 999px; font-size: 0.78rem; font-weight: 600; }
.status-badge.free   { background: #dcfce7; color: #166534; }
.status-badge.booked { background: #fef9c3; color: #854d0e; }
.btn-primary { background: var(--primary); color: white; padding: 0.5rem 1.3rem; }
.btn-primary:hover:not(:disabled) { background: var(--primary-dark); }
.btn-danger { background: transparent; color: var(--danger); border: 1.5px solid var(--danger); padding: 0.3rem 0.8rem; border-radius: 6px; font-size: 0.85rem; transition: background 0.15s, color 0.15s; }
.btn-danger:hover:not(:disabled) { background: var(--danger); color: white; }
.empty { color: var(--muted); }
.error { color: var(--danger); font-size: 0.9rem; margin-top: 0.5rem; }
</style>
