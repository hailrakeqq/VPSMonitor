import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  email: string | undefined;
  password: string | undefined;
  confirmPassword: string | undefined;

  constructor(private router: Router) { }

  public Register(): void {
    console.log(`email: ${this.email}`);
    console.log(`password: ${this.password}`);
    console.log(`confirm password: ${this.confirmPassword}`);
    //TODO: create user register function using API
    //this.router.navigate(['/signin'])
  }
}
