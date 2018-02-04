import { Service } from "./service";

export interface Application {
    applicationKey: string;
    applicationName: string;
    services: Service[]
}
