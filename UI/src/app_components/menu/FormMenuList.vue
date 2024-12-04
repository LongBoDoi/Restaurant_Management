<!-- Màn danh sách thực đơn -->

<template>
    <VSheet style="height: 100%; padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <VLabel style="font-weight: bold; font-size: 2rem;">Thực đơn</VLabel>

        <VBtn width="fit-content" class="mt-4" color="primary" prepend-icon="mdi-plus" @click="handleAddNewMenuItem">Thêm món mới</VBtn>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <VDataTableServer
            :items-length="totalCount"
            :loading="loading"
            loading-text="Đang tải dữ liệu..." 
            no-data-text="Không có dữ liệu" 
            items-per-page-text="Số bản ghi" 
            :headers="tableHeaders"
            :items="menuItems"
            style="flex-grow: 1;"
            :items-per-page-options="[10, 25, 50, 100]"
            :hover="true"
            v-model:options="options"
            @update:options="getMenuItems"
        >
            <template v-slot:item="{ item, index }">
              <tr v-if="item !== null" style="cursor: pointer;" :class="{'selected-row': index === selectedIndex}" @click="setSelectedIndex(index)" @dblclick="handleOpenMenuDetail">
                <!-- <td>
                  <v-checkbox
                    :value="item"
                    hide-details
                    color="primary"
                  ></v-checkbox>
                </td> -->
                <td>{{ item.Name }}</td>
                <td style="text-align: end;">{{ item.Price }}</td>
                <td>{{ item.Description }}</td>
                <td>
                    <MLVbox class="align-center">
                        <VCheckboxBtn :color="index === selectedIndex ? 'background' : 'primary'" v-model:model-value="item.OutOfStock" readonly />
                    </MLVbox>
                </td>
              </tr>
            </template>
        </VDataTableServer>
    </VSheet>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { MenuItem } from '@/models';
import { menuItemStore } from '@/stores/menuItemStore';
import { mapActions, mapState } from 'pinia';

export default {
    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10
            }
        }
    },

    methods: {
        ...mapActions(menuItemStore, ['getMenuItemsData', 'setSelectedIndex']),

        /**
         * Xử lý thêm món ăn mới
         */
        handleAddNewMenuItem() {
            const newMenuItem = {
                EditMode: this.$enumeration.EnumEditMode.Add
            } as MenuItem;

            this.menuItems.push(newMenuItem);
            this.setSelectedIndex(this.menuItems.indexOf(newMenuItem));

            EventBus.emit(this.$eventName.ShowFormMenuDetail, newMenuItem);
        },

        /**
         * Lấy danh sách món
         */
        async getMenuItems() {
            this.loading = true;
            await this.getMenuItemsData(this.options.page, this.options.itemsPerPage);
            this.loading = false;
        },

        /**
         * Xử lý mở chi tiết món ăn
         */
        handleOpenMenuDetail() {
            const selectedMenuItem = this.menuItems[this.selectedIndex];
            selectedMenuItem.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormMenuDetail, selectedMenuItem);
        }
    },

    computed: {
        ...mapState(menuItemStore, ['menuItems', 'totalCount', 'selectedIndex']),

        tableHeaders():any {
            return [
                { title: 'Tên món', value: 'Name', width: 300, color: 'primary' },
                { title: 'Giá món', value: 'Price', align: 'end', width: 150 },
                { title: 'Mô tả', value: 'Description' },
                { title: 'Hết hàng', value: 'OutOfStock', align: 'center', width: 150 }
            ]
        }
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>