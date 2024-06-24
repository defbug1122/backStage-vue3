<template>
  <div>
    <div class="user-info">
      用戶: {{ currentUser.un }}
      <el-button type="primary" @click="openEditModal(currentUser)"
        >編輯</el-button
      >
      <el-button type="primary" @click="logout">登出</el-button>
    </div>
    <div class="acc-search-bar">
      <SearchBar :searchTerm="searchTerm" @search="fetchUsers" />
      <el-button @click="openCreateModal">新增</el-button>
    </div>
    <List
      :tableTitle="tableTitle"
      :users="users"
      :hasMore="hasMore"
      :pageNumber="pageNumber"
      :pageSize="pageSize"
      @edit="openEditModal"
      @delete="deleteUser"
      @prevPage="handlePrevPage"
      @nextPage="handleNextPage"
    />
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
import { getUserList, createAcc, deleteAcc, editAcc } from "@/service/api";
import SearchBar from "@/components/SearchBar.vue";
import UserModel from "@/components/UserModel.vue";
import List from "@/components/List.vue";
import { store, mutations } from "@/store";

export default {
  name: "Account",
  components: {
    SearchBar,
    UserModel,
    List,
  },
  data() {
    return {
      tableTitle: ["帳號", "權限", "創立時間", "操作"],
      currentUser: {
        un: store.currentUser.un,
        permission: store.currentUser.role,
      },
      searchTerm: "",
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
    };
  },
  methods: {
    // 登出
    logout() {
      mutations.setUserInfo({
        user: "",
        role: "",
        token: "",
      });
      sessionStorage.removeItem("token");
      sessionStorage.removeItem("currentUser");
      sessionStorage.removeItem("role");
      this.$router.push("/login");
      this.$message({
        message: "登出成功",
        type: "success",
        duration: 1200,
      });
    },

    // 取得用戶列表
    fetchUsers(searchTerm, pageNumber = this.pageNumber) {
      getUserList({
        searchTerm: searchTerm || this.searchTerm,
        pageNumber: pageNumber,
        pageSize: this.pageSize,
      })
        .then((response) => {
          this.users = response.data.data || [];
          this.hasMore = response.data.hasMore || false;
          this.pageNumber = pageNumber;
        })
        .catch((error) => {
          console.error("error", error);
          this.users = [];
          this.hasMore = false;
        });
    },

    // 上一頁功能
    handlePrevPage() {
      if (this.pageNumber > 1) {
        this.fetchUsers(this.searchTerm, this.pageNumber - 1);
      }
    },

    // 下一頁功能
    handleNextPage() {
      if (this.hasMore) {
        this.fetchUsers(this.searchTerm, this.pageNumber + 1);
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
            message: "用戶更新失敗222",
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
.user-info {
  display: flex;
  justify-content: end;
  align-items: center;
}
</style>
