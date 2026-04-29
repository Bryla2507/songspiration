<template>
  <div class="add-pin-container">
    <div class="form-card">
      <h1>Dodaj nową tabulaturę</h1>
      
      <form @submit.prevent="handleSubmit" class="pin-form">
        <!-- Tytuł -->
        <div class="form-group">
          <label for="title">Tytuł utworu</label>
          <input v-model="form.title" type="text" id="title" required placeholder="np. Deep Purple - Smoke on the water">
        </div>

        <!-- Opis -->
        <div class="form-group">
          <label for="description">Opis (opcjonalnie)</label>
          <textarea v-model="form.description" id="description" rows="3" placeholder="Dodaj krótki opis..."></textarea>
        </div>

        <div class="form-row">
          <!-- Instrument -->
          <div class="form-group">
            <label for="instrument">Instrument</label>
            <select v-model.number="form.instrument" id="instrument">
              <option :value="0">Gitara</option>
              <option :value="1">Bas</option>
              <option :value="2">Perkusja</option>
            </select>
          </div>

          <!-- Widoczność -->
          <div class="form-group">
            <label for="visibility">Widoczność</label>
            <select v-model.number="form.visibility" id="visibility">
              <option :value="0">Publiczny</option>
              <option :value="1">Prywatny</option>
            </select>
          </div>
        </div>

        <!-- Plik (Guitar Pro) -->
        <div class="form-group">
          <label for="file">Plik tabulatury (.gp3, .gp4, .gp5, .gpx)</label>
          <input type="file" id="file" @change="handleFileChange" accept=".gp3,.gp4,.gp5,.gpx,.gp" required class="file-input">
        </div>

        <!-- Gatunki -->
        <div class="form-group">
          <label>Gatunki</label>
          <div class="genre-selection">
            <label v-for="genre in availableGenres" :key="genre.id" class="checkbox-label">
              <input type="checkbox" :value="genre.id" v-model="form.genreIds">
              {{ genre.name }}
            </label>
          </div>
        </div>

        <div class="form-actions">
          <button type="button" @click="$router.push('/')" class="btn-cancel">Anuluj</button>
          <button type="submit" :disabled="isSubmitting" class="btn-submit">
            {{ isSubmitting ? 'Wysyłanie...' : 'Dodaj Pin' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL; // To jest: http://localhost:5035/api

const isSubmitting = ref(false);
const selectedFile = ref(null);

// Przykładowe gatunki (Wymienione na ID typu GUID, ponieważ tego oczekuje baza danych w C#)
const availableGenres = [
  { id: '3fa85f64-5717-4562-b3fc-2c963f66afa6', name: 'Rock' },
  { id: '11111111-2222-3333-4444-555555555555', name: 'Metal' },
  { id: '22222222-3333-4444-5555-666666666666', name: 'Jazz' },
  { id: '33333333-4444-5555-6666-777777777777', name: 'Blues' }
];

const form = ref({
  title: '',
  description: '',
  instrument: 0,
  visibility: 0,
  genreIds: []
});

const handleFileChange = (event) => {
  selectedFile.value = event.target.files[0];
};

const handleSubmit = async () => {
  if (!selectedFile.value) return alert("Wybierz plik!");

  isSubmitting.value = true;
  
  // Tworzymy FormData zgodnie ze specyfikacją ze Swaggera
  const formData = new FormData();
  formData.append('title', form.value.title);
  formData.append('description', form.value.description);
  formData.append('instrument', form.value.instrument);
  formData.append('visibility', form.value.visibility);
  formData.append('file', selectedFile.value); // Plik binarny
  
  // W C# tablice z FormData odbiera się dodając wielokrotnie ten sam klucz
  form.value.genreIds.forEach(id => {
    formData.append('genreIds', id);
  });

  try {
    // apiUrl zawiera już "/api", więc doklejamy tylko "/Pins/upload"
    const response = await fetch(`${apiUrl}/api/Pins/upload`, {
      method: 'POST',
      body: formData,
      // WAŻNE: Nie ustawiamy nagłówka Content-Type! Przeglądarka zrobi to sama.
    });

    if (response.ok) {
      alert("Pin dodany pomyślnie!");
      router.push('/'); // Przekierowanie na stronę główną
    } else {
      // Pobieranie szczegółów błędu z backendu
      const errorData = await response.json();
      console.error("Błąd z serwera:", errorData);
      alert("Błąd podczas dodawania pina.");
    }
  } catch (error) {
    console.error("Błąd połączenia:", error);
    alert("Nie udało się połączyć z serwerem.");
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
/* Twoje style CSS zostają bez zmian, są bardzo dobre */
.add-pin-container { display: flex; justify-content: center; padding: 40px 20px; }
.form-card { background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); width: 100%; max-width: 600px; }
h1 { margin-bottom: 25px; color: #24292e; font-size: 24px; }
.form-group { margin-bottom: 20px; display: flex; flex-direction: column; gap: 8px; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
label { font-weight: 600; color: #444; font-size: 14px; }
input[type="text"], textarea, select { padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 16px; }
.genre-selection { display: flex; flex-wrap: wrap; gap: 10px; background: #f9f9f9; padding: 15px; border-radius: 8px; }
.checkbox-label { display: flex; align-items: center; gap: 5px; font-size: 14px; background: white; padding: 5px 10px; border: 1px solid #eee; border-radius: 20px; cursor: pointer; }
.form-actions { display: flex; justify-content: flex-end; gap: 15px; margin-top: 30px; }
.btn-submit { background: #2ecc71; color: white; border: none; padding: 12px 25px; border-radius: 6px; font-weight: bold; cursor: pointer; }
.btn-submit:disabled { background: #95a5a6; }
.btn-cancel { background: #eee; border: none; padding: 12px 25px; border-radius: 6px; cursor: pointer; }
</style>