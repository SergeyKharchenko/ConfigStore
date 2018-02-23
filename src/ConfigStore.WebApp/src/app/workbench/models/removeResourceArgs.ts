import { Service } from "../../infrastructure/models/service";
import { ResourceTypes } from "../enums/resourceTypes";
import { Environment } from "../../infrastructure/models/environment";
import { Config } from "../../infrastructure/models/config";
import { SafeHtml } from "@angular/platform-browser";

export interface RemoveResourceArgs {
    type: ResourceTypes;
    service: Service;
    environment?: Environment;
    config?: Config;
    html: SafeHtml
}