<template>
    <div class="flex flex-col items-center justify-center bg-gray-100 px-4" style="min-height: 100vh;">
        <div class="max-w-xl w-full bg-white rounded-xl shadow-lg overflow-hidden border border-gray-200 text-center py-12 px-6 md:px-12">
            <div class="mb-6 flex justify-center">
                <span class="material-symbols-outlined text-8xl text-amber-500 animate-pulse">search_off</span>
            </div>
            <h1 class="text-4xl font-bold text-gray-800 mb-4">404</h1>
            <h2 class="text-2xl font-semibold text-gray-700 mb-6">Không tìm thấy trang!</h2>
            <p class="text-gray-600 mb-8 max-w-md mx-auto">
                Trang bạn đang tìm kiếm không tồn tại hoặc bạn không có quyền truy cập trang này. Vui lòng kiểm tra lại đường dẫn hoặc quay lại trang chủ.
            </p>
            <div class="flex flex-col sm:flex-row justify-center gap-4">
                <a
                    href="/"
                    class="px-8 py-3 bg-amber-500 text-white rounded-lg font-medium transition-all duration-300 hover:bg-amber-600 transform hover:scale-105 flex items-center justify-center"
                >
                    <span class="material-symbols-outlined mr-2">home</span>
                    Về trang chủ
                </a>
                <button
                    @click="goToPreviousPage"
                    class="px-8 py-3 border border-gray-300 text-gray-700 rounded-lg font-medium transition-all duration-300 hover:bg-gray-50 hover:border-gray-400 flex items-center justify-center"
                >
                    <span class="material-symbols-outlined mr-2">arrow_back</span>
                    Quay lại
                </button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
export default {
    methods: {
        goToPreviousPage() {
            this.$router.back();
        },
    }
}
</script>