<template>
    <VDataTable
        hide-default-footer
        style="border: 1px solid rgba(var(--v-border-color), var(--v-disabled-opacity)); border-radius: 24px; height: 300px;"
        :items="(menuItem.MenuItemInventoryItem as MenuItemInventoryItem[])"
        ref="menuItemTable"
    >
        <template #no-data>
            <div class="mt-4">Không có dữ liệu.</div>
        </template>

        <!-- Header -->
        <template #headers>
            <tr class="bg-gray-50">
                <th class="py-3 px-6 text-left font-medium text-gray-500">Tên nguyên liệu</th>
                <th class="py-3 px-6 text-right font-medium text-gray-500" style="width: 200px;">Số lượng</th>
                <th style="width: 100px;"></th>
            </tr>
        </template>

        <!-- Body -->
        <template v-slot:item="{ item, index }">
            <tr v-if="item !== null" style="cursor: pointer; border-bottom: 1px solid rgb(209, 213, 219);" :class="{'bg-emerald-50': index === selectedRow}" @click="selectedRow = index">
                <td @click="editingInventoryItemRow = index">
                    <VCombobox
                        v-if="editingInventoryItemRow === index"
                        class="cbInventoryItem"
                        autofocus
                        hide-details
                        density="compact"
                        variant="outlined"
                        item-title="Name"
                        item-value="MenuItemID"
                        :items="comboboxInventoryItems"
                        v-model:model-value="item.InventoryItem"
                        v-on:update:model-value="onSelectInventoryItem"
                        @blur="editingInventoryItemRow = -1"
                    />
                    <span v-else>{{ item.InventoryItem?.Name ?? '' }}</span>
                </td>
                <td class="text-right" @click="editingQuantityRow = index">
                    <MLNumberField
                        autofocus
                        v-if="editingQuantityRow === index"
                        hide-details
                        variant="outlined"
                        density="compact"
                        class="text-right"
                        @blur="editingQuantityRow = -1"
                        v-model="item.Quantity"
                    />
                    <span v-else>{{ $commonFunction.formatThousands(item.Quantity) }}</span>
                </td>
                <td>
                    {{ item.InventoryItem?.Unit ?? '' }}
                </td>
            </tr>
        </template>

        <!-- Footer -->
        <template v-slot:bottom>
            <MLHbox class="pa-1" style="border-top: 1px solid rgb(229, 231, 235);">
                <VBtn style="text-transform: none; border: none; box-shadow: none; color: #2196F3;" @click="addRow" rounded>
                    <template #prepend>
                        <VIcon icon="mdi-plus" style="margin-top: 3px;" />
                    </template>
                    Thêm dòng
                </VBtn>

                <VBtn style="text-transform: none; box-shadow: none; color: #F44336" rounded @click="removeRow">
                    <template #prepend>
                        <VIcon icon="mdi-close" style="margin-top: 3px;" />
                    </template>
                    Xoá dòng
                </VBtn>
            </MLHbox>
        </template>
    </VDataTable>
</template>

<script lang="ts">
import { InventoryItem, MenuItem, MenuItemInventoryItem } from '@/models';
import { PropType } from 'vue';

export default {
    props: {
        menuItem: {
            type: Object as PropType<MenuItem>,
            required: true
        }
    },

    created() {
        this.$service.InventoryItemService.getAll().then((data:InventoryItem[]) => {
            this.lstInventoryItem = data;
        });

        if (!this.menuItem.MenuItemInventoryItem) {
            this.menuItem.MenuItemInventoryItem = [];
        }
    },

    data() {
        return {
            selectedRow: <number>0,
            editingInventoryItemRow: <number>-1,
            editingQuantityRow: <number>-1,

            lstInventoryItem: <InventoryItem[]>[],
        }
    },

    computed: {
        comboboxInventoryItems() {
            const existItemID = this.menuItem.MenuItemInventoryItem.map(e => e.InventoryItem?.InventoryItemID ?? '');

            const selectedRecord = this.menuItem.MenuItemInventoryItem[this.selectedRow]?.InventoryItem;
            return (selectedRecord ? [selectedRecord] : []).concat(this.lstInventoryItem.filter(mi => !existItemID.includes(mi.InventoryItemID)));
        }
    },

    methods: {
        onSelectInventoryItem() {

        },

        async addRow() {
            var blankRecord:MenuItemInventoryItem|undefined = this.menuItem.MenuItemInventoryItem.find(mi => !mi.InventoryItem);
            if (!blankRecord) {
                blankRecord = {
                    Quantity: 0
                } as MenuItemInventoryItem;
                this.menuItem.MenuItemInventoryItem.push(blankRecord);
            }

            this.selectedRow = this.menuItem.MenuItemInventoryItem.indexOf(blankRecord);

            await this.$nextTick();

            const menuItemTableBody:any = (this.$refs.menuItemTable as any).$el.querySelector('table tbody').lastElementChild;
            menuItemTableBody.scrollIntoView({ behavior: "smooth" });
        },

        removeRow() {
            this.menuItem.MenuItemInventoryItem.splice(this.selectedRow, 1);
            this.selectedRow = Math.min(this.selectedRow, this.menuItem.MenuItemInventoryItem.length - 1);
        }
    }
}
</script>