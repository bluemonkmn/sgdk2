using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;

namespace SGDK2
{
	/// <summary>
	/// Summary description for WizardBase.
	/// </summary>

   public class frmWizardBase : System.Windows.Forms.Form
   {
      public delegate bool ValidateFunctionEvent(StepInfo sender);

      #region Embedded classes
      public class StepInfo : Component
      {
         private Control m_StepControl = null;
         public event System.EventHandler InitFunction;
         public event ValidateFunctionEvent ValidateFunction;
         public string m_TitleText = String.Empty;
         
         public void Init()
         {
            if (InitFunction != null)
               InitFunction(this, null);
            this.StepControl.Visible = true;
            if (StepControl.Left < -2000)
               StepControl.Left = -10000 - StepControl.Left;
         }
         public bool Validate()
         {
            if (ValidateFunction != null)
            {
               if (ValidateFunction(this))
               {
                  StepControl.Visible = false;
                  return true;
               }
               else
                  return false;
            }
            return true;
         }
         public Control StepControl
         {
            get
            {
               return m_StepControl;
            }
            set
            {
               m_StepControl = value;
               if ((value != null) && DesignMode && (value.Parent != null))
               {
                  value.Location = new Point(168,42);
                  value.Size = new Size(m_StepControl.Parent.Width - 176, m_StepControl.Parent.ClientSize.Height - 82);
               }
            }
         }
         public string TitleText
         {
            get
            {
               return m_TitleText;
            }
            set
            {
               m_TitleText = value;
            }
         }
      }

      public class StepCollection : CollectionBase
      {
         public StepInfo this[int index]
         {
            get
            {
               return (StepInfo)List[index];
            }
            set
            {
               List[index] = value;
            }
         }

         public int Add(StepInfo value)
         {
            return List.Add(value);
         }

         public int IndexOf(StepInfo value)
         {
            return List.IndexOf(value);
         }

         public void Insert(int index, StepInfo value )  
         {
            List.Insert(index, value);
         }

         public void Remove(StepInfo value)
         {
            List.Remove(value);
         }

         public bool Contains(StepInfo value)
         {
            // If value is not of type Int16, this will return false.
            return List.Contains(value);
         }

         protected override void OnInsert(int index, Object value)
         {
            if (value.GetType() != typeof(StepInfo))
               throw new ArgumentException( "value must be of type StepInfo.", "value" );
         }

         protected override void OnRemove(int index, Object value)
         {
            if (value.GetType() != typeof(StepInfo))
               throw new ArgumentException( "value must be of type StepInfo.", "value" );
         }

         protected override void OnSet(int index, Object oldValue, Object newValue)
         {
            if (newValue.GetType() != typeof(StepInfo))
               throw new ArgumentException( "newValue must be of type StepInfo.", "newValue" );
         }

         protected override void OnValidate(Object value)
         {
            if (value.GetType() != typeof(StepInfo))
               throw new ArgumentException( "value must be of type StepInfo." );
         }
      }
      #endregion

      #region Non-Control members
      private StepCollection m_Steps = null;
      private int m_nCurrentStepIndex = 0;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Button btnBack;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.PictureBox picSideImage;
      private System.Windows.Forms.Label lblHeading;
      private System.Windows.Forms.Label lblStepNum;

      /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      #endregion

      #region Initialization and Clean-up
		public frmWizardBase()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         if (DesignMode)
            foreach (StepInfo si in Steps)
               si.StepControl.Visible = false;
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
      #endregion

      #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmWizardBase));
         this.btnBack = new System.Windows.Forms.Button();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.picSideImage = new System.Windows.Forms.PictureBox();
         this.lblHeading = new System.Windows.Forms.Label();
         this.lblStepNum = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // btnBack
         // 
         this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnBack.Location = new System.Drawing.Point(208, 282);
         this.btnBack.Name = "btnBack";
         this.btnBack.Size = new System.Drawing.Size(72, 24);
         this.btnBack.TabIndex = 0;
         this.btnBack.Text = "< &Back";
         this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
         // 
         // btnNext
         // 
         this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnNext.Location = new System.Drawing.Point(280, 282);
         this.btnNext.Name = "btnNext";
         this.btnNext.Size = new System.Drawing.Size(72, 24);
         this.btnNext.TabIndex = 1;
         this.btnNext.Text = "&Next >";
         this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(368, 282);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "&Cancel";
         // 
         // picSideImage
         // 
         this.picSideImage.Image = ((System.Drawing.Image)(resources.GetObject("picSideImage.Image")));
         this.picSideImage.Location = new System.Drawing.Point(0, 0);
         this.picSideImage.Name = "picSideImage";
         this.picSideImage.Size = new System.Drawing.Size(164, 313);
         this.picSideImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this.picSideImage.TabIndex = 3;
         this.picSideImage.TabStop = false;
         // 
         // lblHeading
         // 
         this.lblHeading.BackColor = System.Drawing.SystemColors.Window;
         this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblHeading.ForeColor = System.Drawing.SystemColors.ControlText;
         this.lblHeading.Location = new System.Drawing.Point(216, 0);
         this.lblHeading.Name = "lblHeading";
         this.lblHeading.Size = new System.Drawing.Size(232, 40);
         this.lblHeading.TabIndex = 4;
         this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblStepNum
         // 
         this.lblStepNum.BackColor = System.Drawing.SystemColors.Window;
         this.lblStepNum.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblStepNum.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(64)), ((System.Byte)(64)));
         this.lblStepNum.Image = ((System.Drawing.Image)(resources.GetObject("lblStepNum.Image")));
         this.lblStepNum.Location = new System.Drawing.Point(164, 0);
         this.lblStepNum.Name = "lblStepNum";
         this.lblStepNum.Size = new System.Drawing.Size(52, 40);
         this.lblStepNum.TabIndex = 5;
         this.lblStepNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // frmWizardBase
         // 
         this.AcceptButton = this.btnNext;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.lblStepNum);
         this.Controls.Add(this.lblHeading);
         this.Controls.Add(this.picSideImage);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnNext);
         this.Controls.Add(this.btnBack);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmWizardBase";
         this.Text = "WizardBase";
         this.ResumeLayout(false);

      }
		#endregion
	
      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad (e);

         if ((m_Steps != null) && (m_Steps.Count > 0))
         {
            m_Steps[m_nCurrentStepIndex].Init();
            InitHeading();
         }
      }
      #endregion

      #region Event Handlers
      private void btnNext_Click(object sender, System.EventArgs e)
      {
         if (m_Steps[m_nCurrentStepIndex].Validate())
         {
            if (m_nCurrentStepIndex < m_Steps.Count-1)
            {
               m_Steps[m_nCurrentStepIndex].StepControl.Visible = false;
               m_Steps[++m_nCurrentStepIndex].Init();
               if (m_nCurrentStepIndex == m_Steps.Count-1)
                  btnNext.Text = "&Finish";
               InitHeading();
            }
            else
               this.Close();
         }
      }

      private void btnBack_Click(object sender, System.EventArgs e)
      {
         if (m_nCurrentStepIndex > 0)
         {
            m_Steps[m_nCurrentStepIndex].StepControl.Visible = false;
            m_Steps[--m_nCurrentStepIndex].Init();
            if (m_nCurrentStepIndex < m_Steps.Count-1)
               btnNext.Text = "&Next >";
            InitHeading();
         }
      }
      #endregion

      #region Private Members
      private void InitHeading()
      {
         lblHeading.Text = m_Steps[m_nCurrentStepIndex].TitleText;
         lblStepNum.Text = (m_nCurrentStepIndex + 1).ToString();
      }
      #endregion

      #region Design time properties
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      public StepCollection Steps
      {
         get
         {
            if (m_Steps == null)
               m_Steps = new StepCollection();
            return m_Steps;
         }
         set
         {
            m_Steps = value;
         }
      }

      [DesignOnly(true)]
      public StepInfo CurrentStep
      {
         get
         {
            foreach (StepInfo si in m_Steps)
               if (si.StepControl.Left >= -2000)
                  return si;
            return null;
         }
         set
         {
            for (int idx = 0; idx < m_Steps.Count; idx++)
            {
               if ((value != m_Steps[idx]) ^ (m_Steps[idx].StepControl.Left < -2000))
                  m_Steps[idx].StepControl.Left = -10000 - m_Steps[idx].StepControl.Left;
               if (value == m_Steps[idx])
               {
                  m_nCurrentStepIndex = idx;
                  InitHeading();
               }
            }
         }
      }
      #endregion   
   }
}
