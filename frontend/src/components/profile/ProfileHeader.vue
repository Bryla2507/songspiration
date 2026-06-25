<template>
  <header class="user-dashboard">
    <div class="avatar-side">
      <div class="large-avatar-circle" @click="$emit('change-avatar')">
        <img v-if="user.avatarUrl || user.AvatarUrl" :src="`${apiUrl}${user.avatarUrl || user.AvatarUrl}`" alt="Avatar" />
        <span v-else>{{ (user.displayName || user.DisplayName)?.charAt(0).toUpperCase() }}</span>
        <div class="edit-overlay">Zmień zdjęcie</div>
      </div>
    </div>

    <div class="info-side">
      <h2 class="user-name">{{ user.displayName || user.DisplayName }}</h2>
      <p class="user-bio">{{ user.bio || user.Bio || 'Muzyka to moja passion...' }}</p>
      
       <div class="dashboard-btns">
         <button @click="$emit('edit')" class="btn-dashboard primary">Zmień dane</button>
         <button @click="$emit('report')" class="btn-dashboard warning">Reportuj</button>
         <button @click="$emit('delete')" class="btn-dashboard danger">Usuń konto</button>
       </div>
    </div>
  </header>
</template>

<script setup>
defineProps(['user']);
defineEmits(['edit', 'change-avatar', 'report', 'delete']);
const apiUrl = import.meta.env.VITE_API_URL;
</script>

<style scoped>
.user-dashboard { background: white; border-radius: 20px; padding: 40px; display: flex; gap: 40px; align-items: center; margin-bottom: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.05); }
.large-avatar-circle { width: 160px; height: 160px; border-radius: 50%; background: #eef1f4; overflow: hidden; cursor: pointer; position: relative; display: flex; align-items: center; justify-content: center; font-size: 50px; color: #2ecc71; border: 4px solid white; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
.large-avatar-circle img { width: 100%; height: 100%; object-fit: cover; }
.edit-overlay { position: absolute; inset: 0; background: rgba(0,0,0,0.4); color: white; display: flex; align-items: center; justify-content: center; opacity: 0; transition: 0.3s; font-size: 14px; font-weight: 600; }
.large-avatar-circle:hover .edit-overlay { opacity: 1; }
.btn-dashboard { padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; border: none; font-size: 14px; transition: 0.2s; }
.btn-dashboard.primary { background: #2ecc71; color: white; margin-right: 15px; }
.btn-dashboard.warning { background: #fef3c7; color: #d97706; margin-right: 15px; }
.btn-dashboard.danger { background: #fef2f2; color: #ef4444; }
.user-name { font-size: 28px; margin: 0 0 10px 0; }
.user-bio { color: #64748b; margin-bottom: 20px; line-height: 1.6; }
</style>