import { Component } from '@angular/core';
import { HttpClient } from 'src/HttpClient';
import '../../entities/user'
import '../../entities/dataToSend'
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {
  users: user[] = []

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
        "http://localhost:5081/api/Core/GetUsers",
        body, header)
  
      this.users = await response
      this.users.forEach(item => item.permissions.join(", "));
    }
  }
}
