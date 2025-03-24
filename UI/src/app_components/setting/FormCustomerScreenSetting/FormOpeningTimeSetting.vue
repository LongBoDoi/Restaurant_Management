<template>
    <VCard class="pa-6 ml-1 mr-1 bg-gray-50" style="border-radius: 24px;">
        <b style="font-size: 18px;">Giờ mở cửa</b>

        <VDataTable
            class="main-table mt-4"
            :items="lstOpeningTimes"
            color="white"
            hide-default-footer
            no-data-text=""
            :headers="[
                {
                    title: 'Thời gian',
                    width: 275
                },
                {
                    title: 'Thứ Hai',
                    align: 'center'
                },
                {
                    title: 'Thứ Ba',
                    align: 'center'
                },
                {
                    title: 'Thứ Tư',
                    align: 'center'
                },
                {
                    title: 'Thứ Năm',
                    align: 'center'
                },
                {
                    title: 'Thứ Sáu',
                    align: 'center'
                },
                {
                    title: 'Thứ Bảy',
                    align: 'center'
                },
                {
                    title: 'Chủ Nhật',
                    align: 'center'
                },
                {
                    title: 'Ngày lễ',
                    align: 'center'
                },
                {
                    title: '',
                    width: 90
                }
            ]"
        >
        <template v-slot:item="{ item }">
              <tr v-if="item !== null">
                <td>
                    <MLHbox align="center">
                        <MLTimeField variant="outlined" style="width: 100px;" :hide-details="true" v-model="item.StartTime" compact />
                        <span class="ml-4 mr-4">-</span>
                        <MLTimeField variant="outlined" style="width: 100px;" :hide-details="true" v-model="item.EndTime" compact />
                    </MLHbox>
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Monday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Tuesday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Wednesday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Thursday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Friday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Saturday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Sunday" />
                </td>
                <td>
                    <VCheckbox hide-details color="primary" class="ckb-day-of-week" v-model:model-value="item.Days" :value="$enumeration.EnumDayOfWeek.Holiday" />
                </td>
                <td>
                    <VBtn variant="flat" icon="mdi-trash-can-outline" style="background-color: transparent; color: red;" @click="lstOpeningTimes.splice(lstOpeningTimes.indexOf(item), 1)" />
                </td>
              </tr>
            </template>
        </VDataTable>

        <VBtn rounded variant="plain" style="color: #2196F3; text-transform: none; background-color: transparent;" prepend-icon="mdi-plus-circle-outline" class="mt-2" @click="addNewOpeningTime">Thêm giờ</VBtn>
    </VCard>
</template>

<script lang="ts">
import { EnumDayOfWeek } from '@/common/Enumeration';
import { Setting } from '@/models';
import { PropType } from 'vue';

interface OpeningTime {
    StartTime: Date,
    EndTime: Date,
    Days: EnumDayOfWeek[]
}

export default {
    props: {
        setting: {
            type: Object as PropType<Setting>
        }
    },

    data() {
        return {
            lstOpeningTimes: <OpeningTime[]>[]
        }
    },

    watch: {
        setting() {
            if (this.setting && this.setting.Value) {
                this.lstOpeningTimes = JSON.parse(this.setting.Value);
            }
        }
    },

    methods: {
        prepareData() {
            if (this.setting) {
                this.setting.SettingValue = JSON.stringify(this.lstOpeningTimes);
            }
        },

        /**
         * Thêm dòng mới
         */
        addNewOpeningTime() {
            this.lstOpeningTimes.push({
                StartTime: new Date('2000-01-01 00:00'),
                EndTime: new Date('2000-01-01 23:59'),
                Days: []
            } as OpeningTime);
        },
    }
}
</script>

<style lang="scss" scoped>
:deep() {
    .ckb-day-of-week .v-selection-control__wrapper {
        width: 100%;
    }

    .main-table, .main-table th {
        background-color: transparent !important;
    }
}
</style>