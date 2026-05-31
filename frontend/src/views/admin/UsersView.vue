<template>
  <div class="users-view">
    <h2>Manage Users</h2>
    <div class="search-box">
      <input type="text" v-model="searchTerm" placeholder="Search users..." @input="onSearchInput" />
    </div>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <th>Email</th>
          <th>Role</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in filteredUsers" :key="user.id">
          <td>{{ user.id }}</td>
          <td>{{ user.displayName }}</td>
          <td>{{ user.email }}</td>
          <td>{{ user.roles }}</td>
          <td>
            <button @click="deleteUser(user.id)">Delete</button>
            <button @click="banUser(user.id)" v-if="!user.isBanned">Ban</button>
            <button @click="unbanUser(user.id)" v-if="user.isBanned">Unban</button>
            <button @click="promoteToAdmin(user.id)" v-if="user.roles !== 'Admin'">Promote to Admin</button>
            <button @click="demoteFromAdmin(user.id)" v-if="user.roles === 'Admin'">Demote</button>
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
import { ref, onMounted, computed, watch } from 'vue';
import axios from 'axios';

interface User {
  id: string;
  displayName: string;
  email: string;
  isBanned: boolean;
  isEditorChoice: boolean;
  roles: string;
  avatarUrl?: string | null;
  bio?: string | null;
  createdAt?: string;
  lastLogin?: string;
  isEmailVerified?: boolean;
}

export default {
  name: 'UsersView',
  setup() {
    const users = ref<User[]>([]);
    const searchTerm = ref('');
    let searchTimeout: number | null = null;

    const apiUrl = import.meta.env.VITE_API_URL;

    const fetchUsers = async () => {
      try {
        const token = sessionStorage.getItem('token');
        console.log("Fetching users with token:", token ? "Token exists" : "No token");

        const response = await axios.get(`${apiUrl}/api/users`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        console.log("Users fetched successfully:", response.data);
        users.value = response.data.map((user: any) => ({
          id: user.id,
          displayName: user.displayName || user.username || user.email.split('@')[0],
          email: user.email,
          isBanned: user.roles === 'Banned',
          isEditorChoice: user.isEditorChoice,
          roles: user.roles,
          avatarUrl: user.avatarUrl,
          bio: user.bio,
          createdAt: user.createdAt,
          lastLogin: user.lastLogin,
          isEmailVerified: user.isEmailVerified
        }));
      } catch (error: unknown) {
        if (axios.isAxiosError(error)) {
          console.error('Error fetching users:', error.response?.data || error.message);
        } else {
          console.error('Unexpected error:', error);
        }
      }
    };

    const onSearchInput = () => {
      if (searchTimeout) {
        clearTimeout(searchTimeout);
      }
      searchTimeout = setTimeout(fetchUsers, 500) as unknown as number;
    };

    const getAxiosConfig = () => {
      const token = sessionStorage.getItem('token');
      return {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      };
    };

    const deleteUser = async (id: string) => {
      try {
        await axios.delete(`${apiUrl}/api/AdminPanel/users/${id}`, getAxiosConfig());
        users.value = users.value.filter(user => user.id !== id);
      } catch (error) {
        console.error('Error deleting user:', error);
      }
    };

    const banUser = async (id: string) => {
      try {
        await axios.put(`${apiUrl}/api/AdminPanel/users/${id}/ban`, {}, getAxiosConfig());
        users.value = users.value.map(user => {
          if (user.id === id) {
            return { ...user, isBanned: true, roles: 'Banned' };
          }
          return user;
        });
      } catch (error) {
        console.error('Error banning user:', error);
      }
    };

    const deleteUserPins = async (id: string) => {
      try {
        await axios.delete(`${apiUrl}/api/AdminPanel/users/${id}/pins`, getAxiosConfig());
        alert(`Pins for user with ID ${id} have been deleted`);
      } catch (error) {
        console.error('Error deleting user pins:', error);
      }
    };

    const toggleEditorChoice = async (user: User) => {
      try {
        const newStatus = !user.isEditorChoice;
        const config = {
          ...getAxiosConfig(),
          headers: {
            ...getAxiosConfig().headers,
            'Content-Type': 'application/json'
          }
        };
        await axios.put(`${apiUrl}/api/AdminPanel/users/${user.id}/editor-choice`, newStatus, config);
        users.value = users.value.map(u => {
          if (u.id === user.id) {
            return { ...u, isEditorChoice: newStatus };
          }
          return u;
        });
      } catch (error) {
        console.error('Error toggling editor choice:', error);
      }
    };

    const unbanUser = async (id: string) => {
      try {
        await axios.put(`${apiUrl}/api/AdminPanel/users/${id}/unban`, {}, getAxiosConfig());
        users.value = users.value.map(user => {
          if (user.id === id) {
            return { ...user, isBanned: false, roles: 'User' };
          }
          return user;
        });
      } catch (error) {
        console.error('Error unbanning user:', error);
      }
    };

    const promoteToAdmin = async (id: string) => {
      try {
        await axios.put(`${apiUrl}/api/AdminPanel/users/${id}/promote`, {}, getAxiosConfig());
        users.value = users.value.map(user => {
          if (user.id === id) {
            return { ...user, roles: 'Admin' };
          }
          return user;
        });
      } catch (error) {
        console.error('Error promoting user to admin:', error);
      }
    };

    const demoteFromAdmin = async (id: string) => {
      try {
        await axios.put(`${apiUrl}/api/AdminPanel/users/${id}/demote`, {}, getAxiosConfig());
        users.value = users.value.map(user => {
          if (user.id === id) {
            return { ...user, roles: 'User' };
          }
          return user;
        });
      } catch (error) {
        console.error('Error demoting user from admin:', error);
      }
    };

    const filteredUsers = computed(() => {
      if (!searchTerm.value) {
        return users.value;
      }
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
      unbanUser,
      promoteToAdmin,
      demoteFromAdmin,
      deleteUserPins,
      toggleEditorChoice,
      onSearchInput
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