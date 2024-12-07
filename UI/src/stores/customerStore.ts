import { Customer } from '@/models';
import { createStoreOnBase } from './baseStore';
import { customerService } from '@/services/customerService';

export const customerStore = createStoreOnBase<Customer>('customerStore', customerService);