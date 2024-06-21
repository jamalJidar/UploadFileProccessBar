using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace UploadFileProccessBar.Models
{
    public enum DocumentType
    {
        IMG=0,
        VIDEO=1, 
        AUDIO, 
        OTHER
    }
    public class Documents
    {

        [Key]
        public Guid Id { get; set; }
         public string DocumentType { get; set; }
         
        /// <summary>
        /// طول کل فایل
        /// </summary>
        public Double FileLength { get; set; }
        /// <summary>
        /// چه انداز از فایل آپلود شده است
        /// </summary>
        public long LengthUpload { get; set; }


        public DateTime CreateAt { get; set; }
        public int CountItem { get; set; }

    }

    public class DocumentItem
    {

        [Key]
        public Guid Id { get; set; }
        public byte[] Document { get; set; } = null!;
        public Guid DocumentId { get; set; }
      //  [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
       // public MessageDocumentStatus Status { get; set; }
        /// <summary>
        /// چه انداز از فایل آپلود شده است
        /// </summary>
        public long LengthUpload { get; set; }

        public int Index { get; set; }

        public bool Completed { get; set; }

    }

}
