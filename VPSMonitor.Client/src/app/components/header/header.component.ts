import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
    constructor(private router: Router) { }
    
    title = 'VPSMonitor.Client'
    
    public HomeRedirect(): void{
        this.router.navigate([''])
    }
    public AboutRedirect(): void{
        this.router.navigate(['/about'])
    }
    public SignInRedirect(): void{
        this.router.navigate(['/signin'])
    }
    public SignUpRedirect (): void{
        this.router.navigate(['/signup'])
    }
}
