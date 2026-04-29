<script setup>
import { ref, onMounted } from 'vue';
import TopBar from '@/components/TopBar.vue';
import Pin from '@/components/Pin.vue';

const pins = ref([]);
const loading = ref(true);
const apiUrl = import.meta.env.VITE_API_URL;

// Funkcja pobierająca dane z backendu
const fetchPins = async () => {
  try {
    const response = await fetch(`${apiUrl}/api/Pins`);
    
    if (!response.ok) throw new Error('Błąd połączenia z serwerem');
    
    const data = await response.json();

    // Mapujemy dane z API na format, którego oczekuje Twój komponent Pin.vue
    pins.value = data.map(pin => ({
      ...pin,
      // Tworzymy pełny adres URL do pliku gp5 na podstawie nazwy z API
      filePath: `${apiUrl}/api/Pins/files/${pin.filename}`,
      // Mapujemy gatunki (backend zwraca ["Rock"], komponent może oczekiwać innej struktury)
      // Jeśli Twój komponent Pin oczekuje pinGenres, dostosowujemy to tutaj:
      pinGenres: pin.genres.map(g => ({ genre: { name: g } }))
    }));

  } catch (error) {
    console.error("Błąd podczas pobierania pinów:", error);
    // Jeśli API nie działa, ładujemy dane testowe (Mock Data)
    loadMockData();
  } finally {
    loading.value = false;
  }
};

// Dane testowe w razie braku połączenia z API
const loadMockData = () => {
  pins.value = [
    {
      id: "1",
      title: "Deep Purple - Smoke on the water (Mock)",
      instrument: 0, 
      filePath: "/sample_pin.gp5", 
      pinGenres: [{ genre: { name: "Rock" } }]
    },
    {
      id: "2",
      title: "Ocipieje (Mock)",
      instrument: 2,
      filePath: "/heavy.gp5",
      pinGenres: [{ genre: { name: "Metal" } }]
    }
  ];
};

const handleSearch = (query) => {
  console.log("Szukam:", query);
};

// Uruchomienie pobierania przy starcie komponentu
onMounted(() => {
  if (apiUrl) {
    fetchPins();
  } else {
    console.warn("VITE_API_URL nie jest zdefiniowane.");
    loadMockData();
    loading.value = false;
  }
});
</script>

<template>
  <div class="app-wrapper">
    <TopBar @search="handleSearch" />
    
    <main class="main-content">
      <!-- Loader podczas ładowania -->
      <div v-if="loading" class="loader">Ładowanie riffów...</div>

      <!-- Komunikat o braku wyników -->
      <div v-else-if="pins.length === 0" class="no-results">
        Nie znaleziono żadnych pinów.
      </div>

      <!-- Siatka pinów -->
      <div v-else class="pin-grid">
        <Pin 
          v-for="item in pins" 
          :key="item.id" 
          :pin="item" 
        />
      </div>
    </main>
  </div>
</template>

<style>
/* ... Twoje istniejące style ... */

.loader, .no-results {
  text-align: center;
  padding: 50px;
  font-size: 1.2rem;
  color: #666;
}

/* Zachowujemy resztę stylów z Twojego pytania */
body {
  margin: 0;
  padding: 0;
  background-color: #f4f7f9;
  font-family: sans-serif;
}

.main-content {
  padding: 20px;
  width: 100%;
  box-sizing: border-box;
}

.pin-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 20px;
}

.pin-grid :deep(.tab-viewport) {
  overflow-x: hidden !important;
  overflow-y: hidden !important;
}
</style>