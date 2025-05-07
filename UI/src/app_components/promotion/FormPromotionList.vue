<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2 overflow-hidden">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-3" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm khuyến mại</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm khách hàng/SĐT..."
                    color="primary"
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <!-- <MLSortPopup :items="sortOptionList" v-model="options.sort" /> -->
            </div>

            <!-- Bảng dữ liệu -->
            <VDataTableServer
                sticky
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as Customer[])"
                style="height: 100%;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Tên khách hàng</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Số điện thoại</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Email</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 100%; min-width: 300px;">Địa chỉ</th>
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
                        <td class="py-4 px-6">{{ item.CustomerName }}</td>
                        <td class="py-4 px-6">
                            {{ $commonFunction.formatPhoneNumber(item.PhoneNumber) }}
                        </td>
                        <td class="py-4 px-6">{{ item.Email }}</td>
                        <td class="py-4 px-6">{{ item.Address }}</td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" />
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
import EventBus from '@/common/EventBus';
import { Customer } from '@/models';
import MLSortCondition from '@/models/MLSortCondition';
import { customerStore } from '@/stores/customerStore';
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
            }
        }
    },

    methods: {
        ...mapActions(customerStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormCustomerDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getData() {
            this.loading = true;
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search, '', this.options.sort);
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        openDetail(record: Customer) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormCustomerDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item:Customer) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá Khuyến mại',
                Message: `Bạn có chắc chắn muốn xoá Khách hàng <b>${item.CustomerName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.CustomerService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });

                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },
    },

    computed: {
        ...mapState(customerStore, ['dataList', 'selectedIndex', 'totalCount']),

        tableHeaders():any {
            return [
                { title: 'Tên món', value: 'Name', width: 300, color: 'primary' },
                { title: 'Giá món', value: 'Price', align: 'end', width: 150 },
                { title: 'Mô tả', value: 'Description' },
                { title: 'Hết hàng', value: 'OutOfStock', align: 'center', width: 150 }
            ]
        },

        sortOptionList():MLSortCondition[] {
            return [
                {
                    Text: 'Tên khách hàng (A-Z)',
                    Name: 'CustomerName',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên khách hàng (Z-A)',
                    Name: 'CustomerName',
                    Direction: 'DESC'
                },
            ]
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