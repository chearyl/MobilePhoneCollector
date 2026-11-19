using Microsoft.EntityFrameworkCore;
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
    [Table("specs")]
    public class Specs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("phone_id")]
        [AllowNull]
        public int PhoneId { get; set; }

        [Column("category")]
        [AllowNull]
        public string Category { get; set; } = string.Empty;

        [Column("key")]
        [AllowNull]
        public string Key { get; set; } = string.Empty;

        [Column("value")]
        [AllowNull]
        public string Value { get; set; } = string.Empty;

        [ForeignKey(nameof(PhoneId))]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Phone? Phone { get; set; }

        [NotMapped]
        public bool ToDelete { get; set; } = false;

        [NotMapped]
        public bool NewAdd { get; set; } = false;

        [NotMapped]
        public int TempId { get; set; }
    }
}
