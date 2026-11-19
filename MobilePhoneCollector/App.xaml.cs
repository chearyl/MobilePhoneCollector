using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using MobilePhoneCollector.Source.Core.Constants;
using MobilePhoneCollector.Source.Core.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public static Window? Window { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            _ = ConfigureDb();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Closed += (s, a) => { CleanUp(s, a); };
            Window = _window;
            _window.Activate();
        }

        private static async Task ConfigureDb()
        {
            using var db = new DBContext();
            await db.Database.MigrateAsync();
        }

        private static void CleanUp(object sender, WindowEventArgs args)
        {
            string previewRoot = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Constants.AppProgramName);
            if (!Directory.Exists(previewRoot)) return;

            foreach (string dir in Directory.GetDirectories(previewRoot))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(dir)) File.SetAttributes(file, FileAttributes.Normal);
                    Directory.Delete(dir, recursive: true);
                }
                catch { }
            }
        }
    }
}
