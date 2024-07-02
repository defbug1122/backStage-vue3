<template>
  <div class="acc-container">
    <UserInfo />
    <SearchableList
      :searchTerm="searchTerm"
      @search="fetchUsers"
      :showSort="true"
      :sortOptions="sortOptions"
      :tableTitle="tableTitle"
      :tableData="users"
      :hasMore="hasMore"
      :pageNumber="pageNumber"
      :pageSize="pageSize"
      :showAddButton="canAddUser()"
      @prevPage="handlePrevPage"
      @nextPage="handleNextPage"
      @add="openCreateModal"
    >
      <template #table-rows="{ tableData }">
        <tr v-for="item in tableData" :key="item.id">
          <td>{{ item.un }}</td>
          <td>{{ getPermissionLabels(item.permission) }}</td>
          <td>{{ item.createTime }}</td>
          <td>
            <el-button v-if="canEditUser(item)" @click="openEditModal(item)"
              >編輯</el-button
            >
            <el-popover
              v-if="canDeleteUser(item)"
              placement="top"
              width="160"
              trigger="click"
              :key="item.un"
              v-model="popoversVisible[item.un]"
            >
              <p>確認刪除此用戶？</p>
              <div class="btn-group" style="text-align: right">
                <el-button
                  size="mini"
                  type="text"
                  @click="popoversVisible[item.un] = false"
                  >取消</el-button
                >
                <el-button
                  type="primary"
                  size="mini"
                  @click="deleteUser(item.un)"
                  >確認</el-button
                >
              </div>
              <el-button slot="reference" type="danger">刪除</el-button>
            </el-popover>
          </td>
        </tr>
      </template>
    </SearchableList>
    <UserModel
      :showModal="showModal"
      :isEditMode="isEditMode"
      :user="userData"
      @save="handleSave"
      @close="closeModal"
    />
  </div>
</template>

<script>
import {
  getUserList,
  createAcc,
  deleteAcc,
  editAcc,
  logout,
} from "@/service/api";
import SearchableList from "@/components/SearchableList.vue";
import UserModel from "@/components/UserModel.vue";
import UserInfo from "@/components/UserInfo.vue";
import { store, mutations } from "@/store";

export default {
  name: "Account",
  components: {
    SearchableList,
    UserModel,
    UserInfo,
  },
  data() {
    return {
      tableTitle: ["帳號", "權限", "創立時間", "操作"],
      sortOptions: [
        { label: "按會員名稱排序", value: 1 },
        { label: "按創立日期排序", value: 2 },
      ],
      currentUser: store.currentUser.un,
      role: store.currentUser.role,
      searchTerm: "",
      sortBy: 1,
      users: [],
      showModal: false,
      hasMore: false,
      isEditMode: false,
      userData: {
        un: "",
        pwd: "",
        permission: "",
      },
      pageNumber: 1,
      pageSize: 10,
      pattern: /^[a-zA-Z0-9_-]{4,16}$/,
      popoversVisible: {},
      permissionMap: store.permissionMap,
    };
  },
  methods: {
    // 是否有新增用戶權限
    canAddUser() {
      return (this.role & 2) === 2;
    },

    // 呼叫登出 API
    async logout() {
      try {
        const response = await logout();
        if (response.data.code === 0) {
          this.$message({
            message: "登出成功",
            type: "success",
            duration: 1200,
          });
          // 清理相關資料
          sessionStorage.removeItem("token");
          sessionStorage.removeItem("currentUser");
          sessionStorage.removeItem("role");
          mutations.setUserInfo({
            user: "",
            role: "",
            token: "",
          });
          this.$router.push("/login");
        } else {
          this.$message({
            message: "登出失败",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        this.$message({
          message: "登出请求失败",
          type: "error",
          duration: 1200,
        });
      }
    },

    // 取得用戶列表
    async fetchUsers(
      searchTerm,
      pageNumber = this.pageNumber,
      sortBy = this.sortBy
    ) {
      const response = await getUserList({
        searchTerm: searchTerm || this.searchTerm,
        pageNumber: pageNumber,
        pageSize: this.pageSize,
        sortBy: sortBy,
      });
      try {
        if (response.data.code === 0) {
          this.users = response.data.data || [];
          this.hasMore = response.data.hasMore || false;
          this.pageNumber = pageNumber;
        } else {
          this.$message({
            message: "資料獲取失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.users = [];
        this.hasMore = false;
      }
    },

    // 上一頁功能
    handlePrevPage(searchTerm, sortBy) {
      if (this.pageNumber > 1) {
        this.fetchUsers(searchTerm, this.pageNumber - 1, sortBy);
      }
    },

    // 下一頁功能
    handleNextPage(searchTerm, sortBy) {
      if (this.hasMore) {
        this.fetchUsers(searchTerm, this.pageNumber + 1, sortBy);
      }
    },

    // 打開"新增用戶"彈窗
    openCreateModal() {
      this.resetUser();
      this.isEditMode = false;
      this.showModal = true;
    },

    // 打開"編輯用戶"彈窗
    openEditModal(user) {
      this.userData = { ...user };
      this.isEditMode = true;
      this.showModal = true;
    },

    // 關閉"新增用戶" 或是 "編輯用戶" 彈窗
    closeModal() {
      this.showModal = false;
    },

    // 清空用戶資料
    resetUser() {
      this.userData = {
        un: "",
        pwd: "",
        permission: "",
      };
    },

    // 保存彈窗設定
    handleSave(user) {
      if (this.isEditMode) {
        this.updateUser(user);
      } else {
        this.createUser(user);
      }
    },

    // 呼叫 新增用戶 API
    async createUser(user) {
      if (!user.un || !user.pwd) {
        this.$message({
          message: "創建的帳號或密碼不能為空",
          type: "error",
          duration: 1200,
        });
        return;
      }
      if (!user.permission) {
        this.$message({
          message: "請選擇權限",
          type: "error",
          duration: 1200,
        });
        return;
      }
      if (!this.pattern.test(user.un) || !this.pattern.test(user.pwd)) {
        this.$message({
          message:
            "帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符",
          type: "error",
          duration: 1200,
        });
        return;
      }

      try {
        const response = await createAcc(user);
        if (response.data.code === 0) {
          this.$message({
            message: "新增成功",
            type: "success",
            duration: 1200,
          });
          this.fetchUsers();
          this.closeModal();
        } else {
          this.$message({
            message: "該用戶已存在",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.log(error);
        this.$message({
          message: "創建失敗",
          type: "error",
          duration: 1200,
        });
      }
    },

    // 呼叫 更新用戶 API
    async updateUser(user) {
      if (!user.pwd) {
        this.$message({
          message: "密碼為空",
          type: "error",
          duration: 1200,
        });
        return;
      }
      if (!this.pattern.test(user.pwd)) {
        this.$message({
          message:
            "帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符",
          type: "error",
          duration: 1200,
        });
        return;
      }
      try {
        const response = await editAcc(user);
        if (response.data.code === 0) {
          this.fetchUsers();
          this.closeModal();
          this.$message({
            message: "用戶更新成功",
            type: "success",
            duration: 1200,
          });
        } else {
          this.$message({
            message: "用戶更新失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "用戶更新失敗",
          type: "error",
          duration: 1200,
        });
      }
    },

    // 呼叫 刪除用戶 API
    async deleteUser(un) {
      try {
        const response = await deleteAcc({ un: un });
        if (response.data.code === 0) {
          this.fetchUsers();
          this.$message({
            message: "用戶刪除成功",
            type: "success",
            duration: 2000,
          });
        } else {
          this.$message({
            message: "用戶刪除失敗",
            type: "error",
            duration: 2000,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "用戶刪除失敗",
          type: "error",
          duration: 2000,
        });
      }
    },

    // 確認刪除用戶
    confirmDelete(un) {
      this.$emit("delete", un);
    },

    // 判斷是否能編輯用戶
    canEditUser(item) {
      if (item.permission === 16383 && this.currentUser !== item.un) {
        return false;
      } else if (this.role === 16383 || (this.role & 4) === 4) {
        return true;
      } else {
        return false;
      }
    },

    // 判斷是否能刪除用戶
    canDeleteUser(item) {
      const itemPermission = item.permission;
      if (item.un === this.currentUser) {
        return false;
      }
      if (itemPermission === 16383) {
        return false;
      }
      if ((this.role & 2) === 2) {
        return true;
      }
      return false;
    },

    // 獲取權限標籤
    getPermissionLabels(permission) {
      let labels = [];
      for (let key in this.permissionMap) {
        if (permission & key) {
          labels.push(this.permissionMap[key]);
        }
      }
      return labels.join(", ");
    },
  },
  created() {
    this.fetchUsers();
  },
};
</script>

<style scoped>
.acc-search-bar {
  display: flex;
  justify-content: center;
}
</style>
