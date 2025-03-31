<template>
    <VContainer ref="mainContainer" style="display: flex; align-items: center; justify-content: center;" >
        <MLVbox style="width: 100%; overflow: hidden;" align="center">
            <h2 class="text-2xl md:text-3xl font-bold text-emerald-600 text-center mb-6 mt-6 md:mb-8">
                Đặt bàn ngay
            </h2>

            <div style="width: 100%; background-color: rgba(var(--v-theme-primary), 0.9);" class="max-w-3xl rounded-lg shadow-xl overflow-hidden transform transition-all duration-500 hover:shadow-2xl">
                <VProgressLinear color="white" indeterminate v-if="loading" />
                <VForm ref="form" class="p-8" :disabled="loading">
                    <div class="grid grid-cols-1 md:grid-cols-2 md:gap-6">
                        <div class="space-y-2">
                            <label htmlFor="name" class="block text-white font-medium">
                                Họ tên
                            </label>
                            <VTextField class="placeholder-emerald-700/50" bg-color="rgba(209, 250, 229, 0.9)" variant="outlined" placeholder="Nhập họ tên của bạn" v-model:model-value="reservation.CustomerName"
                                :rules="[textFieldRequireRule]"
                             />
                        </div>

                        <div class="space-y-2">
                            <label htmlFor="phone" class="block text-white font-medium">
                                SĐT
                            </label>
                            <VTextField variant="outlined" class="placeholder-emerald-700/50" bg-color="rgba(209, 250, 229, 0.9)" placeholder="Nhập số điện thoại của bạn" v-model:model-value="reservation.CustomerPhoneNumber"
                                :rules="[textFieldRequireRule]"
                             />
                        </div>
                    </div>

                    <div class="grid md:grid-cols-2 md:gap-6">
                        <div class="space-y-2">
                            <label htmlFor="date" class="block text-white font-medium">
                                Ngày
                            </label>
                            <MLDateField variant="outlined" class="placeholder-emerald-700/50" bg-color="rgba(209, 250, 229, 0.9)" v-model="reservation.ReservationDate" required />
                        </div>

                        <div class="space-y-2">
                            <label htmlFor="time" class="block text-white font-medium">
                                Giờ
                            </label>
                            <MLTimeField variant="outlined" class="placeholder-emerald-700/50" bg-color="rgba(209, 250, 229, 0.9)" v-model="reservation.ReservationDate" />
                        </div>
                    </div>

                    <div class="space-y-2">
                        <label htmlFor="guests" class="block text-white font-medium">
                            Số lượng người
                        </label>
                        <div class="flex items-center space-x-3" style="width: 168px;">
                            <VBtn :width="40" icon="mdi-minus" style="cursor: pointer; opacity: 1; background-color: rgb(52, 211, 153); color: white;" @click="if(reservation.GuestCount > 1) { reservation.GuestCount--; }" />
                            <VTextField :rules="[textFieldRequireRule]" class="text-center placeholder-emerald-700/50" type="number" variant="outlined" bg-color="rgba(209, 250, 229, 0.9)" hide-spin-buttons v-model:model-value="reservation.GuestCount" hide-details />
                            <VBtn :width="40" icon="mdi-plus" style="cursor: pointer; opacity: 1; background-color: rgb(52, 211, 153); color: white;" @click="reservation.GuestCount++;" />
                        </div>
                    </div>

                    <div class="space-y-2 mt-4">
                        <label htmlFor="request" class="block text-white font-medium">
                            Yêu cầu
                        </label>
                        <VTextarea class="placeholder-emerald-700/50" variant="outlined" bg-color="rgba(209, 250, 229, 0.9)" placeholder="Nhập các yêu cầu đặc biệt nếu có" no-resize v-model:model-value="reservation.Note" />
                    </div>

                    <div class="flex justify-end">
                        <VBtn style="border-radius: 18px; height: 48px !important; color: rgb(var(--v-theme-primary))" @click="handleBookReservationRequest">ĐẶT BÀN</VBtn>
                    </div>
                </VForm>
            </div>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
import { EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, MLActionResult } from '@/models';
import Reservation from '@/models/Reservation';

export default {
    data() {
        return {
            loading: <boolean>false,

            reservation: <Reservation>{
                CustomerID: (this.$session.UserType === this.$enumeration.EnumUserType.Customer ? (this.$session.UserData as Customer).CustomerID : undefined),
                CustomerName: (this.$session.UserType === this.$enumeration.EnumUserType.Customer ? (this.$session.UserData as Customer).CustomerName : ''),
                CustomerPhoneNumber: (this.$session.UserType === this.$enumeration.EnumUserType.Customer ? (this.$session.UserData as Customer).PhoneNumber : ''),
                ReservationDate: new Date(),
                ReservationInfo: '',
                Status: EnumReservationStatus.Pending,
                GuestCount: 1,
                Note: ''
            } as Reservation
        }
    },

    methods: {
        async handleBookReservationRequest() {
            const form:any = this.$refs.form;
            const validateResult = await form.validate();
            if (validateResult.valid) {
                this.loading = true;
                
                try {
                    const result:MLActionResult = await this.$service.ReservationService.createCustomerReservation(this.reservation);
                    if (result.Success) {
                        EventBus.emit('ShowToastMessage', {
                            Message: 'Đặt bàn thành công. Nhân viên nhà hàng sẽ sớm liên hệ với bạn.',
                            Type: 'success'
                        });
                    } else {
                        EventBus.emit('ShowToastMessage', {
                            Message: result.ErrorMsg,
                            Type: 'error'
                        });
                    }
                } catch (e) {
                    EventBus.emit('ShowToastMessage', {
                        Message: 'Lỗi hệ thống',
                        Type: 'error'
                    });
                } finally {
                    this.loading = false;
                }
            }
        }
    },

    computed: {
        currentDate() {
            return new Date();
        },

        suggestBookingTime() {
            const suggestDate = new Date(this.currentDate);
            suggestDate.setHours(suggestDate.getHours() + 1);

            return `${suggestDate.getHours()}:00`;
        },

        textFieldRequireRule() {
            return (v:string|undefined) => {
                return v !== undefined && v !== '';
            };
        },

        isCustomerLoggedIn() {
            return this.$session.UserType === this.$enumeration.EnumUserType.Customer && this.$session.UserData !== undefined;
        }
    }
}
</script>

<style lang="scss" scoped>
:deep() {
    .v-input.placeholder-emerald-700\/50 .v-field__input::placeholder {
        color: rgba(4, 120, 87, 0.7);
        opacity: 1;
    }
}
</style>