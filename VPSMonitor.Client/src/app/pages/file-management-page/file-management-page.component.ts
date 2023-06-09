import { HttpFeatureKind } from '@angular/common/http';
import { splitNsName } from '@angular/compiler';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from 'src/HttpClient';
import { sftpData } from 'src/app/entities/sftpData';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-file-management-page',
  templateUrl: './file-management-page.component.html',
  styleUrls: ['./file-management-page.component.scss']
})
export class FileManagementPageComponent {
  isPageLoading: boolean = true
  showAllFiles: boolean = false;
  isAllFilesSelected: boolean = false;
  isModalOpen: boolean = false
  newItemIsFile: boolean = false;
  newItemIsDirectory: boolean = false;
  isButtonDisabled: boolean = true;
  uploadedFiles: FileList | undefined;
  selectedFiles: string[] = []
  filesAndFolders: any[] = [];
  displayedArray: any[] = [];
  currentDirectory: string = '';
  localFilePath: string = '';
  copyToPath: string | undefined | null = '';
  moveToPath: string | undefined | null = '';
  newItemName: string | undefined | null = '';
  selectedFile: any; 
  apiUrl: string = '';
  contextMenuStyle: any = {};
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
    this.setApiUrl();
    this.filesAndFolders = await this.getListFilesAndFolders();
    this.displayedArray = this.filesAndFolders.filter(item => item.name[0] != '.')   
    this.currentDirectory = this.getCurrentDirectory(this.filesAndFolders[0].fullName)
    this.isPageLoading = false;
    console.log(this.currentDirectory);
    
  }
  setApiUrl():void {
    if (environment.production)
      this.apiUrl = environment.apiUrl
    else 
      this.apiUrl = environment.apiUrl
  }

  async getListFilesAndFolders(): Promise<any> {      
    const request = await HttpClient.httpRequest("POST", `${this.apiUrl}/api/Sftp/list`,
      this.body, this.header)
    
    if (request.status == 200)
      return await request.json()
    
    return "";
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
      const request = await fetch(`${this.apiUrl}/api/Sftp/upload`, {
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
        SelectedFiles: []
      }
    } else {
      this.router.navigate(['/terminal'])
    }
  }
  
  onFileCheckboxChange(event: any, file: any) {
    if (event.target.checked) {
      this.selectedFiles.push(file);
      this.isButtonDisabled = false;
    } else {
      const index = this.selectedFiles.indexOf(file);
      if (index > -1) {
        this.selectedFiles.splice(index, 1);
      }
    }
    if (this.selectedFiles.length == 0)
    this.isButtonDisabled = true;
  }
  
  async downloadSelectedFile(downloadFileName?: string) {    
    alert("download")
    if (this.selectedFiles.length == 0) {
      alert("You should choose file to download")
      return;
    }
    
    this.body.SelectedFiles = this.selectedFiles
    const response = await HttpClient.httpRequest("POST", `${this.apiUrl}/api/Sftp/download`, this.body, this.header)
    
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

  openContextMenu(event: MouseEvent, fileOrFolder: any) {
    event.preventDefault();
    this.selectedFile = fileOrFolder;
    this.contextMenuStyle = {
      position: 'absolute',
      top: `${event.clientY}px`,
      left: `${event.clientX}px`
    };
  }

  async deleteSelectedItem(itemName?: string) {
     if (this.selectedFiles.length == 0 && itemName == undefined) {
      alert("You should choose file to delete")
      return;
     }
    if (itemName != null) {
      this.body.SelectedFiles.push(itemName)
    } else {
      this.body.SelectedFiles = this.selectedFiles
    }
    const response = await HttpClient.httpRequest("DELETE", `${this.apiUrl}/api/Sftp/delete`, this.body, this.header)

    if (response.status == 200)
      window.location.reload()
  }

  async renameSelectedItem(fullName: string) {
    this.newItemName = prompt("Enter new name:")
    if (this.newItemName != null) {
      this.body.newItemName = this.newItemName
      this.body.ItemPath = fullName
    }
    const response = await HttpClient.httpRequest("PUT", `${this.apiUrl}/api/Sftp/rename`, this.body, this.header)

    this.newItemName = ''

    if (response.status == 200)
      window.location.reload()
  }

  async copyToSelectedItem(itemPath: string) {
    this.copyToPath = prompt("Enter new path to move item:")
    if (this.copyToPath != null) {
      this.body.DestinationPath = this.copyToPath
      this.body.SourcePath = itemPath

      await HttpClient.httpRequest("PUT", `${this.apiUrl}/api/Sftp/copy`, this.body, this.header)
      this.copyToPath = ''
    }
  }

  async moveToSelectedItem(itemPath: string) {
    this.moveToPath = prompt("Enter new path to move item:")
    if (this.moveToPath != null) {
      this.body.DestinationPath = this.moveToPath
      this.body.SourcePath = itemPath
    }
    
    await HttpClient.httpRequest("PUT", `${this.apiUrl}/api/Sftp/move`, this.body, this.header)
    this.moveToPath = ''
  }

  getCurrentDirectory(path: string) {
    const lastSlashIndex = path.lastIndexOf('/');
    if (lastSlashIndex !== -1) {
      return path.substring(0, lastSlashIndex);
    }
    return path;
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

  async createItem() {
    if(this.newItemName != undefined)
      this.body.newItemName = this.newItemName;
    if (this.newItemIsDirectory)
      this.body.newItemType = "directory"
    else
      this.body.newItemType = "file"
        
    const response = await HttpClient.httpRequest("POST", `${this.apiUrl}/api/Sftp/create`, this.body, this.header)

    if (response.status == 200)
      window.location.reload()
    else
      alert("something going wrong")
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

  handleModalOpened(): void{
    this.isModalOpen = true
  }

  handleModalClosed(): void{
    this.isModalOpen = false
  }

  chooseDirectoryItemType() {
    this.newItemIsFile = false
  }

  chooseFileItemType() {
    this.newItemIsDirectory = false
  }
}
