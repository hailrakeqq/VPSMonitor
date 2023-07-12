import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import '../../entities/linuxUser'
import '../../entities/dataToSend'
import { Toolchain } from 'src/toolchain';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {
  isLoading: boolean = true
  sortByDownId: boolean = false
  users: linuxUser[] = []
  isModalOpen: boolean = false
  newUsername: string = ''
  password: string = ''
  ConfirmPassword: string = ''
  host = sessionStorage.getItem('host')?.split("@")[1]
  username = sessionStorage.getItem('host')?.split("@")[0]
  hostPassword = sessionStorage.getItem('password')
  header = {
    'Authorization': `bearer ${localStorage.getItem('access-token')}`,
    'Content-Type': 'application/json'
  }
  apiUrl: string = '';

  constructor(private router: Router) {}

  async ngOnInit() {
    this.setApiUrl();
    const hostAddress = sessionStorage?.getItem('host')?.split('@');
    const password = sessionStorage.getItem('password');

    if (hostAddress != null && password != null) {
      const body: dataToSend= { 
        host: hostAddress[1],
        username: hostAddress[0],
        password: password
      }
  
      const response = await HttpClient.httpRequest("POST",
        "https://localhost:5081/api/CoreUserCrud/GetUsers",
        body, this.header)
      
      this.users = await response.json()            
      this.users.forEach(item => item.permissions.join(", "));
      this.users.map(user => {
        if (user.username === 'root')
          user.homeDirectoryPath = '/root'
      })
    } else {
      this.router.navigate(['/terminal'])
    }

    this.isLoading = false
  }
  setApiUrl():void {
    if (environment.production)
      this.apiUrl = environment.apiUrl
    else 
      this.apiUrl = environment.apiUrl
  }

  handleModalOpened(): void{
    this.isModalOpen = true
  }

  handleModalClosed(): void{
    this.isModalOpen = false
  }

  async addNewUser(): Promise<void>{
    const dataToSend: newUserDataToSend = {
      hostAddress: this.host,
      hostUsername: this.username,
      hostPassword: this.hostPassword,
      userUsername: this.newUsername,
      userPassword: this.password,
      userConfirmPassword: this.ConfirmPassword
    };
    
    if (Toolchain.ValidateInputNewUserData(dataToSend)) {
      const response = await fetch("https://localhost:5081/api/CoreUserCrud/CreateUser", {
        method: "POST",
        headers: this.header,
        body: JSON.stringify(dataToSend)
      })
      console.log(response);
      if (response.status == 200)
        window.location.reload();
      
    } else {
      alert('error')
    }
  }

  async deleteUser(username: string): Promise<any> {
    const dataToSend: newUserDataToSend = {
      hostAddress: this.host,
      hostUsername: this.username,
      hostPassword: this.hostPassword,
      userUsername: username,
      userPassword: this.password,
      userConfirmPassword: this.ConfirmPassword
    };
    
    const response = await HttpClient.httpRequest("DELETE", "https://localhost:5081/api/CoreUserCrud/DeleteUser", dataToSend, this.header)
    if (response.status == 200)
      window.location.reload();
  }

  sortUsersByIdDesc() {
    this.users.sort((a, b) => a.id - b.id)
    this.sortByDownId = true
  }
  sortUsersByIdAsc() {
    this.users.sort((a, b) => b.id - a.id)
    this.sortByDownId = false
  }
}
