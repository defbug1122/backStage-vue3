<template>
  <div class="member-container">
    <UserInfo />
    <SearchList
      :searchTerm="searchTerm"
      @search="FetchMembers"
      :showSort="true"
      :sortOptions="sortOptions"
      :sortBy="sortBy"
      :tableTitle="tableTitle"
      :tableData="members"
      :hasMore="hasMore"
      :pageNumber="pageNumber"
      :pageSize="pageSize"
      @prevPage="HandlePrevPage"
      @nextPage="HandleNextPage"
    >
      <template #table-rows="{ tableData }">
        <tr v-for="item in tableData" :key="item.memberId">
          <td>{{ item.memberId }}</td>
          <td>{{ item.memberName }}</td>
          <td>
            <div v-if="isEditing[item.memberId]">
              <el-select
                v-model="editedLevel[item.memberId]"
                placeholder="選擇等級"
              >
                <el-option
                  v-for="level in levelMap"
                  :key="level.value"
                  :label="level.label"
                  :value="level.value"
                ></el-option>
              </el-select>
              <el-button @click="ConfirmEdit(item)">確定</el-button>
              <el-button @click="CancelEdit(item)">取消</el-button>
            </div>
            <div v-else>
              {{ GetLevelName(item.level) }}
              <el-button
                v-if="(currentUser.permission & 64) === 64"
                @click="EditLevel(item)"
                >編輯</el-button
              >
            </div>
          </td>
          <td>{{ item.totalSpent }}</td>
          <td>
            <el-switch
              v-model="item.status"
              :disabled="(currentUser.permission & 128) !== 128"
              @change="HandleStatusChange(item)"
              active-text="啟用"
              inactive-text="停用"
            />
          </td>
        </tr>
      </template>
    </SearchList>
  </div>
</template>

<script>
import {
  GetMemberList,
  UpdateMemberStatus,
  UpdateMemberLevel,
} from "@/service/api";
import SearchList from "@/components/SearchList.vue";
import UserInfo from "@/components/UserInfo.vue";
import { store, mutations } from "@/store";

export default {
  name: "Member",
  components: {
    SearchList,
    UserInfo,
  },
  data() {
    return {
      tableTitle: ["會員編號", "名稱", "等級", "消費金額", "狀態"],
      currentUser: {
        user: store.currentUser.user,
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
        { label: "一般等級", value: 1 },
        { label: "黃金等級", value: 2 },
        { label: "白金等級", value: 3 },
        { label: "鑽石等級", value: 4 },
      ],
      isEditing: {},
      editedLevel: {},
    };
  },
  methods: {
    // 取得會員列表
    async FetchMembers(
      searchTerm,
      pageNumber = this.pageNumber,
      sortBy = this.sortBy
    ) {
      const response = await GetMemberList({
        searchTerm: searchTerm || this.searchTerm,
        pageNumber: pageNumber,
        pageSize: this.pageSize,
        sortBy: sortBy,
      });
      try {
        if (response.data.code === 0) {
          this.members = response.data.data || [];
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
        this.members = [];
        this.hasMore = false;
      }
    },

    // 上一頁功能
    HandlePrevPage(searchTerm, sortBy) {
      if (this.pageNumber > 1) {
        this.FetchMembers(searchTerm, this.pageNumber - 1, sortBy);
      }
    },

    // 下一頁功能
    HandleNextPage(searchTerm, sortBy) {
      if (this.hasMore) {
        this.FetchMembers(searchTerm, this.pageNumber + 1, sortBy);
      }
    },

    // 會員等級名稱處理
    GetLevelName(level) {
      const foundLevel = this.levelMap.find((v) => v.value === level);
      return foundLevel ? foundLevel.label : "未知等級";
    },

    // 處理狀態變更
    HandleStatusChange(member) {
      UpdateMemberStatus({
        memberId: member.memberId,
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
    EditLevel(member) {
      this.$set(this.isEditing, member.memberId, true);
      this.$set(this.editedLevel, member.memberId, member.level);
    },

    // 取消编辑等级
    CancelEdit(member) {
      this.$set(this.isEditing, member.memberId, false);
      this.$delete(this.editedLevel, member.memberId);
    },
    // 確認编辑等级
    ConfirmEdit(member) {
      UpdateMemberLevel({
        memberId: member.memberId,
        level: this.editedLevel[member.memberId],
      })
        .then((response) => {
          if (response.data.code === 0) {
            this.$message({
              message: "等級更新成功",
              type: "success",
              duration: 1200,
            });
          }
          member.level = this.editedLevel[member.memberId];
          this.$set(this.isEditing, member.memberId, false);
          this.$delete(this.editedLevel, member.memberId);
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
    this.FetchMembers();
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
