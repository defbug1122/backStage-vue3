<template>
  <el-dialog
    :visible.sync="$props.showModal"
    width="100%"
    top="0vh"
    :before-close="CloseModal"
    :close-on-click-modal="false"
    class="order-modal"
  >
    <div v-if="order">
      <el-form :model="order">
        <div class="order-subtitle">
          <span>訂單資料</span>
          <div style="display: flex; align-items: center">
            <el-select
              v-model="selectedOrderStatus"
              placeholder="-- 更改訂單狀態 --"
            >
              <el-option
                v-for="status in orderStatusMap"
                :key="status.value"
                :label="status.label"
                :value="status.value"
              ></el-option>
            </el-select>
            <el-button
              type="primary"
              @click="UpdateOrderStatus"
              class="save-order-status-button"
              >更改</el-button
            >
          </div>
        </div>
        <div class="order-info">
          <el-form-item label="訂單號碼">
            <el-input v-model="order.orderNumber" readonly></el-input>
          </el-form-item>
          <el-form-item label="訂單日期">
            <el-input v-model="order.orderDate" readonly></el-input>
          </el-form-item>
          <el-form-item label="訂單狀態">
            <el-input
              :value="GetOrderStatus(order.orderStatus)"
              readonly
            ></el-input>
          </el-form-item>
          <el-form-item label="會員名稱">
            <el-input v-model="order.memberName" readonly></el-input>
          </el-form-item>
        </div>
        <div class="order-subtitle">
          <span>配送資料</span>
          <div v-if="!isEditable" style="display: flex; align-items: center">
            <el-select
              v-model="selectedDeliveryStatus"
              placeholder="-- 更改配送狀態 --"
            >
              <el-option
                v-for="status in deliveryStatusMap"
                :key="status.value"
                :label="status.label"
                :value="status.value"
              ></el-option>
            </el-select>
            <el-button
              type="primary"
              @click="UpdateDeliveryStatus"
              class="save-delivery-status-button"
              >更改</el-button
            >
            <el-button
              v-if="
                (order.deliveryStatus === 1 || order.deliveryStatus === 7) &&
                !isEditable
              "
              type="primary"
              @click="ToggleEdit"
              class="edit-button"
              >編輯</el-button
            >
          </div>
          <div v-else style="display: flex">
            <el-button
              v-if="isEditable"
              type="primary"
              @click="SaveDeliveryInfo(order)"
              class="save-button"
              >保存</el-button
            >
            <el-button
              v-if="isEditable"
              type="primary"
              @click="isEditable = false"
              class="save-button"
              >取消</el-button
            >
          </div>
        </div>
        <div class="order-info">
          <el-form-item label="配送狀態">
            <el-input
              :value="GetDeliveryStatus(order.deliveryStatus)"
              readonly
            ></el-input>
          </el-form-item>
          <el-form-item label="收件人">
            <el-input
              v-model="order.receiver"
              :readonly="!isEditable"
              maxlength="15"
            ></el-input>
          </el-form-item>
          <el-form-item label="聯絡手機">
            <el-input
              v-model="order.mobileNumber"
              :readonly="!isEditable"
              maxlength="10"
            ></el-input>
          </el-form-item>
          <el-form-item label="配送方式">
            <el-select
              v-model="order.deliveryMethod"
              placeholder="選擇配送方式"
              :disabled="!isEditable"
            >
              <el-option
                v-for="method in deliveryMethodsMap"
                :key="method.value"
                :label="method.label"
                :value="method.value"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="order.deliveryMethod === 1" label="配送地址">
            <el-input
              style="width: 450px"
              v-model="order.deliveryAddress"
              :readonly="!isEditable"
              maxlength="50"
            ></el-input>
          </el-form-item>
        </div>
        <div class="order-subtitle">訂購項目</div>
        <table class="order-details-table">
          <thead>
            <tr>
              <th>商品名稱</th>
              <th>數量</th>
              <th>單價</th>
              <th>小計</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in orderDetails" :key="index">
              <td>{{ item.productName }}</td>
              <td>{{ item.quantity }}</td>
              <td>{{ item.price }}</td>
              <td>{{ item.subtotal }}</td>
            </tr>
          </tbody>
        </table>
        <el-form-item class="order-total" label="訂單總金額">
          <span>{{ order.orderAmount }}</span>
        </el-form-item>
        <div class="order-subtitle">退貨資訊</div>
        <el-form-item>
          <el-input
            :value="GetReturnStatus(order.returnStatus)"
            readonly
          ></el-input>
        </el-form-item>
      </el-form>
    </div>
    <div slot="footer" class="dialog-footer">
      <el-button @click="CloseModal">關閉</el-button>
    </div>
  </el-dialog>
</template>

<script>
import {
  GetOrderDetails,
  UpdateDeliveryMethod,
  UpdateOrderStatus,
  UpdateDeliveryStatus,
} from "@/service/api";

export default {
  name: "OrderDetailModal",
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    orderId: {
      type: Number,
      required: false,
    },
  },
  data() {
    return {
      selectedOrderStatus: null,
      selectedDeliveryStatus: null,
      isEditable: false,
      order: null,
      orderDetails: [],
      orderStatusMap: [
        { value: 1, label: "處理中" },
        { value: 2, label: "已確認" },
        { value: 3, label: "已取消" },
        { value: 4, label: "訂單已完成" },
        { value: 5, label: "宅配-配送失敗" },
        { value: 6, label: "自取-未取貨" },
        { value: 7, label: "退貨處理中" },
        { value: 8, label: "退貨完成" },
      ],
      returnStatusMap: {
        1: "未申請退貨",
        2: "退貨申請中",
        3: "退貨成功",
      },
      deliveryMethodsMap: [
        { value: 1, label: "宅配" },
        { value: 2, label: "到店取貨" },
      ],
      deliveryStatusMap: [
        { value: 1, label: "備貨中" },
        { value: 2, label: "已發貨" },
        { value: 3, label: "已送達" },
        { value: 4, label: "配送失敗" },
        { value: 5, label: "回收退貨中" },
        { value: 6, label: "已收到退貨" },
        { value: 7, label: "店鋪準備商品中" },
        { value: 8, label: "待取貨" },
        { value: 9, label: "已取貨" },
        { value: 10, label: "未取貨" },
      ],
    };
  },
  watch: {
    showModal(val) {
      if (val && this.orderId) {
        this.FetchOrderDetails(this.orderId);
      }
    },
  },
  methods: {
    async FetchOrderDetails(orderId) {
      if (orderId == null) return;

      const response = await GetOrderDetails({ orderId });

      try {
        if (response.data.code === 0) {
          this.order = response.data.data;
          this.orderDetails = response.data.data.subtotalItems;
          this.selectedOrderStatus = null;
          this.selectedDeliveryStatus = null;
          this.isEditable = false;
        } else {
          this.$message({
            message: "資料獲取失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
      }
    },
    CloseModal() {
      this.$emit("close");
    },
    GetOrderStatus(status) {
      const statusOption = this.orderStatusMap.find(
        (option) => option.value === status
      );
      return statusOption ? statusOption.label : "未知狀態";
    },
    GetReturnStatus(status) {
      return this.returnStatusMap[status] || "未知狀態";
    },
    GetDeliveryStatus(status) {
      const statusOption = this.deliveryStatusMap.find(
        (option) => option.value === status
      );
      return statusOption ? statusOption.label : "未知狀態";
    },
    ToggleEdit() {
      this.isEditable = !this.isEditable;
    },
    async SaveDeliveryInfo(item) {
      const mobileRegex = /^09\d{8}$/;
      const addressRegex =
        /^[\u4e00-\u9fa5]{2,3}[縣市][\u4e00-\u9fa5]{2,3}[鄉鎮市區].+$/;

      if (this.orderId == null) return;

      if (item.deliveryMethod === 1 && !item.deliveryAddress) {
        this.$message({
          message: "請輸入收貨地址",
          type: "error",
          duration: 1200,
        });
        return;
      }

      if (!item.receiver) {
        this.$message({
          message: "請輸入收貨人",
          type: "error",
          duration: 1200,
        });
        return;
      }

      if (!item.mobileNumber) {
        this.$message({
          message: "請輸入手機聯絡電話",
          type: "error",
          duration: 1200,
        });
        return;
      }

      if (!mobileRegex.test(item.mobileNumber)) {
        this.$message({
          message: "手機聯絡電話格式不正確",
          type: "error",
          duration: 1200,
        });
        return;
      }

      if (!addressRegex.test(item.deliveryAddress)) {
        this.$message({
          message: "地址格式不正確，應有縣市、鄉鎮市區",
          type: "error",
          duration: 1200,
        });
        return;
      }

      try {
        const response = await UpdateDeliveryMethod({
          orderId: item.orderId,
          deliveryMethod: Number(item.deliveryMethod),
          deliveryAddress:
            item.deliveryMethod === 1 ? item.deliveryAddress : null,
          receiver: item.receiver,
          mobileNumber: item.mobileNumber,
        });

        if (response.data.code === 0) {
          this.$message({
            message: "配送信息修改成功",
            type: "success",
            duration: 1200,
          });
          this.isEditable = false;
          this.$emit("deliveryMethodUpdated");
        } else {
          this.$message({
            message: "修改失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "修改失敗",
          type: "error",
          duration: 1200,
        });
      }
    },
    async UpdateOrderStatus() {
      if (this.orderId == null || this.selectedOrderStatus == null) return;

      try {
        const response = await UpdateOrderStatus({
          orderId: this.orderId,
          orderStatus: this.selectedOrderStatus,
        });

        if (response.data.code === 0) {
          this.$message({
            message: "訂單狀態修改成功",
            type: "success",
            duration: 1200,
          });
          this.$emit("orderStatusUpdated");
          this.order.orderStatus = this.selectedOrderStatus; // 更新
        } else {
          this.$message({
            message: "修改失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "修改失敗",
          type: "error",
          duration: 1200,
        });
      }
    },
    async UpdateDeliveryStatus() {
      if (this.orderId == null || this.selectedDeliveryStatus == null) return;

      try {
        const response = await UpdateDeliveryStatus({
          orderId: this.orderId,
          deliveryStatus: this.selectedDeliveryStatus,
        });

        if (response.data.code === 0) {
          this.$message({
            message: "配送狀態修改成功",
            type: "success",
            duration: 1200,
          });
          this.$emit("deliveryStatusUpdated");
          this.order.deliveryStatus = this.selectedDeliveryStatus; // 更新
        } else {
          this.$message({
            message: "修改失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "修改失敗",
          type: "error",
          duration: 1200,
        });
      }
    },
  },
};
</script>

<style scoped>
.order-details-table {
  width: 100%;
  border-collapse: collapse;
  margin: 10px 0;
}

.order-details-table th,
.order-details-table td {
  border: 1px solid #ddd;
  padding: 8px;
}

.order-subtitle {
  margin-top: 10px;
  font-weight: bold;
  border-bottom: 1px #ccc solid;
  font-size: 16px;
  color: #448f8c;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.order-details-table th {
  background-color: #f2f2f2;
  text-align: left;
}

.el-dialog__wrapper {
  overflow: unset;
}

.order-modal {
  .el-dialog__body {
    padding-top: 20px;
  }

  .el-form-item {
    margin-bottom: 0;
  }

  .el-form-item__label {
    line-height: 25px;
  }

  .el-form-item__content {
    line-height: unset;
  }

  .order-total {
    display: flex;
    align-items: center;
    justify-content: flex-end;
  }

  .edit-button,
  .save-button {
    margin-left: 10px;
  }

  .order-info {
    display: flex;

    .el-form-item {
      margin-right: 20px;
    }
  }
}
</style>
