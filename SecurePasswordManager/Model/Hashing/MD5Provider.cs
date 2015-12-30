using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace SecurePasswordManager.Model.Hashing
{
    public class MD5Provider : IHashingServiceProvider
    {
        CryptographicHash cryptHash = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5).CreateHash();

        public string Hash(string input)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
            cryptHash.Append(buffer);
            buffer = cryptHash.GetValueAndReset();

            return CryptographicBuffer.EncodeToHexString(buffer);
        }
    }
}
