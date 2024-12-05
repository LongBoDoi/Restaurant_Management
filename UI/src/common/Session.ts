import { EnumUserType } from "./Enumeration";

interface SessionInterface {
    UserData: any,
    UserName: string,
    UserType: EnumUserType
}

export default SessionInterface;