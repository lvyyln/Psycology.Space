<template>
  <div class="page-card">
    <h2>Мої записи</h2>
    <div class="tabs">
      <button :class="['tab', { active: activeTab === 'upcoming' }]" @click="activeTab = 'upcoming'">
        Майбутні <span v-if="upcoming.length" class="tab-count">{{ upcoming.length }}</span>
      </button>
      <button :class="['tab', { active: activeTab === 'past' }]" @click="activeTab = 'past'">
        Минулі <span v-if="past.length" class="tab-count">{{ past.length }}</span>
      </button>
    </div>
    <p v-if="loading">Завантаження…</p>
    <p v-else-if="error" class="error">{{ error }}</p>
    <p v-else-if="listed.length === 0" class="empty">{{ activeTab === 'upcoming' ? 'Майбутніх записів немає.' : 'Минулих записів немає.' }}</p>
    <ul v-else class="list">
      <li v-for="a in listed" :key="a.id" class="item">
        <div class="item-main">
          <div>
            <strong>{{ formatDate(a.slot.startsAt) }}</strong>
            <span class="duration">{{ a.slot.durationMinutes }} хв</span>
          </div>
          <span :class="['badge', statusClass(a.status)]">{{ statusLabel(a.status) }}</span>
          <p v-if="a.notes" class="notes">{{ a.notes }}</p>
        </div>
        <button
          v-if="a.status === 0 || a.status === 1"
          @click="cancel(a)"
          class="btn-cancel"
          :disabled="cancelling === a.id"
        >{{ cancelling === a.id ? '…' : '✕ Скасувати' }}</button>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useApi } from '../composables/useApi.js'

const { get, del } = useApi()
const appointments = ref([])
const activeTab = ref('upcoming')
const now = () => new Date()
const upcoming = computed(() => appointments.value.filter(a => new Date(a.slot.startsAt) >= now()))
const past     = computed(() => appointments.value.filter(a => new Date(a.slot.startsAt) <  now()))
const listed   = computed(() => activeTab.value === 'upcoming' ? upcoming.value : past.value)
const loading = ref(true)
const error = ref(null)
const cancelling = ref(null)

onMounted(async () => {
  try {
    appointments.value = await get('/api/appointments')
  } catch {
    error.value = 'Не вдалося завантажити записи.'
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

// AppointmentStatus: 0=Pending, 1=Accepted, 2=Declined
function statusLabel(s) { return ['Очікує', 'Підтверджено', 'Відхилено'][s] ?? s }
function statusClass(s) { return ['badge-pending', 'badge-accepted', 'badge-declined'][s] ?? '' }

async function cancel(a) {
  if (!confirm(`Скасувати запис на ${formatDate(a.slot.startsAt)}?`)) return
  cancelling.value = a.id
  try {
    await del(`/api/appointments/${a.id}`)
    appointments.value = appointments.value.filter(x => x.id !== a.id)
  } catch {
    alert('Не вдалося скасувати запис.')
  } finally {
    cancelling.value = null
  }
}
</script>

<style scoped>
h2 { margin-bottom: 1rem; color: var(--text); }
.tabs { display: flex; gap: 0; margin-bottom: 1.25rem; border-bottom: 2px solid var(--border); }
.tab {
  background: none; border: none; padding: 0.55rem 1.1rem;
  font-size: 0.95rem; font-weight: 600; color: var(--muted);
  cursor: pointer; border-bottom: 2px solid transparent;
  margin-bottom: -2px; transition: color 0.15s, border-color 0.15s;
}
.tab:hover { color: var(--text); }
.tab.active { color: var(--primary); border-bottom-color: var(--primary); }
.tab-count {
  display: inline-block; background: var(--primary); color: white;
  border-radius: 999px; font-size: 0.7rem; padding: 0.1rem 0.45rem;
  margin-left: 0.35rem; vertical-align: middle;
}
.empty { color: var(--muted); }
.list { list-style: none; display: flex; flex-direction: column; gap: 0.75rem; }
.item {
  background: var(--card);
  padding: 1rem 1.25rem;
  border-radius: 10px;
  box-shadow: 0 1px 6px rgba(0,0,0,.07);
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  transition: transform 0.15s;
}
.item:hover { transform: translateY(-1px); }
.item-main { flex: 1; }
.duration { margin-left: 0.75rem; color: var(--muted); font-size: 0.9rem; }
.notes { margin-top: 0.4rem; font-size: 0.9rem; color: var(--muted); }
.badge { display: inline-block; margin-top: 0.4rem; padding: 0.2rem 0.7rem; border-radius: 999px; font-size: 0.78rem; font-weight: 600; }
.badge-pending  { background: #fef9c3; color: #854d0e; }
.badge-accepted { background: #dcfce7; color: #166534; }
.badge-declined { background: #fee2e2; color: #991b1b; }
.btn-cancel {
  background: transparent;
  color: var(--danger);
  border: 1.5px solid var(--danger);
  padding: 0.3rem 0.8rem;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.85rem;
  white-space: nowrap;
  transition: background 0.15s, color 0.15s;
}
.btn-cancel:hover:not(:disabled) { background: var(--danger); color: white; }
.btn-cancel:disabled { opacity: 0.5; cursor: not-allowed; }
.error { color: var(--danger); }
</style>
