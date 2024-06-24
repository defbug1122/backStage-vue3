<template>
  <div class="login-container">
    <h1>後臺管理系統</h1>
    <div class="login-form">
      <div class="form-item">
        <div>帳號</div>
        <input type="text" v-model="un" /><br />
      </div>
      <div class="form-item">
        <div>密碼</div>
        <input type="password" v-model="pwd" /><br /><br />
      </div>
      <input type="submit" value="登入" @click="login" class="login-button" />
    </div>
    <span v-if="errorMsg !== ''" class="error-msg">{{ errorMsg }}</span>
  </div>
</template>

<script>
import { login } from "@/service/api";
import { mutations } from "@/store";

export default {
  name: "Login",
  data() {
    return {
      un: "admin",
      pwd: "147147",
      errorMsg: "",
      pattern: /^[a-zA-Z0-9_-]{4,16}$/,
    };
  },
  methods: {
    getCookieValue(cookieName) {
      var name = cookieName + "=";
      var decodedCookie = decodeURIComponent(document.cookie);
      var cookieArray = decodedCookie.split(";");
      for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i].trim();
        if (cookie.indexOf(name) == 0) {
          return cookie.substring(name.length, cookie.length);
        }
      }
      return "";
    },
    async login() {
      if (this.un === "" || this.pwd === "") {
        this.errorMsg = "請輸入帳號、密碼";
        return;
      } else {
        this.errorMsg = "";
      }

      if (!this.pattern.test(this.un) || !this.pattern.test(this.pwd)) {
        this.errorMsg =
          "帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符";
        return;
      } else {
        this.errorMsg = "";
      }

      try {
        const response = await login({ un: this.un, pwd: this.pwd });
        if (response.data.code === 0) {
          this.errorMsg = "";
          sessionStorage.setItem("token", this.getCookieValue("uuid"));
          sessionStorage.setItem("role", this.getCookieValue("permission"));
          sessionStorage.setItem(
            "currentUser",
            this.getCookieValue("currentUser")
          );
          const role = sessionStorage.getItem("role");
          const user = sessionStorage.getItem("currentUser");
          const token = sessionStorage.getItem("token");
          mutations.setUserInfo({
            user: user,
            role: role,
            token: token,
          });
          if (role === "1" || role === "2") {
            this.$router.push("/account");
          } else if (role === "3" || role === "4") {
            this.$router.push("/member");
          } else if (role === "5" || role === "6") {
            this.$router.push("/product");
          } else if (role === "7" || role === "8") {
            this.$router.push("/order");
          }
          this.$message({
            message: "登入成功",
            type: "success",
            duration: 1500,
          });
        } else {
          this.errorMsg = "請輸入正確帳號、密碼";
        }
      } catch (error) {
        console.error("登入失敗:", error);
        this.errorMsg = "登入失敗，請輸入正確帳號、密碼";
      }
    },
  },
};
</script>

<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.login-form {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 380px;
  text-align: left;
  padding: 15px 30px;
}

.form-item {
  margin-bottom: 10px;
}

input {
  width: 95%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.login-button {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 4px;
  background-color: #007bff;
  color: #fff;
  font-size: 1rem;
  cursor: pointer;
}

.login-button:hover {
  background-color: #0056b3;
}

.error-msg {
  color: red;
  margin-top: 1rem;
}
</style>
