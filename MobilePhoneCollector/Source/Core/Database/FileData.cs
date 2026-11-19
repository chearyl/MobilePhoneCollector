using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePhoneCollector.Source.Core.Database
{
    [Table("file_data")]
    public class FileData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("hash_code")]
        [AllowNull]
        public string HashCode { get; set; } = string.Empty;

        [Column("data")]
        [AllowNull]
        public byte[]? Data { get; set; }
    }
}
