<template>
    <VMenu :close-on-content-click="false" v-model:model-value="isShow">
        <template v-slot:activator="{ props }">
            <VBtn
                rounded variant="outlined"
                style="border-color: rgba(0, 0, 0, 0.38);
                background-color: white;"
                prepend-icon="mdi-sort-variant"
                v-bind="props"
                :color="isShow ? 'primary' : ''"
            >
                Sắp xếp
            </VBtn>
        </template>

        <VCard class="rounded-lg">
            <div class="flex justify-between items-center pa-4 pb-2 border-b">
                <h3 class="font-medium text-gray-800 font-bold">Sắp xếp</h3>
            </div>
            
            <VList :selected="selectedValues" color="primary" @update:selected="handleApplySort">
                <VListItem v-for="sortData, index in items" link :value="index" v-slot="{ isActive }">
                    <MLHbox class="items-center space-x-1">
                        <VIcon :icon="isActive ? 'mdi-check' : 'mdi'" class="text-green-500" />
                        <span>{{ sortData.Text }}</span>
                    </MLHbox>
                </VListItem>
            </VList>
        </VCard>
    </VMenu>
</template>

<script lang="ts">
import MLSortCondition from '@/models/MLSortCondition';
import { PropType } from 'vue';

export default {
    props: {
        items: {
            type: Object as PropType<MLSortCondition[]>,
            default: []
        },

        modelValue: {
            type: String
        }
    },

    created() {
        if (this.items.length) {
            this.selectedValues = [0];
            this.$emit('update:modelValue', JSON.stringify([this.items[0]]));
        }
    },

    data() {
        return {
            selectedValues: <number[]>[0],

            isShow: <boolean>false
        }
    },

    methods: {
        handleApplySort(selectedValues: number[]) {
            if (!selectedValues.length) {
                return;
            }

            this.selectedValues = selectedValues;
            const sortConditionIndex = selectedValues[0];

            const selectedSort:MLSortCondition = this.items[sortConditionIndex];
            this.$emit('update:modelValue', JSON.stringify([selectedSort]));
        }
    }
}
</script>