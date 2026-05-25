<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Logowanie</h1>
      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="email">Email</label>
          <input v-model="form.email" type="text" id="email" required placeholder="Wpisz email..." />
        </div>
        <div class="form-group">
          <label for="password">Hasło</label>
          <input v-model="form.password" type="password" id="password" required placeholder="Wpisz hasło..." />
        </div>
        <div class="forgot-password">
          <RouterLink to="/forgot-password">Zapomniałeś hasła?</RouterLink>
        </div>
        <div v-if="errorMessage" class="error-msg">{{ errorMessage }}</div>
        <button type="submit" :disabled="isLoading" class="btn-submit">
          {{ isLoading ? 'Logowanie...' : 'Zaloguj się' }}
        </button>
      </form>
      <div class="register-link">
        Nie masz konta? <RouterLink to="/register">Zarejestruj się</RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL;

const form = ref({ email: '', password: '' });
const errorMessage = ref('');
const isLoading = ref(false);

const handleLogin = async () => {
  isLoading.value = true;
  errorMessage.value = '';

  try {
    const response = await fetch(`${apiUrl}/api/Users/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    });

    if (response.ok) {
      const data = await response.json();
      // Zapisujemy token w sesji (zniknie po zamknięciu przeglądarki)
      sessionStorage.setItem('token', data.accessToken || data.token); // Dostosowane do AuthResponseDto z .NET
      
      // Zapisujemy nazwę użytkownika, by wyświetlić inicjały
      if (data.user && data.user.displayName) {
        sessionStorage.setItem('userName', data.user.displayName);
      } else if (data.user && data.user.email) {
        sessionStorage.setItem('userName', data.user.email);
      }

      if (data.user && data.user.id) {
        sessionStorage.setItem('userId', data.user.id);
      }

      // Przekierowanie na feed
      router.push('/');
    } else {
      errorMessage.value = 'Nieprawidłowy login lub hasło.';
    }
  } catch (error) {
    console.error('Błąd logowania:', error);
    errorMessage.value = 'Błąd połączenia z serwerem.';
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.login-container { display: flex; justify-content: center; align-items: center; height: 100vh; background-color: #f4f7f9; }
.login-card { background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); width: 100%; max-width: 400px; text-align: center; }
h1 { margin-top: 0; margin-bottom: 25px; color: #24292e; }
.login-form { display: flex; flex-direction: column; gap: 20px; text-align: left; }
.form-group { display: flex; flex-direction: column; gap: 8px; }
label { font-weight: 600; color: #444; font-size: 14px; }
input { padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 16px; }
.btn-submit { background: #2ecc71; color: white; border: none; padding: 12px; border-radius: 6px; font-weight: bold; cursor: pointer; font-size: 16px; margin-top: 10px; }
.btn-submit:disabled { background: #95a5a6; cursor: not-allowed; }
.error-msg { color: #e74c3c; font-size: 14px; text-align: center; }
.register-link { margin-top: 20px; font-size: 14px; color: #666; }
.register-link a { color: #2ecc71; text-decoration: none; font-weight: bold; }
.register-link a:hover { text-decoration: underline; }
</style>