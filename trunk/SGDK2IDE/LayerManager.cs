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
            foreach(ProjectDataset.TilesetRow tr in ProjectData.Tileset.Rows)
               TilesetNames.Add(tr.Name);
            return new System.ComponentModel.TypeConverter.StandardValuesCollection(TilesetNames);
         }
      }

      private enum BytesPerTile
      {
         One,
         Two,
         Four
      }

      private class LayerProperties 
      {
         private LayerProperties()
         {
         }

         public LayerProperties(ProjectDataset.LayerRow dr)
         {
            m_drLayer = dr;
         }

         public ProjectDataset.LayerRow m_drLayer;
         private int m_BackgroundTile = -1;

         [Description("Name by which this layer will be referred to in this project"),
         Category("Design"),
         ParenthesizePropertyName(true)]
         public string Name
         {
            get
            {
               return m_drLayer.Name;
            }
            set
            {
               m_drLayer.Name = value;
            }
         }

         [Description("Width of the layer in tiles"),
         Category("Layout")]
         public int Width
         {
            get
            {
               return m_drLayer.Width;
            }
            set
            {
               m_drLayer.Width = value;
            }
         }

         [Description("Height of the layer in tiles"),
         Category("Layout")]
         public int Height
         {
            get
            {
               return m_drLayer.Height;
            }
            set
            {
               m_drLayer.Height = value;
            }
         }

         [Description("Tileset that associates tile values in this layer to their graphics"),
         Category("Appearance"),
         TypeConverter(typeof(TilesetNameProvider))]
         public string Tileset
         {
            get
            {
               return m_drLayer.Tileset;
            }
            set
            {
               if (ProjectData.GetTileSet(value) == null)
                  throw new InvalidOperationException("Tileset " + value + " not found");
               m_drLayer.Tileset = value;
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

         [Description("1 byte allows 256 tiles; 2 bytes allows 65536 tiles; 4 bytes allows 4294967296 tiles"),
         Category("Behavior")]
         public BytesPerTile BytesPerTile
         {
            get
            {
               switch(m_drLayer.BytesPerTile)
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
                     m_drLayer.BytesPerTile = 1;
                     break;
                  case BytesPerTile.Two:
                     m_drLayer.BytesPerTile = 2;
                     break;
                  case BytesPerTile.Four:
                     m_drLayer.BytesPerTile = 4;
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
               return m_drLayer.OffsetX;
            }
            set
            {
               m_drLayer.OffsetX = value;
            }
         }

         [Description("Determines the number of pixels by which the top of the layer is offset from the top of the map (when map is at 0,0)"),
         Category("Layout")]
         public int OffsetY
         {
            get
            {
               return m_drLayer.OffsetY;
            }
            set
            {
               m_drLayer.OffsetY = value;
            }
         }

         [Description("Determines the ratio of horizontal layer movement to horizontal map movement"),
         Category("Behavior")]
         public float ScrollRateX
         {
            get
            {
               return m_drLayer.ScrollRateX;
            }
            set
            {
               m_drLayer.ScrollRateX = value;
            }
         }

         [Description("Determines the ratio of vertical layer movement to horizontal map movement"),
         Category("Behavior")]
         public float ScrollRateY
         {
            get
            {
               return m_drLayer.ScrollRateY;
            }
            set
            {
               m_drLayer.ScrollRateY = value;
            }
         }

         public int ZIndex
         {
            get
            {
               return m_drLayer.ZIndex;
            }
            set
            {
               m_drLayer.ZIndex = value;
            }
         }

         [Description("Sprites with priority values equal to this are interleaved into rows of tiles in the layer (allowing isometric-type views).  Greater values are drawn in front."),
         Category("Behavior")]
         public int Priority
         {
            get
            {
               return m_drLayer.Priority;
            }
            set
            {
               m_drLayer.Priority = value;
            }
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
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and clean-up
      public frmLayerManager(ProjectDataset.MapRow parent)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         int iNew;
         for (iNew = 1; ProjectData.GetLayer(parent.Name, "New Layer " + iNew.ToString()) != null; iNew++)
            ;
         ProjectDataset.LayerRow EditRow = ProjectData.NewLayer();
         EditRow.Name = "New Layer " + iNew.ToString();
         EditRow.MapRow = parent;
         EditRow.Width = 100;
         EditRow.Height = 15;
         EditRow.BytesPerTile = 1;
         if (ProjectData.Tileset.Rows.Count > 0)
            EditRow.Tileset = (ProjectData.Tileset.Rows[0] as ProjectDataset.TilesetRow).Name;
         EditRow.BeginEdit();
         pgrLayer.SelectedObject = DataObject = new LayerProperties(EditRow);
         DataObject.BackgroundTile = 0;
      }

      public frmLayerManager(ProjectDataset.LayerRow EditRow)
      {
         InitializeComponent();
         btnOK.Text = "Update";
         EditRow.BeginEdit();
         pgrLayer.SelectedObject = DataObject = new LayerProperties(EditRow);
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
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted_1);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmLayerManager
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(344, 293);
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
            string sValid = ProjectData.ValidateName(DataObject.m_drLayer.Name);
            if (sValid != null)
            {
               MessageBox.Show(this, sValid, "Layer Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if ((DataObject.BackgroundTile < 0) && (DataObject.m_drLayer.IsTilesNull()))
            {
               MessageBox.Show(this, "You must specify a valid Background Tile for a new layer", "Add Layer", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if ((DataObject.BackgroundTile >= 0) &&
               !DataObject.m_drLayer.IsTilesNull())
            {
               if (DialogResult.OK != MessageBox.Show(this, "Setting the BackgroundTile property will clear the entire layer to the specified tile, erasing all tiles.", "Update Layer", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                  return false;
            }
            DataObject.m_drLayer.EndEdit();

            // Microsoft Dataset Bug?  Errors occur during AcceptChanges if name changes
            // at the same time as (for example) Height.
            if (DataObject.m_drLayer.HasVersion(DataRowVersion.Original))
               if (DataObject.m_drLayer.Name.CompareTo(DataObject.m_drLayer[ProjectData.Layer.NameColumn, DataRowVersion.Original]) != 0)
                  DataObject.m_drLayer.AcceptChanges();

            if (DataObject.m_drLayer.RowState == DataRowState.Detached)
            {
               try
               {
                  DataObject.m_drLayer.Tiles = GetTilesForCurrentParams();
                  ProjectData.AddLayerRow(DataObject.m_drLayer);
                  btnOK.Text = "Update";
               }
               catch (ConstraintException)
               {
                  MessageBox.Show(this, "Unable to add the layer due to invalid data.  Please specify a unique name.", "Add Layer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
            else if ((DataObject.m_drLayer.Width != (Int32)DataObject.m_drLayer[ProjectData.Layer.WidthColumn, DataRowVersion.Original]) ||
               (DataObject.m_drLayer.Height != (Int32)DataObject.m_drLayer[ProjectData.Layer.HeightColumn, DataRowVersion.Original]) ||
               (DataObject.m_drLayer.BytesPerTile != (Byte)DataObject.m_drLayer[ProjectData.Layer.BytesPerTileColumn, DataRowVersion.Original]) ||
               (DataObject.BackgroundTile >= 0))
            {
               DataObject.m_drLayer.Tiles = GetTilesForCurrentParams();
            }
            DataObject.m_drLayer.AcceptChanges();
            btnCancel.Text = "Close";
            ((frmMain)MdiParent).SelectByContext("LR" + DataObject.m_drLayer.Name);
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

         int nNewRows = DataObject.m_drLayer.Height;
         int nNewCols = DataObject.m_drLayer.Width;
         object ClearVal;

         // Create an array of tile values representing the data
         // that will fill the byte array to be returned.
         // The elements are typed according to BytesPerTile, but
         // will be serialized as bytes on return.
         switch(DataObject.m_drLayer.BytesPerTile)
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
         if (!DataObject.m_drLayer.IsTilesNull() && (DataObject.BackgroundTile < 0))
         {
            int nOldCols = (int)DataObject.m_drLayer[ProjectData.Layer.WidthColumn, DataRowVersion.Original];
            int nOldRows = (int)DataObject.m_drLayer[ProjectData.Layer.HeightColumn, DataRowVersion.Original];
            byte nOldBytesPerTile = (byte)DataObject.m_drLayer[ProjectData.Layer.BytesPerTileColumn, DataRowVersion.Original];
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(DataObject.m_drLayer.Tiles));
            int nMinWidth = Math.Min(nOldCols, nNewCols);
            int nMinHeight = Math.Min(nOldRows, nNewRows);

            // Read through the old layer data, and copy those tiles that are within
            // the bounds of the new layer dimensions into the new layer data data.
            // The boxing and unboxing might be bad here, but this operation should be rare.
            for (int nRow = 0; nRow < nMinHeight; nRow++)
               for (int nCol = 0; nCol < nOldCols; nCol++)
               {
                  switch (nOldBytesPerTile)
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
         byte nBytesPerTile = DataObject.m_drLayer.BytesPerTile;
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

      #region Event Handlers
      private void btnOK_Click(object sender, System.EventArgs e)
      {
         if (SaveRecord())
            this.Close();
      }

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         DataObject.m_drLayer.CancelEdit();
         this.Close();
      }

      private void dataMonitor_LayerRowDeleted(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (DataObject.m_drLayer == e.Row)
         {
            DataObject.m_drLayer.CancelEdit();
            this.Close();
         }         
      }

      private void dataMonitor_LayerRowDeleted_1(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Row == DataObject.m_drLayer)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }
      #endregion
   }
}
