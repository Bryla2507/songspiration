<template>
  <div class="admin-panel">
    <h1>Admin Panel</h1>
     <nav class="admin-nav">
       <router-link to="/admin/genres">Manage Genres</router-link>
       <router-link to="/admin/users">Manage Users</router-link>
       <router-link to="/admin/reports">Manage Reports</router-link>
     </nav>
    <router-view></router-view>
  </div>
</template>

<script lang="ts">
export default {
  name: 'AdminPanelView',
};
</script>

<style scoped>
.admin-panel {
  padding: 20px;
}

.admin-nav {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.admin-nav a {
  padding: 10px 15px;
  background-color: #007bff;
  color: white;
  text-decoration: none;
  border-radius: 5px;
}

.admin-nav a:hover {
  background-color: #0056b3;
}
</style>