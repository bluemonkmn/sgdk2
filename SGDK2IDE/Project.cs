using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	public class frmProject : System.Windows.Forms.Form
	{
      private System.Windows.Forms.PropertyGrid grdProject;
      private SGDK2.DataChangeNotifier DataMonitor;
      private System.ComponentModel.IContainer components;
      #region Embedded classes
      private class MapNameProvider : TypeConverter
      {
         public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
         {
            return true;
         }
      
         public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
         {
            return true;
         }
      
         public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
         {
            ArrayList MapNames = new ArrayList();
            MapNames.Add(null);
            foreach(System.Data.DataRowView drv in ProjectData.Map.DefaultView)
               MapNames.Add(((ProjectDataset.MapRow)drv.Row).Name);
            return new System.ComponentModel.TypeConverter.StandardValuesCollection(MapNames);
         }
      }

      private class ProjectProperties
      {
         private ProjectDataset.ProjectRow project;
         public ProjectProperties(ProjectDataset.ProjectRow project)
         {
            this.project = project;
         }

         [Description("Default size for game display screen/window")]
         public GameDisplayMode DisplayMode
         {
            get
            {
               return (GameDisplayMode)System.Enum.Parse(typeof(GameDisplayMode), project.DisplayMode);
            }
            set
            {
               project.DisplayMode = value.ToString();
            }
         }

         [Description("Text to appear in the title bar of the game display window when in windowed mode")]
         public string TitleText
         {
            get
            {
               return project.TitleText;
            }
            set
            {
               project.TitleText = value;
            }
         }

         [Description("Determines if the game display is initially in a window as opposed to full-screen")]
         public bool Windowed
         {
            get
            {
               return project.Windowed;
            }
            set
            {
               project.Windowed = value;
            }
         }

         [Description("Determines the map which is active when the game begins"),
         TypeConverter(typeof(MapNameProvider))]
         public string StartMap
         {
            get
            {
               return project.StartMap;
            }
            set
            {
               project.StartMap = value;
            }
         }

         [Description("Optionally specifies a map which is always active and drawn in front"),
         TypeConverter(typeof(MapNameProvider))]
         public string OverlayMap
         {
            get
            {
               return project.OverlayMap;
            }
            set
            {
               project.OverlayMap = value;
            }
         }
      }
      #endregion


		public frmProject()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         grdProject.SelectedObject = new ProjectProperties(ProjectData.ProjectRow);
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
         this.grdProject = new System.Windows.Forms.PropertyGrid();
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.SuspendLayout();
         // 
         // grdProject
         // 
         this.grdProject.CommandsVisibleIfAvailable = true;
         this.grdProject.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdProject.LargeButtons = false;
         this.grdProject.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdProject.Location = new System.Drawing.Point(0, 0);
         this.grdProject.Name = "grdProject";
         this.grdProject.Size = new System.Drawing.Size(208, 301);
         this.grdProject.TabIndex = 0;
         this.grdProject.Text = "propertyGrid1";
         this.grdProject.ViewBackColor = System.Drawing.SystemColors.Window;
         this.grdProject.ViewForeColor = System.Drawing.SystemColors.WindowText;
         // 
         // DataMonitor
         // 
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         // 
         // frmProject
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(208, 301);
         this.Controls.Add(this.grdProject);
         this.Name = "frmProject";
         this.Text = "Project Properties";
         this.ResumeLayout(false);

      }
		#endregion

      private void DataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }
	}
}
