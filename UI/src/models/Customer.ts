import MLEntity from "./MLEntity";
import UserLogin from "./UserLogin";

interface Customer extends MLEntity {
    CustomerID: string,
    CustomerName: string,
    PhoneNumber: string,
    Address: string,
    Preferences: string,
    LoyaltyPoint: number,

    UserLogin?: UserLogin,

    EntityName: 'Customer'
};

export default Customer;