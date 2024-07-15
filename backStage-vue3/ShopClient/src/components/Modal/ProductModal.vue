<template>
  <el-dialog
    :title="isEditMode ? '編輯商品' : '新增商品'"
    :visible.sync="showModal"
    width="80%"
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
          <el-input maxlength="20" v-model="product.name"></el-input>
        </el-form-item>
        <el-form-item label="商品圖片1">
          <div class="upload-container">
            <div class="image-preview">
              <img
                @error="SetPlaceholder($event)"
                :src="GetImageUrl(product.imagePath1)"
                class="uploaded-image"
                v-if="product.imagePath1 && product.imagePath1 !== 'deleted'"
              />
              <el-button
                v-if="product.imagePath1 && product.imagePath1 !== 'deleted'"
                type="danger"
                icon="el-icon-delete"
                @click="RemoveImage(1)"
                circle
              ></el-button>
            </div>
            <label
              v-if="!product.imagePath1 || product.imagePath1 === 'deleted'"
              class="upload-label"
            >
              <input
                type="file"
                @change="(event) => HandleImageUpload(event, 1)"
                accept="image/*"
              />
              上傳圖片
            </label>
          </div>
        </el-form-item>
        <el-form-item label="商品圖片2">
          <div class="upload-container">
            <div class="image-preview">
              <img
                @error="SetPlaceholder($event)"
                :src="GetImageUrl(product.imagePath2)"
                class="uploaded-image"
                v-if="product.imagePath2 && product.imagePath2 !== 'deleted'"
              />
              <el-button
                v-if="product.imagePath2 && product.imagePath2 !== 'deleted'"
                type="danger"
                icon="el-icon-delete"
                @click="RemoveImage(2)"
                circle
              ></el-button>
            </div>
            <label
              v-if="!product.imagePath2 || product.imagePath2 === 'deleted'"
              class="upload-label"
            >
              <input
                type="file"
                @change="(event) => HandleImageUpload(event, 2)"
                accept="image/*"
              />
              上傳圖片
            </label>
          </div>
        </el-form-item>
        <el-form-item label="商品圖片3">
          <div class="upload-container">
            <div class="image-preview">
              <img
                @error="SetPlaceholder($event)"
                :src="GetImageUrl(product.imagePath3)"
                class="uploaded-image"
                v-if="product.imagePath3 && product.imagePath3 !== 'deleted'"
              />
              <el-button
                v-if="product.imagePath3 && product.imagePath3 !== 'deleted'"
                type="danger"
                icon="el-icon-delete"
                @click="RemoveImage(3)"
                circle
              ></el-button>
            </div>
            <label
              v-if="!product.imagePath3 || product.imagePath3 === 'deleted'"
              class="upload-label"
            >
              <input
                type="file"
                @change="(event) => HandleImageUpload(event, 3)"
                accept="image/*"
              />
              上傳圖片
            </label>
          </div>
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
          <el-input maxlength="50" v-model="product.describe"></el-input>
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
export default {
  name: "ProductModal",
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
        imagePath1: "",
        imagePath2: "",
        imagePath3: "",
        type: null,
        price: 10,
        active: false,
        describe: "",
        stock: 10,
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
      placeholderImage: "/public/placeholder.png",
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
    HandleImageUpload(event, index) {
      console.log("sdsddd", index);
      console.log("evvv", event);
      const file = event.target.files[0];
      console.log("file--", file);
      if (!file) return;

      const isJPG = file.type === "image/jpeg";
      const isPNG = file.type === "image/png";
      const isLt2M = file.size / 1024 / 1024 < 2;

      if (!isJPG && !isPNG) {
        this.$message.error("圖片只能是 JPG 或 PNG 格式!");
        return;
      }
      if (!isLt2M) {
        this.$message.error("圖片大小不能超過 2MB!");
        return;
      }

      const reader = new FileReader();
      reader.onload = (e) => {
        this.$set(this.product, `imagePath${index}`, {
          file,
          url: e.target.result,
        });
      };
      reader.readAsDataURL(file);

      event.target.value = null;
    },
    SetPlaceholder(event) {
      event.target.src = this.placeholderImage;
    },
    RemoveImage(index) {
      this.$set(this.product, `imagePath${index}`, "deleted");
    },
    async SaveProduct() {
      if (!this.product.name) {
        this.$message.error("商品名稱不能為空。");
        return;
      }

      if (this.product.name.length > 20) {
        this.$message.error("商品名稱不能大於20個字。");
        return;
      }

      if (!this.product.type) {
        this.$message.error("請選擇商品類型。");
        return;
      }

      if (this.product.price <= 0) {
        this.$message.error("商品價格應大於0。");
        return;
      }

      if (this.product.describe.length > 50) {
        this.$message.error("商品描述不能大於50個字。");
        return;
      }

      if (this.product.stock < 0) {
        this.$message.error("庫存量不能為負數。");
        return;
      }

      if (
        (!this.product.imagePath1 || this.product.imagePath1 === "deleted") &&
        !this.product.imagePath1.file
      ) {
        this.$message.error("請上傳至少一張圖片。");
        return;
      }

      const payload = {
        ...this.product,
        imagePath1:
          this.product.imagePath1 === "deleted"
            ? "deleted"
            : this.product.imagePath1.file
              ? await this.ToBase64(this.product.imagePath1.file)
              : "",
        imagePath2:
          this.product.imagePath2 === "deleted"
            ? "deleted"
            : this.product.imagePath2.file
              ? await this.ToBase64(this.product.imagePath2.file)
              : "",
        imagePath3:
          this.product.imagePath3 === "deleted"
            ? "deleted"
            : this.product.imagePath3.file
              ? await this.ToBase64(this.product.imagePath3.file)
              : "",
      };

      this.$emit("save", payload);
    },
    CloseModal() {
      this.$emit("close");
    },
    GetImageUrl(imagePath) {
      if (typeof imagePath === "string") {
        return imagePath.startsWith("data:")
          ? imagePath
          : `${import.meta.env.VITE_API_URL}/Uploads/${imagePath}`;
      }
      return imagePath.url;
    },
    async ToBase64(file) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = (error) => reject(error);
      });
    },
  },
};
</script>

<style scoped>
.upload-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.image-preview {
  position: relative;
}

.uploaded-image {
  width: 100px;
  height: 100px;
  object-fit: cover;
}

.upload-container input[type="file"] {
  display: block;
}

.upload-label {
  cursor: pointer;
  display: inline-block;
  padding: 10px 20px;
  background-color: #f5ad42;
  color: white;
  border-radius: 4px;
}

.upload-container button {
  position: absolute;
  top: 5px;
  right: 5px;
  color: white;
  border: none;
  border-radius: 50%;
  width: 25px;
  height: 25px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}
</style>
