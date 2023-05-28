import { Component } from '@angular/core';

@Component({
    selector: 'app-sign-in',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss']
})
export class SignInComponent { 
    title = "Sign In"
    //TODO: create user login function and save user data(token email etc.) in local storage
}