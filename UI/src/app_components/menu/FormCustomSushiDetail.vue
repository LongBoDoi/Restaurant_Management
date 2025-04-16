<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Nguyên liệu</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <div class="flex justify-between mb-4">
                        <!-- Tên món -->
                        <div class="flex items-center">
                            <span class="material-symbols-outlined text-green-500 mr-2">restaurant</span>
                            <span class="text-lg font-semibold text-gray-800">{{ record.MenuItemName }}</span>
                        </div>

                        <!-- Trạng thái -->
                        <span class="px-3 py-1 rounded-full text-xs font-medium" :class="getStatusClass">
                            {{ getStatusName }}
                        </span>
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <!-- Khách hàng -->
                        <div class="bg-gray-50 pa-3 rounded-lg">
                            <div class="text-sm text-gray-500 mb-1">Khách hàng</div>
                            <div class="font-medium">{{ record.CustomerName }}</div>
                        </div>

                        <!-- Ngày tạo -->
                        <div class="bg-gray-50 pa-3 rounded-lg">
                            <div class="text-sm text-gray-500 mb-1">Ngày tạo</div>
                            <div class="font-medium">{{ $commonFunction.formatDateTime(record.CreatedDate) }}</div>
                        </div>
                    </div>

                    <div class="mb-4">
                        <!-- Nguyên liệu -->
                        <div class="text-sm text-gray-500 mb-2">Nguyên liệu</div>
                        <div class="flex flex-wrap gap-2">
                            <span v-for="inventoryItem in record.InventoryItems" class="bg-gray-100 text-gray-700 px-3 py-1 rounded-full text-sm flex items-center hover:bg-gray-200 transition-colors duration-200">
                                {{ inventoryItem.Name }}
                            </span>
                        </div>
                    </div>

                    <div class="bg-gray-50 p-4 rounded-lg mb-4">
                        <!-- Yêu cầu đặc biệt -->
                        <div class="text-sm text-gray-500 mb-2">Yêu cầu đặc biệt</div>
                        <p class="text-gray-700">
                            {{ record.Note }}
                        </p>
                    </div>

                    <div class="mb-4">
                        <!-- Giá bán -->
                        <MLHbox class="align-center mb-2">
                            <div class="text-sm text-gray-500">Giá bán</div>
                            <div class="font-medium ml-auto" v-if="record.Status !== $enumeration.EnumCustomMenuRequestStatus.Pending">{{ $commonFunction.formatThousands(record.Price) }} đ</div>
                        </MLHbox>
                        <MLNumberField v-if="record.Status === $enumeration.EnumCustomMenuRequestStatus.Pending" density="compact" variant="outlined" suffix="đ" class="text-right" color="primary" hide-details v-model="record.Price"
                            :rules="[(v: number) => v >= 0]"
                        />
                    </div>

                    <div class="mb-4" v-if="record.Status === $enumeration.EnumCustomMenuRequestStatus.Pending">
                        <!-- Tuỳ chọn lưu món -->
                        <VCheckbox color="primary" class="text-gray-700" hide-details label="Thêm vào order của khách" v-model:model-value="record.SaveToOrder" v-if="record.OrderID" />
                        <VCheckbox color="primary" class="text-gray-700" hide-details label="Thêm vào thực đơn" v-model:model-value="record.SaveToMenu" />
                    </div>
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <MLHbox class="space-x-3" v-if="record.Status === $enumeration.EnumCustomMenuRequestStatus.Pending">
                    <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                    <VBtn variant="tonal" class="bg-red-500 hover:bg-red-600 text-white ml-1 hover:scale-105" rounded @click="handleSave($enumeration.EnumCustomMenuRequestStatus.Rejected)">Từ chối</VBtn>
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1 hover:scale-105" rounded @click="handleSave($enumeration.EnumCustomMenuRequestStatus.Approved)" :disabled="!record.SaveToOrder && !record.SaveToMenu">Xác nhận</VBtn>
                </MLHbox>

                <MLHbox v-else>
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1 hover:scale-105" rounded @click="isShow = false;">Đóng</VBtn>
                </MLHbox>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumCustomMenuRequestStatus, EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { CustomMenuRequest, MLActionResult } from '@/models';
import { customMenuRequestStore } from '@/stores/customMenuRequestStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormCustomMenuDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormCustomMenuDetail);
    },

    methods: {
        ...mapActions(customMenuRequestStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: CustomMenuRequest) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.record = record;
                Object.assign(this.oldRecord, this.record);

                this.loadAllSubData();

                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    Object.assign(this.record, this.oldRecord);
                    break;
            }
            this.isShow = false;
        },

        /**
         * Xử lý nhấn vào nút Lưu
         */
        async handleSave(status: EnumCustomMenuRequestStatus) {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;
            
            const saveObject = Object.assign({}, this.record);
            saveObject.Status = status;
            saveObject.EditMode = this.editMode;
            const actionResult:MLActionResult = await this.$service.CustomMenuRequestService.saveChanges(saveObject);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: status === EnumCustomMenuRequestStatus.Rejected ? 'Từ chối thành công' : 'Lưu thành công.',
                    Type: 'success'
                });

                this.isShow = false;
            }

            this.loading = false;
        },

        loadAllSubData() {
            if (this.subData.length) {
                this.loading = true;

                var servicesCalled:number = 0;
                this.subData.forEach((subData:any) => {
                    subData().then(() => {
                        servicesCalled++;
                        if (servicesCalled >= this.subData.length) {
                            this.loading = false;
                        }
                    });
                });
            }
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <CustomMenuRequest>{} as CustomMenuRequest,
            oldRecord: <CustomMenuRequest>{} as CustomMenuRequest,
        }
    },

    computed: {
        ...mapState(customMenuRequestStore, ['dataList', 'selectedIndex']),

        /**
         * Hiển thị trạng thái
         */
         getStatusName() {
            switch (this.record.Status) {
                case EnumCustomMenuRequestStatus.Approved:
                    return 'Đã xác nhận';
                case EnumCustomMenuRequestStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumCustomMenuRequestStatus.Rejected:
                    return 'Đã huỷ';
            }
        },

        /**
         * Hiển thị trạng thái
         */
         getStatusClass() {
            switch (this.record.Status) {
                case EnumCustomMenuRequestStatus.Approved:
                    return 'bg-green-300 text-green-700';
                case EnumCustomMenuRequestStatus.Pending:
                    return 'bg-yellow-100 text-yellow-700';
                case EnumCustomMenuRequestStatus.Rejected:
                    return 'bg-red-100 text-red-700';
            }
        },

        subData() {
            return [
                async () => {
                    if (this.editMode === this.$enumeration.EnumEditMode.Edit && this.record.CustomMenuRequestID) {
                        const data:CustomMenuRequest|undefined = await this.$service.CustomMenuRequestService.getByID(this.record.CustomMenuRequestID);
                        if (data) {
                            Object.assign(this.record, data);
                            Object.assign(this.oldRecord, data);

                            this.record.EditMode = this.editMode;
                        }
                    }
                },
                async () => {
                    if (this.record.CustomerID) {
                        const activeOrder = await this.$service.OrderService.getActiveOrderByCustomerID(this.record.CustomerID);
                        this.record.OrderID = activeOrder?.OrderID;
                    }
                }
            ]
        },
    }
}
</script>