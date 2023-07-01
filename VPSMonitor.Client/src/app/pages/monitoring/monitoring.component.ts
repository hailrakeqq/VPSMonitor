import { Component } from '@angular/core';
import { Router } from '@angular/router';
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
  
  constructor(private router: Router) {}

  async ngOnInit() {
    const start = performance.now();

    const parsedSessionStorage = sessionStorage.getItem('host')?.split('@') 
    const vpsPassword = sessionStorage.getItem('password');
    
    if (parsedSessionStorage != undefined && vpsPassword != undefined) {
      const headers = { 
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
        'Content-Type': 'application/json'
      }
      const body = {
        host: parsedSessionStorage[1],
        username: parsedSessionStorage[0],
        password: vpsPassword,
        command: ''
      }
      const request = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Core/GetAllDataForMonitoringPage", body, headers)    
      this.data = await request.json();
      
      const end = performance.now()
      console.log(`execution time: ${(end - start / 1000)} seconds`);
    } else { 
      this.router.navigate(['/terminal'])
    }
    
    this.loading = false
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}


