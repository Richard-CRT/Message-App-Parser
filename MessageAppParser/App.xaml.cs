﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MessageAppParser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow;
            if (e.Args.Length == 0)
                mainWindow = new MainWindow();
            else
                mainWindow = new MainWindow(e.Args[0]);
            mainWindow.Show();
        }
    }
}
