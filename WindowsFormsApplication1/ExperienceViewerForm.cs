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
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        const int PROCESS_WM_READ = 0x0010;

        IntPtr CURRENT_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero;
        IntPtr REQURED_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero; //0x4;
        
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

                Environment.Exit(0);
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

            experienceData.currentBaseExperience = 0;
            experienceData.currentBaseExperience = 0;
            experienceData.lastKillExperience = 0;
            experienceData.previousCurrentBaseExperience = 0;
            experienceData.previousRequiredBaseExperience = 0;
            experienceData.requiredBaseExperience = 0;
            experienceData.baseKillsTilNextLevel = 0;
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
        private double hoursTilLevel = 0;
        
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

                experienceData.previousCurrentBaseExperience = experienceData.currentBaseExperience;
                experienceData.previousRequiredBaseExperience = experienceData.requiredBaseExperience;

                ReadProcessMemory(processId, (IntPtr)CURRENT_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
                experienceData.currentBaseExperience = BitConverter.ToInt32(buffer, 0);
                                
                ReadProcessMemory(processId, (IntPtr)REQURED_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
                experienceData.requiredBaseExperience = BitConverter.ToInt32(buffer, 0);
                
                if(experienceData.currentBaseExperience != experienceData.previousCurrentBaseExperience)
                {
                    if (experienceData.requiredBaseExperience > experienceData.previousRequiredBaseExperience)
                    {
                        experienceData.lastKillExperience = (experienceData.requiredBaseExperience - experienceData.previousCurrentBaseExperience) + experienceData.currentBaseExperience;
                    }
                    else
                    {
                        experienceData.lastKillExperience = experienceData.currentBaseExperience - experienceData.previousCurrentBaseExperience;
                    }

                    experienceData.baseKillsTilNextLevel = (experienceData.requiredBaseExperience - experienceData.currentBaseExperience) / (float)experienceData.lastKillExperience;

                    baseExperienceGained += experienceData.lastKillExperience;

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

                hoursTilLevel = (experienceData.requiredBaseExperience - experienceData.currentBaseExperience) / baseExperiencePerHour;

                if(hoursTilLevel <= 0)
                {
                    hoursTilLevel = 0;
                }

                if(baseExperiencePerHour <= 0)
                {
                    baseExperiencePerHour = 0;
                }
                
                Thread.Sleep(1000);

                backgroundWorker.ReportProgress(100, experienceData);
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
            baseExperiencePerHourLabel.Text = baseExperiencePerHour.ToString("N0");
            hoursTilLevelLabel.Text = hoursTilLevel.ToString("F4");
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
