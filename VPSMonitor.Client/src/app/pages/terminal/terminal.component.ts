import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

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
    if (command.startsWith('sethost')) {
      const inputHost = command.substring(8).trim();
      if (inputHost.indexOf('@')) {
        this.hostAddress = inputHost;
        sessionStorage.setItem('host', this.hostAddress);

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
      const host = sessionStorage.getItem('host')?.split('@');
      const password = sessionStorage.getItem('password')
      if (host != undefined && password != undefined) {
        const username = host[0]
        const address = host[1];
        const objectToSend = {
          host: address,
          username: username,
          password: password,
          command: command
        }
        
        const request = await fetch(`http://localhost:5081/api/Core/ExecuteCommand`, {
          method: 'POST',
          headers: {
            'Authorization': `bearer ${localStorage.getItem('access-token')}`,
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(objectToSend)
        })

        if (request.status === 200) {
          let response = await request.text()
          console.log(response);
          response = '\n' + response 
          this.outputs.push(response)
        }
        return
      }
      this.outputs.push('Command not recognized');
    }
  }

  submitPassword(password: string): void {
    
    this.vpsPassword = password.trim();
    sessionStorage.setItem('password', this.vpsPassword);
    this.outputs.push(`Host set to: ${this.hostAddress}`);
  
    this.command = '';
    this.awaitingPassword = false;
  }

  promptHost(): void {
    this.outputs.unshift('> sethost (example: sethost testuser@test.server)');
    this.outputs.unshift('Enter the host:');
  }
}
