<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-blue-500" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between h-full space-x-3">
            <div>
                <p class="text-gray-500 text-sm">Số order đã hoàn thành</p>
                <div v-if="!loading">
                    <h3 class="text-2xl font-bold mt-1">{{ completedOrders }}</h3>
                    <p class="text-sm mt-1 flex items-center" :class="trendColorClass">
                        <span class="material-symbols-outlined text-sm mr-1">{{ trendIcon }}</span>
                        {{ revenueTrend }}
                    </p>
                </div>
            </div>
            <div class="bg-blue-100 rounded-full pa-3 text-blue-600">
                <span class="material-symbols-outlined text-3xl">receipt_long</span>
            </div>
        </div>
    </VCard>
</template>

<script lang="ts">
interface Data {
    CompletedOrders: number,
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

        const objectData = await this.$service.DashboardService.getCompletedOrders();
        this.data = objectData;

        this.loading = false;
    },

    computed: {
        completedOrders() {
            if (this.data && this.data.CompletedOrders) {
                return this.$commonFunction.formatThousands(this.data.CompletedOrders);
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