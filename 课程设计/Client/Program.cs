﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Connect.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new ChatForm("孙久华","","","张熙贤"));
            Application.Run(new Land());
            if (ProcessingCenter.MainF)
            {
                Application.Run(new MainForm());
            }
        }
    }
}
