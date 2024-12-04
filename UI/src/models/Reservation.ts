import { EnumReservationStatus } from "@/common/Enumeration";
import MLEntity from "./MLEntity";

interface Reservation extends MLEntity {
    ReservationID: string,
    CustomerID?: string,
    CustomerName: string,
    CustomerPhoneNumber: string,
    ReservationDate: Date,
    ReservationInfo: string,
    Status: EnumReservationStatus
    GuestCount: number,
    Note: string
};

export default Reservation;