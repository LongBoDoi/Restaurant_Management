<template>
    <MLVbox>
        <div class="grid grid-cols-2 gap-6 mb-6 w-full">
            <div class="">
                <label class="block text-sm font-medium text-gray-700 mb-2">Bàn</label>
                <VCombobox
                    class="mt-1"
                    variant="outlined"
                    density="compact"
                    color="primary"

                    item-title="TableName"
                    item-value="TableID"
                    :items="lstTablesFiltered"
                    :multiple="true"
                    :model-value="selectedTables"
                    :item-props="(item:Table) => {
                        return {
                            title: item.TableName,
                            subtitle: `${item.Area ? `${item.Area.AreaName} - ` : ''}${item.SeatCount} ghế`
                        }
                    }"
                    v-on:update:model-value="(v:Table[]) => {
                        record.TableName = v.map((table: Table) => table.TableName).join(', ');
                        record.OrderTables = v.map((table:Table) => {
                            return {
                                OrderID: record.OrderID,
                                TableID: table.TableID,
                                Table: table
                            } as OrderTable;
                        })
                    }"
                />
            </div>

            <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Khách hàng</label>
                <VCombobox
                    color="primary"
                    class="mt-1"
                    ref="cbMenuCategory"
                    variant="outlined"
                    density="compact"
                    item-title="CustomerName"
                    item-value="CustomerID"
                    :items="lstCustomers"
                    hide-details
                    placeholder="Nhập tên khách hàng..."
                    v-model:model-value="selectedCustomer"
                />
            </div>
        </div>

        <div class="grid grid-cols-2 gap-6 mb-6">
            <!-- Các món đã chọn -->
            <div class="mb-6">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-lg font-semibold text-gray-800">Món order</h3>
                </div>

                <div class="bg-gray-50 rounded-lg p-4 mb-4 overflow-y-auto" style="min-height: 350px;">
                    <div 
                        v-for="orderDetail in record.OrderDetails"
                        class="grid grid-cols-12 gap-4 border-b pb-4 mb-4"
                    >
                        <div class="col-span-7">
                            <div class="flex items-center gap-3">
                                <img
                                    v-if="orderDetail.MenuItem?.ImageUrl"
                                    :src="$commonFunction.getImageUrl(orderDetail.MenuItem?.ImageUrl)"
                                    class="w-12 h-12 object-cover rounded-md shadow-sm"
                                />
                                <div v-else class="bg-gray-200 w-12 h-12 rounded-md flex items-center justify-center">
                                    <VIcon icon="mdi-food" class="text-gray-400" />
                                </div>
                                <div>
                                    <p class="font-medium">{{ orderDetail.MenuItem?.Name ?? orderDetail.MenuItemName }}</p>
                                    <p class="text-sm text-gray-500">{{ $commonFunction.formatThousands(orderDetail.Amount) }} đ</p>
                                </div>
                            </div>
                        </div>

                        <div class="col-span-3">
                            <label class="block text-sm font-medium text-gray-700">Số lượng</label>
                            <MLNumberField
                                density="compact"
                                variant="outlined"
                                hide-spin-buttons
                                hide-details
                                class="w-full mt-1 bg-white border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-400 transition duration-200"
                                v-model:model-value="orderDetail.Quantity"
                                :rules="[(v: number) => v > 0]"
                                v-on:update:model-value="(v:any) => {
                                    orderDetail.Amount = v * orderDetail.Price;
                                }"
                            />
                        </div>

                        <div class="col-span-2 flex items-end justify-center">
                            <VBtn variant="text" icon="mdi-trash-can-outline" class="text-red-500 hover:text-red-700 p-2 rounded-lg hover:bg-red-50 transition duration-150"
                                @click="record.OrderDetails?.splice(record.OrderDetails.indexOf(orderDetail), 1)"
                            />
                        </div>
                    </div>
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700">
                        Yêu cầu đặc biệt
                    </label>
                    <VTextarea
                        no-resize
                        hide-details
                        variant="outlined"
                        placeholder="Nhập yêu cầu đặc biệt hoặc ghi chú..."
                        rows="3"
                        class="mt-1 w-full bg-white border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-400 transition duration-200"
                        v-model:model-value="record.SpecialRequest"
                        color="primary"
                    />
                </div>

                <div class="bg-gray-50 rounded-lg p-4 mt-4">
                    <h4 class="font-medium mb-3"><b>Tạm tính</b></h4>
                    <div class="space-y-2 mb-4">
                        <div class="flex justify-between">
                            <span class="text-gray-600">Tiền hàng:</span>
                            <span class="font-medium">{{ $commonFunction.formatThousands(orderNetAmount) }} đ</span>
                        </div>
                        <div class="flex justify-between" v-if="false">
                            <span class="text-gray-600">Tax (8%):</span>
                            <span class="font-medium">$3.76</span>
                        </div>
                        <div class="flex justify-between" v-if="record.DiscountAmount">
                            <span class="text-gray-600">Khuyến mại:</span>
                            <span class="font-medium">- {{ $commonFunction.formatThousands(record.DiscountAmount) }} đ</span>
                        </div>
                        <div class="flex justify-between align-center">
                            <span class="text-gray-600">Tip:</span>
                            <div class="relative flex align-center" style="width: 128px;">
                                <MLNumberField
                                    variant="outlined"
                                    density="compact"
                                    hide-details
                                    style="color: black;"
                                    suffix="đ"
                                    class="text-right border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-400 transition duration-200"
                                    v-model="record.TipAmount"
                                    color="primary"
                                    :rules="[(v: number) => v >= 0]"
                                />
                            </div>
                        </div>
                        <div class="border-t pt-2 mt-2">
                            <div class="flex justify-between font-bold">
                                <span>Tổng tiền:</span>
                                <span>{{ $commonFunction.formatThousands(orderTotalAmount) }} đ</span>
                            </div>
                        </div>
                    </div>

                    <div v-if="record.CustomerID && record.Customer">
                        <div className="flex items-center gap-2 mt-4">
                            <span className="material-symbols-outlined text-purple-600">
                                loyalty
                            </span>
                            <span>Điểm tích luỹ của khách hàng</span>

                            <span class="ml-auto">{{ record.Customer.LoyaltyPoint }} điểm</span>
                        </div>

                        <div class="mt-2 ml-4 flex items-center">
                            <VCheckbox
                                hide-details
                                color="primary"
                                v-model="earnPointForCustomer"
                                v-on:update:model-value="(v: boolean|null) => {
                                    if (v) {
                                        useCustomerPoint = false;

                                        record.DiscountAmount = 0;
                                        pointUsed = 0;
                                    }
                                }"
                            >
                                <template #label>
                                    <VIcon icon="mdi-wallet-plus-outline" class="text-yellow-600 mr-2" />
                                    Tích điểm cho khách hàng
                                </template>
                            </VCheckbox>

                            <i class="text-gray-500 ml-auto" v-if="earnPointForCustomer">{{ pointEarned }} điểm</i>
                        </div>

                        <div class="mt-2 ml-4 flex items-center">
                            <VCheckbox
                                hide-details
                                color="primary"
                                v-model="useCustomerPoint"
                                v-on:update:model-value="(v: boolean|null) => {
                                    if (v) {
                                        earnPointForCustomer = false;

                                        record.PointEarnedForCustomer = 0;
                                    }
                                }"
                            >
                                <template #label>
                                    <div>
                                        <div class="flex items-center">
                                            <VIcon icon="mdi-wallet-giftcard" class="text-green-600 mr-2" />
                                            Sử dụng điểm tích luỹ&nbsp;&nbsp;
                                            <i class="text-sm text-gray-500 mr-1">(1 điểm = 10đ)</i>
                                        </div>
                                    </div>
                                </template>
                            </VCheckbox>

                            <MLNumberField
                                v-if="useCustomerPoint"
                                style="max-width: 100px;"
                                hide-details
                                variant="outlined"
                                density="compact"
                                color="primary"
                                class="ml-auto text-right"
                                :rules="[(v: number) => v >= 0 && v <= (record.Customer?.LoyaltyPoint ?? 0) && v * 10 <= record.NetAmount]"
                                v-model="pointUsed"
                                v-on:update:model-value="(v: number) => {
                                    if (v >= 0 && v <= (record.Customer?.LoyaltyPoint ?? 0) && v * 10 <= record.NetAmount) {
                                        record.DiscountAmount = v * 10;
                                    }
                                }"
                            />
                        </div>
                    </div>

                    <div class="flex justify-between gap-2 mt-4">
                        <div class="relative flex-1" v-if="false">
                            <select class="w-full bg-white border border-gray-300 rounded-lg px-4 py-3 appearance-none focus:outline-none focus:ring-2 focus:ring-primary-400 transition duration-200">
                                <option value="cash">Cash</option>
                                <option value="credit">Credit Card</option>
                                <option value="debit">Debit Card</option>
                            </select>
                            <span class="material-symbols-outlined absolute right-3 top-3 pointer-events-none text-gray-500">
                                expand_more
                            </span>
                        </div>

                        <VBtn rounded prepend-icon="mdi-cash-multiple" class="bg-secondary ml-auto hover:bg-yellow-600 text-white transition duration-200 shadow-md hover:shadow-lg transform hover:-translate-y-1"
                            :disabled="!record.OrderDetails?.length"
                            style="color: white !important;"
                            @click="handlePaymentClick"
                        >
                            Thanh toán
                        </VBtn>
                    </div>
                </div>
            </div>

            <div style="flex-grow: 1; height: 100%; display: flex; flex-direction: column;">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-lg font-semibold text-gray-800">Thực đơn</h3>
                    <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" max-width="225" hide-details placeholder="Tìm kiếm tên món..." v-model:model-value="txtSearch" color="primary" />
                </div>
                <!-- Nhóm món -->
                <VSlideGroup>
                    <VSlideGroupItem
                        v-for="category, index in lstMenuItemCategoriesFiltered"
                        :key="index"
                    >
                        <v-btn
                            variant="outlined"
                            class="mx-2 border-gray-300"
                            :style="{
                                'background-color': category.MenuItemCategoryID === selectedMenuCategory ? category.Color : 'white',
                                'color': category.MenuItemCategoryID === selectedMenuCategory ? 'white' : category.Color
                            }"
                            rounded
                            @click="selectedMenuCategory = category.MenuItemCategoryID"
                        >
                            {{ category.MenuItemCategoryName }}
                        </v-btn>
                    </VSlideGroupItem>
                </VSlideGroup>

                <VDivider />

                <!-- Danh sách món -->
                <div class="grid grid-cols-2 qhd:grid-cols-3 gap-4 pb-4 mt-4">
                    <div
                        v-for="menuItem in lstMenuItemsFiltered"
                        @click="handleMenuItemClick(menuItem)"
                        class="bg-white rounded-lg shadow-sm border border-gray-200 pa-3 hover:shadow-md transition duration-200 cursor-pointer group"
                    >
                        <div class="relative">
                            <img
                                v-if="menuItem.ImageUrl"
                                :src="$commonFunction.getImageUrl(menuItem.ImageUrl)"
                                class="w-full object-cover rounded-md"
                                style="aspect-ratio: 16/9;"
                            />
                            <div v-else class="w-full rounded-md bg-gray-200 flex items-center justify-center" style="aspect-ratio: 16/9;">
                                <VIcon icon="mdi-food" class="text-gray-400" :size="48" />
                            </div>
                            <div
                                class="absolute inset-0 bg-black bg-opacity-0 rounded-md group-hover:bg-opacity-20 transition-all duration-200 flex items-center justify-center"
                            >
                                <VBtn
                                    icon="mdi-plus"
                                    class="bg-emerald-600 text-white p-2 rounded-full transform scale-0 group-hover:scale-100 transition-all duration-200"
                                />
                            </div>
                        </div>
                        <h4 data-id="PG2w" class="font-medium mt-2">
                            {{ menuItem.Name }}
                        </h4>
                        <div data-id="bIEP" class="flex justify-between items-center">
                            <p data-id="ftj9" class="text-sm text-gray-500">
                                {{ menuItem.Description }}
                            </p>
                            <p data-id="48MY" class="font-bold">
                                {{ $commonFunction.formatThousands(menuItem.Price) }} đ
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </MLVbox>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { Customer, MenuItem, MenuItemCategory, Order, Table } from '@/models';
import OrderDetail from '@/models/OrderDetail';
import OrderTable from '@/models/OrderTable';
import { PropType } from 'vue';

export default {
    props: {
        record: {
            type: Object as PropType<Order>,
            required: true
        },

        editMode: {
            type: Number,
            required: true
        },

        lstCustomers: {
            type: Object as PropType<Customer[]>,
            default: []
        },

        lstMenuItemCategories: {
            type: Object as PropType<MenuItemCategory[]>,
            default: []
        },

        lstMenuItems: {
            type: Object as PropType<MenuItem[]>,
            default: []
        },
        
        lstTables: {
            type: Object as PropType<Table[]>,
            default: []
        },

        reservedTables: {
            type: Object as PropType<Table[]>,
            default: []
        }
    },

    created() {
        this.selectedCustomer = this.record.Customer ?? this.record.CustomerName;
    },

    data() {
        return {
            selectedCustomer: <Customer|string|undefined>undefined,
            selectedMenuCategory: <string>this.$commonValue.GuidEmpty,

            txtSearch: <string>'',
            earnPointForCustomer: <boolean>true,
            useCustomerPoint: <boolean>false,

            pointUsed: <number>0
        }
    },

    computed: {
        lstMenuItemsFiltered():MenuItem[] {
            return this.lstMenuItems.filter(mi => 
                (this.selectedMenuCategory === this.$commonValue.GuidEmpty || mi.MenuItemCategoryID === this.selectedMenuCategory) 
                && mi.Name.toLowerCase().includes(this.txtSearch.toLowerCase())
            );
        },

        lstMenuItemCategoriesFiltered() {
            return [{
                MenuItemCategoryID: this.$commonValue.GuidEmpty,
                MenuItemCategoryName: 'Tất cả',
                Color: 'rgb(16, 185, 129)'
            } as MenuItemCategory].concat(this.lstMenuItemCategories);
        },

        lstTablesFiltered() {
            return this.lstTables.filter(t => t.Status === this.$enumeration.EnumTableStatus.Available).concat(this.reservedTables);
        },

        selectedTables():Table[] {
            return this.record.OrderTables?.map(ot => ot.Table).filter(t => t !== undefined) ?? [];
        },

        orderNetAmount() {
            return this.record.OrderDetails?.reduce((sum, orderDetail) => sum + orderDetail.Amount, 0) ?? 0;
        },

        orderTotalAmount() {
            return this.orderNetAmount + (this.record.TipAmount ?? 0) - (this.record.DiscountAmount ?? 0);
        },

        pointEarned() {
            if (this.earnPointForCustomer) {
                return this.orderNetAmount / 1000;
            }

            if (this.useCustomerPoint) {
                return this.pointUsed * -1;
            }

            return 0;
        }
    },

    methods: {
        /**
         * Xử lý khi click vào món ăn
         */
         handleMenuItemClick(menuItem: MenuItem) {
            let existDetail:OrderDetail|undefined = this.record.OrderDetails.find(od => od.MenuItemID === menuItem.MenuItemID);
            if (!existDetail) {
                existDetail = {
                    MenuItemID: menuItem.MenuItemID,
                    MenuItemName: menuItem.Name,
                    Quantity: 0,
                    Price: menuItem.Price,

                    MenuItem: menuItem
                } as OrderDetail;
                this.record.OrderDetails.push(existDetail);
            }

            existDetail.Quantity++;
            existDetail.Amount = existDetail.Quantity * existDetail.Price;
        },

        /**
         * Xử lý click vào nút Thanh toán
         */
        handlePaymentClick() {
            EventBus.emit(this.$eventName.ShowFormPayment, this.record);
        }
    },

    watch: {
        selectedCustomer(customer: Customer|string) {
            if (customer) {
                if (typeof(customer) === 'object') {
                    this.record.CustomerID = customer.CustomerID;
                    this.record.CustomerName = customer.CustomerName;
                    this.record.Customer = customer as Customer;
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

        orderNetAmount() {
            this.record.NetAmount = this.orderNetAmount;
        },

        orderTotalAmount() {
            this.record.TotalAmount = this.orderTotalAmount;
        },

        pointEarned() {
            this.record.PointEarnedForCustomer = this.pointEarned;
        }
    },
}
</script>