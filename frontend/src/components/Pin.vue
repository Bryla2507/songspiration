<template>
  <div class="at-container">
    <div class="at-header">
      <!-- JEDEN PRZYCISK AKCJI -->
      <div class="at-action-wrapper">
        <button @click="handleAction" :class="['btn-action', { 'is-playing': isPlaying }]">
          {{ isPlaying ? 'Stop' : 'Graj' }}
        </button>
      </div>

      <!-- METADANE UTWORU Z PROPÓW -->
      <div class="at-track-info">
         <div class="at-main-row">
           <h2 class="at-title">{{ pin.title }}</h2>
           <div class="at-actions">
             <button @click="toggleLike" :class="['btn-like', { 'is-liked': isLiked }]">
               <span class="heart-icon">{{ isLiked ? '❤️' : '🤍' }}</span>
               <span class="like-count">{{ currentLikeCount }}</span>
             </button>
             <button @click="downloadPin" class="btn-download" title="Pobierz plik">
               <span class="download-icon">📥</span>
               <span class="download-count">{{ pin.downloadsCount || 0 }}</span>
             </button>
           </div>
           <!-- Wyświetlanie dynamicznego BPM -->
         </div>
        <div class="at-tags">
          <span class="at-tag">{{ getInstrumentName(pin.instrument) }}</span>
          <span v-for="pg in pin.pinGenres" :key="pg.genreId" class="at-tag">
            {{ pg.genre?.name }}
          </span>
        </div>
      </div>
    </div>

    <!-- WIDOK TABULATURY -->
    <div class="tab-viewport">
      <div v-if="!pin.filePath || !pin.filePath.startsWith('http')" class="missing-file-message">
        🎵 Plik tabulatury niedostępny
      </div>
      <div v-else ref="atElement"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, shallowRef, onMounted, onUnmounted } from 'vue';
import * as alphaTab from '@coderline/alphatab';

const props = defineProps({
  pin: {
    type: Object,
    required: true
  }
});

const isLiked = ref(props.pin.isLiked || false);
const currentLikeCount = ref(props.pin.likeCount || 0);
const apiUrl = import.meta.env.VITE_API_URL;

const atElement = ref(null);
const isPlaying = ref(false);
const isManualStop = ref(false);
const api = shallowRef(null);

// ZMIANA: bpmValue jest teraz reaktywne
const bpmValue = ref(0);

let wasPlaying = false; 

const getInstrumentName = (id) => {
  const names = { 0: 'Gitara', 1: 'Bas', 2: 'Perkusja' };
  return names[id] || 'Instrument';
};

onMounted(() => {
  // Handle missing files gracefully
  if (!props.pin.filePath || !props.pin.filePath.startsWith('http')) {
    console.warn(`Missing or invalid file path for pin: ${props.pin.title}`);
    return;
  }

  api.value = new alphaTab.AlphaTabApi(atElement.value, {
    file: props.pin.filePath,
    player: {
      enablePlayer: true,
      soundFont: 'https://cdn.jsdelivr.net/npm/@coderline/alphatab@latest/dist/soundfont/sonivox.sf2',
      enableCursor: true,
    },
    display: {
    displayTempo: false,
    //   staveProfile: 'Tab',
      layoutMode: 'horizontal',
      resources: {
        fontDirectory: '/font/'
      }
    }
  });

  // NOWA LOGIKA: Pobieranie BPM z pliku po załadowaniu
  api.value.scoreLoaded.on((score) => {
    bpmValue.value = Math.round(score.tempo);
  });

  api.value.isLooping = true;

  api.value.playerStateChanged.on((args) => {
    const currentIsPlaying = args.state === 1;

    if (args.state === 0 && wasPlaying && !isManualStop.value) {
        setTimeout(() => {
            if (api.value) {
                api.value.timePosition = 0; 
                api.value.play();           
            }
        }, 50);
    }

    isPlaying.value = currentIsPlaying;
    wasPlaying = currentIsPlaying;
  });


});

const handleAction = () => {
  if (isPlaying.value) {
    isManualStop.value = true;
    api.value?.stop();
    api.value.timePosition = 0;
  } else {
    isManualStop.value = false;
    api.value?.play();
  }
};

const toggleLike = async () => {
  const visitorId = localStorage.getItem('visitorId');
  const token = sessionStorage.getItem('token');

  try {
    const headers = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const response = await fetch(`${apiUrl}/api/Pins/${props.pin.id}/toggle-like?userId=${visitorId}`, {
      method: 'POST',
      headers: headers
    });

    if (response.ok) {
      const data = await response.json();
      isLiked.value = data.isLiked;
      currentLikeCount.value = data.likeCount;

      // Aktualizacja localStorage, aby HomeView wiedział o zmianie bez przeładowania
      const likedPins = JSON.parse(localStorage.getItem('likedPins') || '[]');
      if (isLiked.value) {
        if (!likedPins.includes(props.pin.id)) likedPins.push(props.pin.id);
      } else {
        const index = likedPins.indexOf(props.pin.id);
        if (index > -1) likedPins.splice(index, 1);
      }
      localStorage.setItem('likedPins', JSON.stringify(likedPins));
    }
  } catch (error) {
    console.error("Błąd podczas lajkowania:", error);
  }
};

const downloadPin = async () => {
  try {
    const token = sessionStorage.getItem('token');
    const headers = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;

    // Call the download endpoint which will increment the download count
    const response = await fetch(`${apiUrl}/api/Pins/${props.pin.id}/download`, {
      method: 'GET',
      headers: headers
    });

    if (response.ok) {
      // Create a blob from the response and trigger download
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = props.pin.filename || `pin_${props.pin.id}.gp`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);

      // Optionally update the download count display
      if (props.pin.downloadsCount !== undefined) {
        props.pin.downloadsCount = (props.pin.downloadsCount || 0) + 1;
      }
    } else {
      console.error("Failed to download file");
    }
  } catch (error) {
    console.error("Error during download:", error);
  }
};

onUnmounted(() => {
  api.value?.destroy();
});
</script>

<style scoped>
/* Stylizacja bez zmian, tak jak prosiłeś */
.at-container {
  max-width: 100%;
  margin: 20px auto;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  padding: 20px;
  border: 1px solid #e1e4e8;
}

.at-header {
  display: flex;
  align-items: center;
  gap: 20px;
  border-bottom: 1px solid #f0f3f6;
}

.btn-action {
  padding: 12px 35px;
  font-size: 16px;
  font-weight: 700;
  border-radius: 50px;
  cursor: pointer;
  border: none;
  background: #2ecc71;
  color: white;
  transition: all 0.2s ease;
  min-width: 120px;
  box-shadow: 0 2px 10px rgba(46, 204, 113, 0.3);
}

.btn-action.is-playing {
  background: #e74c3c;
  box-shadow: 0 2px 10px rgba(231, 76, 60, 0.3);
}

.btn-action:hover {
  transform: translateY(-2px);
  filter: brightness(1.05);
}

.at-track-info {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.at-main-row {
  display: flex;
  align-items: center;
  gap: 12px;
}

.at-title {
  margin: 0;
  font-size: 20px;
  font-weight: 800;
  color: #24292e;
}

.btn-like {
  background: none;
  border: 1px solid #eee;
  border-radius: 20px;
  padding: 4px 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s;
  margin-left: 10px;
}

.btn-like:hover { background: #f8f9fa; transform: scale(1.05); }
.btn-like.is-liked { border-color: #ffb3b3; background: #fff5f5; }

.btn-download {
  background: none;
  border: 1px solid #eee;
  border-radius: 20px;
  padding: 4px 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s;
}

.btn-download:hover { background: #f8f9fa; transform: scale(1.05); }
.btn-download:active { transform: scale(0.98); }

.heart-icon { font-size: 1.1rem; }
.like-count { font-size: 0.9rem; font-weight: 700; color: #666; }

.download-icon { font-size: 1.1rem; }
.download-count { font-size: 0.9rem; font-weight: 700; color: #666; }

.at-bpm-badge {
  background: #f1f8ff;
  color: #0366d6;
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  border: 1px solid #c8e1ff;
}

.at-tags {
  display: flex;
  gap: 8px;
}

.at-tag {
  font-size: 11px;
  color: #586069;
  background: #f6f8fa;
  padding: 2px 8px;
  border-radius: 12px;
  border: 1px solid #e1e4e8;
}

.tab-viewport {
  position: relative;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  overflow-x: auto;
  background: #fff;
  min-height: 180px;
}

.missing-file-message {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
  font-style: italic;
  font-size: 16px;
  padding: 20px;
  text-align: center;
}

:deep(.at-cursor) {
  width: 2px !important;
  background-color: #000 !important;
  opacity: 1 !important;
  display: block !important;
  height: 100% !important;
  top: 0 !important;
  z-index: 100;
}

:deep(.at-cursor-beat) {
  background: rgba(46, 204, 113, 0.5) !important;
}

.tab-viewport::-webkit-scrollbar {
  height: 4px;
}
.tab-viewport::-webkit-scrollbar-thumb {
  background: #ddd;
  border-radius: 10px;
}


</style>