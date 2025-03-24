<!-- Màn quản lý thực đơn -->

<template>
    <VContainer style="padding: unset;" class="flex flex-column">
        <h2 className="text-2xl font-bold text-gray-800">Thực đơn</h2>

        <VTabs color="primary" v-model:model-value="tab" class="flex-shrink-0 mt-4">
            <VTab :value="0">THỰC ĐƠN</VTab>
            <VTab :value="1">NHÓM THỰC ĐƠN</VTab>
        </VTabs>

        <VTabsWindow :model-value="tab" class="mt-4 h-full">
            <VTabsWindowItem :value="0" class="h-full">
                <FormMenuList class="h-full" />
            </VTabsWindowItem>
        </VTabsWindow>
    </VContainer>

    <FormMenuDetail />
</template>

<script lang="ts">
export default {
    data() {
        return {
            tab: <number>0
        }
    },

    methods: {
        
    }
}
</script>

<style lang="scss" scoped>
</style>