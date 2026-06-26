<template>
  <section class="pins-display-area">
    <h3 class="area-title">{{ title }}</h3>
    
    <div v-if="stats" class="stats-row">
      <span>Piny: <strong>{{ stats.pins || 0 }}</strong></span>
      <span>Polubienia: <strong>{{ stats.likes || 0 }}</strong></span>
    </div>

    <div v-if="pins.length > 0" class="pins-grid">
      <div v-for="pin in pins" :key="pin.id" class="pin-card-simple" @click="$emit('open-pin', pin)">
        
        <div v-if="isOwner" class="pin-actions" @click.stop>
          
          <template v-if="activeTab === 'tworczosc'">
            <button class="action-btn" @click="$emit('change-visibility', pin)" title="Zmień widoczność">
              <span v-if="pin.visibility === 0">🌍</span> <span v-else-if="pin.visibility === 1">🔒</span> <span v-else>🔗</span> </button>
            <button class="action-btn delete" @click.stop="$emit('delete-pin', pin)" title="Usuń pin">🗑️</button>
          </template>

          <template v-if="activeTab === 'polubione'">
            <button class="action-btn unlike" @click="$emit('unlike-pin', pin)" title="Usuń z polubionych">💔</button>
          </template>

        </div>

        <div class="pin-icon">🎵</div>
        <h4>{{ pin.title }}</h4>
        <p v-if="pin.description" class="pin-description">{{ pin.description }}</p>
        <p class="pin-meta">{{ getInstrumentName(pin.instrument) }}</p>
      </div>
    </div>
    <div v-else class="empty-state">Brak utworów do wyświetlenia w tej sekcji.</div>
  </section>
</template>

<script setup>
// Dodano 'isOwner' i 'activeTab' do propsów
defineProps(['pins', 'stats', 'title', 'isOwner', 'activeTab']);
// Dodano nowe zdarzenia (emity) do obsługi przycisków
defineEmits(['open-pin', 'change-visibility', 'delete-pin', 'unlike-pin']);

const getInstrumentName = (id) => {
  const names = { 0: 'Gitara', 1: 'Bas', 2: 'Perkusja' };
  return names[id] || 'Instrument';
};
</script>

<style scoped>
.pins-display-area { background: #ffffff; border-radius: 20px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.02); }
.area-title { font-size: 20px; margin-bottom: 20px; font-weight: 700; }
.stats-row { margin-bottom: 20px; display: flex; gap: 20px; font-size: 14px; color: #64748b; }
.pins-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 20px; }

/* Dodano position relative, żeby ładnie umieścić przyciski w rogu */
.pin-card-simple { position: relative; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; padding: 20px; text-align: center; transition: 0.2s; cursor: pointer; }
.pin-card-simple:hover { transform: translateY(-3px); border-color: #2ecc71; background: #fff; }
.pin-icon { font-size: 24px; margin-bottom: 10px; }
.pin-description { font-size: 13px; color: #64748b; margin-top: -5px; margin-bottom: 10px; }
.empty-state { text-align: center; padding: 40px; color: #94a3b8; }

/* STYLOWANIE PRZYCISKÓW AKCJI */
.pin-actions { position: absolute; top: 10px; right: 10px; display: flex; gap: 5px; opacity: 0; transition: opacity 0.2s; }
.pin-card-simple:hover .pin-actions { opacity: 1; } /* Pojawiają się po najechaniu myszką */
.action-btn { background: white; border: 1px solid #e2e8f0; border-radius: 6px; padding: 5px; cursor: pointer; transition: 0.2s; display: flex; align-items: center; justify-content: center; }
.action-btn:hover { background: #f1f5f9; transform: scale(1.1); }
.action-btn.delete:hover { background: #fee2e2; border-color: #ef4444; }
.action-btn.unlike:hover { background: #fce7f3; border-color: #ec4899; }
</style>
