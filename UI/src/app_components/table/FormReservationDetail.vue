<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 className="text-white text-xl font-bold">Đặt chỗ</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <FormReservationSummary v-if="record.Status === $enumeration.EnumReservationStatus.Canceled || record.Status === $enumeration.EnumReservationStatus.Received" :record="record" />
                    <FormReservationEdit v-else :edit-mode="editMode" :record="record" :lst-customers="lstCustomers" :lst-tables="lstTables" :reserved-tables="reservedTables" :receiving-table="receivingTable" />
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end">
                <MLHbox v-if="record.Status === $enumeration.EnumReservationStatus.Canceled">
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-4" rounded @click="isShow = false">Đóng</VBtn>
                </MLHbox>
                <MLHbox v-else>
                    <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                    <VBtn 
                        variant="tonal" 
                        class="bg-green-500 hover:bg-green-600 text-white ml-4" 
                        rounded
                        
                        @click="() => {
                            const saveObject = Object.assign({}, record);
                            saveObject.Status = $enumeration.EnumReservationStatus.Confirmed;
                            handleSaveClick(saveObject);
                        }"
                        v-if="record.Status === $enumeration.EnumReservationStatus.Pending"
                    >
                        Xác nhận
                    </VBtn>
                    <VBtn 
                        variant="tonal" 
                        class="bg-yellow-500 hover:bg-yellow-600 text-white ml-4" 
                        rounded
                        
                        @click="handleReceiveTableClick"
                        v-if="record.Status === $enumeration.EnumReservationStatus.Confirmed && editMode === $enumeration.EnumEditMode.Edit"
                    >
                        Nhận bàn
                    </VBtn>
                    <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-4" rounded @click="handleSaveClick(record)" v-if="record.Status === $enumeration.EnumReservationStatus.Confirmed">Lưu</VBtn>
                </MLHbox>
            </VCardActions>
        </VCard>
    </VDialog>

    <FormOrderDetail v-on:order-created="handleCreateOrderFromReservation" v-on:payment-success="handleCreateOrderFromReservation" />
</template>

<script lang="ts">
import { EnumEditMode, EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Reservation, MLActionResult, Customer, Table, Order } from '@/models';
import OrderTable from '@/models/OrderTable';
import { reservationStore } from '@/stores/reservationStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormReservationDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormReservationDetail);
    },

    methods: {
        ...mapActions(reservationStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: Reservation) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                if (this.editMode === this.$enumeration.EnumEditMode.Add) {
                    record.GuestCount = 1;
                    record.Status = EnumReservationStatus.Confirmed;

                    const reservationDate = (moment().add(1, 'hours') as any)._d as Date;
                    reservationDate.setMinutes(0);
                    reservationDate.setSeconds(0);
                    record.ReservationDate = reservationDate;

                    this.reservedTables = record.ReservationTables?.map(rt => rt.Table).filter(x => x !== undefined) ?? [];
                }
                
                this.record = record;
                Object.assign(this.oldRecord, this.record);

                this.loadAllSubData();

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
        async handleSaveClick(record: Reservation) {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;
            
            const actionResult:MLActionResult = await this.$service.ReservationService.saveChanges(record);
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

        /**
         * Xử lý nhận bàn
         */
        async handleReceiveTableClick() {
            const order:Order = {
                EditMode: this.$enumeration.EnumEditMode.Add,
                CustomerName: this.record.CustomerName,
                CustomerID: this.record.CustomerID,
                Customer: this.record.Customer,
                SpecialRequest: this.record.Note,
                TableName: this.record.ReservationTables?.map(rt => this.lstTables.find(t => t.TableID === rt.TableID)?.TableName).join(', '),
                OrderTables: this.record.ReservationTables?.map(rt => {
                    return {
                        TableID: rt.TableID,
                        Table: this.lstTables.find(t => t.TableID === rt.TableID)
                    } as OrderTable;
                })
            } as Order;
            EventBus.emit(this.$eventName.ShowFormOrderDetail, order);
        },

        /**
         * Xử lý tạo order hoặc thanh toán thành công khi nhận bàn
         */
        async handleCreateOrderFromReservation() {
            this.receivingTable = true;

            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            const saveObject = Object.assign({}, this.record);
            saveObject.EditMode = this.$enumeration.EnumEditMode.Edit;
            saveObject.Status = this.$enumeration.EnumReservationStatus.Received;
            
            const actionResult:MLActionResult = await this.$service.ReservationService.saveChanges(saveObject);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: 'Nhận bàn thành công.',
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

            record: <Reservation>{} as Reservation,
            oldRecord: <Reservation>{} as Reservation,

            lstCustomers: <Customer[]>[],

            lstTables: <Table[]>[],
            reservedTables: <Table[]>[],
            receivingTable: <boolean>false
        }
    },

    computed: {
        ...mapState(reservationStore, ['dataList', 'selectedIndex']),

        subData() {
            return [
                async () => {
                    if (this.editMode === this.$enumeration.EnumEditMode.Edit && this.record.ReservationID) {
                        const data:Reservation|undefined = await this.$service.ReservationService.getByID(this.record.ReservationID);
                        if (data) {
                            Object.assign(this.record, data);
                            Object.assign(this.oldRecord, data);
                            this.reservedTables = data.ReservationTables?.map(rt => rt.Table).filter(x => x !== undefined) ?? [];

                            this.record.EditMode = this.editMode;
                        }
                    }
                },
                async () => {
                    const lstCustomers:Customer[] = await this.$service.CustomerService.getAll();
                    this.lstCustomers = lstCustomers;
                },
                async () => {
                    this.lstTables = await this.$service.TableService.getAll();
                },
            ]
        },
    },
}
</script>