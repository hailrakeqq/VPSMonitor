import { NavigationEnd, Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';
import { Component } from '@angular/core';
import { Terminal } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
@Component({
  selector: 'app-terminal',
  templateUrl: './terminal.component.html',
})
export class TerminalComponent {
  outputs: string[] = [];
  command: string = '';
  hostAddress: string | null = '';
  vpsPassword: string | null = '';
  awaitingPassword: boolean = false;
  currentPath: string = '';
  userInput: string = '';
  inputHistory: any = []
  term = new Terminal({
      fontSize: 18,
      cursorBlink: true,
      cols: 130,
      rows: 35,
    });
  fitAddon = new FitAddon();
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

    this.term.loadAddon(this.fitAddon);
    const terminal = document.getElementById('terminal')
  
    if(terminal)
      this.term.open(terminal);
  
    this.fitAddon.fit();

    if (!this.hostAddress && !this.vpsPassword) {
      this.promptHost()
    }

    this.term.write(`${this.hostAddress}: `); 
    this.term.onData((data) => {
      if (data === '\r') {
        const inputLine = this.userInput.trim();
        this.userInput = ''; 

        console.log('Input Line:', inputLine);
        if (this.awaitingPassword)
          this.submitPassword(inputLine)
        else 
          this.executeCommand(inputLine)

        this.updatePS1();
      } else if (data === '\x7f') {
        if (this.userInput.length > 0) {
          this.userInput = this.userInput.slice(0, -1);
          this.term.write('\b \b');
        }
      } else {
        if (!this.awaitingPassword) {
          this.userInput += data;
          this.term.write(data);
        } else {
          this.userInput += data;
        }
      }
    });
  }

  async executeCommand(command: string): Promise<void> {
    if (command.startsWith('sethost')) {
    const inputHost = command.substring(8).trim();
    if (inputHost.indexOf('@') !== -1) {
      this.hostAddress = inputHost;
      sessionStorage.setItem('host', this.hostAddress);

      const username = this.hostAddress.split("@")[0];
      if (username === 'root')
        sessionStorage.setItem('DirectoryPath', '/root');
      else
        sessionStorage.setItem('DirectoryPath', `/home/${username}`);

      this.awaitingPassword = true;
      this.term.write('\n\rEnter password:');
      return;
    } else {
      this.term.write('\n\rPlease provide a valid host');
    }
  }
  if (command === 'clear') {
    this.term.clear();
  } else {
    let response = await HttpClient.httpExecuteBashCommandRequest(command);

    const formattedResponse = response.replace(/\n/g, '\r\n');
    const lines = formattedResponse.split('\n');
    
    lines.forEach((line, index) => {
      if (index == 0)
        this.term.write('\n\r');
      else
        this.term.write('\r'); 
      
      this.term.writeln(line);
    });
    this.updatePS1();
  }
  }

  submitPassword(password: string): void {
    this.vpsPassword = password.trim();
    sessionStorage.setItem('password', this.vpsPassword);
  
    if (this.hostAddress != null) {
      const username = this.hostAddress.split('@')[0];
      const hostName = this.hostAddress.split('@')[1];
      this.term.writeln(`\r\nHost set to: ${this.hostAddress}`);
      this.term.writeln(`Welcome, ${username}@${hostName}`);
      this.term.writeln(`To change the host, enter the command 'sethost username@hostAddress'`);
    
      this.command = '';
      this.awaitingPassword = false;
    }
  }

  promptHost(): void {
    this.term.write('> sethost (example: sethost testuser@test.server)');
    this.term.write('\n\rEnter the host:');
  }

  updatePS1():void {
    this.term.write(`\r\n${this.hostAddress}: `);  
  }
}