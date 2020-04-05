using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Hangout.Shared;

namespace ServerAdminTool
{
    static class Program
    {
        public static Form1 mForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mForm = new Form1();
            Application.Run(mForm);

            Console.WriteLine("ServerAdminTool started");
        }
    }
}
