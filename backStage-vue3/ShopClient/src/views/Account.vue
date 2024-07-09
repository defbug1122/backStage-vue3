<template>
  <div class="acc-container">
    <UserInfo />
    <SearchList
      :searchTerm="searchTerm"
      @search="FetchUsers"
      :showSort="true"
      :sortOptions="sortOptions"
      :tableTitle="tableTitle"
      :tableData="users"
      :hasMore="hasMore"
      :pageNumber="pageNumber"
      :pageSize="pageSize"
      :showAddButton="CanAddUser()"
      @prevPage="HandlePrevPage"
      @nextPage="HandleNextPage"
      @add="OpenCreateModal"
    >
      <template #table-rows="{ tableData }">
        <tr v-for="item in tableData" :key="item.id">
          <td>{{ item.userName }}</td>
          <td>{{ GetPermissionLabels(item.permission) }}</td>
          <td>{{ item.createTime }}</td>
          <td>
            <el-button
              v-if="CanEditUser(item)"
              icon="el-icon-edit"
              @click="OpenEditUserRoleModal(item)"
              >權限</el-button
            >
            <el-button
              v-if="CanEditUser(item)"
              icon="el-icon-refresh-left"
              @click="OpenEditPasswordModal(item)"
              >密碼</el-button
            >
            <el-popover
              v-if="CanDeleteUser(item)"
              placement="top"
              width="160"
              trigger="click"
              :key="item.userName"
              v-model="popoversVisible[item.userName]"
            >
              <p>確認刪除此用戶？</p>
              <div class="btn-group" style="text-align: right">
                <el-button
                  size="mini"
                  type="text"
                  @click="popoversVisible[item.userName] = false"
                  >取消</el-button
                >
                <el-button
                  type="primary"
                  size="mini"
                  @click="DeleteUser(item.id)"
                  >確認</el-button
                >
              </div>
              <el-button icon="el-icon-delete" slot="reference" type="danger"
                >刪除</el-button
              >
            </el-popover>
          </td>
        </tr>
      </template>
    </SearchList>

    <!-- 新增用戶彈窗 -->
    <AddUserModal
      :showModal="showAddUserModal"
      @close="CloseAddUserModal"
      @save="HandleAddUserSave"
    />

    <!-- 編輯用戶權限彈窗 -->
    <EditUserRoleModal
      :showModal="showEditUserRoleModal"
      :user="selectedUser"
      @close="CloseEditUserRoleModal"
      @save="HandleEditUserRoleSave"
    />

    <!-- 編輯密碼彈窗 -->
    <EditPwdModal
      :showModal="showEditPasswordModal"
      :user="selectedUser"
      @close="CloseEditPasswordModal"
      @save="HandleEditPasswordSave"
    />
  </div>
</template>

<script>
import {
  GetUserList,
  CreateAcc,
  DeleteAcc,
  EditAccPwd,
  EditAccRole,
} from "@/service/api";
import SearchList from "@/components/SearchList.vue";
import UserInfo from "@/components/UserInfo.vue";
import AddUserModal from "@/components/Modal/AddUserModal.vue";
import EditUserRoleModal from "@/components/Modal/EditUserRoleModal.vue";
import EditPwdModal from "@/components/Modal/EditPwdModal.vue";
import { store } from "@/store";
import { permissionMap } from "@/definition/permission";

export default {
  name: "Account",
  components: {
    SearchList,
    UserInfo,
    AddUserModal,
    EditUserRoleModal,
    EditPwdModal,
  },
  data() {
    return {
      tableTitle: ["帳號", "權限", "創立時間", "操作"],
      sortOptions: [
        { label: "按會員名稱排序", value: 1 },
        { label: "按創立日期排序", value: 2 },
      ],
      currentUser: store.currentUser.user,
      role: Number(store.currentUser.role),
      searchTerm: "",
      sortBy: 1,
      users: [],
      hasMore: false,
      pageNumber: 1,
      pageSize: 10,
      popoversVisible: {},
      permissionMap: permissionMap,
      showAddUserModal: false,
      showEditUserRoleModal: false,
      showEditPasswordModal: false,
      selectedUser: {
        userName: "",
        pwd: "",
        permission: 0,
      },
      pattern: /^[a-zA-Z0-9_-]{4,16}$/,
    };
  },
  methods: {
    // 是否有新增用戶權限
    CanAddUser() {
      return (this.role & 2) === 2;
    },

    // 是否有編輯用戶權限
    CanEditUser(item) {
      if (item.permission === 32767 || item.userName === this.currentUser) {
        return false;
      }
      return (this.role & 8) === 8;
    },

    // 取得用戶列表
    async FetchUsers(
      searchTerm,
      pageNumber = this.pageNumber,
      sortBy = this.sortBy
    ) {
      const response = await GetUserList({
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
    HandlePrevPage(searchTerm, sortBy) {
      if (this.pageNumber > 1) {
        this.FetchUsers(searchTerm, this.pageNumber - 1, sortBy);
      }
    },

    // 下一頁功能
    HandleNextPage(searchTerm, sortBy) {
      if (this.hasMore) {
        this.FetchUsers(searchTerm, this.pageNumber + 1, sortBy);
      }
    },

    // 打開"新增用戶"彈窗
    OpenCreateModal() {
      this.ResetUser();
      this.showAddUserModal = true;
    },

    // 打開"編輯用戶權限"彈窗
    OpenEditUserRoleModal(user) {
      this.selectedUser = { ...user };
      this.showEditUserRoleModal = true;
    },

    // 打開"編輯密碼"彈窗
    OpenEditPasswordModal(user) {
      this.selectedUser = { ...user };
      this.showEditPasswordModal = true;
    },

    // 關閉"新增用戶"彈窗
    CloseAddUserModal() {
      this.showAddUserModal = false;
    },

    // 關閉"編輯用戶權限"彈窗
    CloseEditUserRoleModal() {
      this.showEditUserRoleModal = false;
    },

    // 關閉"編輯密碼"彈窗
    CloseEditPasswordModal() {
      this.showEditPasswordModal = false;
    },

    // 清空用戶資料
    ResetUser() {
      this.selectedUser = {
        userName: "",
        pwd: "",
        permission: 0,
      };
    },

    // 保存新增用戶
    HandleAddUserSave(user) {
      this.CreateUser(user);
    },

    // 保存編輯用戶權限
    HandleEditUserRoleSave(user) {
      this.UpdateUser(user);
    },

    // 保存編輯密碼
    HandleEditPasswordSave(user) {
      this.UpdatePassword(user);
    },

    // 呼叫 新增用戶 API
    async CreateUser(user) {
      if (!user.userName || !user.pwd) {
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
      if (!this.pattern.test(user.userName) || !this.pattern.test(user.pwd)) {
        this.$message({
          message:
            "帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符",
          type: "error",
          duration: 1200,
        });
        return;
      }

      try {
        const response = await CreateAcc(user);
        if (response.data.code === 0) {
          this.$message({
            message: "新增成功",
            type: "success",
            duration: 1200,
          });
          this.FetchUsers();
          this.CloseAddUserModal();
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
    async UpdateUser(user) {
      try {
        const response = await EditAccRole(user);
        if (response.data.code === 0) {
          this.FetchUsers();
          this.CloseEditUserRoleModal();
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

    // 呼叫 更新密碼 API
    async UpdatePassword(user) {
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
          message: "密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符",
          type: "error",
          duration: 1200,
        });
        return;
      }
      try {
        const response = await EditAccPwd(user);
        if (response.data.code === 0) {
          this.FetchUsers();
          this.CloseEditPasswordModal();
          this.$message({
            message: "密碼更新成功",
            type: "success",
            duration: 1200,
          });
        } else {
          this.$message({
            message: "密碼更新失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "密碼更新失敗",
          type: "error",
          duration: 1200,
        });
      }
    },

    // 呼叫 刪除用戶 API
    async DeleteUser(id) {
      try {
        const response = await DeleteAcc({ id: id });
        if (response.data.code === 0) {
          this.FetchUsers();
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

    // 判斷是否能刪除用戶
    CanDeleteUser(item) {
      const itemPermission = item.permission;
      if (item.userName === this.currentUser) {
        return false;
      }
      if (itemPermission === 32767) {
        return false;
      }
      if ((this.role & 4) === 4) {
        return true;
      }
      return false;
    },

    // 獲取權限標籤
    GetPermissionLabels(permission) {
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
    this.FetchUsers();
  },
};
</script>

<style scoped>
.acc-search-bar {
  display: flex;
  justify-content: center;
}
</style>
