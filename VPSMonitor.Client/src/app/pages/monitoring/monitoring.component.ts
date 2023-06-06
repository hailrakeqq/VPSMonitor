import { Component } from '@angular/core';
import { Router } from '@angular/router';

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

  constructor(private router: Router) { }
  
  async ngOnInit() {
    const start = performance.now();
    
    const hostAddress = sessionStorage.getItem('host')?.split('@');
    const password = sessionStorage.getItem('password')

    if (hostAddress == undefined && password == undefined) {
      this.router.navigate(['/terminal'])
      return;
    }
    
    const objectToSend: Object = {
      host: hostAddress?.[1],
      username: hostAddress?.[0],
      password: password,
    }
    const request = await fetch(`http://localhost:5081/api/Core/GetResourcesUsageInfo`, {
      method: "POST",
      headers: {
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(objectToSend)
    })
      
    let response = await request.json()

    this.cpuUsage = response[0];
    this.memoryUsage = response[1]
    this.diskpartUsage = response[2]
    this.uptime = response[3]

    const end = performance.now()
    console.log(`execution time: ${(end - start / 1000)} seconds`);
  }
}


