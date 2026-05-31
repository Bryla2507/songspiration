<template>
  <div class="genres-view">
    <h2>Manage Genres</h2>
    <div class="search-add">
      <input type="text" v-model="searchTerm" placeholder="Search genres..." />
      <button @click="addGenre">Add New Genre</button>
    </div>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Name</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="genre in filteredGenres" :key="genre.id">
          <td>{{ genre.id }}</td>
          <td>{{ genre.name }}</td>
          <td>
            <button @click="editGenre(genre)">Edit</button>
            <button @click="deleteGenre(genre.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="showModal" class="modal">
      <div class="modal-content">
        <h3>{{ isEditMode ? 'Edit Genre' : 'Add New Genre' }}</h3>
        <form @submit.prevent="saveGenre">
          <div class="form-group">
            <label for="genreName">Genre Name</label>
            <input type="text" id="genreName" v-model="currentGenre.name" required />
          </div>
          <div class="modal-actions">
            <button type="submit">Save</button>
            <button type="button" @click="cancel">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';

interface Genre {
  id: string; // Guid jako string
  name: string;
  slug?: string;
}

export default {
  name: 'GenresView',
  setup() {
    const genres = ref<Genre[]>([]);
    const searchTerm = ref('');
    const showModal = ref(false);
    const isEditMode = ref(false);
    const currentGenre = ref<Genre>({ id: '', name: '' });

    const getAxiosConfig = () => {
      const token = sessionStorage.getItem('token');
      return {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      };
    };

    const fetchGenres = async () => {
      try {
        const response = await axios.get('/api/Genre', getAxiosConfig());
        genres.value = response.data.map((genre: any) => ({
          id: genre.id,
          name: genre.name,
          slug: genre.slug
        }));
      } catch (error) {
        console.error('Error fetching genres:', error);
      }
    };

    const addGenre = () => {
      currentGenre.value = { id: '', name: '' };
      isEditMode.value = false;
      showModal.value = true;
    };

    const editGenre = (genre: Genre) => {
      currentGenre.value = { ...genre };
      isEditMode.value = true;
      showModal.value = true;
    };

    const saveGenre = async () => {
      try {
        if (isEditMode.value) {
          // Edit existing genre
          const response = await axios.post('/api/Genre', {
            id: currentGenre.value.id,
            name: currentGenre.value.name
          }, getAxiosConfig());
          const index = genres.value.findIndex(genre => genre.id === currentGenre.value.id);
          if (index !== -1) {
            genres.value[index] = response.data;
          }
        } else {
          // Add new genre
          const response = await axios.post('/api/Genre', {
            name: currentGenre.value.name
          }, getAxiosConfig());
          genres.value.push(response.data);
        }
        showModal.value = false;
      } catch (error) {
        console.error('Error saving genre:', error);
      }
    };

    const deleteGenre = async (id: string) => {
      try {
        await axios.delete(`/api/Genre/${id}`, getAxiosConfig());
        genres.value = genres.value.filter(genre => genre.id !== id);
      } catch (error) {
        console.error('Error deleting genre:', error);
      }
    };

    const cancel = () => {
      showModal.value = false;
    };

    const filteredGenres = computed(() => {
      return genres.value.filter(genre =>
        genre.name.toLowerCase().includes(searchTerm.value.toLowerCase())
      );
    });

    onMounted(fetchGenres);

    return {
      genres,
      searchTerm,
      showModal,
      isEditMode,
      currentGenre,
      filteredGenres,
      addGenre,
      editGenre,
      saveGenre,
      deleteGenre,
      cancel,
    };
  },
};
</script>

<style scoped>
.genres-view {
  padding: 20px;
}

.search-add {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.search-add input {
  padding: 8px;
  width: 300px;
}

.search-add button {
  padding: 8px 15px;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
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
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 5px;
  width: 400px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
}

.form-group input {
  width: 100%;
  padding: 8px;
  box-sizing: border-box;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}
</style>