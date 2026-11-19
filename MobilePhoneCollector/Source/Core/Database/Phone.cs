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
    [Table("phone")]
    public class Phone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("model")]
        [AllowNull]
        public string Model { get; set; } = string.Empty;

        [Column("brand")]
        [AllowNull]
        public string Brand { get; set; } = string.Empty;

        [Column("announced")]
        [AllowNull]
        public string Announced { get; set; } = string.Empty;

        [Column("info_url")]
        [AllowNull]
        public string InfoUrl { get; set; } = string.Empty;

        [Column("notes")]
        [AllowNull]
        public string Notes { get; set; } = string.Empty;

        [Column("created_date")]
        [AllowNull]
        public string CreatedDate { get; set; } = string.Empty;

        public List<Specs> SpecList { get; set; } = [];

        public List<PhoneImage> ImageList { get; set; } = [];

        public List<PhoneReview> ReviewList { get; set; } = [];
    }
}
