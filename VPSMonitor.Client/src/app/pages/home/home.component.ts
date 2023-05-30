import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  user = {
    id: localStorage.getItem("id"),
    email: localStorage.getItem("email"),
  }
}
