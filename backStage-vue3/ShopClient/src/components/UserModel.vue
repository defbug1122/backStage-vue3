<template>
  <el-dialog
    :title="isEditMode ? '修改用戶' : '新增用戶'"
    :visible.sync="showModal"
    width="30%"
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
          <el-select v-model="user.permission">
            <el-option
              v-for="option in filteredOptions"
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
        { label: "管理者", value: "2" },
        { label: "會員系統編輯者", value: "3" },
        { label: "會員系統查看者", value: "4" },
        { label: "商品系統編輯者", value: "5" },
        { label: "商品系統查看者", value: "6" },
        { label: "訂單系統編輯者", value: "7" },
        { label: "訂單系統查看者", value: "8" },
      ],
    };
  },
  computed: {
    canEditPermission() {
      // 超級管理員可以更改所有用戶的權限
      if (
        this.currentUserRole === "1" &&
        this.user.un !== this.currentUserName
      ) {
        return true;
      }
      // 管理員不能更改其他管理員的權限，但可以更改其他用戶的權限
      if (
        this.currentUserRole === "2" &&
        this.user.permission !== "1" &&
        this.user.permission !== "2"
      ) {
        return true;
      }
      return false;
    },
    filteredOptions() {
      if (this.currentUserRole === "1") {
        // 超級管理員可以看到所有選項
        return this.options;
      } else {
        // 管理員不能看到管理者選項
        return this.options.filter((option) => option.value !== "2");
      }
    },
  },
  methods: {
    saveUser() {
      this.$emit("save", this.user);
    },
    closeModal() {
      this.$emit("close");
    },
  },
};
</script>

<style scoped></style>
