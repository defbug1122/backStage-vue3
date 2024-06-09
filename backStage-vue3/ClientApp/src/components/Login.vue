<template>
  <h1>我是登入頁-0607</h1>
  <form action="/">
  <label for="fname">帳號</label><br>
  <input type="text" id="fname" v-model="account" value="John"><br>
  <label for="lname">密碼</label><br>
  <input type="password" id="lname" v-model="password" value="Doe"><br><br>
  <input type="submit" value="Submit">
</form> 
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from "axios";

defineProps({
  msg: String,
})

const account = ref('')
const password = ref('')

onMounted(async () => {
  await fetchUnits();
});

const fetchUnits = async () => {
  try {
    const response = await axios.get('https://localhost:44336/api/users');
    console.log('response------',response);
    // if (response.data.success) {
    //   units.value = response.data.data.map(item=>{
    //     return ({...item, value: item.id, label: item.name})
    //   });
    // }
  } catch (error) {
    console.error('API request failed', error);
  }
};

</script>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>