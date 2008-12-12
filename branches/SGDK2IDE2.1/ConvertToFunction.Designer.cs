namespace SGDK2
{
   partial class frmConvertToFunction
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
         this.pnlOutputOption = new System.Windows.Forms.Panel();
         this.rdoOutputExistingFile = new System.Windows.Forms.RadioButton();
         this.rdoOutputNewFile = new System.Windows.Forms.RadioButton();
         this.rdoOutputClipboard = new System.Windows.Forms.RadioButton();
         this.lblOutputOptionInfo = new System.Windows.Forms.Label();
         this.stepOutputOption = new SGDK2.frmWizardBase.StepInfo();
         this.pnlTargetName = new System.Windows.Forms.Panel();
         this.cboCodeName = new System.Windows.Forms.ComboBox();
         this.txtFunctionName = new System.Windows.Forms.TextBox();
         this.lblFunctionName = new System.Windows.Forms.Label();
         this.txtCodeName = new System.Windows.Forms.TextBox();
         this.lblCodeName = new System.Windows.Forms.Label();
         this.stepTargetName = new SGDK2.frmWizardBase.StepInfo();
         this.pnlRuleOptions = new System.Windows.Forms.Panel();
         this.txtNewRuleName = new System.Windows.Forms.TextBox();
         this.lblNewRuleName = new System.Windows.Forms.Label();
         this.chkAddCall = new System.Windows.Forms.CheckBox();
         this.chkDeleteOld = new System.Windows.Forms.CheckBox();
         this.stepRuleOptions = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFinish = new System.Windows.Forms.Panel();
         this.txtFinish = new System.Windows.Forms.TextBox();
         this.stepFinish = new SGDK2.frmWizardBase.StepInfo();
         this.pnlOutputOption.SuspendLayout();
         this.pnlTargetName.SuspendLayout();
         this.pnlRuleOptions.SuspendLayout();
         this.pnlFinish.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlOutputOption
         // 
         this.pnlOutputOption.Controls.Add(this.rdoOutputExistingFile);
         this.pnlOutputOption.Controls.Add(this.rdoOutputNewFile);
         this.pnlOutputOption.Controls.Add(this.rdoOutputClipboard);
         this.pnlOutputOption.Controls.Add(this.lblOutputOptionInfo);
         this.pnlOutputOption.Location = new System.Drawing.Point(168, 42);
         this.pnlOutputOption.Name = "pnlOutputOption";
         this.pnlOutputOption.Size = new System.Drawing.Size(280, 231);
         this.pnlOutputOption.TabIndex = 6;
         // 
         // rdoOutputExistingFile
         // 
         this.rdoOutputExistingFile.Location = new System.Drawing.Point(21, 160);
         this.rdoOutputExistingFile.Name = "rdoOutputExistingFile";
         this.rdoOutputExistingFile.Size = new System.Drawing.Size(249, 41);
         this.rdoOutputExistingFile.TabIndex = 3;
         this.rdoOutputExistingFile.TabStop = true;
         this.rdoOutputExistingFile.Text = "Existing Source Code Object - The code will be appended to an existiyng Source Co" +
             "de object.";
         this.rdoOutputExistingFile.UseVisualStyleBackColor = true;
         // 
         // rdoOutputNewFile
         // 
         this.rdoOutputNewFile.Location = new System.Drawing.Point(21, 105);
         this.rdoOutputNewFile.Name = "rdoOutputNewFile";
         this.rdoOutputNewFile.Size = new System.Drawing.Size(249, 49);
         this.rdoOutputNewFile.TabIndex = 2;
         this.rdoOutputNewFile.TabStop = true;
         this.rdoOutputNewFile.Text = "New Source Code Object - A new Source Code object will be added to the project to" +
             " contain the code.";
         this.rdoOutputNewFile.UseVisualStyleBackColor = true;
         // 
         // rdoOutputClipboard
         // 
         this.rdoOutputClipboard.Location = new System.Drawing.Point(21, 65);
         this.rdoOutputClipboard.Name = "rdoOutputClipboard";
         this.rdoOutputClipboard.Size = new System.Drawing.Size(250, 34);
         this.rdoOutputClipboard.TabIndex = 1;
         this.rdoOutputClipboard.TabStop = true;
         this.rdoOutputClipboard.Text = "Clipboard - You can paste the code wherever you want to put it.";
         this.rdoOutputClipboard.UseVisualStyleBackColor = true;
         // 
         // lblOutputOptionInfo
         // 
         this.lblOutputOptionInfo.Location = new System.Drawing.Point(3, 10);
         this.lblOutputOptionInfo.Name = "lblOutputOptionInfo";
         this.lblOutputOptionInfo.Size = new System.Drawing.Size(269, 44);
         this.lblOutputOptionInfo.TabIndex = 0;
         this.lblOutputOptionInfo.Text = "The specified rules will be converted to code.  Where would you like to put the r" +
             "esulting code?";
         // 
         // stepOutputOption
         // 
         this.stepOutputOption.StepControl = this.pnlOutputOption;
         this.stepOutputOption.TitleText = "Specify Output";
         this.stepOutputOption.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepOutputOption_ValidateFunction);
         // 
         // pnlTargetName
         // 
         this.pnlTargetName.Controls.Add(this.cboCodeName);
         this.pnlTargetName.Controls.Add(this.txtFunctionName);
         this.pnlTargetName.Controls.Add(this.lblFunctionName);
         this.pnlTargetName.Controls.Add(this.txtCodeName);
         this.pnlTargetName.Controls.Add(this.lblCodeName);
         this.pnlTargetName.Location = new System.Drawing.Point(-10168, 42);
         this.pnlTargetName.Name = "pnlTargetName";
         this.pnlTargetName.Size = new System.Drawing.Size(280, 231);
         this.pnlTargetName.TabIndex = 7;
         // 
         // cboCodeName
         // 
         this.cboCodeName.DisplayMember = "Name";
         this.cboCodeName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboCodeName.FormattingEnabled = true;
         this.cboCodeName.Location = new System.Drawing.Point(106, 24);
         this.cboCodeName.Name = "cboCodeName";
         this.cboCodeName.Size = new System.Drawing.Size(166, 21);
         this.cboCodeName.Sorted = true;
         this.cboCodeName.TabIndex = 2;
         this.cboCodeName.Visible = false;
         // 
         // txtFunctionName
         // 
         this.txtFunctionName.Location = new System.Drawing.Point(106, 51);
         this.txtFunctionName.Name = "txtFunctionName";
         this.txtFunctionName.Size = new System.Drawing.Size(166, 20);
         this.txtFunctionName.TabIndex = 4;
         // 
         // lblFunctionName
         // 
         this.lblFunctionName.AutoSize = true;
         this.lblFunctionName.Location = new System.Drawing.Point(3, 54);
         this.lblFunctionName.Name = "lblFunctionName";
         this.lblFunctionName.Size = new System.Drawing.Size(82, 13);
         this.lblFunctionName.TabIndex = 3;
         this.lblFunctionName.Text = "Function Name:";
         // 
         // txtCodeName
         // 
         this.txtCodeName.Location = new System.Drawing.Point(106, 25);
         this.txtCodeName.Name = "txtCodeName";
         this.txtCodeName.Size = new System.Drawing.Size(166, 20);
         this.txtCodeName.TabIndex = 1;
         this.txtCodeName.Visible = false;
         // 
         // lblCodeName
         // 
         this.lblCodeName.AutoSize = true;
         this.lblCodeName.Location = new System.Drawing.Point(3, 28);
         this.lblCodeName.Name = "lblCodeName";
         this.lblCodeName.Size = new System.Drawing.Size(100, 13);
         this.lblCodeName.TabIndex = 0;
         this.lblCodeName.Text = "Code Object Name:";
         // 
         // stepTargetName
         // 
         this.stepTargetName.StepControl = this.pnlTargetName;
         this.stepTargetName.TitleText = "Name Target";
         this.stepTargetName.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepTargetName_IsApplicableFunction);
         this.stepTargetName.InitFunction += new System.EventHandler(this.stepTargetName_InitFunction);
         this.stepTargetName.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepTargetName_ValidateFunction);
         // 
         // pnlRuleOptions
         // 
         this.pnlRuleOptions.Controls.Add(this.txtNewRuleName);
         this.pnlRuleOptions.Controls.Add(this.lblNewRuleName);
         this.pnlRuleOptions.Controls.Add(this.chkAddCall);
         this.pnlRuleOptions.Controls.Add(this.chkDeleteOld);
         this.pnlRuleOptions.Location = new System.Drawing.Point(-10168, 42);
         this.pnlRuleOptions.Name = "pnlRuleOptions";
         this.pnlRuleOptions.Size = new System.Drawing.Size(280, 231);
         this.pnlRuleOptions.TabIndex = 8;
         // 
         // txtNewRuleName
         // 
         this.txtNewRuleName.Enabled = false;
         this.txtNewRuleName.Location = new System.Drawing.Point(100, 72);
         this.txtNewRuleName.Name = "txtNewRuleName";
         this.txtNewRuleName.Size = new System.Drawing.Size(170, 20);
         this.txtNewRuleName.TabIndex = 3;
         // 
         // lblNewRuleName
         // 
         this.lblNewRuleName.AutoSize = true;
         this.lblNewRuleName.Enabled = false;
         this.lblNewRuleName.Location = new System.Drawing.Point(31, 75);
         this.lblNewRuleName.Name = "lblNewRuleName";
         this.lblNewRuleName.Size = new System.Drawing.Size(63, 13);
         this.lblNewRuleName.TabIndex = 2;
         this.lblNewRuleName.Text = "Rule Name:";
         // 
         // chkAddCall
         // 
         this.chkAddCall.AutoSize = true;
         this.chkAddCall.Location = new System.Drawing.Point(6, 50);
         this.chkAddCall.Name = "chkAddCall";
         this.chkAddCall.Size = new System.Drawing.Size(230, 17);
         this.chkAddCall.TabIndex = 1;
         this.chkAddCall.Text = "Create a rule that calls the converted code.";
         this.chkAddCall.UseVisualStyleBackColor = true;
         this.chkAddCall.CheckedChanged += new System.EventHandler(this.chkAddCall_CheckedChanged);
         // 
         // chkDeleteOld
         // 
         this.chkDeleteOld.AutoSize = true;
         this.chkDeleteOld.Location = new System.Drawing.Point(6, 27);
         this.chkDeleteOld.Name = "chkDeleteOld";
         this.chkDeleteOld.Size = new System.Drawing.Size(154, 17);
         this.chkDeleteOld.TabIndex = 0;
         this.chkDeleteOld.Text = "Delete the converted rules.";
         this.chkDeleteOld.UseVisualStyleBackColor = true;
         // 
         // stepRuleOptions
         // 
         this.stepRuleOptions.StepControl = this.pnlRuleOptions;
         this.stepRuleOptions.TitleText = "Rule Options";
         this.stepRuleOptions.InitFunction += new System.EventHandler(this.stepRuleOptions_InitFunction);
         // 
         // pnlFinish
         // 
         this.pnlFinish.Controls.Add(this.txtFinish);
         this.pnlFinish.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFinish.Name = "pnlFinish";
         this.pnlFinish.Size = new System.Drawing.Size(280, 231);
         this.pnlFinish.TabIndex = 9;
         // 
         // txtFinish
         // 
         this.txtFinish.Location = new System.Drawing.Point(3, 3);
         this.txtFinish.Multiline = true;
         this.txtFinish.Name = "txtFinish";
         this.txtFinish.ReadOnly = true;
         this.txtFinish.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txtFinish.Size = new System.Drawing.Size(274, 225);
         this.txtFinish.TabIndex = 0;
         // 
         // stepFinish
         // 
         this.stepFinish.StepControl = this.pnlFinish;
         this.stepFinish.TitleText = "Ready to Convert";
         this.stepFinish.InitFunction += new System.EventHandler(this.stepFinish_InitFunction);
         this.stepFinish.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepFinish_ValidateFunction);
         // 
         // frmConvertToFunction
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlFinish);
         this.Controls.Add(this.pnlRuleOptions);
         this.Controls.Add(this.pnlTargetName);
         this.Controls.Add(this.pnlOutputOption);
         this.Name = "frmConvertToFunction";
         this.Steps.Add(this.stepOutputOption);
         this.Steps.Add(this.stepTargetName);
         this.Steps.Add(this.stepRuleOptions);
         this.Steps.Add(this.stepFinish);
         this.Text = "Convert Rules to Source Code";
         this.Controls.SetChildIndex(this.pnlOutputOption, 0);
         this.Controls.SetChildIndex(this.pnlTargetName, 0);
         this.Controls.SetChildIndex(this.pnlRuleOptions, 0);
         this.Controls.SetChildIndex(this.pnlFinish, 0);
         this.pnlOutputOption.ResumeLayout(false);
         this.pnlTargetName.ResumeLayout(false);
         this.pnlTargetName.PerformLayout();
         this.pnlRuleOptions.ResumeLayout(false);
         this.pnlRuleOptions.PerformLayout();
         this.pnlFinish.ResumeLayout(false);
         this.pnlFinish.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Panel pnlOutputOption;
      private System.Windows.Forms.Label lblOutputOptionInfo;
      private frmWizardBase.StepInfo stepOutputOption;
      private System.Windows.Forms.RadioButton rdoOutputNewFile;
      private System.Windows.Forms.RadioButton rdoOutputClipboard;
      private System.Windows.Forms.RadioButton rdoOutputExistingFile;
      private System.Windows.Forms.Panel pnlTargetName;
      private System.Windows.Forms.Label lblCodeName;
      private frmWizardBase.StepInfo stepTargetName;
      private System.Windows.Forms.ComboBox cboCodeName;
      private System.Windows.Forms.TextBox txtFunctionName;
      private System.Windows.Forms.Label lblFunctionName;
      private System.Windows.Forms.TextBox txtCodeName;
      private System.Windows.Forms.Panel pnlRuleOptions;
      private System.Windows.Forms.CheckBox chkDeleteOld;
      private frmWizardBase.StepInfo stepRuleOptions;
      private System.Windows.Forms.CheckBox chkAddCall;
      private System.Windows.Forms.Panel pnlFinish;
      private System.Windows.Forms.TextBox txtFinish;
      private frmWizardBase.StepInfo stepFinish;
      private System.Windows.Forms.TextBox txtNewRuleName;
      private System.Windows.Forms.Label lblNewRuleName;
   }
}