<template>
    <MLVbox>
    <MLHbox style="padding: 0.625rem 1rem;">
        <VTextField
            hide-details
            variant="outlined"
            density="compact"
            width="300"
            style="flex-grow: 0;"
            v-model:model-value="record.OrderName"
            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
            placeholder="Số bàn, số tầng,..."
        >
            <template v-slot:label>
                Tên order
                <span style="color: red;">*</span>
            </template>
        </VTextField>

        <VCombobox
            class="flex-grow-0 ml-4"
            v-model:model-value="selectedCustomer"
            :disabled="loadingCustomer"
            ref="cbMenuCategory"
            label="Khách hàng"
            width="250"
            variant="outlined"
            density="compact"
            item-title="CustomerName"
            item-value="CustomerID"
            :items="listCustomer"
            hide-details
        >
            <template v-slot:loader>
                <VProgressLinear v-if="loadingCustomer" indeterminate color="primary" />
            </template>
        </VCombobox>

        <VSpacer />

        <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" max-width="300" hide-details placeholder="Nhập tên món..." v-model:model-value="txtSearch" @update:model-value="selectedMenuCategory = undefined" />
    </MLHbox>

    <MLHbox style="flex-grow: 1; padding: 0.625rem 1rem; overflow: hidden;">
        <!-- Các món đã chọn -->
        <VCard style="width: 300px; height: 100%; flex-shrink: 0; display: flex; flex-direction: column;">
            <VCardTitle style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary))">Món ăn</VCardTitle>
            <VDivider />

            <VList style="flex-grow: 1;">
                <VHover v-for="od in record.OrderDetails">
                    <template v-slot:default="{ isHovering, props }">
                        <VListItem v-bind="props">
                            <template v-slot:prepend>
                                <VLabel style="opacity: 1; font-weight: bold;">{{od.Quantity}}x</VLabel>
                            </template>
                            <span style="margin-left: 8px;">{{ od.MenuItem?.Name }}</span>
                            <template v-slot:append>
                                <b>{{ od.Price }} đ</b>
                                
                                <VBtn @click="od.Quantity--; updateOrderDetail(od);" class="ml-1" color="blue" v-if="isHovering" size="24" icon="mdi-minus" />
                                <VBtn @click="od.Quantity = 0; updateOrderDetail(od);" class="ml-1" color="error" v-if="isHovering" size="24" icon="mdi-delete" />
                            </template>
                        </VListItem>
                    </template>
                </VHover>
            </VList>
            
            <VDivider />
            
            <VListItem style="font-size: 1.2rem;">
                <template v-slot:prepend>
                    <b>Tạm tính</b>
                </template>
                <template v-slot:append>
                    <b>{{ record.TotalAmount ?? 0 }} đ</b>
                </template>
            </VListItem>

            <VCardActions>
                <VSpacer />
                <VBtn style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleCheckoutClick">Thanh toán</VBtn>
            </VCardActions>
        </VCard>

        <VSpacer style="width: 16px; flex-grow: 0; flex-shrink: 0;" />

        <VCard :disabled="loadingMenu" style="flex-grow: 1; height: 100%; display: flex; flex-direction: column;">
            <template v-slot:loader>
                <VProgressLinear color="primary" indeterminate v-if="loadingMenu" />
            </template>

            <!-- Nhóm món -->
            <VSheet color="secondary" style="padding: 0.625rem;">
                <VSlideGroup v-model:model-value="selectedMenuCategory">
                    <VSlideGroupItem
                        v-for="category, index in listMenuCategory"
                        :key="index"
                    >
                        <v-btn
                            :color="category.Value === selectedMenuCategory ? 'primary' : undefined"
                            class="ma-2"
                            rounded
                            @click="selectedMenuCategory = category.Value"
                        >
                            {{ category.Text }}
                        </v-btn>
                    </VSlideGroupItem>
                </VSlideGroup>
            </VSheet>

            <VDivider />

            <!-- Danh sách món -->
            <div style="display: ruby; padding: 0.625rem 1rem;">
                <VCard @click="handleMenuItemClick(menu)" color="primary" v-for="menu in listMenu" min-width="150" width="22%" link class="mr-4 mb-4">
                    <VCardItem class="d-flex" style="flex-direction: column;">
                        <VIcon size="108" icon="mdi-noodles" />
                    </VCardItem>
                    <VCardText>
                        <div style="display: flex; flex-direction: column; align-items: center; text-align: center">
                            <b style="font-size: 1.25rem;">{{ menu.Name }}</b>
                            <span>{{menu.Price}} đ</span>
                        </div>
                    </VCardText>
                </VCard>
            </div>
        </VCard>
    </MLHbox>
    </MLVbox>
</template>

<script lang="ts">
import { EnumMenuItemCategory } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, MenuItem, MLActionResult, Order } from '@/models';
import OrderDetail from '@/models/OrderDetail';
import { PropType } from 'vue';

export default {
    created() {
        this.getListCustomer();
        this.getListMenuItem();

        if (this.record.Customer) {
            this.selectedCustomer = this.record.Customer;
        } else {
            this.selectedCustomer = this.record.CustomerName;
        }
    },

    props: {
        record: {
            type: Object as PropType<Order>,
            required: true
        },

        editMode: {
            type: Number,
            required: true
        }
    },

    data() {
        return {
            loadingCustomer: <boolean>false,
            listCustomer: <Customer[]>[],
            selectedCustomer: <Customer|string|undefined>undefined,
            
            loadingMenu: <boolean>false,
            allMenuItems: <MenuItem[]>[],

            selectedMenuCategory: <EnumMenuItemCategory|undefined>undefined,
            txtSearch: <string>'',
        }
    },

    computed: {
        listMenuCategory() {
            return [
                {
                    Text: 'Khai vị',
                    Value: this.$enumeration.EnumMenuItemCategory.Appetizers
                },
                {
                    Text: 'Món chính',
                    Value: this.$enumeration.EnumMenuItemCategory.MainCourse
                },
                {
                    Text: 'Tráng miệng',
                    Value: this.$enumeration.EnumMenuItemCategory.Dessert
                },
                {
                    Text: 'Đồ uống',
                    Value: this.$enumeration.EnumMenuItemCategory.Drink
                }
            ]
        },

        listMenu():MenuItem[] {
            return this.allMenuItems.filter(mi => (this.selectedMenuCategory === undefined || mi.Category === this.selectedMenuCategory) && mi.Name.toLowerCase().includes(this.txtSearch.toLowerCase()));
        },
    },

    methods: {
        updateOrderDetail(orderDetail: OrderDetail) {
            if (orderDetail.Quantity === 0) {
                this.record.OrderDetails.splice(this.record.OrderDetails.indexOf(orderDetail), 1);
            }

            orderDetail.Price = orderDetail.Quantity * (orderDetail.MenuItem?.Price ?? 0);
            this.record.TotalAmount = this.record.OrderDetails.reduce((a, v) => a + v.Price, 0);
        },

        /**
         * Xử lý khi click vào món ăn
         */
         handleMenuItemClick(menuItem: MenuItem) {
            let existDetail:OrderDetail|undefined = this.record.OrderDetails.find(od => od.MenuItemID === menuItem.MenuItemID);
            if (!existDetail) {
                existDetail = {
                    MenuItemID: menuItem.MenuItemID,
                    Quantity: 0,
                    Price: 0,

                    MenuItem: menuItem
                } as OrderDetail;
                this.record.OrderDetails.push(existDetail);
            }

            existDetail.Quantity++;
            this.updateOrderDetail(existDetail);
        },

        /**
         * Lấy danh sách khách hàng
         */
         async getListCustomer() {
            this.loadingCustomer = true;

            try {
                const result:MLActionResult = await this.$service.CustomerService.getDataPaging(1, -1);
                if (result.Success) {
                    this.listCustomer = result.Data.Data as Customer[];
                }
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.loadingCustomer = false;
            }
            
        },

        handleCheckoutClick() {
            EventBus.emit(this.$eventName.SwitchTabFormOrderDetail, 1);
        },

        /**
         * Lấy danh sách món ăn
         */
         async getListMenuItem() {
            this.loadingMenu = true;

            try {
                const result:MLActionResult = await this.$service.MenuItemService.getDataPaging(1, -1);
                if (result.Success) {
                    this.allMenuItems = result.Data.Data as MenuItem[];
                }
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.loadingMenu = false;
            }
        },
    },

    watch: {
        selectedCustomer(customer: Customer|string) {
            if (customer) {
                if (typeof(customer) === 'object') {
                    this.record.CustomerID = customer.CustomerID;
                    this.record.CustomerName = customer.CustomerName;
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
        }
    },
}
</script>