using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using EndpointSystems.BizTalk.Documentation;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;
using Application=Microsoft.BizTalk.ExplorerOM.Application;

namespace ShoBizUI
{
    public partial class frmMain : Form
    {
        delegate void BuildDelegate();

        private readonly BtsCatalogExplorer bce;
        public frmMain()
        {
            bce = new BtsCatalogExplorer();
            InitializeComponent();
        }

        private void btnBaseDir_Click(object sender, EventArgs e)
        {
            DialogResult dr = fdProjectPath.ShowDialog();
            if (dr == DialogResult.OK)
            {
              tbBaseFolder.Text =  fdProjectPath.SelectedPath;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                lblStatus.Text = "Connecting...";
                bce.ConnectionString = tbConnectionString.Text;
                lblStatus.Text = "Connected.";
                foreach (Application app in bce.Applications)
                {
                    lbApps.Items.Add(app.Name);
                }
                lblStatus.Text = "Select the application(s) you wish to document.";
            }
            catch (Exception ex)
            {
                HandleException("Connect",ex);
                throw;
            }        
            finally
            {
                bce.Dispose();
                Cursor = Cursors.Arrow;
            }
        }

        private void HandleException(string fn, Exception ex)
        {
            status(string.Format("[{0}] {1}: {2}", fn, ex.GetType(), ex.Message));
        }

        private void ckSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (ckSelect.Checked)
            {
                for (int i = 0; i < lbApps.Items.Count; i++)
                {
                    lbApps.SetSelected(i, true);
                }
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                BuildApps();
                //BuildDelegate bd = BuildApps;
                //bd.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                HandleException("Build", ex);
                throw;
            }
            finally
            {
                Cursor = Cursors.Arrow;
                btnBuild.Enabled = true;
            }
        }

        private void BuildApps()
        {
            List<string> apps = new List<string>();
            try
            {
                status("Building documentation, please wait...");
                
                foreach (int i in lbApps.SelectedIndices)
                {
                    apps.Add(lbApps.Items[i] as string);
                }

                //the trailing character is important
                string s = tbBaseFolder.Text.Trim() + @"\";

                CatalogExplorerFactory.CatalogExplorer(tbConnectionString.Text.Trim(), true);

                AppsTopic at = new AppsTopic(s, s + @"images\", apps, tbRules.Text);
                do
                {
                    Thread.Sleep(100);
                } while (!at.ReadyToSave);

                at.Save();

                status("Build complete. Please check your build directory for your project.");
            }
            catch(Exception ex)
            {
                HandleException("BuildApps", ex);
            }
        }
        
        private void status(string text)
        {
            lblStatus.Text = text;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            c.AppSettings.Settings["connectionString"].Value = tbConnectionString.Text.Trim();
            c.AppSettings.Settings["rulesDatabase"].Value = tbRules.Text.Trim();
            c.AppSettings.Settings["baseDirectory"].Value = tbBaseFolder.Text.Trim();
            c.Save(ConfigurationSaveMode.Full);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
           tbConnectionString.Text    = ConfigurationManager.AppSettings["connectionString"];
           tbRules.Text               = ConfigurationManager.AppSettings["rulesDatabase"];
           tbBaseFolder.Text          = ConfigurationManager.AppSettings["baseDirectory"];
        }

        private void lbApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbApps.SelectedIndices.Count > 0)
                lblStatus.Text = lbApps.SelectedIndices.Count > 0 ? "Press the 'Build Documentation' button when you are ready." : "Select the application(s) you wish to document.";
        }
    }
}