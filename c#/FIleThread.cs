using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RC4
{
    class FileThread
    {
        public static int BUF_SIZE_1MB = 1048576;

        public static void crypt(object state)
        {
            ThreadHandle handle = (ThreadHandle) state;
            RC4Class rc4 = new RC4Class(handle.Password);

            FileStream fRead = new FileStream(handle.Arg, FileMode.Open);
            FileStream fWrite = new FileStream(handle.Arg + ".rc4", FileMode.Create);

            byte[] buffer = new byte[BUF_SIZE_1MB];
            while(true)
            {
                int count = fRead.Read(buffer, 0, buffer.Length);
                for(int i = 0; i < count; i++)
                {
                    buffer[i] = (byte) (rc4.SecretKey() ^ buffer[i]); 
                }
                fWrite.Write(buffer, 0, count);
                fWrite.Flush();
                handle.Context.Post(handle.Callback, count);
                if (count == 0)
                {
                    fRead.Close();
                    fWrite.Close();
                    File.Delete(handle.Arg);
                    File.Move(handle.Arg + ".rc4", handle.Arg);
                    break;
                }
            }
        }
    }
}
