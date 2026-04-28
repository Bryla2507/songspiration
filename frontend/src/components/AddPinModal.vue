<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <button class="close-button" @click="closeModal">×</button>
      <h2>Dodaj nowy Pin</h2>

      <form @submit.prevent="handleSubmit" class="pin-form">
        <div class="form-group">
          <label for="title">Tytuł:</label>
          <input type="text" id="title" v-model="form.title" required>
        </div>

        <div class="form-group">
          <label for="description">Opis:</label>
          <textarea id="description" v-model="form.description"></textarea>
        </div>

        <div class="form-group">
          <label for="instrument">Instrument:</label>
          <select id="instrument" v-model="form.instrument" required>
            <option value="0">Gitara</option>
            <option value="1">Bas</option>
            <option value="2">Perkusja</option>
          </select>
        </div>

        <div class="form-group">
          <label for="file">Plik tabulatury (.gp5):</label>
          <input type="file" id="file" @change="handleFileChange" accept=".gp5" required>
        </div>

        <button type="submit" class="submit-button" :disabled="isSubmitting">
          {{ isSubmitting ? 'Dodawanie...' : 'Dodaj Pin' }}
        </button>
      </form>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(['close', 'pin-added']);

const form = ref({
  title: '',
  description: '',
  instrument: '0',
  file: null
});

const isSubmitting = ref(false);
const error = ref(null);

const handleFileChange = (event) => {
  form.value.file = event.target.files[0];
};

const closeModal = () => {
  emit('close');
  resetForm();
};

const resetForm = () => {
  form.value = {
    title: '',
    description: '',
    instrument: '0',
    file: null
  };
  error.value = null;
};

const handleSubmit = async () => {
  if (!form.value.file) {
    error.value = 'Proszę wybrać plik tabulatury';
    return;
  }

  isSubmitting.value = true;
  error.value = null;

  try {
    const formData = new FormData();
    formData.append('title', form.value.title);
    formData.append('description', form.value.description || '');
    formData.append('instrument', form.value.instrument);
    formData.append('visibility', '0'); // Public visibility
    formData.append('genreIds', '[]'); // No genres for now
    formData.append('file', form.value.file);

    const response = await fetch('/api/pins/upload', {
      method: 'POST',
      body: formData
    });

    if (!response.ok) {
      throw new Error(`Błąd serwera: ${response.status}`);
    }

    const newPin = await response.json();
    emit('pin-added', newPin);
    closeModal();

  } catch (err) {
    console.error('Błąd podczas dodawania pina:', err);
    error.value = err.message || 'Wystąpił błąd podczas dodawania pina';
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 30px;
  width: 500px;
  max-width: 90%;
  position: relative;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
}

.close-button {
  position: absolute;
  top: 15px;
  right: 15px;
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #999;
}

.close-button:hover {
  color: #666;
}

h2 {
  margin-top: 0;
  color: #24292e;
  font-size: 24px;
  margin-bottom: 25px;
}

.pin-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #4b5563;
  font-size: 14px;
}

.form-group input,
.form-group textarea,
.form-group select {
  padding: 10px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #2ecc71;
  box-shadow: 0 0 0 3px rgba(46, 204, 113, 0.1);
}

.form-group textarea {
  min-height: 100px;
  resize: vertical;
}

.submit-button {
  background: #2ecc71;
  color: white;
  border: none;
  padding: 12px 20px;
  border-radius: 8px;
  font-weight: 700;
  font-size: 16px;
  cursor: pointer;
  transition: background 0.2s;
  margin-top: 10px;
}

.submit-button:hover:not(:disabled) {
  background: #27ae60;
}

.submit-button:disabled {
  background: #95d5b2;
  cursor: not-allowed;
}

.error-message {
  color: #e74c3c;
  padding: 10px;
  background: #fdecea;
  border-radius: 8px;
  margin-top: 15px;
  font-size: 14px;
}
</style>