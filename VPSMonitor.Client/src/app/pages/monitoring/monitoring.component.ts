import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';
import { ServerMonitoringService } from 'src/app/Service/ServerMonitoringService';
import "../../entities/systemInfo"
@Component({
  selector: 'app-monitoring',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent {
  loading: boolean = true
  systemInfo: systemInfo = { hostname: '', os: '', kernel: '', cpuArchitecture: '', dateTime: '' };
  cpuInfo: string[] = []

  constructor(private router: Router, private monitoringService: ServerMonitoringService) { }
  
  async ngOnInit() {
    const start = performance.now();
    
    this.systemInfo = await this.monitoringService.getSystemInfo();
    //console.log(this.systemInfo);
    
    this.cpuInfo = await this.monitoringService.getCPUUsage();
    console.log(this.cpuInfo);
    

    const end = performance.now()
    console.log(`execution time: ${(end - start / 1000)} seconds`);
    this.loading = false
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}

