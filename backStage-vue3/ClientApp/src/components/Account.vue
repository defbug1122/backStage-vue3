<template>
  <div>
    <n-layout has-sider class="account-container">
      <n-layout-sider style="width: 20vw; background-color: #f0f2f5;">
        <Menu />
      </n-layout-sider>
      <n-layout-content style="padding: 16px; width: 80vw; height: 100vh;">
        <div class="content-container">
          <div style="text-align: right;">用戶: {{ currentUser }}</div>
          <div class="block">
            <n-input-group>
              <n-input v-model:value="searchData" :style="{ width: '50%' }" placeholder="請輸入欲查詢帳號"/>
              <n-button type="primary" @click="searchAccount">
                查詢
              </n-button>
            </n-input-group>
            <div class="add-form">
              <div class="form-item">
                <div class="form-title">帳號</div>
                <n-input type="text" v-model:value="newAccount.un"  placeholder="請輸入欲新增帳號"/>
              </div>
              <div class="form-item">
                <div class="form-title">密碼</div>
                <n-input type="passward" v-model:value="newAccount.pwd" placeholder="請輸入欲新增密碼"/>
              </div>
              <div class="form-item">
                <div class="form-title">等級</div>
                <n-select v-model:value="newAccount.permission" :options="options" class="permission-select" />
              </div>
              <n-button type="primary" @click="createAccount">
                新增
              </n-button>
            </div>
            <span class="error-message">{{ errorMsg }}</span>      
            <n-table v-if="userData.length > 0">
              <thead>
                <tr>
                  <th>帳號</th>
                  <th>密碼</th>
                  <th>等級</th>
                  <th>創立時間</th>
                  <th>操作</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, index) in userData" :key="index">
                  <td>{{ item.un }}</td>
                  <td>
                    <span v-if="!item.isEditing">
                      <span v-if="!item.showpwd">•••••</span>
                    </span>
                    <input v-else type="passward" v-model="editPwd">
                  </td>
                  <td v-if="!item.isEditing">
                    <span v-if="item.permission === '1'">超級管理員</span>
                    <span v-else-if="item.permission === '2'">管理員</span>
                    <span v-else-if="item.permission === '3'">會員系統編輯者</span>
                    <span v-else-if="item.permission === '4'">會員系統查看者</span>
                    <span v-else-if="item.permission === '5'">商品系統編輯者</span>
                    <span v-else-if="item.permission === '6'">商品系統查看者</span>
                    <span v-else-if="item.permission === '7'">訂單系統編輯者</span>
                    <span v-else-if="item.permission === '8'">訂單系統查看者</span>
                  </td>
                  <td v-else>
                    <n-select v-if="item.permission !== '1' && item.permission !== '2'" v-model:value="item.permission" :options="options" />
                  </td>
                  <td>{{item.createTime}}</td>
                  <td v-if="!item.isEditing && item.permission !== '1'">
                    <n-button class="button" @click="isEditAccount(item)">修改</n-button>
                    <n-button v-if="currentUser !== item.un" class="button" @click="deleteAccount(item.un)">刪除</n-button>
                  </td>
                  <td v-if="!item.isEditing && currentRole === '1' && item.permission === '1'">
                    <n-button class="button" @click="isEditAccount(item)">修改</n-button>
                  </td>
                  <td v-if="item.isEditing">
                    <n-button class="button" @click="updateAccount(item, editPwd)">確定</n-button>
                    <n-button class="button" @click="isEditAccount(item)">取消</n-button>
                  </td>
                </tr>
              </tbody>
            </n-table>
            <div class="no-data" v-else>查無資料</div>
            <div class="pagination">
              <button @click="prevPage" :disabled="currentPage === 1">上一頁</button>
              <div style="margin: 0 5px;">{{ currentPage }}</div>
              <button @click="nextPage" :disabled="!hasMore">下一頁</button>
            </div>
          </div>
        </div>
      </n-layout-content>
    </n-layout>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import Menu from '../components/Menu.vue'
import { NTable, NInput, NInputGroup, NButton, NLayout, NLayoutSider, NLayoutContent, NSelect } from 'naive-ui'

const newAccount = ref({
  un: '',
  pwd: '',
  permission: ''
})
const editPwd = ref('')
const userData = ref([])
const searchData = ref('')
const errorMsg = ref('')
const currentPage = ref(1);
const pageSize = 10;
const hasMore = ref(true);
const currentUser = sessionStorage.getItem('currentUser')
const currentRole = sessionStorage.getItem('role')
const options = [
  {
    label: '管理者',
    value: '2'
  },
  {
    label: '會員系統編輯者',
    value: '3'
  },
  {
    label: '會員系統查看者',
    value: '4'
  },
  {
    label: '商品系統編輯者',
    value: '5'
  },
  {
    label: '商品系統查看者',
    value: '6'
  },
  {
    label: '訂單系統編輯者',
    value: '7'
  },
  {
    label: '訂單系統查看者',
    value: '8'
  },
]
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

const createAccount = async () => {
  if (newAccount.value.un === '' || newAccount.value.pwd === '') {
    errorMsg.value = '請輸入帳號、密碼'
    return
  } else {
    errorMsg.value = ''
  }

  if (newAccount.value.permission === '') {
    errorMsg.value = '請選擇權限'
    return
  } else {
    errorMsg.value = ''
  }

  if (!pattern.test(newAccount.value.un) || !pattern.test(newAccount.value.pwd)) {
    errorMsg.value = '帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符';
    return
  } else {
    errorMsg.value = ''
  }

  try {
    const response = await axios.post('/api/user/add', newAccount.value)
    if (response.data.code === 0) {
      getUserList()
      alert('創建成功')
    } else {
      alert('創建失敗，用戶已存在')
    }
  } catch (error) {
    console.error('error', error)
  }
}

const deleteAccount = async (un) => {
  try {
    const response = await axios.post('/api/user/delete', {
      un: un
    })
    if (response.data.code === 0) {
      currentPage.value = 1;
      getUserList()
      alert('刪除成功')
    } else if (response.data.code === 4) {
      alert('不能刪除自己')
    }
      else { 
      alert('刪除失敗')
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

const togglepwd = (user) => {
  user.showpwd = !user.showpwd;
}

const isEditAccount = (user) => {
  user.isEditing = !user.isEditing;
}

const updateAccount = async (user, editPwd) => {
  console.log(user,'ssdsdsds')
  if (editPwd === '') {
    errorMsg.value = '請輸入密碼'
    return
  } else {
    errorMsg.value = ''
  }
  if (!pattern.test(editPwd)) {
    errorMsg.value = '帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符';
    return
  } else {
    errorMsg.value = ''
  }
  try {
    const response = await axios.post('/api/user/update', {
      un: user.un,
      pwd: editPwd,
      permission: user.permission
    })
    if (response.data.code === 0) {
      currentPage.value = 1;
      getUserList()
      alert('更新成功')
    } else {
      alert('更新失敗')
    }
  } catch (error) {
    console.error('error', error)
  }
}

</script>

<style scoped>
.account-container {
  display: flex;
  width: 100vw;
}

.content-container {
  flex: 1;
  padding: 16px;
}

.block {
  margin: 0 auto;
  width: 100%;
  max-width: 1200px;
}

.add-form, .search-form {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 20px 0;
}

.permission-select {
  width: 150px;
}

.user-list {
  display: flex;
  flex-direction: column;
}

.user-list-title {
  display: flex;
}

.title {
  text-align: center;
  width: 100px;
  margin: 0 65px 10px 65px;
}

.no-data {
  font-size: 20px;
  text-align: center;
  margin: 20px;
}

.user-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 10px;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
}

.form-item {
  display: flex;
  margin-right: 10px;
}

.form-title {
  width: 50px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.n-input-group {
  justify-content: center;
}

.n-table th, td {
  text-align: center;
}

.button {
  height: 40px;
  margin: 0 5px;
  cursor: pointer;
}

.pwd-container {
  width: 100px;
  text-align: center;
}

.pwd {
  cursor: pointer;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 20px;
}

.error-message {
  color: red;
  display: flex;
  justify-content: center;
  margin-bottom: 10px;
}
</style>
