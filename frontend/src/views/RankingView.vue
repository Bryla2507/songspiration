<template>
  <div class="ranking-view">
    <div class="ranking-header">
      <h1>User Rankings</h1>
      <p>Discover the most popular creators on SongSpiration!</p>
    </div>

    <div class="tabs">
      <button 
        :class="{ active: activeTab === 'editors-choice' }" 
        @click="activeTab = 'editors-choice'"
      >
        Editor's Choice
      </button>
      <button 
        :class="{ active: activeTab === 'likes' }" 
        @click="activeTab = 'likes'"
      >
        Most Liked
      </button>
      <button 
        :class="{ active: activeTab === 'downloads' }" 
        @click="activeTab = 'downloads'"
      >
        Most Downloaded
      </button>
    </div>

    <div class="ranking-content">
      <div v-if="loading" class="loading">
        Loading...
      </div>
      <div v-else-if="users.length === 0" class="empty">
        No users found for this category.
      </div>
      <div v-else class="user-list">
        <div v-for="(user, index) in users" :key="user.id" class="user-card">
          <div class="rank-number">#{{ index + 1 }}</div>
          <div class="avatar-container">
            <img :src="user.avatarUrl ? `${apiUrl}${user.avatarUrl}` : 'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.vecteezy.com%2Fsystem%2Fresources%2Fpreviews%2F058%2F338%2F462%2Fnon_2x%2Fgeneric-profile-picture-placeholder-default-user-profile-image-vector.jpg&f=1&nofb=1&ipt=ab550c542ef4d3714ca39f7c06f1fef678d72f32de077c11f9de5ae47878a9e8'" alt="User Avatar" class="avatar" />
          </div>
          <div class="user-info">
            <h3 class="display-name">
              <router-link :to="`/profile/${user.id}`">{{ user.displayName }}</router-link>
            </h3>
            <div class="stats">
              <span v-if="activeTab === 'editors-choice'" class="badge editor">Editor's Choice</span>
              <span v-if="activeTab === 'likes'">❤️ {{ user.totalLikes }} likes</span>
              <span v-if="activeTab === 'downloads'">⬇️ {{ user.totalDownloads }} downloads</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { ref, watch, onMounted } from 'vue';
import axios from 'axios';

interface UserRankingDto {
  id: string;
  displayName: string;
  avatarUrl: string | null;
  isEditorChoice: boolean;
  totalLikes: number;
  totalDownloads: number;
}

export default {
  name: 'RankingView',
  setup() {
    const activeTab = ref('editors-choice');
    const users = ref<UserRankingDto[]>([]);
    const loading = ref(false);
    const apiUrl = import.meta.env.VITE_API_URL;

    const fetchRanking = async (category: string) => {
      loading.value = true;
      try {
        const response = await axios.get<UserRankingDto[]>(`/api/ranking/${category}`, {
          params: { limit: 10 }
        });
        users.value = response.data;
      } catch (error) {
        console.error(`Error fetching ranking for ${category}:`, error);
        users.value = [];
      } finally {
        loading.value = false;
      }
    };

    onMounted(() => {
      fetchRanking(activeTab.value);
    });

    watch(activeTab, (newTab) => {
      fetchRanking(newTab);
    });

    return {
      activeTab,
      users,
      loading,
      apiUrl
    };
  }
};
</script>

<style scoped>
.ranking-view {
  max-width: 800px;
  margin: 0 auto;
  padding: 40px 20px;
}

.ranking-header {
  text-align: center;
  margin-bottom: 30px;
}

.ranking-header h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
  color: #2ecc71;
}

.ranking-header p {
  color: #666;
  font-size: 1.1rem;
}

.tabs {
  display: flex;
  justify-content: center;
  gap: 15px;
  margin-bottom: 30px;
}

.tabs button {
  padding: 10px 20px;
  font-size: 1rem;
  border: none;
  background-color: #f0f0f0;
  color: #555;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.tabs button:hover {
  background-color: #e0e0e0;
}

.tabs button.active {
  background-color: #2ecc71;
  color: white;
  font-weight: bold;
}

.ranking-content {
  background: white;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  padding: 20px;
}

.loading, .empty {
  text-align: center;
  padding: 40px;
  color: #888;
  font-size: 1.2rem;
}

.user-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.user-card {
  display: flex;
  align-items: center;
  padding: 15px;
  border: 1px solid #eee;
  border-radius: 8px;
  transition: transform 0.2s ease;
}

.user-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.rank-number {
  font-size: 1.5rem;
  font-weight: bold;
  color: #2ecc71;
  min-width: 50px;
  text-align: center;
}

.avatar-container {
  margin: 0 20px;
}

.avatar {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #ddd;
}

.user-info {
  flex-grow: 1;
}

.display-name {
  margin: 0 0 5px 0;
  font-size: 1.2rem;
}

.display-name a {
  text-decoration: none;
  color: #333;
}

.display-name a:hover {
  color: #2ecc71;
}

.stats {
  display: flex;
  gap: 15px;
  font-size: 0.9rem;
  color: #666;
  align-items: center;
}

.badge {
  padding: 3px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: bold;
}

.badge.editor {
  background-color: #f0ad4e;
  color: #000;
}
</style>
