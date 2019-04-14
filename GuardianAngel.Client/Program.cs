using GuardianAngel.Client.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GuardianAngel.Client
{
    class Program
    {
        static List<string> Files { get; set; } = new List<string>();

        static readonly string path = $@"C:\Users\{Environment.UserName}";

        [STAThread]
        static void Main(string[] args)
        {
            PopulateFiles(path, ProcessFile);
            Initialize(path);
            while (true)
                Console.Read();
        }

        static void Initialize(string path)
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            NewNotifyIcon(new Container(), new ContextMenu(new MenuItem[] { HideConsole(), ShowConsole(), Exit() }));
            SetupFileSystemWatcher(NewFileSystemWatcher(path));
            Console.WriteLine($"Guardian Angel Client {Assembly.GetExecutingAssembly().GetName().Version}");
            Application.Run();
        }

        static void PopulateFiles(string path, Action<string> action)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                try { ProcessFile(file); }
                catch { continue; }
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                try { PopulateFiles(dir, action); }
                catch { continue; }
            }
        }

        static void ProcessFile(string file) => Files.Add(file);

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

        static void SetupFileSystemWatcher(FileSystemWatcher fsw)
        {
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Created += new FileSystemEventHandler(OnCreated);
            fsw.Deleted += new FileSystemEventHandler(OnDeleted); 
            fsw.Renamed += new RenamedEventHandler(OnRenamed);
        }

        static MenuItem Exit() => new MenuItem("Exit", OnExit);
        static MenuItem HideConsole() => new MenuItem("Hide Console", OnHideConsole);
        static MenuItem ShowConsole() => new MenuItem("Show Console", OnShowConsole);

        #region Event Delegates

        static void OnChanged(object sender, FileSystemEventArgs args)
        {
            if (!args.FullPath.Contains("AppData"))
                Console.WriteLine($"[{DateTime.Now}] [{args.ChangeType}]\n    {args.FullPath}");
        }

        static void OnCreated(object sender, FileSystemEventArgs args)
        {
            if (!args.FullPath.Contains("AppData"))
                Console.WriteLine($"[{DateTime.Now}] [{args.ChangeType}]\n    {args.FullPath}");
        }

        static void OnDeleted(object sender, FileSystemEventArgs args)
        {
            if (!args.FullPath.Contains("AppData"))
                Console.WriteLine($"[{DateTime.Now}] [{args.ChangeType}]\n    {args.FullPath}");
        }

        static void OnRenamed(object sender, RenamedEventArgs args)
        {
            if (!args.FullPath.Contains("AppData"))
                Console.WriteLine($"[{DateTime.Now}] [{args.ChangeType}]\n    {args.FullPath}");
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
