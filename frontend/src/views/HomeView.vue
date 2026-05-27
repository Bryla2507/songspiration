<script setup>
import { ref, onMounted, onUnmounted, computed, reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import TopBar from '@/components/TopBar.vue';
import Pin from '@/components/Pin.vue';

const pins = ref([]);
const loading = ref(true);
const apiUrl = import.meta.env.VITE_API_URL;
const router = useRouter();

// --- PAGINACJA ---
const start = ref(0);
const limit = 6; // Zwiększyłem limit dla lepszego UX
const hasMore = ref(true);
const loadingMore = ref(false);

// --- STAN FILTRÓW ---
const filters = reactive({
  search: '',
  instrument: 'all',
  genre: 'all',
  sortBy: 'newest',
  sortOrder: 'desc'
});

// Mapowanie instrumentów (dostosuj numery do swojego backendu)
const instrumentMap = {
  0: 'Gitara',
  1: 'Bas',
  2: 'Perkusja'
};

const availableGenres = ref([]);

const fetchGenres = async () => {
  try {
    const response = await fetch(`${apiUrl}/api/Genre`);
    if (!response.ok) throw new Error('Błąd pobierania gatunków');
    const data = await response.json();
    availableGenres.value = data.map(g => g.name).sort();
  } catch (error) {
    console.error("Błąd podczas pobierania gatunków z API:", error);
  }
};

// --- LOGIKA FILTROWANIA ---
const filteredPins = computed(() => {
  return pins.value;
});

const fetchPins = async (reset = false) => {
  if (loadingMore.value || (!hasMore.value && !reset)) return;

  loadingMore.value = true;
  if (reset) {
    start.value = 0;
    hasMore.value = true;
  }

  try {
    const token = sessionStorage.getItem('token');
    
    const queryParams = new URLSearchParams({
      start: start.value,
      limit: limit,
      search: filters.search,
      instrument: filters.instrument,
      genre: filters.genre,
      sortBy: filters.sortBy,
      sortOrder: filters.sortOrder
    });

    const response = await fetch(`${apiUrl}/api/Pins?${queryParams.toString()}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    
    if (response.status === 401) {
      router.push('/login');
      return;
    }
    if (!response.ok) throw new Error('Błąd połączenia');
    
    const data = await response.json();
    const likedPins = JSON.parse(localStorage.getItem('likedPins') || '[]');

    const newPins = data.map(pin => ({
      ...pin,
      filePath: `${apiUrl}${pin.filePath}`,
      pinGenres: pin.genres.map(g => ({ genre: { name: g } })),
      isLiked: likedPins.includes(pin.id)
    }));

    if (reset) pins.value = newPins;
    else pins.value.push(...newPins);

    start.value += data.length;
    if (data.length < limit) hasMore.value = false;

  } catch (error) {
    console.error("Błąd:", error);
    if (reset) loadMockData();
  } finally {
    loading.value = false;
    loadingMore.value = false;
  }
};

const loadMockData = () => {
  pins.value = [
    { id: "1", title: "Deep Purple - Smoke on the water", instrument: 0, filePath: "/sample.gp5", pinGenres: [{ genre: { name: "Rock" } }] },
    { id: "2", title: "Ocipieje", instrument: 1, filePath: "/heavy.gp5", pinGenres: [{ genre: { name: "Metal" } }] }
  ];
};

const handleSearch = (query) => {
  filters.search = query;
};

const resetFilters = () => {
  filters.instrument = 'all';
  filters.genre = 'all';
  filters.search = '';
  filters.sortBy = 'newest';
  filters.sortOrder = 'desc';
};

// Pobieranie lub generowanie unikalnego ID gościa dla polubień
const getVisitorId = () => {
  let id = localStorage.getItem('visitorId');
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem('visitorId', id);
  }
  return id;
};

// Obsługa scrolla
const handleScroll = () => {
  const bottomOfWindow = document.documentElement.scrollTop + window.innerHeight >= document.documentElement.offsetHeight - 200;
  if (bottomOfWindow && !loadingMore.value && hasMore.value) {
    fetchPins();
  }
};

// Resetuj paginację gdy zmieniają się filtry
watch(() => [filters.instrument, filters.genre, filters.sortBy, filters.sortOrder], () => {
  fetchPins(true);
});

onMounted(() => {
  // Inicjalizacja visitorId
  getVisitorId();
  fetchGenres();
  window.addEventListener('scroll', handleScroll);

  if (apiUrl) fetchPins();
  else {
    loadMockData();
    loading.value = false;
  }
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});
</script>

<template>
  <div class="app-wrapper">
    <TopBar @search="handleSearch" />
    
    <main class="main-content">
      <!-- PANEL FILTROWANIA -->
      <div class="filter-panel">
        <div class="filter-group">
          <label>Nazwa:</label>
          <input 
            v-model="filters.search" 
            type="text" 
            placeholder="Szukaj utworu..." 
            class="filter-input"
          />
        </div>

        <div class="filter-group">
          <label>Instrument:</label>
          <select v-model="filters.instrument">
            <option value="all">Wszystkie</option>
            <option v-for="(name, val) in instrumentMap" :key="val" :value="val">
              {{ name }}
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label>Gatunek:</label>
          <select v-model="filters.genre">
            <option value="all">Wszystkie</option>
            <option v-for="genre in availableGenres" :key="genre" :value="genre">
              {{ genre }}
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label>Sortuj:</label>
          <select v-model="filters.sortBy">
            <option value="newest">Najnowsze</option>
            <option value="alpha">Alfabetycznie (A-Z)</option>
            <option value="likes">Najwięcej polubień</option>
          </select>
        </div>

        <div class="filter-group">
          <label>Kierunek:</label>
          <select v-model="filters.sortOrder">
            <option value="desc">Malejąco</option>
            <option value="asc">Rosnąco</option>
          </select>
        </div>

        <button class="reset-btn" @click="resetFilters">Wyczyść</button>
        
        <div class="results-count">
          Znaleziono: {{ filteredPins.length }}
        </div>
      </div>

      <!-- Loader -->
      <div v-if="loading" class="loader">
        <div class="spinner"></div>
        <span>Ładowanie riffów...</span>
      </div>

      <!-- Komunikat o braku wyników -->
      <div v-else-if="filteredPins.length === 0 && !loadingMore" class="no-results">
        Nie znaleziono pinów spełniających kryteria.
      </div>

      <!-- Siatka pinów - Używamy filteredPins zamiast pins -->
      <div v-else class="pin-grid">
        <Pin 
          v-for="item in filteredPins" 
          :key="item.id" 
          :pin="item" 
        />
      </div>

      <div v-if="loadingMore && !loading" class="scroll-loader">
        <div class="spinner small"></div>
        <span>Wczytywanie kolejnych utworów...</span>
      </div>
    </main>
  </div>
</template>

<style scoped>
.filter-panel {
  background: white;
  padding: 15px 20px;
  margin-bottom: 20px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.filter-group label {
  font-weight: bold;
  font-size: 0.9rem;
  color: #444;
}

.filter-group select, .filter-group input {
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid #ddd;
  background-color: #fff;
  outline: none;
}

.filter-group input {
  width: 200px;
}

.reset-btn {
  padding: 8px 15px;
  background: #f0f0f0;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.2s;
}

.reset-btn:hover {
  background: #e0e0e0;
}

.results-count {
  margin-left: auto;
  color: #888;
  font-size: 0.9rem;
}

.loader, .no-results {
  text-align: center;
  padding: 50px;
  font-size: 1.2rem;
  color: #666;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
}

.scroll-loader {
  text-align: center;
  padding: 20px;
  color: #888;
  font-style: italic;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #2ecc71;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.spinner.small {
  width: 20px;
  height: 20px;
  border-width: 2px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.main-content {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.pin-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 25px;
}

/* Responsywność dla małych ekranów */
@media (max-width: 600px) {
  .filter-panel {
    flex-direction: column;
    align-items: flex-start;
  }
  .results-count {
    margin-left: 0;
  }
}
</style>