import { Injectable } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class ServerMonitoringService {
    constructor(private router: Router) { }
    
    headers = {
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
        'Content-Type': 'application/json'
    }
    dataToSend = {
                host: sessionStorage.getItem('host')?.split('@')[1],
                username: sessionStorage.getItem('host')?.split('@')[0],
                password: sessionStorage.getItem('password')
            }

    async getSystemInfo(): Promise<any> {
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpRequest(
                "POST",
                "http://localhost:5081/api/Core/GetSystemInfo",
                this.dataToSend,
                this.headers)
            
            return response;
        }
        this.router.navigate(['/terminal'])
    }

    async getCPUUsage(): Promise<any> {
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpRequest(
                "POST",
                "http://localhost:5081/api/Core/GetCpuInfo",
                this.dataToSend,
                this.headers)
            
            return response;
        }
        this.router.navigate(['/terminal'])
    }

    async getMemoryUsage(): Promise<any> {
    }
}