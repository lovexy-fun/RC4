using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RC4
{
    public partial class MainWindow : Window
    {
        private string[] args;//main方法参数表
        private int validArgNum = 0;//有效参数个数 
        private long totalSize = 0;//文件的总大小
        private long size = 0;//当前已经处理的大小
        private readonly int maxThreads = 4;//最大线程数量
        private readonly SynchronizationContext context;//主线程上下文
        private readonly object lockObj = new object();//锁对象
        private DateTime startTime;//开始时间
        private DateTime endTime;//结束时间

        public MainWindow(string[] args, SynchronizationContext context)
        {
            InitializeComponent();
            this.context = context;
            this.args = args;
            ArgsPretreatment();
            MsgTextBlock.Text = string.Format(Constant.Tip1, validArgNum);
        }

        private void Run_RC4(object sender, RoutedEventArgs e)
        {
            string password = GetPassword();
            if(password != null)
            {
                OKBtn.IsEnabled = false;
                startTime = DateTime.Now;
                FileHandle(password);
            }
        }

        //文件名参数预处理
        private void ArgsPretreatment()
        {
            if (args.Length == 0)
            {
                MessageBox.Show(Constant.Tip4);
                Environment.Exit(0);
            }
            int count = 0;
            for (int i = 0; i < args.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(args[i]);
                if (!fileInfo.Exists || fileInfo.Length == 0)
                {
                    args[i] = null;
                    count++;
                }
                else
                {
                    validArgNum++;
                    totalSize += fileInfo.Length;
                }
            }
            if(count != 0 && validArgNum != 0)
            {
                MessageBox.Show(string.Format(Constant.Tip2, count));
            }
            if(validArgNum == 0)
            {
                MessageBox.Show(Constant.Tip6);
                Environment.Exit(0);
            }
        }

        //文件处理
        private void FileHandle(string password)
        {
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(DispatchThread);
            new Thread(threadStart).Start(password);
        }

        //获取密码
        private string GetPassword()
        {
            string password = PasswordBox.Password;
            if(string.IsNullOrEmpty(password))
            {
                MessageBox.Show(Constant.Tip3);
                return null;
            }
            return password;
        }

        //调度线程
        private void DispatchThread(object state)
        {
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            for (int i = 0; i < args.Length; i++)
            {
                if(args[i] != null)
                {
                    ThreadPool.QueueUserWorkItem(FileThread.crypt, new ThreadHandle(context, (string) state, args[i], new SendOrPostCallback(ThreadCallBack)));
                }
            }
        }

        //线程回到
        private void ThreadCallBack(object state)
        {
            Dispatcher.Invoke(() =>
            {
                lock (lockObj)
                {
                    size += (int) state;
                    ProgressBar.Value = ((double)size / totalSize) * ProgressBar.Maximum;
                    MsgTextBlock.Text = string.Format(Constant.Tip7, ProgressBar.Value);
                }
                if(size == totalSize)
                {
                    endTime = DateTime.Now;
                    string msg = string.Format(Constant.Tip5, (endTime - startTime).TotalSeconds);
                    MsgTextBlock.Text = msg;
                    MessageBox.Show(msg);
                    Environment.Exit(0);
                }
            });
        }
    }
}
