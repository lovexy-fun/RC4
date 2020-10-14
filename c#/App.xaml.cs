using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RC4
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public SynchronizationContext Context { get; }

        public App()
        {
            Context = new SynchronizationContext();
        }

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        static void Main(string[] args)
        {
            App app = new App();
            MainWindow mainWindow = new MainWindow(args, app.Context);
            mainWindow.Show();
            app.MainWindow = mainWindow;
            app.Run();
        }
    }    
}
