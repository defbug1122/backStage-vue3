<template>
  <div class="product-container">
    <h1>商品系統</h1>
  </div>
</template>

<script>
export default {
  name: "Product",
  components: {},
  data() {
    return {};
  },
  mounted() {
    this.fetchData();
  },
  methods: {
    async fetchData() {
      console.log("");
    },
  },
};
</script>

<style scoped>
.read-the-docs {
  color: #888;
}
.product-container {
  display: flex;
}
</style>
