import { Service } from "../../infrastructure/models/service";
import { ResourceTypes } from "../enums/resourceTypes";
import { Environment } from "../../infrastructure/models/environment";

export interface RemoveDialogArgs {
    type: ResourceTypes;
    service: Service;
    environment?: Environment;
}