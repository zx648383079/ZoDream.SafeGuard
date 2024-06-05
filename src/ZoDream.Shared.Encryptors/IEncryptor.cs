using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Encryptors
{
    public interface IEncryptor
    {
        public byte[] Encrypt(byte[] data);

        public byte[] Decrypt(byte[] data);
    }
}
