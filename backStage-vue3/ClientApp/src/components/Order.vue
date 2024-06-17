<template>
    <div>
      <n-layout has-sider class="order-container">
        <n-layout-sider style="width: 20vw; background-color: #f0f2f5;">
          <Menu />
        </n-layout-sider>
        <n-layout-content style="padding: 16px; width: 80vw; height: 100vh;">訂單系統</n-layout-content>
      </n-layout>
    </div>
  </template>
  
  <script setup>
  import {ref, onMounted} from 'vue'
  import Menu from '../components/Menu.vue'
    import { NTable, NInput, NInputGroup, NButton, NLayout, NLayoutSider, NLayoutContent, NSelect } from 'naive-ui'
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
    width: 100vw;
  }
  </style>