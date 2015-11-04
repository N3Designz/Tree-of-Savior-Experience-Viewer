using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace TreeOfSaviorExperienceViewer
{
    public partial class ExperienceViewerForm : Form
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

        IntPtr CURRENT_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero;
        IntPtr REQURED_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero; //0x4;
        
        // 0x10C offset
        private int[] baseExperienceOffsets = new int[] { 0x450, 0x340, 0x190, 0x50, 0x10C };

        public ExperienceViewerForm()
        {
            InitializeComponent();
            WireAllComponentsToClick(this);

            Process[] processes = Process.GetProcessesByName("Client_tos");
            Process process = null;

            if(processes.Length > 0)
            {
                process = processes[0];
            }

            if(processes.Length == 0 || process == null)
            {
                DialogResult dialogResult = MessageBox.Show("Could not find Tree of Savior. Please open it first.", "Error");

                if(DialogResult.OK == dialogResult)
                {
                    Application.Exit();
                }
            }

            var processId = OpenProcess(PROCESS_WM_READ, false, process.Id);

            CURRENT_BASE_EXPERIENCE_ADDRESS = getCurrentBaseExperience(process);
            REQURED_BASE_EXPERIENCE_ADDRESS = CURRENT_BASE_EXPERIENCE_ADDRESS + 0x4;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += UpdateExperience;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();

            currentBaseExperienceLabel.Text = experienceData.currentBaseExperience.ToString();
        }

        private void WireAllComponentsToClick(Control form1)
        {
            foreach(Control control in form1.Controls)
            {
                control.DoubleClick += this.ToggleBorderWithDoubleClick;
                if(control.HasChildren)
                {
                    WireAllComponentsToClick(control);
                }
            }
        }

        private int baseExperienceGained = 0;
        private double baseExperiencePerHour = 0;
        
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
                    baseExperienceGained += experienceData.lastKillExperience;
                    
                    experienceData.baseKillsTilNextLevel = (experienceData.requiredBaseExperience - experienceData.currentBaseExperience) / (float)experienceData.lastKillExperience;

                    experienceData.previousBaseExperience = experienceData.currentBaseExperience;

                    backgroundWorker.ReportProgress(100, experienceData);

                    if(!started)
                    {
                        baseExperienceGained = 0;
                        experienceData.lastKillExperience = 0;
                        started = true;
                    }
                }

                TimeSpan elapsedTime = DateTime.Now - Process.GetCurrentProcess().StartTime;
                baseExperiencePerHour = baseExperienceGained * (3600000 / elapsedTime.TotalMilliseconds);

                Thread.Sleep(30);
            }
        }

        private bool started = false;

        private IntPtr getCurrentBaseExperience(Process process)
        {
            var offsetList = new int[] { 0x10C };
            var buffer = new byte[4];
            var lpOutStorage = 0;
            IntPtr currentAddress = new IntPtr(0x01489F10);

            ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out lpOutStorage);

            Int32 value = BitConverter.ToInt32(buffer, 0);

            currentAddress = (IntPtr)value;

            for (int i = 0; i < offsetList.Length; i++)
            {
                currentAddress = IntPtr.Add(currentAddress, offsetList[i]);
                ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out lpOutStorage);

                value = BitConverter.ToInt32(buffer, 0);
                
                if(i != offsetList.Length - 1)
                {
                    currentAddress = (IntPtr)value;
                }
            }
            
            return currentAddress;
        }
        
        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            ExperienceData experienceData = (ExperienceData)e.UserState;

            float basePercent = (experienceData.currentBaseExperience / (float)experienceData.requiredBaseExperience) * 100;
            
            currentBaseExperienceLabel.Text = experienceData.currentBaseExperience.ToString("N0");
            requiredBaseExperienceLabel.Text = experienceData.requiredBaseExperience.ToString("N0");
            currentBaseExperiencePercentLabel.Text = basePercent.ToString() + "%";
            experienceFromLastKillLabel.Text = experienceData.lastKillExperience.ToString("N0");
            baseKillsTilNextLevelLabel.Text = experienceData.baseKillsTilNextLevel.ToString();
            experiencePercentFromLastKillLabel.Text = ((experienceData.lastKillExperience / (float)experienceData.requiredBaseExperience) * 100).ToString();
            //baseExperiencePerHourLabel.Text = baseExperiencePerHour.ToString("N0");
        }

        private void ToggleBorderWithDoubleClick(object sender, EventArgs e)
        {
            if(this.FormBorderStyle == FormBorderStyle.FixedSingle)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            } else if(this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
            }
        }
    }
}
