import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';

@Component({
  selector: 'app-file-management-page',
  templateUrl: './file-management-page.component.html',
  styleUrls: ['./file-management-page.component.scss']
})
export class FileManagementPageComponent {
  showAllFiles = false;
  filesAndFolders: any[] = [];
  displayedArray: any[] = [];
  remoteFilePath: string = '';
  localFilePath: string = '';
  selectedFile: File | undefined;
  header = {
    'Authorization': `bearer ${localStorage.getItem('access-token')}`,
    'Content-Type': 'application/json'
  }

  constructor(private router: Router) {
    this.updateDisplayedArray();
  }

  async ngOnInit() {
    this.filesAndFolders = await this.getListFilesAndFolders();
    this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')   
  }

  async getListFilesAndFolders(): Promise<any> {
    const connectionData = sessionStorage.getItem('host')?.split('@')
    const connectionPassword = sessionStorage.getItem('password');

    if (connectionData != undefined && connectionPassword != undefined) {
      const body = {
        Host: connectionData[1],
        Username: connectionData[0],
        Password: connectionPassword,
        UserHomeDirectoryPath: sessionStorage.getItem('userHomeDirectoryPath')
      }
      
      const request = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Sftp/list",
        body, this.header)
      
      return await request.json()
    } else {
      this.router.navigate(['/signin'])
    }
  }

  updateDisplayedArray() { 
    if (this.showAllFiles) {
      this.displayedArray = this.filesAndFolders;
    } else {
      this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')
    }
  }  
}
