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
                        <p class="text-xs" :class="`text-${data.Trend >= 0 ? 'green' : 'red'}-600`" v-if="data.Trend !== null">{{ data.Trend }}%</p>
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
                            <p class="text-xs" :class="`text-${data.Trend >= 0 ? 'green' : 'red'}-600`" v-if="data.Trend !== null">{{ data.Trend }}%</p>
                        </div>
                    </div>
                </div>
            </VExpandTransition>
        </div>
    </VCard>
</template>

<script lang="ts">
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

            data: <Data[]>[]
        }
    },

    async created() {
        this.loading = true;

        const objectData = await this.$service.DashboardService.getPopularMenuItems();
        this.data = objectData;

        this.loading = false;
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