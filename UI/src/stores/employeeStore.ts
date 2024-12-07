import { employeeService } from '@/services/employeeService';
import { createStoreOnBase } from './baseStore';
import { Employee } from '@/models';

export const employeeStore = createStoreOnBase<Employee>('employeeStore', employeeService);