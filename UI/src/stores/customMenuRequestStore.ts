import { createStoreOnBase } from './baseStore';
import CustomMenuRequest from '@/models/CustomMenuRequest';
import { customMenuRequestService } from '@/services/customMenuRequestService';

export const customMenuRequestStore = createStoreOnBase<CustomMenuRequest>('customMenuRequestStore', customMenuRequestService);