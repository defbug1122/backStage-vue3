<template>
  <div class="product-container">
    <h1>商品系統</h1>
  </div>
</template>

<script>
import Menu from "../components/Menu.vue";

export default {
  name: "Product",
  components: {
    Menu,
  },
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
.product-container {
  display: flex;
}
</style>
