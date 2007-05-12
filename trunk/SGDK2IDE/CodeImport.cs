/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
	public class frmCodeImport : SGDK2.frmWizardBase
	{
      #region Non-control members
      private ProjectDataset importData;
      #endregion

      #region Form Designer Members
      private SGDK2.frmWizardBase.StepInfo SpecifySource;
      private System.Windows.Forms.Panel pnlSpecifySource;
      private System.Windows.Forms.Label lblSourceInfo;
      private System.Windows.Forms.Label lblImportSource;
      private System.Windows.Forms.TextBox txtImportSource;
      private System.Windows.Forms.Button btnBrowseSource;
      private System.Windows.Forms.TextBox txtSourceDetails;
      private System.Windows.Forms.OpenFileDialog dlgImportSource;
      private System.Windows.Forms.Label lblSourceDetails;
      private System.Windows.Forms.Panel pnlSelectObject;
      private SGDK2.frmWizardBase.StepInfo SelectObject;
      private System.Windows.Forms.Label lblSelectObjectInfo;
      private System.Windows.Forms.Label lblImportList;
      private System.Windows.Forms.CheckedListBox chlImportList;
      private System.Windows.Forms.Panel pnlConfirmOverwrites;
      private SGDK2.frmWizardBase.StepInfo ConfirmOverwrites;
      private System.Windows.Forms.Label lblOverwriteInfo;
      private System.Windows.Forms.Label lblOverwrite;
      private System.Windows.Forms.CheckedListBox chlOverwrite;
      private System.Windows.Forms.Panel pnlReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
		private System.ComponentModel.IContainer components = null;
      #endregion

		public frmCodeImport()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.SpecifySource = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSpecifySource = new System.Windows.Forms.Panel();
         this.lblSourceDetails = new System.Windows.Forms.Label();
         this.txtSourceDetails = new System.Windows.Forms.TextBox();
         this.btnBrowseSource = new System.Windows.Forms.Button();
         this.txtImportSource = new System.Windows.Forms.TextBox();
         this.lblImportSource = new System.Windows.Forms.Label();
         this.lblSourceInfo = new System.Windows.Forms.Label();
         this.dlgImportSource = new System.Windows.Forms.OpenFileDialog();
         this.pnlSelectObject = new System.Windows.Forms.Panel();
         this.chlImportList = new System.Windows.Forms.CheckedListBox();
         this.lblImportList = new System.Windows.Forms.Label();
         this.lblSelectObjectInfo = new System.Windows.Forms.Label();
         this.SelectObject = new SGDK2.frmWizardBase.StepInfo();
         this.pnlConfirmOverwrites = new System.Windows.Forms.Panel();
         this.chlOverwrite = new System.Windows.Forms.CheckedListBox();
         this.lblOverwrite = new System.Windows.Forms.Label();
         this.lblOverwriteInfo = new System.Windows.Forms.Label();
         this.ConfirmOverwrites = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSpecifySource.SuspendLayout();
         this.pnlSelectObject.SuspendLayout();
         this.pnlConfirmOverwrites.SuspendLayout();
         this.pnlReview.SuspendLayout();
         this.SuspendLayout();
         // 
         // SpecifySource
         // 
         this.SpecifySource.StepControl = this.pnlSpecifySource;
         this.SpecifySource.TitleText = "Specify Source";
         this.SpecifySource.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifySource_ValidateFunction);
         // 
         // pnlSpecifySource
         // 
         this.pnlSpecifySource.Controls.Add(this.lblSourceDetails);
         this.pnlSpecifySource.Controls.Add(this.txtSourceDetails);
         this.pnlSpecifySource.Controls.Add(this.btnBrowseSource);
         this.pnlSpecifySource.Controls.Add(this.txtImportSource);
         this.pnlSpecifySource.Controls.Add(this.lblImportSource);
         this.pnlSpecifySource.Controls.Add(this.lblSourceInfo);
         this.pnlSpecifySource.Location = new System.Drawing.Point(168, 42);
         this.pnlSpecifySource.Name = "pnlSpecifySource";
         this.pnlSpecifySource.Size = new System.Drawing.Size(281, 231);
         this.pnlSpecifySource.TabIndex = 6;
         // 
         // lblSourceDetails
         // 
         this.lblSourceDetails.Location = new System.Drawing.Point(8, 120);
         this.lblSourceDetails.Name = "lblSourceDetails";
         this.lblSourceDetails.Size = new System.Drawing.Size(80, 16);
         this.lblSourceDetails.TabIndex = 5;
         this.lblSourceDetails.Text = "File Details:";
         // 
         // txtSourceDetails
         // 
         this.txtSourceDetails.Location = new System.Drawing.Point(88, 120);
         this.txtSourceDetails.Multiline = true;
         this.txtSourceDetails.Name = "txtSourceDetails";
         this.txtSourceDetails.ReadOnly = true;
         this.txtSourceDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtSourceDetails.Size = new System.Drawing.Size(192, 104);
         this.txtSourceDetails.TabIndex = 4;
         this.txtSourceDetails.Text = "";
         // 
         // btnBrowseSource
         // 
         this.btnBrowseSource.Location = new System.Drawing.Point(248, 88);
         this.btnBrowseSource.Name = "btnBrowseSource";
         this.btnBrowseSource.Size = new System.Drawing.Size(24, 20);
         this.btnBrowseSource.TabIndex = 3;
         this.btnBrowseSource.Text = "...";
         this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
         // 
         // txtImportSource
         // 
         this.txtImportSource.Location = new System.Drawing.Point(88, 88);
         this.txtImportSource.Name = "txtImportSource";
         this.txtImportSource.Size = new System.Drawing.Size(160, 20);
         this.txtImportSource.TabIndex = 2;
         this.txtImportSource.Text = "";
         this.txtImportSource.TextChanged += new System.EventHandler(this.txtImportSource_TextChanged);
         // 
         // lblImportSource
         // 
         this.lblImportSource.Location = new System.Drawing.Point(8, 88);
         this.lblImportSource.Name = "lblImportSource";
         this.lblImportSource.Size = new System.Drawing.Size(80, 20);
         this.lblImportSource.TabIndex = 1;
         this.lblImportSource.Text = "Source File:";
         this.lblImportSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblSourceInfo
         // 
         this.lblSourceInfo.Location = new System.Drawing.Point(8, 8);
         this.lblSourceInfo.Name = "lblSourceInfo";
         this.lblSourceInfo.Size = new System.Drawing.Size(264, 72);
         this.lblSourceInfo.TabIndex = 0;
         this.lblSourceInfo.Text = "Code objects can be imported from other projects or templates.  A code object may" +
            " represent a specific object that you want to use in this project, or simply an " +
            "example template of an object that needs to be customized after import.";
         // 
         // dlgImportSource
         // 
         this.dlgImportSource.DefaultExt = "sgdk2";
         this.dlgImportSource.Filter = "SGDK2 Project (*.sgdk2)|*.SGDK2|All Files (*.*)|*.*";
         this.dlgImportSource.Title = "Specify Source Code Import Source";
         // 
         // pnlSelectObject
         // 
         this.pnlSelectObject.Controls.Add(this.chlImportList);
         this.pnlSelectObject.Controls.Add(this.lblImportList);
         this.pnlSelectObject.Controls.Add(this.lblSelectObjectInfo);
         this.pnlSelectObject.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSelectObject.Name = "pnlSelectObject";
         this.pnlSelectObject.Size = new System.Drawing.Size(284, 231);
         this.pnlSelectObject.TabIndex = 7;
         // 
         // chlImportList
         // 
         this.chlImportList.CheckOnClick = true;
         this.chlImportList.Location = new System.Drawing.Point(24, 88);
         this.chlImportList.Name = "chlImportList";
         this.chlImportList.Size = new System.Drawing.Size(232, 139);
         this.chlImportList.TabIndex = 4;
         this.chlImportList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlImportList_ItemCheck);
         // 
         // lblImportList
         // 
         this.lblImportList.Location = new System.Drawing.Point(24, 72);
         this.lblImportList.Name = "lblImportList";
         this.lblImportList.Size = new System.Drawing.Size(232, 16);
         this.lblImportList.TabIndex = 3;
         this.lblImportList.Text = "Objects to Import:";
         // 
         // lblSelectObjectInfo
         // 
         this.lblSelectObjectInfo.Location = new System.Drawing.Point(8, 8);
         this.lblSelectObjectInfo.Name = "lblSelectObjectInfo";
         this.lblSelectObjectInfo.Size = new System.Drawing.Size(264, 64);
         this.lblSelectObjectInfo.TabIndex = 2;
         this.lblSelectObjectInfo.Text = "Imported objects will implicitly import everything that they depend on.  If any o" +
            "bjects by the same name already exist in the project, you will decide in the nex" +
            "t step whether to overwrite them or ignore them.";
         // 
         // SelectObject
         // 
         this.SelectObject.StepControl = this.pnlSelectObject;
         this.SelectObject.TitleText = "Select Object";
         this.SelectObject.InitFunction += new System.EventHandler(this.SelectObject_InitFunction);
         // 
         // pnlConfirmOverwrites
         // 
         this.pnlConfirmOverwrites.Controls.Add(this.chlOverwrite);
         this.pnlConfirmOverwrites.Controls.Add(this.lblOverwrite);
         this.pnlConfirmOverwrites.Controls.Add(this.lblOverwriteInfo);
         this.pnlConfirmOverwrites.Location = new System.Drawing.Point(-10168, 42);
         this.pnlConfirmOverwrites.Name = "pnlConfirmOverwrites";
         this.pnlConfirmOverwrites.Size = new System.Drawing.Size(281, 231);
         this.pnlConfirmOverwrites.TabIndex = 8;
         // 
         // chlOverwrite
         // 
         this.chlOverwrite.CheckOnClick = true;
         this.chlOverwrite.Location = new System.Drawing.Point(16, 88);
         this.chlOverwrite.Name = "chlOverwrite";
         this.chlOverwrite.Size = new System.Drawing.Size(248, 139);
         this.chlOverwrite.TabIndex = 2;
         // 
         // lblOverwrite
         // 
         this.lblOverwrite.Location = new System.Drawing.Point(16, 72);
         this.lblOverwrite.Name = "lblOverwrite";
         this.lblOverwrite.Size = new System.Drawing.Size(248, 16);
         this.lblOverwrite.TabIndex = 1;
         this.lblOverwrite.Text = "Overwrite:";
         // 
         // lblOverwriteInfo
         // 
         this.lblOverwriteInfo.Location = new System.Drawing.Point(8, 8);
         this.lblOverwriteInfo.Name = "lblOverwriteInfo";
         this.lblOverwriteInfo.Size = new System.Drawing.Size(264, 64);
         this.lblOverwriteInfo.TabIndex = 0;
         this.lblOverwriteInfo.Text = "Objects with the names listed below already exist in the project.  Select which o" +
            "bjects you want to overwrite.  Unchecked items will be left untouched.";
         // 
         // ConfirmOverwrites
         // 
         this.ConfirmOverwrites.StepControl = this.pnlConfirmOverwrites;
         this.ConfirmOverwrites.TitleText = "Confirm Overwrites";
         this.ConfirmOverwrites.InitFunction += new System.EventHandler(this.ConfirmOverwrites_InitFunction);
         this.ConfirmOverwrites.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.ConfirmOverwrites_IsApplicableFunction);
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(281, 231);
         this.pnlReview.TabIndex = 9;
         // 
         // txtReview
         // 
         this.txtReview.Location = new System.Drawing.Point(8, 48);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(264, 176);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Location = new System.Drawing.Point(8, 8);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(264, 40);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The following actions will be taken when you click finish:";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // frmCodeImport
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlConfirmOverwrites);
         this.Controls.Add(this.pnlSelectObject);
         this.Controls.Add(this.pnlSpecifySource);
         this.Name = "frmCodeImport";
         this.Steps.Add(this.SpecifySource);
         this.Steps.Add(this.SelectObject);
         this.Steps.Add(this.ConfirmOverwrites);
         this.Steps.Add(this.Review);
         this.Text = "Import Custom Code Objects";
         this.Controls.SetChildIndex(this.pnlSpecifySource, 0);
         this.Controls.SetChildIndex(this.pnlSelectObject, 0);
         this.Controls.SetChildIndex(this.pnlConfirmOverwrites, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.pnlSpecifySource.ResumeLayout(false);
         this.pnlSelectObject.ResumeLayout(false);
         this.pnlConfirmOverwrites.ResumeLayout(false);
         this.pnlReview.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Event Handlers
      private void btnBrowseSource_Click(object sender, System.EventArgs e)
      {
         if (txtImportSource.Text.Length > 0)
            dlgImportSource.FileName = txtImportSource.Text;
         if (DialogResult.OK == dlgImportSource.ShowDialog(this))
         {
            txtImportSource.Text = dlgImportSource.FileName;
         }
      }

      private void txtImportSource_TextChanged(object sender, System.EventArgs e)
      {
         if (System.IO.File.Exists(txtImportSource.Text))
         {
            try
            {
               System.IO.TextReader tr = new System.IO.StreamReader(txtImportSource.Text);
               try
               {
                  System.Xml.XmlReader xml = new System.Xml.XmlTextReader(tr);
                  try
                  {
                     while(xml.Read())
                     {
                        if (xml.Name == "Project")
                        {
                           txtSourceDetails.Text = xml.GetAttribute("TitleText");
                           break;
                        }
                     }
                  }
                  finally
                  {
                     xml.Close();
                  }
               }
               finally
               {
                  tr.Close();
               }
            }
            catch(System.Exception ex)
            {
               txtSourceDetails.Text = ex.Message;
            }
         }
         else
         {
            txtSourceDetails.Text = String.Empty;
         }
      }

      private bool SpecifySource_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            importData = new ProjectDataset();
            importData.ReadXml(txtImportSource.Text, System.Data.XmlReadMode.IgnoreSchema);
            chlImportList.Items.Clear();
            return true;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Reading Import Source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }

      private void SelectObject_InitFunction(object sender, System.EventArgs e)
      {
         if (chlImportList.Items.Count > 0)
            return;

         foreach(System.Data.DataRowView drv in importData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            chlImportList.Items.Add(drCode.Name);
         }
      }

      private void chlImportList_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         ProjectDataset.SourceCodeRow drCode = importData.SourceCode.FindByName(chlImportList.Items[e.Index].ToString());
         if (e.CurrentValue == System.Windows.Forms.CheckState.Checked)
         {
            // Item was checked and will be unchecked.
            // Uncheck items depending on it.
            // Event recursion will propogate recursively.
            for (int i=0; i<chlImportList.Items.Count; i++)
            {
               ProjectDataset.SourceCodeRow drTest = importData.SourceCode.FindByName(chlImportList.Items[i].ToString());
               if (!drTest.IsDependsOnNull() && (drTest.DependsOn == drCode.Name))
                  chlImportList.SetItemChecked(i, false);
            }
         }
         else
         {
            // Item was unchecked and will be checked.
            // Check all its dependency if it has one.
            // Event recursion will propogate recursively.
            if (!drCode.IsDependsOnNull())
            {
               ProjectDataset.SourceCodeRow drParent = importData.SourceCode.FindByName(drCode.DependsOn);
               if (drParent != null)
                  chlImportList.SetItemChecked(chlImportList.FindStringExact(drParent.Name), true);
            }
         }
      }

      private bool ConfirmOverwrites_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         for(int i = 0; i < chlImportList.Items.Count; i++)
         {
            if (chlImportList.GetItemChecked(i) && (ProjectData.GetSourceCode(chlImportList.Items[i].ToString()) != null))
            {
               return true;
            }
         }
         return false;
      }

      private void ConfirmOverwrites_InitFunction(object sender, System.EventArgs e)
      {
         System.Collections.Specialized.StringCollection list = new System.Collections.Specialized.StringCollection();

         for(int i = 0; i < chlImportList.Items.Count; i++)
         {
            if ((chlImportList.GetItemChecked(i)) && (ProjectData.GetSourceCode(chlImportList.Items[i].ToString()) != null))
               list.Add(chlImportList.Items[i].ToString());
         }
         for (int i = 0; i < chlOverwrite.Items.Count; i++)
         {
            if (!list.Contains(chlOverwrite.Items[i].ToString()))
               chlOverwrite.Items.RemoveAt(i--);
         }
         foreach(string addString in list)
         {
            if (!chlOverwrite.Items.Contains(addString))
               chlOverwrite.Items.Add(addString);
         }
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         foreach(string item in chlImportList.CheckedItems)
         {
            ProjectDataset.SourceCodeRow drCode = importData.SourceCode.FindByName(item);
            if (chlOverwrite.CheckedItems.Contains(item) || !chlOverwrite.Items.Contains(item))
            {
               sb.Append(string.Format("Import {0} code object {1}\r\n", drCode.IsCustomObject?"custom":"standard", item));
               if (!drCode.IsCustomObjectDataNull())
                  sb.Append(string.Format("   This object includes {0} of extended data.\r\n", ProjectData.GetCustomObjectDataSize(drCode)));
               if (!drCode.IsDependsOnNull() && (drCode.DependsOn.Length > 0))
                  sb.Append(string.Format("   This object depends on {0}\r\n", drCode.DependsOn));
               if (chlOverwrite.CheckedItems.Contains(item))
                  sb.Append("   This will overwrite the already existing object\r\n");
            }
         }
         sb.Append(ProjectData.GetCreditAdditions(importData));
         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            foreach(string item in chlImportList.CheckedItems)
            {
               ProjectDataset.SourceCodeRow drCode = importData.SourceCode.FindByName(item);
               if (chlOverwrite.CheckedItems.Contains(item) || !chlOverwrite.Items.Contains(item))
               {
                  ProjectDataset.SourceCodeRow existing = ProjectData.GetSourceCode(item);
                  ProjectData.ReencapsulateSourceCode(txtImportSource.Text, drCode);
                  if (existing != null)
                     existing.ItemArray = drCode.ItemArray;
                  else
                     ProjectData.SourceCode.Rows.Add(drCode.ItemArray);
                  if (drCode.Name.EndsWith(".dll"))
                  {
                     string sourceFile = System.IO.Path.GetFileName(drCode.Name);
                     string sourcePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(txtImportSource.Text), sourceFile);
                     if (!System.IO.File.Exists(sourcePath))
                        sourcePath = System.IO.Path.Combine(Application.StartupPath, sourceFile);
                     if (System.IO.File.Exists(sourcePath))
                     {
                        if (SGDK2IDE.CurrentProjectFile != null)
                        {
                           string targetDir = System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile);
                           string targetPath = System.IO.Path.Combine(targetDir, sourceFile);
                           if (!System.IO.File.Exists(targetPath))
                           {
                              if (DialogResult.Yes == MessageBox.Show(this, "Would you like to copy the external file \"" + sourceFile + "\" into \"" + targetDir + "\"?", "Import Code Objects", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                 System.IO.File.Copy(sourcePath, targetPath, false);
                           }
                        }
                        else
                           MessageBox.Show(this, "Since you have not saved your project yet, there is no place to which the external file dependency \"" + sourceFile + "\" can be copied.  You may have to manually copy this file to your project directory in order for this code object to work", "Import Code Objects", MessageBoxButtons.OK);
                     }
                  }
               }
            }
            ProjectData.MergeCredits(importData);
            return true;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Import Code Objects", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }
      #endregion
   }
}

