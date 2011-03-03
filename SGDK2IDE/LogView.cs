/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for LogView.
	/// </summary>
	public class frmLogView : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TextBox txtLogView;
      private SplitContainer splitLog;
      private GroupBox grpErrorList;
      private ListView lvwObjectErrors;
      private MenuItem mnuErrorLog;
      private MenuItem mnuJumpToError;
      private GroupBox grpDetails;
      private IContainer components;

		public frmLogView(string text, System.Collections.Generic.IEnumerable<CodeGenerator.ObjectErrorInfo> errorRows)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

			txtLogView.Text = text;

         lvwObjectErrors.Items.AddRange(System.Linq.Enumerable.ToArray(
            System.Linq.Enumerable.Select(errorRows,
            i => new ListViewItem(new string[] { i.GetSourceType(), i.GetSourceName(), i.Message }) { Tag = i })));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         System.Windows.Forms.ColumnHeader colErrObjType;
         System.Windows.Forms.ColumnHeader colObjName;
         System.Windows.Forms.ColumnHeader colMessage;
         System.Windows.Forms.MainMenu mnuLog;
         this.mnuErrorLog = new System.Windows.Forms.MenuItem();
         this.mnuJumpToError = new System.Windows.Forms.MenuItem();
         this.txtLogView = new System.Windows.Forms.TextBox();
         this.splitLog = new System.Windows.Forms.SplitContainer();
         this.grpErrorList = new System.Windows.Forms.GroupBox();
         this.lvwObjectErrors = new System.Windows.Forms.ListView();
         this.grpDetails = new System.Windows.Forms.GroupBox();
         colErrObjType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         colObjName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         mnuLog = new System.Windows.Forms.MainMenu(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.splitLog)).BeginInit();
         this.splitLog.Panel1.SuspendLayout();
         this.splitLog.Panel2.SuspendLayout();
         this.splitLog.SuspendLayout();
         this.grpErrorList.SuspendLayout();
         this.grpDetails.SuspendLayout();
         this.SuspendLayout();
         // 
         // colErrObjType
         // 
         colErrObjType.Text = "Object Type";
         colErrObjType.Width = 70;
         // 
         // colObjName
         // 
         colObjName.Text = "Object Name";
         colObjName.Width = 250;
         // 
         // colMessage
         // 
         colMessage.Text = "Error Message";
         colMessage.Width = 379;
         // 
         // mnuLog
         // 
         mnuLog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuErrorLog});
         // 
         // mnuErrorLog
         // 
         this.mnuErrorLog.Index = 0;
         this.mnuErrorLog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuJumpToError});
         this.mnuErrorLog.MergeOrder = 2;
         this.mnuErrorLog.Text = "&Error Log";
         // 
         // mnuJumpToError
         // 
         this.mnuJumpToError.Index = 0;
         this.mnuJumpToError.Shortcut = System.Windows.Forms.Shortcut.CtrlJ;
         this.mnuJumpToError.Text = "&Jump to Selected Error";
         this.mnuJumpToError.Click += new System.EventHandler(this.mnuJumpToError_Click);
         // 
         // txtLogView
         // 
         this.txtLogView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtLogView.Location = new System.Drawing.Point(3, 16);
         this.txtLogView.Multiline = true;
         this.txtLogView.Name = "txtLogView";
         this.txtLogView.ReadOnly = true;
         this.txtLogView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtLogView.Size = new System.Drawing.Size(793, 162);
         this.txtLogView.TabIndex = 0;
         // 
         // splitLog
         // 
         this.splitLog.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitLog.Location = new System.Drawing.Point(0, 0);
         this.splitLog.Name = "splitLog";
         this.splitLog.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitLog.Panel1
         // 
         this.splitLog.Panel1.Controls.Add(this.grpErrorList);
         // 
         // splitLog.Panel2
         // 
         this.splitLog.Panel2.Controls.Add(this.grpDetails);
         this.splitLog.Size = new System.Drawing.Size(799, 383);
         this.splitLog.SplitterDistance = 198;
         this.splitLog.TabIndex = 1;
         // 
         // grpErrorList
         // 
         this.grpErrorList.Controls.Add(this.lvwObjectErrors);
         this.grpErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpErrorList.Location = new System.Drawing.Point(0, 0);
         this.grpErrorList.Name = "grpErrorList";
         this.grpErrorList.Size = new System.Drawing.Size(799, 198);
         this.grpErrorList.TabIndex = 1;
         this.grpErrorList.TabStop = false;
         this.grpErrorList.Text = "Error Listing (Double-click to jump to source)";
         // 
         // lvwObjectErrors
         // 
         this.lvwObjectErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            colErrObjType,
            colObjName,
            colMessage});
         this.lvwObjectErrors.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lvwObjectErrors.FullRowSelect = true;
         this.lvwObjectErrors.Location = new System.Drawing.Point(3, 16);
         this.lvwObjectErrors.Name = "lvwObjectErrors";
         this.lvwObjectErrors.Size = new System.Drawing.Size(793, 179);
         this.lvwObjectErrors.TabIndex = 0;
         this.lvwObjectErrors.UseCompatibleStateImageBehavior = false;
         this.lvwObjectErrors.View = System.Windows.Forms.View.Details;
         this.lvwObjectErrors.DoubleClick += new System.EventHandler(this.lvwObjectErrors_DoubleClick);
         // 
         // grpDetails
         // 
         this.grpDetails.Controls.Add(this.txtLogView);
         this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpDetails.Location = new System.Drawing.Point(0, 0);
         this.grpDetails.Name = "grpDetails";
         this.grpDetails.Size = new System.Drawing.Size(799, 181);
         this.grpDetails.TabIndex = 1;
         this.grpDetails.TabStop = false;
         this.grpDetails.Text = "Detailed Compiler Output";
         // 
         // frmLogView
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(799, 383);
         this.Controls.Add(this.splitLog);
         this.Menu = mnuLog;
         this.Name = "frmLogView";
         this.Text = "Log Viewer";
         this.splitLog.Panel1.ResumeLayout(false);
         this.splitLog.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitLog)).EndInit();
         this.splitLog.ResumeLayout(false);
         this.grpErrorList.ResumeLayout(false);
         this.grpDetails.ResumeLayout(false);
         this.grpDetails.PerformLayout();
         this.ResumeLayout(false);

      }
		#endregion
	
      #region Overrides
      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }

      #endregion

      #region Private Members
      private void JumpToError()
      {
         if (lvwObjectErrors.SelectedItems.Count != 1)
         {
            MessageBox.Show("Select one error first.", "Jump to Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         CodeGenerator.ObjectErrorInfo err = ((CodeGenerator.ObjectErrorInfo)lvwObjectErrors.SelectedItems[0].Tag);
         if (err.source is ProjectDataset.SpriteRuleRow)
         {
            ProjectDataset.SpriteRuleRow drSpriteRule = (ProjectDataset.SpriteRuleRow)err.source;
            frmSpriteDefinition.Edit(MdiParent, drSpriteRule);
         }
         else if (err.source is ProjectDataset.PlanRuleRow)
         {
            ProjectDataset.PlanRuleRow drPlanRule = (ProjectDataset.PlanRuleRow)err.source;
            frmPlanEdit.Edit(MdiParent, drPlanRule);
         }
      }
      #endregion

      private void lvwObjectErrors_DoubleClick(object sender, EventArgs e)
      {
         JumpToError();
      }

      private void mnuJumpToError_Click(object sender, EventArgs e)
      {
         JumpToError();
      }
   }
}
