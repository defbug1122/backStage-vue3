<template>
  <div class="account-container">
    <Menu></Menu>
    <div class="block">
      <div class="search-form">
        <div class="form-item">
          <input type="text" v-model="searchData">
        </div>
        <input class="button" type="submit" value="查詢" @click="searchAccount">
      </div>
      <div class="add-form">
        <div class="form-item">
          <label>帳號</label>
          <input type="text" v-model="newAccount.username">
        </div>
        <div class="form-item">
          <label>密碼</label>
          <input type="password" v-model="newAccount.password">
        </div>
        <div class="form-item">
          <label>等級</label>
          <select v-model="newAccount.permission">
            <option value="1">總管理員</option>
            <option value="2">一般帳號</option>
          </select>
        </div>
        <input class="button" type="submit" value="新增" @click="createAccount">
      </div>
      <span style="color: red;">{{ errorMsg }}</span>
      <div class="user-list" v-for="(item, index) in userData" :key="index">
        <span>帳號: {{ item.userName }}</span>
        <span>創建時間: {{ item.createTime }}</span>
        <input class="button" type="submit" value="修改" @click="updateAccount">
        <input class="button" type="submit" value="刪除" @click="deleteAccount">
      </div>
    </div>
  </div>
</template>
  
<script setup>
import { ref, onMounted } from 'vue'
import Menu from '../components/Menu.vue'
import axios from 'axios'

const newAccount = ref({
  username: '',
  password: '',
  permission: '1'
})
const userData = ref([])
const searchData = ref('')
const errorMsg = ref('')
const currentPage = ref(1);
const pageSize = 10;

const fetchUsers = async () => {
    try {
        const response = await axios.get(`/api/user/list?searchTerm=${searchData.value}&pageNumber=${currentPage.value}&pageSize=${pageSize}`);
        if (response.data.code === 0) {
            userData.value = response.data.data;
        } else {
            alert(response.data.message);
        }
    } catch (error) {
        console.error('error', error);
    }
};

const getUserList = async () => {
  try {
      const response = await axios.get('/api/user/list')
      if (response.data.code === 0) {
        userData.value = response.data.data
      } else {
        alert(response.data.message)
      }
    } catch (error) {
      console.error('error', error)
    }
}

onMounted(async () => {
  getUserList()
});


const isValidInput = (input) => {
    return !input.includes("'") && !input.includes(";");
}

const createAccount = async () => {
  if (newAccount.value.username === '' || newAccount.value.password === '') {
        errorMsg.value = '請輸入帳號、密碼'
        return;
    }

    if (!isValidInput(newAccount.value.username) || !isValidInput(newAccount.value.password)) {
        errorMsg.value = '帳號或密碼包含非法字符'
        return;
    }
  try {
    const response = await axios.post('/api/user/add', newAccount.value)
    if (response.data.code === 0) {
      getUserList()
      alert(response.data.message)
    } else {
      alert(response.data.message)
    }
  } catch (error) {
    console.error('error', error)
  }
}

const searchAccount = () => {
    currentPage.value = 1;
    fetchUsers()
};

const nextPage = () => {
    currentPage.value++;
    fetchUsers();
};

const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--;
      fetchUsers();
    }
};

const updateAccount = async () => {}

const deleteAccount = async () => {}
</script>
  
<style scoped>
.account-container {
  display: flex;
}
.add-form, .search-form {
  display: flex;
  justify-content: center;
}
.form-item {
  margin-right: 10px;
}
.button {
  height: 40px;
  cursor: pointer;
}
</style>