<template>
    <div v-for="(menuItem, index) in menuLoginList" :key="index" class="menu-container">
      <div v-if="userRole === menuItem.role">
        <div class="menu-item">
          <span class="menu-link cursor-pointer">
            <div v-for="(item, i) in menuItem.items" :key="i">
              <router-link :to="item.path">{{ item.name }}</router-link>
            </div>
          </span>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue'
  
  const userRole = ref('')
  const menuLoginList = ref([
    {
      role: "",
      items: [
        { name: "", path: "" }
      ]
    }
  ])
  
  onMounted(() => {
    const token = sessionStorage.getItem('token')
    const role = sessionStorage.getItem('role')
    if (role && token) {
      userRole.value = role
    }
    // 设置菜单列表
    menuLoginList.value = [
      {
        role: "1",
        items: [
          { name: "帳號系統", path: "/account" },
          { name: "會員系統", path: "/member" },
          { name: "商品系統", path: "/product" },
          { name: "訂單系統", path: "/order" },
        ]
      },
      {
        role: "2",
        items: [
          { name: "會員系統", path: "/member" },
          { name: "商品系統", path: "/product" },
          { name: "訂單系統", path: "/order" },
        ]
      },
    ]
  })
  </script>
  
  
  <style scoped>
  .menu-container {
    width: 150px;
  }
  </style>
  