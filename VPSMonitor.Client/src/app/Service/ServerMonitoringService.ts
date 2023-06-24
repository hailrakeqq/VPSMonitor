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
                password: sessionStorage.getItem('password'),
                command: ''
            }

    async getSystemInfo(): Promise<any> {
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpRequest(
                "POST",
                "https://localhost:5081/api/Core/GetSystemInfo",
                this.dataToSend,
                this.headers)
            
            return response.json();
        }
        this.router.navigate(['/terminal'])
    }

    async getCpuUsage(): Promise<any> {
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpRequest(
                "POST",
                "https://localhost:5081/api/Core/GetCpuInfo",
                this.dataToSend,
                this.headers)
            
            return response.json();
        }
        this.router.navigate(['/terminal'])
    }

    async getRamUsage(): Promise<any> {
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpExecuteBashCommandRequest('free -h')
            
            return response;
        } 
        this.router.navigate(['/terminal'])
    }

    async getDiskpartUsage(): Promise<any>{
        if (this.dataToSend.host != undefined) {
            const response = await HttpClient.httpExecuteBashCommandRequest('df -h')
            
            return response;
        } 
        this.router.navigate(['/terminal'])
    }
}