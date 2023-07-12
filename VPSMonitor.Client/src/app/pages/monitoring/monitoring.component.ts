import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';
import { MonitoringPageData } from 'src/app/entities/monitoringPageData';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-monitoring',
  templateUrl: './monitoring.component.html',
  styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent {
  loading: boolean = true
  rdns: string | undefined = sessionStorage.getItem('host')?.split('@')[1]
  data: MonitoringPageData | undefined
  apiUrl: string = '';
  
  constructor(private router: Router) {}

  async ngOnInit() {
    
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
      const request = await HttpClient.httpRequest("POST", `${this.apiUrl}/api/Core/GetAllDataForMonitoringPage`, body, headers)    
      this.data = await request.json();
      
    } else { 
      this.router.navigate(['/terminal'])
    }
    
    this.loading = false
  }
    setApiUrl():void {
    if (environment.production)
      this.apiUrl = environment.apiUrl
    else 
      this.apiUrl = environment.apiUrl
  }
  
  async reboot(): Promise<void> {
    HttpClient.httpExecuteBashCommandRequest('reboot')
    alert("The VPS was successfully reloaded")
  }
}


