<template>
  <el-menu
    :default-active="currentRoute"
    class="el-menu"
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
      menuOptions: [],
      menuLoginList: {
        account: {
          name: "帳號系統",
          path: "/account",
          key: "1",
          permission: [1, 2, 4, 8],
        },
        member: {
          name: "會員系統",
          path: "/member",
          key: "2",
          permission: [16, 32, 64],
        },
        product: {
          name: "商品系統",
          path: "/product",
          key: "3",
          permission: [128, 256, 512, 1024],
        },
        order: {
          name: "訂單系統",
          path: "/order",
          key: "4",
          permission: [2048, 4096, 8192],
        },
      },
    };
  },
  created() {
    const token = sessionStorage.getItem("token");
    const role = sessionStorage.getItem("role");

    if (role && token) {
      this.generateMenuOptions(role);
    }
  },
  methods: {
    // 判斷目前用戶權限會有的菜單內容
    generateMenuOptions(role) {
      const menuOptions = [];

      for (const key in this.menuLoginList) {
        const menu = this.menuLoginList[key];
        if (menu.permission.some((p) => (role & p) === p)) {
          menuOptions.push(menu);
        }
      }

      this.menuOptions = menuOptions;
    },

    // 點擊選單導向路徑
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
