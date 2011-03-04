// File version 4 (initially distributed with version 2.1.3)
// Last change: Implement PlayMusic and FadeMusic.

using System;
using System.ComponentModel;

namespace CustomObjects
{
   public enum SoundReplay
   {
      Continue,
      StartNew,
      Restart,
   }

   abstract public partial class FMODBase
   {
      private static FMOD.System system  = null;
      private FMOD.Channel channel = null;
      private FMOD.Sound sound  = null;
      private float lastSetVolume;
      private static FMODBase music = null;
      private static FMODBase oldMusic = null;
      private static System.Collections.ArrayList soundList = new System.Collections.ArrayList();
      // Must keep a reference to the callback to prevent
      // Garbage collector from causing NullReferenceException
      private static FMOD.CHANNEL_CALLBACK updateCallback = null;

      public FMODBase()
      {
         InitFMOD();
      }

      private static void InitFMOD()
      {
         if (system == null)
         {
            ERRCHECK(FMOD.Factory.System_Create(ref system));
            ERRCHECK(system.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null));
            Project.GameWindow.OnFrameStart += new GameForm.SimpleNotification(OnFrameStart);
            updateCallback = (FMOD.CHANNEL_CALLBACK)Delegate.CreateDelegate(typeof(FMOD.CHANNEL_CALLBACK), typeof(FMODBase), "Channel_Callback");
         }
      }

      public static void OnFrameStart()
      {
         ERRCHECK(system.update());
         ManageMusic();
      }

      [Description("Play an FMOD sound effect. Returns true if the sound was (re-)started.")]
      public static bool PlaySound([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound, SoundReplay ReplayOption)
      {
         LoadSound(Sound);
         if (Sound.isPlaying)
         {
            switch(ReplayOption)
            {
               case SoundReplay.Continue:
                  return false;
               case SoundReplay.StartNew:
                  ERRCHECK(system.playSound(FMOD.CHANNELINDEX.FREE, Sound.sound, false, ref Sound.channel));
                  break;
               case SoundReplay.Restart:
                  Sound.Stop();
                  ERRCHECK(system.playSound(FMOD.CHANNELINDEX.REUSE, Sound.sound, false, ref Sound.channel));
                  break;
            }
         }
         else
            ERRCHECK(system.playSound(FMOD.CHANNELINDEX.REUSE, Sound.sound, false, ref Sound.channel));

         ERRCHECK(Sound.channel.setCallback(updateCallback));
         ERRCHECK(Sound.channel.setVolume(Sound.Volume));
         Sound.lastSetVolume = Sound.Volume;
         return true;
      }

      [Description("Fade out any background music which is playing.")]
      public static void FadeMusic()
      {
         if (oldMusic != null)
            oldMusic.Stop();
         if (music != null)
         {
            oldMusic = music;
            music = null;
         }
      }

      [Description("Play an FMOD sound as background music which will fade out when new music starts.")]
      public static void PlayMusic([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound)
      {
         bool newSoundStarted = PlaySound(Sound, SoundReplay.Continue);

         if ((music != Sound) && (music != null))
         {
            if ((oldMusic != null) && (Sound != oldMusic))
               oldMusic.Stop();
            oldMusic = music;
            if (newSoundStarted)
            {
               Sound.lastSetVolume = .005f;
               ERRCHECK(Sound.channel.setVolume(Sound.lastSetVolume));
            }
         }
         music = Sound;
         if (music == oldMusic)
            oldMusic = null;
      }

      private static void ManageMusic()
      {
         if (oldMusic != null)
         {
            oldMusic.lastSetVolume -= .005f;
            if (oldMusic.lastSetVolume < 0)
            {
               oldMusic.Stop();
               oldMusic = null;
            }
            else
            {
               ERRCHECK(oldMusic.channel.setVolume(oldMusic.lastSetVolume));
            }
         }
         if (music != null)
         {
            if (music.lastSetVolume < music.Volume)
            {
               music.lastSetVolume += .005f;
               if (music.lastSetVolume > music.Volume)
                  music.lastSetVolume = music.Volume;
               ERRCHECK(music.channel.setVolume(music.lastSetVolume));
            }
         }
      }

      [Description("Pause or resume an FMOD sound effect")]
      public static void PauseSound([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound, bool Pause)
      {
         ERRCHECK(Sound.channel.setPaused(Pause));
      }

      private static FMOD.RESULT Channel_Callback(IntPtr channelraw, FMOD.CHANNEL_CALLBACKTYPE type, IntPtr commanddata1, IntPtr commanddata2)
      {
         try
         {
            for(int i=0; i<soundList.Count; i++)
            {
               FMODBase snd = (FMODBase)soundList[i];
               if ((snd.channel != null) && (snd.channel.getRaw() == channelraw) && (type == FMOD.CHANNEL_CALLBACKTYPE.END))
               {
                  snd.channel = null;
                  if (snd == oldMusic)
                     oldMusic = null;
                  if (snd == music)
                     music = null;
               }
            }
            return FMOD.RESULT.OK;
         }
         catch(System.Exception)
         {
            return FMOD.RESULT.ERR_UPDATE;
         }
      }

      public bool isPlaying
      {
         get
         {
            if (channel != null)
            {
               bool result = false;
               ERRCHECK(channel.isPlaying(ref result));
               return result;
            }
            return false;
         }
      }

      [Description("Stop a playing FMOD sound effect")]
      public static void StopSound([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound)
      {
         Sound.Stop();
      }

      private void Stop()
      {
         if(isPlaying)
            ERRCHECK(channel.stop());
         channel = null;
         if (this == oldMusic)
            oldMusic = null;
          if (this == music)
            music = null;         
      }

      [Description("Release the memory associated with the specified FMOD object")]
      public static void UnloadSound([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound)
      {
         Sound.Unload();
      }

      private void Unload()
      {
         ERRCHECK(sound.release());
         sound = null;
      }

      [Description("Force the specified sound to be loaded into FMOD memory")]
      public static void LoadSound([System.ComponentModel.Editor("CustomObject", "UITypeEditor")] CustomObjects.FMODBase Sound)
      {
         Sound.Load();
      }

      private void Load()
      {
         if (sound == null)
         {
            System.IO.Stream fmodrc = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Name + ".bin");
            byte[] buf = new byte[fmodrc.Length];
            fmodrc.Read(buf, 0, (int)fmodrc.Length);
            fmodrc.Close();
            FMOD.CREATESOUNDEXINFO sndinf = new FMOD.CREATESOUNDEXINFO();
            sndinf.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(sndinf);
            sndinf.length = (uint)buf.Length;
            sndinf.suggestedsoundtype = SoundType;
            ERRCHECK(system.createSound(buf, FMOD.MODE.OPENMEMORY | FMOD.MODE.SOFTWARE, ref sndinf, ref sound));
            soundList.Add(this);
         }
      }

      protected FMOD.SOUND_TYPE SoundType
      {
         get
         {
            return FMOD.SOUND_TYPE.UNKNOWN;
         }
      }

      // Override this member to override default volume.
      protected virtual float Volume
      {
         get
         {
            return 1.0f;
         }
      }

      private static void ERRCHECK(FMOD.RESULT result)
      {
         if (result != FMOD.RESULT.OK)
         {
            throw new ApplicationException("FMOD error! " + result + " - " + FMOD.Error.String(result));
         }
      }
   }
}

