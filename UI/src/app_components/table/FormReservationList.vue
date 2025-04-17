<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-2" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm đặt bàn</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div className="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm tên khách hàng/SĐT..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />
                <MLDateField variant="outlined" compact hide-details class="ml-4" style="width: 180px;" v-model="options.filterDate" />
            </div>

            <!-- Bảng dữ liệu -->
            <VDataTableServer
                sticky
                fixed-header
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as Reservation[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Tên khách hàng</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Số điện thoại</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Thời gian</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Bàn</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="min-width: 120px;">Số người</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Trạng thái</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 128px; width: 128px;">Thao tác</th>
                    </tr>
                </template>

                <template #body v-if="loading">
                    <div class="absolute mt-4 text-center w-full">Đang tải dữ liệu...</div>
                </template>

                <template #no-data>
                    <div class="absolute mt-4 text-center w-full">Không có dữ liệu.</div>
                </template>

                <template v-slot:item="{ item, index }">
                    <span v-if="loading">Đang tải dữ liệu</span>
                    <tr v-if="item !== null" style="cursor: pointer;" class="hover:bg-green-50 transition-colors duration-150" 
                        :class="[
                            {'bg-gray-50' : index % 2 !== 0},
                            {'bg-green-100' : selectedIndex === index}
                        ]" 
                        @click="setSelectedIndex(index)" @dblclick="openDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.CustomerName }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatPhoneNumber(item.CustomerPhoneNumber) }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatDateTime(item.ReservationDate) }}</td>
                        <td class="py-4 px-6 text-sm text-gray-600">{{ item.TableName }}</td>
                        <td class="py-4 px-6 text-right">{{ item.GuestCount }}</td>
                        <td class="py-4 px-6 text-left">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="getReservationStatusClass(item.Status)"
                            >
                                {{ getReservationStatus(item.Status) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <MLHbox v-if="item.Status === $enumeration.EnumReservationStatus.Pending || item.Status === $enumeration.EnumReservationStatus.Confirmed">
                                    <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                    <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)"/>
                                </MLHbox>
                                
                                <VBtn v-else icon="mdi-eye-outline" class="text-gray-700" :width="40" variant="text" @click="openDetail(item)" />
                            </MLHbox>
                        </td>
                    </tr>
                </template>

                <template #bottom>
                    <MLDataTableFooter :options="options" :total-count="totalCount" />
                </template>
            </VDataTableServer>
        </VCard>
    </VSheet>
</template>

<script lang="ts">
import { EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Reservation } from '@/models';
import MLFilterCondition from '@/models/MLFilterCondition';
import { reservationStore } from '@/stores/reservationStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getData();
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                filterDate: <string>moment().startOf('day').utc().format()
            }
        }
    },

    methods: {
        ...mapActions(reservationStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormReservationDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getData() {
            this.loading = true;

            var filters:MLFilterCondition[] = [];
            if (this.options.filterDate) {
                const endFilterTime = moment(this.options.filterDate).add(1, 'days').utc().format();
                filters = [
                    {
                        Name: 'ReservationDate',
                        Operator: '>=',
                        Value: this.options.filterDate
                    } as MLFilterCondition,
                    {
                        Name: 'ReservationDate',
                        Operator: '<',
                        Value: endFilterTime
                    }
                ];
            }
            
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search, JSON.stringify(filters));
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        openDetail(record: Reservation) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormReservationDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: Reservation) {
            switch (item.Status) {
                case EnumReservationStatus.Pending:
                    this.$commonFunction.showDialog({
                        Title: 'Xác nhận xoá đặt chỗ',
                        Message: `Bạn có chắc chắn muốn xoá Đặt chỗ của khách hàng <b>${item.CustomerName}</b> không?`,
                        ConfirmAction: async () => {
                            const saveObject = Object.assign({}, item);
                            saveObject.EditMode = this.$enumeration.EnumEditMode.Delete;

                            const actionResult = await this.$service.ReservationService.saveChanges(saveObject);
                            if (actionResult.Success) {
                                EventBus.emit(this.$eventName.ShowToastMessage, {
                                    Message: 'Xoá thành công.',
                                    Type: 'success'
                                });

                                this.dataList.splice(this.dataList.indexOf(item), 1);
                            }
                        }
                    });
                    break;
                case EnumReservationStatus.Confirmed:
                    this.$commonFunction.showDialog({
                        Title: 'Xác nhận huỷ đặt chỗ',
                        Message: `Bạn có chắc chắn muốn huỷ Đặt chỗ của khách hàng <b>${item.CustomerName}</b> không?`,
                        ConfirmAction: async () => {
                            const saveObject = Object.assign({}, item);
                            saveObject.EditMode = this.$enumeration.EnumEditMode.Edit;
                            saveObject.Status = EnumReservationStatus.Canceled;

                            const actionResult = await this.$service.ReservationService.saveChanges(saveObject);
                            if (actionResult.Success) {
                                Object.assign(item, actionResult.Data as Reservation);

                                EventBus.emit(this.$eventName.ShowToastMessage, {
                                    Message: 'Huỷ thành công.',
                                    Type: 'success'
                                });
                            }
                        }
                    });
                    break;
            }
        },

        /**
         * Tên trạng thái đặt bàn
         */
        getReservationStatus(status: EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumReservationStatus.Confirmed:
                    return 'Đã xác nhận';
                case EnumReservationStatus.Canceled:
                    return 'Đã huỷ';
                case EnumReservationStatus.Received:
                    return 'Đã nhận bàn'
            }
        },

        getReservationStatusClass(status: EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'bg-yellow-100 text-yellow-700';
                case EnumReservationStatus.Confirmed:
                    return 'bg-blue-100 text-blue-800';
                case EnumReservationStatus.Canceled:
                    return 'bg-red-100 text-red-700';
                case EnumReservationStatus.Received:
                    return 'bg-green-300 text-green-700';
            }
        }
    },

    computed: {
        ...mapState(reservationStore, ['dataList', 'selectedIndex', 'totalCount']),
    },

    watch: {
        options: {
            handler() {
                this.getData();
            },
            deep: true
        }
    },
}
</script>

<style lang="scss" scoped>
</style>