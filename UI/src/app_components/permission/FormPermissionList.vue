<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-3" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm vai trò</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm mã/tên vai trò..."
                    color="primary"
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <MLSortPopup :items="sortOptionList" v-model="options.sort" />
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
                :items="(dataList as Role[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Mã vai trò</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px; width: 100%;">Tên vai trò</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="min-width: 150px;">Số nhân viên</th>
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
                        <td class="py-4 px-6">{{ item.RoleCode }}</td>
                        <td class="py-4 px-6">{{ item.RoleName }}</td>
                        <td class="py-4 px-6 text-right">{{ $commonFunction.formatThousands(item.Employees?.length ?? 0) }}</td>
                        <td class="py-4 px-6">
                            <MLHbox class="justify-center" >
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" v-if="item.RoleCode !== 'AD'" />
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
import MLSortCondition from '@/models/MLSortCondition';
import Role from '@/models/Role';
import { roleStore } from '@/stores/roleStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                sort: ''
            },
        }
    },

    methods: {
        ...mapActions(roleStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormRoleDetail, newRecord);
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
        openDetail(record: Role) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormRoleDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: Role) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá vai trò',
                Message: `Bạn có chắc chắn muốn xoá Vai trò <b>${item.RoleName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.RoleService.saveChanges(item)).Success) {
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
        ...mapState(roleStore as any, ['dataList', 'selectedIndex', 'totalCount']),

        sortOptionList():MLSortCondition[] {
            return [
                {
                    Text: 'Tên vai trò (A-Z)',
                    Name: 'RoleName',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên vai trò (Z-A)',
                    Name: 'RoleName',
                    Direction: 'DESC'
                },
                {
                    Text: 'Mã vai trò (A-Z)',
                    Name: 'RoleCode',
                    Direction: 'ASC'
                },
                {
                    Text: 'Mã vai trò (Z-A)',
                    Name: 'RoleCode',
                    Direction: 'DESC'
                }
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