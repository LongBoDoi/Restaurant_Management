import MLEntity from "./MLEntity";
import Reservation from "./Reservation";
import Table from "./Table";

interface ReservationTable extends MLEntity {
    ReservationTableID: string,
    ReservationID: string,
    TableID: string,

    Reservation?: Reservation,
    Table?: Table
};

export default ReservationTable;