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

      private static string lastProj = null;
      private static string lastDir = null;
      private static string lastFile = null;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         string htmlOptionString = SGDK2IDE.GetUserOption("HTMLExportOptions");
            CodeGenerator.HtmlGeneratorOptions options;
         if (!string.IsNullOrEmpty(htmlOptionString))
         {
            options = (CodeGenerator.HtmlGeneratorOptions)Enum.Parse(typeof(CodeGenerator.HtmlGeneratorOptions), htmlOptionString);
            chkEmbedPng.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedPng));
            chkMapButtons.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.GenerateMapButtons));
            rdoEmbedJS.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedJs));
            rdoFilePerObject.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.SeparateJsPerObj));
            rdoSeparateJS.Checked = !(rdoEmbedJS.Checked || rdoFilePerObject.Checked);
            rdoFill.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.FillBrowser));
            rdoFixed.Checked = !rdoFill.Checked;
            chkCamelCase.Checked = (0 != (options & CodeGenerator.HtmlGeneratorOptions.CamelCase));
         }
         else
            options = GetOptions();

         if (lastProj != SGDK2IDE.CurrentProjectFile)
         {
            lastDir = lastFile = null;
         }
         if (lastDir != null)
            txtDirectory.Text = lastDir;
         if (lastFile != null)
            txtFilename.Text = lastFile;

         EnableFilename(options);

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/d07a3ab3-7056-48e4-9700-1b54d8205acc.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing(e);
         SGDK2IDE.SetUserOption("HTMLExportOptions", GetOptions().ToString());
         lastDir = txtDirectory.Text;
         lastFile = (txtFilename.Text == "(Multiple)") ? null : txtFilename.Text;
         lastProj = SGDK2IDE.CurrentProjectFile;
      }

      private void btnBrowse_Click(object sender, EventArgs e)
      {
         try
         {
            dlgOutFolder.SelectedPath = txtDirectory.Text;
         }
         catch (System.Exception) { }

         if (dlgOutFolder.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
         {
            txtDirectory.Text = dlgOutFolder.SelectedPath;
         }
      }

      public CodeGenerator.HtmlGeneratorOptions GetOptions()
      {
         return (chkEmbedPng.Checked ? CodeGenerator.HtmlGeneratorOptions.EmbedPng : 0) |
            (rdoEmbedJS.Checked ? CodeGenerator.HtmlGeneratorOptions.EmbedJs : 0) |
            (rdoFilePerObject.Checked ? CodeGenerator.HtmlGeneratorOptions.SeparateJsPerObj : 0) |
            (rdoFill.Checked ? CodeGenerator.HtmlGeneratorOptions.FillBrowser : 0) |
            (chkMapButtons.Checked ? CodeGenerator.HtmlGeneratorOptions.GenerateMapButtons : 0) |
            (chkCamelCase.Checked ? CodeGenerator.HtmlGeneratorOptions.CamelCase : 0);
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         try
         {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
               if (txtFilename.Text.IndexOf(c) >= 0)
               {
                  MessageBox.Show(this, "Character '" + c + "' is not valid in a filename.", "Export HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  DialogResult = System.Windows.Forms.DialogResult.None;
                  return;
               }
            }
            if (!System.IO.Path.IsPathRooted(txtDirectory.Text))
            {
               MessageBox.Show(this, "A full directory name must be entered", "Export HTML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               DialogResult = System.Windows.Forms.DialogResult.None;
               return;
            }
            CodeGenerator g = new CodeGenerator();
            g.htmlOptions = GetOptions();
            string htmlFile;
            if (txtFilename.Enabled)
            {
               htmlFile = System.IO.Path.Combine(txtDirectory.Text, txtFilename.Text);
            }
            else
            {
               string projectFile = ((frmMain)Owner).ProjectFile;
               if (string.IsNullOrEmpty(projectFile))
               {
                  // Should never occur
                  htmlFile = System.IO.Path.Combine(txtDirectory.Text, "Unnamed.html");
               }
               else
               {
                  htmlFile = System.IO.Path.Combine(txtDirectory.Text, System.IO.Path.GetFileNameWithoutExtension(projectFile) + ".html");
               }
            }
            string errs;
            System.Collections.Generic.IEnumerable<CodeGenerator.ObjectErrorInfo> errorRules;
            string[] files = g.GetHtmlFileList(htmlFile);
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
            string outFile = g.GenerateHtml5(htmlFile, out errs, out errorRules);
            if (errs.Length > 0)
            {
               frmLogView frm = new frmLogView(errs, errorRules);
               frm.MdiParent = this.Owner;
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

      private void EnableFilename(CodeGenerator.HtmlGeneratorOptions options)
      {
         string projectFile = SGDK2IDE.CurrentProjectFile;
         if (string.IsNullOrEmpty(projectFile))
         {
            txtFilename.Enabled = true;
            txtFilename.Text = (lastFile == null) ? "Unnamed.html" : lastFile;
         }
         else
         {
            string dirName = (lastDir == null) ? System.IO.Path.GetDirectoryName(projectFile) : lastDir;
            if (0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedJs) &&
                0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedPng))
            {
               txtFilename.Enabled = true;
               txtFilename.Text = (lastFile == null) ? System.IO.Path.GetFileNameWithoutExtension(projectFile) + ".html" : lastFile;
            }
            else
            {
               txtFilename.Enabled = false;
               txtFilename.Text = "(Multiple)";
               string subDir = System.IO.Path.GetFileNameWithoutExtension(projectFile) + System.IO.Path.DirectorySeparatorChar + "html";
               if ((lastDir == null) || (lastDir == System.IO.Path.GetDirectoryName(projectFile)))
                  dirName = System.IO.Path.Combine(dirName, subDir);
            }
            txtDirectory.Text = dirName;
         }
      }

      private void Output_CheckedChanged(object sender, EventArgs e)
      {
         EnableFilename(GetOptions());
      }

      public static void ExportAndRun(Form parent)
      {
         try
         {
            string htmlOptionString = SGDK2IDE.GetUserOption("HTMLExportOptions");
            CodeGenerator.HtmlGeneratorOptions options;
            if (!string.IsNullOrEmpty(htmlOptionString))
               options = (CodeGenerator.HtmlGeneratorOptions)Enum.Parse(typeof(CodeGenerator.HtmlGeneratorOptions), htmlOptionString);
            else
            {
               PromptDisplayDialog(parent);
               return;
            }

            if (string.IsNullOrEmpty(SGDK2IDE.CurrentProjectFile))
            {
               PromptDisplayDialog(parent);
               return;
            }

            CodeGenerator g = new CodeGenerator();
            g.htmlOptions = options;
            string htmlFile;
            if (0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedJs) &&
                0 != (options & CodeGenerator.HtmlGeneratorOptions.EmbedPng))
            {
               if (String.IsNullOrEmpty(lastDir) || String.IsNullOrEmpty(lastFile))
               {
                  PromptDisplayDialog(parent);
                  return;
               }
               htmlFile = System.IO.Path.Combine(lastDir, lastFile);
            }
            else
            {
               if (String.IsNullOrEmpty(lastDir))
               {
                  PromptDisplayDialog(parent);
                  return;
               }
               htmlFile = System.IO.Path.Combine(lastDir, System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile) + ".html");
            }
            string errs;
            System.Collections.Generic.IEnumerable<CodeGenerator.ObjectErrorInfo> errorRules;
            string outFile = g.GenerateHtml5(htmlFile, out errs, out errorRules);
            if (errs.Length > 0)
            {
               frmLogView frm = new frmLogView(errs, errorRules);
               frm.MdiParent = parent;
               frm.Show();
               return;
            }
            System.Diagnostics.Process.Start(htmlFile);
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(parent, ex.Message, parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private static void PromptDisplayDialog(Form parent)
      {
         if (MessageBox.Show(parent, "The project must be saved and exported once before running it in this manner. Do you want to Export now?", "Export and Run HMTL 5", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            new frmHTML5Export().ShowDialog(parent);
      }
   }
}
