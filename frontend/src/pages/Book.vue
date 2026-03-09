<template>
  <div class="page-card">
    <h2>Записатись на прийом</h2>
    <p v-if="loadError" class="error">{{ loadError }}</p>
    <p v-else-if="loading">Завантаження слотів…</p>
    <p v-else-if="slots.length === 0" class="empty">Наразі немає доступних слотів.</p>
    <ul v-else class="slot-list">
      <li v-for="slot in slots" :key="slot.id" class="slot-item">
        <div>
          <strong>{{ formatDate(slot.startsAt) }}</strong>
          <span class="duration">{{ slot.durationMinutes }} хв</span>
        </div>
        <button @click="openBook(slot)" class="btn-primary">▶ Записатись</button>
      </li>
    </ul>

    <div v-if="booking" class="modal-overlay" @click.self="booking = null">
      <div class="modal">
        <h3>Запис на {{ formatDate(booking.startsAt) }}</h3>
        <label>Нотатки (необов'язково)
          <textarea v-model="notes" rows="3" placeholder="Будь-які нотатки для психолога…"></textarea>
        </label>
        <p v-if="bookError" class="error">{{ bookError }}</p>
        <div class="modal-actions">
          <button @click="booking = null" class="btn-secondary">Скасувати</button>
          <button @click="confirmBook" :disabled="bookLoading" class="btn-primary">
            {{ bookLoading ? 'Запис…' : 'Підтвердити' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useApi } from '../composables/useApi.js'

const { get, post } = useApi()
const slots = ref([])
const loading = ref(true)
const loadError = ref(null)
const booking = ref(null)
const notes = ref('')
const bookError = ref(null)
const bookLoading = ref(false)

onMounted(async () => {
  try {
    slots.value = await get('/api/slots')
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

function openBook(slot) {
  booking.value = slot
  notes.value = ''
  bookError.value = null
}

async function confirmBook() {
  bookLoading.value = true
  bookError.value = null
  try {
    await post('/api/appointments', { slotId: booking.value.id, notes: notes.value })
    slots.value = slots.value.filter(s => s.id !== booking.value.id)
    booking.value = null
  } catch (e) {
    bookError.value = e.data ?? 'Помилка запису.'
  } finally {
    bookLoading.value = false
  }
}
</script>

<style scoped>
h2 { margin-bottom: 1.5rem; }
.empty { color: var(--muted); }
.slot-list { list-style: none; display: flex; flex-direction: column; gap: 0.6rem; }
.slot-item {
  background: var(--card);
  padding: 0.9rem 1.25rem;
  border-radius: 10px;
  border: 1.5px solid var(--border);
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: transform 0.15s, box-shadow 0.15s;
}
.slot-item:hover { transform: translateY(-1px); box-shadow: 0 3px 12px rgba(74,144,164,.12); }
.duration { margin-left: 0.75rem; color: var(--muted); font-size: 0.9rem; }
.btn-primary { background: var(--primary); color: white; padding: 0.4rem 1.1rem; border-radius: 7px; }
.btn-primary:hover:not(:disabled) { background: var(--primary-dark); }
.btn-secondary { background: transparent; color: var(--muted); border: 1.5px solid var(--border); padding: 0.4rem 1rem; border-radius: 7px; }
.btn-secondary:hover { background: var(--bg); }
.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,.4); display: flex; align-items: center; justify-content: center; z-index: 100; }
.modal { background: var(--card); padding: 2rem; border-radius: 14px; width: 100%; max-width: 420px; display: flex; flex-direction: column; gap: 1rem; box-shadow: 0 8px 32px rgba(0,0,0,.2); }
.modal h3 { color: var(--text); margin: 0; }
label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.9rem; color: var(--muted); font-weight: 500; }
textarea { padding: 0.5rem 0.75rem; border: 1.5px solid var(--border); border-radius: 7px; font-size: 1rem; resize: vertical; font-family: inherit; }
textarea:focus { border-color: var(--primary); outline: none; box-shadow: 0 0 0 3px rgba(74,144,164,.15); }
.modal-actions { display: flex; gap: 0.75rem; justify-content: flex-end; }
.error { color: var(--danger); font-size: 0.9rem; }
</style>
