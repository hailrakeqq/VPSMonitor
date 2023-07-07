import { HttpFeatureKind } from '@angular/common/http';
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
  isPageLoading: boolean = true
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
    this.isPageLoading = false;
  }

  async getListFilesAndFolders(): Promise<any> {      
    const request = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Sftp/list",
      this.body, this.header)
    
    if (request.status == 200)
      return await request.json()
    
    return "";
    // let result = ''
    // try {
    //   result = await request.json()
    // } catch {
    //   result = await request.text()
    // } finally { 
    //   return result
    // }
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

      if (request.status == 200)
        window.location.reload()
      else
        alert("Error was occurate when you tried to delete files");
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

  async downloadSelectedFile(downloadFileName?: string) {    
    this.body.SelectedFiles = this.selectedFiles
    const response = await HttpClient.httpRequest("POST", "https://localhost:5081/api/Sftp/download", this.body, this.header)
  
    if (response.status == 200) {    
      const blob = await response.blob()

      if(this.isAllFilesAndDirectorySelected())
        this.downloadBlob(blob, this.currentDirectory)
      else
        this.downloadBlob(blob)
      
    } else {
      console.log(await response.text());  
    }  
  }

  async deleteSelectedFile() {
    this.body.SelectedFiles = this.selectedFiles
    const response = await HttpClient.httpRequest("DELETE", "https://localhost:5081/api/Sftp/delete", this.body, this.header)

    if (response.status == 200)
      window.location.reload()
  }

  getParsedFilename(filePath: string): string {
    const parsedPath = filePath.split('/');
    return parsedPath[parsedPath.length - 1];
  }

  isAllFilesAndDirectorySelected(): boolean {
    for (let file of this.filesAndFolders) 
      if (!file.isSelected)
        return false
    
    return true
  }

  downloadBlob(blob: Blob, filename?: string) {
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url
      
    if (blob.type == "application/zip" && this.selectedFiles.length == 1) 
      link.download = `${this.getParsedFilename(this.selectedFiles[0])}.zip`
    else if (blob.type == "application/zip" && this.isAllFilesAndDirectorySelected() && filename != undefined)
      link.download = filename
    else if (this.selectedFiles.length == 1) 
      link.download = this.getParsedFilename(this.selectedFiles[0])
    else link.download = "archive.zip"
      
      link.click()
      URL.revokeObjectURL(url)
  }

  async openItem(fileOrFolderName: string) {   
    this.isPageLoading = true;
    const itemName = this.getParsedFilename(fileOrFolderName)

    this.currentDirectory = fileOrFolderName
    this.body.DirectoryPath = fileOrFolderName
    const response = await this.getListFilesAndFolders()

    if (response == "")
      this.filesAndFolders = ["empty dir or file"]
    else {
      this.filesAndFolders = response;
      this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')
    }
 
    this.isPageLoading = false
  }

  getWords(text: string): string[] {
    return text.split('/').filter(word => word.trim() !== '');
  }
  
  async redirectToPreviusDirectory(item: string): Promise<void> {
    if(item == 'root')
      await this.openItem(`/${item}`)
    else 
      await this.openItem(item)
  }

  addSlashIfNeeded(word: string): string {
    if (word === 'root') {
      return '/' + word + '/';
    }
    return word + '/';
  }

  isFolder(itemName: string): boolean {
    if (!itemName.startsWith('.') && itemName.includes('.'))
      return false;
    return true;
  }
}
