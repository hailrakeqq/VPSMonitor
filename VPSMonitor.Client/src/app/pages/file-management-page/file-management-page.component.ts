import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';
import { sftpData } from 'src/app/entities/sftpData';

@Component({
  selector: 'app-file-management-page',
  templateUrl: './file-management-page.component.html',
  styleUrls: ['./file-management-page.component.scss']
})
export class FileManagementPageComponent {
  showAllFiles = false;
  uploadedFiles: FileList | undefined;
  selectedFiles: string[] = []
  filesAndFolders: any[] = [];
  displayedArray: any[] = [];
  currentDirectory: string = '';
  localFilePath: string = '';
  header = {
    'Authorization': `bearer ${localStorage.getItem('access-token')}`,
    'Content-Type': 'application/json'
  }
  body!: sftpData;

  constructor(private router: Router) {
    this.updateDisplayedArray();
  }

  async ngOnInit() {
    this.createBodyForHttpRequst()
    this.filesAndFolders = await this.getListFilesAndFolders();
    this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')   
    this.currentDirectory = `/${this.filesAndFolders[0].fullName.split('/')[1]}`
  }

  async getListFilesAndFolders(): Promise<any> {      
    const request = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Sftp/list",
      this.body, this.header)
    
    return await request.json()
  }

  updateDisplayedArray() { 
    if (this.showAllFiles) {
      this.displayedArray = this.filesAndFolders;
    } else {
      this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')
    }
  }  

  onFileSelected(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    console.log(inputElement)
    if (inputElement.files && inputElement.files.length > 0) {
      this.uploadedFiles = inputElement.files;
    }
  }

  async uploadFile() {
    if (this.uploadedFiles && this.currentDirectory) {
      const formData = new FormData();
      formData.append('host', this.body.Host)
      formData.append('username', this.body.Username)
      formData.append('password', this.body.Password)
      formData.append('remoteFilePath', this.currentDirectory);

      for (let i = 0; i < this.uploadedFiles.length; i++) {
        formData.append('files', this.uploadedFiles[i]);
      }
      const header = {
        'Authorization': `bearer ${localStorage.getItem('access-token')}`,
      }
      const request = await fetch("https://localhost:5081/api/Sftp/upload", {
        method: "POST",
        headers: header,
        body: formData
      })

      console.log(await request.text());
    }
  }

  private createBodyForHttpRequst(): void { 
    const connectionData = sessionStorage.getItem('host')?.split('@')
    const connectionPassword = sessionStorage.getItem('password');
    const directoryPath = sessionStorage.getItem('DirectoryPath')
    if (connectionData != undefined && connectionPassword != undefined && directoryPath != undefined){
      this.body = {
        Host: connectionData[1],
        Username: connectionData[0],
        Password: connectionPassword,
        DirectoryPath: directoryPath,
      }
    } else {
      this.router.navigate(['/terminal'])
    }
  }

  onFileCheckboxChange(event: any, file: any) {
    if (event.target.checked) {
      this.selectedFiles.push(file);
    } else {
      const index = this.selectedFiles.indexOf(file);
      if (index > -1) {
        this.selectedFiles.splice(index, 1);
      }
    }
  }

  downloadSelectedFile() {
    alert("***download***")
  }

  deleteSelectedFile() {
      alert("***delete***")
  }
}
