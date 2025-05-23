import { Setting } from "@/models";
import { EnumUserType } from "./Enumeration";

interface SessionInterface {
    UserData: any,
    UserName: string,
    UserType: EnumUserType,

    Settings: Setting[],
    Permissions: Set<string>,
}

const session = {} as SessionInterface;

export default SessionInterface;
export { session };