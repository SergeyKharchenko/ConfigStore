import { Service } from "../../infrastructure/models/service";
import { ResourceTypes } from "../enums/resourceTypes";

export interface AddDialogResult {
    type: ResourceTypes;
    name: string,
    service?: Service;
}