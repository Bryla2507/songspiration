<template>
  <div class="at-container">
    <div class="at-header">
      <!-- JEDEN PRZYCISK AKCJI -->
      <div class="at-action-wrapper">
        <button @click="handleAction" :class="['btn-action', { 'is-playing': isPlaying }]">
          {{ isPlaying ? 'Stop' : 'Graj' }}
        </button>
      </div>

      <!-- METADANE UTWORU -->
      <div class="at-track-info">
        <div class="at-main-row">
          <h2 class="at-title">Poprawione Takty</h2>
          <span class="at-bpm-badge">{{ bpmValue }} BPM</span>
        </div>
        <div class="at-tags">
          <span class="at-tag">Gitara</span>
          <span class="at-tag">Ćwiczenie</span>
          <span class="at-tag">Lick</span>
        </div>
      </div>
    </div>

    <!-- WIDOK TABULATURY -->
    <div class="tab-viewport">
      <div ref="atElement">
        \title "Pętla"
        \tempo 140
        \ts 4 4
        .
        :4 0.6 1.6 3.6 0.5 | :4 2.5 3.5 0.4 2.4 |
        :4 3.4 0.3 2.3 0.2 | :4 1.2 3.2 0.1 1.1 |
        :4 3.1 2.1 1.1 0.1 |
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, shallowRef, onMounted, onUnmounted } from 'vue';
import * as alphaTab from '@coderline/alphatab';

const atElement = ref(null);
const isPlaying = ref(false);
const isManualStop = ref(false);
const api = shallowRef(null);
const bpmValue = 140;

let wasPlaying = false; 

onMounted(() => {
  api.value = new alphaTab.AlphaTabApi(atElement.value, {
    tex: true,
    player: {
      enablePlayer: true,
      soundFont: 'https://cdn.jsdelivr.net/npm/@coderline/alphatab@latest/dist/soundfont/sonivox.sf2',
      enableCursor: true,
    },
    display: {
      layoutMode: 'horizontal',
      resources: {
        fontDirectory: '/font/' 
      }
    }
  });

  api.value.isLooping = true;

  api.value.playerStateChanged.on((args) => {
    const currentIsPlaying = args.state === 1;

    // Logika pętli
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

onUnmounted(() => {
  api.value?.destroy();
});
</script>

<style scoped>
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

/* NAGŁÓWEK Z PRZYCISKIEM I INFAMI */
.at-header {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 1px solid #f0f3f6;
}

/* PRZYCISK */
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

/* INFO O UTWORZE */
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

.at-bpm-badge {
  background: #f1f8ff;
  color: #0366d6;
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  border: 1px solid #c8e1ff;
}

/* TAGI */
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

/* OBSZAR TABULATURY */
.tab-viewport {
  position: relative;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  overflow-x: auto;
  background: #fff;
  min-height: 180px;
}

/* KURSOR PIONOWY */
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
  background: rgba(0, 0, 0, 0.05) !important;
}

/* SCROLLBAR */
.tab-viewport::-webkit-scrollbar {
  height: 8px;
}
.tab-viewport::-webkit-scrollbar-thumb {
  background: #ddd;
  border-radius: 10px;
}
</style>