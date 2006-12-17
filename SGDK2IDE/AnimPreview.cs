using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for AnimPreview.
	/// </summary>
	public class frmAnimPreview : System.Windows.Forms.Form
	{
      [System.Runtime.InteropServices.DllImport("kernel32.dll")]
      extern static short QueryPerformanceCounter(ref long x);
      [System.Runtime.InteropServices.DllImport("kernel32.dll")]
      extern static short QueryPerformanceFrequency(ref long x);

      #region Non-control members
      private ProjectDataset.TileRow m_TileRow;
      private TileCache m_TileCache;
      private FrameCache m_FrameCache;
      private ProjectDataset.SpriteStateRow m_SpriteStateRow;
      private CachedSpriteDef m_SpriteCache;
      private System.Threading.Thread m_AnimateThread;
      private System.Threading.Thread m_UIThread;
      private long m_startTime;
      private bool m_bEndThread;
      #endregion

      #region Form Designer Members
      private SGDK2.Display display;
      #endregion
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.Timer tmrReset;
      private System.Windows.Forms.Label lblFPS;
      private System.Windows.Forms.TrackBar trbFPS;
      private System.ComponentModel.IContainer components;
      
      private frmAnimPreview()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         QueryPerformanceCounter(ref m_startTime);
         System.Threading.ThreadStart start = new System.Threading.ThreadStart(Animate);
         m_bEndThread = false;
         m_UIThread = System.Threading.Thread.CurrentThread;
         m_AnimateThread = new System.Threading.Thread(start);
      }

      public frmAnimPreview(ProjectDataset.TileRow drTile) : this()
		{
         m_TileRow = drTile;
         m_TileCache = new TileCache(drTile.TilesetRow);
         m_FrameCache = FrameCache.GetFrameCache(drTile.TilesetRow.Frameset, display);
         m_SpriteStateRow = null;
         m_SpriteCache = null;
		}

      public frmAnimPreview(ProjectDataset.SpriteStateRow drState) : this()
      {
         m_SpriteStateRow = drState;
         m_SpriteCache = new CachedSpriteDef(drState.SpriteDefinitionRow, display);
         m_TileRow = null;
         m_TileCache = null;
         m_FrameCache = null;
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
         m_bEndThread = true;
         if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
            m_AnimateThread.Join();
         m_AnimateThread = null;

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
         this.display = new SGDK2.Display();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.tmrReset = new System.Windows.Forms.Timer(this.components);
         this.trbFPS = new System.Windows.Forms.TrackBar();
         this.lblFPS = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.trbFPS)).BeginInit();
         this.SuspendLayout();
         // 
         // display
         // 
         this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.display.Dock = System.Windows.Forms.DockStyle.Fill;
         this.display.GameDisplayMode = SGDK2.GameDisplayMode.m640x480x24;
         this.display.Location = new System.Drawing.Point(0, 0);
         this.display.Name = "display";
         this.display.Size = new System.Drawing.Size(128, 102);
         this.display.TabIndex = 0;
         this.display.Windowed = true;
         this.display.Paint += new System.Windows.Forms.PaintEventHandler(this.display_Paint);
         // 
         // dataMonitor
         // 
         this.dataMonitor.FrameRowChanged += new SGDK2.ProjectDataset.FrameRowChangeEventHandler(this.dataMonitor_FrameRowChanged);
         this.dataMonitor.TileFrameRowChanged += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.TileRowDeleted += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowDeleted);
         this.dataMonitor.SpriteFrameRowDeleted += new SGDK2.ProjectDataset.SpriteFrameRowChangeEventHandler(this.dataMonitor_SpriteFrameRowChanged);
         this.dataMonitor.SpriteFrameRowChanged += new SGDK2.ProjectDataset.SpriteFrameRowChangeEventHandler(this.dataMonitor_SpriteFrameRowChanged);
         this.dataMonitor.TilesetRowChanged += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowChanged);
         this.dataMonitor.FrameRowDeleted += new SGDK2.ProjectDataset.FrameRowChangeEventHandler(this.dataMonitor_FrameRowChanged);
         this.dataMonitor.SpriteStateRowDeleted += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.dataMonitor_SpriteStateRowChanged);
         this.dataMonitor.TileFrameRowDeleted += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         // 
         // tmrReset
         // 
         this.tmrReset.Tick += new System.EventHandler(this.tmrReset_Tick);
         // 
         // trbFPS
         // 
         this.trbFPS.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.trbFPS.LargeChange = 20;
         this.trbFPS.Location = new System.Drawing.Point(0, 118);
         this.trbFPS.Maximum = 120;
         this.trbFPS.Minimum = 15;
         this.trbFPS.Name = "trbFPS";
         this.trbFPS.Size = new System.Drawing.Size(128, 34);
         this.trbFPS.TabIndex = 1;
         this.trbFPS.TickFrequency = 10;
         this.trbFPS.Value = 60;
         this.trbFPS.ValueChanged += new System.EventHandler(this.trbFPS_ValueChanged);
         // 
         // lblFPS
         // 
         this.lblFPS.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.lblFPS.Location = new System.Drawing.Point(0, 102);
         this.lblFPS.Name = "lblFPS";
         this.lblFPS.Size = new System.Drawing.Size(128, 16);
         this.lblFPS.TabIndex = 2;
         this.lblFPS.Text = "FPS: 60";
         // 
         // frmAnimPreview
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(128, 152);
         this.Controls.Add(this.display);
         this.Controls.Add(this.lblFPS);
         this.Controls.Add(this.trbFPS);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.Name = "frmAnimPreview";
         this.ShowInTaskbar = false;
         this.Text = "Animation Preview";
         ((System.ComponentModel.ISupportInitialize)(this.trbFPS)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion
   
      void Animate()
      {
         int tileValue = 0;
         string spriteName = null;
         
         try
         {
            // To prevent synchronization problems when deleting data rows,
            // cache the row information so we don't try to access a deleted row.
            if (m_SpriteStateRow != null)
               spriteName = m_SpriteStateRow.Name;
            if (m_TileRow != null)
               tileValue = m_TileRow.TileValue;

            long oldTimeIndex = -1;
            long oldRealIndex = -1;
            long freq = 0;
            QueryPerformanceFrequency(ref freq);
            while(!m_bEndThread)
            {
               long frameStart = 0;
               QueryPerformanceCounter(ref frameStart);
               long timeIndex = (long)((frameStart-m_startTime) * trbFPS.Value / freq);
               if (oldTimeIndex != timeIndex)
               {
                  int realIndex;
                  if (m_TileRow != null)
                     realIndex = m_TileCache.GetIndexFromCounterValue(tileValue, (int)timeIndex);
                  else
                     realIndex = m_SpriteCache.GetIndexFromSequenceNumber(spriteName, (int)timeIndex);
                  if (oldRealIndex != realIndex)
                  {
                     lock(display)
                     {
                        display.Device.BeginScene();
                        Microsoft.DirectX.Direct3D.Sprite sprite = display.Sprite;
                        sprite.Begin(Microsoft.DirectX.Direct3D.SpriteFlags.AlphaBlend);
                        display.Device.Clear(Microsoft.DirectX.Direct3D.ClearFlags.Target, 0, 0, 0);
                        if (m_TileRow != null)
                        {
                           int[] sf = m_TileCache.GetSubFramesByFrameIndex(tileValue, realIndex);
                           for (int i=0; i<sf.Length; i++)
                           {
                              int fi = sf[i];
                              sprite.Transform = Microsoft.DirectX.Matrix.Multiply(
                                 m_FrameCache[fi].Transform,Microsoft.DirectX.Matrix.Translation(
                                 (display.ClientSize.Width - m_FrameCache[fi].Bounds.Width) / 2,
                                 (display.ClientSize.Height - m_FrameCache[fi].Bounds.Height) / 2, 0));
                              sprite.Draw(m_FrameCache[fi].GraphicSheetTexture.Texture,
                                 m_FrameCache[fi].SourceRect, Microsoft.DirectX.Vector3.Empty,
                                 Microsoft.DirectX.Vector3.Empty, m_FrameCache[fi].Color);
                           }
                        }
                        else
                        {
                           StateInfo si = m_SpriteCache[spriteName];
                           int[] sf = si.frames[realIndex].subFrames;
                           for (int i=0; i<sf.Length; i++)
                           {
                              int fi = sf[i];
                              sprite.Transform = Microsoft.DirectX.Matrix.Multiply(
                                 si.frameset[fi].Transform, Microsoft.DirectX.Matrix.Translation(
                                 (display.ClientSize.Width - si.frameset[fi].Bounds.Width) / 2,
                                 (display.ClientSize.Height - si.frameset[fi].Bounds.Height) / 2, 0));
                              sprite.Draw(si.frameset[fi].GraphicSheetTexture.Texture,
                                 si.frameset[fi].SourceRect, Microsoft.DirectX.Vector3.Empty,
                                 Microsoft.DirectX.Vector3.Empty, si.frameset[fi].Color);
                           }
                        }
                        sprite.End();
                        display.Device.EndScene();
                        display.Device.Present();
                     }
                     oldRealIndex = realIndex;
                  }
                  oldTimeIndex = timeIndex;
               }
               long usedTime = 0;
               QueryPerformanceCounter(ref usedTime);
               usedTime -= frameStart;
               int sleepTime = (int)(1000 - (usedTime * 1000 * trbFPS.Value / freq ))/trbFPS.Value;
               sleepTime -= 1;
               if (sleepTime < 0)
                  sleepTime = 0;
               System.Threading.Thread.Sleep(sleepTime);
            }
         }
         catch(System.Exception)
         {
            m_AnimateThread = null;
            this.Close();
         }
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad (e);
         Show();
         m_AnimateThread.Start();
      }

      protected override void OnClosed(EventArgs e)
      {
         base.OnClosed (e);
         m_bEndThread = true;
         if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
            m_AnimateThread.Join();
         m_AnimateThread = null;
      }

      private void QueueReset()
      {
         m_bEndThread = true;
         tmrReset.Stop();
         tmrReset.Start();
      }

      private void dataMonitor_FrameRowChanged(object sender, SGDK2.ProjectDataset.FrameRowChangeEvent e)
      {
         bool bAffected = false;
         if ((m_TileRow != null) && (m_TileRow.TilesetRow.FramesetRow == e.Row.FramesetRow))
            bAffected = true;
         else if ((m_SpriteStateRow != null) && (m_SpriteStateRow.FramesetRow == e.Row.FramesetRow))
            bAffected = true;
         if (bAffected)
         {
            if (e.Action == System.Data.DataRowAction.Change)
            {
               QueueReset();
            }
            else if (e.Action == System.Data.DataRowAction.Delete)
            {
               m_bEndThread = true;
               if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
                  m_AnimateThread.Join();
               m_AnimateThread = null;
               Close();
            }
         }
      }

      private void tmrReset_Tick(object sender, System.EventArgs e)
      {
         tmrReset.Stop();
         if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
            m_AnimateThread.Join();
         m_AnimateThread = null;

         if (IsDisposed)
            return;

         if (m_TileRow != null)
         {
            m_TileCache = new TileCache(m_TileRow.TilesetRow);
            FrameCache.ClearDisplayCache(display);
            m_FrameCache = FrameCache.GetFrameCache(m_TileRow.TilesetRow.Frameset, display);
         }
         else
         {
            FrameCache.ClearDisplayCache(display);
            m_SpriteCache.RefreshState(m_SpriteStateRow, display);
         }
         System.Threading.ThreadStart start = new System.Threading.ThreadStart(Animate);
         m_bEndThread = false;
         m_AnimateThread = new System.Threading.Thread(start);
         m_AnimateThread.Start();
      }

      private void dataMonitor_SpriteFrameRowChanged(object sender, SGDK2.ProjectDataset.SpriteFrameRowChangeEvent e)
      {
         if (m_SpriteStateRow == e.Row.SpriteStateRowParent)
         {
            if ((e.Action == System.Data.DataRowAction.Change) ||
               (e.Action == System.Data.DataRowAction.Delete))
               QueueReset();
         }
      }

      private void dataMonitor_SpriteStateRowChanged(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if (m_SpriteStateRow == e.Row)
         {
            if (e.Action == System.Data.DataRowAction.Delete)
            {
               m_bEndThread = true;
               if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
                  m_AnimateThread.Join();
               m_AnimateThread = null;
               Close();
            }
         }
      }

      private void dataMonitor_TileFrameRowChanged(object sender, SGDK2.ProjectDataset.TileFrameRowChangeEvent e)
      {
         if (m_TileRow == e.Row.TileRowParent)
         {
            if ((e.Action == System.Data.DataRowAction.Change) ||
               (e.Action == System.Data.DataRowAction.Delete))
               QueueReset();
         }      
      }

      private void dataMonitor_TileRowDeleted(object sender, SGDK2.ProjectDataset.TileRowChangeEvent e)
      {
         if (m_TileRow == e.Row)
         {
            m_bEndThread = true;
            if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
               m_AnimateThread.Join();
            m_AnimateThread = null;
            Close();
         }
      }

      private void dataMonitor_TilesetRowChanged(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if ((m_TileRow != null) && (m_TileRow.TilesetRow == e.Row))
         {
            if (e.Action == System.Data.DataRowAction.Change)
            {
               QueueReset();
            }
         }
      }

      private void display_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if ((display.Device != null) && (!display.Device.Disposed))
         {
            lock(display)
               display.Device.Present();
         }
      }

      private void trbFPS_ValueChanged(object sender, System.EventArgs e)
      {
         lblFPS.Text = "FPS: " + trbFPS.Value.ToString();
      }
   }
}
