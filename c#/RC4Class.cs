using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RC4
{
    class RC4Class
    {
        private byte[] S = new byte[256];
        private byte[] T = new byte[256];
        private int secretKey_i = 0;
        private int secretKey_j = 0;

        public RC4Class(string key)
        {
            //初始化 S和T
            byte[] K = Encoding.ASCII.GetBytes(MD5(key));
            for (int i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
                T[i] = K[i % K.Length];
            }
            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + S[i] + T[i]) % 256;
                swap(S, i, j);
            }
        }

        private string MD5(string data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5_data = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(md5_data).Replace("-", "");
        }

        private void swap(byte[] bs, int i, int j)
        {
            byte t = bs[i];
            bs[i] = bs[j];
            bs[j] = t;
        }

        public byte SecretKey()
        {
            secretKey_i = (secretKey_i + 1) % 256;
            secretKey_j = (secretKey_j + S[secretKey_i]) % 256;
            swap(S, secretKey_i, secretKey_j);
            int t = (S[secretKey_i] + S[secretKey_j]) % 256;
            return S[t];
        }
    }
}
