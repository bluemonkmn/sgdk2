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
   /// Summary description for LayerManager.
   /// </summary>
   public class frmLayerManager : System.Windows.Forms.Form
   {
      #region Embedded classes
      private class TilesetNameProvider : TypeConverter
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
            ArrayList TilesetNames = new ArrayList();
            foreach(System.Data.DataRowView drv in ProjectData.Tileset.DefaultView)
            {
               ProjectDataset.TilesetRow tr = (ProjectDataset.TilesetRow)drv.Row;
               TilesetNames.Add(tr.Name);
            }
            return new System.ComponentModel.TypeConverter.StandardValuesCollection(TilesetNames);
         }
      }

      public enum BytesPerTile
      {
         One,
         Two,
         Four
      }

      public class LayerProperties 
      {
         private LayerProperties()
         {
         }

         public LayerProperties(ProjectDataset.LayerRow dr)
         {
            m_persistLayer = dr;
            m_tempLayer = (ProjectDataset.LayerRow)dr.Table.NewRow();
            m_tempLayer.ItemArray = (object[])dr.ItemArray.Clone();
         }

         public ProjectDataset.LayerRow m_tempLayer;
         public ProjectDataset.LayerRow m_persistLayer;
         private int m_BackgroundTile = -1;

         [Description("Name by which this layer will be referred to in this project"),
         Category("Design"),
         ParenthesizePropertyName(true)]
         public string Name
         {
            get
            {
               return m_tempLayer.Name;
            }
            set
            {
               m_tempLayer.Name = value;
            }
         }

         [Description("Defines how many tiles are stored by the layer"),
         Category("Layout")]
         public Size SizeInTiles
         {
            get
            {
               return new Size(m_tempLayer.Width, m_tempLayer.Height);
            }
            set
            {
               m_tempLayer.Width = value.Width;
               m_tempLayer.Height = value.Height;
            }
         }

         [Description("Displayable size of the layer in tiles (the data is wrapped if this is larger than SizeInTiles).  Defaults of 0 defer to SizeInTiles."),
         Category("Layout")]
         public Size VirtualSize
         {
            get
            {
               return new Size(m_tempLayer.VirtualWidth, m_tempLayer.VirtualHeight);
            }
            set
            {
               m_tempLayer.VirtualWidth = value.Width;
               m_tempLayer.VirtualHeight = value.Height;
            }
         }

         [Description("Tileset that associates tile values in this layer to their graphics"),
         Category("Appearance"),
         TypeConverter(typeof(TilesetNameProvider))]
         public string Tileset
         {
            get
            {
               return m_tempLayer.Tileset;
            }
            set
            {
               if (ProjectData.GetTileSet(value) == null)
                  throw new InvalidOperationException("Tileset " + value + " not found");
               m_tempLayer.Tileset = value;
            }
         }

         [AmbientValue(-1),
         DescriptionAttribute("Tile value to which the background of the layer will be initialized"),
         Category("Appearance")]
         public int BackgroundTile
         {
            get
            {
               return m_BackgroundTile;
            }
            set
            {
               m_BackgroundTile = value;
            }
         }

         [Description("1 byte allows 256 tiles; 2 bytes allows 32,768 tiles; 4 bytes allows 2,147,483,648 tiles"),
         Category("Behavior")]
         public BytesPerTile BytesPerTile
         {
            get
            {
               switch (m_tempLayer.BytesPerTile)
               {
                  case 1:
                     return BytesPerTile.One;
                  case 2:
                     return BytesPerTile.Two;
                  case 4:
                     return BytesPerTile.Four;
                  default:
                     throw new InvalidOperationException("BytesPerTile conained an invalid value");
               }
            }
            set
            {
               switch(value)
               {
                  case BytesPerTile.One:
                     m_tempLayer.BytesPerTile = 1;
                     break;
                  case BytesPerTile.Two:
                     m_tempLayer.BytesPerTile = 2;
                     break;
                  case BytesPerTile.Four:
                     m_tempLayer.BytesPerTile = 4;
                     break;
                  default:
                     throw new InvalidOperationException("BytesPerTile must be 1, 2 or 4");
               }
            }
         }

         [Description("Determines the number of pixels by which the left side of the layer is offset from the left side of the map (when map is at 0,0)"),
         Category("Layout")]
         public int OffsetX
         {
            get
            {
               return m_tempLayer.OffsetX;
            }
            set
            {
               m_tempLayer.OffsetX = value;
            }
         }

         [Description("Determines the number of pixels by which the top of the layer is offset from the top of the map (when map is at 0,0)"),
         Category("Layout")]
         public int OffsetY
         {
            get
            {
               return m_tempLayer.OffsetY;
            }
            set
            {
               m_tempLayer.OffsetY = value;
            }
         }

         [Description("Determines the ratio of horizontal layer movement to horizontal map movement"),
         Category("Behavior")]
         public float ScrollRateX
         {
            get
            {
               return m_tempLayer.ScrollRateX;
            }
            set
            {
               m_tempLayer.ScrollRateX = value;
            }
         }

         [Description("Determines the ratio of vertical layer movement to horizontal map movement"),
         Category("Behavior")]
         public float ScrollRateY
         {
            get
            {
               return m_tempLayer.ScrollRateY;
            }
            set
            {
               m_tempLayer.ScrollRateY = value;
            }
         }

         [Description("Determines the order of layers in the map.  The lowest ZIndex is drawn farthest in the background"),
         Category("Layout")]
         public int ZIndex
         {
            get
            {
               return m_tempLayer.ZIndex;
            }
            set
            {
               m_tempLayer.ZIndex = value;
            }
         }

         [Description("Sprites with priority values equal to this are interleaved into rows of tiles in the layer (allowing isometric-type views).  Greater values are drawn in front."),
         Category("Behavior")]
         public int Priority
         {
            get
            {
               return m_tempLayer.Priority;
            }
            set
            {
               m_tempLayer.Priority = value;
            }
         }

         public void Persist()
         {
            m_persistLayer.ItemArray = m_tempLayer.ItemArray;
         }
      }
      #endregion

      #region Non-Control Members
      LayerProperties DataObject;
      #endregion

      #region Form designer members
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.PropertyGrid pgrLayer;
      private System.Windows.Forms.Button btnLayerWizard;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and clean-up
      public frmLayerManager(ProjectDataset.MapRow parent)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         int iNew;
         for (iNew = 1; ProjectData.GetLayer(parent.Name, "New Layer " + iNew.ToString()) != null; iNew++)
            ;
         ProjectDataset.LayerRow EditRow = ProjectData.NewLayer();
         EditRow.Name = "New Layer " + iNew.ToString();
         EditRow.MapRow = parent;
         EditRow.Width = 100;
         EditRow.Height = 15;
         EditRow.BytesPerTile = 1;
         if (ProjectData.Tileset.DefaultView.Count > 0)
            EditRow.Tileset = (ProjectData.Tileset.DefaultView[0].Row as ProjectDataset.TilesetRow).Name;
         EditRow.BeginEdit();
         pgrLayer.SelectedObject = DataObject = new LayerProperties(EditRow);
         DataObject.BackgroundTile = 0;

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"LayerManager.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmLayerManager(ProjectDataset.LayerRow EditRow)
      {
         InitializeComponent();
         SGDK2IDE.LoadFormSettings(this);
         btnOK.Text = "Update";
         EditRow.BeginEdit();
         pgrLayer.SelectedObject = DataObject = new LayerProperties(EditRow);

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"LayerManager.html");
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmLayerManager));
         this.pgrLayer = new System.Windows.Forms.PropertyGrid();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.btnLayerWizard = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // pgrLayer
         // 
         this.pgrLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pgrLayer.CommandsVisibleIfAvailable = true;
         this.pgrLayer.LargeButtons = false;
         this.pgrLayer.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.pgrLayer.Location = new System.Drawing.Point(0, 0);
         this.pgrLayer.Name = "pgrLayer";
         this.pgrLayer.Size = new System.Drawing.Size(256, 293);
         this.pgrLayer.TabIndex = 0;
         this.pgrLayer.Text = "PropertyGrid";
         this.pgrLayer.ViewBackColor = System.Drawing.SystemColors.Window;
         this.pgrLayer.ViewForeColor = System.Drawing.SystemColors.WindowText;
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
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // btnLayerWizard
         // 
         this.btnLayerWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnLayerWizard.Location = new System.Drawing.Point(264, 88);
         this.btnLayerWizard.Name = "btnLayerWizard";
         this.btnLayerWizard.Size = new System.Drawing.Size(72, 24);
         this.btnLayerWizard.TabIndex = 3;
         this.btnLayerWizard.Text = "Wizard...";
         this.btnLayerWizard.Click += new System.EventHandler(this.btnLayerWizard_Click);
         // 
         // frmLayerManager
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(344, 293);
         this.Controls.Add(this.btnLayerWizard);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.pgrLayer);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmLayerManager";
         this.Text = "Layer Manager";
         this.ResumeLayout(false);

      }
      #endregion

      #region Private Members
      private Boolean SaveRecord()
      {
         try
         {
            string sValid = ProjectData.ValidateName(DataObject.m_tempLayer.Name);
            if (sValid != null)
            {
               MessageBox.Show(this, sValid, "Layer Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if ((DataObject.BackgroundTile < 0) && (DataObject.m_tempLayer.IsTilesNull()))
            {
               MessageBox.Show(this, "You must specify a valid Background Tile for a new layer", "Add Layer", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if ((DataObject.BackgroundTile >= 0) &&
               !DataObject.m_tempLayer.IsTilesNull())
            {
               if (DialogResult.OK != MessageBox.Show(this, "Setting the BackgroundTile property will clear the entire layer to the specified tile, erasing all tiles.", "Update Layer", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                  return false;
            }

            foreach (ProjectDataset.LayerRow lr in DataObject.m_tempLayer.MapRow.GetLayerRows())
            {
               if (lr != DataObject.m_persistLayer && lr.ZIndex == DataObject.m_tempLayer.ZIndex)
               {
                  if (DialogResult.OK != MessageBox.Show(this, "Another layer with the same ZIndex value already exists. This may cause an unexpected sequencing of the layers.", "Update Layer", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                     return false;
               }
            }

            DataObject.m_tempLayer.EndEdit();

            if (DataObject.m_persistLayer.RowState == DataRowState.Detached)
            {
               try
               {
                  DataObject.m_tempLayer.Tiles = GetTilesForCurrentParams();
                  DataObject.Persist();
                  ProjectData.AddLayerRow(DataObject.m_persistLayer);
                  btnOK.Text = "Update";
                  frmMapEditor.Edit(MdiParent, DataObject.m_persistLayer);
               }
               catch (ConstraintException)
               {
                  MessageBox.Show(this, "Unable to add the layer due to invalid data.  Please specify a unique name.", "Add Layer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
            else if ((DataObject.m_tempLayer.Width != DataObject.m_persistLayer.Width) ||
               (DataObject.m_tempLayer.Height != DataObject.m_persistLayer.Height) ||
               (DataObject.m_tempLayer.BytesPerTile != DataObject.m_persistLayer.BytesPerTile) ||
               (DataObject.BackgroundTile >= 0))
            {
               DataObject.m_tempLayer.Tiles = GetTilesForCurrentParams();
            }
            DataObject.Persist();
            btnCancel.Text = "Close";
            ((frmMain)MdiParent).SelectByContext("LR" + DataObject.m_persistLayer.Name);
            return true;
         }
         catch (ConstraintException)
         {
            MessageBox.Show(this, "Unable to modify the layer due to invalid data.  Please specify a unique name.", "Add Layer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }

      private byte[] GetTilesForCurrentParams()
      {
         System.Array Tiles;

         int nNewRows = DataObject.m_tempLayer.Height;
         int nNewCols = DataObject.m_tempLayer.Width;
         object ClearVal;

         // Create an array of tile values representing the data
         // that will fill the byte array to be returned.
         // The elements are typed according to BytesPerTile, but
         // will be serialized as bytes on return.
         switch (DataObject.m_tempLayer.BytesPerTile)
         {
            case 1:
               Tiles = new byte[nNewRows,nNewCols];
               ClearVal = (byte)DataObject.BackgroundTile;
               break;
            case 2:
               Tiles = new Int16[nNewRows,nNewCols];
               ClearVal = (Int16)DataObject.BackgroundTile;
               break;
            default:
               Tiles = new Int32[nNewRows,nNewCols];
               ClearVal = (Int32)DataObject.BackgroundTile;
               break;
         }

         // Check if we need to copy data from the old layer data
         // (if we have old data and will not be clearing it).
         if (!DataObject.m_tempLayer.IsTilesNull() && (DataObject.BackgroundTile < 0))
         {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(DataObject.m_tempLayer.Tiles));
            int nMinWidth = Math.Min(DataObject.m_persistLayer.Width, nNewCols);
            int nMinHeight = Math.Min(DataObject.m_persistLayer.Height, nNewRows);

            // Read through the old layer data, and copy those tiles that are within
            // the bounds of the new layer dimensions into the new layer data data.
            // The boxing and unboxing might be bad here, but this operation should be rare.
            for (int nRow = 0; nRow < nMinHeight; nRow++)
               for (int nCol = 0; nCol < DataObject.m_persistLayer.Width; nCol++)
               {
                  switch (DataObject.m_persistLayer.BytesPerTile)
                  {
                     case 1:
                        if (nCol < nMinWidth)
                           Tiles.SetValue(br.ReadByte(),nRow,nCol);
                        else
                           br.ReadByte();
                        break;
                     case 2:
                        if (nCol < nMinWidth)
                           Tiles.SetValue(br.ReadInt16(),nRow,nCol);
                        else
                           br.ReadInt16();
                        break;
                     default:
                        if (nCol < nMinWidth)
                           Tiles.SetValue(br.ReadInt32(),nRow,nCol);
                        else
                           br.ReadInt32();
                        break;
                  }
               }
            br.Close();
         }
         // If clearing the layer to a single value is requested,
         // fill the new layer with the value.
         if (DataObject.BackgroundTile >= 0)
         {
            for (int nRow = 0; nRow < nNewRows; nRow++)
               for (int nCol = 0; nCol < nNewCols; nCol++)
                  Tiles.SetValue(ClearVal, nRow, nCol);
         }

         // Serialize the layer tiles into a byte array.
         byte nBytesPerTile = DataObject.m_tempLayer.BytesPerTile;
         System.IO.MemoryStream ms = new System.IO.MemoryStream(nNewRows * nNewCols * nBytesPerTile);
         System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
         
         for (int nRow = 0; nRow < nNewRows; nRow++)
            for (int nCol = 0; nCol < nNewCols; nCol++)
            switch (nBytesPerTile)
            {
               case 1:
                  bw.Write((byte)Tiles.GetValue(nRow,nCol));
                  break;
               case 2:
                  bw.Write((Int16)Tiles.GetValue(nRow,nCol));
                  break;
               default:
                  bw.Write((Int32)Tiles.GetValue(nRow,nCol));
                  break;
            }
         bw.Close();
         return ms.GetBuffer();
      }

      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.LayerRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmLayerManager f = frm as frmLayerManager;
            if (f != null)
            {
               if (f.DataObject.m_persistLayer == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmLayerManager frmNew = new frmLayerManager(EditRow);
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
         //DataObject.m_drLayer.CancelEdit();
         this.Close();
      }

      private void dataMonitor_LayerRowDeleted(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Row == DataObject.m_persistLayer)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void btnLayerWizard_Click(object sender, System.EventArgs e)
      {
         frmLayerWizard frm = new frmLayerWizard(DataObject);
         frm.ShowDialog(this);
         frm.Dispose();
         pgrLayer.SelectedObject = pgrLayer.SelectedObject;
      }
      #endregion
   }
}
