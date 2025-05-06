<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Khu vực</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <!-- Tên khu vực -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Tên khu vực <span style="color: red;">*</span></label>
                        
                        <VTextField class="mt-2" density="compact" variant="outlined" v-model:model-value="record.AreaName" hide-details
                            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        />
                    </div>
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Lưu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Area, MLActionResult } from '@/models';
import { areaStore } from '@/stores/areaStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormAreaDetail, this.handleShowDialog as any);


    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormAreaDetail);
    },

    methods: {
        ...mapActions(areaStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: Area) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.loadAllSubData();

                this.record = record;
                Object.assign(this.oldRecord, this.record);
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    Object.assign(this.record, this.oldRecord);
                    break;
            }
            this.isShow = false;
        },

        /**
         * Xử lý nhấn vào nút Lưu
         */
        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;
            
            const actionResult:MLActionResult = await this.$service.AreaService.saveChanges(this.record);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: 'Lưu thành công.',
                    Type: 'success'
                });

                this.isShow = false;
            }

            this.loading = false;
        },

        loadAllSubData() {
            if (this.subData.length) {
                this.loading = true;

                var servicesCalled:number = 0;
                this.subData.forEach((subData:any) => {
                    subData().then(() => {
                        servicesCalled++;
                        if (servicesCalled >= this.subData.length) {
                            this.loading = false;
                        }
                    });
                });
            }
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <Area>{} as Area,
            oldRecord: <Area>{} as Area
        }
    },

    computed: {
        ...mapState(areaStore, ['dataList', 'selectedIndex']),

        subData() {
            return []
        },
    }
}
</script>