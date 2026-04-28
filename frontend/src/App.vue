<script setup>
import TopBar from './components/TopBar.vue';
import Pin from './components/Pin.vue';
import { ref } from 'vue';

const pins = ref([
  {
    id: "1",
    title: "Deep Purple - Smoke on the water",
    instrument: 0, 
    filePath: "/sample_pin.gp5", 
    pinGenres: [{ genre: { name: "Rock" } }, { genre: { name: "Classic" } }]
  },
  {
    id: "2",
    title: "Ocipieje",
    instrument: 2,
    filePath: "/heavy.gp5",
    pinGenres: [{ genre: { name: "Metal" } }, { genre: { name: "Progressive Metal" } }]
  },
  {
    id: "3",
    title: "funky bass",
    instrument: 1,
    filePath: "/bass.gp5",
    pinGenres: [{ genre: { name: "Sraka" } }]
  },
    {
    id: "4",
    title: "Gotki skaczo",
    instrument: 2,
    filePath: "/drums.gp5",
    pinGenres: [{ genre: { name: "Goth" } }]
  }
]);

const handleSearch = (query) => {
  console.log("Szukam:", query);
};
</script>

<template>
  <div class="app-wrapper">
    <TopBar @search="handleSearch" />
    
    <main class="main-content">
      <div class="pin-grid">
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
/* Reset globalny */
body {
  margin: 0;
  padding: 0;
  background-color: #f4f7f9;
  font-family: sans-serif;
}

.main-content {
  padding: 20px;
  /* Pozwalamy kontenerowi rosnąć do pełnej szerokości okna */
  width: 100%;
  max-width: 100%; 
  box-sizing: border-box;
}

/* ELASTYCZNA SIATKA USTAWIONA NA PEŁNĄ SZEROKOŚĆ */
.pin-grid {
  display: grid;
  /* 
     Zmieniamy minmax na 100%, aby każdy pin zawsze zajmował pełną szerokość.
     Dzięki '1fr' piny będą się rozciągać do krawędzi ekranu.
  */
  grid-template-columns: 1fr;
  gap: 20px;
}

/* 
   WYMUSZENIE BRAKU SCROLLA (Globalne nadpisanie dla komponentów Pin) 
   Używamy selektora głębokiego, aby dostać się do wnętrza komponentów Pin.vue
*/
.pin-grid :deep(.tab-viewport) {
  overflow-x: hidden !important; /* Całkowite wyłączenie poziomego scrolla */
  overflow-y: hidden !important; /* Całkowite wyłączenie pionowego scrolla */
}

.pin-grid :deep(.at-container) {
  width: 100% !important;
  max-width: none !important; /* Usuwamy wszelkie ograniczenia szerokości */
}

/* Responsywność dla urządzeń mobilnych */
@media (max-width: 600px) {
  .main-content {
    padding: 10px;
  }
}
</style>