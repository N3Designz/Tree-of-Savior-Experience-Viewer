using System;
using System.Configuration;
using System.Windows.Forms;

namespace TreeOfSaviorExperienceViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String baseExperience = ConfigurationManager.AppSettings["baseExperience"];

            Console.WriteLine("base experience: " + baseExperience);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ExperienceViewerForm());
        }
    }
}
