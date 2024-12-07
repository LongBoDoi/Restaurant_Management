<template>
    <VDialog max-width="800" persistent :model-value="isShow">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                Thông tin đặt bàn
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem>
                <VLabel class="mb-8" v-if="reservation.EditMode !== $enumeration.EnumEditMode.Add"><b>Trạng thái:&nbsp;</b><span :style="{'color': reservationStatusColor}">{{ reservationStatusName }}</span></VLabel>

                <VForm ref="form" :readonly="reservation.Status === $enumeration.EnumReservationStatus.Closed">
                <MLHbox class="mt-2">
                    <!-- Khách hàng -->
                    <VTextField 
                        density="compact" 
                        variant="outlined" 
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        v-model="reservation.CustomerName"
                        style="flex-grow: 0.6;"
                    >
                        <template v-slot:label>
                            Khách hàng
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>

                    <VSpacer style="flex-grow: 0; flex-shrink: 0; width: 16px;" />

                    <!-- Số điện thoại -->
                    <VTextField
                        density="compact"
                        variant="outlined" 
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        style="flex-grow: 0.4;"
                        v-model="reservation.CustomerPhoneNumber"
                    >
                        <template v-slot:label>
                            Số điện thoại
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>
                </MLHbox>

                <MLHbox>
                    <!-- Ngày -->
                    <MLDateField variant="outlined" style="width: 70%;" v-model="reservation.ReservationDate" />

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Giờ -->
                    <MLTimeField variant="outlined" style="width: 30%;" v-model="reservation.ReservationDate" />
                </MLHbox>

                <!-- Thông tin -->
                <VTextField 
                    density="compact" 
                    label="Thông tin"
                    variant="outlined"
                    placeholder="vd: Số bàn, số tầng,..." 
                    v-model="reservation.ReservationInfo" 
                    :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                >
                    <template v-slot:label>
                        Thông tin
                        <span style="color: red;">*</span>
                    </template>
                </VTextField>

                <VTextarea density="compact" variant="outlined" label="Yêu cầu của khách hàng" no-resize v-model="reservation.Note" />
                </VForm>
            </VCardItem>

            <VCardActions v-if="editMode === $enumeration.EnumEditMode.Add">
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleAddClick">Lưu</VBtn>
            </VCardActions>
            <VCardActions v-if="editMode === $enumeration.EnumEditMode.Edit && reservation.Status === $enumeration.EnumReservationStatus.Pending">
                <VBtn color="error" @click="handleDeleteReservation">Từ chối</VBtn>
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleConfirmClick($enumeration.EnumReservationStatus.Active)">Xác nhận</VBtn>
            </VCardActions>
            <VCardActions v-if="editMode === $enumeration.EnumEditMode.Edit && reservation.Status === $enumeration.EnumReservationStatus.Active">
                <VBtn color="error" @click="handleDeleteReservation">Huỷ đặt bàn</VBtn>
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleConfirmClick($enumeration.EnumReservationStatus.Closed)">Nhận bàn</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode, EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MLActionResult, Reservation } from '@/models';
import { reservationStore } from '@/stores/reservationStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormReservationDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormReservationDetail);
    },

    methods: {
        ...mapActions(reservationStore as any, ['setSelectedIndex', 'removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(reservation: Reservation) {
            if (reservation) {
                this.editMode = reservation.EditMode;
                this.reservation = reservation;
                this.oldReservation = JSON.parse(JSON.stringify(this.reservation));
            }
            this.isShow = true;
        },

        /**
         * Xử lý đóng form
         */
        handleCloseDialog() {
            switch (this.reservation.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    this.dataList[this.selectedIndex] = this.oldReservation;
                    break;
            }
            this.isShow = false;
        },

        /**
         * Xử lý xác nhận đặt bàn
         */
        async handleConfirmClick(newStatus: EnumReservationStatus) {
            const formValid = await this.validateForm();
            if (!formValid) return;

            this.reservation.Status = newStatus;

            this.loading = true;
            if (await this.saveChanges('Xác nhận thành công')) {
                this.removeSelectedRecord();
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Xử lý tạo đặt bàn mới
         */
        async handleAddClick() {
            const formValid = await this.validateForm();
            if (!formValid) return;

            this.reservation.Status = EnumReservationStatus.Active;

            this.loading = true;
            if (await this.saveChanges('Lưu thành công')) {
                this.isShow = false;
            }
            this.loading = false;
        },

        async validateForm():Promise<boolean> {
            const form = this.$refs.form as any;
            return (await form.validate()).valid as boolean;
        },

        handleDeleteReservation() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận huỷ đặt bàn',
                Message: 'Bạn có chắc chắn muốn huỷ đặt bàn này không?',
                ConfirmAction: async () => {
                    this.reservation.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if (await this.saveChanges('Huỷ đặt bàn thành công')) {
                        this.isShow = false;
                        this.removeSelectedRecord();
                    }
                }
            });
        },

        /**
         * Xử lý lưu dữ liệu bản ghi
         */
        async saveChanges(confirmMessage: string):Promise<boolean> {
            let result:MLActionResult|undefined = undefined;

            try {
                result = await this.$service.ReservationService.saveChanges(this.reservation);
            } catch (e) {
                this.$commonFunction.handleException(e);
            }

            if (!result) return false;

            if (result.Success) {
                if (this.dataList[this.selectedIndex]) {
                    this.dataList[this.selectedIndex] = result.Data as Reservation;
                }
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: confirmMessage,
                    Type: 'success'
                });
            } else {
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: result.ErrorMsg,
                    Type: 'error'
                });
            }

            return result.Success;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            editMode: <EnumEditMode>EnumEditMode.Add,

            oldReservation: <Reservation>{} as Reservation,
            reservation: <Reservation>{} as Reservation
        }
    },

    computed: {
        ...mapState(reservationStore as any, ['dataList', 'selectedIndex']),

        reservationStatusName() {
            switch (this.reservation.Status) {
                case EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumReservationStatus.Active:
                    return 'Đang hoạt động';
                case EnumReservationStatus.Closed:
                    return 'Đã nhận';
            }
        },

        reservationStatusColor() {
            switch (this.reservation.Status) {
                case EnumReservationStatus.Pending:
                    return 'rgb(var(--v-theme-warning))';
                case EnumReservationStatus.Active:
                    return 'rgb(var(--v-theme-success))';
                case EnumReservationStatus.Closed:
                    return 'rgb(var(--v-theme-error))';
            }
        },
    }
}
</script>