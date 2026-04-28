<template>
  <nav class="top-bar">
    <div class="top-bar-content">
      <!-- LEWA STRONA: LOGO -->
      <div class="brand">
        <span class="logo-icon">🎸</span>
        <h1 class="logo-text">Song<span>Spiration</span></h1>
      </div>

      <!-- ŚRODEK: WYSZUKIWARKA -->
      <div class="search-container">
        <div class="search-input-wrapper">
          <span class="search-icon">🔍</span>
          <input 
            type="text" 
            placeholder="Szukaj riffów, gatunków, instrumentów..." 
            v-model="searchQuery"
            @input="handleSearch"
          />
        </div>
      </div>

      <!-- PRAWA STRONA: NAWIGACJA -->
      <div class="nav-actions">
        <button class="btn-create">+ Dodaj Pin</button>
        <div class="user-profile">
          <div class="avatar">JD</div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue';

const searchQuery = ref('');
const emit = defineEmits(['search']);

const handleSearch = () => {
  emit('search', searchQuery.value);
};
</script>

<style scoped>
.top-bar {
  background: #ffffff;
  height: 70px;
  display: flex;
  align-items: center;
  border-bottom: 1px solid #eaedf0;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
  position: sticky;
  top: 0;
  z-index: 1000;
  padding: 0 20px;
}

.top-bar-content {
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
}

/* BRAND / LOGO */
.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
}

.logo-icon {
  font-size: 24px;
}

.logo-text {
  font-size: 20px;
  font-weight: 900;
  color: #1a1d21;
  margin: 0;
  letter-spacing: -0.5px;
}

.logo-text span {
  color: #2ecc71; /* Kolor zielony z Twojego przycisku */
}

/* WYSZUKIWARKA */
.search-container {
  flex: 1;
  max-width: 600px;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 15px;
  font-size: 14px;
  color: #9ca3af;
}

.search-input-wrapper input {
  width: 100%;
  padding: 10px 15px 10px 40px;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  background: #f9fafb;
  font-size: 14px;
  transition: all 0.2s;
}

.search-input-wrapper input:focus {
  outline: none;
  background: #fff;
  border-color: #2ecc71;
  box-shadow: 0 0 0 4px rgba(46, 204, 113, 0.1);
}

/* AKCJE */
.nav-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.btn-nav {
  background: none;
  border: none;
  color: #4b5563;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 8px;
  transition: background 0.2s;
}

.btn-nav:hover {
  background: #f3f4f6;
}

.btn-create {
  background: #2ecc71;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 10px;
  font-weight: 700;
  font-size: 14px;
  cursor: pointer;
  transition: transform 0.2s;
}

.btn-create:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
}

.avatar {
  width: 38px;
  height: 38px;
  background: #e5e7eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  color: #4b5563;
  border: 2px solid #fff;
  box-shadow: 0 0 0 1px #e5e7eb;
  cursor: pointer;
}

/* Responsywność */
@media (max-width: 768px) {
  .search-container, .btn-nav {
    display: none;
  }
  .logo-text {
    font-size: 16px;
  }
}
</style>