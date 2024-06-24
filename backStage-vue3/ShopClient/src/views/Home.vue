<template>
  <div class="home-container">
    <h1>首頁</h1>
  </div>
</template>

<script>
export default {
  name: "Home",
  data() {
    return {
      menuList: [],
    };
  },
  mounted() {
    this.fetchData();
  },
  methods: {
    async fetchData() {
      console.log("df");
    },
  },
};
</script>

<style scoped>
.read-the-docs {
  color: #888;
}
.home-container {
  display: flex;
}
</style>
