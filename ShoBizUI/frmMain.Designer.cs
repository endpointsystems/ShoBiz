namespace ShoBizUI
{
    partial class frmMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.fdProjectPath = new System.Windows.Forms.FolderBrowserDialog();
            this.btnBaseDir = new System.Windows.Forms.Button();
            this.tbBaseFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRules = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbApps = new System.Windows.Forms.ListBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.ckSelect = new System.Windows.Forms.CheckBox();
            this.btnBuild = new System.Windows.Forms.Button();
            this.btnConnectString = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "BizTalk Server database connection string:";
            this.label1.Visible = false;
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Location = new System.Drawing.Point(159, 12);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(245, 20);
            this.tbConnectionString.TabIndex = 0;
            this.tbConnectionString.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 356);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(546, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(348, 17);
            this.lblStatus.Text = "Press the \'Connect\' button to connect to your Biztalk connection string.";
            // 
            // fdProjectPath
            // 
            this.fdProjectPath.Description = "Project Base Directory";
            this.fdProjectPath.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fdProjectPath.SelectedPath = "C:\\DEVlll";
            // 
            // btnBaseDir
            // 
            this.btnBaseDir.Location = new System.Drawing.Point(442, 69);
            this.btnBaseDir.Name = "btnBaseDir";
            this.btnBaseDir.Size = new System.Drawing.Size(75, 23);
            this.btnBaseDir.TabIndex = 4;
            this.btnBaseDir.Text = "Browse...";
            this.btnBaseDir.UseVisualStyleBackColor = true;
            this.btnBaseDir.Click += new System.EventHandler(this.btnBaseDir_Click);
            // 
            // tbBaseFolder
            // 
            this.tbBaseFolder.Location = new System.Drawing.Point(159, 71);
            this.tbBaseFolder.Name = "tbBaseFolder";
            this.tbBaseFolder.Size = new System.Drawing.Size(245, 20);
            this.tbBaseFolder.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Project base directory:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Business Rules Database:";
            // 
            // tbRules
            // 
            this.tbRules.Location = new System.Drawing.Point(158, 40);
            this.tbRules.Name = "tbRules";
            this.tbRules.Size = new System.Drawing.Size(132, 20);
            this.tbRules.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Applications:";
            // 
            // lbApps
            // 
            this.lbApps.FormattingEnabled = true;
            this.lbApps.Location = new System.Drawing.Point(15, 140);
            this.lbApps.Name = "lbApps";
            this.lbApps.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbApps.Size = new System.Drawing.Size(482, 173);
            this.lbApps.TabIndex = 6;
            this.lbApps.SelectedIndexChanged += new System.EventHandler(this.lbApps_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(442, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // ckSelect
            // 
            this.ckSelect.AutoSize = true;
            this.ckSelect.Location = new System.Drawing.Point(180, 112);
            this.ckSelect.Name = "ckSelect";
            this.ckSelect.Size = new System.Drawing.Size(70, 17);
            this.ckSelect.TabIndex = 5;
            this.ckSelect.Text = "Select All";
            this.ckSelect.UseVisualStyleBackColor = true;
            this.ckSelect.CheckedChanged += new System.EventHandler(this.ckSelect_CheckedChanged);
            // 
            // btnBuild
            // 
            this.btnBuild.Location = new System.Drawing.Point(159, 320);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(245, 23);
            this.btnBuild.TabIndex = 7;
            this.btnBuild.Text = "Build Documentation";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // btnConnectString
            // 
            this.btnConnectString.Location = new System.Drawing.Point(411, 12);
            this.btnConnectString.Name = "btnConnectString";
            this.btnConnectString.Size = new System.Drawing.Size(25, 23);
            this.btnConnectString.TabIndex = 9;
            this.btnConnectString.Text = "...";
            this.btnConnectString.UseVisualStyleBackColor = true;
            this.btnConnectString.Visible = false;
            this.btnConnectString.Click += new System.EventHandler(this.btnConnectString_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 378);
            this.Controls.Add(this.btnConnectString);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.ckSelect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lbApps);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbRules);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBaseFolder);
            this.Controls.Add(this.btnBaseDir);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tbConnectionString);
            this.Controls.Add(this.label1);
            this.Name = "frmMain";
            this.Text = "ShoBiz - Sandcastle documentation generation tool for  BizTalk Server";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.FolderBrowserDialog fdProjectPath;
        private System.Windows.Forms.Button btnBaseDir;
        private System.Windows.Forms.TextBox tbBaseFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbRules;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbApps;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox ckSelect;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Button btnConnectString;
    }
}

