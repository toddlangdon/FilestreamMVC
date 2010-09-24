namespace FilestreamMVC.Models
{
    public class Document
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public byte[] Context { get; set; }
        public byte[] Bytes { get; set; }
        public string ContentType
        {
            get
            {

                string contentType;
                switch (this.Extension)
                {
                    case "txt":
                        contentType = "text/plain";
                        break;
                    case "htm":
                    case "html":
                        contentType = "text/html";
                        break;
                    case "rtf":
                        contentType = "text/richtext";
                        break;
                    case "jpg":
                    case "jpeg":
                        contentType = "image/jpeg";
                        break;
                    case "gif":
                        contentType = "image/gif";
                        break;
                    case "bmp":
                        contentType = "image/bmp";
                        break;
                    case "mpg":
                    case "mpeg":
                        contentType = "video/mpeg";
                        break;
                    case "avi":
                        contentType = "video/avi";
                        break;
                    case "pdf":
                        contentType = "application/pdf";
                        break;
                    case "doc":
                    case "docx":
                    case "dot":
                        contentType = "application/msword";
                        break;
                    case "csv":
                    case ".xls":
                    case ".xlsx":
                    case ".xlt":
                        contentType = "application/vnd.msexcel";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;
                }

                return contentType;
            }
        }
    }
}