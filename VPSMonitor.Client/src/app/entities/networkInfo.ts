class NetworkInfoBase{
    ipAddress!: string;
    netmask!: string;
    gateway!: string;
}

export class IpV4NetworkInfo extends NetworkInfoBase{
}

export class IpV6NetworkInfo extends NetworkInfoBase{
}

export interface networkInfo {
    ipV4NetworkInfo: IpV4NetworkInfo,
    ipV6NetworkInfo: IpV6NetworkInfo
}
