namespace UploadFileProccessBar.Models.DataBase
{
    public class FileUploadContext
    {
        public string Name { get; init; } = null!;
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; }=null!;
        public string Document { get; set; } = null!;
        public string DocumentItem { get; set; } = null!;   

        /*
            "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "FileStore",
    "Document": "Document",
    "DocumentItem": "DocumentItem"
         
         */
    }
}
