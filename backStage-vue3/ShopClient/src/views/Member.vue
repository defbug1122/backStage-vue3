<template>
  <div class="member-container">
    <h1>會員系統</h1>
    <List />
  </div>
</template>

<script>
import List from "@/components/List.vue";
import { store } from "@/store";

export default {
  name: "Member",
  components: {
    List,
  },
  data() {
    return {
      tableTitle: ["會員名稱", "等級", "帳號狀態", "操作"],
      menuList: [],
      curr: store,
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
.member-container {
  display: flex;
}
</style>
