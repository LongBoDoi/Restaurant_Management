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
                        timeFilter === $enumeration.EnumTimeFilter.ByDay ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="() => {
                        handleTimeFilterButtonClick($enumeration.EnumTimeFilter.ByDay);
                    }"
                >
                    Ngày
                </button>
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === $enumeration.EnumTimeFilter.ByWeek ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="() => {
                        handleTimeFilterButtonClick($enumeration.EnumTimeFilter.ByWeek);
                    }"
                >
                    Tuần
                </button>
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === $enumeration.EnumTimeFilter.ByMonth ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="() => {
                        handleTimeFilterButtonClick($enumeration.EnumTimeFilter.ByMonth);
                    }"
                >
                    Tháng
                </button>
                <button class="px-3 py-1 text-sm rounded-md transition duration-150"
                    :class="[
                        timeFilter === $enumeration.EnumTimeFilter.ByYear ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-emerald-100 hover:bg-emerald-200 text-emerald-700'
                    ]"
                    @click="() => {
                        handleTimeFilterButtonClick($enumeration.EnumTimeFilter.ByYear);
                    }"
                >
                    Năm
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
import { EnumTimeFilter } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';

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
        EventBus.on(this.$eventName.LoadReportData, this.handleTimeFilterEvent);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.LoadReportData);
    },

    data() {
        return {
            loading: <boolean>false,
            timeFilter: <EnumTimeFilter>this.$enumeration.EnumTimeFilter.ByDay,
            data: <RevenueData[]>[],

            fromDate: <string>'',
            toDate: <string>'',
            isCustomTimeRange: <boolean>false,
        }
    },

    methods: {
        async getData() {
            this.loading = true;

            this.data = await this.$service.DashboardService.getRevenueTrend(this.fromDate, this.toDate, this.timeFilter);

            this.loading = false;
        },

        handleTimeFilterButtonClick(value: EnumTimeFilter) {
            if (value !== this.timeFilter) {
                this.timeFilter = value;

                if (!this.isCustomTimeRange) {
                    this.configDateRangeOnFilter(value)
                }

                this.getData();
            }
        },

        handleTimeFilterEvent(data: any) {
            this.isCustomTimeRange = false;
            if (data.TimeFilter === this.$enumeration.EnumTimeFilter.Custom) {
                this.fromDate = data.FromDate;
                this.toDate = data.ToDate;
                this.isCustomTimeRange = true;
            } else {
                this.configDateRangeOnFilter(data.TimeFilter);
            }

            this.getData()
        },

        configDateRangeOnFilter(timeFilter: EnumTimeFilter) {
            this.toDate = moment().format('YYYY-MM-DD HH:mm:ss');
            switch (timeFilter) {
                case this.$enumeration.EnumTimeFilter.ByDay:
                    this.fromDate = moment().startOf('days').add(-1, 'months').format('YYYY-MM-DD HH:mm:ss');
                    this.timeFilter = timeFilter;
                    break;
                case this.$enumeration.EnumTimeFilter.ByWeek:
                    this.fromDate = moment().startOf('weeks').add(1, 'days').add(-9, 'weeks').format('YYYY-MM-DD HH:mm:ss');
                    this.timeFilter = timeFilter;
                    break;
                case this.$enumeration.EnumTimeFilter.ByMonth:
                    this.fromDate = moment().startOf('months').add(-11, 'months').format('YYYY-MM-DD HH:mm:ss');
                    this.timeFilter = timeFilter;
                    break;
                case this.$enumeration.EnumTimeFilter.ByYear:
                    this.fromDate = moment().startOf('years').add(-5, 'years').format('YYYY-MM-DD HH:mm:ss');
                    this.timeFilter = timeFilter;
                    break;
            }
        }
    },

    computed: {
        xAsisData() {
            switch (this.timeFilter) {
                case this.$enumeration.EnumTimeFilter.ByDay:
                    return this.data.map(d => moment(d.FromDate).format('DD/MM/YY'));
                case this.$enumeration.EnumTimeFilter.ByWeek:
                    return this.data.map(d => `${moment(d.FromDate).format('DD/MM/YY')} - ${moment(d.ToDate).add(-1, 'seconds').format('DD/MM/YY')}`);
                case this.$enumeration.EnumTimeFilter.ByMonth:
                    return this.data.map(d => `${moment(d.FromDate).format('MM/YY')}`);
                case this.$enumeration.EnumTimeFilter.ByYear:
                    return this.data.map(d => `${moment(d.FromDate).format('YYYY')}`);
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