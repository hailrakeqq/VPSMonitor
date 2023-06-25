import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import { ServerMonitoringService } from 'src/app/Service/ServerMonitoringService';
import "../../entities/systemInfo"
import "../../entities/networkInfo"


@Component({
  selector: 'app-monitoring',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent {
  loading: boolean = true
  rdns: string | undefined = ''
  systemInfo: systemInfo = { hostname: '', os: '', kernel: '', cpuArchitecture: '', dateTime: '' };
  cpuInfo: string[] = []
  ramInfo: string = ''
  diskpartInfo: string = ''
  networkInfo: networkInfo = {ipAddress: '', netmask: '', gateway: ''}

  constructor(private monitoringService: ServerMonitoringService) { }
  
  //TODO: спробувати зробити 1 контроллер метод для всіх потрібних запитів(виконання всіх команд в одному методі)
  async ngOnInit() {
    const start = performance.now();

    this.systemInfo = await this.monitoringService.getSystemInfo();
    this.rdns = sessionStorage.getItem('host')?.split('@')[1]
    this.cpuInfo = await this.monitoringService.getCpuUsage();
    this.ramInfo = await this.monitoringService.getRamUsage();
    this.diskpartInfo = await this.monitoringService.getDiskpartUsage();
    this.networkInfo = await this.monitoringService.getNetworkInfo();
    
    const end = performance.now()
    console.log(`execution time: ${(end - start / 1000)} seconds`);

    console.log(`\nnetwork info:\n${this.networkInfo}`);
    
    this.loading = false
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}


