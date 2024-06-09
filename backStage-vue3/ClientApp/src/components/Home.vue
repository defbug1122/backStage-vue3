<template>
  <h1>我是首頁</h1>
  <nav>
    <router-link to="/login">Go to Login</router-link>
  </nav>
</template>

<script setup>
import { ref } from 'vue'

defineProps({
  msg: String,
})

const count = ref(0)
</script>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>
