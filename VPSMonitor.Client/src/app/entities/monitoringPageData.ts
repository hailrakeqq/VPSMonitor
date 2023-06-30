import { networkInfo } from "./networkInfo";
import { systemInfo } from "./systemInfo"

// export interface MonitoringPageData {
//   systemInfo: systemInfo;
//   cpuInfo: string[];
//   ramInfo: string;
//   discpartInfo: string;
//   networkInfo: networkInfo;
// }

export interface MonitoringPageData {
  systemInfo?: SystemInfo;
  cpuUsageInfo?: string[];
  ramUsageInfo?: string;
  diskpartUsageInfo?: string;
  networkInfo?: NetworkInfo;
}

export interface SystemInfo {
  hostname: string;
  os: string;
  kernel: string;
  cpuArchitecture: string;
  dateTime: string;
}

export interface NetworkInfo {
  ipV4NetworkInfo: IPNetworkInfo;
  ipV6NetworkInfo: IPNetworkInfo;
}

export interface IPNetworkInfo {
  ipAddress: string;
  netmask: string;
  gateway: string;
}