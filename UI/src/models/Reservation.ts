import { EnumReservationStatus } from "@/common/Enumeration";
import MLEntity from "./MLEntity";
import Customer from "./Customer";
import ReservationTable from "./ReservationTable";

interface Reservation extends MLEntity {
    ReservationID: string,
    CustomerID?: string,
    CustomerName: string,
    CustomerPhoneNumber: string,
    ReservationDate: Date,
    ReservationInfo: string,
    Status: EnumReservationStatus
    GuestCount: number,
    Note: string,

    Customer?: Customer,
    ReservationTables?: ReservationTable[]
};

export default Reservation;