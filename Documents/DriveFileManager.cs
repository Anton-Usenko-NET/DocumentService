using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Drive.v3.Data;
using static Google.Apis.Drive.v3.FilesResource;
using Google.Apis.Download;
using Google.Apis.Sheets.v4;
using DocumentService.Documents;
//https://console.developers.google.com/apis/api/sheets.googleapis.com/overview?project=200596342819
namespace Documents
{
    public class DriveFileManager : IGoogleDriveService
    {
        static string[] Scopes = { "https://www.googleapis.com/auth/drive", "https://www.googleapis.com/auth/documents" };
        static string ApplicationName = "TEM";
        UserCredential credential;
        public static string TemplateContract =  "1PbJyJ1PzEUc6kdAF2yUiF4TPPCZYKusd7TdCE4gdNA0";
        public static string TemplateContract2 = "1wdy7If4VLSU4lqM7r0NaAmpSdP9oVBUOg8TdSbYw3T4";
        public static string TemplateContract3 = "1pSLFaUuBM-9DDDiEIvw9EL-unsS5MRQbvgJ9OOBxQqk";
        public static string TemplateContract4 = "1vIJXsr85Kn4mx4JU9YW5F23Uf21gUyaHtnLx6ODAmNU";

        public static string TemplateTTNSheetId =               "19HdepTKLBBSXnxlwL9asShw9kiSkf3TJnImiUC05L1s";
        public static int TemplateTTNSheetGrid =               583253378;
        public static string TemplateExpenceInvoiceSheetId =    "1hPygv5_LQPx_1RmJmPZau1apdioMfvrQRmEYVosj9Gg";
        public static int TemplateExpenceInvoiceSheetGrid =    55539154;
        public static string TemplateInvoiceForPaymentSheetId =   "1BOOzbt-UBX0OvKMdjy1eSSEaurbM4O2txDK0F2ebhyg";
        public static int TemplateInvoiceForPaymentSheetGrid = 561611553;


        public static string TemplateData = "1xnX3KMYord1psCHvb7fupfyy1IKyWE9tOoyEHC95L-M";
        public static string FolderResultId = "1Eho8PykrIHUwU7fnQyaRM8CR9isxQP8_";
        DocsService docService;
        SheetsService sheetsService;
        DriveService _driveService;
        public DriveFileManager()
        {
            Authentification();
            // Create Google Docs API service.
            docService = new DocsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
            sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
            // Create Drive API service.
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
        private void Authentification()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            try
            {
                // Load client secrets.
                using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void DeleteFile(string file_id)
        {
            DeleteRequest request = _driveService.Files.Delete(file_id);
            string s = request.Execute();
        }
        public Google.Apis.Drive.v3.Data.File GetFile(string fileId)
        {
            try
            {
                FilesResource.GetRequest request = _driveService.Files.Get(fileId);
                // Optionally, specify the fields you want to retrieve to optimize the request
                request.Fields = "id, name, mimeType, size, createdTime, modifiedTime, parents";

                Google.Apis.Drive.v3.Data.File file = request.Execute();
                return file;
            }
            catch (Google.GoogleApiException ex)
            {
                // Handle API errors, e.g., file not found, permission issues
                System.Console.WriteLine($"Error getting file: {ex.Message}");
                return null;
            }
        }
        public Google.Apis.Drive.v3.Data.File CopyFile(string fileId, string name, string? parentFolder = null)
        {
            if (parentFolder is null)
                parentFolder = DriveFileManager.FolderResultId;
            var dataFile = new Google.Apis.Drive.v3.Data.File();
            dataFile.Name = name;
            dataFile.Parents = new List<string>() { parentFolder };
            FilesResource.CopyRequest request1 = _driveService.Files.Copy(dataFile, fileId);
            request1.SupportsAllDrives = true;
            return request1.Execute();
        }
        
        public string CreateFolder(string name, string parentId = null)
        {
            if (parentId is null)
                parentId = DriveFileManager.FolderResultId;
            var dataFile = new Google.Apis.Drive.v3.Data.File();
            dataFile.MimeType = "application/vnd.google-apps.folder";
            dataFile.Name = name;
            dataFile.Parents = new List<string>() { parentId };

            FilesResource.CreateRequest request1 = _driveService.Files.Create(dataFile);
            request1.SupportsAllDrives = true;
            var folder = request1.Execute();
            return folder.Id;
        }
        public BatchUpdateDocumentResponse UpdateDocument(string documentId, Dictionary<string, string> textToReplace)
        {

            List<Request> requests = new List<Request>();

            foreach (var text in textToReplace)
            {
                var repl = new Request();
                var substrMatchCriteria = new SubstringMatchCriteria();
                var replaceAlltext = new ReplaceAllTextRequest();

                replaceAlltext.ReplaceText = text.Value;
                substrMatchCriteria.Text = text.Key;

                replaceAlltext.ContainsText = substrMatchCriteria;
                repl.ReplaceAllText = replaceAlltext;

                requests.Add(repl);
            }
            BatchUpdateDocumentRequest body = new BatchUpdateDocumentRequest { Requests = requests };

            var res = docService.Documents.BatchUpdate(body, documentId).Execute();
            return res;
        }
        public Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetResponse UpdateSheet(string sheetId, int grid, Dictionary<string, string> textToReplace)
        {

            List<Google.Apis.Sheets.v4.Data.Request> requests = new List<Google.Apis.Sheets.v4.Data.Request>();

            foreach (var text in textToReplace)
            {
                var repl = new Google.Apis.Sheets.v4.Data.Request();
                var findReplace = new Google.Apis.Sheets.v4.Data.FindReplaceRequest();
                findReplace.Find = text.Key;
                findReplace.Replacement = text.Value;
                findReplace.Range = new Google.Apis.Sheets.v4.Data.GridRange();
                findReplace.Range.SheetId = grid;
                repl.FindReplace = findReplace;
                
                requests.Add(repl);
            }
            Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest body = new Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest { Requests = requests };

            var res = sheetsService.Spreadsheets.BatchUpdate(body, sheetId).Execute();
            return res;
        }
        private void downloadDocAsPdf(string documentId)
        {
            ExportRequest exportRequest = _driveService.Files.Export(documentId, "application/pdf");
        }
        private void InsertPermission(string fileId)
        {
            Permission permission = new Permission();
            permission.Role = "writer";
            permission.Type = "anyone";
            var resultPerm = _driveService.Permissions.Create(permission, fileId).Execute(); ;
            ;
        }
        public MemoryStream DownloadDocAsPdf(string fileId)
        {
            DriveService service = _driveService;
            FilesResource.ExportRequest request = service.Files.Export(fileId, "application/pdf");
            MemoryStream outputStream = new MemoryStream();
            bool done = false;
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            done = true;
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }

            };
            request.Download(outputStream);
            while (!done)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            return outputStream;
        }
        public static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }
        public string CopyAndUpdateRelation2(Dictionary<string, string> updateDictionary, string templateId, string words)
        {
            string name = updateDictionary.GetValueOrDefault("{{НазваниеДоговора}}")!;
            Google.Apis.Drive.v3.Data.File file;
            if (!name.Equals("{{Авто}}"))
                file = CopyFile(templateId, name);
            else
            {
                string buf = updateDictionary.GetValueOrDefault("{{Постачальник_особа}}")!;
                buf = buf.Remove(buf.IndexOf(" "));
                string fileName = updateDictionary.GetValueOrDefault("{{Форма власності}}") + " "+buf;
                   
                file = CopyFile(templateId,
                    fileName+" Договір "+ words +" " + DateTime.Now.ToString());
            }
            //InsertPermission(file.Id);
            UpdateDocument(file.Id, updateDictionary);
            return file.Id;
        }
        public string CopyAndUpdateRelationSheet2(Dictionary<string, string> updateDictionary, string templateId, int grid, string name)
        {
            
            Google.Apis.Drive.v3.Data.File file;
            if (!name.Equals("{{Авто}}"))
                file = CopyFile(templateId, name);
            else
            {
                string buf = updateDictionary.GetValueOrDefault("{{Постачальник_особа}}");
                buf = buf.Remove(buf.IndexOf(" "));
                string fileName = updateDictionary.GetValueOrDefault("{{Форма власності}}") + " " + buf;

                file = CopyFile(templateId, buf+" Договор " + DateTime.Now.ToString());
            }
            //InsertPermission(file.Id);
            UpdateSheet(file.Id,grid, updateDictionary);
            return file.Id;
        }
        public string CopyAndUpdateRelation(Dictionary<string, string> updateDictionary, string templateId, string parentFolder = null)
        {
            string name = updateDictionary.GetValueOrDefault("{{ContractNumber}}") + " (" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + ")";
            var file = CopyFile(templateId, name, parentFolder);
            //InsertPermission(file.Id);
            UpdateDocument(file.Id, updateDictionary);
            return file.Id;
        }
        public string CopyAndUpdateRelationSheet(Dictionary<string, string> updateDictionary, string templateId, int grid, string parentFolder = null)
        {
            string name = updateDictionary.GetValueOrDefault("{{ContractNumber}}") + " (" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + ")";
            var file = CopyFile(templateId, name, parentFolder);
            //InsertPermission(file.Id);
            UpdateSheet(file.Id,grid, updateDictionary);
            return file.Id;
        }
        public Dictionary<string,string> GetDictionaryDataFromDoc(string docId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var document = docService.Documents.Get(docId).Execute();
            var table = document.Body.Content.Where(l => l.Table != null).First().Table;
            //(new System.Collections.Generic.ICollectionDebugView<Google.Apis.Docs.v1.Data.ParagraphElement>((new System.Collections.Generic.ICollectionDebugView<Google.Apis.Docs.v1.Data.StructuralElement>((new System.Collections.Generic.ICollectionDebugView<Google.Apis.Docs.v1.Data.TableCell>((new System.Collections.Generic.ICollectionDebugView<Google.Apis.Docs.v1.Data.TableRow>((new System.Collections.Generic.ICollectionDebugView<Google.Apis.Docs.v1.Data.StructuralElement>(document.Body.Content).Items[2]).Table.TableRows).Items[0]).TableCells).Items[0]).Content).Items[0]).Paragraph.Elements).Items[0]).TextRun.Content
            foreach (var r in table.TableRows)
            {
                string s1 = r.TableCells[0].Content[0].Paragraph.Elements[0].TextRun.Content.Trim();
                string s2 = r.TableCells[1].Content[0].Paragraph.Elements[0].TextRun.Content.Trim();
                if(!s1.Equals(""))
                    dic.Add(s1, s2);

            }
            return dic;
        }
    }
}
