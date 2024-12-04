import { createStoreOnBase } from './baseStore';
import { customerService } from '@/services/customerService';

export const customerStore = createStoreOnBase('customerStore', customerService);