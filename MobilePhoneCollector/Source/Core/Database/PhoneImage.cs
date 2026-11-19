using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MobilePhoneCollector.Source.Core.Database
{
    [Table("phone_image")]
    public class PhoneImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("phone_id")]
        [AllowNull]
        public int PhoneId { get; set; }

        [Column("data_id")]
        [AllowNull]
        public int DataId { get; set; }

        [Column("width")]
        [AllowNull]
        public int Width { get; set; }

        [Column("height")]
        [AllowNull]
        public int Height { get; set; }

        [Column("file_type")]
        [AllowNull]
        public string FileType { get; set; } = string.Empty;

        [Column("file_name")]
        [AllowNull]
        public string FileName { get; set; } = string.Empty;

        [Column("is_cover")]
        public bool IsCover { get; set; } = false;

        [Column("created_date")]
        [AllowNull]
        public string CreatedDate { get; set; } = string.Empty;

        [ForeignKey(nameof(PhoneId))]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Phone? Phone { get; set; }

        [ForeignKey(nameof(DataId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public FileData? Data { get; set; }

        [NotMapped]
        public bool Existing { get; set; } = false;
    }
}
