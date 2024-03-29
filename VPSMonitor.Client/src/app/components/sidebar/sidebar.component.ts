import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
    isUserLogin = localStorage.getItem("access-token") 
   
constructor(private router: Router) { }
    
    public HomeRedirect(): void{
        this.router.navigate([''])
    }
    public SignInRedirect(): void{
        this.router.navigate(['/signin'])
    }
    public SignUpRedirect (): void{
        this.router.navigate(['/signup'])
    }
    public MonitoringPageRedirect(): void {
        this.router.navigate(['/monitoring'])
    }
    public UsersPageRedirect(): void {
        this.router.navigate(['/users'])
    }
    public TerminalPageRedirect(): void{
        this.router.navigate(['/terminal'])
    }   
    public FileManagerPageRedirect(): void {
        this.router.navigate(['/filemanager'])
    }
    public SettingsRedirect(): void{
        this.router.navigate(['/settings'])
    }
    public SignOut(): void{
        localStorage.clear()
        window.location.reload()
        this.router.navigate([''])
    }
}




