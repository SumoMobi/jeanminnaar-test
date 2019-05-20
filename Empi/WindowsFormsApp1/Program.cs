using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 1)
            {
                Uri uri = new Uri(args[0]);
                NameValueCollection nameValues = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string firstName = nameValues.Get("firstName");
                string lastName = nameValues.Get("lastName");
                string dobMonth = nameValues.Get("dobMonth");
                string dobDay = nameValues.Get("dobDay");
                string dobYear = nameValues.Get("dobYear");
                Application.Run(new Form1(firstName, lastName, dobMonth, dobDay, dobYear, uri.OriginalString));
                return;
            }
            Application.Run(new Form1());
        }
    }
}
