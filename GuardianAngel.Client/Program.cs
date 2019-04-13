using GuardianAngel.Client.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GuardianAngel.Client
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Initialize();
            while (true)
                Console.Read();
        }

        static void Initialize()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            Console.WriteLine($"Guardian Angel Client {Assembly.GetExecutingAssembly().GetName().Version}");
            NewNotifyIcon(new Container(), new ContextMenu(new MenuItem[] { HideConsole(), ShowConsole(), Exit() }));
            SetupFileSystemWatcher(NewFileSystemWatcher(@"C:\Users"));
            Application.Run();
        }

        static FileSystemWatcher NewFileSystemWatcher(string path) =>
            new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

        static NotifyIcon NewNotifyIcon(IContainer container, ContextMenu context) =>
            new NotifyIcon(container)
            {
                ContextMenu = context,
                Icon = Resources.icon,
                Visible = true
            };

        static void SetupFileSystemWatcher(FileSystemWatcher fsw) =>
            fsw.Deleted += new FileSystemEventHandler(OnDeleted);

        static MenuItem Exit() => new MenuItem("Exit", OnExit);
        static MenuItem HideConsole() => new MenuItem("Hide Console", OnHideConsole);
        static MenuItem ShowConsole() => new MenuItem("Show Console", OnShowConsole);

        #region Event Delegates

        static void OnDeleted(object sender, FileSystemEventArgs args)
        {
            if (!args.FullPath.Contains("AppData"))
            {
                Console.WriteLine("DELETED, TIME: " + DateTime.Now);
                Console.WriteLine("DELETED, FULLPATH: " + args.FullPath);
            }
        }

        static void OnExit(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        static void OnHideConsole(object sender, EventArgs args)
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        static void OnShowConsole(object sender, EventArgs args)
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        #endregion

        #region Interop Services

        [DllImport("kernel32")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        #endregion
    }
}
