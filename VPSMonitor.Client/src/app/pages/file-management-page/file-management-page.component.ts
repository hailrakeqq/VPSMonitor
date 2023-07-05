import { splitNsName } from '@angular/compiler';
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
  isAllFilesSelected = false;
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

  selectAllFiles() {
    for (let fileOrFolder of this.displayedArray) {
      fileOrFolder.isSelected = this.isAllFilesSelected;
    }
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
        LocalFilePath: '',
        SelectedFiles: []
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

  async downloadSelectedFile() {
    alert("***download***\n" + this.selectedFiles.toString().split(',').join('\n'))
    
    const header = {
      'Authorization': `bearer ${localStorage.getItem('access-token')}`,
      'Content-Type': 'application/json'
    }

    this.body.SelectedFiles = this.selectedFiles

    this.body.LocalFilePath = "/home/hailrake/media/Download"
    const request = await fetch("https://localhost:5081/api/Sftp/download", {
      method: "POST",
      headers: header,
      body: JSON.stringify(this.body),
    })  

    if (request.status == 200) {      
      const blob = await request.blob()
      console.log(blob);
      
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url
      link.download = this.getParsedFilename(this.selectedFiles[0])
   
      link.click()
      URL.revokeObjectURL(url)
  
    } else {
      console.log(await request.text());  
    }  
  }

  deleteSelectedFile() {
      alert("***delete***\n" + this.selectedFiles.toString().split(',').join('\n'))
  }

  getParsedFilename(filePath: string): string {
    const parsedPath = filePath.split('/');
    const filename = parsedPath[parsedPath.length - 1];
    return filename;
  }
}
