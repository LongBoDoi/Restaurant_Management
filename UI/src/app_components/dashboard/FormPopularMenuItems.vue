<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition duration-300" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-bold">Món ăn phổ biến</h3>
            <button class="text-emerald-600 hover:text-emerald-700 flex items-center gap-1 text-sm transition duration-150" @click="expanded=!expanded" v-if="expandItems.length">
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="expanded">chevron_left</span>
                {{ expanded ? 'Thu gọn' : 'Xem tất cả' }}
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="!expanded">chevron_right</span>
            </button>
        </div>

        <div class="text-center" v-if="!data.length">
            Không có dữ liệu.
        </div>

        <div v-else>
            <div class="space-y-4">
                <div v-for="data in displayItems" class="flex items-center gap-3 pa-2 hover:bg-gray-100 rounded-lg transition duration-150">
                    <img
                        v-if="data.MenuItem.ImageUrl"
                        :src="$commonFunction.getImageUrl(data.MenuItem.ImageUrl)"
                        class="w-12 h-12 object-cover rounded-md"
                    />
                    <div v-else class="bg-gray-200 w-12 h-12 rounded-md flex items-center justify-center">
                        <VIcon icon="mdi-food" class="text-gray-400" :size="16" />
                    </div>
                    <div class="flex-1">
                        <h4 class="font-medium">{{ data.MenuItem.Name }}</h4>
                        <p class="text-sm text-gray-500">Đã gọi {{ data.Count }} lần</p>
                    </div>
                    <div class="text-right">
                        <p class="font-bold">{{ $commonFunction.formatThousands(data.MenuItem.Price) }} đ</p>
                        <p class="text-xs flex" :class="`text-${data.Trend >= 0 ? 'green' : 'red'}-600`" v-if="data.Trend !== null">
                            <span class="material-symbols-outlined text-sm mr-1">{{ getTrendIcon(data.Trend) }}</span>
                            {{ getTrendText(data.Trend) }}
                        </p>
                    </div>
                </div>
            </div>

            <VExpandTransition>
                <div v-if="expanded" class="space-y-4">
                    <div class="mb-4" />
                    <div v-for="data in expandItems" class="flex items-center gap-3 pa-2 hover:bg-gray-100 rounded-lg transition duration-150">
                        <img
                            v-if="data.MenuItem.ImageUrl"
                            :src="$commonFunction.getImageUrl(data.MenuItem.ImageUrl)"
                            class="w-12 h-12 object-cover rounded-md"
                        />
                        <div v-else class="bg-gray-200 w-12 h-12 rounded-md flex items-center justify-center">
                            <VIcon icon="mdi-food" class="text-gray-400" :size="16" />
                        </div>
                        <div class="flex-1">
                            <h4 class="font-medium">{{ data.MenuItem.Name }}</h4>
                            <p class="text-sm text-gray-500">Đã gọi {{ data.Count }} lần</p>
                        </div>
                        <div class="text-right">
                            <p class="font-bold">{{ $commonFunction.formatThousands(data.MenuItem.Price) }} đ</p>
                            <p class="text-xs" :class="`text-${data.Trend >= 0 ? 'green' : 'red'}-600`" v-if="data.Trend !== null">
                                <span class="material-symbols-outlined text-sm mr-1">{{ getTrendIcon(data.Trend) }}</span>
                                {{ getTrendText(data.Trend) }}
                            </p>
                        </div>
                    </div>
                </div>
            </VExpandTransition>
        </div>
    </VCard>
</template>

<script lang="ts">
import { EnumTimeFilter } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MenuItem } from '@/models';

interface Data {
    MenuItem: MenuItem,
    Count: number,
    Trend: number
}

export default {
    data() {
        return {
            loading: false,
            expanded: <boolean>false,

            data: <Data[]>[],
            timeFilter: <EnumTimeFilter>this.$enumeration.EnumTimeFilter.ByDay
        }
    },

    created() {
        EventBus.on(this.$eventName.LoadReportData, this.loadData);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.LoadReportData);
    },

    methods: {
        async loadData(data: any) {
            this.loading = true;

            this.timeFilter = data.TimeFilter;
            const objectData = await this.$service.DashboardService.getPopularMenuItems(data.FromDate, data.ToDate, data.TimeFilter);
            this.data = objectData;

            this.loading = false;
        },

        getTrendText(trend: number) {
            var compareName = '';
            switch (this.timeFilter) {
                case this.$enumeration.EnumTimeFilter.ByDay:
                    compareName = 'hôm qua';
                    break;
                case this.$enumeration.EnumTimeFilter.ByWeek:
                    compareName = 'tuần trước';
                    break;
                case this.$enumeration.EnumTimeFilter.ByMonth:
                    compareName = 'tháng trước';
                    break;
                case this.$enumeration.EnumTimeFilter.ByDay:
                    compareName = 'năm ngoái';
                    break;
            }

            if (trend !== null) {
                if (trend >= 0) {
                    return `+${trend}% so với ${compareName}`;
                } else {
                    return `${trend}% so với ${compareName}`;
                }
            }

            return '';
        },

        getTrendIcon(trend: number) {
            if (trend !== null) {
                if (trend >= 0) {
                    return 'trending_up';
                } else {
                    return 'trending_down';
                }
            }

            return '';
        }
    },

    computed: {
        displayItems() {
            return this.data.slice(0, 4);
        },

        expandItems() {
            return this.data.slice(4, this.data.length);
        }
    }
}
</script>