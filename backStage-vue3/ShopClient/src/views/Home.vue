<template>
  <div class="home-container">
    <span>"歡迎回來，</span>
    <span>請點選左側選單繼續"</span>
  </div>
</template>

<script>
export default {
  name: "Home",
  data() {
    return {};
  },
};
</script>

<style scoped>
.home-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: 400;
  font-size: 22px;
  color: #333;
  font-family:
    Zapf Humanist,
    serif;
}
</style>
