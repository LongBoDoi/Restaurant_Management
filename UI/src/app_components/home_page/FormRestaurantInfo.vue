<template>
    <VContainer class="w-full sm:h-fit md:h-full d-flex flex-column mx-auto bg-white font-sans" style="flex-shrink: 0; padding: unset;">
        <!-- <header class="bg-gradient-to-r from-emerald-600 to-emerald-500 shadow-lg">
            <div
                class="container mx-auto py-4 px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row justify-between items-center gap-4 sm:gap-0"
            >
                <div class="flex items-center gap-3">
                    <img
                        src="https://i.imgur.com/RBcYetj.png"
                        alt="Logo"
                        class="h-12 w-12 sm:h-14 sm:w-14 rounded-full border-2 border-white shadow-md transform hover:scale-105 transition-transform duration-300"
                    />
                    <h1 class="text-white text-xl sm:text-2xl font-semibold tracking-wide">Nhà Hàng 123</h1>
                </div>
                <nav class="hidden md:flex items-center space-x-6">
                    <a href="#" class="text-white hover:text-emerald-100 transition-colors duration-300">
                        Trang chủ
                    </a>
                    <a href="#" class="text-white hover:text-emerald-100 transition-colors duration-300">
                        Thực đơn
                    </a>
                    <a href="#" class="text-white hover:text-emerald-100 transition-colors duration-300">
                        Giới thiệu
                    </a>
                    <a href="#" class="text-white hover:text-emerald-100 transition-colors duration-300">
                        Liên hệ
                    </a>
                </nav>
                <button
                    class="bg-white text-emerald-600 px-5 py-2 rounded-btn font-medium hover:bg-emerald-50 transition-colors duration-300 shadow-md transform hover:-translate-y-0.5"
                >
                    ĐĂNG NHẬP
                </button>
            </div>
        </header> -->
        <main class="flex flex-col md:flex-row md:overflow-hidden" style="flex-grow: 1;">
            <div class="w-full md:w-1/2 lg:w-3/5 h-[300px] sm:h-[400px] md:h-full overflow-hidden">
                <img
                    :src="introImageUrl"
                    alt="Restaurant Interior"
                    class="w-full h-full object-cover hover:scale-105 transition-transform duration-700 ease-in-out"
                />
            </div>
            <div
                class="w-full md:w-1/2 lg:w-2/5 bg-gradient-to-br p-6 sm:p-8 md:p-10 lg:p-12 flex flex-col justify-center"
                style="background-color: rgb(236, 253, 245);"
            >
                <!-- Tên nhà hàng -->
                <h2 class="text-3xl sm:text-4xl md:text-5xl font-bold text-emerald-600 mb-4 sm:mb-6">
                    {{ getSettingValue('RestaurantName') ?? 'Tên nhà hàng' }}
                </h2>

                <!-- Địa chỉ -->
                <div class="flex items-start mb-3 sm:mb-4 gap-3 group" v-if="getSettingValue('RestaurantAddress')">
                    <span
                        class="material-symbols-outlined text-emerald-500 mt-1 group-hover:text-emerald-600 transition-colors"
                    >
                        location_on
                    </span>
                    <p class="text-gray-700 leading-relaxed text-sm sm:text-base">
                        {{ getSettingValue('RestaurantAddress') ?? 'Địa chỉ nhà hàng' }}
                    </p>
                </div>

                <!-- Số điện thoại -->
                <div class="flex items-center mb-4 sm:mb-6 gap-3 group" v-if="getSettingValue('RestaurantPhoneNumber')">
                    <span
                        class="material-symbols-outlined text-emerald-500 group-hover:text-emerald-600 transition-colors"
                    >
                        call
                    </span>
                    <p class="text-gray-700 font-medium text-sm sm:text-base">
                        {{ getSettingValue('RestaurantPhoneNumber') }}
                    </p>
                </div>
                <p class="text-gray-600 mb-6 sm:mb-8 leading-relaxed text-sm sm:text-base">
                    <span v-if="getSettingValue('RestaurantSlogan')">{{ getSettingValue('RestaurantSlogan') }}</span>
                    <span v-else>
                        Chúng tôi phục vụ những món ăn ngon nhất với giá cả hợp lý. Không gian ấm cúng, dịch vụ chuyên
                        nghiệp và hương vị đặc trưng sẽ mang đến cho bạn trải nghiệm ẩm thực tuyệt vời.
                    </span>
                </p>
                <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
                    <VBtn class="bg-emerald-500 text-white" rounded prepend-icon="mdi-book-open-variant-outline" @click="viewRestaurantMenu">XEM THỰC ĐƠN</VBtn>
                    <VBtn class="text-emerald-600 border-2 border-emerald-500" rounded prepend-icon="mdi-seat-outline" @click="bookReservation">ĐẶT BÀN</VBtn>
                </div>
                <div class="mt-6 sm:mt-8 flex sm:justify-start gap-4">
                    <a
                        target="_blank"
                        v-for="socialMedia in socialMediaLinks"
                        :href="socialMedia.Link"
                        :class="[
                            {'hover:text-blue-600' : socialMedia.Name === 'facebook'},
                            {'hover:text-red-600' : socialMedia.Name === 'youtube'},
                            {'hover:text-pink-600' : socialMedia.Name === 'instagram'},
                            {'hover:text-black' : socialMedia.Name === 'tiktok'},
                            'text-gray-500 transition-colors duration-300 transform hover:scale-110'
                        ]"
                    >
                        <i :class="[
                            'fa-brands text-xl sm:text-2xl',
                            `fa-${socialMedia.Name}`
                        ]"></i>
                    </a>
                </div>
            </div>
        </main>
        <section class="bg-white py-8 sm:py-12 px-4 sm:px-8 md:h-300">
            <h3 class="text-xl sm:text-2xl font-semibold text-center mb-6 sm:mb-8 text-gray-800">Giờ mở cửa</h3>
            <div class="flex flex-col sm:flex-row flex-wrap justify-center gap-4 sm:gap-6 max-w-3xl mx-auto">
                <div
                    v-for="openingTime in openingTimes"
                    class="bg-emerald-50 p-4 sm:p-5 rounded-btn shadow-sm hover:shadow-md transition-shadow duration-300 w-full sm:w-[200px] text-center transform hover:translate-y-[-4px]"
                >
                    <p class="font-medium text-gray-800">{{ formatDaysOfWeek(openingTime.Days) }}</p>
                    <p class="text-gray-600">{{ $commonFunction.formatTime(openingTime.StartTime) }} - {{ $commonFunction.formatTime(openingTime.EndTime) }}</p>
                </div>
            </div>
        </section>
    </VContainer>
</template>

<script lang="ts">
export default {
    methods: {
        getSettingValue(settingKey: string):any {
            return this.$session.Settings?.find(s => s.SettingKey === settingKey)?.Value;
        },

        viewRestaurantMenu() {
            this.$emit('viewRestaurantMenu');
        },

        bookReservation() {
            this.$emit('bookReservation');
        },

        formatDaysOfWeek(daysArray:number[]) {
            if (!daysArray || daysArray.length === 0) {
                return "";
            }

            const dayNames = ["Chủ nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Ngày lễ"];
            daysArray.sort((a, b) => {
                if (a === 0) {
                    return 1;
                }
                return a - b;
            });

            const result = [];
            let rangeStart = null;

            for (let i = 0; i < daysArray.length; i++) {
                if (rangeStart === null) {
                    rangeStart = daysArray[i];
                }

                if (i + 1 === daysArray.length || daysArray[i + 1] !== daysArray[i] + 1) {
                    if (rangeStart === daysArray[i]) {
                        result.push(dayNames[rangeStart]);
                    } else {
                        result.push(`${dayNames[rangeStart]} - ${dayNames[daysArray[i]]}`);
                    }
                    rangeStart = null;
                }
            }

            return result.join(", ");
        }
    },

    computed: {
        introImageUrl() {
            const introImageUrl = this.getSettingValue('IntroImageUrl');
            return introImageUrl ? this.$commonFunction.getImageUrl(introImageUrl) : 'https://images.unsplash.com/photo-1712630514718-3830cc6c0d0a?w=400&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8cmVzdGF1cmFudCUyMGJhY2tncm91bmR8ZW58MHx8MHx8fDA%3D';
        },

        socialMediaLinks() {
            const setting = this.getSettingValue('SocialMediaUrls');
            if (setting) {
                return JSON.parse(setting);
            }
            return [];
        },

        openingTimes() {
            const setting = this.getSettingValue('OpeningTimes');
            if (setting) {
                return JSON.parse(setting);
            }
            return [];
        }
    }
}
</script>

<style lang="scss" scoped>
</style>