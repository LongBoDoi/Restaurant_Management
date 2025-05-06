<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2 overflow-hidden">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-3" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm nhân viên</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm mã/tên nhân viên..."
                    color="primary"
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <MLFilterPopup :filter-count="lstFilters.length" v-on:apply-filter="handleApplyFilters" v-on:reset-filter="handleResetFilters">
                    <div class="space-y-4">
                        <label class="block font-medium text-gray-700">Trạng thái làm việc</label>
                        <VSelect
                            :items="[
                                {
                                    Text: 'Tất cả',
                                    Value: -1
                                },
                                {
                                    Text: 'Chính thức',
                                    Value: $enumeration.EnumEmployeeWorkStatus.Active
                                },
                                {
                                    Text: 'Thử việc',
                                    Value: $enumeration.EnumEmployeeWorkStatus.Probation
                                },
                                {
                                    Text: 'Nghỉ phép',
                                    Value: $enumeration.EnumEmployeeWorkStatus.OnLeave
                                },
                                {
                                    Text: 'Nghỉ việc',
                                    Value: $enumeration.EnumEmployeeWorkStatus.Terminated
                                }
                            ]"
                            item-title="Text"
                            item-value="Value"
                            v-model:model-value="workStatusFilter"

                            hide-details
                            density="compact"
                            variant="outlined"
                            color="primary"
                            class="mt-1"
                        />
                    </div>
                </MLFilterPopup>

                <MLSortPopup :items="sortOptionList" v-model="options.sort" />
            </div>

            <!-- Bảng dữ liệu -->
            <VDataTableServer
                sticky
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as Employee[])"
                style="height: 100%;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Mã nhân viên</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px; width: 100%;">Tên nhân viên</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Số điện thoại</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Email</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Trạng thái</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 128px;">Thao tác</th>
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
                        <td class="py-4 px-6">{{ item.EmployeeCode }}</td>
                        <td class="py-4 px-6">{{ item.EmployeeName }}</td>
                        <td class="py-4 px-6">
                            {{ $commonFunction.formatPhoneNumber(item.PhoneNumber) }}
                        </td>
                        <td class="py-4 px-6">{{ item.Email }}</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="getWorkStatusClass(item.WorkStatus)"
                            >
                                {{ getWorkStatusName(item.WorkStatus) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox style="height: 40px;">
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" v-if="item.EmployeeCode !== 'admin'" />
                                <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" v-if="item.EmployeeCode !== 'admin'" />
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
import { EnumEmployeeWorkStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Employee, UserLogin } from '@/models';
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { employeeStore } from '@/stores/employeeStore';
import { mapActions, mapState } from 'pinia';

export default {
    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                sort: ''
            },

            workStatusFilter: <any>-1,
            lstFilters: <MLFilterCondition[]>[],
        }
    },

    methods: {
        ...mapActions(employeeStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
         handleAddNewClick() {
            const newRecord:Employee = this.addNewRecord();
            newRecord.WorkStatus = this.$enumeration.EnumEmployeeWorkStatus.Active;
            newRecord.UserLogin = {

            } as UserLogin;
            
            EventBus.emit(this.$eventName.ShowFormEmployeeDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getData() {
            this.loading = true;
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search, this.filterJson, this.options.sort);
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        openDetail(record: Employee) {
            if (record.EmployeeCode === 'admin') {
                return;
            }
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormEmployeeDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: Employee) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá món',
                Message: `Bạn có chắc chắn muốn xoá Nhân viên <b>${item.EmployeeName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.EmployeeService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });

                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },

        handleResetFilters() {
            this.workStatusFilter = -1;

            this.lstFilters = [];
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.workStatusFilter !== -1) {
                this.lstFilters.push({
                    Name: 'WorkStatus',
                    Operator: '==',
                    Value: this.workStatusFilter
                } as MLFilterCondition);
            }

            this.getData();
        },

        getWorkStatusName(status: EnumEmployeeWorkStatus) {
            switch (status) {
                case EnumEmployeeWorkStatus.Active:
                    return 'Chính thức';
                case EnumEmployeeWorkStatus.Probation:
                    return 'Thử việc';
                case EnumEmployeeWorkStatus.OnLeave:
                    return 'Nghỉ phép';
                case EnumEmployeeWorkStatus.Terminated:
                    return 'Nghỉ việc';
            }
        },

        getWorkStatusClass(status: EnumEmployeeWorkStatus) {
            switch (status) {
                case EnumEmployeeWorkStatus.Active:
                    return 'bg-green-300 text-green-700';
                case EnumEmployeeWorkStatus.Probation:
                    return 'bg-blue-100 text-blue-700';
                case EnumEmployeeWorkStatus.OnLeave:
                    return 'bg-yellow-100 text-yellow-700';
                case EnumEmployeeWorkStatus.Terminated:
                    return 'bg-red-100 text-red-700';
            }
        }
    },

    computed: {
        ...mapState(employeeStore, ['dataList', 'selectedIndex', 'totalCount']),

        sortOptionList():MLSortCondition[] {
            return [
                {
                    Text: 'Mã nhân viên (A-Z)',
                    Name: 'EmployeeCode',
                    Direction: 'ASC'
                },
                {
                    Text: 'Mã nhân viên (Z-A)',
                    Name: 'EmployeeCode',
                    Direction: 'DESC'
                },
                {
                    Text: 'Tên nhân viên (A-Z)',
                    Name: 'EmployeeName',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên nhân viên (Z-A)',
                    Name: 'EmployeeName',
                    Direction: 'DESC'
                }
            ]
        },

        filterJson() {
            if (this.lstFilters.length) {
                return JSON.stringify(this.lstFilters);
            }
            return '';
        }
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
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>