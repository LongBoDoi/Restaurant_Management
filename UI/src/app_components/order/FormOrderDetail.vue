<template>
    <VDialog persistent :model-value="isShow">
        <VResponsive :aspect-ratio="16/9" style="max-height: 85vh;">
            <VContainer height="100%">
                <VCard style="height: 100%;" :disabled="loading" class="d-flex flex-column">
                    <template v-slot:loader>
                        <VProgressLinear v-if="loading" indeterminate color="primary" />
                    </template>
                    <VCardTitle class="d-flex align-center">
                        Chi tiết order
                        <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
                    </VCardTitle>

                    <div style="flex-grow: 1; overflow: hidden;">
                        <VForm ref="form" style="height: 100%;">
                        <VTabsWindow v-model="tab" style="height: 100%;">
                            <VTabsWindowItem :value="0" style="height: 100%;">
                                <FormOrderInfo style="height: 100%;" :record="record" :edit-mode="editMode" />
                            </VTabsWindowItem>
                            <VTabsWindowItem :value="1" style="height: 100%;">
                                <FormOrderReceipt :order="record" style="height: 100%;" />
                            </VTabsWindowItem>
                        </VTabsWindow>
                        </VForm>
                    </div>

                    <VCardActions v-if="tab === 0">
                        <VBtn v-if="editMode === $enumeration.EnumEditMode.Edit" prepend-icon="mdi-trash-can-outline" color="error" @click="handleDeleteRecord">Huỷ order</VBtn>

                        <VSpacer />

                        <VBtn @click="handleCloseDialog">Đóng</VBtn>
                        <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" :disabled="disableSave" @click="handleSaveClick">Lưu</VBtn>
                    </VCardActions>
                    <VCardActions v-if="tab === 1 && record.Status === $enumeration.EnumOrderStatus.Active">
                        <VSpacer />

                        <VBtn @click="tab = 0">Quay lại</VBtn>
                        <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" :disabled="disableSave" @click="handlePayOrderClick">Thanh toán</VBtn>
                    </VCardActions>
                </VCard>
            </VContainer>
        </VResponsive>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode, EnumOrderStatus, } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MLActionResult, Order } from '@/models';
import { orderStore } from '@/stores/orderStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormOrderDetail, this.handleShowDialog as any);
        EventBus.on(this.$eventName.SwitchTabFormOrderDetail, this.handleSwitchTab as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormOrderDetail);
        EventBus.off(this.$eventName.SwitchTabFormOrderDetail);
    },

    methods: {
        ...mapActions(orderStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(order: Order) {
            if (order) {
                if (order.EditMode !== undefined) {
                    this.editMode = order.EditMode;
                }

                if (order.EditMode === this.$enumeration.EnumEditMode.Add) {
                    order.OrderDetails = [];
                    order.Status = EnumOrderStatus.Active;
                }

                switch (order.Status) {
                    case EnumOrderStatus.Paid:
                        this.tab = 1;
                        break;
                    default:
                        this.tab = 0;
                }

                this.record = order;
                this.oldRecord = JSON.parse(JSON.stringify(this.record));
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    this.dataList[this.selectedIndex] = this.oldRecord;
                    break;
            }
            this.isShow = false;
        },

        /**
         * Xử lý chuyển tab
         */
        handleSwitchTab(tab: number) {
            this.tab = tab;
        },

        /**
         * Xử lý nhấn vào nút Lưu
         */
        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;
            const success = await this.saveChanges('Lưu thành công');
            if (success) {
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Xử lý nhấn Thanh toán màn thanh toán
         */
        async handlePayOrderClick() {
            this.loading = true;

            this.record.Status = EnumOrderStatus.Paid;

            const success = await this.saveChanges('Lưu thành công');
            if (success) {
                this.isShow = false;
                this.removeSelectedRecord();
            }
            this.loading = false;
        },

        /**
         * Xử lý xoá món
         */
        handleDeleteRecord() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận huỷ order',
                Message: `Bạn có chắc chắn muốn huỷ order <b>${this.record.OrderName}</b> không?`,
                ConfirmAction: async () => {
                    this.record.EditMode = this.$enumeration.EnumEditMode.Delete;
                    const success = await this.saveChanges('Huỷ order thành công');

                    if (success) {
                        this.isShow = false;
                        this.removeSelectedRecord();
                    }
                }
            });
        },

        /**
         * Lưu thông tin khách hàng
         * @param confirmMessage Câu thông báo khi lưu thành công
         */
        async saveChanges(confirmMessage: string):Promise<boolean> {
            let result:MLActionResult|undefined = undefined;

            try {
                result = await this.$service.OrderService.saveChanges(this.record);
            } catch (e) {
                this.$commonFunction.handleException(e);
            }

            if (!result) return false;

            if (result.Success) {
                this.dataList[this.selectedIndex] = result.Data as Order;
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: confirmMessage,
                    Type: 'success'
                });
            } else {
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: result.ErrorMsg,
                    Type: 'error'
                });
            };

            return result.Success;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            editMode: <EnumEditMode>EnumEditMode.Add,
            tab: <number>0,

            record: <Order>{} as Order,
            oldRecord: <Order>{} as Order,
        }
    },

    computed: {
        ...mapState(orderStore, ['dataList', 'selectedIndex']),

        disableSave() {
            return !this.record.OrderDetails?.length;
        }
    }
}
</script>