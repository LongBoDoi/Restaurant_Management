import { orderService } from '@/services/orderService';
import { createStoreOnBase } from './baseStore';
import { Order } from '@/models';

export const orderStore = createStoreOnBase<Order>('orderStore', orderService);