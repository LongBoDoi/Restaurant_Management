<template>
    <VContainer style="padding: unset;" class="flex flex-column">
        <h2 class="text-2xl font-bold text-gray-800">Báo cáo</h2>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VSelect
                    :items="[
                        {
                            Text: 'Hôm nay',
                            Value: $enumeration.EnumTimeFilter.ByDay
                        },
                        {
                            Text: 'Tuần này',
                            Value: $enumeration.EnumTimeFilter.ByWeek
                        },
                        {
                            Text: 'Tháng này',
                            Value: $enumeration.EnumTimeFilter.ByMonth
                        },
                        {
                            Text: 'Năm nay',
                            Value: $enumeration.EnumTimeFilter.ByYear
                        },
                        {
                            Text: 'Tuỳ chọn',
                            Value: $enumeration.EnumTimeFilter.Custom
                        },
                    ]"
                    item-title="Text"
                    item-value="Value"
                    v-model:model-value="timeFilter"
                    v-on:update:model-value="handleSelectTimeFilter"

                    color="primary"
                    density="compact"
                    variant="outlined"
                    hide-details
                    style="max-width: 200px;"
                />

                <VExpandXTransition>
                    <MLHbox class="space-x-4" v-if="timeFilter === 4">
                        <MLDateField
                            variant="outlined"
                            compact
                            hide-details
                            label="Từ"
                            color="primary"
                            style="width: 170px;"
                            v-model="fromDate"
                        />
                        
                        <MLDateField
                            variant="outlined"
                            compact
                            hide-details
                            label="Đến"
                            color="primary"
                            style="width: 170px;"
                            v-model="toDate"
                        />

                        <VBtn rounded class="bg-green-500 hover:bg-green-600 text-white" @click="applyCustomTimeFilters">Áp dụng</VBtn>
                    </MLHbox>
                </VExpandXTransition>
            </div>

            <MLVbox style="overflow-y: auto; height: 100%;" class="py-4 px-6 bg-white">
                <!-- Dòng 1 -->
                <div class="grid grid-cols-4 gap-6 mb-6">
                    <FormTotalRevenue />

                    <FormTotalOrders />

                    <FormAverageOrderValue />

                    <FormActiveTable />
                </div>

                <!-- Dòng 2 -->
                <div class="grid grid-cols-3 gap-6 mb-6">
                    <FormRevenueTrend />

                    <FormPopularMenuItems />
                </div>

                <!-- Dòng 3 -->
                <div class="grid grid-cols-2 gap-6">
                    <FormRecentOrders />

                    <FormReservationToday />
                </div>
            </MLVbox>
        </VCard>
    </VContainer>
</template>

<script lang="ts">
import { EnumTimeFilter } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import moment from 'moment';

export default {
    data() {
        return {
            timeFilter: <EnumTimeFilter>this.$enumeration.EnumTimeFilter.ByDay,
            fromDate: <string|undefined>undefined,
            toDate: <string|undefined>undefined,
        }
    },

    mounted() {
        this.handleSelectTimeFilter(this.timeFilter);
    },

    methods: {
        handleSelectTimeFilter(filter: number) {
            if (filter === 4) {
                return;
            }

            var fromDate = '';
            switch (filter) {
                case 0:
                    fromDate = moment().startOf('days').format('YYYY-MM-DD HH:mm:ss');
                    break;
                case 1:
                    fromDate = moment().startOf('weeks').add(1, 'days').format('YYYY-MM-DD HH:mm:ss');
                    break;
                case 2:
                    fromDate = moment().startOf('months').format('YYYY-MM-DD HH:mm:ss');
                    break;
                case 3:
                    fromDate = moment().startOf('years').format('YYYY-MM-DD HH:mm:ss');
                    break;
            }
            
            const toDate = moment().format('YYYY-MM-DD HH:mm:ss');

            EventBus.emit(this.$eventName.LoadReportData, {
                TimeFilter: this.timeFilter,
                FromDate: fromDate,
                ToDate: toDate
            })
        },

        applyCustomTimeFilters() {
            EventBus.emit(this.$eventName.LoadReportData, {
                TimeFilter: this.timeFilter,
                FromDate: moment.utc(this.fromDate).local().format('YYYY-MM-DD HH:mm:ss'),
                ToDate: moment.utc(this.toDate).local().endOf('days').format('YYYY-MM-DD HH:mm:ss')
            });
        }
    }
}
</script>