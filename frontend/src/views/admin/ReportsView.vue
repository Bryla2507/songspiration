<template>
  <div class="reports-view">
    <h2>Manage Reports</h2>
    <button @click="fetchReports" class="refresh-btn">Refresh</button>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Reported User</th>
          <th>Reporting User</th>
          <th>Content</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="report in reports" :key="report.id">
          <td>{{ report.id }}</td>
          <td>
            <div>{{ report.reportedUser.displayName }}</div>
            <div class="email">{{ report.reportedUser.email }}</div>
          </td>
          <td>
            <div>{{ report.reportingUser.displayName }}</div>
            <div class="email">{{ report.reportingUser.email }}</div>
          </td>
          <td>{{ report.content }}</td>
          <td>
            <button @click="deleteReport(report.id)" class="delete-btn">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

interface Report {
  id: number;
  reportedUserId: string;
  reportingUserId: string;
  content: string;
  reportedUser: {
    id: string;
    displayName: string;
    email: string;
  };
  reportingUser: {
    id: string;
    displayName: string;
    email: string;
  };
}

export default {
  setup() {
    const reports = ref<Report[]>([]);
    const apiUrl = import.meta.env.VITE_API_URL;

    const fetchReports = async () => {
      try {
        const token = sessionStorage.getItem('token');
        const response = await axios.get(`${apiUrl}/api/Report`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        reports.value = response.data;
      } catch (error) {
        console.error('Error fetching reports:', error);
      }
    };

    const deleteReport = async (reportId: number) => {
      try {
        const token = sessionStorage.getItem('token');
        await axios.delete(`${apiUrl}/api/Report/${reportId}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        fetchReports();
      } catch (error) {
        console.error('Error deleting report:', error);
      }
    };

    onMounted(fetchReports);

    return {
      reports,
      fetchReports,
      deleteReport
    };
  }
};
</script>

<style scoped>
.reports-view {
  padding: 20px;
  font-family: 'Inter', sans-serif;
}

h2 {
  margin-bottom: 20px;
  color: #1a1d21;
}

.refresh-btn {
  background: #2ecc71;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  margin-bottom: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

th, td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e2e8f0;
}

th {
  background: #f8fafc;
  font-weight: 600;
  color: #475569;
}

.email {
  font-size: 12px;
  color: #64748b;
}

.delete-btn {
  background: #ef4444;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
}
</style>