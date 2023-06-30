import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
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
  
  async ngOnInit() {
    const start = performance.now();

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
    // console.log(await request.json());
    
    this.data = await request.json();

    
    const end = performance.now()
    console.log(`execution time: ${(end - start / 1000)} seconds`);
    
    this.loading = false
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}


