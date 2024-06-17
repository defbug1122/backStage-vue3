<template>
  <div>
    <RouterView />
  </div>
</template>

<script setup>
import {ref, onMounted} from 'vue'
import { RouterView,useRoute, useRouter } from 'vue-router';
const router = useRouter();

onMounted(() => {
  const token = sessionStorage.getItem('token')
  if (!token) {
    router.push('/login')
  }
})

</script>

<style scoped>
</style>
