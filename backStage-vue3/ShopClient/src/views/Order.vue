<template>
  <div class="order-container">
    <h1>訂單系統</h1>
  </div>
</template>

<script>
import Menu from "../components/Menu.vue";

export default {
  name: "Order",
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
.order-container {
  display: flex;
}
</style>
