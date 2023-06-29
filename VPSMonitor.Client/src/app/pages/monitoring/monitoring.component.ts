import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import { ServerMonitoringService } from 'src/app/Service/ServerMonitoringService';
import "../../entities/systemInfo"
import { IpV4NetworkInfo, IpV6NetworkInfo, networkInfo } from "../../entities/networkInfo"
import { MonitoringPageData } from 'src/app/entities/monitoringPageData';


@Component({
  selector: 'app-monitoring',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent {
  loading: boolean = true
  rdns: string | undefined = sessionStorage.getItem('host')?.split('@')[1]
  data: MonitoringPageData | undefined
  // systemInfo: systemInfo = { hostname: '', os: '', kernel: '', cpuArchitecture: '', dateTime: '' };
  // cpuInfo: string[] = []
  // ramInfo: string = ''
  // diskpartInfo: string = ''
  // networkInfo: networkInfo = {
  //   ipV4NetworkInfo: new IpV4NetworkInfo(),
  //   ipV6NetworkInfo: new IpV6NetworkInfo()
  // }

  constructor(private monitoringService: ServerMonitoringService) { }
  
  async ngOnInit() {
    const start = performance.now();

    // this.systemInfo = await this.monitoringService.getSystemInfo();
    // this.rdns = sessionStorage.getItem('host')?.split('@')[1]
    // this.cpuInfo = await this.monitoringService.getCpuUsage();
    // this.ramInfo = await this.monitoringService.getRamUsage();
    // this.diskpartInfo = await this.monitoringService.getDiskpartUsage();
    // this.networkInfo = await this.monitoringService.getNetworkInfo();

    const headers = { 
      'Authorization': `bearer ${localStorage.getItem('access-token')}`,
      'Content-Type': 'application/json'
    }

    const body = {
      host: sessionStorage.getItem('host')?.split('@')[1],
      username: sessionStorage.getItem('host')?.split('@')[0],
      password: sessionStorage.getItem('password'),
      command: ''
    }

    const request = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Core/GetAllDataForMonitoringPage", body, headers)
    this.data = await request.json();
    console.log(this.data);
    
    const end = performance.now()
    console.log(`execution time: ${(end - start / 1000)} seconds`);
    
    this.loading = false
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}


