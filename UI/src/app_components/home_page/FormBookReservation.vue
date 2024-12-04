<template>
    <VContainer ref="mainContainer" style="display: flex; align-items: center; justify-content: center;" >
        <MLVbox style="width: 100%; overflow: hidden;" align="center">
            <VLabel style="font-size: 3rem; font-weight: bold; flex-shrink: 0; color: rgb(var(--v-theme-primary))">Đặt bàn ngay</VLabel>

            <VSpacer style="height: 24px; flex-grow: 0; flex-shrink: 0;" />

            <VCard :loading="loading" :disabled="loading" color="primary" width="100%" height="100%" maxWidth="500" class="d-flex flex-column">
                <VCardItem>
                    <VForm ref="form">
                        <MLHbox>
                            <VTextField v-model:model-value="reservation.CustomerName" :rules="[textFieldRequireRule]" variant="underlined" label="Họ tên" />
                            <VSpacer style="width: 24px; flex-shrink: 0; flex-grow: 0;" />
                            <VTextField v-model:model-value="reservation.CustomerPhoneNumber" :rules="[textFieldRequireRule]" variant="underlined" label="SĐT" />
                        </MLHbox>

                        <MLHbox>
                            <MLDateField ref="txtReservationDate" style="flex-grow: 0.7;" :value="currentDate" />
                            <VSpacer style="width: 24px; flex-shrink: 0; flex-grow: 0;" />
                            <MLTimeField ref="txtReservationTime" style="flex-grow: 0.3;" :value="suggestBookingTime" />
                        </MLHbox>

                        <VTextField v-model:model-value="reservation.GuestCount" type="number" :rules="[textFieldRequireRule]" hideSpinButtons variant="underlined" label="Số lượng người" />

                        <VTextarea v-model:model-value="reservation.Note" label="Yêu cầu" variant="outlined" placeholder="vd: Số tầng, phòng riêng,..." />
                    </VForm>
                </VCardItem>

                <VCardActions>
                    <VBtn style="background-color: white; color: rgb(var(--v-theme-primary)); margin-left: auto; font-weight: bold;" @click="handleBookReservationRequest">Đặt bàn</VBtn>
                </VCardActions>
            </VCard>
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