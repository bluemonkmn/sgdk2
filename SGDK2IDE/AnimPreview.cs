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

      delegate void DrawFrame(string spriteName, int tileValue, int realIndex);

      #region Non-control members
      private ProjectDataset.TileRow m_TileRow;
      private TileCache m_TileCache;
      private FrameCache m_FrameCache;
      private ProjectDataset.SpriteStateRow m_SpriteStateRow;
      private CachedSpriteDef m_SpriteCache;
      private System.Threading.Thread m_AnimateThread;
      private long m_startTime;
      private bool m_bEndThread;
      private bool m_DangerWillRobinson;
      private Color m_Background;
      #endregion

      #region Form Designer Members
      private SGDK2.Display display;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.Timer tmrReset;
      private System.Windows.Forms.Label lblFPS;
      private System.Windows.Forms.TrackBar trbFPS;
      private System.Windows.Forms.ComboBox cboColor;
      private System.Windows.Forms.Panel pnlOptions;
      private System.Windows.Forms.Label label1;
      private System.ComponentModel.IContainer components;
      #endregion
      
      private frmAnimPreview()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         QueryPerformanceCounter(ref m_startTime);
         System.Threading.ThreadStart start = new System.Threading.ThreadStart(Animate);
         m_bEndThread = false;
         m_AnimateThread = new System.Threading.Thread(start);
         m_DangerWillRobinson = false;
         cboColor.SelectedIndex=0;
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
         EndAnimationThread();

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
         this.cboColor = new System.Windows.Forms.ComboBox();
         this.pnlOptions = new System.Windows.Forms.Panel();
         this.label1 = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.trbFPS)).BeginInit();
         this.pnlOptions.SuspendLayout();
         this.SuspendLayout();
         // 
         // display
         // 
         this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.display.Dock = System.Windows.Forms.DockStyle.Fill;
         this.display.GameDisplayMode = SGDK2.GameDisplayMode.m640x480x24;
         this.display.Location = new System.Drawing.Point(0, 0);
         this.display.Name = "display";
         this.display.Size = new System.Drawing.Size(160, 96);
         this.display.TabIndex = 0;
         this.display.Windowed = true;
         this.display.Paint += new System.Windows.Forms.PaintEventHandler(this.display_Paint);
         // 
         // dataMonitor
         // 
         this.dataMonitor.FrameRowChanged += new SGDK2.ProjectDataset.FrameRowChangeEventHandler(this.dataMonitor_FrameRowChanged);
         this.dataMonitor.GraphicSheetRowChanged += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowChanged);
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
         this.trbFPS.Location = new System.Drawing.Point(0, 46);
         this.trbFPS.Maximum = 120;
         this.trbFPS.Minimum = 15;
         this.trbFPS.Name = "trbFPS";
         this.trbFPS.Size = new System.Drawing.Size(160, 34);
         this.trbFPS.TabIndex = 1;
         this.trbFPS.TickFrequency = 10;
         this.trbFPS.Value = 60;
         this.trbFPS.ValueChanged += new System.EventHandler(this.trbFPS_ValueChanged);
         // 
         // lblFPS
         // 
         this.lblFPS.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.lblFPS.Location = new System.Drawing.Point(0, 30);
         this.lblFPS.Name = "lblFPS";
         this.lblFPS.Size = new System.Drawing.Size(160, 16);
         this.lblFPS.TabIndex = 2;
         this.lblFPS.Text = "FPS: 60";
         // 
         // cboColor
         // 
         this.cboColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboColor.Items.AddRange(new object[] {
                                                      "Black",
                                                      "White",
                                                      "Red",
                                                      "Green",
                                                      "Blue",
                                                      "Cyan",
                                                      "Magenta",
                                                      "Yellow"});
         this.cboColor.Location = new System.Drawing.Point(80, 8);
         this.cboColor.Name = "cboColor";
         this.cboColor.Size = new System.Drawing.Size(80, 21);
         this.cboColor.TabIndex = 0;
         this.cboColor.SelectedIndexChanged += new System.EventHandler(this.cboColor_SelectedIndexChanged);
         // 
         // pnlOptions
         // 
         this.pnlOptions.Controls.Add(this.label1);
         this.pnlOptions.Controls.Add(this.cboColor);
         this.pnlOptions.Controls.Add(this.lblFPS);
         this.pnlOptions.Controls.Add(this.trbFPS);
         this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.pnlOptions.Location = new System.Drawing.Point(0, 96);
         this.pnlOptions.Name = "pnlOptions";
         this.pnlOptions.Size = new System.Drawing.Size(160, 80);
         this.pnlOptions.TabIndex = 3;
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(0, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(80, 21);
         this.label1.TabIndex = 3;
         this.label1.Text = "Background:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // frmAnimPreview
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(160, 176);
         this.Controls.Add(this.display);
         this.Controls.Add(this.pnlOptions);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.Name = "frmAnimPreview";
         this.ShowInTaskbar = false;
         this.Text = "Animation Preview";
         ((System.ComponentModel.ISupportInitialize)(this.trbFPS)).EndInit();
         this.pnlOptions.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion
   
      private void EndAnimationThread()
      {
         m_bEndThread = true;
         if ((m_AnimateThread != null) && (m_AnimateThread.IsAlive))
         {
            if (!m_AnimateThread.Join(1000))
               m_AnimateThread.Abort();
         }
         m_AnimateThread = null;
      }

      private void Animate()
      {
         int tileValue = 0;
         string spriteName = null;
         
         // To prevent synchronization problems when deleting data rows,
         // cache the row information so we don't try to access a deleted row.
         if (m_SpriteStateRow != null)
            spriteName = m_SpriteStateRow.Name;
         if (m_TileRow != null)
            tileValue = m_TileRow.TileValue;

         long oldTimeIndex = -1;
         long oldRealIndex = -1;
         long freq = 0;
         int fps = 60;
         QueryPerformanceFrequency(ref freq);
         DrawFrame draw = (DrawFrame)Delegate.CreateDelegate(typeof(DrawFrame), this, "DrawCurrentFrame");
         while(!m_bEndThread)
         {
            long frameStart = 0;
            if (!trbFPS.IsDisposed)
               fps = trbFPS.Value;
            if (fps <=0)
               fps = 60;
            QueryPerformanceCounter(ref frameStart);
            long timeIndex = (long)((frameStart-m_startTime) * fps / freq);
            if (oldTimeIndex != timeIndex)
            {
               int realIndex;
               if (m_TileRow != null)
                  realIndex = m_TileCache.GetIndexFromCounterValue(tileValue, (int)timeIndex);
               else
                  realIndex = m_SpriteCache.GetIndexFromSequenceNumber(spriteName, (int)timeIndex);
               if (oldRealIndex != realIndex)
               {
                  // Perform UI work on UI thread.  This ensures that the UI thread doesn't
                  // do something nasty to the display object while we're using it.
                  if (!m_bEndThread)
                  {
                     System.IAsyncResult async = this.BeginInvoke(draw, new object[] {spriteName, tileValue, realIndex});
                     // Monitor requests to end this thread while waiting for invoke to complete
                     // in order to avoid deadlick during end thread request.
                     while(!async.IsCompleted && !m_bEndThread)
                        System.Threading.Thread.Sleep(0);
                     if (async.IsCompleted)
                        this.EndInvoke(async);
                     else
                     {
                        System.Diagnostics.Debug.Assert(m_bEndThread, "Expected m_bEndThread to be true when invoked DrawCurrentFrame did not complete");
                        break;
                     }
                  }
                  oldRealIndex = realIndex;
               }
               oldTimeIndex = timeIndex;
            }
            long usedTime = 0;
            QueryPerformanceCounter(ref usedTime);
            usedTime -= frameStart;
            int sleepTime = (int)(1000 - (usedTime * 1000 * fps / freq )) / fps;
            sleepTime -= 1;
            if (sleepTime < 0)
               sleepTime = 0;
            System.Threading.Thread.Sleep(sleepTime);
         }
      }

      /// <summary>
      /// Exists to be invoked accross threads so UI activity can happen on UI thread.
      /// </summary>
      private void DrawCurrentFrame(string spriteName, int tileValue, int realIndex)
      {
         try
         {
            if (m_bEndThread)
               return;
            display.Device.BeginScene();
            Microsoft.DirectX.Direct3D.Sprite sprite = display.Sprite;
            sprite.Begin(Microsoft.DirectX.Direct3D.SpriteFlags.AlphaBlend);
            display.Device.Clear(Microsoft.DirectX.Direct3D.ClearFlags.Target, m_Background, 0, 0);
            Rectangle bounds = m_FrameCache[m_TileCache[tileValue, 0][0]].Bounds;
            if (m_TileRow != null)
            {
               int[] sf = m_TileCache.GetSubFramesByFrameIndex(tileValue, realIndex);
               for (int i=0; i<sf.Length; i++)
               {
                  int fi = sf[i];
                  sprite.Transform = Microsoft.DirectX.Matrix.Multiply(
                     m_FrameCache[fi].Transform, Microsoft.DirectX.Matrix.Translation(
                     (display.ClientSize.Width - bounds.Width) / 2,
                     (display.ClientSize.Height - bounds.Height) / 2, 0));
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
                     (display.ClientSize.Width - bounds.Width) / 2,
                     (display.ClientSize.Height - bounds.Height) / 2, 0));
                  sprite.Draw(si.frameset[fi].GraphicSheetTexture.Texture,
                     si.frameset[fi].SourceRect, Microsoft.DirectX.Vector3.Empty,
                     Microsoft.DirectX.Vector3.Empty, si.frameset[fi].Color);
               }
            }
            sprite.End();
            display.Device.EndScene();
            display.Device.Present();
         }
         catch(System.Exception ex)
         {
            m_bEndThread = true;
            m_AnimateThread = null;
            MessageBox.Show(MdiParent, "An error occurred in the preview window. This may be a result of having too many displays active. The animation preview window is attempting to contain the problem by exiting the animation process without exiting the application or losing any data. It is recommended that you close the last animation preview window that was opened. Details:\r\n" + ex.ToString(), "Preview Animation", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
         EndAnimationThread();
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
               EndAnimationThread();
               Close();
            }
         }
      }

      private void tmrReset_Tick(object sender, System.EventArgs e)
      {
         tmrReset.Stop();
         EndAnimationThread();

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
               EndAnimationThread();
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
            EndAnimationThread();
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
         try
         {
            if (!m_DangerWillRobinson && (display.Device != null) && (!display.Device.Disposed))
            {
               display.Device.Present();
            }
         }
         catch (System.Exception ex)
         {
            try
            {
               m_DangerWillRobinson = true;
               m_bEndThread = true;
               m_AnimateThread = null;
               MessageBox.Show(MdiParent, "An error occurred trying to draw the animation preview display. This may be a result of another error that would appear in a separate pop-up. In order to tiptoe through this problem, the animation window will deactivate the animation, but leave the window open.  It is recommended that you close the animation window yourself, and hopefully we can avoid an unexpected termination or loss/corruption of data.  Error details:\r\n" + ex.ToString(), "Animation Preview", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (System.Exception)
            {
            }
         }
      }

      private void trbFPS_ValueChanged(object sender, System.EventArgs e)
      {
         lblFPS.Text = "FPS: " + trbFPS.Value.ToString();
      }

      private void cboColor_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         m_Background = Color.FromName(cboColor.Text);
      }

      private void dataMonitor_GraphicSheetRowChanged(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Action == System.Data.DataRowAction.Change)
            display.DisposeAllTextures();
      }
   }
}
