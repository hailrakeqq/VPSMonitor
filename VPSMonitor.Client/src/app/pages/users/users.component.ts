import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import '../../entities/user'
import '../../entities/dataToSend'
import { Toolchain } from 'src/toolchain';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {
  users: user[] = []
  isModalOpen: boolean = false
  newUsername: string = ''
  password: string = ''
  ConfirmPassword: string = ''

  async ngOnInit() {
    const header = {
      'Authorization': `bearer ${localStorage.getItem('access-token')}`,
      'Content-Type': 'application/json'
    }

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
        body, header)
  
      this.users = await response
      this.users.forEach(item => item.permissions.join(", "));
    }
  }

  handleModalOpened(): void{
    this.isModalOpen = true
  }

  handleModalClosed(): void{
    this.isModalOpen = false
  }


  async addNewUser(): Promise<void>{
    const hostData = {
      host: sessionStorage.getItem('host'),
      password: sessionStorage.getItem('password')
    }

    const dataToSend: newUserDataToSend = {
      hostAddress: hostData.host?.split('@')[1],
      hostUsername: hostData.host?.split('@')[0],
      hostPassword: hostData.password,
      userUsername: this.newUsername,
      userPassword: this.password,
      userConfirmPassword: this.ConfirmPassword
    };
    
    if (Toolchain.ValidateInputNewUserData(dataToSend)) {
      console.log(dataToSend);
      
    } else {
      alert('error')
    }
  }
}
