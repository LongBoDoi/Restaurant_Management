import { createStoreOnBase } from './baseStore';
import { Setting } from '@/models';
import { settingService } from '@/services/settingService';

export const settingStore = createStoreOnBase<Setting>('employeeStore', settingService);