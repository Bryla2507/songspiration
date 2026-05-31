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
    const users = ref<User[]>([
      {
        id: 'a1b2c3d4-e5f6-7890-g1h2-i3j4k5l6m7n8',
        displayName: 'adfag',
        email: 'adfag@gmail.com',
        isBanned: false,
        isEditorChoice: false,
        roles: 'User',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-01T00:00:00',
        lastLogin: '2023-01-02T00:00:00',
        isEmailVerified: true
      },
      {
        id: 'b2c3d4e5-f6g7-8901-h2i3-j4k5l6m7n8o9',
        displayName: 'lkjlj',
        email: 'lkjlj@gmail.com',
        isBanned: false,
        isEditorChoice: false,
        roles: 'User',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-03T00:00:00',
        lastLogin: '2023-01-04T00:00:00',
        isEmailVerified: true
      },
      {
        id: 'c3d4e5f6-g7h8-9012-i3j4-k5l6m7n8o9p0',
        displayName: 'hwajgha',
        email: 'hwajgha@gmail.com',
        isBanned: false,
        isEditorChoice: false,
        roles: 'User',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-05T00:00:00',
        lastLogin: '2023-01-06T00:00:00',
        isEmailVerified: true
      },
      {
        id: 'd4e5f6g7-h8i9-0123-j4k5-l6m7n8o9p0q1',
        displayName: 'qwerty',
        email: 'qwerty@gmail.com',
        isBanned: false,
        isEditorChoice: false,
        roles: 'User',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-07T00:00:00',
        lastLogin: '2023-01-08T00:00:00',
        isEmailVerified: true
      },
      {
        id: 'e5f6g7h8-i9j0-1234-k5l6-m7n8o9p0q1r2',
        displayName: 'bryla',
        email: 'filipbry2507@gmail.com',
        isBanned: false,
        isEditorChoice: true,
        roles: 'Admin',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-09T00:00:00',
        lastLogin: '2023-01-10T00:00:00',
        isEmailVerified: true
      },
      {
        id: 'f6g7h8i9-j0k1-2345-l6m7-n8o9p0q1r2s3',
        displayName: 'test',
        email: 'test@test.com',
        isBanned: false,
        isEditorChoice: false,
        roles: 'User',
        avatarUrl: null,
        bio: null,
        createdAt: '2023-01-11T00:00:00',
        lastLogin: '2023-01-12T00:00:00',
        isEmailVerified: true
      }
    ]);
    const searchTerm = ref('');
    let searchTimeout: number | null = null;

    const getAxiosConfig = () => {
      const token = sessionStorage.getItem('token');
      return {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      };
    };

    const fetchUsers = async () => {
      // No need to fetch users from API, as they are hardcoded
    };

    const onSearchInput = () => {
      if (searchTimeout) {
        clearTimeout(searchTimeout);
      }
      searchTimeout = setTimeout(fetchUsers, 500) as unknown as number;
    };

    const deleteUser = (id: string) => {
      users.value = users.value.filter(user => user.id !== id);
    };

    const banUser = (id: string) => {
      users.value = users.value.map(user => {
        if (user.id === id) {
          return { ...user, isBanned: true };
        }
        return user;
      });
    };

    const deleteUserPins = (id: string) => {
      alert(`Pins for user with ID ${id} would be deleted (simulated)`);
    };

    const toggleEditorChoice = (user: User) => {
      users.value = users.value.map(u => {
        if (u.id === user.id) {
          return { ...u, isEditorChoice: !u.isEditorChoice };
        }
        return u;
      });
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