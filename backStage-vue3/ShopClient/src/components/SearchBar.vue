<template>
  <div class="search-bar">
    <el-input
      v-model="localSearchTerm"
      placeholder="請輸入欲查詢關鍵字"
    ></el-input>
    <el-button @click="handleSearch">查詢</el-button>
    <el-select v-if="showSort" v-model="localSortBy" placeholder="排序方式">
      <el-option
        v-for="option in sortOptions"
        :key="option.value"
        :label="option.label"
        :value="option.value"
      ></el-option>
    </el-select>
  </div>
</template>

<script>
export default {
  name: "SearchBar",
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
  },
  data() {
    return {
      localSearchTerm: this.searchTerm,
      localSortBy: this.sortOptions.length ? this.sortOptions[0].value : null,
    };
  },
  watch: {
    searchTerm(newVal) {
      this.localSearchTerm = newVal;
    },
  },
  methods: {
    handleSearch() {
      this.$emit("search", this.localSearchTerm, this.localSortBy);
    },
  },
};
</script>

<style scoped>
.search-bar {
  display: flex;
  width: 500px;
}
</style>
