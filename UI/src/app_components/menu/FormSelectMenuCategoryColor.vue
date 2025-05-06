<template>
    <VDialog width="400" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Tuỳ chọn màu sắc</h2>
                <VBtn variant="text" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="isShow = false" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <div class="mb-6">
                    <label class="block text-gray-700 text-sm font-medium">
                        Giá trị màu
                    </label>
                    <div class="flex space-x-3 mt-2">
                        <div
                            style="width: 64px; height: 40px; border-radius: 12px;"
                            type="color"
                            :value="colorValue"
                            class="border border-gray-300 transition-transform d-flex align-center justify-center"
                        >
                            <div style="width: 56px; height: 32px;"
                                :style="[
                                    {'background-color': colorValue}
                                ]"
                             />
                        </div>

                        <VTextField hide-details density="compact" variant="outlined" v-model:model-value="colorString" prefix="#" v-mask="'XXXXXX'" />
                        <!-- <input
                            type="text"
                            value="#3fbc6e"
                            class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200 font-mono"
                        /> -->
                    </div>
                </div>

                <div class="mb-4">
                    <p class="text-sm text-gray-600 mb-2">Xem trước:</p>
                    <div class="flex space-x-3 items-center">
                        <div style="width: 40px; height: 40px;" class="rounded-full shadow-md"
                            :style="[
                                {'background-color': colorValue}
                            ]"
                        />
                        <div class="px-3 py-1 rounded-full text-white text-sm font-medium shadow-md"
                            :style="[
                                {'background-color': colorValue}
                            ]"
                        >
                            Tag
                        </div>
                    </div>
                </div>

                <VCheckbox hide-details class="text-gray-700" label="Lưu màu vào hệ thống" color="primary" v-model:model-value="isSaveColorToSystem" />
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="isShow = false;">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="onAddColor">Thêm màu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { MLActionResult, Setting } from '@/models';

export default {
    created() {
        EventBus.on(this.$eventName.ShowFormAddMenuCategoryColor, () => {
            this.colorString = '3fbc6e';
            this.isShow = true;
        });
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormAddMenuCategoryColor);
    },
    
    data() {
        return {
            loading: <boolean>false,
            isShow: <boolean>false,

            colorString: <string>'3fbc6e',
            isSaveColorToSystem: <boolean>false
        }
    },
    
    computed: {
        colorValue() {
            return `#${this.colorString}`;
        }
    },

    methods: {
        /**
         * Xác nhận thêm màu
         */
        async onAddColor() {
            if (this.isSaveColorToSystem) {
                this.loading = true;

                const setting:Setting|undefined = this.$session.Settings.find(s => s.SettingKey === 'MenuCategoryColors');
                if (setting) {
                    const lstColor:string[] = JSON.parse(setting.SettingValue);
                    lstColor.push(this.colorValue);
                    setting.SettingValue = JSON.stringify(lstColor);
                    setting.EditMode = this.$enumeration.EnumEditMode.Edit;

                    const actionResult:MLActionResult = await this.$service.SettingService.saveChanges(setting);
                    if (actionResult.Success) {
                        Object.assign(setting, actionResult.Data);
                    }
                }

                this.loading = false;
            }

            this.$emit('addColor', this.colorValue);
            this.isShow = false;
        }
    }
}
</script>