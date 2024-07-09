<template>
  <div>
    <UserInfo />
    <SearchList
      :search-term="searchTerm"
      :show-sort="true"
      :sort-options="sortOptions"
      :table-title="tableTitle"
      :table-data="tableData"
      :has-more="hasMore"
      :page-number="pageNumber"
      :show-add-button="true"
      @search="FetchProducts"
      @prevPage="HandlePrevPage"
      @nextPage="HandleNextPage"
      @add="OpenAddModal"
    >
      <template v-slot:table-rows="{ tableData }">
        <tr v-for="(item, index) in tableData" :key="index">
          <td>{{ item.name }}</td>
          <td><img :src="item.imagePath" alt="product image" width="100" /></td>
          <td>{{ GetTypeName(item.type) }}</td>
          <td>{{ item.price }}</td>
          <td>{{ item.active ? "是" : "否" }}</td>
          <td>{{ item.describe }}</td>
          <td>{{ item.stock }}</td>
          <td>
            <el-button type="text" @click="OpenEditModal(item)">編輯</el-button>
          </td>
        </tr>
      </template>
    </SearchList>
    <ProductModal
      :showModal="showAddModal"
      :isEditMode="isEditMode"
      :initialProduct="currentProduct"
      @save="SaveProduct"
      @close="CloseAddModal"
    ></ProductModal>
  </div>
</template>

<script>
import { GetProductList, CreateProduct } from "@/service/api";
import SearchList from "@/components/SearchList.vue";
import UserInfo from "@/components/UserInfo.vue";
import ProductModal from "@/components/Modal/ProductModal.vue";

export default {
  components: {
    SearchList,
    ProductModal,
    UserInfo,
  },
  data() {
    return {
      searchTerm: "",
      sortOptions: [
        { label: "按商品名稱排序", value: 1 },
        { label: "按商品類型排序", value: 2 },
      ],
      tableTitle: [
        "名稱",
        "圖片",
        "類型",
        "價格",
        "是否開放",
        "描述",
        "庫存量",
        "操作",
      ],
      tableData: [],
      hasMore: false,
      pageNumber: 1,
      showAddModal: false,
      isEditMode: false,
      currentProduct: {
        name: "",
        imagePath: "",
        type: null,
        price: 0,
        active: false,
        describe: "",
        stock: 0,
      },
      typeMap: [
        { label: "頭髮類", value: 1 },
        { label: "臉部類", value: 2 },
        { label: "身體類", value: 3 },
      ],
    };
  },
  methods: {
    async FetchProducts(
      searchTerm,
      pageNumber = this.pageNumber,
      sortBy = this.sortOptions[0].value
    ) {
      const response = await GetProductList({
        searchTerm: searchTerm || this.searchTerm,
        pageNumber: pageNumber,
        pageSize: this.pageSize,
        sortBy: sortBy,
      });
      try {
        if (response.data.code === 0) {
          this.tableData = response.data.data || [];
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
        this.tableData = [];
        this.hasMore = false;
      }
    },

    HandlePrevPage(searchTerm, sortBy) {
      if (this.pageNumber > 1) {
        this.pageNumber -= 1;
        this.FetchProducts(searchTerm, this.pageNumber, sortBy);
      }
    },

    HandleNextPage(searchTerm, sortBy) {
      if (this.hasMore) {
        this.pageNumber += 1;
        this.FetchProducts(searchTerm, this.pageNumber, sortBy);
      }
    },

    // 產品類型名稱處理
    GetTypeName(type) {
      const foundType = this.typeMap.find((v) => v.value === type);
      return foundType ? foundType.label : "未知種類";
    },

    OpenAddModal() {
      this.isEditMode = false;
      this.currentProduct = {
        name: "",
        imagePath: "",
        type: null,
        price: 0,
        active: false,
        describe: "",
        stock: 0,
      };
      this.showAddModal = true;
    },
    OpenEditModal(product) {
      this.isEditMode = true;
      this.currentProduct = { ...product };
      this.showAddModal = true;
    },
    CloseAddModal() {
      this.showAddModal = false;
    },
    async SaveProduct(product) {
      // 編輯商品
      if (this.isEditMode) {
        axios
          .put(`/api/product/${product.productId}`, product)
          .then((response) => {
            this.showAddModal = false;
            this.FetchProducts(
              this.searchTerm,
              this.pageNumber,
              this.sortOptions[0].value
            );
          })
          .catch((error) => {
            console.error(error);
          });
      } else {
        console.log("pro----", product);
        // 新增商品
        const response = await CreateProduct({
          name: product.name,
          imageFile: "",
          price: product.price,
          type: product.type,
          describe: product.describe,
          stock: product.stock,
          active: product.active,
        });
        try {
          if (response.data.code === 0) {
            this.showAddModal = false;
            this.FetchProducts(
              this.searchTerm,
              this.pageNumber,
              this.sortOptions[0].value
            );
            this.$message({
              message: "新增商品成功",
              type: "success",
              duration: 1200,
            });
          }
        } catch (error) {
          console.error("error", error);
          this.$message({
            message: "新增商品失敗",
            type: "error",
            duration: 1200,
          });
        }
      }
    },
  },
  created() {
    this.FetchProducts(
      this.searchTerm,
      this.pageNumber,
      this.sortOptions[0].value
    );
  },
};
</script>
