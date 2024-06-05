using System;

namespace ZoDream.Shared.Encryptors
{
    public class Encryptor: IEncryptor
    {

        public byte[] Encrypt(byte[] data)
        {
            return data;
        }

        public byte[] Decrypt(byte[] data) 
        {
            return data;
        }
    }
}
