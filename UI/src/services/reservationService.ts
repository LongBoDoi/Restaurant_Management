import { MLActionResult, Reservation } from "@/models";
import BaseService from "./baseService";
import { EnumReservationStatus } from "@/common/Enumeration";

class ReservationService extends BaseService<Reservation> {
    protected entityName: string = 'Reservation';

    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async createCustomerReservation(reservation:Reservation) : Promise<MLActionResult> {
        const response = await this.api.post('/CreateCustomerReservation', reservation);
        return response?.data as MLActionResult;
    }

    /**
     * Lấy danh sách đặt bàn
     */
    async getReservations(status: EnumReservationStatus, page: number, itemsPerPage: number) : Promise<MLActionResult> {
        const response = await this.api.get('/GetReservations', {
            params: {
                status: status,
                page: page,
                itemsPerPage: itemsPerPage
            }
        });
        return response?.data as MLActionResult;
    }
}

export default ReservationService;

const reservationService = new ReservationService();
export {reservationService};