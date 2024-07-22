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
      @search="FetchOrders"
      @prevPage="HandlePrevPage"
      @nextPage="HandleNextPage"
    >
      <template v-slot:table-rows="{ tableData }">
        <tr v-for="(item, index) in tableData" :key="index">
          <td>{{ item.orderNumber }}</td>
          <td>{{ item.orderDate }}</td>
          <td>{{ item.memberName }}</td>
          <td>{{ item.receiver }}</td>
          <td>{{ item.orderAmount }}</td>
          <td>{{ GetOrderStatus(item.orderStatus) }}</td>
          <td>
            <el-button plain @click="OpenOrderDetail(item.orderId)">
              查看詳情
            </el-button>
            <el-popover
              placement="top"
              width="160"
              trigger="click"
              :key="item.orderId"
              v-model="popoversVisible[item.orderId]"
            >
              <p>確認刪除此商品？</p>
              <div class="btn-group" style="text-align: right">
                <el-button
                  size="mini"
                  type="text"
                  @click="popoversVisible[item.orderId] = false"
                  >取消</el-button
                >
                <el-button
                  type="primary"
                  size="mini"
                  @click="DeleteOrder(item.orderId)"
                  >確認</el-button
                >
              </div>
              <el-button slot="reference" type="danger" plain>刪除</el-button>
            </el-popover>
          </td>
        </tr>
      </template>
    </SearchList>
    <OrderDetailModal
      :showModal="showDetailModal"
      :orderId="currentOrderId"
      @close="CloseDetailModal"
    />
  </div>
</template>

<script>
import { GetOrderList, DeleteOrder } from "@/service/api";
import SearchList from "@/components/SearchList.vue";
import UserInfo from "@/components/UserInfo.vue";
import OrderDetailModal from "@/components/Modal/OrderDetailModal.vue";

export default {
  components: {
    SearchList,
    UserInfo,
    OrderDetailModal,
  },
  data() {
    return {
      searchTerm: "",
      sortOptions: [
        { label: "按訂單編號排序", value: 1 },
        { label: "按訂單日期排序", value: 2 },
        { label: "按配送日期排序", value: 3 },
      ],
      tableTitle: [
        "訂單號碼",
        "訂單日期",
        "訂購會員名稱",
        "收件人",
        "訂單金額",
        "訂單狀態",
        "操作",
      ],
      popoversVisible: {},
      tableData: [],
      hasMore: false,
      pageNumber: 1,
      pageSize: 10,
      showDetailModal: false,
      currentOrderId: null,
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
    };
  },
  methods: {
    async FetchOrders(
      searchTerm,
      pageNumber = this.pageNumber,
      sortBy = this.sortOptions[0].value
    ) {
      const response = await GetOrderList({
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
        this.FetchOrders(searchTerm, this.pageNumber, sortBy);
      }
    },
    HandleNextPage(searchTerm, sortBy) {
      if (this.hasMore) {
        this.pageNumber += 1;
        this.FetchOrders(searchTerm, this.pageNumber, sortBy);
      }
    },
    OpenOrderDetail(orderId) {
      this.currentOrderId = orderId;
      this.showDetailModal = true;
    },
    CloseDetailModal() {
      this.showDetailModal = false;
      this.currentOrderId = null;
    },
    GetOrderStatus(status) {
      return this.orderStatusMap[status] || "未知狀態";
    },
    async DeleteOrder(id) {
      console.log(999999999, id);
      try {
        const response = await DeleteOrder({ orderId: id });
        if (response.data.code === 0) {
          this.$message({
            message: "訂單刪除成功",
            type: "success",
            duration: 1200,
          });
          this.FetchOrders(
            this.searchTerm,
            this.pageNumber,
            this.sortOptions[0].value
          );
        } else {
          this.$message({
            message: "刪除失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "刪除失敗",
          type: "error",
          duration: 1200,
        });
      }
    },
  },
  created() {
    this.FetchOrders(
      this.searchTerm,
      this.pageNumber,
      this.sortOptions[0].value
    );
  },
};
</script>

<style scoped></style>
