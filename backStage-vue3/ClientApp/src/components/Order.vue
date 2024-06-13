<template>
    <div class="order-container">
      <Menu></Menu>
      <h1>訂單系統</h1>
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
  .order-container {
    display: flex;
  }
  </style>