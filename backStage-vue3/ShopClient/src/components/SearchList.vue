<template>
  <div class="search-list">
    <div class="search-bar">
      <el-input
        v-model="localSearchTerm"
        placeholder="請輸入欲查詢關鍵字"
      ></el-input>
      <el-select v-if="showSort" v-model="localSortBy" placeholder="排序方式">
        <el-option
          v-for="option in sortOptions"
          :key="option.value"
          :label="option.label"
          :value="option.value"
        ></el-option>
      </el-select>
      <el-button @click="HandleSearch">查詢</el-button>
      <el-button v-if="showAddButton" @click="$emit('add')">新增</el-button>
    </div>

    <table>
      <thead>
        <tr>
          <th v-for="(title, index) in tableTitle" :key="index">{{ title }}</th>
        </tr>
      </thead>
      <tbody>
        <slot name="table-rows" :tableData="tableData"></slot>
      </tbody>
    </table>
    <div class="pagination">
      <el-button
        @click="$emit('prevPage', localSearchTerm, localSortBy)"
        :disabled="pageNumber === 1"
      >
        上一頁
      </el-button>
      <span>第 {{ pageNumber }} 頁</span>
      <el-button
        @click="$emit('nextPage', localSearchTerm, localSortBy)"
        :disabled="!hasMore"
      >
        下一頁
      </el-button>
    </div>
  </div>
</template>

<script>
export default {
  name: "SearchList",
  props: {
    searchTerm: {
      type: String,
      default: "",
    },
    showSort: {
      type: Boolean,
      default: false,
    },
    sortOptions: {
      type: Array,
      default: () => [],
    },
    tableTitle: {
      type: Array,
      default: () => [],
    },
    tableData: {
      type: Array,
      default: () => [],
    },
    hasMore: {
      type: Boolean,
      default: false,
    },
    pageNumber: {
      type: Number,
      default: 1,
    },
    pageSize: {
      type: Number,
      default: 10,
    },
    showAddButton: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      localSearchTerm: this.searchTerm,
      localSortBy: this.sortOptions.length ? this.sortOptions[0].value : null,
    };
  },
  watch: {
    // 監聽查詢值
    searchTerm(newVal) {
      this.localSearchTerm = newVal;
    },
    // 監聽排序選項
    sortOptions(newVal) {
      this.localSortBy = newVal.length ? newVal[0].value : null;
    },
  },
  methods: {
    // 查詢功能
    HandleSearch() {
      if (this.localSearchTerm.length < 18) {
        this.$emit("search", this.localSearchTerm, 1, this.localSortBy);
      } else {
        this.$message({
          message: "輸入太長了",
          type: "error",
          duration: 1200,
        });
      }
    },
  },
};
</script>

<style scoped>
.search-list {
  padding: 0 20px 20px 20px;
}
.search-bar {
  display: flex;
  margin: 0 auto 20px auto;
}

.list {
  padding: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 10px;
  border: 1px solid #ddd;
}

tbody tr:nth-child(odd) {
  background-color: #f9f9f9;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 20px 0;
}

.pagination button {
  margin: 0 5px;
}

.el-button + .el-button {
  margin: 0;
}
</style>
