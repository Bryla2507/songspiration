<template>
  <nav class="top-bar">
    <div class="top-bar-content">
      <div class="brand" @click="$router.push('/')">
        <span class="logo-icon">🎸</span>
        <h1 class="logo-text">Song<span>Spiration</span></h1>
      </div>


      <div class="nav-actions">
        <RouterLink to="/ranking" class="btn-ranking">
          🏆 Ranking
        </RouterLink>
        <RouterLink to="/add-pin" class="btn-create">
          + Dodaj Pin
        </RouterLink>
        <div class="user-profile" @click="toggleDropdown" style="position: relative;">
          <div class="avatar">
            <img v-if="avatarUrl" :src="`${apiUrl}${avatarUrl}`" alt="Avatar" class="avatar-img" />
            <span v-else>{{ userInitials }}</span>
          </div>
          
          <div v-if="isDropdownOpen" class="dropdown-menu">
            <button @click="goToProfile" class="dropdown-item profile-link">Mój Profil</button>
            <div class="dropdown-divider"></div>
            <button @click.stop="logout" class="dropdown-item logout-btn">Wyloguj</button>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL;
const searchQuery = ref('');
const isDropdownOpen = ref(false);
const userInitials = ref('?');
const avatarUrl = ref(null);
const emit = defineEmits(['search']);

const handleSearch = () => {
  emit('search', searchQuery.value);
};

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value;
};

const logout = () => {
  sessionStorage.clear();
  router.push('/login');
};

const goToProfile = () => {
  const userId = sessionStorage.getItem('userId');
  if (userId) {
    router.push({ name: 'profile', params: { id: userId } });
    isDropdownOpen.value = false;
  }
};

const fetchUserData = async () => {
  const userId = sessionStorage.getItem('userId');
  const token = sessionStorage.getItem('token');
  const userName = sessionStorage.getItem('userName');

  if (userName) {
    userInitials.value = userName.substring(0, 2).toUpperCase();
  }

  if (userId && token) {
    try {
      const res = await fetch(`${apiUrl}/api/Users/profile/${userId}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        const data = await res.json();
        avatarUrl.value = data.avatarUrl || data.AvatarUrl;
      }
    } catch (error) {
      console.error(error);
    }
  }
};

onMounted(fetchUserData);
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
  color: #2ecc71;
}

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

.nav-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.btn-ranking {
  background: #f0f0f0;
  color: #2ecc71;
  border: 2px solid #2ecc71;
  padding: 10px 20px;
  border-radius: 10px;
  font-weight: 700;
  font-size: 14px;
  cursor: pointer;
  text-decoration: none;
  transition: all 0.2s;
}

.btn-ranking:hover {
  background: #2ecc71;
  color: white;
  transform: translateY(-1px);
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
  text-decoration: none;
  transition: transform 0.2s;
}

.btn-create:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
}

.avatar {
  width: 38px;
  height: 38px;
  background: #eef1f4;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  color: #2ecc71;
  border: 2px solid #fff;
  box-shadow: 0 0 0 1px #e5e7eb;
  cursor: pointer;
  overflow: hidden;
}

.avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.dropdown-menu {
  position: absolute;
  top: 45px;
  right: 0;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  min-width: 140px;
  z-index: 1000;
  padding: 8px 0;
}

.dropdown-item {
  width: 100%;
  text-align: left;
  padding: 10px 15px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
  color: #4b5563;
  transition: background 0.2s;
}

.dropdown-item:hover {
  background: #f3f4f6;
}

.dropdown-divider {
  height: 1px;
  background: #f3f4f6;
  margin: 4px 0;
}

.logout-btn {
  color: #e74c3c;
  font-weight: 600;
}

@media (max-width: 768px) {
  .search-container {
    display: none;
  }
}
</style>