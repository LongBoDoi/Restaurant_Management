import { EnumEditMode } from "@/common/Enumeration";

interface MLEntity {
    EditMode: EnumEditMode,
    EntityName: string,

    CreatedDate: Date,
}

export default MLEntity;