<template>
    <VDialog max-height="90vh" persistent :model-value="isShow">
        <VCard style="border-radius: 36px; padding: unset;" :disabled="loading" class="v-container">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Title -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 className="text-white text-xl font-bold">Order</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <div class="pa-6 overflow-y-auto">
                <VForm ref="form" style="height: 100%;">
                    <FormOrderInfo
                        v-if="record.Status === $enumeration.EnumOrderStatus.Active"
                        style="height: 100%;"
                        :record="record" 
                        :edit-mode="editMode"
                        :lst-customers="lstCustomers"
                        :lst-menu-item-categories="lstMenuItemCategories"
                        :lst-menu-items="lstMenuItems"
                        :lst-tables="lstTables"
                        :reserved-tables="reservedTables"
                    />
                    <FormOrderSummary :order="record" v-else />
                </VForm>
            </div>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end">
                <VSpacer />

                <MLHbox v-if="record.Status === $enumeration.EnumOrderStatus.Active" class="space-x-3">
                    <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Đóng</VBtn>
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white" rounded :disabled="disableSave" @click="handleSaveClick">Lưu</VBtn>
                </MLHbox>
                <MLHbox v-else class="space-x-3">
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white" rounded :disabled="disableSave" @click="isShow = false">Đóng</VBtn>
                </MLHbox>
            </VCardActions>
            <!-- <VCardActions v-if="tab === 1 && record.Status === $enumeration.EnumOrderStatus.Active">
                <VSpacer />

                <VBtn @click="tab = 0">Quay lại</VBtn>
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" :disabled="disableSave" @click="handlePayOrderClick">Thanh toán</VBtn>
            </VCardActions> -->
        </VCard>
    </VDialog>
    
    <FormPayment :order="record" v-on:paymentSuccess="onPaymentSuccess" />
</template>

<script lang="ts">
import { EnumEditMode, EnumOrderStatus, } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, MenuItem, MenuItemCategory, MLActionResult, Order, Table } from '@/models';
import { orderStore } from '@/stores/orderStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},
    emits: ['orderCreated', 'paymentSuccess'],

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
                console.log(order)
                if (order.EditMode !== undefined) {
                    this.editMode = order.EditMode;
                }

                if (order.EditMode === this.$enumeration.EnumEditMode.Add) {
                    order.OrderDetails = [];
                    order.Status = EnumOrderStatus.Active;
                    this.reservedTables = order.OrderTables?.map(ot => ot.Table).filter(x => x !== undefined) ?? [];
                }

                this.record = order;
                Object.assign(this.oldRecord, order);
                
                this.loadAllSubData();

                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.editMode) {
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

                if (this.editMode === this.$enumeration.EnumEditMode.Add) {
                    this.$emit('orderCreated');
                }
            }
            this.loading = false;
        },

        onPaymentSuccess() {
            this.isShow = false;
            
            this.$commonFunction.showToastMessage('Thanh toán thành công', 'success');

            this.$emit('paymentSuccess');
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
        },

        loadAllSubData() {
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
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            editMode: <EnumEditMode>EnumEditMode.Add,
            tab: <number>0,

            record: <Order>{} as Order,
            oldRecord: <Order>{} as Order,

            lstCustomers: <Customer[]>[],
            lstMenuItemCategories: <MenuItemCategory[]>[],
            lstMenuItems: <MenuItem[]>[],
            lstTables: <Table[]>[],
            reservedTables: <Table[]>[]
        }
    },

    computed: {
        ...mapState(orderStore, ['dataList', 'selectedIndex']),

        disableSave() {
            return !this.record.OrderDetails?.length;
        },

        subData() {
            return [
                async () => {
                    if (this.editMode === this.$enumeration.EnumEditMode.Edit && this.record.OrderID) {
                        const order:Order|undefined = await this.$service.OrderService.getOrderDetail(this.record.OrderID);
                        if (order) {
                            Object.assign(this.record, order);
                            this.record.EditMode = this.editMode;
                            this.reservedTables = order.OrderTables?.map(ot => ot.Table).filter(x => x !== undefined) ?? [];

                            Object.assign(this.oldRecord, order);
                        }
                    }
                },
                async () => {
                    const lstCustomers:Customer[] = await this.$service.CustomerService.getAll();
                    this.lstCustomers = lstCustomers;
                },
                async () => {
                    const lstMenuItemCategories:MenuItemCategory[] = await this.$service.MenuItemCategoryService.getAll();
                    this.lstMenuItemCategories = lstMenuItemCategories.filter(x => !x.Inactive);
                },
                async () => {
                    const lstMenuItems:MenuItem[] = await this.$service.MenuItemService.getAll();
                    this.lstMenuItems = lstMenuItems.filter(x => !x.Inactive);
                },
                async () => {
                    const lstTables:Table[] = await this.$service.TableService.getAll();
                    this.lstTables = lstTables;
                }
            ]
        }
    }
}
</script>