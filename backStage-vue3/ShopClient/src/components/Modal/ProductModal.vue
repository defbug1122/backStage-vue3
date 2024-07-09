<template>
  <el-dialog
    :title="isEditMode ? '編輯商品' : '新增商品'"
    :visible.sync="showModal"
    width="50%"
    top="8vh"
    @close="CloseModal"
    @closed="CloseModal"
    :close-on-click-modal="false"
    :show-close="false"
    class="product-modal"
  >
    <div>
      <el-form :model="product">
        <el-form-item label="商品名稱">
          <el-input v-model="product.name"></el-input>
        </el-form-item>
        <el-form-item label="商品圖片">
          <UploadImage @upload-success="HandleUploadSuccess"></UploadImage>
        </el-form-item>
        <el-form-item label="商品類型">
          <el-select v-model="product.type">
            <el-option
              v-for="option in typeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="商品價格">
          <el-input v-model="product.price" type="number"></el-input>
        </el-form-item>
        <el-form-item label="是否開放">
          <el-checkbox v-model="product.active"></el-checkbox>
        </el-form-item>
        <el-form-item label="商品描述">
          <el-input v-model="product.describe"></el-input>
        </el-form-item>
        <el-form-item label="庫存量">
          <el-input v-model="product.stock" type="number"></el-input>
        </el-form-item>
      </el-form>
      <div class="dialog-footer">
        <el-button @click="CloseModal">取消</el-button>
        <el-button type="primary" @click="SaveProduct">{{
          isEditMode ? "保存" : "新增"
        }}</el-button>
      </div>
    </div>
  </el-dialog>
</template>

<script>
import UploadImage from "@/components/UploadImage.vue";

export default {
  name: "ProductModal",
  components: {
    UploadImage,
  },
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    isEditMode: {
      type: Boolean,
      default: false,
    },
    initialProduct: {
      type: Object,
      default: () => ({
        name: "",
        imagePath: "",
        type: null,
        price: 0,
        active: false,
        describe: "",
        stock: 0,
      }),
    },
  },
  data() {
    return {
      product: { ...this.initialProduct },
      typeOptions: [
        { label: "頭髮類", value: 1 },
        { label: "臉部類", value: 2 },
        { label: "身體類", value: 3 },
      ],
    };
  },
  watch: {
    initialProduct: {
      handler(newVal) {
        this.product = { ...newVal };
      },
      deep: true,
    },
  },
  methods: {
    HandleUploadSuccess(response) {
      this.product.imagePath = response.filePath;
    },
    SaveProduct() {
      if (!this.product.name) {
        this.$message.error("商品名稱不能為空。");
        return;
      }

      if (!this.product.name.length > 20) {
        this.$message.error("商品名稱不能大於20個字。");
        return;
      }

      // if (!this.product.imagePath) {
      //   this.$message.error("請上傳商品圖片。");
      //   return;
      // }

      if (!this.product.type) {
        this.$message.error("請選擇商品類型。");
        return;
      }

      if (this.product.price <= 0) {
        this.$message.error("商品價格應大於0。");
        return;
      }

      if (!this.product.describe.length > 50) {
        this.$message.error("商品描述不能大於50個字。");
        return;
      }

      if (this.product.stock < 0) {
        this.$message.error("庫存量不能為負數。");
        return;
      }

      this.$emit("save", this.product);
    },
    CloseModal() {
      this.$emit("close");
    },
  },
};
</script>

<style scoped>
.el-select {
  width: 100%;
}
.product-modal {
  .el-dialog__body {
    padding: 0px 20px 15px;
  }

  .el-dialog__header {
    padding: 15px 15px 15px;
  }

  .el-form-item__label {
    line-height: unset;
  }
}
</style>
