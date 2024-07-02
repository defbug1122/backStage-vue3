<template>
  <el-dialog
    :title="isEditMode ? '修改用戶' : '新增用戶'"
    :visible.sync="showModal"
    width="50%"
    top="6vh"
    @close="closeModal"
    @closed="closeModal"
    :close-on-click-modal="false"
    :show-close="false"
  >
    <div>
      <el-form :model="user">
        <el-form-item label="帳號">
          <el-input v-model="user.un" :readonly="isEditMode"></el-input>
        </el-form-item>
        <el-form-item label="密碼">
          <el-input v-model="user.pwd" type="password"></el-input>
        </el-form-item>
        <el-form-item v-if="canEditPermission" label="權限">
          <el-select
            v-model="selectedPermissions"
            multiple
            @change="handlePermissionChange"
          >
            <el-option
              v-for="option in options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            ></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div class="dialog-footer">
        <el-button @click="closeModal">取消</el-button>
        <el-button type="primary" @click="saveUser">
          {{ isEditMode ? "保存" : "新增" }}
        </el-button>
      </div>
    </div>
  </el-dialog>
</template>

<script>
import { store } from "@/store";

export default {
  name: "UserModel",
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    isEditMode: {
      type: Boolean,
      default: false,
    },
    user: {
      type: Object,
      default: () => ({
        un: "",
        pwd: "",
        permission: "",
      }),
    },
  },
  data() {
    return {
      currentUserName: store.currentUser.un,
      currentUserRole: store.currentUser.role,
      options: [
        { label: "新增帳號", value: 1 },
        { label: "刪除帳號", value: 2 },
        { label: "修改帳號", value: 4 },
        { label: "查詢帳號", value: 8 },
        { label: "查詢會員", value: 16 },
        { label: "設定會員等級", value: 32 },
        { label: "會員停權設定", value: 64 },
        { label: "新增商品", value: 128 },
        { label: "查看商品", value: 256 },
        { label: "編輯商品", value: 512 },
        { label: "刪除商品", value: 1024 },
        { label: "查看訂單", value: 2048 },
        { label: "編輯訂單", value: 4096 },
        { label: "刪除訂單", value: 8192 },
      ],
      selectedPermissions: [],
    };
  },
  watch: {
    user: {
      handler(newVal) {
        this.selectedPermissions = this.parsePermissions(newVal.permission);
      },
      immediate: true,
      deep: true,
    },
  },
  created() {
    if (this.isEditMode) {
      this.selectedPermissions = this.parsePermissions(this.user.permission);
    }
  },
  computed: {
    canEditPermission() {
      console.log("this.currentUserRole", this.currentUserRole);
      // 如果當前用戶具有最高權限並且不是編輯自己的情況
      if (
        this.currentUserRole === 16383 &&
        this.user.un !== this.currentUserName
      ) {
        return true;
      }
      // 如果當前用戶具有編輯權限，並且不是編輯自己的情況，且目標用戶的權限不是最高權限
      else if (
        (this.currentUserRole & 4) === 4 &&
        this.user.un !== this.currentUserName &&
        this.user.permission !== 16383
      ) {
        return true;
      } else {
        return false;
      }
    },
  },
  methods: {
    parsePermissions(permission) {
      let permissions = [];
      this.options.forEach((option) => {
        if (permission & option.value) {
          permissions.push(option.value);
        }
      });
      return permissions;
    },
    calculatePermissionSum() {
      return this.selectedPermissions.reduce((sum, value) => sum + value, 0);
    },
    // 規則處理
    handlePermissionChange() {
      const permissionsSet = new Set(this.selectedPermissions);

      if (permissionsSet.has(1)) permissionsSet.add(8);
      if (permissionsSet.has(2)) {
        permissionsSet.add(1);
        permissionsSet.add(4);
        permissionsSet.add(8);
      }
      if (permissionsSet.has(4)) permissionsSet.add(8);
      if (permissionsSet.has(32)) permissionsSet.add(16);
      if (permissionsSet.has(64)) permissionsSet.add(16);
      if (permissionsSet.has(128)) permissionsSet.add(256);
      if (permissionsSet.has(1024)) {
        permissionsSet.add(128);
        permissionsSet.add(256);
        permissionsSet.add(512);
      }
      if (permissionsSet.has(512)) permissionsSet.add(256);
      if (permissionsSet.has(8192)) {
        permissionsSet.add(2048);
        permissionsSet.add(4096);
      }
      if (permissionsSet.has(4096)) permissionsSet.add(2048);

      this.selectedPermissions = Array.from(permissionsSet);
    },
    saveUser() {
      this.user.permission = this.calculatePermissionSum();
      if (
        this.currentUserRole === 16383 &&
        this.user.un === this.currentUserName
      ) {
        this.user.permission = store.currentUser.role;
      }
      this.$emit("save", this.user);
    },
    closeModal() {
      this.$emit("close");
    },
  },
};
</script>

<style scoped>
.el-select {
  width: 100%;
}
</style>
