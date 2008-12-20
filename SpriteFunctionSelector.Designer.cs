namespace SGDK2
{
   partial class frmSpriteFunctionSelector
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
         this.stepSelectSprite = new SGDK2.frmWizardBase.StepInfo();
         this.lvwSprites = new System.Windows.Forms.ListView();
         this.colSpriteName = new System.Windows.Forms.ColumnHeader();
         this.colSpriteType = new System.Windows.Forms.ColumnHeader();
         this.stepSelectFunction = new SGDK2.frmWizardBase.StepInfo();
         this.lvwFunctions = new System.Windows.Forms.ListView();
         this.colFunction = new System.Windows.Forms.ColumnHeader();
         this.pnlParameters = new System.Windows.Forms.Panel();
         this.cboParam3 = new System.Windows.Forms.ComboBox();
         this.lblParam3 = new System.Windows.Forms.Label();
         this.cboParam2 = new System.Windows.Forms.ComboBox();
         this.lblParam2 = new System.Windows.Forms.Label();
         this.cboParam1 = new System.Windows.Forms.ComboBox();
         this.lblParam1 = new System.Windows.Forms.Label();
         this.stepParameters = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFinish = new System.Windows.Forms.Panel();
         this.lblFinish = new System.Windows.Forms.Label();
         this.txtFinish = new System.Windows.Forms.TextBox();
         this.stepFinish = new SGDK2.frmWizardBase.StepInfo();
         this.pnlParameters.SuspendLayout();
         this.pnlFinish.SuspendLayout();
         this.SuspendLayout();
         // 
         // stepSelectSprite
         // 
         this.stepSelectSprite.StepControl = this.lvwSprites;
         this.stepSelectSprite.TitleText = "Select Sprite";
         this.stepSelectSprite.InitFunction += new System.EventHandler(this.stepSelectSprite_InitFunction);
         this.stepSelectSprite.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepSelectSprite_ValidateFunction);
         // 
         // lvwSprites
         // 
         this.lvwSprites.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSpriteName,
            this.colSpriteType});
         this.lvwSprites.FullRowSelect = true;
         this.lvwSprites.HideSelection = false;
         this.lvwSprites.Location = new System.Drawing.Point(168, 42);
         this.lvwSprites.MultiSelect = false;
         this.lvwSprites.Name = "lvwSprites";
         this.lvwSprites.Size = new System.Drawing.Size(280, 231);
         this.lvwSprites.TabIndex = 0;
         this.lvwSprites.UseCompatibleStateImageBehavior = false;
         this.lvwSprites.View = System.Windows.Forms.View.Details;
         // 
         // colSpriteName
         // 
         this.colSpriteName.Text = "Sprite Name";
         this.colSpriteName.Width = 131;
         // 
         // colSpriteType
         // 
         this.colSpriteType.Text = "Sprite Type";
         this.colSpriteType.Width = 123;
         // 
         // stepSelectFunction
         // 
         this.stepSelectFunction.StepControl = this.lvwFunctions;
         this.stepSelectFunction.TitleText = "Select Function";
         this.stepSelectFunction.InitFunction += new System.EventHandler(this.stepSelectFunction_InitFunction);
         this.stepSelectFunction.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepSelectFunction_ValidateFunction);
         // 
         // lvwFunctions
         // 
         this.lvwFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFunction});
         this.lvwFunctions.FullRowSelect = true;
         this.lvwFunctions.HideSelection = false;
         this.lvwFunctions.Location = new System.Drawing.Point(-10168, 42);
         this.lvwFunctions.MultiSelect = false;
         this.lvwFunctions.Name = "lvwFunctions";
         this.lvwFunctions.Size = new System.Drawing.Size(280, 231);
         this.lvwFunctions.TabIndex = 0;
         this.lvwFunctions.UseCompatibleStateImageBehavior = false;
         this.lvwFunctions.View = System.Windows.Forms.View.Details;
         // 
         // colFunction
         // 
         this.colFunction.Text = "Function Name";
         this.colFunction.Width = 242;
         // 
         // pnlParameters
         // 
         this.pnlParameters.Controls.Add(this.cboParam3);
         this.pnlParameters.Controls.Add(this.lblParam3);
         this.pnlParameters.Controls.Add(this.cboParam2);
         this.pnlParameters.Controls.Add(this.lblParam2);
         this.pnlParameters.Controls.Add(this.cboParam1);
         this.pnlParameters.Controls.Add(this.lblParam1);
         this.pnlParameters.Location = new System.Drawing.Point(-10168, 42);
         this.pnlParameters.Name = "pnlParameters";
         this.pnlParameters.Size = new System.Drawing.Size(280, 231);
         this.pnlParameters.TabIndex = 6;
         // 
         // cboParam3
         // 
         this.cboParam3.FormattingEnabled = true;
         this.cboParam3.Location = new System.Drawing.Point(6, 114);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(266, 21);
         this.cboParam3.TabIndex = 5;
         this.cboParam3.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam3.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         // 
         // lblParam3
         // 
         this.lblParam3.Location = new System.Drawing.Point(3, 95);
         this.lblParam3.Name = "lblParam3";
         this.lblParam3.Size = new System.Drawing.Size(269, 16);
         this.lblParam3.TabIndex = 4;
         this.lblParam3.Text = "Parameter 3:";
         // 
         // cboParam2
         // 
         this.cboParam2.FormattingEnabled = true;
         this.cboParam2.Location = new System.Drawing.Point(6, 71);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(266, 21);
         this.cboParam2.TabIndex = 3;
         this.cboParam2.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam2.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         // 
         // lblParam2
         // 
         this.lblParam2.Location = new System.Drawing.Point(3, 52);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(267, 16);
         this.lblParam2.TabIndex = 2;
         this.lblParam2.Text = "Parameter 2:";
         // 
         // cboParam1
         // 
         this.cboParam1.FormattingEnabled = true;
         this.cboParam1.Location = new System.Drawing.Point(6, 28);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(266, 21);
         this.cboParam1.TabIndex = 1;
         this.cboParam1.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam1.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         // 
         // lblParam1
         // 
         this.lblParam1.Location = new System.Drawing.Point(3, 9);
         this.lblParam1.Name = "lblParam1";
         this.lblParam1.Size = new System.Drawing.Size(269, 16);
         this.lblParam1.TabIndex = 0;
         this.lblParam1.Text = "Parameter 1:";
         // 
         // stepParameters
         // 
         this.stepParameters.StepControl = this.pnlParameters;
         this.stepParameters.TitleText = "Function Parameters";
         this.stepParameters.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepParameters_IsApplicableFunction);
         this.stepParameters.InitFunction += new System.EventHandler(this.stepParameters_InitFunction);
         // 
         // pnlFinish
         // 
         this.pnlFinish.Controls.Add(this.lblFinish);
         this.pnlFinish.Controls.Add(this.txtFinish);
         this.pnlFinish.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFinish.Name = "pnlFinish";
         this.pnlFinish.Size = new System.Drawing.Size(280, 231);
         this.pnlFinish.TabIndex = 7;
         // 
         // lblFinish
         // 
         this.lblFinish.AutoSize = true;
         this.lblFinish.Location = new System.Drawing.Point(3, 7);
         this.lblFinish.Name = "lblFinish";
         this.lblFinish.Size = new System.Drawing.Size(186, 13);
         this.lblFinish.TabIndex = 1;
         this.lblFinish.Text = "The rule will be updated as described:";
         // 
         // txtFinish
         // 
         this.txtFinish.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.txtFinish.Location = new System.Drawing.Point(0, 25);
         this.txtFinish.Multiline = true;
         this.txtFinish.Name = "txtFinish";
         this.txtFinish.ReadOnly = true;
         this.txtFinish.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtFinish.Size = new System.Drawing.Size(280, 206);
         this.txtFinish.TabIndex = 0;
         // 
         // stepFinish
         // 
         this.stepFinish.StepControl = this.pnlFinish;
         this.stepFinish.TitleText = "Finish";
         this.stepFinish.InitFunction += new System.EventHandler(this.stepFinish_InitFunction);
         this.stepFinish.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.stepFinish_ValidateFunction);
         // 
         // frmSpriteFunctionSelector
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlFinish);
         this.Controls.Add(this.pnlParameters);
         this.Controls.Add(this.lvwFunctions);
         this.Controls.Add(this.lvwSprites);
         this.Name = "frmSpriteFunctionSelector";
         this.Steps.Add(this.stepSelectSprite);
         this.Steps.Add(this.stepSelectFunction);
         this.Steps.Add(this.stepParameters);
         this.Steps.Add(this.stepFinish);
         this.Text = "Select Sprite Function";
         this.Controls.SetChildIndex(this.lvwSprites, 0);
         this.Controls.SetChildIndex(this.lvwFunctions, 0);
         this.Controls.SetChildIndex(this.pnlParameters, 0);
         this.Controls.SetChildIndex(this.pnlFinish, 0);
         this.pnlParameters.ResumeLayout(false);
         this.pnlFinish.ResumeLayout(false);
         this.pnlFinish.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ListView lvwSprites;
      private System.Windows.Forms.ColumnHeader colSpriteName;
      private System.Windows.Forms.ColumnHeader colSpriteType;
      private frmWizardBase.StepInfo stepSelectSprite;
      private frmWizardBase.StepInfo stepSelectFunction;
      private System.Windows.Forms.ListView lvwFunctions;
      private System.Windows.Forms.ColumnHeader colFunction;
      private System.Windows.Forms.Panel pnlParameters;
      private System.Windows.Forms.ComboBox cboParam3;
      private System.Windows.Forms.Label lblParam3;
      private System.Windows.Forms.ComboBox cboParam2;
      private System.Windows.Forms.Label lblParam2;
      private System.Windows.Forms.ComboBox cboParam1;
      private System.Windows.Forms.Label lblParam1;
      private frmWizardBase.StepInfo stepParameters;
      private System.Windows.Forms.Panel pnlFinish;
      private frmWizardBase.StepInfo stepFinish;
      private System.Windows.Forms.Label lblFinish;
      private System.Windows.Forms.TextBox txtFinish;
   }
}