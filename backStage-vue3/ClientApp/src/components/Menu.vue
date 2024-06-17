<template>
  <n-menu :options="menuOptions" v-model:value="currentRoute" @update:value="handleMenuClick" />
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { NMenu } from 'naive-ui'

const router = useRouter()
const route = useRoute()

const userRole = ref('')
const currentRoute = ref(route.path)
const menuLoginList = ref([
  {
    role: "1",
    items: [
      { name: "帳號系統", path: "/account", key: '1' },
      { name: "會員系統", path: "/member", key: '2' },
      { name: "商品系統", path: "/product", key: '3' },
      { name: "訂單系統", path: "/order", key: '4' },
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
  {
    role: "3",
    items: [
      { name: "商品系統", path: "/product" },
      { name: "訂單系統", path: "/order" },
    ]
  },
])

const menuOptions = ref([])

onMounted(() => {
  const token = sessionStorage.getItem('token')
  const role = sessionStorage.getItem('role')
  if (role && token) {
    userRole.value = role
    const menu = menuLoginList.value.find(menu => menu.role === role)
    if (menu) {
      menuOptions.value = menu.items.map(item => ({
        label: item.name,
        key: item.path,
      }))
    }
  }
})

const handleMenuClick = (key) => {
  router.push(key)
}
</script>

<style scoped>
</style>
