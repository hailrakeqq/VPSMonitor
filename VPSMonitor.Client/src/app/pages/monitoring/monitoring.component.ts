import { Component } from '@angular/core';
import { httpExecuteBashCommandRequest } from '../../../httpExecuteBashCommandRequest'
import { Parser } from 'src/parser';

@Component({
  selector: 'app-monitoring',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent {
  cpuUsage: string | undefined
  memoryUsage: string | undefined
  diskpartUsage: string | undefined
  uptime: string | undefined

  async ngOnInit() {
    
    const start = performance.now();

    const [mpstatResult, freeResult, dfResult, uptimeResult] = await Promise.all([
      httpExecuteBashCommandRequest("mpstat -P ALL -o JSON"),
      httpExecuteBashCommandRequest("free -h"),
      httpExecuteBashCommandRequest("df -h"),
      httpExecuteBashCommandRequest("uptime")
    ]);

    const [cpuUsage, memoryUsage, diskpartUsage] = await Promise.all([
      Parser.mpstatCommandParse(mpstatResult),
      Parser.freeCommandParse(freeResult),
      Parser.dfCommandParse(dfResult)
    ]);

    this.cpuUsage = cpuUsage;
    this.memoryUsage = memoryUsage;
    this.diskpartUsage = diskpartUsage;
    this.uptime = uptimeResult;


    const end = performance.now()

    console.log(`execution time: ${(end - start / 1000)} seconds`);
  }
}
