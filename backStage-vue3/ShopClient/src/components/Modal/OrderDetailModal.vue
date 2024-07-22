<template>
  <el-dialog
    title="訂單明細"
    :visible.sync="$props.showModal"
    width="70%"
    top="1vh"
    :before-close="CloseModal"
    :close-on-click-modal="false"
    class="order-modal"
  >
    <div v-if="order">
      <el-form :model="order">
        <el-form-item label="訂單號碼">
          <el-input v-model="order.orderNumber" readonly></el-input>
        </el-form-item>
        <el-form-item label="訂單日期">
          <el-input v-model="order.orderDate" readonly></el-input>
        </el-form-item>
        <el-form-item label="會員名稱">
          <el-input v-model="order.memberName" readonly></el-input>
        </el-form-item>
        <el-form-item label="收件人">
          <el-input v-model="order.receiver" readonly></el-input>
        </el-form-item>
        <el-form-item label="配送方式">
          <el-select v-model="order.deliveryMethod" placeholder="選擇配送方式">
            <el-option
              v-for="method in deliveryMethodsMap"
              :key="method.value"
              :label="method.label"
              :value="method.value"
            ></el-option>
          </el-select>
          <el-button
            type="primary"
            @click="updateDeliveryMethod(order)"
            class="save-delivery-method-button"
            >保存配送方式</el-button
          >
        </el-form-item>
        <el-form-item label="配送地址">
          <el-input v-model="order.deliveryAddress"></el-input>
        </el-form-item>
        <el-form-item label="配送狀態">
          <el-input
            :value="GetDeliveryStatus(order.deliveryStatus)"
            readonly
          ></el-input>
        </el-form-item>
        <el-form-item label="訂單金額">
          <el-input v-model="order.orderAmount" readonly></el-input>
        </el-form-item>
        <el-form-item label="訂單狀態">
          <el-input
            :value="GetOrderStatus(order.orderStatus)"
            readonly
          ></el-input>
        </el-form-item>
        <el-form-item label="退貨狀態">
          <el-input
            :value="GetReturnStatus(order.returnStatus)"
            readonly
          ></el-input>
        </el-form-item>

        <el-form-item label="訂購項目">
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
        </el-form-item>
      </el-form>
    </div>
    <div slot="footer" class="dialog-footer">
      <el-button @click="CloseModal">關閉</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { GetOrderDetails, UpdateDeliveryMethod } from "@/service/api";

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
      order: null,
      orderDetails: [],
      orderStatusMap: {
        1: "處理中",
        2: "已確認",
        3: "已取消",
        4: "訂單已完成",
        5: "宅配-配送失敗",
        6: "自取-未取貨",
        7: "退貨處理中",
        8: "退貨完成",
      },
      returnStatusMap: {
        1: "未申請退貨",
        2: "退貨申請中",
        3: "退貨成功",
      },
      deliveryMethodsMap: [
        { value: 1, label: "宅配" },
        { value: 2, label: "到店取貨" },
      ],
      deliveryStatusMap: {
        1: "備貨中",
        2: "已發貨",
        3: "已送達",
        4: "配送失敗",
        5: "回收退貨中",
        6: "已收到退貨",
        7: "店鋪準備商品中",
        8: "待取貨",
        9: "已取貨",
        10: "未取貨",
      },
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
          console.log(66666666, this.orderDetails);
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
      return this.orderStatusMap[status] || "未知狀態";
    },
    GetReturnStatus(status) {
      return this.returnStatusMap[status] || "未知狀態";
    },
    GetDeliveryStatus(status) {
      return this.deliveryStatusMap[status] || "未知狀態";
    },
    GetDeliveryMethods(status) {
      return this.deliveryMethodsMap[status] || "未知狀態";
    },
    async updateDeliveryMethod(item) {
      if (this.orderId == null) return;
      try {
        const response = await UpdateDeliveryMethod({
          orderId: item.orderId,
          deliveryMethod: Number(item.deliveryMethod),
          deliveryAddress: item.deliveryAddress,
        });
        if (response.data.code === 0) {
          this.$message({
            message: "配送方式修改成功",
            type: "success",
            duration: 1200,
          });
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
  },
};
</script>

<style scoped>
.order-details-table {
  width: 100%;
  border-collapse: collapse;
  margin: 20px 0;
}

.order-details-table th,
.order-details-table td {
  border: 1px solid #ddd;
  padding: 8px;
}

.order-details-table th {
  background-color: #f2f2f2;
  text-align: left;
}
</style>
