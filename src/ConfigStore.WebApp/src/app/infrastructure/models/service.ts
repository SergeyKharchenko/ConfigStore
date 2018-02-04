import { Environment } from "./environment";

export interface Service {
    serviceKey: string;
    serviceName: string;
    environments: Environment[]
}
