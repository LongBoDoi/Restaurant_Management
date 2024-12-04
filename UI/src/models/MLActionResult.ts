import { EnumApplicationErrorCode } from "@/common/Enumeration";

interface MLActionResult {
    Success: boolean,
    Data?: any,
    ErrorMsg?: string,
    ErrorCode?: EnumApplicationErrorCode
}

export default MLActionResult;