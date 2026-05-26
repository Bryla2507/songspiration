<template>
  <div class="users-view">
    <h2>Manage Users</h2>
    <div class="search-box">
      <input type="text" v-model="searchTerm" placeholder="Search users..." />
    </div>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <th>Email</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
    <tr v-for="user in filteredUsers" :key="user.id">
      <td>{{ user.id }}</td>
      <td>{{ user.displayName }}</td>
      <td>{{ user.email }}</td>
      <td>
        <button @click="deleteUser(user.id)">Delete</button>
        <button @click="banUser(user.id)" v-if="!user.isBanned">Ban</button>
        <button @click="deleteUserPins(user.id)">Delete Pins</button>
        <button @click="toggleEditorChoice(user)" :class="{ active: user.isEditorChoice }">
          {{ user.isEditorChoice ? 'Remove Editor Choice' : 'Set Editor Choice' }}
        </button>
      </td>
    </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';

interface User {
  id: string;
  displayName: string;
  email: string;
  isBanned: boolean;
  isEditorChoice: boolean;
}

export default {
  name: 'UsersView',
  setup() {
    const users = ref<User[]>([]);
    const searchTerm = ref('');

    const fetchUsers = async () => {
      try {
        const response = await axios.get('/api/admin/users', {
          params: { criteria: searchTerm.value }
        });
        users.value = response.data;
      } catch (error) {
        console.error('Error fetching users:', error);
      }
    };

    const deleteUser = async (id: string) => {
      try {
        await axios.delete(`/api/admin/users/${id}`);
        await fetchUsers();
      } catch (error) {
        console.error('Error deleting user:', error);
      }
    };

    const banUser = async (id: string) => {
      try {
        await axios.put(`/api/admin/users/${id}/ban`); // Updated to PUT as per AdminPanelController
        await fetchUsers();
      } catch (error) {
        console.error('Error banning user:', error);
      }
    };

    const deleteUserPins = async (id: string) => {
      try {
        await axios.delete(`/api/admin/users/${id}/pins`);
        alert('User pins deleted successfully');
      } catch (error) {
        console.error('Error deleting user pins:', error);
      }
    };

    const toggleEditorChoice = async (user: User) => {
      try {
        const newValue = !user.isEditorChoice;
        await axios.put(`/api/admin/users/${user.id}/editor-choice`, newValue, {
          headers: { 'Content-Type': 'application/json' }
        });
        await fetchUsers();
      } catch (error) {
        console.error('Error toggling editor choice:', error);
      }
    };

    const filteredUsers = computed(() => {
      return users.value.filter(user =>
        user.displayName.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
        user.email.toLowerCase().includes(searchTerm.value.toLowerCase())
      );
    });

    onMounted(fetchUsers);

    return {
      users,
      searchTerm,
      filteredUsers,
      deleteUser,
      banUser,
      deleteUserPins,
      toggleEditorChoice,
    };
  },
};
</script>

<style scoped>
.users-view {
  padding: 20px;
}

.search-box {
  margin-bottom: 20px;
}

.search-box input {
  padding: 8px;
  width: 300px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 10px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #f2f2f2;
}

button {
  padding: 5px 10px;
  margin-right: 5px;
  background-color: #dc3545;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #c82333;
}

button.active {
  background-color: #ffc107;
  color: black;
}
</style>