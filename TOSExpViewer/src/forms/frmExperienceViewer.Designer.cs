namespace TreeOfSaviorExperienceViewer
{
    partial class ExperienceViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExperienceViewerForm));
            this.currentBaseExperienceLabel = new System.Windows.Forms.Label();
            this.requiredBaseExperienceLabel = new System.Windows.Forms.Label();
            this.currentBaseExperiencePercentLabel = new System.Windows.Forms.Label();
            this.tlpExpTable = new System.Windows.Forms.TableLayoutPanel();
            this.experienceFromLastKillLabel = new System.Windows.Forms.Label();
            this.baseKillsTilNextLevelLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.experiencePercentFromLastKillLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.baseExperiencePerHourLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.hoursTilLevelLabel = new System.Windows.Forms.Label();
            this.tlpExpTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // currentBaseExperienceLabel
            // 
            this.currentBaseExperienceLabel.AutoSize = true;
            this.currentBaseExperienceLabel.Location = new System.Drawing.Point(6, 25);
            this.currentBaseExperienceLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.currentBaseExperienceLabel.Name = "currentBaseExperienceLabel";
            this.currentBaseExperienceLabel.Size = new System.Drawing.Size(40, 25);
            this.currentBaseExperienceLabel.TabIndex = 0;
            this.currentBaseExperienceLabel.Text = "XX";
            // 
            // requiredBaseExperienceLabel
            // 
            this.requiredBaseExperienceLabel.AutoSize = true;
            this.requiredBaseExperienceLabel.Location = new System.Drawing.Point(144, 25);
            this.requiredBaseExperienceLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.requiredBaseExperienceLabel.Name = "requiredBaseExperienceLabel";
            this.requiredBaseExperienceLabel.Size = new System.Drawing.Size(40, 25);
            this.requiredBaseExperienceLabel.TabIndex = 1;
            this.requiredBaseExperienceLabel.Text = "XX";
            // 
            // currentBaseExperiencePercentLabel
            // 
            this.currentBaseExperiencePercentLabel.AutoSize = true;
            this.currentBaseExperiencePercentLabel.Location = new System.Drawing.Point(298, 25);
            this.currentBaseExperiencePercentLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.currentBaseExperiencePercentLabel.Name = "currentBaseExperiencePercentLabel";
            this.currentBaseExperiencePercentLabel.Size = new System.Drawing.Size(40, 25);
            this.currentBaseExperiencePercentLabel.TabIndex = 2;
            this.currentBaseExperiencePercentLabel.Text = "XX";
            // 
            // tlpExpTable
            // 
            this.tlpExpTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpExpTable.ColumnCount = 8;
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpExpTable.Controls.Add(this.currentBaseExperiencePercentLabel, 2, 1);
            this.tlpExpTable.Controls.Add(this.currentBaseExperienceLabel, 0, 1);
            this.tlpExpTable.Controls.Add(this.experienceFromLastKillLabel, 3, 1);
            this.tlpExpTable.Controls.Add(this.baseKillsTilNextLevelLabel, 5, 1);
            this.tlpExpTable.Controls.Add(this.label6, 0, 0);
            this.tlpExpTable.Controls.Add(this.label8, 2, 0);
            this.tlpExpTable.Controls.Add(this.label9, 3, 0);
            this.tlpExpTable.Controls.Add(this.label10, 5, 0);
            this.tlpExpTable.Controls.Add(this.label11, 4, 0);
            this.tlpExpTable.Controls.Add(this.experiencePercentFromLastKillLabel, 4, 1);
            this.tlpExpTable.Controls.Add(this.label1, 6, 0);
            this.tlpExpTable.Controls.Add(this.baseExperiencePerHourLabel, 6, 1);
            this.tlpExpTable.Controls.Add(this.label2, 7, 0);
            this.tlpExpTable.Controls.Add(this.hoursTilLevelLabel, 7, 1);
            this.tlpExpTable.Controls.Add(this.requiredBaseExperienceLabel, 1, 1);
            this.tlpExpTable.Controls.Add(this.label7, 1, 0);
            this.tlpExpTable.Location = new System.Drawing.Point(15, 15);
            this.tlpExpTable.Margin = new System.Windows.Forms.Padding(6);
            this.tlpExpTable.Name = "tlpExpTable";
            this.tlpExpTable.RowCount = 2;
            this.tlpExpTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpExpTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpExpTable.Size = new System.Drawing.Size(1000, 117);
            this.tlpExpTable.TabIndex = 2;
            // 
            // experienceFromLastKillLabel
            // 
            this.experienceFromLastKillLabel.AutoSize = true;
            this.experienceFromLastKillLabel.Location = new System.Drawing.Point(418, 25);
            this.experienceFromLastKillLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.experienceFromLastKillLabel.Name = "experienceFromLastKillLabel";
            this.experienceFromLastKillLabel.Size = new System.Drawing.Size(40, 25);
            this.experienceFromLastKillLabel.TabIndex = 3;
            this.experienceFromLastKillLabel.Text = "XX";
            // 
            // baseKillsTilNextLevelLabel
            // 
            this.baseKillsTilNextLevelLabel.AutoSize = true;
            this.baseKillsTilNextLevelLabel.Location = new System.Drawing.Point(686, 25);
            this.baseKillsTilNextLevelLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.baseKillsTilNextLevelLabel.Name = "baseKillsTilNextLevelLabel";
            this.baseKillsTilNextLevelLabel.Size = new System.Drawing.Size(40, 25);
            this.baseKillsTilNextLevelLabel.TabIndex = 4;
            this.baseKillsTilNextLevelLabel.Text = "XX";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 25);
            this.label6.TabIndex = 5;
            this.label6.Text = "Current Exp";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(144, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 25);
            this.label7.TabIndex = 6;
            this.label7.Text = "Required Exp";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(298, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 25);
            this.label8.TabIndex = 7;
            this.label8.Text = "Current %";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(418, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 25);
            this.label9.TabIndex = 8;
            this.label9.Text = "Last Kill Exp";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(686, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 25);
            this.label10.TabIndex = 9;
            this.label10.Text = "Kills TNL";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(561, 0);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 25);
            this.label11.TabIndex = 10;
            this.label11.Text = "Last Kill %";
            // 
            // experiencePercentFromLastKillLabel
            // 
            this.experiencePercentFromLastKillLabel.AutoSize = true;
            this.experiencePercentFromLastKillLabel.Location = new System.Drawing.Point(561, 25);
            this.experiencePercentFromLastKillLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.experiencePercentFromLastKillLabel.Name = "experiencePercentFromLastKillLabel";
            this.experiencePercentFromLastKillLabel.Size = new System.Drawing.Size(40, 25);
            this.experiencePercentFromLastKillLabel.TabIndex = 11;
            this.experiencePercentFromLastKillLabel.Text = "XX";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(796, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 25);
            this.label1.TabIndex = 12;
            this.label1.Text = "Exp/Hr";
            // 
            // baseExperiencePerHourLabel
            // 
            this.baseExperiencePerHourLabel.AutoSize = true;
            this.baseExperiencePerHourLabel.Location = new System.Drawing.Point(796, 25);
            this.baseExperiencePerHourLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.baseExperiencePerHourLabel.Name = "baseExperiencePerHourLabel";
            this.baseExperiencePerHourLabel.Size = new System.Drawing.Size(40, 25);
            this.baseExperiencePerHourLabel.TabIndex = 13;
            this.baseExperiencePerHourLabel.Text = "XX";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(885, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 25);
            this.label2.TabIndex = 14;
            this.label2.Text = "Hr/Lv";
            // 
            // hoursTilLevelLabel
            // 
            this.hoursTilLevelLabel.AutoSize = true;
            this.hoursTilLevelLabel.Location = new System.Drawing.Point(885, 25);
            this.hoursTilLevelLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.hoursTilLevelLabel.Name = "hoursTilLevelLabel";
            this.hoursTilLevelLabel.Size = new System.Drawing.Size(40, 25);
            this.hoursTilLevelLabel.TabIndex = 15;
            this.hoursTilLevelLabel.Text = "XX";
            // 
            // ExperienceViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 147);
            this.Controls.Add(this.tlpExpTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExperienceViewerForm";
            this.Text = "Tree of Savior Experience Viewer (by Excrulon)";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ExperienceViewerForm_Load);
            this.DoubleClick += new System.EventHandler(this.ToggleBorderWithDoubleClick);
            this.tlpExpTable.ResumeLayout(false);
            this.tlpExpTable.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label currentBaseExperienceLabel;
        private System.Windows.Forms.Label requiredBaseExperienceLabel;
        private System.Windows.Forms.Label currentBaseExperiencePercentLabel;
        private System.Windows.Forms.TableLayoutPanel tlpExpTable;
        private System.Windows.Forms.Label experienceFromLastKillLabel;
        private System.Windows.Forms.Label baseKillsTilNextLevelLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label experiencePercentFromLastKillLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label baseExperiencePerHourLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label hoursTilLevelLabel;
    }
}

