﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Spray_Paint_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense
                ("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx0QHxb" +
                "f1xzZFRHalhWTnJeUiweQnxTdEZiWH1bcXVRQWRdWUNxVw==");
        }
    }
}
