<template>
    <VDialog width="600" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Cập nhật tồn kho</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="isShow = false;" />
            </VCardTitle>

            <VForm ref="form">
            <div class="pa-6 space-y-4">
                <div className="text-gray-600 mb-2">Lựa chọn nguyên liệu để cập nhật:</div>
                <VDataTable
                    no-data-text="Không có nguyên liệu nào được chọn."
                    ref="menuItemTable"
                    class="menu-item-table rounded-lg"
                    hide-default-footer
                    style="border: 1px solid rgba(var(--v-border-color), var(--v-disabled-opacity)); height: 300px;"
                    :headers="[
                        {
                            title: 'Tên nguyên liệu',
                            value: 'Name',
                            width: 250,
                        },
                        {
                            title: 'Số lượng tồn',
                            value: 'Price',
                            align: 'end',
                        },
                        {
                            title: 'Số lượng mới',
                            value: 'Price',
                            align: 'end',
                        },
                    ]"
                    :items="lstInventoryItem"
                    fixed-header
                    fixed-footer
                    hover
                >
                    <template v-slot:item="{ item, index }">
                        <tr v-if="item !== null" style="cursor: pointer; border-bottom: 1px solid rgb(209, 213, 219);" :class="{'bg-emerald-50': index === selectedInventoryItemIndex}" @click="selectedInventoryItemIndex = index">
                            <td @click="editingInventoryItemIndex = index">
                                <VCombobox
                                    v-if="editingInventoryItemIndex == index"
                                    class="cbMenuItem"
                                    autofocus
                                    hide-details
                                    density="compact"
                                    variant="outlined"
                                    item-title="Name"
                                    item-value="InventoryItemID"
                                    color="primary"
                                    :items="comboboxInventoryItems"
                                    v-model:model-value="(item as InventoryItem)"
                                    v-on:update:model-value="onSelectInventoryItem"
                                    @blur="editingInventoryItemIndex = -1"
                                />
                                <span v-else>{{ item.Name }}</span>
                            </td>
                            <td class="text-right"
                                :class="[
                                    {'text-green-600' : item.Quantity > item.WarningStockQuantity},
                                    {'text-yellow-600' : item.Quantity > 0 && item.Quantity <= item.WarningStockQuantity},
                                    {'text-red-600' : item.Quantity <= 0}
                                ]"
                            >{{ $commonFunction.formatThousands(item.Quantity) }} {{ item.Unit }}</td>
                            <td>
                                <MLNumberField
                                    v-if="item.InventoryItemID"
                                    class="w-full text-right"
                                    hide-details
                                    variant="outlined"
                                    density="compact"
                                    color="primary"
                                    placeholder="Nhập số lượng mới"
                                    v-model:model-value="item.Quantity"
                                />
                            </td>
                        </tr>
                    </template>

                    <template v-slot:bottom>
                        <MLHbox class="pa-1" style="border-top: 1px solid rgb(229, 231, 235);">
                            <VBtn style="text-transform: none; border: none; box-shadow: none; color: #2196F3;" @click="addRecord" rounded>
                                <template #prepend>
                                    <VIcon icon="mdi-plus" color="blue" />
                                </template>
                                Thêm dòng
                            </VBtn>

                            <VBtn style="text-transform: none; box-shadow: none; color: #F44336" rounded @click="removeRecord">
                                <template #prepend>
                                    <VIcon icon="mdi-close" color="red" />
                                </template>
                                Xoá dòng
                            </VBtn>
                        </MLHbox>
                    </template>
                </VDataTable>
            </div>
            </VForm>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="isShow = false">Đóng</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Cập nhật</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { InventoryItem } from '@/models';

export default {
    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
                
            lstInventoryItem: <InventoryItem[]>[],
            editingInventoryItemIndex: <number>-1,
            selectedInventoryItemIndex: <number>0,
            allInventoryItems: <InventoryItem[]>[],
        }
    },

    created() {
        EventBus.on(this.$eventName.ShowFormUpdateStock, this.handleShowDialog);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormUpdateStock);
    },

    methods: {
        async handleShowDialog() {
            this.isShow = true;

            this.loading = true;
            this.allInventoryItems = await this.$service.InventoryItemService.getAll();
            this.loading = false;
        },

        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            this.lstInventoryItem = this.lstInventoryItem.filter(item => item.InventoryItemID);
            this.lstInventoryItem.forEach(item => {
                item.EditMode = this.$enumeration.EnumEditMode.Edit;
            });
            const actionResult = await this.$service.InventoryItemService.saveChangesMultiple(this.lstInventoryItem);
            if (actionResult.Success) {
                this.$commonFunction.showToastMessage('Cập nhật tồn kho thành công.', 'success');
                this.isShow = false;
                EventBus.emit(this.$eventName.ReloadInventoryItemData);
            }

            this.loading = false;
        },

        onSelectInventoryItem(inventoryItem: InventoryItem) {
            if (typeof(inventoryItem) === 'object' && inventoryItem !== null) {
                this.lstInventoryItem[this.editingInventoryItemIndex] = inventoryItem;
            }
        },

        async addRecord() {
            var blankRecord:InventoryItem|undefined = this.lstInventoryItem.find(mi => !mi.InventoryItemID);
            if (!blankRecord) {
                blankRecord = {Name: ''} as InventoryItem;
                this.lstInventoryItem.push(blankRecord);
            }

            this.selectedInventoryItemIndex = this.lstInventoryItem.indexOf(blankRecord);

            await this.$nextTick();

            const menuItemTableBody:any = (this.$refs.menuItemTable as any).$el.querySelector('table tbody').lastElementChild;
            menuItemTableBody.scrollIntoView({ behavior: "smooth" });
        },

        removeRecord() {
            this.lstInventoryItem.splice(this.selectedInventoryItemIndex, 1);
            this.selectedInventoryItemIndex = Math.min(this.selectedInventoryItemIndex, this.lstInventoryItem.length - 1);
        },
    },

    computed: {
        comboboxInventoryItems() {
            const result = [];

            const selectedRecord = this.lstInventoryItem[this.editingInventoryItemIndex];
            if (selectedRecord?.InventoryItemID) {
                result.push(selectedRecord);
            }

            return result.concat(this.allInventoryItems.filter((item) => {
                return this.lstInventoryItem.findIndex((inventoryItem) => inventoryItem.InventoryItemID === item.InventoryItemID) === -1;
            }));
        }
    }
}
</script>