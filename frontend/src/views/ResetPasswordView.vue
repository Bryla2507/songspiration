<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Nowe hasło</h1>
      <p v-if="successMessage" class="success-msg">
        {{ successMessage }}
        <br/><br/>
        <RouterLink to="/login" class="login-link">Przejdź do logowania</RouterLink>
      </p>
      <form v-else @submit.prevent="handleResetPassword" class="login-form">
        <p class="description">Wprowadź swoje nowe hasło.</p>
        <div class="form-group">
          <label for="password">Nowe hasło</label>
          <input v-model="password" type="password" id="password" required placeholder="Wpisz nowe hasło..." />
        </div>
        <div class="form-group">
          <label for="confirmPassword">Potwierdź nowe hasło</label>
          <input v-model="confirmPassword" type="password" id="confirmPassword" required placeholder="Potwierdź nowe hasło..." />
        </div>
        <div v-if="errorMessage" class="error-msg">{{ errorMessage }}</div>
        <button type="submit" :disabled="isLoading" class="btn-submit">
          {{ isLoading ? 'Zapisywanie...' : 'Zapisz hasło' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL;

const password = ref('');
const confirmPassword = ref('');
const errorMessage = ref('');
const successMessage = ref('');
const isLoading = ref(false);
const token = ref('');

onMounted(() => {
  token.value = route.query.token;
  if (!token.value) {
    errorMessage.value = 'Brakujący lub nieprawidłowy token resetowania hasła.';
  }
});

const handleResetPassword = async () => {
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Hasła nie są identyczne.';
    return;
  }

  if (password.value.length < 6) {
    errorMessage.value = 'Hasło musi mieć co najmniej 6 znaków.';
    return;
  }

  if (!token.value) {
    errorMessage.value = 'Brakujący token resetowania hasła.';
    return;
  }

  isLoading.value = true;
  errorMessage.value = '';

  try {
    const response = await fetch(`${apiUrl}/api/Users/reset-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token: token.value, newPassword: password.value })
    });

    if (response.ok) {
      successMessage.value = 'Twoje hasło zostało pomyślnie zmienione.';
    } else {
      const data = await response.json();
      errorMessage.value = data.message || 'Nie udało się zresetować hasła. Możliwe, że link wygasł.';
    }
  } catch (error) {
    console.error('Błąd:', error);
    errorMessage.value = 'Błąd połączenia z serwerem.';
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.login-container { display: flex; justify-content: center; align-items: center; height: 100vh; background-color: #f4f7f9; }
.login-card { background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); width: 100%; max-width: 400px; text-align: center; }
h1 { margin-top: 0; margin-bottom: 10px; color: #24292e; }
.description { color: #666; margin-bottom: 25px; font-size: 14px; }
.login-form { display: flex; flex-direction: column; gap: 20px; text-align: left; }
.form-group { display: flex; flex-direction: column; gap: 8px; }
label { font-weight: 600; color: #444; font-size: 14px; }
input { padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 16px; }
.btn-submit { background: #2ecc71; color: white; border: none; padding: 12px; border-radius: 6px; font-weight: bold; cursor: pointer; font-size: 16px; margin-top: 10px; }
.btn-submit:disabled { background: #95a5a6; cursor: not-allowed; }
.error-msg { color: #e74c3c; font-size: 14px; text-align: center; }
.success-msg { color: #2ecc71; font-size: 14px; text-align: center; padding: 20px; background: #e8f8f5; border-radius: 6px; }
.login-link { color: #3498db; text-decoration: none; font-weight: bold; }
.login-link:hover { text-decoration: underline; }
</style>