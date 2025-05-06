<template>
    <VMenu :close-on-content-click="false" v-model:model-value="isShow">
        <template v-slot:activator="{ props }">
            <VBtn
                rounded variant="outlined"
                style="border-color: rgba(0, 0, 0, 0.38);
                background-color: white;"
                prepend-icon="mdi-filter-variant"
                v-bind="props"
                :color="isShow ? 'primary' : ''"
            >
                Bộ lọc
                <span
                    v-if="filterCount > 0"
                    style="margin-left: 6px; width: 20px; height: 20px;" 
                    class="flex items-center justify-center bg-green-500 text-white text-xs rounded-full font-medium"
                >
                    {{ filterCount }}
                </span>
            </VBtn>
        </template>

        <VCard class="rounded-lg pa-4" :width="width">
            <div class="flex justify-between items-center mb-3 pb-2 border-b">
                <h3 class="font-medium text-gray-800 font-bold">Bộ lọc</h3>
            </div>

            <MLForm ref="formFilter" >
                <slot />
            </MLForm>

            <VExpandTransition>
                <div v-if="filterFormError" class="mt-4">
                    <i class="text-error text-sm" v-for="error in filterFormError">{{ error }}</i>
                </div>
            </VExpandTransition>

            <div class="flex space-x-2 mt-8">
                <VSpacer />
                <VBtn class="bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg text-sm transition-all duration-200" @click="handleResetFilters">
                    Đặt lại
                </VBtn>
                <VBtn class="bg-green-500 hover:bg-green-600 text-white rounded-lg text-sm transition-all duration-200" @click="handleApplyFilters">
                    Áp dụng
                </VBtn>
            </div>
        </VCard>
    </VMenu>
</template>

<script lang="ts">
import MLFilterCondition from '@/models/MLFilterCondition';

export default {
    emits: ['resetFilter', 'applyFilter'],

    props: {
        filterCount: {
            type: Number,
            required: true
        },

        width: {
            type: [Number, String],
            default: 350
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            lstFilters: <MLFilterCondition[]>[],

            filterFormError: <string>''
        }
    },

    methods: {
        handleResetFilters() {
            this.filterFormError = '';
            this.$emit('resetFilter');
        },

        async handleApplyFilters() {
            const form = this.$refs.formFilter as any;
            const formValid:boolean = (await form.validate());

            if (!formValid) {
                this.filterFormError = form.getErrorMessages()[0];
                return;
            }
            
            this.filterFormError = '';
            this.$emit('applyFilter');
            this.isShow = false;
        }
    }
}
</script>