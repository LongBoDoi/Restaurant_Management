<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition duration-300 col-span-2" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-bold">Xu hướng doanh thu</h3>
            <div class="flex gap-2">
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === 0 ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="timeFilter = 0"
                >
                    Ngày
                </button>
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === 1 ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="timeFilter = 1"
                >
                    Tuần
                </button>
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === 2 ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="timeFilter = 2"
                >
                    Tháng
                </button>
            </div>
        </div>
        <div class="w-full" style="height: 300px;" v-if="!loading">
            <v-chart :option="option" class="revenue-chart" />
        </div>
    </VCard>
</template>

<script lang="ts">
import VChart from 'vue-echarts';
import * as echarts from 'echarts/core'
import { LineChart } from 'echarts/charts'
import { TitleComponent, TooltipComponent, GridComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers';
import moment from 'moment';

interface RevenueData {
    Revenue: number,
    FromDate: Date,
    ToDate: Date
}

export default {
    components: {
        VChart
    },

    beforeCreate() {
        echarts.use([LineChart, TitleComponent, TooltipComponent, GridComponent, LegendComponent, CanvasRenderer]);
    },

    created() {
        this.getData();
    },

    data() {
        return {
            loading: <boolean>false,
            timeFilter: <number>0,
            data: <RevenueData[]>[],

            
        }
    },

    methods: {
        async getData() {
            this.loading = true;

            this.data = await this.$service.DashboardService.getRevenueTrend(this.timeFilter);

            this.loading = false;
        }
    },

    watch: {
        timeFilter() {
            this.getData();
        }
    },

    computed: {
        xAsisData() {
            switch (this.timeFilter) {
                case 0:
                    return this.data.map(d => moment(d.FromDate).format('DD/MM/YY'));
                case 1:
                    return this.data.map(d => `${moment(d.FromDate).format('DD/MM/YY')} - ${moment(d.ToDate).add(-1, 'days').format('DD/MM/YY')}`);
                case 2:
                    return this.data.map(d => `${moment(d.FromDate).format('MM/YY')}`);
            }
        },

        option() {
            return {
                tooltip: { trigger: 'axis' },
                xAxis: {
                    type: 'category',
                    data: this.xAsisData
                },
                yAxis: { type: 'value' },
                series: [
                    {
                        name: 'Doanh thu',
                        type: 'line',
                        data: this.data.map(d => d.Revenue),
                        smooth: true
                    }
                ]
            }
        }
    }
}
</script>

<style lang="scss" scoped>
</style>