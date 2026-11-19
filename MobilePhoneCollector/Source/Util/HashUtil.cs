using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MobilePhoneCollector.Source.Util
{
    public class HashUtil
    {
        public static string ByteToMd5(byte[] data)
        {
            byte[] hashBytes = MD5.HashData(data);
            return Convert.ToHexString(hashBytes);
        }
    }
}
