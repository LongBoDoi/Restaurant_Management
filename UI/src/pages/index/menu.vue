<template>
    <div class="bg-emerald-50" style="min-height: 100%;">
        <VContainer class="py-6 px-4 sm:py-8 sm:px-8">
            <div class="text-center mb-12">
                <div class="flex justify-center items-center mb-4">
                    <div class="h-0.5 bg-emerald-200 w-24"></div>
                    <h2 class="text-3xl font-bold px-6 text-emerald-800">Thực Đơn Của Chúng Tôi</h2>
                    <div class="h-0.5 bg-emerald-200 w-24"></div>
                </div>
                <p class="text-gray-600 max-w-2xl mx-auto px-2 sm:px-0">
                    Khám phá bộ sưu tập sushi tươi ngon của chúng tôi, được chế biến từ nguyên liệu cao cấp và kỹ thuật truyền thống.
                </p>
            </div>

            <div class="mb-10" v-for="category in lstDisplayItems">
                <h3 class="text-xl font-semibold border-b-2 border-emerald-500 pb-2 mb-6">
                    {{ category.MenuItemCategoryName }}
                </h3>
                <!-- <div
                    class="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6"
                > -->
                <VSlideGroup :direction="(($commonFunction.getScreenCode() === 'xs' || $commonFunction.getScreenCode() === 'sm') ? 'vertical' : 'horizontal')">
                    <template #prev>
                        <VBtn icon="mdi-chevron-left" />
                    </template>
                    <template #next>
                        <VBtn icon="mdi-chevron-right" />
                    </template>

                    <VSlideGroupItem
                        v-for="menuItem in category.MenuItems"
                    >
                        <div class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300 border border-gray-100 my-4 md:my-0 md:mx-4 mb-4 w-full md:max-w-md group">
                            <div class="relative overflow-hidden">
                                <img
                                    v-if="menuItem.ImageUrl"
                                    :src="$commonFunction.getImageUrl(menuItem.ImageUrl)"
                                    style="aspect-ratio: 16/9;"
                                    class="w-full object-cover hover:scale-105 transition-transform duration-500"
                                />
                                <div v-else class="bg-gray-200 w-full flex items-center justify-center"
                                    style="aspect-ratio: 16/9;"
                                >
                                    <VIcon icon="mdi-food" class="text-gray-400" style="font-size: 6rem;" />
                                </div>
                            </div>
                            <div class="p-4 group-hover:text-emerald-600 transition-colors">
                                <div class="flex justify-between">
                                    <h4 class="text-lg font-semibold">
                                        {{ menuItem.Name }}
                                    </h4>
                                    <span class="font-bold">
                                        {{ $commonFunction.formatThousands(menuItem.Price) }} đ
                                    </span>
                                </div>
                                <p class="text-gray-600 text-sm mt-1">
                                    {{ menuItem.Description }}
                                </p>
                            </div>
                        </div>
                    </VSlideGroupItem>
                <!-- </div> -->
                </VSlideGroup>
            </div>

            <FormCreateSushi />
        </VContainer>
    </div>
</template>

<script lang="ts">
import { MenuItemCategory } from '@/models';

export default {
    async created() {
        this.lstDisplayItems = await this.$service.SettingService.getMenuItemsForCustomerScreen();
    },

    data() {
        return {
            lstDisplayItems: <MenuItemCategory[]>[]
        }
    },
    
    computed: {
        lstMenuImageUrls() {
            const settingValue = this.$commonFunction.getSettingValue('ListMenuScreenForCustomerImages');
            if (settingValue) {
                return (JSON.parse(settingValue) as string[]).map(url => this.$commonFunction.getImageUrl(url));
            }
            return [];
        },

        displayMenuType() {
            return this.$commonFunction.getSettingValue('DisplayMenuScreenForCustomerType');
        },

        screenCode():string {
            return this.$commonFunction.getScreenCode();
        }
    }
}
</script>