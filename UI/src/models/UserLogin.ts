interface UserLogin {
    UserLoginID: string,
    EmployeeID?: string,
    CustomerID?: string,
    Username: string,
    Password: string,

    IsChangePassword: boolean,
    OldPassword: string
}

export default UserLogin;