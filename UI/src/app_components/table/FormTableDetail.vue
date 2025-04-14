<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 className="text-white text-xl font-bold">Bàn</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <!-- Tên nguyên liệu -->
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-medium">Tên bàn <span style="color: red;">*</span></label>
                        
                        <VTextField class="mt-2" density="compact" variant="outlined" v-model:model-value="record.TableName" hide-details
                            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        />
                    </div>

                    <div className="mb-4">
                        <!-- Số ghế -->
                        <div>
                            <label className="block text-gray-700 text-sm font-medium">Số ghế</label>
                            <MLNumberField :config="$commonValue.moneyConfig" class="mt-2 text-right" hide-spin-buttons density="compact" variant="outlined" v-model:model-value="record.SeatCount" hide-details />
                        </div>
                    </div>

                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-medium">
                            Khu vực
                        </label>
                        
                        <VSelect
                            :items="lstArea"
                            item-title="AreaName"
                            item-value="AreaID"
                            v-model:model-value="record.AreaID"
                            v-on:update:model-value="(value: string) => {
                                const area = lstArea.find(x => x.AreaID === value);
                                if (area) {
                                    record.Area = area;
                                }
                            }"
                            no-data-text="Không có dữ liệu"

                            density="compact"
                            variant="outlined"
                            hide-details
                            class="mt-2"
                        />
                    </div>

                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-medium">
                            Trạng thái
                        </label>
                        
                        <VSelect
                            :items="lstTableStatus"
                            item-title="Text"
                            item-value="Value"
                            v-model:model-value="record.Status"
                            no-data-text="Không có dữ liệu"

                            density="compact"
                            variant="outlined"
                            hide-details
                            class="mt-2"
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
import { Area, MLActionResult, Table } from '@/models';
import { tableStore } from '@/stores/tableStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormTableDetail, this.handleShowDialog as any);


    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormTableDetail);
    },

    methods: {
        ...mapActions(tableStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: Table) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                if (this.editMode === this.$enumeration.EnumEditMode.Add) {
                    record.Status = this.$enumeration.EnumTableStatus.Available;
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
            
            const actionResult:MLActionResult = await this.$service.TableService.saveChanges(this.record);
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
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <Table>{} as Table,
            oldRecord: <Table>{} as Table,

            lstArea: <Area[]>[]
        }
    },

    computed: {
        ...mapState(tableStore, ['dataList', 'selectedIndex']),

        subData() {
            return [
                async () => {
                    this.lstArea = await this.$service.AreaService.getAll();
                }
            ]
        },

        lstTableStatus() {
            return [
                {
                    Text: 'Còn trống',
                    Value: this.$enumeration.EnumTableStatus.Available
                },
                {
                    Text: 'Hết chỗ',
                    Value: this.$enumeration.EnumTableStatus.Occupied
                },
                {
                    Text: 'Đã đặt chỗ',
                    Value: this.$enumeration.EnumTableStatus.Reserved
                }
            ]
        }
    }
}
</script>