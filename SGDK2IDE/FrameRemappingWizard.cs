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
	public class frmFrameRemappingWizard : SGDK2.frmWizardBase
	{
      #region Form Designer Members
      private System.Windows.Forms.Panel pnlIntro;
      private SGDK2.frmWizardBase.StepInfo Intro;
      private System.Windows.Forms.Label lblIntro;
      private System.Windows.Forms.Panel pnlSourceRange;
      private SGDK2.frmWizardBase.StepInfo SourceRange;
      private System.Windows.Forms.Label lblSpecifyRange;
      private SGDK2.GraphicBrowser SourceFrames;
      private System.Windows.Forms.Panel pnlSpecifyOperation;
      private SGDK2.frmWizardBase.StepInfo SpecifyOperation;
      private System.Windows.Forms.Label lblNewGfxSheet;
      private System.Windows.Forms.ComboBox cboNewGfxSheet;
      private System.Windows.Forms.RadioButton rdoCellOffset;
      private System.Windows.Forms.Label lblOffset;
      private System.Windows.Forms.NumericUpDown nudOffset;
      private System.Windows.Forms.RadioButton rdoCellSet;
      private System.Windows.Forms.Panel pnlTarget;
      private SGDK2.frmWizardBase.StepInfo Target;
      private System.Windows.Forms.Label lblTarget;
      private System.Windows.Forms.RadioButton rdoTargetSelf;
      private System.Windows.Forms.Label lblNewFrameset;
      private SGDK2.GraphicBrowser SetCellList;
      private System.Windows.Forms.NumericUpDown nudSetCellValue;
      private System.Windows.Forms.Label lblSetCellValue;
      private System.Windows.Forms.Panel pnlReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
      private System.Windows.Forms.ComboBox cboOtherFrameset;
      private System.Windows.Forms.Label lblOtherInfo;
      private System.Windows.Forms.RadioButton rdoTargetAppend;
		private System.ComponentModel.IContainer components = null;
      #endregion

		public frmFrameRemappingWizard(ProjectDataset.FramesetRow frameset)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

         SourceFrames.Frameset = frameset;
         if (ProjectData.GetSortedFrameRows(frameset).Length > 0)
            SourceFrames.CurrentCellIndex = 0;
         cboNewGfxSheet.Items.Add("(No change)");
         foreach (System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
            cboNewGfxSheet.Items.Add(drv.Row);
         cboNewGfxSheet.SelectedIndex = 0;
         foreach (System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
            cboOtherFrameset.Items.Add(((ProjectDataset.FramesetRow)drv.Row).Name);
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
         this.pnlIntro = new System.Windows.Forms.Panel();
         this.lblIntro = new System.Windows.Forms.Label();
         this.Intro = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSourceRange = new System.Windows.Forms.Panel();
         this.SourceFrames = new SGDK2.GraphicBrowser();
         this.lblSpecifyRange = new System.Windows.Forms.Label();
         this.SourceRange = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSpecifyOperation = new System.Windows.Forms.Panel();
         this.lblSetCellValue = new System.Windows.Forms.Label();
         this.nudSetCellValue = new System.Windows.Forms.NumericUpDown();
         this.SetCellList = new SGDK2.GraphicBrowser();
         this.rdoCellSet = new System.Windows.Forms.RadioButton();
         this.nudOffset = new System.Windows.Forms.NumericUpDown();
         this.lblOffset = new System.Windows.Forms.Label();
         this.rdoCellOffset = new System.Windows.Forms.RadioButton();
         this.cboNewGfxSheet = new System.Windows.Forms.ComboBox();
         this.lblNewGfxSheet = new System.Windows.Forms.Label();
         this.SpecifyOperation = new SGDK2.frmWizardBase.StepInfo();
         this.pnlTarget = new System.Windows.Forms.Panel();
         this.lblNewFrameset = new System.Windows.Forms.Label();
         this.rdoTargetAppend = new System.Windows.Forms.RadioButton();
         this.rdoTargetSelf = new System.Windows.Forms.RadioButton();
         this.lblTarget = new System.Windows.Forms.Label();
         this.Target = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.cboOtherFrameset = new System.Windows.Forms.ComboBox();
         this.lblOtherInfo = new System.Windows.Forms.Label();
         this.pnlIntro.SuspendLayout();
         this.pnlSourceRange.SuspendLayout();
         this.pnlSpecifyOperation.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudSetCellValue)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudOffset)).BeginInit();
         this.pnlTarget.SuspendLayout();
         this.pnlReview.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlIntro
         // 
         this.pnlIntro.Controls.Add(this.lblIntro);
         this.pnlIntro.Location = new System.Drawing.Point(168, 42);
         this.pnlIntro.Name = "pnlIntro";
         this.pnlIntro.Size = new System.Drawing.Size(282, 231);
         this.pnlIntro.TabIndex = 6;
         // 
         // lblIntro
         // 
         this.lblIntro.Location = new System.Drawing.Point(8, 8);
         this.lblIntro.Name = "lblIntro";
         this.lblIntro.Size = new System.Drawing.Size(264, 128);
         this.lblIntro.TabIndex = 0;
         this.lblIntro.Text = @"The frame remapping wizard will help you remap frames referring to a particular graphic sheet or cell number to refer to another graphic sheet or cell number.  This is useful if you want to create many similar framesets based on different graphics.  For example, you might create a frameset that rotates a single graphic cell to 36 different positions, and then want to create another frameset with similarly rotated images based on a different graphic.";
         // 
         // Intro
         // 
         this.Intro.StepControl = this.pnlIntro;
         this.Intro.TitleText = "Introduction";
         // 
         // pnlSourceRange
         // 
         this.pnlSourceRange.Controls.Add(this.SourceFrames);
         this.pnlSourceRange.Controls.Add(this.lblSpecifyRange);
         this.pnlSourceRange.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSourceRange.Name = "pnlSourceRange";
         this.pnlSourceRange.Size = new System.Drawing.Size(282, 231);
         this.pnlSourceRange.TabIndex = 7;
         // 
         // SourceFrames
         // 
         this.SourceFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SourceFrames.CellPadding = new System.Drawing.Size(2, 2);
         this.SourceFrames.CellSize = new System.Drawing.Size(0, 0);
         this.SourceFrames.CurrentCellIndex = -1;
         this.SourceFrames.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SourceFrames.Frameset = null;
         this.SourceFrames.FramesToDisplay = null;
         this.SourceFrames.GraphicSheet = null;
         this.SourceFrames.Location = new System.Drawing.Point(0, 63);
         this.SourceFrames.Name = "SourceFrames";
         this.SourceFrames.SheetImage = null;
         this.SourceFrames.Size = new System.Drawing.Size(282, 168);
         this.SourceFrames.TabIndex = 1;
         // 
         // lblSpecifyRange
         // 
         this.lblSpecifyRange.Location = new System.Drawing.Point(8, 8);
         this.lblSpecifyRange.Name = "lblSpecifyRange";
         this.lblSpecifyRange.Size = new System.Drawing.Size(264, 56);
         this.lblSpecifyRange.TabIndex = 0;
         this.lblSpecifyRange.Text = "Select the frames to be affected.  You can use the Ctrl and Shift keys to easily " +
            "select ranges of cells or all cells.";
         // 
         // SourceRange
         // 
         this.SourceRange.StepControl = this.pnlSourceRange;
         this.SourceRange.TitleText = "Specify Range";
         this.SourceRange.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SourceRange_ValidateFunction);
         // 
         // pnlSpecifyOperation
         // 
         this.pnlSpecifyOperation.Controls.Add(this.lblSetCellValue);
         this.pnlSpecifyOperation.Controls.Add(this.nudSetCellValue);
         this.pnlSpecifyOperation.Controls.Add(this.SetCellList);
         this.pnlSpecifyOperation.Controls.Add(this.rdoCellSet);
         this.pnlSpecifyOperation.Controls.Add(this.nudOffset);
         this.pnlSpecifyOperation.Controls.Add(this.lblOffset);
         this.pnlSpecifyOperation.Controls.Add(this.rdoCellOffset);
         this.pnlSpecifyOperation.Controls.Add(this.cboNewGfxSheet);
         this.pnlSpecifyOperation.Controls.Add(this.lblNewGfxSheet);
         this.pnlSpecifyOperation.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSpecifyOperation.Name = "pnlSpecifyOperation";
         this.pnlSpecifyOperation.Size = new System.Drawing.Size(281, 231);
         this.pnlSpecifyOperation.TabIndex = 8;
         // 
         // lblSetCellValue
         // 
         this.lblSetCellValue.Enabled = false;
         this.lblSetCellValue.Location = new System.Drawing.Point(40, 208);
         this.lblSetCellValue.Name = "lblSetCellValue";
         this.lblSetCellValue.Size = new System.Drawing.Size(80, 20);
         this.lblSetCellValue.TabIndex = 8;
         this.lblSetCellValue.Text = "Cell value:";
         this.lblSetCellValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudSetCellValue
         // 
         this.nudSetCellValue.Enabled = false;
         this.nudSetCellValue.Location = new System.Drawing.Point(120, 208);
         this.nudSetCellValue.Name = "nudSetCellValue";
         this.nudSetCellValue.Size = new System.Drawing.Size(80, 20);
         this.nudSetCellValue.TabIndex = 7;
         // 
         // SetCellList
         // 
         this.SetCellList.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SetCellList.CellPadding = new System.Drawing.Size(2, 2);
         this.SetCellList.CellSize = new System.Drawing.Size(0, 0);
         this.SetCellList.CurrentCellIndex = -1;
         this.SetCellList.Enabled = false;
         this.SetCellList.Frameset = null;
         this.SetCellList.FramesToDisplay = null;
         this.SetCellList.GraphicSheet = null;
         this.SetCellList.Location = new System.Drawing.Point(32, 120);
         this.SetCellList.Name = "SetCellList";
         this.SetCellList.SheetImage = null;
         this.SetCellList.Size = new System.Drawing.Size(240, 80);
         this.SetCellList.TabIndex = 6;
         this.SetCellList.CurrentCellChanged += new System.EventHandler(this.SetCellList_CurrentCellChanged);
         // 
         // rdoCellSet
         // 
         this.rdoCellSet.Location = new System.Drawing.Point(16, 88);
         this.rdoCellSet.Name = "rdoCellSet";
         this.rdoCellSet.Size = new System.Drawing.Size(248, 16);
         this.rdoCellSet.TabIndex = 5;
         this.rdoCellSet.Text = "Set cell value";
         this.rdoCellSet.CheckedChanged += new System.EventHandler(this.Operation_CheckedChanged);
         // 
         // nudOffset
         // 
         this.nudOffset.Location = new System.Drawing.Point(120, 64);
         this.nudOffset.Maximum = new System.Decimal(new int[] {
                                                                  30000,
                                                                  0,
                                                                  0,
                                                                  0});
         this.nudOffset.Minimum = new System.Decimal(new int[] {
                                                                  30000,
                                                                  0,
                                                                  0,
                                                                  -2147483648});
         this.nudOffset.Name = "nudOffset";
         this.nudOffset.Size = new System.Drawing.Size(80, 20);
         this.nudOffset.TabIndex = 4;
         // 
         // lblOffset
         // 
         this.lblOffset.Location = new System.Drawing.Point(40, 64);
         this.lblOffset.Name = "lblOffset";
         this.lblOffset.Size = new System.Drawing.Size(80, 20);
         this.lblOffset.TabIndex = 3;
         this.lblOffset.Text = "Offset by:";
         this.lblOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // rdoCellOffset
         // 
         this.rdoCellOffset.Checked = true;
         this.rdoCellOffset.Location = new System.Drawing.Point(16, 40);
         this.rdoCellOffset.Name = "rdoCellOffset";
         this.rdoCellOffset.Size = new System.Drawing.Size(248, 16);
         this.rdoCellOffset.TabIndex = 2;
         this.rdoCellOffset.TabStop = true;
         this.rdoCellOffset.Text = "Offset cell index";
         this.rdoCellOffset.CheckedChanged += new System.EventHandler(this.Operation_CheckedChanged);
         // 
         // cboNewGfxSheet
         // 
         this.cboNewGfxSheet.DisplayMember = "Name";
         this.cboNewGfxSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboNewGfxSheet.Location = new System.Drawing.Point(120, 8);
         this.cboNewGfxSheet.Name = "cboNewGfxSheet";
         this.cboNewGfxSheet.Size = new System.Drawing.Size(152, 21);
         this.cboNewGfxSheet.TabIndex = 1;
         this.cboNewGfxSheet.SelectedIndexChanged += new System.EventHandler(this.cboNewGfxSheet_SelectedIndexChanged);
         // 
         // lblNewGfxSheet
         // 
         this.lblNewGfxSheet.Location = new System.Drawing.Point(8, 8);
         this.lblNewGfxSheet.Name = "lblNewGfxSheet";
         this.lblNewGfxSheet.Size = new System.Drawing.Size(112, 21);
         this.lblNewGfxSheet.TabIndex = 0;
         this.lblNewGfxSheet.Text = "New Graphic Sheet:";
         this.lblNewGfxSheet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // SpecifyOperation
         // 
         this.SpecifyOperation.StepControl = this.pnlSpecifyOperation;
         this.SpecifyOperation.TitleText = "Specify Operation";
         // 
         // pnlTarget
         // 
         this.pnlTarget.Controls.Add(this.lblOtherInfo);
         this.pnlTarget.Controls.Add(this.cboOtherFrameset);
         this.pnlTarget.Controls.Add(this.lblNewFrameset);
         this.pnlTarget.Controls.Add(this.rdoTargetAppend);
         this.pnlTarget.Controls.Add(this.rdoTargetSelf);
         this.pnlTarget.Controls.Add(this.lblTarget);
         this.pnlTarget.Location = new System.Drawing.Point(-10168, 42);
         this.pnlTarget.Name = "pnlTarget";
         this.pnlTarget.Size = new System.Drawing.Size(282, 231);
         this.pnlTarget.TabIndex = 9;
         // 
         // lblNewFrameset
         // 
         this.lblNewFrameset.Enabled = false;
         this.lblNewFrameset.Location = new System.Drawing.Point(32, 112);
         this.lblNewFrameset.Name = "lblNewFrameset";
         this.lblNewFrameset.Size = new System.Drawing.Size(56, 20);
         this.lblNewFrameset.TabIndex = 4;
         this.lblNewFrameset.Text = "Name:";
         this.lblNewFrameset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // rdoTargetAppend
         // 
         this.rdoTargetAppend.Location = new System.Drawing.Point(16, 88);
         this.rdoTargetAppend.Name = "rdoTargetAppend";
         this.rdoTargetAppend.Size = new System.Drawing.Size(248, 16);
         this.rdoTargetAppend.TabIndex = 3;
         this.rdoTargetAppend.Text = "Append copies to a Frameset";
         this.rdoTargetAppend.CheckedChanged += new System.EventHandler(this.Target_CheckedChanged);
         // 
         // rdoTargetSelf
         // 
         this.rdoTargetSelf.Checked = true;
         this.rdoTargetSelf.Location = new System.Drawing.Point(16, 64);
         this.rdoTargetSelf.Name = "rdoTargetSelf";
         this.rdoTargetSelf.Size = new System.Drawing.Size(248, 16);
         this.rdoTargetSelf.TabIndex = 1;
         this.rdoTargetSelf.TabStop = true;
         this.rdoTargetSelf.Text = "Change existing frames";
         this.rdoTargetSelf.CheckedChanged += new System.EventHandler(this.Target_CheckedChanged);
         // 
         // lblTarget
         // 
         this.lblTarget.Location = new System.Drawing.Point(8, 8);
         this.lblTarget.Name = "lblTarget";
         this.lblTarget.Size = new System.Drawing.Size(264, 48);
         this.lblTarget.TabIndex = 0;
         this.lblTarget.Text = "The affected frames can be manipulated in place, copied to a new frameset, or app" +
            "ended to the current frameset.";
         // 
         // Target
         // 
         this.Target.StepControl = this.pnlTarget;
         this.Target.TitleText = "Specify Target";
         this.Target.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Target_ValidateFunction);
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(282, 231);
         this.pnlReview.TabIndex = 10;
         // 
         // txtReview
         // 
         this.txtReview.Location = new System.Drawing.Point(8, 40);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(264, 184);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Location = new System.Drawing.Point(8, 8);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(264, 32);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The following actions will be performed when you click finish:";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // cboOtherFrameset
         // 
         this.cboOtherFrameset.Location = new System.Drawing.Point(88, 112);
         this.cboOtherFrameset.Name = "cboOtherFrameset";
         this.cboOtherFrameset.Size = new System.Drawing.Size(176, 21);
         this.cboOtherFrameset.TabIndex = 6;
         // 
         // lblOtherInfo
         // 
         this.lblOtherInfo.Enabled = false;
         this.lblOtherInfo.Location = new System.Drawing.Point(32, 136);
         this.lblOtherInfo.Name = "lblOtherInfo";
         this.lblOtherInfo.Size = new System.Drawing.Size(232, 32);
         this.lblOtherInfo.TabIndex = 7;
         this.lblOtherInfo.Text = "(Type a new name to create a new Frameset)";
         // 
         // frmFrameRemappingWizard
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlTarget);
         this.Controls.Add(this.pnlSpecifyOperation);
         this.Controls.Add(this.pnlSourceRange);
         this.Controls.Add(this.pnlIntro);
         this.Name = "frmFrameRemappingWizard";
         this.Steps.Add(this.Intro);
         this.Steps.Add(this.SourceRange);
         this.Steps.Add(this.SpecifyOperation);
         this.Steps.Add(this.Target);
         this.Steps.Add(this.Review);
         this.Text = "Frame Remapping Wizard";
         this.Controls.SetChildIndex(this.pnlIntro, 0);
         this.Controls.SetChildIndex(this.pnlSourceRange, 0);
         this.Controls.SetChildIndex(this.pnlSpecifyOperation, 0);
         this.Controls.SetChildIndex(this.pnlTarget, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.pnlIntro.ResumeLayout(false);
         this.pnlSourceRange.ResumeLayout(false);
         this.pnlSpecifyOperation.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudSetCellValue)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudOffset)).EndInit();
         this.pnlTarget.ResumeLayout(false);
         this.pnlReview.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      private bool SourceRange_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (SourceFrames.GetSelectedCellCount() > 0)
            return true;
         MessageBox.Show(this, "Select at least one frame to affect.", "Specify Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private void cboNewGfxSheet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         object selItem = cboNewGfxSheet.Items[cboNewGfxSheet.SelectedIndex];
         if (selItem is ProjectDataset.GraphicSheetRow)
         {
            SetCellList.GraphicSheet = (ProjectDataset.GraphicSheetRow)selItem;
            nudSetCellValue.Maximum = SetCellList.GraphicSheet.Rows * SetCellList.GraphicSheet.Columns - 1;
            SetCellList.Enabled = true;
         }
         else
         {
            SetCellList.GraphicSheet = null;
            SetCellList.Enabled = false;
            nudSetCellValue.Maximum = nudOffset.Maximum;
         }
      }

      private void SetCellList_CurrentCellChanged(object sender, System.EventArgs e)
      {
         int cell = SetCellList.GetFirstSelectedCell();
         if (cell >= 0)
            nudSetCellValue.Value = cell;
      }

      private void Operation_CheckedChanged(object sender, System.EventArgs e)
      {
         if (((RadioButton)sender).Checked)
         {
            if (sender == rdoCellOffset)
            {
               nudOffset.Enabled = lblOffset.Enabled = true;
               nudSetCellValue.Enabled = lblSetCellValue.Enabled = SetCellList.Enabled = false;
            }
            else
            {
               nudOffset.Enabled = lblOffset.Enabled = false;
               nudSetCellValue.Enabled = lblSetCellValue.Enabled = true;
               SetCellList.Enabled = cboNewGfxSheet.Items[cboNewGfxSheet.SelectedIndex] is ProjectDataset.GraphicSheetRow;
            }
         }
      }

      private void Target_CheckedChanged(object sender, System.EventArgs e)
      {
         if (((RadioButton)sender).Checked)
            cboOtherFrameset.Enabled = lblNewFrameset.Enabled = lblOtherInfo.Enabled =
               (sender == rdoTargetAppend);
      }

      private bool Target_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (rdoTargetAppend.Checked)
         {
            string result = ProjectData.ValidateName(cboOtherFrameset.Text);
            if (result == null)
               return true;
            MessageBox.Show(this, result, "Target Frameset Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         return true;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         ProjectDataset.FrameRow[] toAlter = SourceFrames.GetSelectedFrames();
         if (rdoTargetSelf.Checked)
            sb.Append("Change ");
         else
         {
            if (toAlter.Length > 1)
               sb.Append("Make copies of ");
            else
               sb.Append("Make a copy of ");
         }

         bool bExceptions = false;
         if (toAlter.Length > SourceFrames.CellCount / 2)
            bExceptions = true;

         ProjectDataset.FrameRow[] toNotAlter = null;
         if (toAlter.Length < SourceFrames.CellCount)
         {
            ProjectDataset.FrameRow[] useList;
            if (bExceptions)
            {
               toNotAlter = new SGDK2.ProjectDataset.FrameRow[SourceFrames.CellCount - toAlter.Length];
               ProjectDataset.FrameRow[] allFrames = ProjectData.GetSortedFrameRows(SourceFrames.Frameset);
               int notIndex = 0;
               for (int i = 0; i < SourceFrames.CellCount; i++)
               {
                  if(!SourceFrames.Selected[i])
                     toNotAlter[notIndex++] = allFrames[i];
               }
               sb.Append("all frames except ");
               useList = toNotAlter;
            }
            else
            {
               if (toAlter.Length == 1)
                  sb.Append("frame ");
               else
                  sb.Append("frames ");
               useList = toAlter;
            }
            for (int i=0; i<useList.Length; i++)
            {
               if ((i > 0) && (i == useList.Length - 1))
                  sb.Append(" and ");
               else if (i > 0)
                  sb.Append(", ");
               sb.Append(useList[i].FrameValue.ToString());
            }
         }
         else
         {
            sb.Append("all frames");
         }

         bool existingChange = false;
         if (cboNewGfxSheet.Items[cboNewGfxSheet.SelectedIndex] is ProjectDataset.GraphicSheetRow)
         {
            if (rdoTargetSelf.Checked)
               sb.Append(" to use");
            else
            {
               if (toAlter.Length == 1)
                  sb.Append(" that uses");
               else
               {
                  sb.Append(".\r\nSet the copies to use");
               }
            }
            sb.Append(" graphic sheet \"" + ((ProjectDataset.GraphicSheetRow)cboNewGfxSheet.Items[cboNewGfxSheet.SelectedIndex]).Name + "\"");
            existingChange = true;
         }

         if (rdoCellOffset.Checked && nudOffset.Value != 0)
         {
            if (existingChange)
               sb.Append(", and offset");
            else
               sb.Append(", offsetting");

            sb.Append(" the cell index ");
            if (toAlter.Length > 1)
               sb.Append("of each frame ");
            sb.Append("by " + nudOffset.Value.ToString());
         }
         else if (rdoCellSet.Checked)
         {
            if (existingChange)
               sb.Append(", and set");
            else
               sb.Append(", setting");

            sb.Append(" the cell index ");
            if (toAlter.Length > 1)
               sb.Append("of each frame ");
            sb.Append("to " + nudSetCellValue.Value.ToString());
         }
         if (rdoTargetAppend.Checked)
         {
            if (ProjectData.GetFrameSet(cboOtherFrameset.Text) == null)
               sb.Append(".\r\nPut the results in a new frameset named \"" + cboOtherFrameset.Text + "\".");
            else
            {
               if (cboOtherFrameset.Text == SourceFrames.Frameset.Name)
                  sb.Append(".\r\nAppend the results to the same frameset.");
               else
                  sb.Append(".\r\nAppend the results to existing frameset \"" + cboOtherFrameset.Text + "\".");
            }
         }
         else
            sb.Append(".");
         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            ProjectDataset.FrameRow[] allFrames;
            ProjectDataset.FramesetRow drTarget = SourceFrames.Frameset;
            int lastFrameIndex = -1;
            if (rdoTargetAppend.Checked)
            {
               drTarget = ProjectData.GetFrameSet(cboOtherFrameset.Text);
               if (drTarget == null)
                  drTarget = ProjectData.AddFramesetRow(cboOtherFrameset.Text, string.Empty);
               else
               {
                  allFrames = ProjectData.GetSortedFrameRows(drTarget);
                  if (allFrames.Length > 0)
                     lastFrameIndex = allFrames[allFrames.Length-1].FrameValue;
               }
            }
            foreach(ProjectDataset.FrameRow source in SourceFrames.GetSelectedFrames())
            {
               ProjectDataset.GraphicSheetRow drGfx = cboNewGfxSheet.Items[cboNewGfxSheet.SelectedIndex] as ProjectDataset.GraphicSheetRow;
               if (drGfx == null)
                  drGfx = ProjectData.GetGraphicSheet(source.GraphicSheet);
               short cellIdx;
               if (rdoCellOffset.Checked)
               {
                  int cycleOffset = (short)((drGfx.Columns * drGfx.Rows) * Math.Floor((double)(source.CellIndex + nudOffset.Value)/(double)(drGfx.Columns * drGfx.Rows)));
                  cellIdx = (short)(source.CellIndex + nudOffset.Value - cycleOffset);
               }
               else
                  cellIdx = (short)nudSetCellValue.Value;
               if(rdoTargetSelf.Checked)
               {
                  source.GraphicSheet = drGfx.Name;
                  source.CellIndex = cellIdx;
               }
               else
               {
                  ProjectData.AddFrameRow(drTarget, ++lastFrameIndex, drGfx.Name, cellIdx,
                     source.m11, source.m12, source.m21, source.m22, source.dx, source.dy, source.color);
               }
            }
            return true;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Frame Remapping Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }
	}
}

