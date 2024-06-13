<template>
  <h1>登入頁</h1>
  <div>
    <div class="form-item">
      <label for="fname">帳號</label><br>
      <input type="text" v-model="username"><br>
    </div>
    <div class="form-item">
      <label for="lname">密碼</label><br>
      <input type="password" v-model="password"><br><br>
    </div>
    <input type="submit" value="登入" @click="login">
  </div>
  <span v-if="errorMsg !== ''" style="color: red;">{{ errorMsg }}</span>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import {useRoute, useRouter} from 'vue-router'
import axios from "axios";
const router = useRouter();
const username = ref('')
const password = ref('')
const errorMsg = ref('') 

onMounted(async () => {
});

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

const isValidInput = (input) => {
    return !input.includes("'") && !input.includes(";");
}

const login = async () => {
    if (username.value === '' || password.value === '') {
        errorMsg.value = '請輸入帳號、密碼'
        return;
    }

    if (!isValidInput(username.value) || !isValidInput(password.value)) {
        errorMsg.value = '帳號或密碼包含非法字符'
        return;
    }

    try {
      const response = await axios.post('/api/user/login', {
          username: username.value,
          password: password.value,
      });
      console.log('response----',response);
      if (response.data.code === 0) {
        errorMsg.value = ''
          sessionStorage.setItem('token',getCookieValue('uuid') );
          sessionStorage.setItem('role',getCookieValue('permission'))
          if (sessionStorage.getItem('role') === '1') {
            router.push('/account')
          } else {
            router.push('/member')
          }
      } else {
        alert(response.data.message)
        errorMsg.value = '請輸入正確帳號、密碼'
      }
    } catch (error) {
        console.error('Login failed:', error);
        errorMsg.value = '請輸入正確帳號、密碼'
    }
};

</script>

<style scoped>
.read-the-docs {
  color: #888;
}
.form-item {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 30px;
  margin-bottom: 5px;
}
label {
  margin-right: 5px;
}
</style>