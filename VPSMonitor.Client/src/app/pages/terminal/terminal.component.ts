import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';

@Component({
  selector: 'app-terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.scss']
})
export class TerminalComponent {
  outputs: string[] = [];
  command: string = '';
  hostAddress: string | null = '';
  vpsPassword: string | null = '';
  awaitingPassword: boolean = false;
  currentPath: string = '';

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentPath = event.url;
      }
    });
  }

  ngOnInit(): void {
    this.hostAddress = sessionStorage.getItem('host');
    this.vpsPassword = sessionStorage.getItem('password')

    if (!this.hostAddress && !this.vpsPassword) {
      this.promptHost()
    }
  }

  handleCommand(event: any): void {
    if (event.key === 'Enter') {
      event.preventDefault();

      const inputElement = event.target as HTMLInputElement;
      const command = this.command.trim();
      if (this.awaitingPassword) {
        this.submitPassword(command)
      } else {
        this.outputs.push(`> ${command}`);
        this.executeCommand(command);
        this.command = '';
      }
    }
  }

  async executeCommand(command: string): Promise<void> {
    if (command.startsWith('sethost') || command.startsWith('changehost')) {
      const inputHost = command.substring(8).trim();
      if (inputHost.indexOf('@')) {
        this.hostAddress = inputHost;
        sessionStorage.setItem('host', this.hostAddress);

        const username = this.hostAddress.split("@")[0]
        if(username === 'root')
          sessionStorage.setItem('userHomeDirectoryPath', '/root')
        else
          sessionStorage.setItem('userHomeDirectoryPath', `/home/${username}`)
        
        this.awaitingPassword = true;
        this.outputs.push('Enter password:');
        return
      } else {
        this.outputs.push('Please provide a valid host');
      }
    }
    if (command === 'clear') {
      this.outputs = []
    } else {
        let response = await HttpClient.httpExecuteBashCommandRequest(command);
        console.log(response);
        response = '\n' + response 
        this.outputs.push(response)
    }
  }

  submitPassword(password: string): void {
    
    this.vpsPassword = password.trim();
    sessionStorage.setItem('password', this.vpsPassword);
    this.outputs.push(`Host set to: ${this.hostAddress}\nTo Change host enter command \`changehost username@hostAddress\``);
  
    this.command = '';
    this.awaitingPassword = false;
  }

  promptHost(): void {
    this.outputs.unshift('> sethost (example: sethost testuser@test.server)');
    this.outputs.unshift('Enter the host:');
  }
}
