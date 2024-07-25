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
      :show-add-button="CanShowAddButton()"
      @search="FetchProducts"
      @prevPage="HandlePrevPage"
      @nextPage="HandleNextPage"
      @add="OpenAddModal"
    >
      <template v-slot:table-rows="{ tableData }">
        <tr v-for="(item, index) in tableData" :key="index">
          <td>{{ item.name }}</td>
          <td>
            <img
              :src="GetImageUrl(item.imagePath1)"
              alt="product image"
              width="100"
              class="product-img"
            />
          </td>
          <td>{{ GetTypeName(item.type) }}</td>
          <td>{{ item.describe }}</td>
          <td>{{ item.price }}</td>
          <td>{{ item.active ? "是" : "否" }}</td>
          <td class="stock">
            <i v-if="item.stock < safetyStock" class="el-icon-warning"> </i>
            <span class="tips">安全庫存量建議大於{{ safetyStock }}</span>
            {{ item.stock }}
          </td>
          <td>
            <el-button
              v-if="(currentUser.permission & 1024) === 1024"
              plain
              @click="OpenEditModal(item)"
            >
              編輯
            </el-button>
            <el-popover
              v-if="(currentUser.permission & 2048) === 2048"
              placement="top"
              width="160"
              trigger="click"
              :key="item.productId"
              v-model="popoversVisible[item.productId]"
            >
              <p>確認刪除此商品？</p>
              <div class="btn-group" style="text-align: right">
                <el-button
                  size="mini"
                  type="text"
                  @click="popoversVisible[item.productId] = false"
                  >取消</el-button
                >
                <el-button
                  type="primary"
                  size="mini"
                  @click="DeleteProduct(item.productId)"
                  >確認</el-button
                >
              </div>
              <el-button slot="reference" type="danger" plain>刪除</el-button>
            </el-popover>
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
import {
  GetProductList,
  CreateProduct,
  EditProduct,
  DeleteProduct,
} from "@/service/api";
import SearchList from "@/components/SearchList.vue";
import UserInfo from "@/components/UserInfo.vue";
import ProductModal from "@/components/Modal/ProductModal.vue";
import { store } from "@/store";

export default {
  components: {
    SearchList,
    ProductModal,
    UserInfo,
  },
  data() {
    return {
      currentUser: {
        permission: store.currentUser.role,
      },
      searchTerm: "",
      sortOptions: [
        { label: "按商品名稱排序", value: 1 },
        { label: "按商品類型排序", value: 2 },
      ],
      tableTitle: [
        "名稱",
        "圖片",
        "類型",
        "描述",
        "價格",
        "是否開放",
        "庫存量",
        "操作",
      ],
      popoversVisible: {},
      tableData: [],
      hasMore: false,
      safetyStock: 0,
      pageNumber: 1,
      pageSize: 10,
      showAddModal: false,
      isEditMode: false,
      currentProduct: {
        name: "",
        images: [],
        imagePath1: "",
        imagePath2: "",
        imagePath3: "",
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
      placeholderImage: "/public/placeholder.png",
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
          this.safetyStock = response.data.safetyStock;
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
    CanShowAddButton() {
      return (this.currentUser.permission & 256) === 256;
    },
    OpenAddModal() {
      this.isEditMode = false;
      this.currentProduct = {
        name: "",
        images: [],
        imagePath1: "",
        imagePath2: "",
        imagePath3: "",
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
      this.initialProduct = JSON.parse(JSON.stringify(product)); // 保存初始状态
      this.currentProduct = {
        ...product,
        images: [
          { file: null, url: product.imagePath1 },
          { file: null, url: product.imagePath2 },
          { file: null, url: product.imagePath3 },
        ].filter((image) => image.url),
      };
      this.showAddModal = true;
    },
    CloseAddModal() {
      this.showAddModal = false;
    },
    async SaveProduct(product) {
      // 比较 initialProduct 和 currentProduct
      const initialProduct = { ...this.initialProduct };
      const currentProduct = { ...product };
      delete currentProduct.images;
      const initialProductStr = JSON.stringify({
        ...initialProduct,
        imagePath1: initialProduct.imagePath1 === "" ? null : "",
        imagePath2: initialProduct.imagePath2 === "" ? null : "",
        imagePath3: initialProduct.imagePath3 === "" ? null : "",
      });
      const currentProductStr = JSON.stringify({
        ...currentProduct,
      });

      if (initialProductStr === currentProductStr) {
        this.$message({
          message: "沒有修改任何內容。",
          type: "info",
          duration: 1200,
        });
        this.showAddModal = false;
        return;
      }

      // 編輯商品
      if (this.isEditMode) {
        const response = await EditProduct({
          productId: product.productId,
          name: product.name.trim(),
          imagePath1: product.imagePath1,
          imagePath2: product.imagePath2,
          imagePath3: product.imagePath3,
          price: Number(product.price),
          type: product.type,
          describe: product.describe.trim(),
          stock: Number(product.stock),
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
              message: "編輯商品成功",
              type: "success",
              duration: 1200,
            });
          } else {
            this.showAddModal = false;
            this.FetchProducts(
              this.searchTerm,
              this.pageNumber,
              this.sortOptions[0].value
            );
            this.$message({
              message: "編輯商品失敗，請重新再試一次",
              type: "error",
              duration: 1200,
            });
          }
        } catch (error) {
          console.error("error", error);
          this.$message({
            message: "編輯商品失敗，請重新再試一次",
            type: "error",
            duration: 1200,
          });
        }
      } else {
        // 新增商品
        const response = await CreateProduct({
          name: product.name.trim(),
          imagePath1: product.imagePath1,
          imagePath2: product.imagePath2,
          imagePath3: product.imagePath3,
          price: Number(product.price),
          type: product.type,
          describe: product.describe.trim(),
          stock: Number(product.stock),
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
          } else {
            this.showAddModal = false;
            this.FetchProducts(
              this.searchTerm,
              this.pageNumber,
              this.sortOptions[0].value
            );
            this.$message({
              message: "新增商品失敗，請重新再試一次",
              type: "error",
              duration: 1200,
            });
          }
        } catch (error) {
          console.error("error", error);
          this.$message({
            message: "新增商品失敗，請重新再試一次",
            type: "error",
            duration: 1200,
          });
        }
      }
    },
    GetImageUrl(relativePath) {
      return `${import.meta.env.VITE_API_URL}/Uploads/${relativePath}`;
    },
    SetPlaceholder(event) {
      event.target.src = this.placeholderImage;
    },
    async DeleteProduct(id) {
      try {
        const response = await DeleteProduct({ productId: id });
        if (response.data.code === 0) {
          this.FetchProducts(
            this.searchTerm,
            this.pageNumber,
            this.sortOptions[0].value
          );
          this.$message({
            message: "商品刪除成功",
            type: "success",
            duration: 2000,
          });
        } else {
          this.$message({
            message: "商品刪除失敗",
            type: "error",
            duration: 2000,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "商品刪除失敗",
          type: "error",
          duration: 2000,
        });
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

<style scoped>
.el-icon-warning {
  color: #ca432f;
  position: relative;
  cursor: pointer;
}

.tips {
  display: none;
}

.el-icon-warning:hover + .tips {
  display: block;
  position: absolute;
  top: -30px;
  left: 0;
  padding: 12px 20px;
  font-size: 14px;
  border-radius: 4px;
  background-color: #fff;
  filter: drop-shadow(0px 0px 10px #b2b2b2);
  color: #ca432f;
  width: 70px;
}

.tip::after {
  content: "";
  position: absolute;
  border: 20px solid;
  border-color: #fff transparent transparent;
  bottom: -40px;
  left: calc(50% - 20px);
}

.stock {
  position: relative;
}

.product-img {
  object-fit: contain;
  object-position: center;
  background: #000;
  width: 100px;
  height: 100px;
}
</style>
