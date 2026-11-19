using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MobilePhoneCollector.Source.Core.Database
{
    [Table("brands")]
    public class Brands
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [AllowNull]
        public string Name { get; set; } = string.Empty;

        [Column("origin")]
        [AllowNull]
        public string Origin { get; set; } = string.Empty;

        [Column("logo_id")]
        [AllowNull]
        public int? LogoId { get; set; }

        [ForeignKey(nameof(LogoId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public FileData? Data { get; set; }
    }

    /*
     * "1": "Nokia",
    "2": "Ericsson",
    "3": "Siemens",
    "4": "Motorola",
    "5": "alcatel",
    "6": "Panasonic",
    "7": "Sony",
    "8": "Mitsubishi",
    "9": "Samsung",
    "10": "Bosch",
    "11": "Philips",
    "12": "NEC",
    "13": "Sagem",
    "14": "Maxon",
    "15": "Benefon",
    "16": "Telit",
    "17": "Kyocera",
    "18": "Sendo",
    "19": "Sony Ericsson",
    "20": "LG",
    "21": "Tel.Me.",
    "22": "Neonode",
    "23": "Sharp",
    "24": "Chea",
    "25": "Mitac",
    "26": "Sewon",
    "27": "Palm",
    "28": "Amoi",
    "29": "Innostream",
    "30": "O2",
    "31": "BenQ",
    "32": "Pantech",
    "33": "Haier",
    "34": "Bird",
    "35": "i-mate",
    "36": "BlackBerry",
    "37": "VK Mobile",
    "38": "Qtek",
    "39": "Vertu",
    "40": "Eten",
    "41": "HP",
    "42": "BenQ-Siemens",
    "43": "XCute",
    "44": "Toshiba",
    "45": "HTC",
    "46": "Asus",
    "47": "Gigabyte",
    "48": "Apple",
    "49": "Thuraya",
    "50": "Fujitsu Siemens",
    "51": "WND",
    "52": "i-mobile",
    "53": "Vodafone",
    "54": "Sonim",
    "55": "T-Mobile",
    "56": "MWg",
    "57": "AT&T",
    "58": "Huawei",
    "59": "Acer",
    "60": "iNQ",
    "61": "Dell",
    "62": "ZTE",
    "63": "Modu",
    "64": "Microsoft",
    "65": "Garmin-Asus",
    "66": "Micromax",
    "67": "BLU",
    "68": "Spice",
    "69": "Icemobile",
    "70": "verykool",
    "71": "Orange",
    "72": "Plum",
    "73": "Lenovo",
    "74": "Meizu",
    "75": "Celkon",
    "76": "Amazon",
    "77": "Casio",
    "78": "Yezz",
    "79": "NIU",
    "80": "Xiaomi",
    "81": "Parla",
    "82": "Oppo",
    "83": "Karbonn",
    "84": "Jolla",
    "85": "XOLO",
    "86": "Prestigio",
    "87": "Maxwest",
    "88": "Allview",
    "89": "Cat",
    "90": "Archos",
    "91": "Unnecto",
    "92": "Gionee",
    "93": "Emporia",
    "94": "Lava",
    "95": "OnePlus",
    "96": "Wiko",
    "97": "Nvidia",
    "98": "vivo",
    "99": "Yota",
    "100": "YU",
    "101": "Posh",
    "102": "Intex",
    "103": "QMobile",
    "104": "EE",
    "105": "Coolpad",
    "106": "Energizer",
    "107": "Google",
    "108": "BQ",
    "109": "LeEco",
    "110": "",
    "116": "Blackview",
    "117": "Razer",
    "118": "Realme",
    "119": "Infinix",
    "120": "Tecno",
    "121": "Honor",
    "123": "TCL",
    "124": "Ulefone",
    "127": "Fairphone",
    "128": "Nothing",
    "129": "Doogee",
    "130": "Cubot",
    "131": "itel",
    "132": "Oukitel",
    "133": "HMD",
    "134": "Oscal",
    "135": "Umidigi",
    "136": "RugOne",
    "137": "ZEED"
     */
}
