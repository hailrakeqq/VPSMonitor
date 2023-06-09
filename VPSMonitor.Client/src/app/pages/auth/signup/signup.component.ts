import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Toolchain } from 'src/toolchain';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  email: string = '';
  password: string = '';
  confirmPassword: string | undefined;

  constructor(private router: Router) {}

  public async Register(): Promise<void> {
    if (Toolchain.ValidateInputData(this.email, this.password, this.confirmPassword)) {
      const userData = {
        Email: this.email,
        Password: this.password,
        ConfirmPassword: this.confirmPassword
      }
      let apiUrl = ''
      if (environment.production)
        apiUrl = environment.apiUrl
      else
        apiUrl = environment.apiUrl
      
      const request = await fetch(`${apiUrl}/api/Auth/Registration`, {
        method: 'POST',
        headers: {'content-type' : 'application/json'},
        body: JSON.stringify(userData)
      })
      const response = await request.status
      if (response == 200) {
        alert("user was successfully created")
        this.router.navigate(['/signin'])
      } 
    } else {
      alert("data that you enter is not valid")
    }
  }
}
