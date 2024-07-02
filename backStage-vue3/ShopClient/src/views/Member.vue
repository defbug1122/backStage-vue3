<template>
  <div class="member-container">
    <UserInfo />
    <SearchableList
      :searchTerm="searchTerm"
      @search="fetchMembers"
      :showSort="true"
      :sortOptions="sortOptions"
      :tableTitle="tableTitle"
      :tableData="members"
      :hasMore="hasMore"
      :pageNumber="pageNumber"
      :pageSize="pageSize"
      @prevPage="handlePrevPage"
      @nextPage="handleNextPage"
    >
      <template #table-rows="{ tableData }">
        <tr v-for="item in tableData" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.mn }}</td>
          <td>
            <div v-if="isEditing[item.id]">
              <el-select v-model="editedLevel[item.id]" placeholder="選擇等級">
                <el-option
                  v-for="level in levelMap"
                  :key="level.value"
                  :label="level.label"
                  :value="level.value"
                ></el-option>
              </el-select>
              <el-button @click="confirmEdit(item)">確定</el-button>
              <el-button @click="cancelEdit(item)">取消</el-button>
            </div>
            <div v-else>
              {{ getLevelName(item.level) }}
              <el-button
                v-if="(currentUser.permission & 32) === 32"
                @click="editLevel(item)"
                >編輯</el-button
              >
            </div>
          </td>
          <td>{{ item.totalSpent }}</td>
          <td>
            <el-switch
              v-model="item.status"
              :disabled="(currentUser.permission & 64) !== 64"
              @change="handleStatusChange(item)"
              active-text="啟用"
              inactive-text="停用"
            />
          </td>
        </tr>
      </template>
    </SearchableList>
  </div>
</template>

<script>
import {
  getMemberList,
  updateMemberStatus,
  updateMemberLevel,
} from "@/service/api";
import SearchableList from "@/components/SearchableList.vue";
import UserInfo from "@/components/UserInfo.vue";
import { store, mutations } from "@/store";

export default {
  name: "Member",
  components: {
    SearchableList,
    UserInfo,
  },
  data() {
    return {
      tableTitle: ["會員編號", "名稱", "等級", "消費金額", "狀態"],
      currentUser: {
        un: store.currentUser.un,
        permission: store.currentUser.role,
      },
      searchTerm: "",
      members: [],
      hasMore: false,
      pageNumber: 1,
      pageSize: 10,
      sortBy: 1,
      popoversVisible: {},
      sortOptions: [
        { label: "按會員編號排序", value: 1 },
        { label: "按等級排序", value: 2 },
      ],
      levelMap: [
        { label: "鑽石等級", value: 1 },
        { label: "白金等級", value: 2 },
        { label: "黃金等級", value: 3 },
        { label: "一般等級", value: 4 },
      ],
      isEditing: {},
      editedLevel: {},
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

    // 取得會員列表
    fetchMembers(
      searchTerm,
      sortBy = this.sortBy,
      pageNumber = this.pageNumber
    ) {
      getMemberList({
        searchTerm: searchTerm || this.searchTerm,
        sortBy: sortBy,
        pageNumber: pageNumber,
        pageSize: this.pageSize,
      })
        .then((response) => {
          this.members = response.data.data || [];
          this.hasMore = response.data.hasMore || false;
          this.pageNumber = pageNumber;
        })
        .catch((error) => {
          console.error("error", error);
          this.members = [];
          this.hasMore = false;
        });
    },

    // 上一頁功能
    handlePrevPage() {
      if (this.pageNumber > 1) {
        this.fetchMembers(this.searchTerm, this.sortBy, this.pageNumber - 1);
      }
    },

    // 下一頁功能
    handleNextPage() {
      if (this.hasMore) {
        this.fetchMembers(this.searchTerm, this.sortBy, this.pageNumber + 1);
      }
    },

    // 會員等級名稱處理
    getLevelName(level) {
      const foundLevel = this.levelMap.find((l) => l.value === level);
      return foundLevel ? foundLevel.label : "未知等級";
    },

    // 處理狀態變更
    handleStatusChange(member) {
      updateMemberStatus({
        memberId: member.id,
        status: member.status,
      })
        .then((response) => {
          if (response.data.code === 0) {
            this.$message({
              message: `會員狀態已更新為${member.status ? "啟用" : "停用"}`,
              type: "success",
              duration: 1200,
            });
          } else {
            this.$message({
              message: `會員狀態更新失敗`,
              type: "error",
              duration: 1200,
            });
          }
        })
        .catch((error) => {
          this.$message({
            message: "更新狀態失敗",
            type: "error",
            duration: 1200,
          });
          // 恢復原狀態
          member.status = !member.status;
        });
    },

    // 编辑等级
    editLevel(member) {
      this.$set(this.isEditing, member.id, true);
      this.$set(this.editedLevel, member.id, member.level);
    },

    // 取消编辑等级
    cancelEdit(member) {
      this.$set(this.isEditing, member.id, false);
      this.$delete(this.editedLevel, member.id);
    },
    // 確認编辑等级
    confirmEdit(member) {
      updateMemberLevel({
        memberId: member.id,
        level: this.editedLevel[member.id],
      })
        .then((response) => {
          if (response.data.code === 0) {
            this.$message({
              message: "等級更新成功",
              type: "success",
              duration: 1200,
            });
          }
          member.level = this.editedLevel[member.id];
          this.$set(this.isEditing, member.id, false);
          this.$delete(this.editedLevel, member.id);
        })
        .catch((error) => {
          this.$message({
            message: "等級更新失敗",
            type: "error",
            duration: 1200,
          });
        });
    },
  },
  created() {
    this.fetchMembers();
  },
};
</script>

<style scoped>
.member-info {
  display: flex;
  justify-content: end;
  align-items: center;
}
</style>
