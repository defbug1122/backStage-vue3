<template>
  <div class="login-container">
    <div class="login-title">
      <!-- <img src="/public/login-bg.jpg" alt="bg" /> -->
    </div>
    <div class="login-form">
      <h2>後臺管理系統</h2>
      <div class="form-item">
        <div>帳號</div>
        <input type="text" v-model="userName" /><br />
      </div>
      <div class="form-item">
        <div>密碼</div>
        <input type="password" v-model="pwd" /><br /><br />
      </div>
      <input type="submit" value="登入" @click="Login" class="login-button" />
      <span v-if="errorMsg !== ''" class="error-msg">{{ errorMsg }}</span>
    </div>
  </div>
</template>

<script>
import { Login } from "@/service/api";
import { mutations } from "@/store";

export default {
  name: "Login",
  data() {
    return {
      userName: "admin",
      pwd: "147147",
      errorMsg: "",
      pattern: /^[a-zA-Z0-9_-]{4,16}$/,
    };
  },
  methods: {
    GetCookieValue(cookieName) {
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

    // 呼叫登入API
    async Login() {
      if (this.userName === "" || this.pwd === "") {
        this.errorMsg = "請輸入帳號、密碼";
        return;
      } else {
        this.errorMsg = "";
      }

      if (!this.pattern.test(this.userName) || !this.pattern.test(this.pwd)) {
        this.errorMsg =
          "帳號或密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符";
        return;
      } else {
        this.errorMsg = "";
      }

      try {
        const response = await Login({
          userName: this.userName,
          pwd: this.pwd,
        });
        if (response.data.code === 0) {
          this.errorMsg = "";
          sessionStorage.setItem("id", this.GetCookieValue("userId"));
          sessionStorage.setItem("token", this.GetCookieValue("uuid"));
          sessionStorage.setItem("role", this.GetCookieValue("permission"));
          sessionStorage.setItem(
            "currentUser",
            this.GetCookieValue("currentUser")
          );
          const id = sessionStorage.getItem("id");
          const role = sessionStorage.getItem("role");
          const user = sessionStorage.getItem("currentUser");
          const token = sessionStorage.getItem("token");
          mutations.SetUserInfo({
            id: id,
            user: user,
            role: role,
            token: token,
          });
          this.$router.push("/");
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
  align-items: center;
  justify-content: center;
  background-color: #fffef2;
}

.login-title {
  background-color: #1a1915;
  height: 100%;
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-size: cover;
  background-repeat: no-repeat;
  background-image: url("/public/login-bg-2.jpg");
}

h2 {
  text-align: center;
}

.login-form {
  border-radius: 8px;
  width: 100%;
  max-width: 430px;
  text-align: left;
  padding: 15px 50px;
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
  background-color: #333333;
  color: #fff;
  font-size: 1rem;
  cursor: pointer;
}

.login-button:hover {
  background-color: #000000;
}

.error-msg {
  color: #ca432f;
  display: flex;
  justify-content: center;
  margin-top: 15px;
}
</style>
