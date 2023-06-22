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
  header = {
     'Authorization': `bearer ${localStorage.getItem('access-token')}`,
      'Content-Type': 'application/json'
  }

  async ngOnInit() {
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
      
      this.users = await response
      console.log(this.users);
      
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
    const host = sessionStorage.getItem('host')?.split("@")[1]
    const username = sessionStorage.getItem('host')?.split("@")[0]
    const password = sessionStorage.getItem('password')
  
   
    const dataToSend: newUserDataToSend = {
      hostAddress: host,
      hostUsername: username,
      hostPassword: password,
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
      
    } else {
      alert('error')
    }
  }

  deleteUser(username: string) {
    alert(username)
  }
}
