export interface sftpData {
  Host: string;
  Username: string;
  Password: string;
  DirectoryPath: string;
  LocalFilePath?: string;
  SelectedFiles: string[]
  files?: FileList;
}