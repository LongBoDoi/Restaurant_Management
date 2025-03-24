<template>
    <VForm style="max-height: 100%; overflow-y: auto;">
        <VCard class="mt-2 pa-6 ml-1 mr-1 bg-gray-50" style="border-radius: 24px;" :disabled="loading">
            <b style="font-size: 18px;">Thông tin nhà hàng</b>

            <!-- Ảnh giới thiệu -->
            <MLVbox class="mt-6"> 
                <span style="width: 150px;">Ảnh giới thiệu</span>
                <div class="intro-image-box mt-3">
                    <template v-if="introImageUrl">
                        <VImg :src="introImageUrl" />
                        <VIcon class="close-icon" icon="mdi-close" color="red" @click="onRemoveIntroImage" />
                    </template>

                    <template v-else>
                        <div @click="updateImageCallback = updateIntroImage; selectImage()" style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                            <VIcon icon="mdi-image-plus-outline" color="rgb(156, 163, 175)" />
                        </div>
                    </template>
                </div>

                <VFileInput
                    ref="introImage"
                    v-show="false"
                    accept="image/*"
                    :multiple="multipleImageSelector"
                    v-model:model-value="imagesSelected"
                    v-on:update:model-value="getImageUrls"
                />
            </MLVbox>

            <!-- Tên nhà hàng, Số điện thoại -->
            <MLHbox class="mt-6">
                <MLVbox class="flex-grow-1">
                    <span>Tên nhà hàng</span>
                    <VTextField 
                        class="mt-2"
                        density="compact" 
                        variant="outlined"
                        hide-details
                        placeholder="Nhập tên nhà hàng" 
                        v-model:model-value="getSettingByKey('RestaurantName').SettingValue"
                    />
                </MLVbox>

                <VSpacer style="width: 24px; flex-grow: 0;" />

                <MLVbox class="flex-grow-1">
                    <span>Số điện thoại</span>
                    <VTextField 
                        class="mt-2" 
                        density="compact" 
                        variant="outlined"
                        hide-details
                        placeholder="Nhập số điện thoại nhà hàng" 
                        v-model="getSettingByKey('RestaurantPhoneNumber').SettingValue" 
                    />
                </MLVbox>
            </MLHbox>

            <!-- Địa chỉ -->
            <MLVbox class="mt-4">
                <span>Địa chỉ</span>
                <VTextField 
                    class="mt-2"
                    density="compact" 
                    variant="outlined"
                    hide-details
                    style="width: 65%;" 
                    placeholder="Nhập địa chỉ nhà hàng" 
                    v-model="getSettingByKey('RestaurantAddress').SettingValue" 
                />
            </MLVbox>

            <!-- Châm ngôn -->
            <MLVbox class="mt-4 mb-6">
                <span style="width: 150px;">Châm ngôn phục vụ</span>
                <VTextField 
                    class="mt-2"
                    density="compact" 
                    variant="outlined"
                    hide-details
                    style="width: 65%;" 
                    placeholder="Nhập châm ngôn phục vụ của nhà hàng" 
                    v-model="getSettingByKey('RestaurantSlogan').SettingValue" 
                />
            </MLVbox>
        </VCard>

        <FormOpeningTimeSetting :disabled="loading" class="mt-6" ref="frmOpeningTimeSetting" :setting="openingTimeSetting" />

        <FormSocialMediaSetting :disabled="loading" class="mt-6" ref="frmSocialMediaSetting" :setting="socialMediaSetting" />

        <VCard class="mt-6 mb-1 pa-6 ml-1 mr-1 bg-gray-50" style="border-radius: 24px;" :disabled="loading">
            <b style="font-size: 18px;">Tuỳ chỉnh hiển thị</b>

            <VCheckbox color="primary" class="mt-2" hide-details v-model="(getSettingByKey('DisplayMenuScreenForCustomer').Value)" label="Hiển thị màn Thực đơn" />

            <VExpandTransition>
                <MLVbox class="ml-8" v-if="(getSettingByKey('DisplayMenuScreenForCustomer').Value as boolean)">
                    <VRadioGroup inline color="primary" v-model:model-value="(getSettingByKey('DisplayMenuScreenForCustomerType').Value as number)" hide-details>
                        <VRadio label="Hiển thị theo ảnh" :value="0" />
                        <VRadio class="ml-4" label="Hiển thị theo danh sách món" :value="1" />
                    </VRadioGroup>

                    <VTabsWindow v-model:model-value="(getSettingByKey('DisplayMenuScreenForCustomerType').Value as number)">
                        <VTabsWindowItem :value="0">
                            <MLVbox class="ml-4 mt-2">
                                <span>Chọn ảnh thực đơn</span>
                                <MLHbox style="overflow-x: auto;" class="mt-2">
                                    <div class="intro-image-box mr-4" v-for="imageUrl, index in lstMenuImageUrls">
                                        <VImg :src="imageUrl" />
                                        <VIcon class="close-icon" icon="mdi-close" color="red" @click="onRemoveMenuImage(index, imageUrl)" />
                                    </div>

                                    <div class="intro-image-box">
                                        <div @click="updateImageCallback = updateMenuImages; selectImage(true)" style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                                            <VIcon icon="mdi-image-plus-outline" color="rgb(156, 163, 175)" />
                                        </div>
                                    </div>
                                </MLHbox>
                            </MLVbox>
                        </VTabsWindowItem>

                        <VTabsWindowItem :value="1">
                            <MLHbox class="ml-4 mt-2">
                                <span style="width: 150px; margin-top: 8px;">Chọn thực đơn</span>

                                <MLVbox>
                                    <VRadioGroup color="primary" v-model:model-value="(getSettingByKey('DisplayMenuScreenByItemsForCustomerType').Value as number)" hide-details>
                                        <VRadio label="Toàn bộ thực đơn" :value="0" />
                                        <VRadio label="Tuỳ chọn" :value="1" />
                                    </VRadioGroup>

                                    <VExpandTransition>
                                        <MLVbox v-if="(getSettingByKey('DisplayMenuScreenByItemsForCustomerType').Value as number) === 1" class="mt-2">
                                            <VDataTable
                                                no-data-text="Không có món."
                                                ref="menuItemTable"
                                                class="menu-item-table"
                                                hide-default-footer
                                                style="border: 1px solid rgba(var(--v-border-color), var(--v-disabled-opacity)); border-radius: 8px; width: 500px; height: 300px;"
                                                :headers="[
                                                    {
                                                        title: 'Tên món',
                                                        value: 'Name',
                                                        width: 250,
                                                    },
                                                    {
                                                        title: 'Giá món',
                                                        value: 'Price',
                                                    },
                                                ]"
                                                :items="lstMenuItem"
                                                fixed-header
                                                fixed-footer
                                                hover
                                            >
                                                <template v-slot:item="{ item, index }">
                                                    <tr v-if="item !== null" style="cursor: pointer; border-bottom: 1px solid rgb(209, 213, 219);" :class="{'bg-emerald-50': index === selectedMenuItemIndex}" @click="selectedMenuItemIndex = index">
                                                        <td @click="editingMenuItemIndex = index">
                                                            <VCombobox
                                                                v-if="editingMenuItemIndex == index"
                                                                class="cbMenuItem"
                                                                autofocus
                                                                hide-details
                                                                density="compact"
                                                                variant="outlined"
                                                                item-title="Name"
                                                                item-value="MenuItemID"
                                                                :items="comboboxMenuItems"
                                                                v-model:model-value="(item as MenuItem)"
                                                                v-on:update:model-value="onSelectMenuItem"
                                                                @blur="editingMenuItemIndex = -1"
                                                            />
                                                            <span v-else>{{ item.Name }}</span>
                                                        </td>
                                                        <td>{{ $commonFunction.formatThousands(item.Price) }} đ</td>
                                                    </tr>
                                                </template>

                                                <template v-slot:bottom>
                                                    <MLHbox class="pa-1" style="border-top: 1px solid rgb(229, 231, 235);">
                                                        <VBtn style="text-transform: none; border: none; box-shadow: none; color: #2196F3;" @click="addMenuItem" rounded>
                                                            <template #prepend>
                                                                <VIcon icon="mdi-plus" color="blue" />
                                                            </template>
                                                            Thêm dòng
                                                        </VBtn>

                                                        <VBtn style="text-transform: none; box-shadow: none; color: #F44336" rounded @click="removeMenuItem">
                                                            <template #prepend>
                                                                <VIcon icon="mdi-close" color="red" />
                                                            </template>
                                                            Xoá dòng
                                                        </VBtn>
                                                    </MLHbox>
                                                </template>
                                            </VDataTable>
                                        </MLVbox>
                                    </VExpandTransition>
                                </MLVbox>
                            </MLHbox>
                        </VTabsWindowItem>
                    </VTabsWindow>
                </MLVbox>
            </VExpandTransition>

            <VCheckbox color="primary" hide-details label="Hiển thị màn Đặt chỗ" v-model="(getSettingByKey('DisplayBookingScreenForCustomer').Value)" />
        </VCard>
    </VForm>
</template>

<script lang="ts">
import { MenuItem, Setting } from '@/models';
import { PropType } from 'vue';

export default {
    props: {
        loading: {
            type: Boolean,
            default: false
        },
        lstSettings: {
            type: Object as PropType<Setting[]>,
            default: []
        },
        lstMenuItem: {
            type: Object as PropType<MenuItem[]>,
            default: []
        },
        imageSetting: {
            type: Object
        }
    },

    async created() {
        // Lấy toàn bộ danh sách món để gán vào combobox
        const data:any = await this.$service.MenuItemService.getDataPaging(-1, -1);
        if (data && data.Data && data.Data.length) {
            this.allMenuItems = data.Data as MenuItem[];
        }

        // Lấy url ảnh giới thiệu
        const introImageUrlSetting = this.lstSettings.find(s => s.SettingKey === 'IntroImageUrl');
        if (introImageUrlSetting && introImageUrlSetting.Value) {
            this.introImageUrl = this.$commonFunction.getImageUrl(introImageUrlSetting.Value);
        }

        // Lấy url danh sách ảnh thực đơn
        const menuImageSetting = this.lstSettings.find(s => s.SettingKey === 'ListMenuScreenForCustomerImages');
        if (menuImageSetting && menuImageSetting.Value) {
            const lstMenuImages:string[] = JSON.parse(menuImageSetting.Value);
            lstMenuImages.forEach((menuImage:string) => {
                this.lstMenuImageUrls.push(this.$commonFunction.getImageUrl(menuImage));
            })
        }
    },

    methods: {
        async selectImage(multiple?:boolean) {
            this.multipleImageSelector = multiple === true;

            await this.$nextTick();

            const introImage = (this.$refs.introImage as any);

            const input = introImage.$el.querySelector('input[type=file]');
            input.click();
        },

        getImageUrls(files:File|File[]) {
            if ((files as File[]).length) {
                (files as File[]).forEach(file => {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        if (this.updateImageCallback) {
                            this.updateImageCallback(file, e.target?.result);
                        }
                    };
                    reader.readAsDataURL(file as File);
                });
            } else {
                const reader = new FileReader();
                reader.onload = (e) => {
                    if (this.updateImageCallback) {
                        this.updateImageCallback(files, e.target?.result);
                    }
                };
                reader.readAsDataURL(files as File);
            }
        },

        updateIntroImage(image: File, imageUrl: string) {
            if (this.imageSetting) {
                this.imageSetting.introImage = image;
            }
            
            this.introImageUrl = imageUrl;

            this.imagesSelected = undefined;
        },

        updateMenuImages(image: File, imageUrl: any) {
            if (this.imageSetting) {
                if (this.imageSetting.menuImages === undefined) {
                    this.imageSetting.menuImages = [];
                }
                
                this.imageSetting.menuImages.push(image);
            }
            this.lstMenuImageUrls.push(imageUrl);

            this.imagesSelected = undefined;
        },

        onRemoveIntroImage() {
            if (this.imageSetting) {
                this.imageSetting.introImage = null;
            }

            this.introImageUrl = '';
            
            const introImageUrlSetting = this.getSettingByKey('IntroImageUrl');
            introImageUrlSetting.SettingValue = '';
        },

        onRemoveMenuImage(imageIndex: number, imageUrl: string) {
            if (this.imageSetting && this.imageSetting.lstMenuImages) {
                this.imageSetting.lstMenuImages.splice(imageIndex, 1);
            }
            this.lstMenuImageUrls.splice(imageIndex, 1);

            const menuImageSetting = this.lstSettings.find(s => s.SettingKey === 'ListMenuScreenForCustomerImages');
            if (menuImageSetting && menuImageSetting.SettingValue) {
                const lstMenuImages:string[] = JSON.parse(menuImageSetting.SettingValue);
                const oldImageIndex = lstMenuImages.findIndex(mi => this.$commonFunction.getImageUrl(mi) === imageUrl);
                if (oldImageIndex !== -1) {
                    lstMenuImages.splice(oldImageIndex, 1);

                    menuImageSetting.SettingValue = JSON.stringify(lstMenuImages);
                }
            }
        },

        getSettingByKey(settingKey: string):Setting {
            return this.lstSettings.find(s => s.SettingKey === settingKey) ?? {
                SettingKey: settingKey
            } as Setting;
        },

        async addMenuItem() {
            var blankRecord:MenuItem|undefined = this.lstMenuItem.find(mi => !mi.MenuItemID);
            if (!blankRecord) {
                blankRecord = {Name: ''} as MenuItem;
                this.lstMenuItem.push(blankRecord);
            }

            this.selectedMenuItemIndex = this.lstMenuItem.indexOf(blankRecord);

            await this.$nextTick();

            const menuItemTableBody:any = (this.$refs.menuItemTable as any).$el.querySelector('table tbody').lastElementChild;
            menuItemTableBody.scrollIntoView({ behavior: "smooth" });;
        },

        removeMenuItem() {
            this.lstMenuItem.splice(this.selectedMenuItemIndex, 1);
            this.selectedMenuItemIndex = Math.min(this.selectedMenuItemIndex, this.lstMenuItem.length - 1);
        },

        onSelectMenuItem(menuItem:MenuItem) {
            if (typeof(menuItem) === 'object' && menuItem !== null) {
                this.lstMenuItem[this.editingMenuItemIndex] = menuItem;
            }
        },

        /**
         * Chuẩn bị dữ liệu trước khi lưu
         */
        prepareData() {
            const frmSocialMediaSetting:any = this.$refs.frmSocialMediaSetting;
            if (frmSocialMediaSetting) {
                frmSocialMediaSetting.prepareData();
            }

            const frmOpeningTimeSetting:any = this.$refs.frmOpeningTimeSetting;
            if (frmOpeningTimeSetting) {
                frmOpeningTimeSetting.prepareData();
            }
        }
    },

    data() {
        return {
            multipleImageSelector: <boolean>false,
            updateImageCallback: <Function|undefined>undefined,

            imagesSelected: <any>undefined,
            introImageUrl: <string>'',
            lstMenuImageUrls: <string[]>[],

            selectedMenuItemIndex: <number>0,
            editingMenuItemIndex: <number>-1,

            allMenuItems: <MenuItem[]>[]
        }
    },

    computed: {
        comboboxMenuItems():MenuItem[] {
            const existItemID = this.lstMenuItem.map(mi => mi.MenuItemID);

            return this.allMenuItems.filter(mi => !existItemID.includes(mi.MenuItemID));
        },

        openingTimeSetting() {
            return this.lstSettings.find(s => s.SettingKey === 'OpeningTimes');
        },

        socialMediaSetting() {
            return this.lstSettings.find(s => s.SettingKey === 'SocialMediaUrls');
        }
    }
}
</script>

<style scoped lang="scss">
.intro-image-box {
    width: 100px; 
    height: 100px; 
    border: 2px dashed rgb(209, 213, 219);
    display: flex; 
    align-items: center; 
    justify-content: center; 
    border-radius: 24px; 
    position: relative;
    flex-shrink: 0;

    .close-icon {
        position: absolute; 
        top: 0; 
        right: 0; 
        display: none;
    }

    &:hover .close-icon {
        display: block !important;
    }
}

.selected-row {
    background-color: #81c784;
    color: white;
}

:deep() {
    .cbMenuItem .v-field {
        background-color: white;
        color: black;
    }

    .menu-item-table th {
        background-color: rgb(249, 250, 251) !important;
    }
}
</style>