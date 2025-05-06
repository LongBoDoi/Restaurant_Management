<template>
    <div class="flex flex-col">
        <div class="flex align-center mb-6" v-if="editMode !== $enumeration.EnumEditMode.Add">
            <label class="block text-gray-700 text-sm font-medium">Trạng thái:</label>
            <span class="px-2.5 py-1 ml-2 rounded-full text-xs font-medium"
                :class="reservationStatusClass"
            >
                {{ reservationStatus }}
            </span>
        </div>

        <div class="grid grid-cols-2 gap-4 mb-4">
            <!-- Khách hàng -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Khách hàng <span style="color: red;">*</span></label>
                <VCombobox
                    class="mt-1"
                    ref="cbMenuCategory"
                    variant="outlined"
                    density="compact"
                    color="primary"

                    item-title="CustomerName"
                    item-value="CustomerID"
                    :items="lstCustomers"
                    hide-details
                    v-model:model-value="selectedCustomer"
                    :rules="[(v:any) => {
                        return v !== null;
                    }]"
                />
            </div>
            
            <!-- Số điện thoại -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Số điện thoại <span style="color: red;">*</span></label>
                <VTextField class="mt-1" density="compact" variant="outlined" hide-details
                    v-mask="'0### ### ###'"
                    :rules="[$commonValue.textFieldRequireRule]"
                    :model-value="record.CustomerPhoneNumber"
                    color="primary"
                    v-on:update:model-value="(v:string) => {
                        record.CustomerPhoneNumber = $commonFunction.getRealPhoneNumberValue(v);
                    }"
                />
            </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
            <!-- Ngày -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Ngày <span style="color: red;">*</span></label>
                <MLDateField
                    class="mt-1"
                    variant="outlined"
                    compact
                    color="primary"

                    :hide-details="false"
                    v-model:model-value="record.ReservationDate"
                    required
                    :min-date="receivingTable ? undefined : currentDate"
                />
            </div>
            
            <!-- Giờ -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Giờ <span style="color: red;">*</span></label>
                <MLTimeField
                    class="mt-1"
                    variant="outlined"
                    compact
                    hide-details
                    color="primary"
                    
                    v-model:model-value="record.ReservationDate"
                    required
                    :min-date="receivingTable ? undefined : currentDate"
                />
            </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
            <!-- Số người -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Số người</label>
                <MLHbox class="align-center space-x-1">
                    <VBtn icon="mdi-minus" variant="text" class="mt-1" @click="record.GuestCount--" />
                    <MLNumberField
                        style="max-width: 80px;"
                        class="mt-1 text-center"
                        variant="outlined"
                        density="compact"
                        hide-details
                        color="primary"

                        v-model:model-value="record.GuestCount"
                        required
                        :rules="[(v: number) => v > 0]"
                    />
                    <VBtn icon="mdi-plus" variant="text" class="mt-1" @click="record.GuestCount++" />
                </MLHbox>
            </div>

            <!-- Bàn -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Bàn <span style="color: red;">*</span></label>
                <VCombobox
                    class="mt-1"
                    variant="outlined"
                    density="compact"
                    color="primary"

                    item-title="TableName"
                    item-value="TableID"
                    :items="lstTablesFiltered"
                    :multiple="true"
                    chips
                    :model-value="selectedTables"
                    :rules="[(v:Table[]) => {
                        if (!v?.length) {
                            return false;
                        }

                        const totalSeatCount = v.reduce((v, table) => v + table.SeatCount, 0) ?? 0;
                        return totalSeatCount >= record.GuestCount || 'Không đủ ghế';
                    }]"
                    :item-props="(item:Table) => {
                        return {
                            title: item.TableName,
                            subtitle: `${item.SeatCount} ghế`
                        }
                    }"
                    v-on:update:model-value="(v:any) => {
                        record.TableName = v.map((table:Table) => table.TableName).join(', ');
                        record.ReservationTables = v.map((table:Table) => {
                            return {
                                ReservationID: record.ReservationID,
                                TableID: table.TableID,
                                Table: table
                            } as ReservationTable;
                        })
                    }"
                />
            </div>
        </div>

        <div>
            <!-- Yêu cầu đặc biệt -->
            <div>
                <label class="block text-gray-700 text-sm font-medium">Yêu cầu đặc biệt</label>
                <VTextarea
                    class="mt-1"
                    v-model:model-value="record.Note"
                    variant="outlined"
                    hide-details
                    density="compact"
                    no-resize
                    color="primary"
                />
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { EnumEditMode, EnumReservationStatus } from '@/common/Enumeration';
import { Customer, Reservation, Table } from '@/models';
import ReservationTable from '@/models/ReservationTable';
import { PropType } from 'vue';

export default {
    props: {
        editMode: {
            type: Number as PropType<EnumEditMode>
        },

        record: {
            type: Object as PropType<Reservation>,
            required: true
        },

        lstCustomers: {
            type: Object as PropType<Customer[]>,
            default: []
        },

        lstTables: {
            type: Object as PropType<Table[]>,
            default: []
        },

        reservedTables: {
            type: Object as PropType<Table[]>,
            default: []
        },

        receivingTable: {
            type: Boolean,
            default: false
        }
    },

    data() {
        return {
            selectedCustomer: <Customer|string|undefined>undefined
        }
    },

    created() {
        this.selectedCustomer = this.record.Customer ?? this.record.CustomerName;
    },

    computed: {
        /**
         * Tên trạng thái đặt bàn
         */
         reservationStatus() {
            switch (this.record.Status) {
                case EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumReservationStatus.Confirmed:
                    return 'Đã xác nhận';
                case EnumReservationStatus.Canceled:
                    return 'Đã huỷ'
            }
        },

        reservationStatusClass() {
            switch (this.record.Status) {
                case EnumReservationStatus.Pending:
                    return 'bg-yellow-100 text-yellow-700';
                case EnumReservationStatus.Confirmed:
                    return 'bg-blue-100 text-blue-800';
                case EnumReservationStatus.Canceled:
                    return 'bg-red-100 text-red-700';
            }
        },

        lstTablesFiltered() {
            return this.lstTables.filter(t => t.Status === this.$enumeration.EnumTableStatus.Available).concat(this.reservedTables);
        },

        selectedTables() {
            const result:Table[] = [];

            this.record.ReservationTables?.forEach((rt:ReservationTable) => {
                const table:Table|undefined = this.lstTables.find(t => t.TableID === rt.TableID);
                if (table) {
                    result.push(table);
                }
            });

            return result;
        },
        
        currentDate() {
            return new Date();
        },
    },

    watch: {
        selectedCustomer(customer: Customer|string) {
            if (customer) {
                if (typeof(customer) === 'object') {
                    this.record.CustomerID = customer.CustomerID;
                    this.record.CustomerName = customer.CustomerName;
                    this.record.CustomerPhoneNumber = customer.PhoneNumber;
                } else {
                    this.record.CustomerID = undefined;
                    this.record.Customer = undefined;
                    this.record.CustomerName = customer;
                }
            } else {
                this.record.CustomerID = undefined;
                this.record.Customer = undefined;
                this.record.CustomerName = '';
            }
        },
    }
}
</script>