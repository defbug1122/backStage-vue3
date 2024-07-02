<template>
  <div class="list">
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
      <el-button @click="$emit('prevPage')" :disabled="pageNumber === 1">
        上一頁
      </el-button>
      <span>第 {{ pageNumber }} 頁</span>
      <el-button @click="$emit('nextPage')" :disabled="!hasMore">
        下一頁
      </el-button>
    </div>
  </div>
</template>

<script>
export default {
  name: "List",
  props: {
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
  },
};
</script>

<style scoped>
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
</style>
