<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-yellow-500" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between h-full space-x-3">
            <div>
                <p class="text-gray-500 text-sm">Giá trị order trung bình</p>
                <div v-if="!loading">
                    <h3 class="text-2xl font-bold mt-1">{{ averageOrderValue }} đ</h3>
                    <p class="text-sm mt-1 flex items-center" :class="trendColorClass">
                        <span class="material-symbols-outlined text-sm mr-1">{{ trendIcon }}</span>
                        {{ revenueTrend }}
                    </p>
                </div>
            </div>
            <div class="bg-yellow-100 rounded-full pa-3 text-yellow-600">
                <span class="material-symbols-outlined text-3xl">shopping_cart</span>
            </div>
        </div>
    </VCard>
</template>

<script lang="ts">
interface Data {
    AverageOrderValue: number,
    Trend: number
}

export default {
    data() {
        return {
            loading: false,

            data: <Data>{}
        }
    },

    async created() {
        this.loading = true;

        const objectData = await this.$service.DashboardService.getAverageOrderValue();
        this.data = objectData;

        this.loading = false;
    },

    computed: {
        averageOrderValue() {
            if (this.data && this.data.AverageOrderValue) {
                return this.$commonFunction.formatThousands(this.data.AverageOrderValue);
            }
            return 0;
        },

        revenueTrend() {
            if (this.data && this.data.Trend !== null) {
                if (this.data.Trend >= 0) {
                    return `+${this.data.Trend}% so với hôm qua`;
                } else {
                    return `${this.data.Trend}% so với hôm qua`
                }
            }

            return '';
        },

        trendIcon() {
            if (this.data && this.data.Trend !== null) {
                if (this.data.Trend >= 0) {
                    return 'trending_up';
                } else {
                    return 'trending_down';
                }
            }

            return '';
        },

        trendColorClass() {
            if (this.data && this.data.Trend !== null) {
                if (this.data.Trend >= 0) {
                    return 'text-green-600';
                } else {
                    return 'text-red-500';
                }
            }

            return '';
        }
    }
}
</script>