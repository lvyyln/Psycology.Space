<template>
  <div class="page-card">
    <h2>Управління записами</h2>

    <!-- Appointments list -->
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
        <div class="info">
          <strong>{{ a.clientName }}</strong>
          <span class="when">{{ formatDate(a.slot.startsAt) }} · {{ a.slot.durationMinutes }} хв</span>
          <span v-if="a.notes" class="notes">{{ a.notes }}</span>
          <div v-if="intakeByEmail[a.clientEmail]" class="intake-inline">
            <div class="scores">
              <span :class="['score-badge', scoreCls(intakeByEmail[a.clientEmail].moodScore)]">Настрій {{ intakeByEmail[a.clientEmail].moodScore }}/5</span>
              <span :class="['score-badge', scoreCls(intakeByEmail[a.clientEmail].sleepScore)]">Сон {{ intakeByEmail[a.clientEmail].sleepScore }}/5</span>
              <span :class="['score-badge', anxietyCls(intakeByEmail[a.clientEmail].anxietyScore)]">Тривога {{ intakeByEmail[a.clientEmail].anxietyScore }}/5</span>
            </div>
            <p v-if="intakeByEmail[a.clientEmail].mainConcern" class="intake-text"><em>Турбує:</em> {{ intakeByEmail[a.clientEmail].mainConcern }}</p>
          </div>
        </div>
        <div class="actions">
          <span :class="['badge', statusClass(a.status)]">{{ statusLabel(a.status) }}</span>
          <template v-if="a.status === 0">
            <button @click="setStatus(a, 1)" :disabled="updating === a.id" class="btn-accept">▶ Підтвердити</button>
            <button @click="setStatus(a, 2)" :disabled="updating === a.id" class="btn-decline">✕ Відхилити</button>
          </template>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useApi } from '../composables/useApi.js'

const { get, put } = useApi()
const appointments = ref([])
const activeTab = ref('upcoming')
const now = () => new Date()
const upcoming = computed(() => appointments.value.filter(a => new Date(a.slot.startsAt) >= now()))
const past     = computed(() => appointments.value.filter(a => new Date(a.slot.startsAt) <  now()))
const listed   = computed(() => activeTab.value === 'upcoming' ? upcoming.value : past.value)
const loading = ref(true)
const error = ref(null)
const updating = ref(null)

// Most recent intake per client email (populated after both fetches)
const intakeByEmail = ref({})

onMounted(async () => {
  const [apptResult, intakeResult] = await Promise.allSettled([
    get('/api/appointments'),
    get('/api/intake')
  ])

  if (apptResult.status === 'fulfilled') {
    appointments.value = apptResult.value
  } else {
    error.value = 'Не вдалося завантажити записи.'
  }
  loading.value = false

  if (intakeResult.status === 'fulfilled') {
    // intakes are ordered by submittedAt desc — first entry per email is the most recent
    const map = {}
    for (const r of intakeResult.value) {
      if (!map[r.clientEmail]) map[r.clientEmail] = r
    }
    intakeByEmail.value = map
  }
})

function formatDate(iso) {
  return new Date(iso).toLocaleString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function statusLabel(s) { return ['Очікує', 'Підтверджено', 'Відхилено'][s] ?? s }
function statusClass(s) { return ['badge-pending', 'badge-accepted', 'badge-declined'][s] ?? '' }

function scoreCls(n) {
  if (n >= 4) return 'score-good'
  if (n === 3) return 'score-mid'
  return 'score-bad'
}

function anxietyCls(n) {
  if (n <= 2) return 'score-good'
  if (n === 3) return 'score-mid'
  return 'score-bad'
}

async function setStatus(appointment, status) {
  updating.value = appointment.id
  try {
    await put(`/api/appointments/${appointment.id}/status`, { status })
    appointment.status = status
  } catch {
    alert('Не вдалося оновити статус.')
  } finally {
    updating.value = null
  }
}
</script>

<style scoped>
h2 { margin-bottom: 1rem; }
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
.muted { color: var(--muted); }
.list { list-style: none; display: flex; flex-direction: column; gap: 0.6rem; }
.item {
  background: var(--card);
  padding: 1rem 1.25rem;
  border-radius: 10px;
  border: 1.5px solid var(--border);
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  transition: transform 0.15s;
}
.item:hover { transform: translateY(-1px); }
.info { display: flex; flex-direction: column; gap: 0.2rem; }
.when { color: var(--muted); font-size: 0.9rem; }
.notes { font-size: 0.88rem; color: var(--muted); font-style: italic; }
.actions { display: flex; align-items: center; gap: 0.5rem; flex-shrink: 0; flex-wrap: wrap; justify-content: flex-end; }
.badge { display: inline-block; padding: 0.2rem 0.7rem; border-radius: 999px; font-size: 0.78rem; font-weight: 600; }
.badge-pending  { background: #fef9c3; color: #854d0e; }
.badge-accepted { background: #dcfce7; color: #166534; }
.badge-declined { background: #fee2e2; color: #991b1b; }
.btn-accept { background: var(--accent); color: white; border: none; padding: 0.3rem 0.8rem; border-radius: 6px; font-size: 0.85rem; transition: background 0.15s; }
.btn-accept:hover:not(:disabled) { background: #4d8f7a; }
.btn-decline { background: transparent; color: var(--danger); border: 1.5px solid var(--danger); padding: 0.3rem 0.8rem; border-radius: 6px; font-size: 0.85rem; transition: background 0.15s, color 0.15s; }
.btn-decline:hover:not(:disabled) { background: var(--danger); color: white; }
button:disabled { opacity: 0.55; }
.error { color: var(--danger); }

/* Inline intake per appointment */
.intake-inline { margin-top: 0.4rem; }
.scores { display: flex; gap: 0.4rem; flex-wrap: wrap; }
.score-badge { padding: 0.15rem 0.55rem; border-radius: 999px; font-size: 0.75rem; font-weight: 600; }
.score-good { background: #dcfce7; color: #166534; }
.score-mid  { background: #fef9c3; color: #854d0e; }
.score-bad  { background: #fee2e2; color: #991b1b; }
.intake-text { font-size: 0.85rem; color: var(--text); margin: 0.25rem 0 0; }
</style>
