import { Component } from '@angular/core';
import { Toolchain } from 'src/toolchain';

@Component({
    selector: 'app-sign-in',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss']
})
export class SignInComponent { 
    email: string = '';
    password: string = '';
    
    public async Login(): Promise<void> {
        if (Toolchain.ValidateInputData(this.email, this.password)) {
            
            const userData = {
                Email: this.email,
                Password: this.password
            }

            const request = await fetch(`http://localhost:5081/api/Auth/Login`, {
                method: 'POST',
                headers: { 'content-type': 'application/json' },
                body: JSON.stringify(userData)
            })
            if (request.status == 200) {
                this.SaveDataToLocalStorage(await request.json())
                
                alert("user was successfully login")
                window.location.href = ''
            }
        } else {
            alert("data that you enter is not valid")
        }
    }

    private SaveDataToLocalStorage(response: any): void {
        localStorage.setItem("id", response.id)
        localStorage.setItem("email", response.email)
        localStorage.setItem("access-token", response.accessToken)
        localStorage.setItem("refresh-token", response.refreshToken)
    }
}