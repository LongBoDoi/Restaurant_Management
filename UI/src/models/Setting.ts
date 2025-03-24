import { EnumDataType } from "@/common/Enumeration";
import MLEntity from "./MLEntity";

interface Setting extends MLEntity {
    SettingID: string,
    SettingKey: string,
    SettingValue: string,
    DataType: EnumDataType,
    Value: any
};

export default Setting;