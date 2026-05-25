<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Resetowanie hasła</h1>
      <p v-if="successMessage" class="success-msg">{{ successMessage }}</p>
      <form v-else @submit.prevent="handleForgotPassword" class="login-form">
        <p class="description">Podaj swój adres email, aby otrzymać link do resetu hasła.</p>
        <div class="form-group">
          <label for="email">Email</label>
          <input v-model="email" type="email" id="email" required placeholder="Wpisz email..." />
        </div>
        <div v-if="errorMessage" class="error-msg">{{ errorMessage }}</div>
        <button type="submit" :disabled="isLoading" class="btn-submit">
          {{ isLoading ? 'Wysyłanie...' : 'Wyślij link' }}
        </button>
      </form>
      <div class="register-link">
        <RouterLink to="/login">Wróć do logowania</RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const apiUrl = import.meta.env.VITE_API_URL;

const email = ref('');
const errorMessage = ref('');
const successMessage = ref('');
const isLoading = ref(false);

const handleForgotPassword = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    const response = await fetch(`${apiUrl}/api/Users/forgot-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value })
    });

    if (response.ok) {
      successMessage.value = 'Jeśli konto istnieje, link do resetu hasła został wysłany na podany adres e-mail.';
    } else {
      let data = {};
      try {
        data = await response.json();
      } catch (e) {}
      errorMessage.value = data.message || 'Wystąpił błąd podczas wysyłania żądania.';
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
.btn-submit { background: #3498db; color: white; border: none; padding: 12px; border-radius: 6px; font-weight: bold; cursor: pointer; font-size: 16px; margin-top: 10px; }
.btn-submit:disabled { background: #95a5a6; cursor: not-allowed; }
.error-msg { color: #e74c3c; font-size: 14px; text-align: center; }
.success-msg { color: #2ecc71; font-size: 14px; text-align: center; padding: 20px; background: #e8f8f5; border-radius: 6px; }
.register-link { margin-top: 20px; font-size: 14px; color: #666; }
.register-link a { color: #3498db; text-decoration: none; font-weight: bold; }
.register-link a:hover { text-decoration: underline; }
</style>