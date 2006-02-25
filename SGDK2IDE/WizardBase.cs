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
                  value.Location = new Point(0,32);
                  value.Size = new Size(m_StepControl.Parent.Width - 8, m_StepControl.Parent.ClientSize.Height - 72);
               }
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
         m_Steps[m_nCurrentStepIndex].Init();
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
         this.btnBack = new System.Windows.Forms.Button();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnBack
         // 
         this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnBack.Location = new System.Drawing.Point(208, 272);
         this.btnBack.Name = "btnBack";
         this.btnBack.Size = new System.Drawing.Size(72, 24);
         this.btnBack.TabIndex = 0;
         this.btnBack.Text = "< &Back";
         this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
         // 
         // btnNext
         // 
         this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnNext.Location = new System.Drawing.Point(280, 272);
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
         this.btnCancel.Location = new System.Drawing.Point(368, 272);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "&Cancel";
         // 
         // frmWizardBase
         // 
         this.AcceptButton = this.btnNext;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(450, 303);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnNext);
         this.Controls.Add(this.btnBack);
         this.Name = "frmWizardBase";
         this.Text = "WizardBase";
         this.ResumeLayout(false);

      }
		#endregion
	
      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad (e);
      }
      #endregion

      #region Event Handlers
      private void btnNext_Click(object sender, System.EventArgs e)
      {
         if (m_Steps[m_nCurrentStepIndex].Validate())
         {
            m_Steps[++m_nCurrentStepIndex].Init();
         }
      }

      private void btnBack_Click(object sender, System.EventArgs e)
      {
         m_Steps[m_nCurrentStepIndex].StepControl.Visible = false;
         m_Steps[--m_nCurrentStepIndex].Init();
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
            foreach(StepInfo si in m_Steps)
               if ((!(value == si)) ^ (si.StepControl.Left < -2000))
                  si.StepControl.Left = -10000 - si.StepControl.Left;
         }
      }
      #endregion   
   }
}
