import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './pages/home/home.component';
import { SignInComponent } from './pages/auth/signin/signin.component';
import { SignupComponent } from './pages/auth/signup/signup.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { TerminalComponent } from './pages/terminal/terminal.component';
import { MonitoringComponent } from './pages/monitoring/monitoring.component';
import { UsersComponent } from './pages/users/users.component';
import { FileManagementPageComponent } from './pages/file-management-page/file-management-page.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'signin', component: SignInComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'settings', component: SettingsComponent },
  { path: 'terminal', component: TerminalComponent },
  { path: 'filemanager', component: FileManagementPageComponent},
  { path: 'monitoring', component: MonitoringComponent },
  { path: 'users', component: UsersComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
