using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RC4
{
    //线程间数据传递
    class ThreadHandle
    {
        public SynchronizationContext Context { get; set; }
        public string Arg { get; set; }
        public SendOrPostCallback Callback { get; set; }
        public string Password { get; set; }

        public ThreadHandle(SynchronizationContext context, string password, string arg, SendOrPostCallback callback)
        {
            Context = context;
            Arg = arg;
            Callback = callback;
            Password = password;
        }
    }
}
