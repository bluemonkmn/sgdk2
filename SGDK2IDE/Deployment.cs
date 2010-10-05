using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SGDK2
{
   public partial class frmDeployment : Form
   {
      string downloading = null;
      bool updating = false;
      frmMain mainWindow;

      public frmDeployment()
      {
         InitializeComponent();
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupProgressChanged += new System.Deployment.Application.DeploymentProgressChangedEventHandler(CurrentDeployment_DownloadFileGroupProgressChanged);
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupCompleted += new System.Deployment.Application.DownloadFileGroupCompletedEventHandler(CurrentDeployment_DownloadFileGroupCompleted);
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += new System.Deployment.Application.DeploymentProgressChangedEventHandler(CurrentDeployment_UpdateProgressChanged);
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateCompleted += new AsyncCompletedEventHandler(CurrentDeployment_UpdateCompleted);
      }

      void CurrentDeployment_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
      {
         updating = false;
         if (e.Cancelled)
            return;
         if (e.Error != null)
         {
            MessageBox.Show(e.Error.Message, "Download Updates", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }
         if (MessageBox.Show("The update will not take effect until you restart the application.  Do you want to restart now?", "Download Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            Application.Restart();
         else
         {
            btnCancel.Text = "Close";
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
         }
      }

      void CurrentDeployment_UpdateProgressChanged(object sender, System.Deployment.Application.DeploymentProgressChangedEventArgs e)
      {
         lblProgress.Text = "Downloading Update: " + e.BytesCompleted.ToString() + " / " + e.BytesTotal.ToString();
         progress.Value = e.ProgressPercentage;
      }

      public static void CheckDeploymentOptions(bool forceResponse, frmMain mainWindow)
      {
         bool doUpdate = false;

         if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
         {
            if (forceResponse && System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdate())
            {
               switch (MessageBox.Show("A new version of SGDK2 is available.  Do you want to download it?", "Download Updates", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
               {
                  case DialogResult.Yes:
                     doUpdate = true;
                     break;
                  case DialogResult.No:
                     break;
                  default:
                     return;
               }
            }
            using (frmDeployment frm = new frmDeployment())
            {
               frm.mainWindow = mainWindow;
               int installedCount = frm.InitializeComponentStates();
               if (doUpdate)
               {
                  System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateAsync();
                  frm.updating = true;
                  frm.btnDownload.Enabled = false;
                  frm.ShowDialog(mainWindow);
                  return;
               }
               if (installedCount == frm.lvwDeployment.Items.Count)
               {
                  if (forceResponse)
                     frm.ShowDialog(mainWindow);
                  return;
               }
               string option = SGDK2IDE.GetUserOption("DeploymentOptions");
               if (forceResponse || string.IsNullOrEmpty(option) || (option == "0"))
                  frm.ShowDialog(mainWindow);
            }
         }
         else
         {
            if (forceResponse)
               MessageBox.Show("This command only applies when SGDK2 is deployed via the web.", "Deployment Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
         }
      }

      public int InitializeComponentStates()
      {
         int installedFeatures = 0;
         foreach(ListViewItem lvi in lvwDeployment.Items)
            if (System.Deployment.Application.ApplicationDeployment.CurrentDeployment.IsFileGroupDownloaded(lvi.Text))
            {
               lvi.SubItems[2].Text = "Installed";
               lvi.SubItems[2].Tag = true;
               lvi.Checked = false;
               installedFeatures++;
            }
            else
            {
               lvi.SubItems[2].Text = "Not Installed";
               lvi.SubItems[2].Tag = false;
            }
         return installedFeatures;
      }

      private void btnDownload_Click(object sender, EventArgs e)
      {
         foreach (ListViewItem lvi in lvwDeployment.Items)
            if (lvi.Checked && ((bool)lvi.SubItems[2].Tag == false))
            {
               System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupAsync(lvi.Text);
               btnDownload.Enabled = false;
               downloading = lvi.Text;
               break;
            }
      }

      void CurrentDeployment_DownloadFileGroupCompleted(object sender, System.Deployment.Application.DownloadFileGroupCompletedEventArgs e)
      {
         btnCancel.Text = "Close";
         btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
         InitializeComponentStates();
         foreach (ListViewItem lvi in lvwDeployment.Items)
            if (lvi.Checked && ((bool)lvi.SubItems[2].Tag == false))
            {
               System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupAsync(lvi.Text);
               downloading = lvi.Text;
               return;
            }
         downloading = null;
         btnDownload.Enabled = true;
         SGDK2IDE.FindHelpFile();
         mainWindow.CreateTemplateMenuItems();
      }

      void CurrentDeployment_DownloadFileGroupProgressChanged(object sender, System.Deployment.Application.DeploymentProgressChangedEventArgs e)
      {
         lblProgress.Text = e.Group + ": " + e.BytesCompleted.ToString() + " / " + e.BytesTotal.ToString();
         progress.Value = e.ProgressPercentage;
      }

      private void chkDontShow_CheckedChanged(object sender, EventArgs e)
      {
         SGDK2IDE.SetUserOption("DeploymentOptions", chkDontShow.Checked ? "1" : "0");
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing(e);
         if (downloading != null) System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupAsyncCancel(downloading);
         if (updating) System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateAsyncCancel();
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupProgressChanged -= new System.Deployment.Application.DeploymentProgressChangedEventHandler(CurrentDeployment_DownloadFileGroupProgressChanged);
         System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DownloadFileGroupCompleted -= new System.Deployment.Application.DownloadFileGroupCompletedEventHandler(CurrentDeployment_DownloadFileGroupCompleted);
      }
   }
}
