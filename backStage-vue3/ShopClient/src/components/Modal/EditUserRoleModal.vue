<template>
  <el-dialog
    title="編輯用戶權限"
    :visible.sync="$props.showModal"
    width="50%"
    top="6vh"
    :before-close="CloseModal"
    :close-on-click-modal="false"
    class="edit-role-model"
  >
    <div>
      <el-form :model="user">
        <el-form-item label="帳號">
          <el-input v-model="user.userName" readonly></el-input>
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
        <el-button type="primary" @click="SaveUser">保存</el-button>
      </div>
    </div>
  </el-dialog>
</template>

<script>
export default {
  name: "EditUserRoleModal",
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    user: {
      type: Object,
      default: () => ({
        userName: "",
        permission: 0,
      }),
    },
  },
  data() {
    return {
      currentUserName: sessionStorage.getItem("currentUser"),
      currentUserRole: Number(sessionStorage.getItem("role")),
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
    };
  },
  watch: {
    user: {
      handler(newVal) {
        this.selectedPermissions = this.ParsePermissions(newVal.permission);
      },
      immediate: true,
      deep: true,
    },
  },
  methods: {
    IsOptionDisabled(option) {
      if (
        this.currentUserRole === 32767 &&
        this.user.userName === this.currentUserName
      ) {
        // 當前用戶是超級管理員，並且正在編輯自己的權限時，禁用所有選項
        return true;
      } else if (
        this.currentUserRole !== 32767 &&
        this.user.userName === this.currentUserName
      ) {
        // 當前用戶不是超級管理員，且正在編輯自己的權限時，禁用所有選項
        return true;
      } else if (
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
    ParsePermissions(permission) {
      let permissions = [];
      this.options.forEach((option) => {
        if ((permission & option.value) === option.value) {
          permissions.push(option.value);
        }
      });
      return permissions;
    },
    CalculatePermissionSum() {
      return this.selectedPermissions.reduce((sum, value) => sum + value, 0);
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
    SaveUser() {
      const newPermission = this.CalculatePermissionSum();

      if (newPermission === 0 || newPermission > 32766) {
        this.$message.error("權限不能為空且必須小於等於 32766。");
        return;
      }

      this.user.permission = newPermission;
      this.$emit("save", this.user);
    },
    CloseModal() {
      this.$emit("close");
    },
  },
};
</script>

<style scoped>
.el-select {
  width: 100%;
}
.edit-role-model {
  .el-tag .el-tag__close {
    display: none !important;
  }
}
</style>
