<template>
  <div>
    <el-upload
      class="upload-demo"
      action="#"
      :on-success="HandleSuccess"
      :before-upload="BeforeUpload"
    >
      <el-button slot="trigger" size="small" type="primary">選擇圖片</el-button>
      <div slot="tip" class="el-upload__tip">
        只能上傳jpg/png文件，且不超過2MB
      </div>
    </el-upload>
    <!-- <el-upload
      class="upload-demo"
      action="#"
      list-type="picture-card"
      :auto-upload="false"
      :on-change="BeforeUpload"
      accept=".jpg, .png, .jpeg"
      :limit="1"
    >
      <el-button slot="trigger" size="small" type="primary">選擇圖片</el-button>
      <div slot="tip" class="el-upload__tip">
        只能上傳jpg/png文件，且不超過2MB
      </div>
    </el-upload> -->
  </div>
</template>

<script>
export default {
  name: "UploadImage",
  methods: {
    HandleSuccess(response, file, fileList) {
      this.$emit("upload-success", response);
    },
    BeforeUpload(file) {
      if (file.size / 1024 / 1024 > 2) {
        this.$message.error("上傳圖片大小不能超過 2MB!");
      }
      if (file.raw.type !== "image/jpeg" && file.raw.type !== "image/png") {
        this.$message.error("上傳圖片只能是 JPG 或 PNG 格式!");
      }
    },
  },
};
</script>
