<template>
  <section class="pins-display-area">
    <h3 class="area-title">Twoja twórczość</h3>
    <div class="stats-row">
      <span>Piny: <strong>{{ stats.pins || 0 }}</strong></span>
      <span>Polubienia: <strong>{{ stats.likes || 0 }}</strong></span>
    </div>
    <div v-if="pins.length > 0" class="pins-grid">
      <div v-for="pin in pins" :key="pin.id" class="pin-card-simple" @click="$emit('open-pin', pin)">
        <div class="pin-icon">🎵</div>
        <h4>{{ pin.title }}</h4>
        <p class="pin-meta">{{ getInstrumentName(pin.instrument) }}</p>
      </div>
    </div>
    <div v-else class="empty-state">Nie dodałeś jeszcze żadnych pinów.</div>
  </section>
</template>

<script setup>
defineProps(['pins', 'stats']);
defineEmits(['open-pin']);

const getInstrumentName = (id) => {
  const names = { 0: 'Gitara', 1: 'Bas', 2: 'Perkusja' };
  return names[id] || 'Instrument';
};
</script>

<style scoped>
.pins-display-area { background: #ffffff; border-radius: 20px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.02); }
.area-title { font-size: 20px; margin-bottom: 20px; }
.stats-row { margin-bottom: 20px; display: flex; gap: 20px; font-size: 14px; color: #64748b; }
.pins-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 20px; }
.pin-card-simple { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; padding: 20px; text-align: center; transition: 0.2s; cursor: pointer; }
.pin-card-simple:hover { transform: translateY(-3px); border-color: #2ecc71; background: #fff; }
.pin-icon { font-size: 24px; margin-bottom: 10px; }
.empty-state { text-align: center; padding: 40px; color: #94a3b8; }
</style>