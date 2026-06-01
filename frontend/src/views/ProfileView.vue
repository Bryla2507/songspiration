<template>
  <div class="profile-page">
    <nav class="profile-top-nav">
      <div class="logo-wrapper" @click="$router.push('/')">
        <h1 class="logo-text">Song<span>Spiration</span></h1>
      </div>
    </nav>

    <div class="profile-container" v-if="user">
      <ProfileHeader
        :user="user"
        :isOwner="isOwner"
        @edit="showEditModal = true"
        @change-avatar="handleAvatarClick"
        @report="showReportModal = true"
        @delete="showDeleteModal = true"
      />

      <div class="profile-tabs">
        <button 
          :class="{ active: activeTab === 'tworczosc' }" 
          @click="activeTab = 'tworczosc'"
        >
          {{ isOwner ? '🎵 Moja twórczość' : '🎵 Piny użytkownika' }}
        </button>
        <button 
          :class="{ active: activeTab === 'polubione' }" 
          @click="activeTab = 'polubione'"
        >
          ❤️ Polubione piny
        </button>
      </div>

      <div class="filter-panel">
        <div class="filter-group">
          <label>Sortuj według:</label>
          <select v-model="sortBy">
            <option value="newest">Najnowsze</option>
            <option value="oldest">Najstarsze</option>
            <option v-if="activeTab === 'tworczosc'" value="likes">Najwięcej polubień</option>
          </select>
        </div>
        <div class="results-count">
          Znaleziono: {{ pins.length }} pinów
        </div>
      </div>

      <PinGrid 
        :pins="pins" 
        :title="activeTab === 'tworczosc' 
          ? (isOwner ? 'Twoja twórczość' : 'Piny użytkownika') 
          : 'Polubione piny'"
        :stats="activeTab === 'tworczosc' ? { 
          pins: user.addedPinsCount || user.AddedPinsCount || 0, 
          likes: user.totalLikesReceived || user.TotalLikesReceived || 0 
        } : null"
        :isOwner="isOwner"
        :activeTab="activeTab"
        @open-pin="openApiModal"
        @delete-pin="promptDeletePin"
        @change-visibility="promptChangeVisibility"
        @unlike-pin="promptUnlikePin"
      />

    <div v-if="pinToDelete" class="modal-overlay" @click.self="pinToDelete = null">
      <div class="modal-card">
        <div class="modal-header"><h3>⚠️ Usuwanie pinu</h3></div>
        <div class="modal-body"><p>Czy na pewno chcesz usunąć utwór "<strong>{{ pinToDelete.title }}</strong>"? Tego nie da się cofnąć.</p></div>
        <div class="modal-footer">
          <button @click="pinToDelete = null" class="btn-modal-cancel">Anuluj</button>
          <button @click="executeDeletePin" class="btn-modal-confirm">Tak, usuń</button>
        </div>
      </div>
    </div>

    <div v-if="pinToUnlike" class="modal-overlay" @click.self="pinToUnlike = null">
      <div class="modal-card">
        <div class="modal-header"><h3>💔 Usuwanie polubienia</h3></div>
        <div class="modal-body"><p>Odebrać polubienie dla "<strong>{{ pinToUnlike.title }}</strong>"?</p></div>
        <div class="modal-footer">
          <button @click="pinToUnlike = null" class="btn-modal-cancel">Anuluj</button>
          <button @click="executeUnlikePin" class="btn-modal-confirm">Odbierz polubienie</button>
        </div>
      </div>
    </div>

    <div v-if="pinToChangeVisibility" class="modal-overlay" @click.self="pinToChangeVisibility = null">
      <div class="modal-card">
        <div class="modal-header"><h3>👁️ Zmiana widoczności</h3></div>
        <div class="modal-body">
          <p>Ustaw nową widoczność dla "<strong>{{ pinToChangeVisibility.title }}</strong>":</p>
          <div class="filter-group" style="margin-top: 15px;">
            <select v-model="newVisibilitySelection" style="width: 100%;">
              <option :value="0">🌍 Publiczny (widoczny dla wszystkich)</option>
              <option :value="1">🔒 Prywatny (tylko dla Ciebie)</option>
              <option :value="2">🔗 Niepubliczny (dostępny tylko z linku)</option>
            </select>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="pinToChangeVisibility = null" class="btn-modal-cancel">Anuluj</button>
          <button @click="executeChangeVisibility" class="btn-modal-confirm save">Zapisz zmianę</button>
        </div>
      </div>
    </div>

    </div>
    
    <div v-else class="loading-screen">Ładowanie profilu...</div>

    <EditProfileModal 
      v-if="showEditModal" 
      :user="user" 
      @close="showEditModal = false" 
      @save="executeEdit" 
    />

    <div v-if="showAvatarModal" class="modal-overlay" @click.self="closeAvatarModal">
      <div class="modal-card">
        <div class="modal-header"><h3>Zmień zdjęcie profilowe</h3></div>
        <div class="modal-body">
          <div 
            class="drop-zone"
            :class="{ 'drop-zone--active': isDragging }"
            @dragover.prevent="isDragging = true"
            @dragleave.prevent="isDragging = false"
            @drop.prevent="handleFileDrop"
            @click="triggerFileInput"
          >
            <p v-if="!selectedFile">Przeciągnij zdjęcie tutaj lub <span>kliknij, aby wybrać</span></p>
            <div v-else class="file-preview">
              <p>Wybrano: <strong>{{ selectedFile.name }}</strong></p>
            </div>
            <input type="file" ref="fileInput" hidden @change="handleFileSelect" accept="image/*" />
          </div>
        </div>
        <div class="modal-footer">
          <button @click="closeAvatarModal" class="btn-modal-cancel">Anuluj</button>
          <button @click="uploadAvatar" class="btn-modal-confirm save" :disabled="!selectedFile">Ustaw zdjęcie</button>
        </div>
      </div>
    </div>

    <TabPlayerModal 
      v-if="showApiModal" 
      :pin="selectedPin" 
      :apiUrl="apiUrl" 
      @close="closeApiModal" 
    />

    <div v-if="showDeleteModal" class="modal-overlay" @click.self="showDeleteModal = false">
      <div class="modal-card">
        <div class="modal-header"><h3>⚠️ Usuwanie konta</h3></div>
        <div class="modal-body"><p>Czy na pewno chcesz usunąć konto? Tej operacji nie da się cofnąć.</p></div>
        <div class="modal-footer">
          <button @click="showDeleteModal = false" class="btn-modal-cancel">Anuluj</button>
          <button @click="executeDelete" class="btn-modal-confirm">Tak, usuń konto</button>
        </div>
      </div>
    </div>

    <ReportModal
      v-if="showReportModal"
      :userId="route.params.id"
      @close="showReportModal = false"
      @success="fetchData"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import ProfileHeader from '@/components/profile/ProfileHeader.vue';
import PinGrid from '@/components/profile/PinGrid.vue';
import EditProfileModal from '@/components/modals/EditProfileModal.vue';
import TabPlayerModal from '@/components/modals/TabPlayerModal.vue';
import ReportModal from '@/components/modals/ReportModal.vue';

const route = useRoute();
const router = useRouter();
const user = ref(null);
const pins = ref([]);
const apiUrl = import.meta.env.VITE_API_URL;

const showDeleteModal = ref(false);
const showEditModal = ref(false);
const showAvatarModal = ref(false);
const showApiModal = ref(false);
const showReportModal = ref(false);
const isDragging = ref(false);
const selectedFile = ref(null);
const fileInput = ref(null);
const selectedPin = ref(null);

// STANY DLA ZARZĄDZANIA PINAMI
const pinToDelete = ref(null);
const pinToUnlike = ref(null);
const pinToChangeVisibility = ref(null);
const newVisibilitySelection = ref(0);

// STAN ZAKŁADEK I FILTRÓW
const activeTab = ref('tworczosc'); // 'tworczosc' lub 'polubione'
const sortBy = ref('newest');       // 'newest', 'oldest', 'likes'

const isOwner = computed(() => {
  const loggedInId = sessionStorage.getItem('userId');
  return route.params.id === loggedInId;
});

// Watcher zabezpieczający: resetuje sortowanie na "najnowsze", jeśli użytkownik przełączy na 
// zakładkę polubionych mając zaznaczone sortowanie po lajkach (które tam nie istnieje)
watch(activeTab, (newTab) => {
  if (newTab === 'polubione' && sortBy.value === 'likes') {
    sortBy.value = 'newest';
  }
});

// Automatyczne pobieranie nowej listy przy zmianie zakładki lub kryterium sortowania
watch([activeTab, sortBy], () => {
  fetchPins();
});

const fetchUserData = async () => {
  const userId = route.params.id;
  const token = sessionStorage.getItem('token');
  try {
    const userRes = await fetch(`${apiUrl}/api/Users/profile/${userId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (userRes.ok) user.value = await userRes.json();
  } catch (error) { console.error(error); }
};

const fetchPins = async () => {
  const userId = route.params.id;
  const token = sessionStorage.getItem('token');
  
  // Tłumaczenie uproszczonego pola select na parametry backendu (tak jak na Twojej stronie głównej)
  let backendSortBy = 'newest';
  let backendSortOrder = 'desc';

  if (sortBy.value === 'oldest') {
    backendSortBy = 'newest';
    backendSortOrder = 'asc';
  } else if (sortBy.value === 'likes') {
    backendSortBy = 'likes';
    backendSortOrder = 'desc';
  }

  const queryParams = new URLSearchParams({
    sortBy: backendSortBy,
    sortOrder: backendSortOrder
  });

  const endpoint = activeTab.value === 'tworczosc'
    ? `${apiUrl}/api/Pins/user/${userId}?${queryParams.toString()}`
    : `${apiUrl}/api/Pins/user/${userId}/liked?${queryParams.toString()}`; // <--- Poprawione na strukturę z Twojego C#

  try {
    const pinsRes = await fetch(endpoint, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (pinsRes.ok) pins.value = await pinsRes.json();
  } catch (error) { console.error(error); }
};

const fetchData = async () => {
  await fetchUserData();
  await fetchPins();
};

const executeEdit = async (formData) => {
  try {
    const token = sessionStorage.getItem('token');
    const res = await fetch(`${apiUrl}/api/Users/profile/${user.value.id || user.value.Id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
      body: JSON.stringify(formData)
    });
    if (res.ok) {
      Object.assign(user.value, formData);
      showEditModal.value = false;
    }
  } catch (e) { console.error(e); }
};

const handleAvatarClick = () => { showAvatarModal.value = true; };
const triggerFileInput = () => fileInput.value?.click();
const handleFileSelect = (e) => { if (e.target.files.length > 0) selectedFile.value = e.target.files[0]; };
const handleFileDrop = (e) => { isDragging.value = false; if (e.dataTransfer.files.length > 0) selectedFile.value = e.dataTransfer.files[0]; };

const uploadAvatar = async () => {
  if (!selectedFile.value) return;
  const formData = new FormData();
  formData.append('avatar', selectedFile.value);
  try {
    const token = sessionStorage.getItem('token');
    const res = await fetch(`${apiUrl}/api/Users/profile/${user.value.id || user.value.Id}/avatar`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData
    });
    if (res.ok) { closeAvatarModal(); fetchData(); }
  } catch (e) { console.error(e); }
};

const closeAvatarModal = () => { showAvatarModal.value = false; selectedFile.value = null; };

const executeDelete = async () => {
  try {
    const id = user.value.id || user.value.Id;
    const res = await fetch(`${apiUrl}/api/Users/${id}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${sessionStorage.getItem('token')}` }
    });
    if (res.ok) { sessionStorage.clear(); router.push('/login'); }
  } catch (e) { console.error(e); }
};

const openApiModal = (pin) => { selectedPin.value = pin; showApiModal.value = true; };
const closeApiModal = () => { showApiModal.value = false; selectedPin.value = null; };

// --- ZARZĄDZANIE PINAMI LOGIKA ---

const promptDeletePin = (pin) => { pinToDelete.value = pin; };
const promptUnlikePin = (pin) => { pinToUnlike.value = pin; };
const promptChangeVisibility = (pin) => { 
  pinToChangeVisibility.value = pin; 
  newVisibilitySelection.value = pin.visibility || 0; // Ustaw domyślną wartość w selectie
};

const executeDeletePin = async () => {
  if (!pinToDelete.value) return;
  try {
    const token = sessionStorage.getItem('token');
    const res = await fetch(`${apiUrl}/api/Pins/${pinToDelete.value.id}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) {
      // Aktualizuj listę i statystyki po udanym usunięciu
      fetchData();
      pinToDelete.value = null;
    }
  } catch (e) { console.error("Błąd podczas usuwania", e); }
};

const executeUnlikePin = async () => {
  if (!pinToUnlike.value) return;
  try {
    const token = sessionStorage.getItem('token');
    
    // ZMIANA TUTAJ: Zaktualizowany URL z "/toggle-like"
    const res = await fetch(`${apiUrl}/api/Pins/${pinToUnlike.value.id}/toggle-like`, { 
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` }
    });
    
    if (res.ok) {
      // Skoro cofnęliśmy lajka, odświeżamy listę pinów
      fetchPins(); 
      pinToUnlike.value = null;
    }
  } catch (e) { console.error("Błąd podczas cofania polubienia", e); }
};

const executeChangeVisibility = async () => {
  if (!pinToChangeVisibility.value) return;
  try {
    const token = sessionStorage.getItem('token');
    const res = await fetch(`${apiUrl}/api/Pins/${pinToChangeVisibility.value.id}`, {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}` 
      },
      // Zgodnie z backendowym UpdatePinDto aktualizujemy pole Visibility
      body: JSON.stringify({ visibility: newVisibilitySelection.value })
    });
    if (res.ok) {
      fetchPins(); // Odśwież żeby zastosować zmianę
      pinToChangeVisibility.value = null;
    }
  } catch (e) { console.error("Błąd podczas zmiany widoczności", e); }
};

onMounted(fetchData);
</script>

<style scoped>
.profile-page { background: #f0f2f5; min-height: 100vh; font-family: 'Inter', sans-serif; color: #1a1d21; }
.profile-top-nav { height: 80px; background: white; display: flex; justify-content: center; align-items: center; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
.logo-text { font-size: 28px; font-weight: 900; cursor: pointer; }
.logo-text span { color: #2ecc71; }
.profile-container { max-width: 900px; margin: 40px auto; padding: 0 20px; }
.loading-screen { text-align: center; padding: 50px; font-size: 18px; color: #64748b; }

/* STYLOWANIE ZAKŁADEK */
.profile-tabs { display: flex; gap: 15px; margin-bottom: 20px; }
.profile-tabs button { padding: 12px 24px; font-size: 15px; font-weight: 700; border: none; background: #e2e8f0; color: #475569; border-radius: 12px; cursor: pointer; transition: 0.2s; }
.profile-tabs button:hover { background: #cbd5e1; }
.profile-tabs button.active { background: #2ecc71; color: white; }

/* PANEL FILTROWANIA (WZOROWANY NA STRONIE GŁÓWNEJ) */
.filter-panel { background: white; padding: 15px 20px; margin-bottom: 20px; border-radius: 15px; display: flex; align-items: center; gap: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.02); }
.filter-group { display: flex; align-items: center; gap: 10px; }
.filter-group label { font-weight: bold; font-size: 0.9rem; color: #444; }
.filter-group select { padding: 8px 12px; border-radius: 8px; border: 1px solid #ddd; background-color: #fff; outline: none; font-size: 14px; }
.results-count { margin-left: auto; color: #888; font-size: 0.9rem; }

.modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.6); backdrop-filter: blur(4px); display: flex; justify-content: center; align-items: center; z-index: 9999; }
.modal-card { background: white; padding: 30px; border-radius: 20px; width: 90%; max-width: 450px; position: relative; }
.drop-zone { border: 2px dashed #e2e8f0; border-radius: 15px; padding: 40px; text-align: center; cursor: pointer; background: #f8fafc; transition: 0.2s; }
.drop-zone--active { border-color: #2ecc71; background: #f0fff4; }
.modal-footer { display: flex; gap: 12px; justify-content: flex-end; margin-top: 25px; }
.btn-modal-cancel { background: #f1f5f9; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm { background: #ef4444; color: white; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm.save { background: #2ecc71; }
</style>