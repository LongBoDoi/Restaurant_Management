<template>
    <v-chart :option="option" class="revenue-chart" />
</template>

<script lang="ts">
import VChart from 'vue-echarts';
import * as echarts from 'echarts/core'
import { LineChart } from 'echarts/charts'
import { TitleComponent, TooltipComponent, GridComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'

export default {
    components: {
        VChart
    },

    beforeCreate() {
        echarts.use([LineChart, TitleComponent, TooltipComponent, GridComponent, LegendComponent, CanvasRenderer]);
    },

    data() {
        return {
            option: {
                tooltip: { trigger: 'axis' },
                xAxis: {
                    type: 'category',
                    data: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
                },
                yAxis: { type: 'value' },
                series: [
                    {
                        name: 'Sales',
                        type: 'line',
                        data: [150, 230, 224, 218, 135],
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