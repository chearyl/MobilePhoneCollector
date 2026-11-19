using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePhoneCollector.Source.Core.Constants
{
    public class GsmArenaDataSpec
    {
        public static readonly string[] DataSpec =
            [
                "nettech", "net2g", "net3g", "net4g", "net5g", "gprstext", "edge",
                "year",
                "dimensions", "weight", "build", "sim", "bodyother",
                "displaytype", "displaysize", "displayresolution", "displayother",
                "os", "chipset", "cpu", "gpu",
                "memoryslot", "internalmemory", "memoryother",
                "cam1modules", "cam1features", "cam1video",
                "cam2modules", "cam2features", "cam2video",
                "loudspeaker", "headphonejack",//audio
                "wlan", "bluetooth", "gps", "nfc", "radio", "usb", "infrared",
                "sensors", "featuresother",
                "batdescription1", "charging",//charging
                "colors", 
            ];

        public static readonly Dictionary<string, string> GsmToActual = new()
        {
            {"nettech", "Network"}, {"net2g", "2G"}, {"net3g", "3G"}, {"net4g", "4G"}, {"net5g", "5G"}, {"gprstext", "GPRS"}, {"edge", "EDGE"},
            {"year", "Announced"},
            {"dimensions", "Dimension"}, {"weight", "Weight"}, {"build", "Build"}, {"sim", "Sim"}, {"bodyother", "Other"},
            {"displaytype", "Display Type"}, {"displaysize", "Display Size"}, {"displayresolution", "Display Resolution"}, {"displayother", "Display Other"},
            {"os", "OS"}, {"chipset", "Chipset"}, {"cpu", "CPU"}, {"gpu", "GPU"},
            {"memoryslot", "Memory Slot"}, {"internalmemory", "Internal Memory"}, {"memoryother", "Memory Other"},
            {"cam1modules", "Main Camera"}, {"cam1features", "Main Camera Feature"}, {"cam1video", "Main Camera Video"},
            {"cam2modules", "Secondary Camera"}, {"cam2features", "Secondary Camera Feature"}, {"cam2video", "Secondary Camera Video"},
            {"loudspeaker", "Loudspeaker"}, {"headphonejack", "3.5mm Headphone Jack"},
            {"wlan", "WLAN"}, {"bluetooth", "Bluetooth"}, {"gps", "GPS"}, {"nfc", "NFC"}, {"radio", "Radio"}, {"usb", "USB"}, {"infrared", "Infrared"},
            {"sensors", "Sensors"}, {"featuresother", "Other Features"},
            {"batdescription1", "Battery"}, {"charging", "Charging"},
            {"colors", "Colors"},
        };

        public static readonly Dictionary<string, string> SpecCategory = new()
        {
            {"nettech", "Network"}, {"net2g", "Network"}, {"net3g", "Network"}, {"net4g", "Network"}, {"net5g", "Network"}, {"gprstext", "Network"}, {"edge", "Network"},
            {"year", "Launch"},
            {"dimensions", "Body"}, {"weight", "Body"}, {"build", "Body"}, {"sim", "Body"}, {"bodyother", "Body"},
            {"displaytype", "Display"}, {"displaysize", "Display"}, {"displayresolution", "Display"}, {"displayother", "Display"},
            {"os", "Platform"}, {"chipset", "Platform"}, {"cpu", "Platform"}, {"gpu", "Platform"},
            {"memoryslot", "Memory"}, {"internalmemory", "Memory"}, {"memoryother", "Memory"},
            {"cam1modules", "Main Camera"}, {"cam1features", "Main Camera"}, {"cam1video", "Main Camera"},
            {"cam2modules", "Secondary Camera"}, {"cam2features", "Secondary Camera"}, {"cam2video", "Secondary Camera"},
            {"loudspeaker", "Audio"}, {"headphonejack", "Audio"},
            {"wlan", "Connections"}, {"bluetooth", "Connections"}, {"gps", "Connections"}, {"nfc", "Connections"}, {"radio", "Connections"}, {"usb", "Connections"}, {"infrared", "Connections"},
            {"sensors", "Features"}, {"featuresother", "Features"},
            {"batdescription1", "Battery"}, {"charging", "Battery"},
            {"colors", "MISC"},
        };
    }
}
