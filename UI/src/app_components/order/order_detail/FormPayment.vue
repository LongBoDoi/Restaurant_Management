<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard style="border-radius: 36px; padding: unset;" :disabled="loading" class="v-container">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Title -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Thanh toán</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VForm ref="form">
            <div class="pa-6">
                <div class="mb-6">
                    <div class="flex justify-between mb-2">
                        <span class="text-gray-600">Tổng tiền order:</span>
                        <span class="font-bold text-xl">{{ $commonFunction.formatThousands(order.TotalAmount) }} đ</span>
                    </div>
                    <div class="w-full bg-gray-200 rounded-full">
                        <div
                            class="bg-emerald-500 rounded-full"
                            style="height: 4px;"
                        ></div>
                    </div>
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700">
                        Phương thức thanh toán
                    </label>
                    <div class="grid grid-cols-3 gap-3 mb-4 mt-2">
                        <div class="bg-white border-2 rounded-lg pa-3 text-gray-600 flex flex-col items-center justify-center cursor-pointer hover:bg-gray-50 transition duration-200"
                            :class="[
                                {'border-emerald-500' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Cash}
                            ]"
                            @click="order.PaymentMethod = $enumeration.EnumPaymentMethod.Cash"
                        >
                            <span class="material-symbols-outlined text-3xl mb-1"
                                :class="[
                                    {'text-emerald-600' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Cash}
                                ]"
                            >
                                payments
                            </span>
                            <span class="text-sm font-medium">Tiền mặt</span>
                        </div>
                        <div class="bg-white border border-2 rounded-lg pa-3 text-gray-600 flex flex-col items-center justify-center cursor-pointer hover:bg-gray-50 transition duration-200"
                            :class="[
                                {'border-emerald-500' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Transaction}
                            ]"
                            @click="order.PaymentMethod = $enumeration.EnumPaymentMethod.Transaction"
                        >
                            <span class="material-symbols-outlined text-3xl mb-1"
                                :class="[
                                    {'text-emerald-600' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Transaction}
                                ]"
                            >
                                account_balance
                            </span>
                            <span class="text-sm font-medium text-center">
                                Chuyển khoản ngân hàng
                            </span>
                        </div>
                        <div class="bg-white border border-2 rounded-lg pa-3 flex flex-col text-gray-600 items-center justify-center cursor-pointer hover:bg-gray-50 transition duration-200"
                            :class="[
                                {'border-emerald-500' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Card}
                            ]"
                            @click="order.PaymentMethod = $enumeration.EnumPaymentMethod.Card"
                        >
                            <span class="material-symbols-outlined text-3xl mb-1"
                                :class="[
                                    {'text-emerald-600' : order.PaymentMethod === $enumeration.EnumPaymentMethod.Card}
                                ]"
                            >
                                credit_card
                            </span>
                            <span class="text-sm font-medium">
                                Thẻ
                            </span>
                        </div>
                    </div>
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700">
                        Tiền thực nhận
                    </label>
                    
                    <MLNumberField
                        class="mt-2"
                        density="compact"
                        variant="outlined"
                        hide-details
                        v-model="paymentAmount"

                        :rules="[
                            (v:any) => {
                                return v >= order.TotalAmount || $commonFunction.getRealFloatValue(v) >= order.TotalAmount;
                            }
                        ]"
                        suffix="đ"
                    />
                </div>

                <div class="bg-gray-50 rounded-lg p-4">
                    <div class="flex justify-between">
                        <span class="text-gray-600">Tiền thừa:</span>
                        <span class="font-bold text-green-600">{{ $commonFunction.formatThousands(returnAmount) }} đ</span>
                    </div>
                </div>

                <div class="grid grid-cols-2 gap-4" v-if="false">
                    <div class="flex items-center gap-3 justify-center border border-gray-300 rounded-lg py-3 px-4 hover:bg-gray-50 transition duration-200 cursor-pointer">
                        <span class="material-symbols-outlined">print</span>
                        <span>Print Receipt</span>
                    </div>
                    <div class="flex items-center gap-3 justify-center border border-gray-300 rounded-lg py-3 px-4 hover:bg-gray-50 transition duration-200 cursor-pointer">
                        <span class="material-symbols-outlined">mail</span>
                        <span>Email Receipt</span>
                    </div>
                </div>
            </div>
            </VForm>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Đóng</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded prepend-icon="mdi-check-circle-outline" @click="handleSaveClick">Thanh toán</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { MLActionResult, Order } from '@/models';

export default {
    created() {
        EventBus.on(this.$eventName.ShowFormPayment, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormPayment);
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            order: <Order>{},
            paymentAmount: <number>0,
        }
    },

    computed: {
        returnAmount() {
            return Math.max(this.paymentAmount - this.order.TotalAmount, 0);
        }
    },

    methods: {
        /**
         * Xử lý mở form
         */
         handleShowDialog(order: Order) {
            if (order) {
                if (order.PaymentMethod === null || order.PaymentMethod === undefined) {
                    order.PaymentMethod = this.$enumeration.EnumPaymentMethod.Cash;
                }

                this.order = order;
                this.paymentAmount = order.TotalAmount;

                this.isShow = true;
            }
        },

        handleCloseDialog() {
            // switch (this.record.EditMode) {
            //     case this.$enumeration.EnumEditMode.Add:
            //         this.removeSelectedRecord();
            //         break;
            //     case this.$enumeration.EnumEditMode.Edit:
            //         Object.assign(this.record, this.oldRecord);
            //         break;
            // }
            this.isShow = false;
        },

        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            const saveObject:Order = Object.assign({}, this.order);

            saveObject.Status = this.$enumeration.EnumOrderStatus.Paid;
            saveObject.EditMode = this.$enumeration.EnumEditMode.Edit;

            const actionResult:MLActionResult = await this.$service.OrderService.saveChanges(saveObject);
            if (actionResult.Success) {
                Object.assign(this.order, actionResult.Data as Order);

                this.isShow = false;
                
                this.$emit('paymentSuccess');
            }
            
            this.loading = false;
        }
    },
}
</script>