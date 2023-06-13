import { Component } from '@angular/core';
import { Toolchain } from 'src/toolchain';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  newEmail: string = '';
  sshKey: string ='';
  username: string ='';
  confirmPassword: string ='';
  newPassword: string ='';

  async changeEmail(): Promise<void> {
    if (Toolchain.ValidateInputEmail(this.newEmail)) {
      const newUserData = {
        Email: this.newEmail,
        ConfirmPassword: this.confirmPassword
      }

      const request = await fetch(`http://localhost:5081/api/UserSettings/ChangeUserData/${localStorage.getItem('id')}`, {
        method: 'PUT',
        headers: {
          'Authorization': `bearer ${localStorage.getItem('access-token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(newUserData)
      })

      if (request.status == 200) {
        const response = await request.text()
        alert(response)
      }
    }
  }

  async changePassword(): Promise<void> {
    if (Toolchain.ValidateInputForChangePassword(this.newPassword, this.confirmPassword)) {
      const newUserData = {
        Password: this.newPassword,
        ConfirmPassword: this.confirmPassword
      }

      const request = await fetch(`http://localhost:5081/api/UserSettings/ChangeUserData/${localStorage.getItem('id')}`, {
        method: 'PUT',
        headers: {
          'Authorization': `bearer ${localStorage.getItem('access-token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(newUserData)
      })

      if (request.status == 200) {
        const response = await request.text()
        alert(response)
      }
    }
  }

  async deleteAccount(): Promise<void> {
      const confirmPassword = {
        ConfirmPassword: this.confirmPassword
      }

      const request = await fetch(`http://localhost:5081/api/UserSettings/DeleteAccount/${localStorage.getItem('id')}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `bearer ${localStorage.getItem('access-token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(confirmPassword)
      })

      if (request.status == 200) {
        const response = await request.text()
        alert(response)
      }
  }
}
