<template>
  <div class="login-container">
    <h1>後臺管理系統</h1>
    <div class="login-form">
      <div class="form-item">
        <div>帳號</div>
        <input type="text" id="username" v-model="username"><br>
      </div>
      <div class="form-item">
        <div>密碼</div>
        <input type="password" id="password" v-model="password"><br><br>
      </div>
      <input type="submit" value="登入" @click="login" class="login-button">
    </div>
    <span v-if="errorMsg !== ''" class="error-msg">{{ errorMsg }}</span>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from "axios"

const router = useRouter()
const username = ref('admin')
const password = ref('147147')
const errorMsg = ref('')
const pattern = /^[a-zA-Z0-9_-]{4,16}$/

const getCookieValue = (cookieName) => {
    var name = cookieName + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var cookieArray = decodedCookie.split(';');
    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i].trim();
        if (cookie.indexOf(name) == 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return "";
}

const login = async () => {
  if (username.value === '' || password.value === '') {
    errorMsg.value = '請輸入帳號、密碼'
    return
  } else {
    errorMsg.value = ''
  }

  if (!pattern.test(username.value) || !pattern.test(password.value)) {
    errorMsg.value = '帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符'
    return
  } else {
    errorMsg.value = ''
  }

  try {
    const response = await axios.post('/api/user/login', {
      username: username.value,
      password: password.value,
    })
    if (response.data.code === 0) {
      errorMsg.value = ''
      sessionStorage.setItem('token', getCookieValue('uuid'))
      sessionStorage.setItem('role', getCookieValue('permission'))
      if (sessionStorage.getItem('role') === '1') {
        router.push('/account')
      } else if (sessionStorage.getItem('role') === '2') {
        router.push('/member')
      } else {
        router.push('/product')
      }
    } else {
      errorMsg.value = '請輸入正確帳號、密碼'
    }
  } catch (error) {
    console.error('登入失敗:', error)
    errorMsg.value = '登入失敗，請輸入正確帳號、密碼'
  }
}

</script>

<style scoped>
.login-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100vw;
}

.login-form {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 380px;
  text-align: left;
  padding: 15px 30px;
}

.form-item {
  margin-bottom: 10px;
}

input {
  width: 95%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.login-button {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 4px;
  background-color: #007bff;
  color: #fff;
  font-size: 1rem;
  cursor: pointer;
}

.login-button:hover {
  background-color: #0056b3;
}

.error-msg {
  color: red;
  margin-top: 1rem;
}
</style>
