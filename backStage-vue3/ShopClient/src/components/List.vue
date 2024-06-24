<template>
  <div class="list">
    <table>
      <thead>
        <tr>
          <th v-for="(title, index) in tableTitle" :key="index">{{ title }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.un }}</td>
          <td>{{ getPermissionLabel(user.permission) }}</td>
          <td>{{ user.createTime }}</td>
          <td v-if="user.permission !== '1'">
            <el-button v-if="canEditUser(user)" @click="$emit('edit', user)"
              >編輯</el-button
            >
            <el-popover
              v-if="canDeleteUser(user)"
              placement="top"
              width="160"
              trigger="click"
              :key="user.un"
              v-model="popoversVisible[user.un]"
            >
              <p>確認刪除此用戶？</p>
              <div class="btn-group" style="text-align: right">
                <el-button
                  size="mini"
                  type="text"
                  @click="popoversVisible[user.un] = false"
                  >取消</el-button
                >
                <el-button
                  type="primary"
                  size="mini"
                  @click="confirmDelete(user.un)"
                  >確認</el-button
                >
              </div>
              <el-button slot="reference" type="danger">刪除</el-button>
            </el-popover>
          </td>
          <td v-else>
            <el-button v-if="role === '1'" @click="$emit('edit', user)">
              編輯
            </el-button>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="pagination">
      <el-button @click="$emit('prevPage')" :disabled="pageNumber === 1">
        上一頁
      </el-button>
      <span>第 {{ pageNumber }} 頁</span>
      <el-button @click="$emit('nextPage')" :disabled="!hasMore"
        >下一頁</el-button
      >
    </div>
  </div>
</template>

<script>
import { store } from "@/store";

export default {
  name: "List",
  props: {
    tableTitle: {
      type: Array,
      default: () => [],
    },
    users: {
      type: Array,
      default: () => [],
    },
    hasMore: {
      type: Boolean,
      default: false,
    },
    pageNumber: {
      type: Number,
      default: 1,
    },
    pageSize: {
      type: Number,
      default: 10,
    },
  },
  data() {
    return {
      popoversVisible: {},
      currentUser: store.currentUser.un,
      role: store.currentUser.role,
      permissions: {
        1: "超級管理者",
        2: "管理者",
        3: "會員系統編輯者",
        4: "會員系統查看者",
        5: "商品系統編輯者",
        6: "商品系統查看者",
        7: "訂單系統編輯者",
        8: "訂單系統查看者",
      },
    };
  },
  methods: {
    getPermissionLabel(value) {
      return this.permissions[value];
    },
    confirmDelete(un) {
      this.$emit("delete", un);
    },
    canEditUser(user) {
      // 超級管理員可以編輯所有用戶
      if (this.role === "1") {
        return true;
      }
      // 管理員不能編輯其他管理員，但可以編輯其他權限用戶
      if (
        this.role === "2" &&
        user.permission !== "1" &&
        user.permission !== "2"
      ) {
        return true;
      }
      // 用戶可以編輯自己的信息
      if (user.un === this.currentUser) {
        return true;
      }
      return false;
    },
    canDeleteUser(user) {
      // 超級管理員可以刪除所有用戶
      if (this.role === "1" && user.un !== this.currentUser) {
        return true;
      }
      // 管理員不能刪除其他管理員，但可以刪除其他權限用戶
      if (
        this.role === "2" &&
        user.permission !== "1" &&
        user.permission !== "2"
      ) {
        return true;
      }
      return false;
    },
  },
};
</script>

<style scoped>
.list {
  padding: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 10px;
  text-align: center;
  border: 1px solid #ddd;
}

tbody tr:nth-child(odd) {
  background-color: #f9f9f9;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 20px 0;
}

.pagination button {
  margin: 0 5px;
}
</style>
