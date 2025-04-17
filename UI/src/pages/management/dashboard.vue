<template>
    <VContainer style="padding: unset;" class="flex flex-column">
        <h2 class="text-2xl font-bold text-gray-800">Tổng quan</h2>

        <MLVbox style="overflow-y: auto;" class="mt-6 px-1">
            <!-- Dòng 1 -->
            <div class="grid grid-cols-4 gap-6 mb-6">
                <FormTodayRevenue />

                <FormCompletedOrders />

                <FormAverageOrderValue />

                <FormActiveTable />
            </div>

            <!-- Dòng 2 -->
            <div class="grid grid-cols-3 gap-6 mb-6">
                <FormRevenueTrend />

                <FormPopularMenuItems />
            </div>

            <!-- Dòng 3 -->
            <div class="grid grid-cols-2 gap-6 mb-6">
                <FormRecentOrders />

                <FormReservationToday />
            </div>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
export default {
    
}
</script>