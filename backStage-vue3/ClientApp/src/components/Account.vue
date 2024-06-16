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
            <option value="1">超級管理員</option>
            <option value="2">管理員</option>
            <option value="3">普通用戶</option>
          </select>
        </div>
        <input class="button" type="submit" value="新增" @click="createAccount">
      </div>
      <span style="color: red;">{{ errorMsg }}</span>
      <div v-if="userData.length > 0" class="user-list" v-for="(item, index) in userData" :key="index">
        <span>帳號: {{ item.userName }}</span>
        <div class="password-container">
          <label>密碼:</label>
          <span class="password" v-if="!item.isEditing" @click="togglePassword(item)">
            <span v-if="!item.showPassword" >•••••</span>
            <span v-else>{{ item.password }}</span>
          </span>
          <input v-else type="password" v-model="item.password">
          <!-- <button v-if="!item.isEditing" @click="togglePassword(item)">{{ item.showPassword ? '隱藏' : '顯示' }}</button> -->
        </div>
        <div v-if="!item.isEditing">
          <span v-if="item.permission === '1'">等級:超級管理員</span>
          <span v-else-if="item.permission === '2'">等級:管理員</span>
          <span v-else>等級:普通用戶</span>
        </div>
        <select v-else v-model="item.permission" :value="item.permission">
          <option value="1">超級管理員</option>
          <option value="2">管理員</option>
          <option value="3">普通用戶</option>
        </select>
        <span>創立時間{{item.createTime}}</span>
        <div v-if="!item.isEditing">
          <input class="button" type="submit" value="修改" @click="isEditAccount(item)">
          <input class="button" type="submit" value="刪除" @click="deleteAccount(item.userName)">
        </div>
        <div v-else>
          <input class="button" type="submit" value="確定" @click="updateAccount(item)">
          <input class="button" type="submit" value="取消" @click="isEditAccount(item)">
        </div>
      </div>
      <span v-else>查無資料</span>
      <div class="pagination">
        <button @click="prevPage" :disabled="currentPage === 1">上一頁</button>
        {{ currentPage }}
        <button @click="nextPage" :disabled="!hasMore">下一頁</button>
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
const editAccount  = ref ({
    password: '',
    permission: ''
})
const userData = ref([])
const searchData = ref('')
const errorMsg = ref('')
const currentPage = ref(1);
const pageSize = 10;
const hasMore = ref(true);
const pattern = /^[a-zA-Z0-9_-]{4,16}$/;

const fetchUsers = async () => {
  try {
    const response = await axios.get(`/api/user/list?searchTerm=${searchData.value}&pageNumber=${currentPage.value}&pageSize=${pageSize}`);
    if (response.data.code === 0) {
      userData.value = response.data.data;
      hasMore.value = response.data.hasMore;
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
      hasMore.value = response.data.hasMore;
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
  //const passwordPattern = "[~!/@#$%^&*()?\\|[{}];:\'\",<.>/?]+"
  if (newAccount.value.username === '' || newAccount.value.password === '') {
    errorMsg.value = '請輸入帳號、密碼'
    return;
  }

  if (!pattern.test(newAccount.value.username) || !pattern.test(newAccount.value.password)) {
    errorMsg.value = '帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符';
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

const deleteAccount = async (userName) => {
  try {
    const response = await axios.post('/api/user/delete', {
    userName: userName
  })
    if (response.data.code === 0) {
      currentPage.value = 1;
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

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--;
    fetchUsers();
  }
};

const nextPage = () => {
  if (hasMore.value) {
    currentPage.value++;
    fetchUsers();
  }
};

const togglePassword = (user) => {
  user.showPassword = !user.showPassword;
}

const isEditAccount = (user) => {
  user.isEditing = !user.isEditing;
}

const updateAccount = async (user) => {
  if (!pattern.test(user.password)) {
    errorMsg.value = '帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符';
    return;
  } else {
    errorMsg.value = ''
  }
  try {
    const response = await axios.post('/api/user/update', {
    userName: user.userName,
    password: user.password,
    permission: user.permission
  })
    if (response.data.code === 0) {
      currentPage.value = 1;
      getUserList()
      alert(response.data.message)
    } else {
      alert(response.data.message)
    }
  } catch (error) {
    console.error('error', error)
  }
}

</script>
  
<style scoped>
.account-container {
  display: flex;
}
.block {
  width: 80%;
}
.add-form, .search-form {
  display: flex;
  justify-content: center;
}
.user-list {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.form-item {
  margin-right: 10px;
}
.button {
  height: 40px;
  cursor: pointer;
}
.password {
  cursor: pointer;
}
</style>