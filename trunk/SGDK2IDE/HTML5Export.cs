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
   public partial class frmHTML5Export : Form
   {
      public frmHTML5Export()
      {
         InitializeComponent();
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         string projectFile = ((frmMain)Owner).ProjectFile;
         if (!string.IsNullOrEmpty(projectFile))
         {
            string dirName = System.IO.Path.GetDirectoryName(projectFile);
            dlgFilename.InitialDirectory = dirName;
            dlgFilename.FileName = System.IO.Path.Combine(dirName, System.IO.Path.GetFileNameWithoutExtension(projectFile) + ".html");
            txtFilename.Text = dlgFilename.FileName;
         }
         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/d07a3ab3-7056-48e4-9700-1b54d8205acc.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }
      private void btnBrowse_Click(object sender, EventArgs e)
      {
         if (dlgFilename.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
         {
            txtFilename.Text = dlgFilename.FileName;
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         try
         {
            CodeGenerator g = new CodeGenerator();
            string errs;
            System.Collections.Generic.IEnumerable<CodeGenerator.ObjectErrorInfo> errorRules;
            CodeGenerator.HtmlGeneratorOptions options = 0;
            if (rdoSingleFile.Checked)
            {
               options = CodeGenerator.HtmlGeneratorOptions.SingleFile;
               if (System.IO.File.Exists(txtFilename.Text) && DialogResult.Yes !=
                  MessageBox.Show(this, "File \"" + txtFilename.Text + "\" already exists. Overwrite?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
               {
                  DialogResult = System.Windows.Forms.DialogResult.None;
                  return;
               }
            }
            else
            {
               string[] files = g.GetHtmlFileList(txtFilename.Text);
               string existFile = null;
               int existCount = 0;
               for (int i = 0; i < files.Length; i++)
               {
                  if (System.IO.File.Exists(files[i]))
                  {
                     existCount++;
                     if (existFile == null)
                        existFile = files[i];
                  }
               }
               if ((existCount > 0) && DialogResult.Yes !=
                  MessageBox.Show(this, "File \"" + existFile + "\" " + (existCount > 1 ? "and " + (existCount - 1).ToString() + " other files already exist." :
                  "already exists.") + " Overwrite?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
               {
                  DialogResult = System.Windows.Forms.DialogResult.None;
                  return;
               }
            }
            if (rdoFill.Checked)
               options |= CodeGenerator.HtmlGeneratorOptions.FillBrowser;
            if (chkMapButtons.Checked)
               options |= CodeGenerator.HtmlGeneratorOptions.GenerateMapButtons;
            string outFile = g.GenerateHtml5(txtFilename.Text, options, out errs, out errorRules);
            if (errs.Length > 0)
            {
               frmLogView frm = new frmLogView(errs, errorRules);
               frm.MdiParent = this;
               frm.Show();
               return;
            }
            MessageBox.Show(this, outFile + " Written", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}
