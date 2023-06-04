import { Component } from '@angular/core';

@Component({
  selector: 'app-sshkeys',
  templateUrl: './sshkeys.component.html',
  styleUrls: ['./sshkeys.component.scss']
})
export class SshkeysComponent {
  sshKey: string ='';
  username: string = '';
  isModalOpen = false;
  modalTitle = 'Add new SSH key';
  sshKeys:Array<any> = []

  ngOnInit(): void{
    this.getUserSshKeys();
  }

  async getUserSshKeys(): Promise<void>{
    var request = await fetch(`http://localhost:5081/api/Ssh/GetUserSshKeys`, {
      method: "GET",
      headers: {
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
        'Content-Type': 'application/json'
      }
    })
   
    if (request.status === 200) {
      const response = await request.json(); 
            console.log(response);
      this.sshKeys.push(response);
    } else {
      alert(request.statusText)
    }
  }

  async addSSHKey(): Promise<void> {
    const sshKeyObject = {
      Username: this.username,
      SSHKey: this.sshKey
    }
    const request = await fetch(`http://localhost:5081/api/Ssh/AddSshKey/${localStorage.getItem('id')}`, {
      method: 'POST',
      headers: {
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(sshKeyObject)
    })
    if (request.status == 200) {
      const response = await request.json()
      alert(response)
    } 
  }

  openModal() {
    this.isModalOpen = true;
  }
  
  closeModal() {
    this.isModalOpen = false;
  }
}
