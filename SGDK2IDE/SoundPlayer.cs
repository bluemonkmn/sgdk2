/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for SoundPlayer.
	/// </summary>
	public class frmSoundPlayer : System.Windows.Forms.Form
	{
      FMOD.System fmodSystem = null;
      FMOD.Sound fmodSound;
      FMOD.Channel fmodChannel;
      private System.Windows.Forms.Button btnPlay;
      private System.Windows.Forms.ProgressBar prgSound;
      private System.Windows.Forms.Button btnStop;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.Timer tmrPlayStatus;
      private System.Windows.Forms.TrackBar trbVolume;
      private System.Windows.Forms.Label lblVolume;
      private System.ComponentModel.IContainer components;

		private frmSoundPlayer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
         if (fmodSound != null)
         {
            ERRCHECK(fmodSound.release());
            fmodSound = null;
         }
         if (fmodSystem != null)
         {
            ERRCHECK(fmodSystem.close());
            ERRCHECK(fmodSystem.release());
            fmodSystem = null;
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
         this.btnPlay = new System.Windows.Forms.Button();
         this.prgSound = new System.Windows.Forms.ProgressBar();
         this.btnStop = new System.Windows.Forms.Button();
         this.btnClose = new System.Windows.Forms.Button();
         this.lblStatus = new System.Windows.Forms.Label();
         this.tmrPlayStatus = new System.Windows.Forms.Timer(this.components);
         this.trbVolume = new System.Windows.Forms.TrackBar();
         this.lblVolume = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
         this.SuspendLayout();
         // 
         // btnPlay
         // 
         this.btnPlay.Location = new System.Drawing.Point(8, 88);
         this.btnPlay.Name = "btnPlay";
         this.btnPlay.Size = new System.Drawing.Size(80, 24);
         this.btnPlay.TabIndex = 0;
         this.btnPlay.Text = "&Play";
         this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
         // 
         // prgSound
         // 
         this.prgSound.Location = new System.Drawing.Point(8, 8);
         this.prgSound.Name = "prgSound";
         this.prgSound.Size = new System.Drawing.Size(264, 16);
         this.prgSound.TabIndex = 1;
         // 
         // btnStop
         // 
         this.btnStop.Location = new System.Drawing.Point(96, 88);
         this.btnStop.Name = "btnStop";
         this.btnStop.Size = new System.Drawing.Size(80, 24);
         this.btnStop.TabIndex = 2;
         this.btnStop.Text = "&Stop";
         this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
         // 
         // btnClose
         // 
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(184, 88);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(88, 24);
         this.btnClose.TabIndex = 3;
         this.btnClose.Text = "&Close";
         // 
         // lblStatus
         // 
         this.lblStatus.Location = new System.Drawing.Point(8, 24);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(264, 24);
         this.lblStatus.TabIndex = 4;
         this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // tmrPlayStatus
         // 
         this.tmrPlayStatus.Tick += new System.EventHandler(this.tmrPlayStatus_Tick);
         // 
         // trbVolume
         // 
         this.trbVolume.LargeChange = 10;
         this.trbVolume.Location = new System.Drawing.Point(112, 48);
         this.trbVolume.Maximum = 100;
         this.trbVolume.Name = "trbVolume";
         this.trbVolume.Size = new System.Drawing.Size(160, 34);
         this.trbVolume.TabIndex = 5;
         this.trbVolume.TickFrequency = 5;
         this.trbVolume.Value = 100;
         this.trbVolume.Scroll += new System.EventHandler(this.trbVolume_Scroll);
         // 
         // lblVolume
         // 
         this.lblVolume.Location = new System.Drawing.Point(8, 48);
         this.lblVolume.Name = "lblVolume";
         this.lblVolume.Size = new System.Drawing.Size(104, 34);
         this.lblVolume.TabIndex = 6;
         this.lblVolume.Text = "Preview Volume:\r\n100%";
         this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // frmSoundPlayer
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(282, 119);
         this.Controls.Add(this.lblVolume);
         this.Controls.Add(this.trbVolume);
         this.Controls.Add(this.lblStatus);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.btnStop);
         this.Controls.Add(this.prgSound);
         this.Controls.Add(this.btnPlay);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmSoundPlayer";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Sound Player";
         ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      public static void PlaySound(IWin32Window owner, byte[] soundData)
      {
         frmSoundPlayer frm;
         frm = new frmSoundPlayer();
         try
         {
            FMOD.System sys = null;         
            frm.ERRCHECK(FMOD.Factory.System_Create(ref sys));
            frm.fmodSystem = sys;
            frm.ERRCHECK(frm.fmodSystem.init(1, FMOD.INITFLAG.NORMAL, IntPtr.Zero));
            FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
            exinfo.length = (uint)soundData.Length;
            FMOD.Sound snd = null;
            frm.ERRCHECK(frm.fmodSystem.createSound(soundData, FMOD.MODE.OPENMEMORY | FMOD.MODE.ACCURATETIME, ref exinfo, ref snd));
            frm.fmodSound = snd;
            frm.ShowDialog(owner);
         }
         catch(ApplicationException ex)
         {
            MessageBox.Show(owner, ex.Message, "Sound Player Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         finally
         {
            if (frm != null)
            {
               frm.Close();
               frm.Dispose(true);
            }
            frm = null;
         }
      }

      private void ERRCHECK(FMOD.RESULT result)
      {
         if (result != FMOD.RESULT.OK)
            throw new ApplicationException(result.ToString());
      }

      private void btnPlay_Click(object sender, System.EventArgs e)
      {
         ERRCHECK(fmodSystem.playSound(0, fmodSound, false, ref fmodChannel));
         fmodChannel.setVolume(trbVolume.Value / 100f);
         btnPlay.Enabled = false;
         btnStop.Enabled = true;
         tmrPlayStatus.Start();
         uint length = 0;
         ERRCHECK(fmodSound.getLength(ref length, FMOD.TIMEUNIT.MS));
         if (length < uint.MaxValue)
         {
            prgSound.Visible = true;
            prgSound.Maximum = (int)length;
         }
         else
         {
            prgSound.Visible = false;
         }
      }

      private void btnStop_Click(object sender, System.EventArgs e)
      {
         if (fmodChannel != null)
         {
            ERRCHECK(fmodChannel.stop());
            fmodChannel = null;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            tmrPlayStatus.Stop();
            prgSound.Value = 0;
         }
      }

      private void tmrPlayStatus_Tick(object sender, System.EventArgs e)
      {
         bool isPlaying = false;
         ERRCHECK(fmodChannel.isPlaying(ref isPlaying));
         if ((fmodChannel != null) && isPlaying)
         {
            uint position = 0;
            ERRCHECK(fmodChannel.getPosition(ref position, FMOD.TIMEUNIT.MS));
            if (prgSound.Visible)
            {
               lblStatus.Text = string.Format("{0:#0.000} sec / {1:#0.000} sec", position / 1000.0d, prgSound.Maximum / 1000.0d);
               prgSound.Value = (int)position;
            }
            else
            {
               lblStatus.Text = string.Format("{0:#0.000} sec", position / 1000.0d);
            }
         }
         else
         {
            fmodChannel = null;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            tmrPlayStatus.Stop();
            prgSound.Value = 0;
            if (prgSound.Visible)
               lblStatus.Text = string.Format("Length = {0:#0.000} ms", prgSound.Maximum / 1000.0d);
            else
               lblStatus.Text = string.Empty;
         }
         fmodSystem.update();
      }

      private void trbVolume_Scroll(object sender, System.EventArgs e)
      {
         if (fmodChannel != null)
            fmodChannel.setVolume(trbVolume.Value / 100f);
         lblVolume.Text = lblVolume.Text.Replace("\r\n","\n").Split('\n')[0] + "\r\n" + trbVolume.Value.ToString() + "%";
      }
   }
}
