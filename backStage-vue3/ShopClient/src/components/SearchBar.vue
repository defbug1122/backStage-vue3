<template>
  <div class="search-bar">
    <el-input
      v-model="localSearchTerm"
      placeholder="請輸入欲查詢帳號"
    ></el-input>
    <el-button @click="handleSearch">查詢</el-button>
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
  },
  data() {
    return {
      localSearchTerm: this.searchTerm,
    };
  },
  watch: {
    searchTerm(newVal) {
      this.localSearchTerm = newVal;
    },
  },
  methods: {
    handleSearch() {
      this.$emit("search", this.localSearchTerm);
    },
  },
};
</script>

<style scoped>
.search-bar {
  display: flex;
  width: 300px;
}
</style>
