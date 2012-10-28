/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Summary description for MapManager.
	/// </summary>
	public class frmMapManager : System.Windows.Forms.Form
	{
      #region Embedded classes
      public class MapProperties 
      {
         private MapProperties()
         {
         }

         public MapProperties(ProjectDataset.MapRow dr)
         {
            m_drMap = dr;
         }

         public ProjectDataset.MapRow m_drMap;

         [Description("Name by which this map will be referred to in this project"),
         Category("Design"),
         ParenthesizePropertyName(true)]
         public string Name
         {
            get
            {
               return m_drMap.Name;
            }
            set
            {
               m_drMap.Name = value;
            }
         }

         [Description("Backslash-delimited path specifying parent folder structure in which this map is nested in the tree view"),
         Category("Design"),
         ParenthesizePropertyName(true)]
         public string Folder
         {
            get
            {
               return m_drMap.Folder;
            }
            set
            {
               m_drMap.Folder = value;
            }
         }

         [Description("Minimum amount of space allowed between the left edge of the player and the left edge of the map window"),
         Category("Layout")]
         public short ScrollMarginLeft
         {
            get
            {
               return m_drMap.ScrollMarginLeft;
            }
            set
            {
               m_drMap.ScrollMarginLeft = value;
            }
         }
         [Description("Minimum amount of space allowed between the top of the player and the top of the map window"),
         Category("Layout")]
         public short ScrollMarginTop
         {
            get
            {
               return m_drMap.ScrollMarginTop;
            }
            set
            {
               m_drMap.ScrollMarginTop = value;
            }
         }
         [Description("Minimum amount of space allowed between the right edge of the player and the right edge of the map window"),
         Category("Layout")]
         public short ScrollMarginRight
         {
            get
            {
               return m_drMap.ScrollMarginRight;
            }
            set
            {
               m_drMap.ScrollMarginRight = value;
            }
         }
         [Description("Minimum amount of space allowed between the bottom of the player and the bottom of the map window"),
         Category("Layout")]
         public short ScrollMarginBottom
         {
            get
            {
               return m_drMap.ScrollMarginBottom;
            }
            set
            {
               m_drMap.ScrollMarginBottom = value;
            }
         }
   
         [Description("Rectangle in which the layers of the map are drawn within the display (default of all 0 fills display)"),
         Category("Layout")]
         public Rectangle View
         {
            get
            {
               return new Rectangle(m_drMap.ViewLeft, m_drMap.ViewTop, m_drMap.ViewWidth, m_drMap.ViewHeight);
            }
            set
            {
               m_drMap.ViewLeft = (short)value.Left;
               m_drMap.ViewTop = (short)value.Top;
               m_drMap.ViewWidth = (short)value.Width;
               m_drMap.ViewHeight = (short)value.Height;
            }
         }

         [Description("Scrollable pixel size of the map independent of its layers"),
         Category("Layout")]
         public Size ScrollableSize
         {
            get
            {
               return new Size(m_drMap.ScrollWidth, m_drMap.ScrollHeight);
            }
            set
            {
               m_drMap.ScrollWidth = value.Width;
               m_drMap.ScrollHeight = value.Height;
            }
         }
      }
      #endregion

      #region Non-Control Members
      MapProperties DataObject;
      #endregion

      #region Form designer members
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.PropertyGrid pgrMap;
      private System.Windows.Forms.Button btnScrollWizard;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and clean-up
      public frmMapManager()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         int iNew;
         for (iNew = 1; ProjectData.GetMap("New Map " + iNew.ToString()) != null; iNew++)
            ;
         ProjectDataset.MapRow EditRow = ProjectData.NewMap();
         EditRow.Name = "New Map " + iNew.ToString();
         EditRow.ScrollMarginLeft = 32;
         EditRow.ScrollMarginTop = 32;
         EditRow.ScrollMarginRight = 32;
         EditRow.ScrollMarginBottom = 32;
         EditRow.BeginEdit();
         pgrMap.SelectedObject = DataObject = new MapProperties(EditRow);

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/03872e63-7ce8-444e-8811-ae71524bc343.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmMapManager(ProjectDataset.MapRow EditRow)
      {
         InitializeComponent();
         SGDK2IDE.LoadFormSettings(this);
         btnOK.Text = "Update";
         EditRow.BeginEdit();
         pgrMap.SelectedObject = DataObject = new MapProperties(EditRow);

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/03872e63-7ce8-444e-8811-ae71524bc343.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
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
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMapManager));
         this.pgrMap = new System.Windows.Forms.PropertyGrid();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.btnScrollWizard = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // pgrMap
         // 
         this.pgrMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pgrMap.CommandsVisibleIfAvailable = true;
         this.pgrMap.LargeButtons = false;
         this.pgrMap.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.pgrMap.Location = new System.Drawing.Point(0, 0);
         this.pgrMap.Name = "pgrMap";
         this.pgrMap.Size = new System.Drawing.Size(256, 293);
         this.pgrMap.TabIndex = 0;
         this.pgrMap.Text = "PropertyGrid";
         this.pgrMap.ViewBackColor = System.Drawing.SystemColors.Window;
         this.pgrMap.ViewForeColor = System.Drawing.SystemColors.WindowText;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(264, 8);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "Add";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(264, 40);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // dataMonitor
         // 
         this.dataMonitor.MapRowDeleted += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // btnScrollWizard
         // 
         this.btnScrollWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnScrollWizard.Location = new System.Drawing.Point(264, 80);
         this.btnScrollWizard.Name = "btnScrollWizard";
         this.btnScrollWizard.Size = new System.Drawing.Size(72, 24);
         this.btnScrollWizard.TabIndex = 3;
         this.btnScrollWizard.Text = "Wizard...";
         this.btnScrollWizard.Click += new System.EventHandler(this.btnScrollWizard_Click);
         // 
         // frmMapManager
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(344, 293);
         this.Controls.Add(this.btnScrollWizard);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.pgrMap);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmMapManager";
         this.Text = "Map Manager";
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private Boolean SaveRecord()
      {
         try
         {
            string sValid = ProjectData.ValidateName(DataObject.m_drMap.Name);
            if (sValid != null)
            {
               MessageBox.Show(this, sValid, "Map Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            DataObject.m_drMap.EndEdit();

            if (DataObject.m_drMap.RowState == DataRowState.Detached)
            {
               try
               {
                  ProjectData.AddMapRow(DataObject.m_drMap);
                  ProjectDataset.LayerRow bgLayer = ProjectData.NewLayer();
                  bgLayer.Name = "Designer Background";
                  bgLayer.Tiles = new byte[] {0};
                  bgLayer.Height = 1;
                  bgLayer.Width = 1;
                  bgLayer.VirtualHeight = 4000;
                  bgLayer.VirtualWidth = 4000;
                  bgLayer.BytesPerTile = 1;
                  bgLayer.ScrollRateX = 0;
                  bgLayer.ScrollRateY = 0;
                  bgLayer.MapRow = DataObject.m_drMap;
                  ProjectData.AddLayerRow(bgLayer);
                  btnOK.Text = "Update";
               }
               catch (ConstraintException)
               {
                  MessageBox.Show(this, "Unable to add the map due to invalid data.  Please specify a unique name.", "Add Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
            btnCancel.Text = "Close";
            ((frmMain)MdiParent).SelectByContext("MP" + DataObject.m_drMap.Name);
            return true;
         }
         catch (ConstraintException)
         {
            MessageBox.Show(this, "Unable to modify the map due to invalid data.  Please specify a unique name.", "Add Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }      
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.MapRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmMapManager f = frm as frmMapManager;
            if (f != null)
            {
               if (f.DataObject.m_drMap == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmMapManager frmNew = new frmMapManager(EditRow);
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
      private void btnOK_Click(object sender, System.EventArgs e)
      {
         if (SaveRecord())
            this.Close();
      }

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         DataObject.m_drMap.CancelEdit();
         this.Close();
      }

      private void dataMonitor_MapRowDeleted(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if (DataObject.m_drMap == e.Row)
         {
            DataObject.m_drMap.CancelEdit();
            this.Close();
         }         
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void btnScrollWizard_Click(object sender, System.EventArgs e)
      {
         frmMapScrollWizard frm = new frmMapScrollWizard(DataObject.m_drMap);
         frm.ShowDialog(this);
         frm.Dispose();
         pgrMap.SelectedObject = pgrMap.SelectedObject;
      }
      #endregion
   }
}
