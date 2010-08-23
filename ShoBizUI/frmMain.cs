using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using EndpointSystems.BizTalk.Documentation;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;
using Application=Microsoft.BizTalk.ExplorerOM.Application;
using ADODB;
using MSDASC;

namespace ShoBizUI
{
    public partial class frmMain : Form
    {
        private const string MULTI = ";MultipleActiveResultSets=True";

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
            
            if (lbApps.Items.Count > 0) lbApps.Items.Clear();
            
            try
            {
                lblStatus.Text = "Connecting...";
                foreach (Application app in CatalogExplorerFactory.CatalogExplorer().Applications)
                {
                    lbApps.Items.Add(app.Name);
                }
                lblStatus.Text = "Select the application(s) you wish to document.";
            }
            catch (Exception ex)
            {
                HandleException("Connect",ex);
                MessageBox.Show(string.Format("A {0} occurred connecting to the BizTalk Database: \r\n{1}", ex.GetType(),
                                              ex.Message),"Connection Failure",MessageBoxButtons.OK,MessageBoxIcon.Error);
                
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
            if (!ckSelect.Checked) return;
            for (var i = 0; i < lbApps.Items.Count; i++)
            {
                lbApps.SetSelected(i, true);
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            status("Building documentation, please wait...");
            if (lbApps.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please choose one or more BizTalk applications you wish to document first.",
                                "ShoBiz", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Cursor = Cursors.WaitCursor;
            try
            {
                BuildApps();
            }
            catch (Exception ex)
            {
                HandleException("Build", ex);
                MessageBox.Show(ex.GetType() + ": " + ex.Message + "\r\n\r\n" + ex.StackTrace, "Build error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                btnBuild.Enabled = true;
            }
        }

        private void BuildApps()
        {
            var apps = new List<string>();
            try
            {
                
                //build the directory path, if it doesn't exist
                if (!Directory.Exists(tbBaseFolder.Text))
                    Directory.CreateDirectory(tbBaseFolder.Text);

                foreach (int i in lbApps.SelectedIndices)
                {
                    apps.Add(lbApps.Items[i] as string);
                }


                //CatalogExplorerFactory.CatalogExplorer(tbConnectionString.Text.Trim(), bool.TrueString);
                var at = new AppsTopic(tbBaseFolder.Text.Trim(), Path.Combine(tbBaseFolder.Text.Trim(), "images"), apps, tbRules.Text);
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
            var c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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

        private void btnConnectString_Click(object sender, EventArgs e)
        {
            var dlDlg = new DataLinksClass();
            _Connection con = null;

            con = (_Connection)dlDlg.PromptNew();
            if (con == null) return;
            var str = con.ConnectionString;
            if (!str.Contains(MULTI)) str = str + MULTI;
            tbConnectionString.Text = str;
        }
    }
}