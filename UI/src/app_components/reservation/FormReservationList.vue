<template>
    <VSheet style="height: calc(100vh - 64px); padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <VLabel style="font-weight: bold; font-size: 2rem; flex-shrink: 0;">Đặt bàn</VLabel>
        
        <VBtn width="fit-content" class="mt-4 flex-shrink-0" color="primary" prepend-icon="mdi-plus" @click="handleAddNewReservation">Tạo đặt bàn</VBtn>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <VTabs class="flex-shrink-0" color="primary" v-model="reservationStatus" @update:model-value="getReservations">
            <VTab :value="EnumReservationStatus.Pending">Chờ xác nhận</VTab>
            <VTab :value="EnumReservationStatus.Active">Đang hoạt động</VTab>
            <VTab :value="EnumReservationStatus.Closed">Đã nhận</VTab>
        </VTabs>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <MLVbox style="flex-grow: 1; overflow: hidden;">
            <VDataTableServer
                :sticky="true"
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi" 
                :headers="[
                    {
                        title: 'Khách hàng',
                        value: 'CustomerName'
                    },
                    {
                        title: 'Số điện thoại',
                        value: 'CustomerPhoneNumber',
                        width: 200
                    },
                    {
                        title: 'Thời gian',
                        value: 'ReservationDate',
                        align: 'center',
                        width: 250
                    },
                    {
                        title: 'Trạng thái',
                        value: 'Status',
                        width: 250
                    }
                ]"
                :items="(dataList as Reservation[])"
                style="height: 100%"
                :items-per-page-options="[10, 25, 50, 100]"
                :hover="true"
                v-model:options="options"
                @update:options="getReservations"
            >
            <template v-slot:item="{ item, index }">
              <tr v-if="item !== null" style="cursor: pointer;" :class="{'selected-row': index === selectedIndex}" @click="setSelectedIndex(index)" @dblclick="handleOpenDetail">
                <!-- <td>
                  <v-checkbox
                    :value="item"
                    hide-details
                    color="primary"
                  ></v-checkbox>
                </td> -->
                <td>{{ item.CustomerName }}</td>
                <td>{{ item.CustomerPhoneNumber }}</td>
                <td :style="{'text-align': 'center'}">{{ CommonFunction.formatDateTime(item.ReservationDate) }}</td>
                <td :style="{'color': getReservationStatusColor(item.Status)}">{{ getReservationStatusName(item.Status) }}</td>
              </tr>
            </template>
        </VDataTableServer>
        </MLVbox>
    </VSheet>
</template>

<script lang="ts">
import { EnumEditMode, EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Reservation } from '@/models';
import { reservationStore } from '@/stores/reservationStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';

export default {
    data() {
        return {
            reservationStatus: <EnumReservationStatus>EnumReservationStatus.Pending,
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10
            }
        }
    },

    methods: {
        ...mapActions(reservationStore as any, ['getReservationsData', 'setSelectedIndex', 'addNewRecord']),

        /**
         * Lấy danh sách đặt bàn
         */
        async getReservations() {
            this.loading = true;
            await this.getReservationsData(this.reservationStatus, this.options.page, this.options.itemsPerPage);
            this.loading = false;
        },

        handleAddNewReservation() {
            let newReservation:Reservation = {
                EditMode: EnumEditMode.Add
            } as Reservation;
            if (this.reservationStatus === EnumReservationStatus.Active) {
                newReservation = this.addNewRecord();
            }
            newReservation.ReservationDate = moment(new Date()).utc().format() as any;

            EventBus.emit(this.$eventName.ShowFormReservationDetail, newReservation);
        },

        getReservationStatusName(status:EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumReservationStatus.Active:
                    return 'Đang hoạt động';
                case EnumReservationStatus.Closed:
                    return 'Đã nhận';
            }
        },

        getReservationStatusColor(status:EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'rgb(var(--v-theme-warning))';
                case EnumReservationStatus.Active:
                    return 'rgb(var(--v-theme-success))';
                case EnumReservationStatus.Closed:
                    return 'rgb(var(--v-theme-error))';
            }
        },

        handleOpenDetail() {
            const selectedReservation = this.dataList[this.selectedIndex];
            selectedReservation.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormReservationDetail, selectedReservation);
        }
    },

    computed: {
        ...mapState(reservationStore as any, ['dataList', 'totalCount', 'selectedIndex']),

        EnumReservationStatus() {
            return EnumReservationStatus;
        },

        CommonFunction() {
            return this.$commonFunction;
        }
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;

    td {
        color: white !important;
    }
}
</style>