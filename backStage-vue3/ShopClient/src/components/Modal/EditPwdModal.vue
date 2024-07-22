<template>
  <el-dialog
    title="編輯密碼"
    :visible.sync="$props.showModal"
    width="50%"
    top="6vh"
    :before-close="CloseModal"
    :close-on-click-modal="false"
    :show-close="false"
  >
    <div>
      <el-form :model="user">
        <el-form-item label="帳號">
          <el-input v-model="user.userName" readonly></el-input>
        </el-form-item>
        <el-form-item label="新密碼">
          <el-input v-model="user.pwd" type="password"></el-input>
        </el-form-item>
        <el-form-item label="確認新密碼">
          <el-input v-model="confirmPwd" type="password"></el-input>
        </el-form-item>
      </el-form>
      <div class="dialog-footer">
        <el-button @click="CloseModal">取消</el-button>
        <el-button type="primary" @click="SavePassword">保存</el-button>
      </div>
    </div>
  </el-dialog>
</template>

<script>
export default {
  name: "EditPwdModal",
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    user: {
      type: Object,
      default: () => ({
        id: "",
        userName: "",
        pwd: "",
      }),
    },
  },
  data() {
    return {
      confirmPwd: "",
    };
  },
  methods: {
    SavePassword() {
      if (!this.user.pwd || !this.confirmPwd) {
        this.$message.error("密碼或確認密碼不能為空。");
        return;
      }

      if (this.user.pwd !== this.confirmPwd) {
        this.$message.error("兩次密碼輸入不一樣，請重新輸入");
        return;
      }

      this.$emit("save", this.user);
    },
    CloseModal() {
      this.user.pwd = "";
      this.confirmPwd = "";
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
