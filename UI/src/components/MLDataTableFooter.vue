<template>
    <div class="px-6 py-4 bg-gray-50 border-t flex items-center justify-between">
        <div class="text-gray-600">{{ recordNumberText }}</div>
        <div class="flex align-center">
            <span class="text-gray-600">Số bản ghi</span>

            <VSelect
                class="ml-4" 
                style="width: 100px;" 
                density="compact" 
                variant="outlined" 
                hide-details 
                :items="[10, 25, 50, 100]"
                v-model:model-value="options.itemsPerPage"
            />

            <VBtn icon="mdi-skip-previous" variant="outlined" class="text-gray-600 ml-10" :width="40" :height="40" :disabled="options.page <= 1" @click="options.page = 1;" />
            <VBtn icon="mdi-chevron-left" variant="outlined" class="text-gray-600 ml-2" :width="40" :height="40" style="" :disabled="options.page <= 1" @click="options.page--;" />

            <VTextField type="number" hide-spin-buttons style="width: 72px;" class="ml-2 text-center" density="compact" variant="outlined" hide-details :model-value="options.page" 
                :rules="[(v:string) => {
                    return parseInt(v) > 0 && parseInt(v) <= lastPage;
                }]"
                @blur="onPageNumberBlur"
                @keypress.enter="onPageNumberBlur"
            />

            <VBtn icon="mdi-chevron-right" variant="outlined" class="text-gray-600 ml-2" :width="40" :height="40" :disabled="options.page >= lastPage" @click="options.page++;" />
            <VBtn icon="mdi-skip-next" variant="outlined" class="text-gray-600 ml-2" :width="40" :height="40" :disabled="options.page >= lastPage" @click="options.page = lastPage;" />
        </div>
    </div>
</template>

<script lang="ts">
export default {
    props: {
        totalCount: {
            type: Number,
            required: true,
            default: 0
        },

        options: {
            type: Object as any,
            required: true,
            default: {
                page: 1,
                itemsPerPage: 10
            }
        }
    },

    methods: {
        onPageNumberBlur(event: any) {
            const pageNum = parseInt(event.target.value);
            if (pageNum > 0 && pageNum <= Math.ceil(this.totalCount / this.options.itemsPerPage)) {
                this.options.page = pageNum;
            }
        },
    },

    computed: {
        recordNumberText():string {
            if (this.totalCount === 0) {
                return 'Hiển thị 0-0 trên 0 bản ghi';
            }

            const startIndex:number = (this.options.page - 1) * (this.options.itemsPerPage) + 1;
            const endIndex:number = Math.min(startIndex + this.options.itemsPerPage - 1, this.totalCount);
            return `Hiển thị ${startIndex}-${endIndex} trên ${this.totalCount} bản ghi`;
        },

        lastPage():number {
            return Math.ceil(this.totalCount / this.options.itemsPerPage);
        },
    }
}
</script>