import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SignInComponent } from './pages/auth/signin/signin.component';
import { AboutComponent } from './pages/about/about.component';
import { HomeComponent } from './pages/home/home.component';
import { SignupComponent } from './pages/auth/signup/signup.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { ModalComponent } from './components/modal/modal.component';
import { TerminalComponent } from './pages/terminal/terminal.component';
import { MonitoringComponent } from './pages/monitoring/monitoring.component';
import { UsersComponent } from './pages/users/users.component';
import { AuthErrorInterceptor } from './auth-error.interceptor';
import { FileManagementPageComponent } from './pages/file-management-page/file-management-page.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    AboutComponent,
    HomeComponent,
    SignupComponent,
    SidebarComponent,
    SettingsComponent,
    ModalComponent,
    TerminalComponent,
    MonitoringComponent,
    UsersComponent,
    FileManagementPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
