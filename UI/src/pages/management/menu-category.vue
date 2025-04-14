<!-- Màn quản lý thực đơn -->

<template>
    <VContainer style="padding: unset;" class="flex flex-column">
        <h2 className="text-2xl font-bold text-gray-800">Nhóm thực đơn</h2>

        <FormMenuCategoryList class="h-full" />
    </VContainer>

    <FormMenuCategoryDetail />
</template>

<script lang="ts">
export default {
}
</script>

<style lang="scss" scoped>
</style>