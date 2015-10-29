using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private readonly BackgroundWorker backgroundWorker;

        private ExperienceData experienceData = new ExperienceData();

        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        const int CURRENT_BASE_EXPERIENCE_ADDRESS = 0x3348100C;
        const int REQURED_BASE_EXPERIENCE_ADDRESS = 0x33481010;

        public Form1()
        {
            InitializeComponent();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += UpdateExperience;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();

            label1.Text = experienceData.currentBaseExperience.ToString();
        }
        
        private void UpdateExperience(object sender, DoWorkEventArgs e)
        {
            Process process = Process.GetProcessesByName("Client_tos")[0];
            var processId = OpenProcess(PROCESS_WM_READ, false, process.Id);

            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            while (!backgroundWorker.CancellationPending)
            {
                int bytesRead = 0;
                var buffer = new byte[4];

                ReadProcessMemory(processId, (IntPtr)CURRENT_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
                Int32 currentBaseExperience = BitConverter.ToInt32(buffer, 0);

                ReadProcessMemory(processId, (IntPtr)REQURED_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
                Int32 requiredBaseExperience = BitConverter.ToInt32(buffer, 0);
                
                experienceData.currentBaseExperience = currentBaseExperience;
                experienceData.requiredBaseExperience = requiredBaseExperience;

                if(experienceData.currentBaseExperience != experienceData.previousBaseExperience)
                {
                    experienceData.lastKillExperience = experienceData.currentBaseExperience - experienceData.previousBaseExperience;
                    experienceData.baseKillsTilNextLevel = (experienceData.requiredBaseExperience - experienceData.currentBaseExperience) / (float)experienceData.lastKillExperience;

                    experienceData.previousBaseExperience = experienceData.currentBaseExperience;

                    backgroundWorker.ReportProgress(100, experienceData);
                }
            }
        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("update ui");
            ExperienceData experienceData = (ExperienceData)e.UserState;

            float basePercent = (experienceData.currentBaseExperience / (float)experienceData.requiredBaseExperience) * 100;

            label1.Text = experienceData.currentBaseExperience.ToString();
            label2.Text = experienceData.requiredBaseExperience.ToString();
            label3.Text = basePercent.ToString() + "%";
            label4.Text = experienceData.lastKillExperience.ToString();
            label5.Text = experienceData.baseKillsTilNextLevel.ToString();
            label12.Text = ((experienceData.lastKillExperience / (float)experienceData.requiredBaseExperience) * 100).ToString();
        }
    }
}
