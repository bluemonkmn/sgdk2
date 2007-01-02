using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
	public class frmGfxSheetImport : SGDK2.frmWizardBase
	{
      #region Non-control members
      private ProjectDataset importData;
      private string[] importNames = null;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlSpecifySource;
      private System.Windows.Forms.Label lblSourceDetails;
      private System.Windows.Forms.TextBox txtSourceDetails;
      private System.Windows.Forms.Button btnBrowseSource;
      private System.Windows.Forms.TextBox txtImportSource;
      private System.Windows.Forms.Label lblImportSource;
      private System.Windows.Forms.Label lblSourceInfo;
      private SGDK2.frmWizardBase.StepInfo SpecifySource;
      private System.Windows.Forms.Panel pnlSelectObject;
      private System.Windows.Forms.CheckedListBox chlImportList;
      private System.Windows.Forms.Label lblImportList;
      private System.Windows.Forms.Label lblSelectObjectInfo;
      private SGDK2.frmWizardBase.StepInfo SelectObject;
      private System.Windows.Forms.Panel pnlUniqueNames;
      private System.Windows.Forms.DataGrid grdNameMap;
      private System.Windows.Forms.Label lblUniqueNames;
      private SGDK2.frmWizardBase.StepInfo SpecifyNames;
      private System.Windows.Forms.DataGridTableStyle gridStyle;
      private System.Data.DataSet dsMapping;
      private System.Data.DataTable dtGraphicNames;
      private System.Data.DataColumn dcOldGSName;
      private System.Data.DataColumn dcNewGSName;
      private System.Data.DataView dvGraphicNames;
      private System.Windows.Forms.DataGridTextBoxColumn colSource;
      private System.Windows.Forms.DataGridTextBoxColumn colTarget;
      private System.Windows.Forms.Panel pnlReview;
      private System.Windows.Forms.TextBox txtReview;
      private System.Windows.Forms.Label lblReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.OpenFileDialog dlgImportSource;
		private System.ComponentModel.IContainer components = null;
      #endregion

		public frmGfxSheetImport()
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
         this.pnlSpecifySource = new System.Windows.Forms.Panel();
         this.lblSourceDetails = new System.Windows.Forms.Label();
         this.txtSourceDetails = new System.Windows.Forms.TextBox();
         this.btnBrowseSource = new System.Windows.Forms.Button();
         this.txtImportSource = new System.Windows.Forms.TextBox();
         this.lblImportSource = new System.Windows.Forms.Label();
         this.lblSourceInfo = new System.Windows.Forms.Label();
         this.SpecifySource = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSelectObject = new System.Windows.Forms.Panel();
         this.chlImportList = new System.Windows.Forms.CheckedListBox();
         this.lblImportList = new System.Windows.Forms.Label();
         this.lblSelectObjectInfo = new System.Windows.Forms.Label();
         this.SelectObject = new SGDK2.frmWizardBase.StepInfo();
         this.pnlUniqueNames = new System.Windows.Forms.Panel();
         this.grdNameMap = new System.Windows.Forms.DataGrid();
         this.dvGraphicNames = new System.Data.DataView();
         this.dtGraphicNames = new System.Data.DataTable();
         this.dcOldGSName = new System.Data.DataColumn();
         this.dcNewGSName = new System.Data.DataColumn();
         this.gridStyle = new System.Windows.Forms.DataGridTableStyle();
         this.colSource = new System.Windows.Forms.DataGridTextBoxColumn();
         this.colTarget = new System.Windows.Forms.DataGridTextBoxColumn();
         this.lblUniqueNames = new System.Windows.Forms.Label();
         this.SpecifyNames = new SGDK2.frmWizardBase.StepInfo();
         this.dsMapping = new System.Data.DataSet();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.dlgImportSource = new System.Windows.Forms.OpenFileDialog();
         this.pnlSpecifySource.SuspendLayout();
         this.pnlSelectObject.SuspendLayout();
         this.pnlUniqueNames.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdNameMap)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvGraphicNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtGraphicNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dsMapping)).BeginInit();
         this.pnlReview.SuspendLayout();
         this.SuspendLayout();
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
         this.pnlSpecifySource.Size = new System.Drawing.Size(283, 231);
         this.pnlSpecifySource.TabIndex = 7;
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
         this.lblSourceInfo.Text = "Graphic sheets can be imported from other projects or templates.  A graphic sheet" +
            " may be imported and used as-is or customized/expanded after import.";
         // 
         // SpecifySource
         // 
         this.SpecifySource.StepControl = this.pnlSpecifySource;
         this.SpecifySource.TitleText = "Specify Source";
         this.SpecifySource.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifySource_ValidateFunction);
         // 
         // pnlSelectObject
         // 
         this.pnlSelectObject.Controls.Add(this.chlImportList);
         this.pnlSelectObject.Controls.Add(this.lblImportList);
         this.pnlSelectObject.Controls.Add(this.lblSelectObjectInfo);
         this.pnlSelectObject.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSelectObject.Name = "pnlSelectObject";
         this.pnlSelectObject.Size = new System.Drawing.Size(285, 231);
         this.pnlSelectObject.TabIndex = 8;
         // 
         // chlImportList
         // 
         this.chlImportList.CheckOnClick = true;
         this.chlImportList.Location = new System.Drawing.Point(24, 72);
         this.chlImportList.Name = "chlImportList";
         this.chlImportList.Size = new System.Drawing.Size(232, 139);
         this.chlImportList.TabIndex = 4;
         this.chlImportList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlImportList_ItemCheck);
         // 
         // lblImportList
         // 
         this.lblImportList.Location = new System.Drawing.Point(24, 56);
         this.lblImportList.Name = "lblImportList";
         this.lblImportList.Size = new System.Drawing.Size(232, 16);
         this.lblImportList.TabIndex = 3;
         this.lblImportList.Text = "Objects to Import:";
         // 
         // lblSelectObjectInfo
         // 
         this.lblSelectObjectInfo.Location = new System.Drawing.Point(8, 8);
         this.lblSelectObjectInfo.Name = "lblSelectObjectInfo";
         this.lblSelectObjectInfo.Size = new System.Drawing.Size(264, 48);
         this.lblSelectObjectInfo.TabIndex = 2;
         this.lblSelectObjectInfo.Text = "If any imported graphic sheet names already exist in the project, you will be pro" +
            "mpted for new names in the next step.";
         // 
         // SelectObject
         // 
         this.SelectObject.StepControl = this.pnlSelectObject;
         this.SelectObject.TitleText = "Select Sheet";
         this.SelectObject.InitFunction += new System.EventHandler(this.SelectObject_InitFunction);
         this.SelectObject.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SelectObject_IsApplicableFunction);
         this.SelectObject.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SelectObject_ValidateFunction);
         // 
         // pnlUniqueNames
         // 
         this.pnlUniqueNames.Controls.Add(this.grdNameMap);
         this.pnlUniqueNames.Controls.Add(this.lblUniqueNames);
         this.pnlUniqueNames.Location = new System.Drawing.Point(-10168, 42);
         this.pnlUniqueNames.Name = "pnlUniqueNames";
         this.pnlUniqueNames.Size = new System.Drawing.Size(285, 231);
         this.pnlUniqueNames.TabIndex = 9;
         // 
         // grdNameMap
         // 
         this.grdNameMap.AllowNavigation = false;
         this.grdNameMap.CaptionVisible = false;
         this.grdNameMap.DataMember = "";
         this.grdNameMap.DataSource = this.dvGraphicNames;
         this.grdNameMap.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdNameMap.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdNameMap.Location = new System.Drawing.Point(0, 56);
         this.grdNameMap.Name = "grdNameMap";
         this.grdNameMap.RowHeadersVisible = false;
         this.grdNameMap.Size = new System.Drawing.Size(285, 175);
         this.grdNameMap.TabIndex = 1;
         this.grdNameMap.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
                                                                                               this.gridStyle});
         // 
         // dvGraphicNames
         // 
         this.dvGraphicNames.AllowDelete = false;
         this.dvGraphicNames.AllowNew = false;
         this.dvGraphicNames.Table = this.dtGraphicNames;
         // 
         // dtGraphicNames
         // 
         this.dtGraphicNames.Columns.AddRange(new System.Data.DataColumn[] {
                                                                              this.dcOldGSName,
                                                                              this.dcNewGSName});
         this.dtGraphicNames.Constraints.AddRange(new System.Data.Constraint[] {
                                                                                  new System.Data.UniqueConstraint("Constraint1", new string[] {
                                                                                                                                                  "Old Name"}, true)});
         this.dtGraphicNames.PrimaryKey = new System.Data.DataColumn[] {
                                                                          this.dcOldGSName};
         this.dtGraphicNames.TableName = "GraphicNames";
         // 
         // dcOldGSName
         // 
         this.dcOldGSName.AllowDBNull = false;
         this.dcOldGSName.ColumnName = "Old Name";
         // 
         // dcNewGSName
         // 
         this.dcNewGSName.ColumnName = "New Name";
         // 
         // gridStyle
         // 
         this.gridStyle.DataGrid = this.grdNameMap;
         this.gridStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
                                                                                                    this.colSource,
                                                                                                    this.colTarget});
         this.gridStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.gridStyle.MappingName = "GraphicNames";
         this.gridStyle.RowHeadersVisible = false;
         // 
         // colSource
         // 
         this.colSource.Format = "";
         this.colSource.FormatInfo = null;
         this.colSource.HeaderText = "Old (Source) Name";
         this.colSource.MappingName = "Old Name";
         this.colSource.ReadOnly = true;
         this.colSource.Width = 130;
         // 
         // colTarget
         // 
         this.colTarget.Format = "";
         this.colTarget.FormatInfo = null;
         this.colTarget.HeaderText = "New (Target) Name";
         this.colTarget.MappingName = "New Name";
         this.colTarget.NullText = "";
         this.colTarget.Width = 130;
         // 
         // lblUniqueNames
         // 
         this.lblUniqueNames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblUniqueNames.Location = new System.Drawing.Point(0, 0);
         this.lblUniqueNames.Name = "lblUniqueNames";
         this.lblUniqueNames.Size = new System.Drawing.Size(285, 56);
         this.lblUniqueNames.TabIndex = 0;
         this.lblUniqueNames.Text = "Imported graphic sheet names are duplicates of graphic sheet names that already e" +
            "xist in the project.  Specify unique names for the imported graphic sheets.";
         // 
         // SpecifyNames
         // 
         this.SpecifyNames.StepControl = this.pnlUniqueNames;
         this.SpecifyNames.TitleText = "Specify Names";
         this.SpecifyNames.InitFunction += new System.EventHandler(this.SpecifyNames_InitFunction);
         this.SpecifyNames.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifyNames_IsApplicableFunction);
         this.SpecifyNames.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifyNames_ValidateFunction);
         // 
         // dsMapping
         // 
         this.dsMapping.DataSetName = "Name Mapping";
         this.dsMapping.Locale = new System.Globalization.CultureInfo("en-US");
         this.dsMapping.Tables.AddRange(new System.Data.DataTable[] {
                                                                       this.dtGraphicNames});
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(282, 231);
         this.pnlReview.TabIndex = 12;
         // 
         // txtReview
         // 
         this.txtReview.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtReview.Location = new System.Drawing.Point(0, 56);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(282, 175);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblReview.Location = new System.Drawing.Point(0, 0);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(282, 56);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The graphic sheet import wizard has all the information needed to import the requ" +
            "ested graphic sheets.  The following actions will occur when you click Finish..." +
            "";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // dlgImportSource
         // 
         this.dlgImportSource.DefaultExt = "sgdk2";
         this.dlgImportSource.Filter = "SGDK2 Project (*.sgdk2)|*.SGDK2|All Files (*.*)|*.*";
         this.dlgImportSource.Title = "Specify Graphic Sheet Import Source";
         // 
         // frmGfxSheetImport
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlUniqueNames);
         this.Controls.Add(this.pnlSelectObject);
         this.Controls.Add(this.pnlSpecifySource);
         this.Name = "frmGfxSheetImport";
         this.Steps.Add(this.SpecifySource);
         this.Steps.Add(this.SelectObject);
         this.Steps.Add(this.SpecifyNames);
         this.Steps.Add(this.Review);
         this.Text = "Import Graphic Sheets";
         this.Controls.SetChildIndex(this.pnlSpecifySource, 0);
         this.Controls.SetChildIndex(this.pnlSelectObject, 0);
         this.Controls.SetChildIndex(this.pnlUniqueNames, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.pnlSpecifySource.ResumeLayout(false);
         this.pnlSelectObject.ResumeLayout(false);
         this.pnlUniqueNames.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdNameMap)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvGraphicNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtGraphicNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dsMapping)).EndInit();
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
            if (importData.GraphicSheet.Count <= 0)
            {
               MessageBox.Show(this, "There are no graphic sheets in the specified file", "Import Graphic Sheets", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            importNames = null;
            chlImportList.Items.Clear();
            dsMapping.Clear();
            return true;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Reading Import Source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }

      private bool SelectObject_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (importData.GraphicSheet.Count > 1)
            return true;
         importNames = new string[] {importData.GraphicSheet[0].Name};
         return false;
      }

      private void SelectObject_InitFunction(object sender, System.EventArgs e)
      {
         if (chlImportList.Items.Count > 0)
            return;

         foreach(ProjectDataset.GraphicSheetRow drGfx in importData.GraphicSheet)
            chlImportList.Items.Add(drGfx.Name);
      }

      private bool SelectObject_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (chlImportList.SelectedIndices.Count <= 0)
         {
            System.Windows.Forms.MessageBox.Show(this, "Select at least one graphic sheet to import.", "Import Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         importNames = new string[chlImportList.CheckedItems.Count];
         for (int i=0; i<chlImportList.CheckedItems.Count; i++)
            importNames[i] = (string)chlImportList.CheckedItems[i];
         return true;
      }

      private bool SpecifyNames_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(string name in importNames)
         {
            if (ProjectData.GetGraphicSheet(name) != null)
               return true;
         }
         return false;
      }

      private void SpecifyNames_InitFunction(object sender, System.EventArgs e)
      {
         if (dtGraphicNames.Rows.Count > 0)
            return;

         foreach(string name in importNames)
            if (ProjectData.GetGraphicSheet(name) != null)
               dtGraphicNames.Rows.Add(new object[] {name, null});
      }

      private bool SpecifyNames_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(System.Data.DataRow dr in dtGraphicNames.Rows)
         {
            if (!(dr[dcNewGSName] is string))
            {
               MessageBox.Show(this, "Please enter a new name for \"" + dr[dcOldGSName].ToString() + "\".", "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            string msg = ProjectData.ValidateName(dr[dcNewGSName].ToString());
            if (msg != null)
            {
               MessageBox.Show(this, "Invalid new name entered for \"" + dr[dcNewGSName].ToString() + "\".  " + msg, "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            ProjectDataset.GraphicSheetRow gfxOld = ProjectData.GetGraphicSheet(dr[dcNewGSName].ToString());
            if (gfxOld != null)
            {
               MessageBox.Show(this, "The new name specified for \"" + dr[dcOldGSName].ToString() + "\" conflicts with the name of an existing graphic sheet.  Specify a unique name.", "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
         }
         return true;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         foreach(string name in importNames)
         {
            System.Data.DataRow drName = dtGraphicNames.Rows.Find(name);
            if (drName != null)
               sb.Append("Import " + name + " as " + drName[dcNewGSName].ToString() + "\r\n");
            else
               sb.Append("Import " + name + "\r\n");
         }
         sb.Append(ProjectData.GetCreditAdditions(importData));
         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            foreach(string name in importNames)
            {
               System.Data.DataRow drName = dtGraphicNames.Rows.Find(name);
               ProjectDataset.GraphicSheetRow drTmp = importData.GraphicSheet.FindByName(name);
               ProjectData.ReencapsulateGraphicSheet(txtImportSource.Text, drTmp);
               if (drName == null)
                  ProjectData.GraphicSheet.Rows.Add(drTmp.ItemArray);
               else
                  ProjectData.GraphicSheet.AddGraphicSheetRow(drName[dcNewGSName].ToString(),
                     drTmp.Columns, drTmp.Rows, drTmp.CellWidth, drTmp.CellHeight, drTmp.Image);
            }
            ProjectData.MergeCredits(importData);
            return true;
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.ToString(), "Import Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }

      private void chlImportList_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         dsMapping.Clear();
      }
      #endregion
   }
}

