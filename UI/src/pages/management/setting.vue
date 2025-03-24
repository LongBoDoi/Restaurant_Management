<template>
    <VContainer>
        <MLVbox style="height: 100%;">
            <MLHbox>
                <VLabel class="flex-shrink-0" style="font-weight: bold; font-size: 24px;">Thiết lập</VLabel>

                <VBtn rounded style="text-transform: none; font-size: 16px; color: white;" prepend-icon="mdi-content-save-outline" class="ml-auto bg-green-500 hover:scale-105" @click="saveSettings">Lưu thay đổi</VBtn>
            </MLHbox>

            <VTabs color="primary" v-model:model-value="tab" style="flex-shrink: 0;">
                <VTab value="customerScreen">Màn hình của khách hàng</VTab>
            </VTabs>

            <VTabsWindow :model-value="tab" class="mt-4" style="height: 100%; flex-shrink: 1;">
                <VProgressLinear color="primary" indeterminate v-if="loading" />

                <VTabsWindowItem value="customerScreen" style="overflow-y: hidden;">
                    <FormCustomerScreenSetting ref="frmCustomerScreenSetting" v-bind:lst-settings="lstSettings" v-bind:loading="loading" :image-setting="imageSetting" :lst-menu-item="menuItems" />
                </VTabsWindowItem>
            </VTabsWindow>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
import { EnumDataType } from '@/common/Enumeration';
import { MenuItem, Setting } from '@/models';

export default {
    async created() {
        this.loading = true;
        this.lstSettings = await this.$service.SettingService.getSettings();
        this.menuItems = (await this.$service.SettingService.getMenuItemsForCustomerScreen()).map(category => category.MenuItems).flat();
        this.loading = false;
    },

    methods: {
        async saveSettings() {
            this.loading = true;

            this.lstSettings.forEach(s => {
                switch (s.DataType) {
                    case EnumDataType.Boolean:
                    case EnumDataType.Integer:
                        s.SettingValue = s.Value.toString();
                        break;
                }
            });

            const frmCustomerScreenSetting:any = this.$refs.frmCustomerScreenSetting;
            if (frmCustomerScreenSetting) {
                frmCustomerScreenSetting.prepareData();
            }

            const menuItemSetting = this.lstSettings.find(s => s.SettingKey === 'ListMenuScreenForCustomerItems');
            if (menuItemSetting) {
                menuItemSetting.SettingValue = JSON.stringify(this.menuItems.filter(mi => mi.MenuItemID).map(mi => mi.MenuItemID));
            }

            const saveSuccess:boolean = await this.$service.SettingService.updateSettings(this.lstSettings, this.imageSetting.introImage, this.imageSetting.menuImages);
            if (saveSuccess) {
                this.imageSetting = {
                    introImage: null,
                    menuImages: []
                };
            }
            this.loading = false;
        }
    },

    data() {
        return {
            tab: <string>"customerScreen",

            loading: <boolean>false,
            lstSettings: <Setting[]>[],
            menuItems: <MenuItem[]>[],

            imageSetting: <any>{
                introImage: null,
                menuImages: []
            }
        }
    }
}
</script>

<style scoped lang="scss">
</style>