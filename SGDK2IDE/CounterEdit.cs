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
	public class frmCounterEdit : System.Windows.Forms.Form
	{
      #region
      private ProjectDataset.CounterRow m_Counter = null;
      #endregion

      #region Form Designer members

      private System.Windows.Forms.Label lblMaximum;
      private System.Windows.Forms.NumericUpDown nudValue;
      private System.Windows.Forms.Label lblValue;
      private System.Windows.Forms.TextBox txtCounterName;
      private System.Windows.Forms.Label lblCounterName;
      private System.Windows.Forms.NumericUpDown nudMaximum;
      private SGDK2.DataChangeNotifier dataMonitor;
		private System.ComponentModel.IContainer components = null;
      #endregion

		public frmCounterEdit()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Counter " + (nIdx++).ToString();
         while (ProjectData.GetCounter(sName) != null);

         m_Counter = ProjectData.AddCounter(sName, 1, 100);
         nudMaximum.Value = m_Counter.Max;
         nudValue.Value = m_Counter.Value;
         txtCounterName.Text = sName;

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"EditCounters.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmCounterEdit(ProjectDataset.CounterRow drCounter)
      {
         // This call is required by the Windows Form Designer.
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_Counter = drCounter;
         txtCounterName.Text = drCounter.Name;
         nudMaximum.Value = drCounter.Max;
         nudValue.Value = drCounter.Value;

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"EditCounters.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
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
         this.components = new System.ComponentModel.Container();
         this.nudMaximum = new System.Windows.Forms.NumericUpDown();
         this.lblMaximum = new System.Windows.Forms.Label();
         this.nudValue = new System.Windows.Forms.NumericUpDown();
         this.lblValue = new System.Windows.Forms.Label();
         this.txtCounterName = new System.Windows.Forms.TextBox();
         this.lblCounterName = new System.Windows.Forms.Label();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.nudMaximum)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
         this.SuspendLayout();
         // 
         // nudMaximum
         // 
         this.nudMaximum.Location = new System.Drawing.Point(72, 32);
         this.nudMaximum.Maximum = new System.Decimal(new int[] {
                                                                   2000000000,
                                                                   0,
                                                                   0,
                                                                   0});
         this.nudMaximum.Minimum = new System.Decimal(new int[] {
                                                                   1,
                                                                   0,
                                                                   0,
                                                                   0});
         this.nudMaximum.Name = "nudMaximum";
         this.nudMaximum.Size = new System.Drawing.Size(88, 20);
         this.nudMaximum.TabIndex = 4;
         this.nudMaximum.Value = new System.Decimal(new int[] {
                                                                 1,
                                                                 0,
                                                                 0,
                                                                 0});
         this.nudMaximum.Validated += new System.EventHandler(this.nudMaximum_Validated);
         this.nudMaximum.ValueChanged += new System.EventHandler(this.nudMaximum_ValueChanged);
         // 
         // lblMaximum
         // 
         this.lblMaximum.Location = new System.Drawing.Point(8, 32);
         this.lblMaximum.Name = "lblMaximum";
         this.lblMaximum.Size = new System.Drawing.Size(64, 20);
         this.lblMaximum.TabIndex = 3;
         this.lblMaximum.Text = "&Maximum:";
         this.lblMaximum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudValue
         // 
         this.nudValue.Location = new System.Drawing.Point(72, 56);
         this.nudValue.Name = "nudValue";
         this.nudValue.Size = new System.Drawing.Size(88, 20);
         this.nudValue.TabIndex = 6;
         this.nudValue.Validated += new System.EventHandler(this.nudValue_Validated);
         this.nudValue.ValueChanged += new System.EventHandler(this.nudValue_ValueChanged);
         // 
         // lblValue
         // 
         this.lblValue.Location = new System.Drawing.Point(8, 56);
         this.lblValue.Name = "lblValue";
         this.lblValue.Size = new System.Drawing.Size(64, 20);
         this.lblValue.TabIndex = 5;
         this.lblValue.Text = "&Value:";
         this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtCounterName
         // 
         this.txtCounterName.Location = new System.Drawing.Point(72, 8);
         this.txtCounterName.Name = "txtCounterName";
         this.txtCounterName.Size = new System.Drawing.Size(168, 20);
         this.txtCounterName.TabIndex = 2;
         this.txtCounterName.Text = "";
         this.txtCounterName.Validating += new System.ComponentModel.CancelEventHandler(this.txtCounterName_Validating);
         this.txtCounterName.Validated += new System.EventHandler(this.txtCounterName_Validated);
         // 
         // lblCounterName
         // 
         this.lblCounterName.Location = new System.Drawing.Point(8, 8);
         this.lblCounterName.Name = "lblCounterName";
         this.lblCounterName.Size = new System.Drawing.Size(64, 20);
         this.lblCounterName.TabIndex = 1;
         this.lblCounterName.Text = "&Name:";
         this.lblCounterName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // dataMonitor
         // 
         this.dataMonitor.CounterRowDeleted += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmCounterEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(264, 87);
         this.Controls.Add(this.nudMaximum);
         this.Controls.Add(this.lblMaximum);
         this.Controls.Add(this.nudValue);
         this.Controls.Add(this.lblValue);
         this.Controls.Add(this.txtCounterName);
         this.Controls.Add(this.lblCounterName);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.MaximizeBox = false;
         this.Name = "frmCounterEdit";
         this.ShowInTaskbar = false;
         this.Text = "Edit Counter";
         ((System.ComponentModel.ISupportInitialize)(this.nudMaximum)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.CounterRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmCounterEdit f = frm as frmCounterEdit;
            if (f != null)
            {
               if (f.m_Counter == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmCounterEdit frmNew = new frmCounterEdit(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Overrides
      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }
      #endregion

      #region Event Handlers
      private void nudMaximum_ValueChanged(object sender, System.EventArgs e)
      {
         if (nudValue.Value > nudMaximum.Value)
            nudValue.Value = nudMaximum.Value;
         nudValue.Maximum = nudMaximum.Value;
         m_Counter.Max = (int)nudMaximum.Value;
      }

      private void nudValue_ValueChanged(object sender, System.EventArgs e)
      {
         m_Counter.Value = (int)nudValue.Value;
      }

      private void txtCounterName_Validated(object sender, System.EventArgs e)
      {
         m_Counter.Name = txtCounterName.Text;
      }

      private void txtCounterName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtCounterName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Counter Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtCounterName.Text = m_Counter.Name;
            e.Cancel = true;
         }
         ProjectDataset.CounterRow tr = ProjectData.GetCounter(txtCounterName.Text);
         if ((null != tr) && (m_Counter != tr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtCounterName.Text + " already exists", "Counter Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtCounterName.Text = m_Counter.Name;
            e.Cancel = true;
         }
      }

      private void dataMonitor_CounterRowDeleted(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if (e.Row == m_Counter)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void nudMaximum_Validated(object sender, System.EventArgs e)
      {
         decimal dummy = nudMaximum.Value; // Force control to fire ValueChanged
      }

      private void nudValue_Validated(object sender, System.EventArgs e)
      {
         decimal dummy = nudValue.Value; // Force control to fire ValueChanged      
      }
      #endregion
   }
}

