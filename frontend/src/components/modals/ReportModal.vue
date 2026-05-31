<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-card">
      <div class="modal-header">
        <h3>Zgłoś użytkownika</h3>
      </div>
      <div class="modal-body">
        <p>Powiedz nam, dlaczego zgłaszasz tego użytkownika.</p>
        <textarea
          v-model="content"
          placeholder="Wpisz treść zgłoszenia (max 200 znaków)"
          maxlength="200"
          class="report-textarea"
        ></textarea>
        <p class="char-counter">{{ content.length }}/200</p>
      </div>
      <div class="modal-footer">
        <button @click="$emit('close')" class="btn-modal-cancel">Anuluj</button>
        <button @click="submitReport" class="btn-modal-confirm" :disabled="!content.trim()">Wyślij</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const props = defineProps({
  userId: {
    type: String,
    required: true
  }
});

const emit = defineEmits(['close', 'success']);

const content = ref('');
const apiUrl = import.meta.env.VITE_API_URL;

const submitReport = async () => {
  if (!content.value.trim()) return;

  try {
    const token = sessionStorage.getItem('token');
    const response = await fetch(`${apiUrl}/api/Report`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        reportedUserId: props.userId,
        content: content.value
      })
    });

    if (response.ok) {
      emit('success');
      emit('close');
    } else {
      alert('Wystąpił błąd podczas wysyłania zgłoszenia.');
    }
  } catch (error) {
    console.error('Błąd:', error);
    alert('Wystąpił błąd podczas wysyłania zgłoszenia.');
  }
};
</script>

<style scoped>
.modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.6); backdrop-filter: blur(4px); display: flex; justify-content: center; align-items: center; z-index: 9999; }
.modal-card { background: white; padding: 30px; border-radius: 20px; width: 90%; max-width: 450px; position: relative; }
.modal-header h3 { margin: 0 0 15px 0; font-size: 22px; }
.modal-body p { margin: 0 0 15px 0; color: #64748b; }
.report-textarea { width: 100%; min-height: 120px; padding: 12px; border: 1px solid #e2e8f0; border-radius: 10px; resize: vertical; font-family: inherit; font-size: 14px; }
.char-counter { text-align: right; font-size: 12px; color: #94a3b8; margin: 5px 0 0 0; }
.modal-footer { display: flex; gap: 12px; justify-content: flex-end; margin-top: 25px; }
.btn-modal-cancel { background: #f1f5f9; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm { background: #2ecc71; color: white; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm:disabled { background: #a7f3d0; cursor: not-allowed; }
</style>