<template>
    <VContainer ref="mainContainer" style="display: flex; align-items: center; justify-content: center;" class="sm:h-fit md:p-10" >
        <MLVbox style="width: 100%; height: 75%; overflow: hidden;" align="center" class="sm:h-fit">
            <h2 className="text-2xl md:text-3xl font-bold text-emerald-600 text-center mb-6 mt-6 md:mb-8">
                Thực Đơn Của Chúng Tôi
            </h2>
            <p className="text-gray-600 text-center max-w-2xl mx-auto mb-10">
                Chúng tôi tự hào mang đến những món ăn đặc trưng với nguyên liệu tươi ngon được chọn lọc kỹ lưỡng, chế
                biến bởi đội ngũ đầu bếp chuyên nghiệp.
            </p>
                
            <VCarousel v-if="displayMenuType === 0" class="mt-6" color="primary" style="height: 100%; background-color: rgba(var(--v-theme-primary), 0.7); border-radius: 8px; flex-grow: 1;" show-arrows="hover" hide-delimiter-background cycle >
                <VCarouselItem v-for="url in lstMenuImageUrls" :src="url" />
            </VCarousel>

            <MLVbox v-if="displayMenuType === 1" style="width: 100%; overflow-y: auto;" class="mt-10">
                <MLVbox v-for="category in lstDisplayItems">
                    <h3 className="text-2xl font-semibold text-emerald-600 mb-6 pb-2 border-b-2 border-emerald-200">
                        {{ category.MenuItemCategoryName }}
                    </h3>
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-1 px-1">
                        <div v-for="menuItem in category.MenuItems" className="bg-white pa-3 md:p-4 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300 flex flex-col md:flex-row gap-3 md:gap-4 group">
                            <div className="w-full h-48 md:w-24 md:h-24 rounded-lg overflow-hidden flex-shrink-0">
                                <div style="background-color: rgb(var(--v-theme-primary));" class="w-full h-full d-flex align-center justify-center">
                                    <VIcon icon="mdi-food" size="40" color="white" />
                                </div>
                                <!-- <img
                                    src="https://i.imgur.com/6JxRwDT.jpg"
                                    alt="Gỏi cuốn tôm thịt"
                                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                                /> -->
                            </div>
                            <div className="flex-1">
                                <div className="flex justify-between items-start">
                                    <h4 className="font-medium text-gray-800 group-hover:text-emerald-600 transition-colors">
                                        <b>{{ menuItem.Name }}</b>
                                    </h4>
                                    <p className="font-semibold text-emerald-600">{{ $commonFunction.formatThousands(menuItem.Price) }} đ</p>
                                </div>
                                <p className="text-gray-600 text-sm mt-2">
                                    <i>{{ menuItem.Description }}</i>
                                </p>
                            </div>
                        </div>
                    </div>
                </MLVbox>
            </MLVbox>
        </MLVbox>
    </VContainer>
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

    methods: {
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
        }
    }
}
</script>

<style lang="scss" scoped>
</style>