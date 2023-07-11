export interface sftpData {
  Host: string;
  Username: string;
  Password: string;
  DirectoryPath: string;
  ItemPath?: string;
  SelectedFiles: string[]
  DestinationPath?: string,
  SourcePath?: string,
  files?: FileList;
  newItemName?: string;
  newItemType?: string;
}