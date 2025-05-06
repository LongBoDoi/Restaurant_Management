import MLEntity from "./MLEntity";
import UserLogin from "./UserLogin";

interface Customer extends MLEntity {
    CustomerID: string,
    CustomerName: string,
    PhoneNumber: string,
    Email: string,
    Address: string,
    Preferences: string,
    LoyaltyPoint: number,
    ImageUrl: string,

    UserLogin?: UserLogin,

    EntityName: 'Customer'
};

export default Customer;