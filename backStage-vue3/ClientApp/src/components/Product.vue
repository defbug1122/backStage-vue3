<template>
    <div class="product-container">
      <Menu></Menu>
      <h1>商品系統</h1>
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
  .product-container {
    display: flex;
  }
  </style>