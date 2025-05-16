<template>
    <div class="bg-emerald-50 d-flex align-center justify-center" style="min-height: 100%;">
    <VContainer ref="mainContainer" style="display: flex; align-items: center; justify-content: center;" class="h-full">
        <MLVbox style="width: 100%; overflow: hidden;" align="center" class="py-10">
            <div class="flex justify-center items-center mb-8">
                <div class="h-0.5 bg-emerald-200 w-24"></div>
                <h2 class="text-3xl font-bold px-6 text-emerald-800 text-center">Đặt Bàn Ngay</h2>
                <div class="h-0.5 bg-emerald-200 w-24"></div>
            </div>

            <div style="width: 100%;" class="bg-emerald-200 max-w-3xl rounded-lg shadow-xl overflow-hidden transform transition-all duration-500 hover:shadow-2xl">
                <VProgressLinear color="primary" indeterminate v-if="loading" />
                <VForm v-if="!bookReservationSuccess" ref="form" class="p-8" :disabled="loading">
                    <div class="grid grid-cols-1 md:grid-cols-2 md:gap-6">
                        <div class="space-y-2">
                            <label htmlFor="name" class="block text-emerald-800 font-medium">
                                Họ tên
                            </label>
                            <VTextField class="" bg-color="white" variant="outlined" placeholder="Nhập họ tên của bạn" v-model:model-value="reservation.CustomerName"
                                density="compact"
                                :rules="[textFieldRequireRule]"
                             />
                        </div>

                        <div class="space-y-2">
                            <label htmlFor="phone" class="block text-emerald-800 font-medium">
                                SĐT
                            </label>
                            <VTextField variant="outlined" class="" bg-color="white" placeholder="Nhập số điện thoại của bạn"
                                density="compact"
                                v-mask="'0### ### ###'"
                                :model-value="reservation.CustomerPhoneNumber"
                                v-on:update:model-value="(v:string) => {
                                    reservation.CustomerPhoneNumber = $commonFunction.getRealPhoneNumberValue(v);
                                }"
                                :rules="[textFieldRequireRule]"
                             />
                        </div>
                    </div>

                    <div class="grid md:grid-cols-2 md:gap-6">
                        <div class="space-y-2">
                            <label htmlFor="date" class="block text-emerald-800 font-medium">
                                Ngày
                            </label>
                            <MLDateField compact variant="outlined" class="placeholder-emerald-700/50" bg-color="white" v-model="reservation.ReservationDate" required />
                        </div>

                        <div class="space-y-2">
                            <label htmlFor="time" class="block text-emerald-800 font-medium">
                                Giờ
                            </label>
                            <MLTimeField compact variant="outlined" class="placeholder-emerald-700/50" bg-color="white" v-model="reservation.ReservationDate" />
                        </div>
                    </div>

                    <div class="space-y-2">
                        <label htmlFor="guests" class="block text-emerald-800 font-medium">
                            Số lượng người
                        </label>
                        <div class="flex items-center space-x-3" style="width: 168px;">
                            <VBtn :width="40" icon="mdi-minus" style="cursor: pointer; opacity: 1; background-color: rgb(52, 211, 153); color: white;" @click="if(reservation.GuestCount > 1) { reservation.GuestCount--; }" />
                            <VTextField density="compact" :rules="[textFieldRequireRule]" class="text-center placeholder-emerald-700/50" type="number" variant="outlined" bg-color="white" hide-spin-buttons v-model:model-value="reservation.GuestCount" hide-details />
                            <VBtn :width="40" icon="mdi-plus" style="cursor: pointer; opacity: 1; background-color: rgb(52, 211, 153); color: white;" @click="reservation.GuestCount++;" />
                        </div>
                    </div>

                    <div class="space-y-2 mt-4">
                        <label htmlFor="request" class="block text-emerald-800 font-medium">
                            Yêu cầu
                        </label>
                        <VTextarea class="" variant="outlined" bg-color="white" placeholder="Nhập các yêu cầu đặc biệt nếu có" no-resize v-model:model-value="reservation.Note" />
                    </div>

                    <div class="flex justify-end">
                        <VBtn prepend-icon="mdi-check" class="bg-primary text-white" style="border-radius: 18px; opacity: 1 !important;" @click="handleBookReservationRequest">ĐẶT BÀN</VBtn>
                    </div>
                </VForm>
                <div v-else class="flex flex-col items-center justify-center py-8 space-y-6">
                    <VIcon class="bg-emerald-400 text-white rounded-full pa-4" icon="mdi-check" :size="80" />

                    <h3 class="text-2xl font-bold text-emerald-800 text-center">Đặt bàn thành công!</h3>

                    <p class="text-emerald-700 text-center max-w-md">
                        Cảm ơn bạn đã đặt bàn tại {{ $commonFunction.getSettingValue('RestaurantName') }}. Chúng tôi sẽ sớm liên hệ với bạn để xác nhận thông tin.
                    </p>

                    <div class="bg-emerald-50 p-6 rounded-lg w-full max-w-md border border-emerald-200">
                        <h4 class="font-semibold text-emerald-800 mb-4 text-lg">Thông tin đặt bàn</h4>

                        <div class="space-y-3">
                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Họ tên:</span>
                                <span class="text-emerald-900">{{ reservation.CustomerName }}</span>
                            </div>

                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Số điện thoại:</span>
                                <span class="text-emerald-900">{{ reservation.CustomerPhoneNumber }}</span>
                            </div>

                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Ngày:</span>
                                <span class="text-emerald-900">{{ $commonFunction.formatDate(reservation.ReservationDate) }}</span>
                            </div>

                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Giờ:</span>
                                <span class="text-emerald-900">{{ $commonFunction.formatTime(reservation.ReservationDate) }}</span>
                            </div>

                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Số người:</span>
                                <span class="text-emerald-900">{{ reservation.GuestCount }} người</span>
                            </div>

                            <div class="flex justify-between">
                                <span class="text-emerald-700 font-medium">Yêu cầu:</span>
                                <span class="text-emerald-900 text-right" style="max-width: 10rem;">{{ reservation.Note }}</span>
                            </div>

                            <div class="border-t border-emerald-200 pt-3 mt-3">
                            </div>
                        </div>
                    </div>

                    <h4 class="font-bold text-emerald-800">Chúng tôi rất mong đợi được phục vụ bạn tốt nhất!</h4>
                    <p class="text-emerald-700 mb-3 text-center max-w-md">
                        Vui lòng đến đúng giờ hẹn. Nếu bạn cần thay đổi thông tin hoặc huỷ đặt bàn, vui lòng liên hệ lại cho chúng tôi trước ít nhất 04 tiếng.
                    </p>
                </div>
            </div>
        </MLVbox>
    </VContainer>
    </div>
</template>

<script lang="ts">
import { EnumReservationStatus } from '@/common/Enumeration';
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
            } as Reservation,

            bookReservationSuccess: <boolean>false,
        }
    },

    methods: {
        async handleBookReservationRequest() {
            const form:any = this.$refs.form;
            const validateResult = await form.validate();
            if (validateResult.valid) {
                this.loading = true;
                
                const result:MLActionResult = await this.$service.ReservationService.createCustomerReservation(this.reservation);
                if (result.Success) {
                    this.bookReservationSuccess = true;

                    Object.assign(this.reservation, result.Data as Reservation);
                }

                this.loading = false;
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