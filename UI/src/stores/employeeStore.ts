import { employeeService } from '@/services/employeeService';
import { createStoreOnBase } from './baseStore';

export const employeeStore = createStoreOnBase('employeeStore', employeeService);