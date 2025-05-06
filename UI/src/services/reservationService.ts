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
            const response = await this.api.post('/CreateCustomerReservation', reservation, {
                headers: {
                    Authorization: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiJkMDkyOWFlZi0xYTViLTQ0ZjYtOTYyZC0wMWY3ZjliYjJiMmIiLCJVc2VybmFtZSI6ImFkbWluIiwiVXNlclR5cGUiOiIwIiwiZXhwIjo0OTAyMjA1ODExLCJpc3MiOiJtbF9pc3N1ZXIiLCJhdWQiOiJtbF9hdWRpZW5jZSJ9.4PyLxWfy0_yU9HCtoNkgFnDTrV3JgQnDoNJjL_42zN8}`
                }
            });
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