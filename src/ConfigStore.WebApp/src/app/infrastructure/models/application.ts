export interface Application {
    applicationKey: string;
    applicationName: string;
    services: [{
        serviceKey: string;
        serviceName: string;
        environments: [{
            environmentKey: string;
            environmentName: string;
            configs: string[],
        }]
    }];
}
