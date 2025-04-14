import { MLActionResult, Reservation } from "@/models";
import MLBaseService from "./baseService";
import { EnumReservationStatus } from "@/common/Enumeration";
import CommonFunction from "@/common/CommonFunction";

class ReservationService extends MLBaseService<Reservation> {
    protected entityName: string = 'Reservation';

    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async createCustomerReservation(reservation:Reservation) : Promise<MLActionResult> {
        var result:MLActionResult = {
            Success: false
        } as MLActionResult;

        try {
            const response = await this.api.post('/CreateCustomerReservation', reservation);
            result = response.data as MLActionResult;
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return result;
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