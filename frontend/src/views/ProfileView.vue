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
        @edit="showEditModal = true"
        @change-avatar="handleAvatarClick"
        @report="showReportModal = true"
        @delete="showDeleteModal = true"
      />

      <PinGrid 
        :pins="pins" 
        :stats="{ 
          pins: user.addedPinsCount || user.AddedPinsCount || 0, 
          likes: user.totalLikesReceived || user.TotalLikesReceived || 0 
        }"
        @open-pin="openApiModal" 
      />
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
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
// Sprawdź czy plik na dysku nazywa się ProfileHeader.vue
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

const fetchData = async () => {
  const userId = route.params.id;
  const token = sessionStorage.getItem('token');
  try {
    const userRes = await fetch(`${apiUrl}/api/Users/profile/${userId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (userRes.ok) user.value = await userRes.json();

    const pinsRes = await fetch(`${apiUrl}/api/Pins/user/${userId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (pinsRes.ok) pins.value = await pinsRes.json();
  } catch (error) { console.error(error); }
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

// Funkcja otwierająca modal po kliknięciu w awatar
const handleAvatarClick = () => {
  showAvatarModal.value = true;
};

const triggerFileInput = () => fileInput.value?.click();

const handleFileSelect = (e) => { 
  if (e.target.files.length > 0) selectedFile.value = e.target.files[0];
};

const handleFileDrop = (e) => {
  isDragging.value = false;
  if (e.dataTransfer.files.length > 0) selectedFile.value = e.dataTransfer.files[0];
};

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
    if (res.ok) {
      closeAvatarModal();
      fetchData();
    }
  } catch (e) { console.error(e); }
};

const closeAvatarModal = () => { 
  showAvatarModal.value = false; 
  selectedFile.value = null; 
};

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

onMounted(fetchData);
</script>

<style scoped>
.profile-page { background: #f0f2f5; min-height: 100vh; font-family: 'Inter', sans-serif; color: #1a1d21; }
.profile-top-nav { height: 80px; background: white; display: flex; justify-content: center; align-items: center; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
.logo-text { font-size: 28px; font-weight: 900; cursor: pointer; }
.logo-text span { color: #2ecc71; }
.profile-container { max-width: 900px; margin: 40px auto; padding: 0 20px; }
.loading-screen { text-align: center; padding: 50px; font-size: 18px; color: #64748b; }

.modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.6); backdrop-filter: blur(4px); display: flex; justify-content: center; align-items: center; z-index: 9999; }
.modal-card { background: white; padding: 30px; border-radius: 20px; width: 90%; max-width: 450px; position: relative; }
.drop-zone { border: 2px dashed #e2e8f0; border-radius: 15px; padding: 40px; text-align: center; cursor: pointer; background: #f8fafc; transition: 0.2s; }
.drop-zone--active { border-color: #2ecc71; background: #f0fff4; }
.modal-footer { display: flex; gap: 12px; justify-content: flex-end; margin-top: 25px; }
.btn-modal-cancel { background: #f1f5f9; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm { background: #ef4444; color: white; border: none; padding: 12px 20px; border-radius: 10px; cursor: pointer; font-weight: 600; }
.btn-modal-confirm.save { background: #2ecc71; }
</style>