using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using TreeOfSaviorExperienceViewer;

namespace TreeOfSaviorExperienceViewer
{
    public partial class ExperienceViewerForm : Form
    {
        private BackgroundWorker expUpdateBackgroundWorker;

        private TosClient client = null; 
        private ExperienceData experienceData = new ExperienceData();
        
        public ExperienceViewerForm()
        {
            InitializeComponent();
            WireAllComponentsToClick(this);

            experienceData.currentBaseExperience = 0;
            experienceData.currentBaseExperience = 0;
            experienceData.lastKillExperience = 0;
            experienceData.previousCurrentBaseExperience = 0;
            experienceData.previousRequiredBaseExperience = 0;
            experienceData.requiredBaseExperience = 0;
            experienceData.baseKillsTilNextLevel = 0;
        }

        private void WireAllComponentsToClick(Control control)
        {
            foreach(Control subControl in control.Controls)
            {
                subControl.DoubleClick += this.ToggleBorderWithDoubleClick;
                if(subControl.HasChildren)
                {
                    WireAllComponentsToClick(subControl);
                }
            }
        }

        private int baseExperienceGained = 0;
        private double baseExperiencePerHour = 0;
        private double hoursTilLevel = 0;
        
        private void UpdateExperience(object sender, DoWorkEventArgs e)
        {
            if (expUpdateBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            while (!expUpdateBackgroundWorker.CancellationPending)
            {
                experienceData.previousCurrentBaseExperience = experienceData.currentBaseExperience;
                experienceData.previousRequiredBaseExperience = experienceData.requiredBaseExperience;

                experienceData.currentBaseExperience = client.GetBaseExperience();
                experienceData.requiredBaseExperience = client.GetRequiredExperience();
                
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

                    expUpdateBackgroundWorker.ReportProgress(100, experienceData);
                    
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

                expUpdateBackgroundWorker.ReportProgress(100, experienceData);
            }
        }
        
        private bool started = false;

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

        private void ExperienceViewerForm_Load(object sender, EventArgs e)
        {
            client = new TosClient();

            try
            {
                client.Attach();
            }
            catch(Exception)
            {
                DialogResult dialogResult = MessageBox.Show("Error on attaching to Tree of Savior client. Make sure that it is running.", "Error");
                Environment.Exit(0);
            }
            
            expUpdateBackgroundWorker = new BackgroundWorker();
            expUpdateBackgroundWorker.WorkerSupportsCancellation = true;
            expUpdateBackgroundWorker.DoWork += UpdateExperience;
            expUpdateBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            expUpdateBackgroundWorker.WorkerReportsProgress = true;
            expUpdateBackgroundWorker.RunWorkerAsync();

            client.ClientExitEvent += (obj, args) => 
            {
                expUpdateBackgroundWorker.CancelAsync();
                Application.Exit();
            };

            currentBaseExperienceLabel.Text = experienceData.currentBaseExperience.ToString();
        }
    }
}
