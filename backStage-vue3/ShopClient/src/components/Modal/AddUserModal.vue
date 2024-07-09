<template>
  <el-dialog
    title="新增用戶"
    :visible.sync="showModal"
    width="50%"
    top="8vh"
    @close="CloseModal"
    @closed="CloseModal"
    :close-on-click-modal="false"
    :show-close="false"
    class="add-user-model"
  >
    <div>
      <el-form :model="user">
        <el-form-item label="帳號">
          <el-input v-model="user.userName"></el-input>
        </el-form-item>
        <el-form-item label="密碼">
          <el-input v-model="user.pwd" type="password"></el-input>
        </el-form-item>
        <el-form-item label="確認密碼">
          <el-input v-model="confirmPwd" type="password"></el-input>
        </el-form-item>
        <el-form-item label="權限">
          <el-select
            v-model="selectedPermissions"
            multiple
            @change="HandlePermissionChange"
          >
            <el-option
              v-for="option in options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
              :disabled="IsOptionDisabled(option)"
            ></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div class="dialog-footer">
        <el-button @click="CloseModal">取消</el-button>
        <el-button type="primary" @click="SaveUser">新增</el-button>
      </div>
    </div>
  </el-dialog>
</template>

<script>
export default {
  name: "AddUserModal",
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      user: {
        userName: "",
        pwd: "",
        permission: 0,
      },
      confirmPwd: "",
      options: [
        { label: "新增帳號", value: 2 },
        { label: "刪除帳號", value: 4 },
        { label: "修改帳號", value: 8 },
        { label: "查詢帳號", value: 16 },
        { label: "查詢會員", value: 32 },
        { label: "設定會員等級", value: 64 },
        { label: "會員停權設定", value: 128 },
        { label: "新增商品", value: 256 },
        { label: "查看商品", value: 512 },
        { label: "編輯商品", value: 1024 },
        { label: "刪除商品", value: 2048 },
        { label: "查看訂單", value: 4096 },
        { label: "編輯訂單", value: 8192 },
        { label: "刪除訂單", value: 16384 },
      ],
      selectedPermissions: [],
      currentUserName: sessionStorage.getItem("currentUser"),
      currentUserRole: Number(sessionStorage.getItem("role")),
    };
  },
  methods: {
    IsOptionDisabled(option) {
      if (
        this.currentUserRole !== 32767 &&
        [2, 4, 8, 32767].includes(option.value)
      ) {
        // 當前用戶不是超級管理員，禁用新增帳號、刪除帳號、修改帳號和超級管理員選項
        return true;
      } else if (
        this.currentUserRole !== 32767 &&
        this.user.permission === 32767
      ) {
        // 當前用戶不是超級管理員，且正在編輯的用戶是超級管理員，禁用所有選項
        return true;
      }
      return false;
    },
    HandlePermissionChange() {
      const permissionsSet = new Set(this.selectedPermissions);
      if (permissionsSet.has(2)) permissionsSet.add(16);
      if (permissionsSet.has(4)) permissionsSet.add(16);
      if (permissionsSet.has(8)) permissionsSet.add(16);
      if (permissionsSet.has(64)) permissionsSet.add(32);
      if (permissionsSet.has(128)) permissionsSet.add(32);
      if (permissionsSet.has(256)) permissionsSet.add(512);
      if (permissionsSet.has(2048)) {
        permissionsSet.add(256);
        permissionsSet.add(512);
        permissionsSet.add(1024);
      }
      if (permissionsSet.has(1024)) permissionsSet.add(512);
      if (permissionsSet.has(16384)) {
        permissionsSet.add(4096);
        permissionsSet.add(8192);
      }
      if (permissionsSet.has(8192)) permissionsSet.add(4096);

      this.selectedPermissions = Array.from(permissionsSet);
    },
    CalculatePermissionSum() {
      return this.selectedPermissions.reduce((sum, value) => sum + value, 0);
    },
    SaveUser() {
      const newPermission = this.CalculatePermissionSum();

      if (!this.user.userName) {
        this.$message.error("帳號輸入不能為空。");
        return;
      }

      if (!this.user.pwd || !this.confirmPwd) {
        this.$message.error("密碼或確認密碼不能為空。");
        return;
      }

      if (this.user.pwd !== this.confirmPwd) {
        this.$message.error("兩次密碼輸入不一樣，請重新輸入");
        return;
      }

      if (newPermission === 0 || newPermission > 32766) {
        this.$message.error("權限不能為空。");
        return;
      }

      this.user.permission = newPermission;
      this.$emit("save", this.user);
    },
    CloseModal() {
      this.selectedPermissions = [];
      this.user = {};
      this.$emit("close");
    },
  },
};
</script>

<style scoped>
.el-select {
  width: 100%;
}
.add-user-model {
  .el-tag .el-tag__close {
    display: none !important;
  }

  .el-dialog__body {
    padding: 0px 20px 15px;
  }

  .el-dialog__header {
    padding: 15px 15px 15px;
  }

  .el-form-item__label {
    line-height: unset;
  }
}
</style>
