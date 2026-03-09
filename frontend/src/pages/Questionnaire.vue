<template>
  <div class="page-card questionnaire">
    <h2>Як ви почуваєтесь сьогодні?</h2>
    <p class="subtitle">Будь ласка, дайте відповіді на кілька коротких запитань перед сесією.</p>

    <form @submit.prevent="submit">
      <div class="question">
        <label>Як ви себе почуваєте сьогодні?</label>
        <div class="scale-row">
          <button
            v-for="n in 5" :key="n"
            type="button"
            :class="['scale-btn', { active: form.moodScore === n }]"
            @click="form.moodScore = n"
          >{{ moodEmoji[n] }}</button>
        </div>
        <div class="scale-labels"><span>Дуже погано</span><span>Чудово</span></div>
      </div>

      <div class="question">
        <label>Оцініть якість вашого сну минулої ночі</label>
        <div class="scale-row">
          <button
            v-for="n in 5" :key="n"
            type="button"
            :class="['scale-btn', { active: form.sleepScore === n }]"
            @click="form.sleepScore = n"
          >{{ n }}</button>
        </div>
        <div class="scale-labels"><span>Дуже погано</span><span>Відмінно</span></div>
      </div>

      <div class="question">
        <label>Рівень тривоги прямо зараз</label>
        <div class="scale-row">
          <button
            v-for="n in 5" :key="n"
            type="button"
            :class="['scale-btn', { active: form.anxietyScore === n }]"
            @click="form.anxietyScore = n"
          >{{ n }}</button>
        </div>
        <div class="scale-labels"><span>Немає тривоги</span><span>Дуже висока</span></div>
      </div>

      <div class="question">
        <label>Що вас найбільше турбує зараз? <span class="optional">(необов'язково)</span></label>
        <textarea
          v-model="form.mainConcern"
          rows="3"
          placeholder="Опишіть своїми словами…"
          maxlength="500"
        />
      </div>

      <div class="question">
        <label>Чи є щось важливе, про що ви хочете поговорити на сесії? <span class="optional">(необов'язково)</span></label>
        <textarea
          v-model="form.additionalNotes"
          rows="3"
          placeholder="Ваші думки…"
          maxlength="1000"
        />
      </div>

      <p v-if="error" class="error">{{ error }}</p>

      <div class="form-actions">
        <button type="submit" class="btn-primary" :disabled="!isValid || loading">
          {{ loading ? 'Збереження…' : 'Надіслати та продовжити' }}
        </button>
        <RouterLink to="/book" class="skip-link">Пропустити</RouterLink>
      </div>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '../composables/useApi.js'

const moodEmoji = { 1: '😞', 2: '😕', 3: '😐', 4: '🙂', 5: '😄' }

const form = reactive({
  moodScore: 0,
  sleepScore: 0,
  anxietyScore: 0,
  mainConcern: '',
  additionalNotes: ''
})

const loading = ref(false)
const error = ref(null)
const { post } = useApi()
const router = useRouter()

const isValid = computed(() =>
  form.moodScore > 0 && form.sleepScore > 0 && form.anxietyScore > 0
)

async function submit() {
  error.value = null
  loading.value = true
  try {
    await post('/api/intake', {
      moodScore: form.moodScore,
      sleepScore: form.sleepScore,
      anxietyScore: form.anxietyScore,
      mainConcern: form.mainConcern || null,
      additionalNotes: form.additionalNotes || null
    })
    router.push('/book')
  } catch {
    error.value = 'Не вдалось надіслати відповіді. Спробуйте ще раз.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.questionnaire { max-width: 600px; margin: 2rem auto; }
h2 { margin-bottom: 0.25rem; }
.subtitle { color: var(--muted); font-size: 0.9rem; margin-bottom: 2rem; }

.question { display: flex; flex-direction: column; gap: 0.5rem; margin-bottom: 1.5rem; }
.question label { font-weight: 600; color: var(--text); }
.optional { font-weight: 400; color: var(--muted); font-size: 0.85em; }

.scale-row { display: flex; gap: 0.5rem; }
.scale-btn {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 8px;
  border: 1.5px solid var(--border);
  background: var(--surface, #f8f9fa);
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.15s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.scale-btn:hover { border-color: var(--primary); background: var(--card); }
.scale-btn.active { border-color: var(--primary); background: var(--primary); color: white; }

.scale-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: var(--muted);
}

textarea {
  width: 100%;
  padding: 0.6rem 0.8rem;
  border: 1.5px solid var(--border);
  border-radius: 8px;
  font-size: 0.95rem;
  font-family: inherit;
  resize: vertical;
  transition: border-color 0.15s;
  box-sizing: border-box;
}
textarea:focus { outline: none; border-color: var(--primary); }

.form-actions { display: flex; align-items: center; gap: 1.5rem; margin-top: 0.5rem; }
.btn-primary {
  background: var(--primary);
  color: white;
  padding: 0.65rem 1.5rem;
  font-size: 1rem;
  font-weight: 600;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  transition: background 0.15s;
}
.btn-primary:hover:not(:disabled) { background: var(--primary-dark); }
.btn-primary:disabled { opacity: 0.55; cursor: not-allowed; }

.skip-link { color: var(--muted); font-size: 0.9rem; text-decoration: underline; }
.skip-link:hover { color: var(--text); }
.error { color: var(--danger); font-size: 0.9rem; margin: 0; }
</style>
