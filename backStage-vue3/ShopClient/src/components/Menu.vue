<template>
  <el-menu
    :default-active="currentRoute"
    class="el-menu-vertical-demo"
    @select="handleMenuClick"
  >
    <el-menu-item
      v-for="menuItem in menuOptions"
      :key="menuItem.key"
      :index="menuItem.path"
    >
      {{ menuItem.name }}
    </el-menu-item>
  </el-menu>
</template>

<script>
export default {
  name: "Menu",
  data() {
    return {
      currentRoute: this.$route.path,
      userRole: "",
      menuOptions: [],
      menuLoginList: [
        {
          role: ["1", "2"],
          items: [
            { name: "帳號系統", path: "/account", key: "1" },
            { name: "會員系統", path: "/member", key: "2" },
            { name: "商品系統", path: "/product", key: "3" },
            { name: "訂單系統", path: "/order", key: "4" },
          ],
        },
        {
          role: ["3", "4"],
          items: [{ name: "會員系統", path: "/member", key: "2" }],
        },
        {
          role: ["5", "6"],
          items: [{ name: "商品系統", path: "/product", key: "3" }],
        },
        {
          role: ["7", "8"],
          items: [{ name: "訂單系統", path: "/order", key: "4" }],
        },
      ],
    };
  },
  created() {
    const token = sessionStorage.getItem("token");
    const role = sessionStorage.getItem("role");
    if (role && token) {
      this.userRole = role;
      const menu = this.menuLoginList.find((menu) => menu.role.includes(role));
      if (menu) {
        this.menuOptions = menu.items;
      }
    }
  },
  methods: {
    handleMenuClick(path) {
      this.$router.push(path);
    },
  },
};
</script>

<style scoped>
.el-menu {
  height: 100%;
}
.el-menu-item {
  font-size: 16px;
  text-align: center;
}
</style>
