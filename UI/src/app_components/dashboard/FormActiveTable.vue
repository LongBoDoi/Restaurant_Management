<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-green-500" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between h-full space-x-3">
            <div>
                <p class="text-gray-500 text-sm">Số lượng bàn hoạt động</p>
                <div v-if="!loading">
                    <h3 class="text-2xl font-bold mt-1">{{ occupiedTables }}</h3>
                    <p class="text-gray-500 text-sm mt-1 flex items-center">
                        {{ occupancyRate }}
                    </p>
                </div>
            </div>
            <div class="bg-green-100 rounded-full pa-3 text-green-600">
                <span class="material-symbols-outlined text-3xl">table_restaurant</span>
            </div>
        </div>
    </VCard>
</template>

<script lang="ts">
interface Data {
    AllTablesCount: number,
    OccupiedTablesCount: number,
    OccupancyRate: number
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

        const objectData = await this.$service.DashboardService.getActiveTables();
        this.data = objectData;

        this.loading = false;
    },

    computed: {
        occupiedTables() {
            if (this.data) {
                const allTables = this.data.AllTablesCount ? this.$commonFunction.formatThousands(this.data.AllTablesCount) : 0;
                const occupiedTables = this.data.OccupiedTablesCount ? this.$commonFunction.formatThousands(this.data.OccupiedTablesCount) : 0;
                return `${occupiedTables}/${allTables}`;
            }
            return '0/0';
        },

        occupancyRate() {
            if (this.data && this.data.OccupancyRate !== null) {
                return `${this.data.OccupancyRate}% tỉ lệ hoạt động`;
            }

            return '';
        }
    }
}
</script>