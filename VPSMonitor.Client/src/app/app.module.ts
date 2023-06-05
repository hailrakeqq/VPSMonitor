import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

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
    MonitoringComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
