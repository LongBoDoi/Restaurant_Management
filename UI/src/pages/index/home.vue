<template>
    <MLVbox class="bg-white h-full">
        <div class="bg-emerald-800 text-white flex-1">
            <VContainer class="relative" style="padding: unset !important;">
                <div class="absolute top-0 right-0 w-1/2 h-full opacity-10">
                    <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg" className="w-full h-full">
                        <path
                            fill="#FFFFFF"
                            d="M47.1,-57.9C59.4,-45.3,67,-28.7,71.1,-10.9C75.2,6.9,75.9,25.9,67.8,41.2C59.7,56.5,42.8,68.1,24.3,73.4C5.8,78.7,-14.4,77.7,-33.2,70.2C-52.1,62.7,-69.7,48.7,-77.9,30.1C-86.1,11.5,-84.8,-11.7,-75.3,-30.3C-65.7,-48.9,-47.9,-62.9,-30.5,-73C-13.1,-83.1,3.9,-89.3,18.4,-83.4C32.9,-77.5,34.9,-70.5,47.1,-57.9Z"
                            transform="translate(100 100)"
                        />
                    </svg>
                </div>

                <div class="container mx-auto px-4 md:px-8 py-12 md:py-20 flex flex-col-reverse md:flex-row items-center gap-8 md:gap-12 relative z-10">
                    <div class="w-full md:w-1/2 space-y-4 md:space-y-6 text-center md:text-left">
                        <h2 class="text-4xl md:text-5xl font-bold leading-tight">{{ getSettingValue('RestaurantName') }}</h2>
                        <!-- <h2 class="text-4xl md:text-5xl font-bold leading-tight">Trải nghiệm ẩm thực Nhật Bản chính hiệu</h2> -->
                        <p class="text-lg text-emerald-100 max-w-md">
                            <span v-if="getSettingValue('RestaurantSlogan')"></span>
                            <span v-else>Thưởng thức sushi và các món Nhật tinh hoa, được chế biến bởi các bậc thầy đầu bếp với những nguyên liệu tươi ngon nhất.</span>
                        </p>
                        <!-- Địa chỉ -->
                        <div class="flex text-start items-start mb-3 sm:mb-4 gap-3 group" v-if="getSettingValue('RestaurantAddress')">
                            <span
                                class="material-symbols-outlined text-emerald-500 transition-colors"
                            >
                                location_on
                            </span>
                            <p class="leading-relaxed text-sm sm:text-base">
                                {{ getSettingValue('RestaurantAddress') }}
                            </p>
                        </div>

                        <!-- Số điện thoại -->
                        <div class="flex items-center mb-4 sm:mb-6 gap-3 group" v-if="getSettingValue('RestaurantPhoneNumber')">
                            <span
                                class="material-symbols-outlined text-emerald-500 transition-colors"
                            >
                                call
                            </span>
                            <p class="font-medium text-sm sm:text-base">
                                {{ getSettingValue('RestaurantPhoneNumber') }}
                            </p>
                        </div>
                        <div class="flex flex-col sm:flex-row gap-3 md:gap-4 pt-2 md:pt-4">
                            <VBtn
                                v-if="$commonFunction.getSettingValue('DisplayBookingScreenForCustomer')"
                                rounded
                                :to="{name: '//reservation'}"
                                className="bg-white text-emerald-800 px-6 py-3 rounded-full font-semibold transition-transform hover:shadow-lg hover:-translate-y-1 active:translate-y-0 inline-block text-center"
                            >
                                Đặt bàn ngay
                            </VBtn>
                            <VBtn
                                v-if="$commonFunction.getSettingValue('DisplayMenuScreenForCustomer')"
                                :to="{name: '//menu'}"
                                className="border border-white px-6 py-3 rounded-full font-semibold transition-transform hover:bg-emerald-700 hover:shadow-lg hover:-translate-y-1 active:translate-y-0 inline-block text-center"
                            >
                                Xem thực đơn
                            </VBtn>
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
                                    'transition-colors duration-300 transform hover:scale-110'
                                ]"
                            >
                                <i :class="[
                                    'fa-brands text-xl sm:text-2xl',
                                    `fa-${socialMedia.Name}`
                                ]"></i>
                            </a>
                        </div>
                    </div>
                    <div class="w-full md:w-1/2 relative md:mt-0">
                        <div class="rounded-2xl overflow-hidden shadow-2xl transform transition-transform hover:scale-105 duration-300">
                            <img
                                :src="introImageUrl"
                                alt="Sushi Platter"
                                class="w-full h-auto"
                            />
                        </div>
                    </div>
                </div>
            </VContainer>
        </div>

        <div class="py-12 md:py-16 px-4 md:px-8 bg-white" v-if="openingTimes.length">
            <VContainer>
                <div class="flex justify-center items-center mb-12">
                    <div class="h-0.5 bg-emerald-200 w-24"></div>
                    <h2 class="text-3xl font-bold px-6 text-center text-emerald-800">Giờ mở cửa</h2>
                    <div class="h-0.5 bg-emerald-200 w-24"></div>
                </div>

                <div class="flex flex-col sm:flex-row flex-wrap justify-center gap-6 md:gap-8 mx-auto">
                    <div v-for="openingTime in openingTimes" class="group relative overflow-hidden rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 bg-emerald-50 w-full md:w-30rem">
                        <div class="p-8">
                            <h3 class="text-xl font-bold text-emerald-800 mb-4">{{ formatDaysOfWeek(openingTime.Days) }}</h3>
                            <div class="flex items-center mb-3 text-emerald-700">
                                <VIcon icon="mdi-clock-outline" />
                                <p class="text-lg ml-2">{{ $commonFunction.formatTime(openingTime.StartTime) }} - {{ $commonFunction.formatTime(openingTime.EndTime) }}</p>
                            </div>
                        </div>
                    </div>

                    <!-- <div class="group relative overflow-hidden rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 bg-emerald-50">
                        <div class="p-8">
                            <h3 class="text-xl font-bold text-emerald-800 mb-4">Friday - Saturday</h3>
                            <div class="flex items-center mb-3">
                                <svg
                                    class="w-6 h-6 text-emerald-600 mr-2"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                >
                                    <path
                                        d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM12 20C7.59 20 4 16.41 4 12C4 7.59 7.59 4 12 4C16.41 4 20 7.59 20 12C20 16.41 16.41 20 12 20ZM12.5 7H11V13L16.2 16.2L17 14.9L12.5 12.2V7Z"
                                        fill="currentColor"
                                    />
                                </svg>
                                <p class="text-lg text-emerald-700">11:00 AM - 11:30 PM</p>
                            </div>
                            <p class="text-emerald-600 mt-4 border-t border-emerald-200 pt-4">
                                Extended hours for weekend dining with special night menu options
                            </p>
                        </div>
                        <div class="absolute top-4 right-4 bg-emerald-600 text-white text-sm font-bold px-3 py-1 rounded-full transform rotate-3 hover:rotate-0 transition-all">
                            Extended Hours
                        </div>
                    </div>

                    <div class="group relative overflow-hidden rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 bg-emerald-50">
                        <div class="p-8">
                            <h3 class="text-xl font-bold text-emerald-800 mb-4">Sunday</h3>
                            <div class="flex items-center mb-3">
                                <svg
                                    class="w-6 h-6 text-emerald-600 mr-2"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                >
                                    <path
                                        d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM12 20C7.59 20 4 16.41 4 12C4 7.59 7.59 4 12 4C16.41 4 20 7.59 20 12C20 16.41 16.41 20 12 20ZM12.5 7H11V13L16.2 16.2L17 14.9L12.5 12.2V7Z"
                                        fill="currentColor"
                                    />
                                </svg>
                                    <p class="text-lg text-emerald-700">12:00 PM - 9:00 PM</p>
                                </div>
                                <p class="text-emerald-600 mt-4 border-t border-emerald-200 pt-4">
                                    Special Sunday menu featuring family-style dining experiences
                                </p>
                            </div>
                            <div class="absolute top-4 right-4 bg-emerald-600 text-white text-sm font-bold px-3 py-1 rounded-full transform rotate-3 hover:rotate-0 transition-all">
                                Sunday Hours
                            </div>
                    </div> -->
                </div>
            </VContainer>
        </div>
    </MLVbox>
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
            return introImageUrl ? this.$commonFunction.getImageUrl(introImageUrl) : 'https://images.unsplash.com/photo-1579871494447-9811cf80d66c?ixlib=rb-1.2.1&auto=format&fit=crop&w=600&q=80';
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
.w-1\/2 {
  width: 50%;
}
</style>