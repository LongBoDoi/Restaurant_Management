<template>
    <VCard class="bg-emerald-100 rounded-xl p-4 sm:p-8 shadow-md relative overflow-hidden mt-8" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div
            style="margin-top: -40px; margin-right: -40px; top: 0; right: 0;"
            class="absolute top-0 right-0 w-32 h-32 bg-emerald-400 rounded-full opacity-30"
        ></div>
        <div
            style="margin-bottom: -32px; margin-left: -32px;"
            class="absolute bottom-0 left-0 w-24 h-24 -mb-8 -ml-8 bg-emerald-200 rounded-full opacity-30"
        ></div>

        <div
            class="relative z-10 flex flex-col items-center sm:items-start text-center sm:text-left"
        >
            <h3 data-id="K3jw" class="text-2xl font-bold mb-6">
                Tạo cuộn sushi của riêng bạn
            </h3>

            <div v-if="!saveSuccess" class="w-full">
                <div v-if="!startCreating">
                    <p class="mb-6 text-gray-700">
                        Chọn những nguyên liệu yêu thích của bạn và để các đầu bếp sushi của chúng tôi tạo ra một cuộn sushi riêng dành cho bạn!
                    </p>
                    <VBtn
                        @click="startCreating = true;"
                        prepend-icon="mdi-pencil-outline"
                        class="bg-emerald-600 hover:bg-emerald-700 text-white font-semibold py-2 sm:py-3 px-4 sm:px-6 rounded-lg transition-all duration-300 transform hover:scale-105 shadow-md hover:shadow-lg flex items-center mt-2"
                        v-if="isCustomerLoggedIn"
                    >
                        Bắt đầu tạo
                    </VBtn>

                    <span v-else>
                        <RouterLink class="text-emerald-600" style="text-decoration: underline;" :to="{ name: '//login' }">Đăng nhập ngay</RouterLink> để bắt đầu.
                    </span>
                </div>

                <VExpandTransition>
                    <div class="w-full" v-if="startCreating">
                        <div class="mb-2" v-if="customMenuRequest.InventoryItems?.length === 0">
                            <i style="color: red;">Vui lòng chọn ít nhất 01 loại nguyên liệu</i>
                        </div>

                        <VForm ref="form">
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8 w-full">
                            <VCard v-for="category in lstInventoryItemCategories" class="bg-white rounded-lg shadow pa-4 hover:shadow-lg transition-shadow duration-300 border border-emerald-100"
                                style="max-height: 19rem;"
                            >
                                <h4 class="font-semibold text-lg border-b border-emerald-200 pb-2 mb-2">
                                    {{ category.InventoryItemCategoryName }}
                                </h4>
                                <div>
                                    <VCheckbox
                                        v-for="inventoryItem in category.InventoryItems"
                                        v-model:model-value="customMenuRequest.InventoryItems"
                                        :label="inventoryItem.Name"
                                        :value="inventoryItem"

                                        color="primary"
                                        hide-details
                                    />
                                </div>
                            </VCard>
                        </div>

                        <div
                            class="bg-white rounded-lg shadow p-4 hover:shadow-lg transition-shadow duration-300 border border-emerald-100 mb-8"
                        >
                            <h4 data-id="lrIA" class="font-semibold text-lg border-b border-emerald-200 pb-2 mb-3">
                                Đặt tên sushi của bạn
                            </h4>
                            <VTextField hide-details variant="outlined" placeholder="Nhập tên cho cuộn sushi theo ý bạn..." :rules="[$commonValue.textFieldRequireRule]" v-model:model-value="customMenuRequest.MenuItemName" />
                        </div>

                        <MLHbox class="gap-4">
                            <VSpacer />
                            <VBtn class="bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold transition-all duration-300" rounded @click="resetSelectedInventoryItems" >Đặt lại</VBtn>
                            <VBtn class="bg-emerald-600 hover:bg-emerald-700 text-white font-semibold transition-all duration-300 hover:scale-105" rounded prepend-icon="mdi-check-circle" @click="handleCreateCustomSushi">Tạo</VBtn>
                        </MLHbox>
                        </VForm>
                    </div>
                </VExpandTransition>
            </div>
            
            <div v-else>
                <div class="bg-emerald-200 border-l-4 border-emerald-500 p-4 rounded-md animate-fadeIn">
                    <div class="flex items-center">
                        <VIcon icon="mdi-check" class="text-emerald-500 mr-2" />
                        <p class="font-semibold text-emerald-800">
                            Thành công! Yêu cầu của bạn đã được tạo.
                        </p>
                    </div>
                    <p class="mt-2 text-emerald-700 text-sm ml-8">
                        Chúng tôi sẽ kiểm tra và xác nhận thông tin. Bạn sẽ được thông báo nếu cuộn sushi của bạn được thêm vào thực đơn của chúng tôi.
                    </p>
                    <div class="mt-4 ml-8">
                        <VBtn class="bg-white hover:bg-gray-100 text-emerald-600 font-semibold py-2 px-4 rounded-lg transition-all duration-300 border border-emerald-500 hover:border-emerald-600"
                            @click="saveSuccess = false; startCreating = true; customMenuRequest.MenuItemName = ''; customMenuRequest.InventoryItems = [];"
                        >
                            Tạo cuộn sushi khác
                        </VBtn>
                    </div>
                </div>
            </div>
        </div>
    </VCard>
</template>

<script lang="ts">
import { Customer, InventoryItem, MLActionResult } from '@/models';
import CustomMenuRequest from '@/models/CustomMenuRequest';
import InventoryItemCategory from '@/models/InventoryItemCategory';

export default {
    data() {
        return {
            startCreating: <boolean>false,
            
            lstInventoryItemCategories: <InventoryItemCategory[]>[],

            loading: <boolean>false,
            customMenuRequest: <CustomMenuRequest>{
                CustomerID: (this.$session.UserData as Customer).CustomerID,
                CustomerName: (this.$session.UserData as Customer).CustomerName,
                InventoryItems: [] as InventoryItem[]
            } as CustomMenuRequest,
            saveSuccess: <boolean>false
        }
    },

    computed: {
        isCustomerLoggedIn() {
            return this.$session.UserType === this.$enumeration.EnumUserType.Customer && this.$session.UserData !== undefined;
        },
    },

    async created() {
        this.lstInventoryItemCategories = await this.$service.InventoryItemCategoryService.getInventoryItemForCustomerCreate();
    },

    methods: {
        resetSelectedInventoryItems() {
            this.customMenuRequest.InventoryItems = [];
        },

        async handleCreateCustomSushi() {
            const form = this.$refs.form as any;
            if (!(await form?.validate()).valid) {
                return;
            }
            
            if (!this.customMenuRequest.InventoryItems?.length) {
                return;
            }
            
            this.loading = true;

            this.customMenuRequest.EditMode = this.$enumeration.EnumEditMode.Add;
            const actionResult:MLActionResult = await this.$service.CustomMenuRequestService.saveChanges(this.customMenuRequest);
            if (actionResult.Success) {
                this.saveSuccess = true;
            }

            this.loading = false;
        }
    }
}
</script>