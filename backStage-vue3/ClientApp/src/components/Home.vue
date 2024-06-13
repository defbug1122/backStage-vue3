<template>
  <div class="home-container">
    <Menu></Menu>
    <h1>首頁</h1>
  </div>
</template>

<script setup>
import {ref, onMounted} from 'vue'
import Menu from '../components/Menu.vue'
const menuList = ref([]);
onMounted(async () => {
})

</script>

<style scoped>
.read-the-docs {
  color: #888;
}
.home-container {
  display: flex;
}
</style>
