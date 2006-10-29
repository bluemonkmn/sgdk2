/* ========================================================================================== */
/* FMOD Ex - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2006.          */
/*                                                                                            */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FMOD
{
    /*
        FMOD version number.  Check this against FMOD::System::getVersion / System_GetVersion
        0xaaaabbcc -> aaaa = major version number.  bb = minor version number.  cc = development version number.
    */
    public class VERSION
    {
        public const int    number = 0x00040435;
        public const string dll    = "fmodex.dll";
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        error codes.  Returned from every function.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]      
    ]
    */
    public enum RESULT
    {
        OK,                        /* No errors. */
        ERR_ALREADYLOCKED,         /* Tried to call lock a second time before unlock was called. */
        ERR_BADCOMMAND,            /* Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound). */
        ERR_CDDA_DRIVERS,          /* Neither NTSCSI nor ASPI could be initialised. */
        ERR_CDDA_INIT,             /* An error occurred while initialising the CDDA subsystem. */
        ERR_CDDA_INVALID_DEVICE,   /* Couldn't find the specified device. */
        ERR_CDDA_NOAUDIO,          /* No audio tracks on the specified disc. */
        ERR_CDDA_NODEVICES,        /* No CD/DVD devices were found. */ 
        ERR_CDDA_NODISC,           /* No disc present in the specified drive. */
        ERR_CDDA_READ,             /* A CDDA read error occurred. */
        ERR_CHANNEL_ALLOC,         /* Error trying to allocate a channel. */
        ERR_CHANNEL_STOLEN,        /* The specified channel has been reused to play another sound. */
        ERR_COM,                   /* A Win32 COM related error occured. COM failed to initialize or a QueryInterface failed meaning a Windows codec or driver was not installed properly. */
        ERR_DMA,                   /* DMA Failure.  See debug output for more information. */
        ERR_DSP_CONNECTION,        /* DSP connection error.  Connection possibly caused a cyclic dependancy. */
        ERR_DSP_FORMAT,            /* DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format. */
        ERR_DSP_NOTFOUND,          /* DSP connection error.  Couldn't find the DSP unit specified. */
        ERR_DSP_RUNNING,           /* DSP error.  Cannot perform this operation while the network is in the middle of running.  This will most likely happen if a connection or disconnection is attempted in a DSP callback. */
        ERR_DSP_TOOMANYCONNECTIONS,/* DSP connection error.  The unit being connected to or disconnected should only have 1 input or output. */
        ERR_FILE_BAD,              /* Error loading file. */
        ERR_FILE_COULDNOTSEEK,     /* Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format. */
        ERR_FILE_EOF,              /* End of file unexpectedly reached while trying to read essential data (truncated data?). */
        ERR_FILE_NOTFOUND,         /* File not found. */
        ERR_FILE_UNWANTED,         /* Unwanted file access occured. */
        ERR_FORMAT,                /* Unsupported file or audio format. */
        ERR_HTTP,                  /* A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere. */
        ERR_HTTP_ACCESS,           /* The specified resource requires authentication or is forbidden. */
        ERR_HTTP_PROXY_AUTH,       /* Proxy authentication is required to access the specified resource. */
        ERR_HTTP_SERVER_ERROR,     /* A HTTP server error occurred. */
        ERR_HTTP_TIMEOUT,          /* The HTTP request timed out. */
        ERR_INITIALIZATION,        /* FMOD was not initialized correctly to support this function. */
        ERR_INITIALIZED,           /* Cannot call this command after System::init. */
        ERR_INTERNAL,              /* An error occured that wasnt supposed to.  Contact support. */
        ERR_INVALID_ADDRESS,       /* On Xbox 360, this memory address passed to FMOD must be physical, (ie allocated with XPhysicalAlloc.) */
        ERR_INVALID_FLOAT,         /* Value passed in was a NaN, Inf or denormalized float. */
        ERR_INVALID_HANDLE,        /* An invalid object handle was used. */
        ERR_INVALID_PARAM,         /* An invalid parameter was passed to this function. */
        ERR_INVALID_SPEAKER,       /* An invalid speaker was passed to this function based on the current speaker mode. */
        ERR_IRX,                   /* PS2 only.  fmodex.irx failed to initialize.  This is most likely because you forgot to load it. */
        ERR_MEMORY,                /* Not enough memory or resources. */
        ERR_MEMORY_IOP,            /* PS2 only.  Not enough memory or resources on PlayStation 2 IOP ram. */
        ERR_MEMORY_SRAM,           /* Not enough memory or resources on console sound ram. */
        ERR_MEMORY_CANTPOINT,      /* Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used. */
        ERR_NEEDS2D,               /* Tried to call a command on a 3d sound when the command was meant for 2d sound. */
        ERR_NEEDS3D,               /* Tried to call a command on a 2d sound when the command was meant for 3d sound. */
        ERR_NEEDSHARDWARE,         /* Tried to use a feature that requires hardware support.  (ie trying to play a VAG compressed sound in software on PS2). */
        ERR_NEEDSSOFTWARE,         /* Tried to use a feature that requires the software engine.  Software engine has either been turned off, or command was executed on a hardware channel which does not support this feature. */
        ERR_NET_CONNECT,           /* Couldn't connect to the specified host. */
        ERR_NET_SOCKET_ERROR,      /* A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere. */
        ERR_NET_URL,               /* The specified URL couldn't be resolved. */
        ERR_NOTREADY,              /* Operation could not be performed because specified sound is not ready. */
        ERR_OUTPUT_ALLOCATED,      /* Error initializing output device, but more specifically, the output device is already in use and cannot be reused. */
        ERR_OUTPUT_CREATEBUFFER,   /* Error creating hardware sound buffer. */
        ERR_OUTPUT_DRIVERCALL,     /* A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted. */
        ERR_OUTPUT_FORMAT,         /* Soundcard does not support the minimum features needed for this soundsystem (16bit stereo output). */
        ERR_OUTPUT_INIT,           /* Error initializing output device. */
        ERR_OUTPUT_NOHARDWARE,     /* FMOD_HARDWARE was specified but the sound card does not have the resources nescessary to play it. */
        ERR_OUTPUT_NOSOFTWARE,     /* Attempted to create a software sound but no software channels were specified in System::init. */
        ERR_PAN,                   /* Panning only works with mono or stereo sound sources. */
        ERR_PLUGIN,                /* An unspecified error has been returned from a 3rd party plugin. */
        ERR_PLUGIN_MISSING,        /* A requested output, dsp unit type or codec was not available. */
        ERR_PLUGIN_RESOURCE,       /* A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback) */
        ERR_RECORD,                /* An error occured trying to initialize the recording device. */
        ERR_REVERB_INSTANCE,       /* Specified Instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because another application has locked the EAX4 FX slot. */
        ERR_SUBSOUNDS,             /* The error occured because the sound referenced contains subsounds.  (ie you cannot play the parent sound as a static sample, only its subsounds.) */
        ERR_SUBSOUND_ALLOCATED,    /* This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first. */
        ERR_TAGNOTFOUND,           /* The specified tag could not be found or there are no tags. */
        ERR_TOOMANYCHANNELS,       /* The sound created exceeds the allowable input channel count.  This can be increased with System::setMaxInputChannels. */
        ERR_UNIMPLEMENTED,         /* Something in FMOD hasn't been implemented when it should be! contact support! */
        ERR_UNINITIALIZED,         /* This command failed because System::init or System::setDriver was not called. */
        ERR_UNSUPPORTED,           /* A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified. */
        ERR_UPDATE,                /* On PS2, System::update was called twice in a row when System::updateFinished must be called first. */
        ERR_VERSION,               /* The version number of this file format is not supported. */
    }

   /*
    [ENUM] 
    [
        [DESCRIPTION]   
        Initialization flags.  Use them with System::init in the flags parameter to change various behaviour.  

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]
        System::init
    ]
    */
   public enum INITFLAG
   {
      NORMAL                  = 0x00000000,   /* All platforms - Initialize normally */
      STREAM_FROM_UPDATE      = 0x00000001,   /* All platforms - No stream thread is created internally.  Streams are driven from System::update.  Mainly used with non-realtime outputs. */
      _3D_RIGHTHANDED         = 0x00000002,   /* All platforms - FMOD will treat +X as left, +Y as up and +Z as forwards. */
      DISABLESOFTWARE         = 0x00000004,   /* All platforms - Disable software mixer to save memory.  Anything created with FMOD_SOFTWARE will fail and DSP will not work. */
      DSOUND_HRTFNONE         = 0x00000200,   /* Win32 only - for DirectSound output - FMOD_HARDWARE | FMOD_3D buffers use simple stereo panning/doppler/attenuation when 3D hardware acceleration is not present. */
      DSOUND_HRTFLIGHT        = 0x00000400,   /* Win32 only - for DirectSound output - FMOD_HARDWARE | FMOD_3D buffers use a slightly higher quality algorithm when 3D hardware acceleration is not present. */
      DSOUND_HRTFFULL         = 0x00000800,   /* Win32 only - for DirectSound output - FMOD_HARDWARE | FMOD_3D buffers use full quality 3D playback when 3d hardware acceleration is not present. */
      PS2_DISABLECORE0REVERB  = 0x00010000,   /* PS2 only - Disable reverb on CORE 0 to regain SRAM. */
      PS2_DISABLECORE1REVERB  = 0x00020000,   /* PS2 only - Disable reverb on CORE 1 to regain SRAM. */
      XBOX_REMOVEHEADROOM     = 0x00100000,   /* XBox only - By default DirectSound attenuates all sound by 6db to avoid clipping/distortion.  CAUTION.  If you use this flag you are responsible for the final mix to make sure clipping / distortion doesn't happen. */
   }

   /*
    [ENUM]
    [
        [DESCRIPTION]   
        These definitions describe the type of song being played.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]      
        Sound::getFormat
    ]
    */
   public enum SOUND_TYPE
   {
      UNKNOWN,    /* 3rd party / unknown plugin format. */
      AAC,        /* AAC */
      AIFF,       /* AIFF */
      ASF,        /* Microsoft Advanced Systems Format (ie WMA/ASF/WMV) */
      AT3,        /* Sony ATRAC 3 format */
      CDDA,       /* Digital CD audio */
      DLS,        /* Sound font / downloadable sound bank. */
      FLAC,       /* FLAC lossless codec. */
      FSB,        /* FMOD Sample Bank */
      GCADPCM,    /* GameCube ADPCM */
      IT,         /* Impulse Tracker. */
      MIDI,       /* MIDI */
      MOD,        /* Protracker / Fasttracker MOD. */
      MPEG,       /* MP2/MP3 MPEG. */
      OGGVORBIS,  /* Ogg vorbis. */
      PLAYLIST,   /* Information only from ASX/PLS/M3U/WAX playlists */
      RAW,        /* Raw PCM data. */
      S3M,        /* ScreamTracker 3. */
      SF2,        /* Sound font 2 format. */
      USER,       /* User created sound */
      WAV,        /* Microsoft WAV. */
      XM,         /* FastTracker 2 XM. */
      XMA,        /* Xbox360 XMA */
      VAG         /* PlayStation 2 / PlayStation Portable adpcm VAG format. */
   }
   
   /*
    [ENUM]
    [
        [DESCRIPTION]   
        These definitions describe the native format of the hardware or software buffer that will be used.

        [REMARKS]
        This is the format the native hardware or software buffer will be or is created in.

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]
        System::createSoundEx
        Sound::getFormat
    ]
    */
   public enum SOUND_FORMAT
   {
      NONE,     /* Unitialized / unknown */
      PCM8,     /* 8bit integer PCM data */
      PCM16,    /* 16bit integer PCM data  */
      PCM24,    /* 24bit integer PCM data  */
      PCM32,    /* 32bit integer PCM data  */
      PCMFLOAT, /* 32bit floating point PCM data  */
      GCADPCM,  /* Compressed GameCube DSP data */
      IMAADPCM, /* Compressed XBox ADPCM data */
      VAG,      /* Compressed PlayStation 2 ADPCM data */
      XMA,      /* Compressed Xbox360 data. */
      MPEG,     /* Compressed MPEG layer 2 or 3 data. */
      MAX       /* Maximum number of sound formats supported. */ 
   }

   /*
    [DEFINE]
    [
        [NAME] 
        FMOD_MODE

        [DESCRIPTION]   
        Sound description bitfields, bitwise OR them together for loading and describing sounds.

        [REMARKS]
        By default a sound will open as a static sound that is decompressed fully into memory.<br>
        To have a sound stream instead, use FMOD_CREATESTREAM.<br>
        Some opening modes (ie FMOD_OPENUSER, FMOD_OPENMEMORY, FMOD_OPENRAW) will need extra information.<br>
        This can be provided using the FMOD_CREATESOUNDEXINFO structure.

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]
        System::createSound
        System::createStream
        Sound::setMode
        Sound::getMode
        Channel::setMode
        Channel::getMode
        Sound::set3DCustomRolloff
        Channel::set3DCustomRolloff
    ]
    */
    public enum MODE
    {
        DEFAULT                = 0x00000000,  /* FMOD_DEFAULT is a default sound type.  Equivalent to all the defaults listed below.  FMOD_LOOP_OFF, FMOD_2D, FMOD_HARDWARE. */
        LOOP_OFF               = 0x00000001,  /* For non looping sounds. (default).  Overrides FMOD_LOOP_NORMAL / FMOD_LOOP_BIDI. */
        LOOP_NORMAL            = 0x00000002,  /* For forward looping sounds. */
        LOOP_BIDI              = 0x00000004,  /* For bidirectional looping sounds. (only works on software mixed static sounds). */
        _2D                    = 0x00000008,  /* Ignores any 3d processing. (default). */
        _3D                    = 0x00000010,  /* Makes the sound positionable in 3D.  Overrides FMOD_2D. */
        HARDWARE               = 0x00000020,  /* Attempts to make sounds use hardware acceleration. (default). */
        SOFTWARE               = 0x00000040,  /* Makes sound reside in software.  Overrides FMOD_HARDWARE.  Use this for FFT, DSP, 2D multi speaker support and other software related features. */
        CREATESTREAM           = 0x00000080,  /* Decompress at runtime, streaming from the source provided (standard stream).  Overrides FMOD_CREATESAMPLE. */
        CREATESAMPLE           = 0x00000100,  /* Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format. (standard sample). */
        CREATECOMPRESSEDSAMPLE = 0x00000200,  /* Load MP2, MP3, IMAADPCM or XMA into memory and leave it compressed.  During playback the FMOD software mixer will decode it in realtime as a 'compressed sample'.  Can only be used in combination with FMOD_SOFTWARE. */
        OPENUSER               = 0x00000400,  /* Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.  If a user created 'sample' is created with no read callback, the sample will be empty.  Use FMOD_Sound_Lock and FMOD_Sound_Unlock to place sound data into the sound if this is the case. */
        OPENMEMORY             = 0x00000800,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds. */
        OPENRAW                = 0x00001000,  /* Will ignore file format and treat as raw pcm.  User may need to declare if data is FMOD_SIGNED or FMOD_UNSIGNED */
        OPENONLY               = 0x00002000,  /* Just open the file, dont prebuffer or read.  Good for fast opens for info, or when sound::readData is to be used. */
        ACCURATETIME           = 0x00004000,  /* For FMOD_CreateSound - for accurate FMOD_Sound_GetLength / FMOD_Channel_SetPosition on VBR MP3, AAC and MOD/S3M/XM/IT/MIDI files.  Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this. */
        MPEGSEARCH             = 0x00008000,  /* For corrupted / bad MP3 files.  This will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k. */
        NONBLOCKING            = 0x00010000,  /* For opening sounds asyncronously, return value from open function must be polled for when it is ready. */
        UNIQUE                 = 0x00020000,  /* Unique sound, can only be played one at a time */
        _3D_HEADRELATIVE       = 0x00040000,  /* Make the sound's position, velocity and orientation relative to the listener. */
        _3D_WORLDRELATIVE      = 0x00080000,  /* Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT) */
        _3D_LOGROLLOFF         = 0x00100000,  /* This sound will follow the standard logarithmic rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (default) */
        _3D_LINEARROLLOFF      = 0x00200000,  /* This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence.  */
        _3D_CUSTOMROLLOFF      = 0x04000000,  /* This sound will follow a rolloff model defined by Sound::set3DCustomRolloff / Channel::set3DCustomRolloff.  */
        CDDA_FORCEASPI         = 0x00400000,  /* For CDDA sounds only - use ASPI instead of NTSCSI to access the specified CD/DVD device. */
        CDDA_JITTERCORRECT     = 0x00800000,  /* For CDDA sounds only - perform jitter correction. Jitter correction helps produce a more accurate CDDA stream at the cost of more CPU time. */
        UNICODE                = 0x01000000,  /* Filename is double-byte unicode. */
        IGNORETAGS             = 0x02000000,  /* Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance). */
        LOWMEM                 = 0x08000000,  /* Removes some features from samples to give a lower memory overhead, like Sound::getName. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These callback types are used with Channel::setCallback.

        [REMARKS]
        Each callback has commanddata parameters passed int unique to the type of callback.
        See reference to FMOD_CHANNEL_CALLBACK to determine what they might mean for each type of callback.

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]      
        Channel::setCallback
        FMOD_CHANNEL_CALLBACK
    ]
    */
    public enum CHANNEL_CALLBACKTYPE
    {
        END,                  /* Called when a sound ends. */
        VIRTUALVOICE,         /* Called when a voice is swapped out or swapped in. */
        SYNCPOINT,            /* Called when a syncpoint is encountered.  Can be from wav file markers. */

        MAX
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of time types that can be returned by Sound::getLength and used with Channel::setPosition or Channel::getPosition.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]      
        Sound::getLength
        Channel::setPosition
        Channel::getPosition
    ]
    */
    public enum TIMEUNIT
    {
        MS                = 0x00000001,  /* Milliseconds. */
        PCM               = 0x00000002,  /* PCM Samples, related to milliseconds * samplerate / 1000. */
        PCMBYTES          = 0x00000004,  /* Bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        RAWBYTES          = 0x00000008,  /* Raw file bytes of (compressed) sound data (does not include headers).  Only used by Sound::getLength and Channel::getPosition. */
        MODORDER          = 0x00000100,  /* MOD/S3M/XM/IT.  Order in a sequenced module format.  Use Sound::getFormat to determine the format. */
        MODROW            = 0x00000200,  /* MOD/S3M/XM/IT.  Current row in a sequenced module format.  Sound::getLength will return the number if rows in the currently playing or seeked to pattern. */
        MODPATTERN        = 0x00000400,  /* MOD/S3M/XM/IT.  Current pattern in a sequenced module format.  Sound::getLength will return the number of patterns in the song and Channel::getPosition will return the currently playing pattern. */
        SENTENCE_MS       = 0x00010000,  /* Currently playing subsound in a sentence time in milliseconds. */
        SENTENCE_PCM      = 0x00020000,  /* Currently playing subsound in a sentence time in PCM Samples, related to milliseconds * samplerate / 1000. */
        SENTENCE_PCMBYTES = 0x00040000,  /* Currently playing subsound in a sentence time in bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        SENTENCE          = 0x00080000,  /* Currently playing sentence index according to the channel. */
        SENTENCE_SUBSOUND = 0x00100000,  /* Currently playing subsound index in a sentence. */
        BUFFERED          = 0x10000000,  /* Time value as seen by buffered stream.  This is always ahead of audible time, and is only used for processing. */
    }

    public delegate RESULT SOUND_NONBLOCKCALLBACK (IntPtr soundraw, RESULT result);
    public delegate RESULT SOUND_PCMREADCALLBACK  (IntPtr soundraw, IntPtr data, uint datalen);
    public delegate RESULT SOUND_PCMSETPOSCALLBACK(IntPtr soundraw, int subsound, uint position, TIMEUNIT postype);


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Use this structure with System::createSound when more control is needed over loading.
        The possible reasons to use this with System::createSound are:
        <li>Loading a file from memory.
        <li>Loading a file from within another larger (possibly wad/pak) file, by giving the loader an offset and length.
        <li>To create a user created / non file based sound.
        <li>To specify a starting subsound to seek to within a multi-sample sounds (ie FSB/DLS/SF2) when created as a stream.
        <li>To specify which subsounds to load for multi-sample sounds (ie FSB/DLS/SF2) so that memory is saved and only a subset is actually loaded/read from disk.
        <li>To specify 'piggyback' read and seek callbacks for capture of sound data as fmod reads and decodes it.  Useful for ripping decoded PCM data from sounds as they are loaded / played.
        <li>To specify a MIDI DLS/SF2 sample set file to load when opening a MIDI file.
        See below on what members to fill for each of the above types of sound you want to create.

        [REMARKS]
        This structure is optional!  Specify 0 or NULL in System::createSound if you don't need it!
        
        Members marked with [in] mean the user sets the value before passing it to the function.
        Members marked with [out] mean FMOD sets the value to be used after the function exits.
        
        <u>Loading a file from memory.</u>
        <li>Create the sound using the FMOD_OPENMEMORY flag.
        <li>Mandantory.  Specify 'length' for the size of the memory block in bytes.
        <li>Other flags are optional.
        
        
        <u>Loading a file from within another larger (possibly wad/pak) file, by giving the loader an offset and length.</u>
        <li>Mandantory.  Specify 'fileoffset' and 'length'.
        <li>Other flags are optional.
        
        
        <u>To create a user created / non file based sound.</u>
        <li>Create the sound using the FMOD_OPENUSER flag.
        <li>Mandantory.  Specify 'defaultfrequency, 'numchannels' and 'format'.
        <li>Other flags are optional.
        
        
        <u>To specify a starting subsound to seek to and flush with, within a multi-sample stream (ie FSB/DLS/SF2).</u>
        
        <li>Mandantory.  Specify 'initialsubsound'.
        
        
        <u>To specify which subsounds to load for multi-sample sounds (ie FSB/DLS/SF2) so that memory is saved and only a subset is actually loaded/read from disk.</u>
        
        <li>Mandantory.  Specify 'inclusionlist' and 'inclusionlistnum'.
        
        
        <u>To specify 'piggyback' read and seek callbacks for capture of sound data as fmod reads and decodes it.  Useful for ripping decoded PCM data from sounds as they are loaded / played.</u>
        
        <li>Mandantory.  Specify 'pcmreadcallback' and 'pcmseekcallback'.
        
        
        <u>To specify a MIDI DLS/SF2 sample set file to load when opening a MIDI file.</u>
        
        <li>Mandantory.  Specify 'dlsname'.
        
        
        Setting the 'decodebuffersize' is for cpu intensive codecs that may be causing stuttering, not file intensive codecs (ie those from CD or netstreams) which are normally altered with System::setStreamBufferSize.  As an example of cpu intensive codecs, an mp3 file will take more cpu to decode than a PCM wav file.
        If you have a stuttering effect, then it is using more cpu than the decode buffer playback rate can keep up with.  Increasing the decode buffersize will most likely solve this problem.

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]
        System::createSound
        System::setStreamBufferSize
        FMOD_MODE
    ]
    */
    public struct CREATESOUNDEXINFO
    {
        public int                         cbsize;                 /* [in] Size of this structure.  This is used so the structure can be expanded in the future and still work on older versions of FMOD Ex. */
        public uint                        length;                 /* [in] Optional. Specify 0 to ignore. Size in bytes of file to load, or sound to create (in this case only if FMOD_OPENUSER is used).  Required if loading from memory.  If 0 is specified, then it will use the size of the file (unless loading from memory then an error will be returned). */
        public uint                        fileoffset;             /* [in] Optional. Specify 0 to ignore. Offset from start of the file to start loading from.  This is useful for loading files from inside big data files. */
        public int                         numchannels;            /* [in] Optional. Specify 0 to ignore. Number of channels in a sound specified only if OPENUSER is used. */
        public int                         defaultfrequency;       /* [in] Optional. Specify 0 to ignore. Default frequency of sound in a sound specified only if OPENUSER is used.  Other formats use the frequency determined by the file format. */
        public SOUND_FORMAT                format;                 /* [in] Optional. Specify 0 or SOUND_FORMAT_NONE to ignore. Format of the sound specified only if OPENUSER is used.  Other formats use the format determined by the file format.   */
        public uint                        decodebuffersize;       /* [in] Optional. Specify 0 to ignore. For streams.  This determines the size of the double buffer (in PCM samples) that a stream uses.  Use this for user created streams if you want to determine the size of the callback buffer passed to you.  Specify 0 to use FMOD's default size which is currently equivalent to 400ms of the sound format created/loaded. */
        public int                         initialsubsound;        /* [in] Optional. Specify 0 to ignore. In a multi-sample file format such as .FSB/.DLS/.SF2, specify the initial subsound to seek to, only if CREATESTREAM is used. */
        public int                         numsubsounds;           /* [in] Optional. Specify 0 to ignore or have no subsounds.  In a user created multi-sample sound, specify the number of subsounds within the sound that are accessable with Sound::getSubSound / SoundGetSubSound. */
        public IntPtr                      inclusionlist;          /* [in] Optional. Specify 0 to ignore. In a multi-sample format such as .FSB/.DLS/.SF2 it may be desirable to specify only a subset of sounds to be loaded out of the whole file.  This is an array of subsound indicies to load into memory when created. */
        public int                         inclusionlistnum;       /* [in] Optional. Specify 0 to ignore. This is the number of integers contained within the */
        public SOUND_PCMREADCALLBACK       pcmreadcallback;        /* [in] Optional. Specify 0 to ignore. Callback to 'piggyback' on FMOD's read functions and accept or even write PCM data while FMOD is opening the sound.  Used for user sounds created with OPENUSER or for capturing decoded data as FMOD reads it. */
        public SOUND_PCMSETPOSCALLBACK     pcmsetposcallback;      /* [in] Optional. Specify 0 to ignore. Callback for when the user calls a seeking function such as Channel::setPosition within a multi-sample sound, and for when it is opened.*/
        public SOUND_NONBLOCKCALLBACK      nonblockcallback;       /* [in] Optional. Specify 0 to ignore. Callback for successful completion, or error while loading a sound that used the FMOD_NONBLOCKING flag.*/
        public string                      dlsname;                /* [in] Optional. Specify 0 to ignore. Filename for a DLS or SF2 sample set when loading a MIDI file.   If not specified, on windows it will attempt to open /windows/system32/drivers/gm.dls, otherwise the MIDI will fail to open.  */
        public string                      encryptionkey;          /* [in] Optional. Specify 0 to ignore. Key for encrypted FSB file.  Without this key an encrypted FSB file will not load. */
        public int                         maxpolyphony;           /* [in] Optional. Specify 0 to ingore. For sequenced formats with dynamic channel allocation such as .MID and .IT, this specifies the maximum voice count allowed while playing.  .IT defaults to 64.  .MID defaults to 32. */
        public IntPtr                      userdata;               /* [in] Optional. Specify 0 to ignore. This is user data to be attached to the sound during creation.  Access via Sound::getUserData. */
        public SOUND_TYPE                  suggestedsoundtype;     /* [in] Optional. Specify 0 or FMOD_SOUND_TYPE_UNKNOWN to ignore.  Instead of scanning all codec types, use this to speed up loading by making it jump straight to this codec. */
    }

    /*
    [ENUM] 
    [
        [NAME] 
        FMOD_MISC_VALUES

        [DESCRIPTION]
        Miscellaneous values for FMOD functions.

        [PLATFORMS]
        Win32, Win64, Linux, Macintosh, Xbox, Xbox360, PlayStation 2, GameCube, PlayStation Portable

        [SEE_ALSO]
        System::playSound
        System::playDSP
        System::getChannel
    ]
    */
    public enum CHANNELINDEX
    {
        FREE   = -1,     /* For a channel index, FMOD chooses a free voice using the priority system. */
        REUSE  = -2      /* For a channel index, re-use the channel handle that was passed in. */
    }


    /* 
        FMOD Callbacks
    */
    public delegate RESULT CHANNEL_CALLBACK      (IntPtr channelraw, CHANNEL_CALLBACKTYPE type, int command, uint commanddata1, uint commanddata2);

    public delegate RESULT FILE_OPENCALLBACK     (string name, int unicode, ref uint filesize, ref IntPtr handle, ref IntPtr userdata);
    public delegate RESULT FILE_CLOSECALLBACK    (IntPtr handle, IntPtr userdata);
    public delegate RESULT FILE_READCALLBACK     (IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata);
    public delegate RESULT FILE_SEEKCALLBACK     (IntPtr handle, int pos, IntPtr userdata);


    /*
        FMOD System factory functions.  Use this to create an FMOD System Instance.  below you will see System_Init/Close to get started.
    */
    public class Factory
    {        
        public static RESULT System_Create(ref System system)
        {
            RESULT result      = RESULT.OK;
            IntPtr      systemraw   = new IntPtr();
            System      systemnew   = null;

            result = FMOD_System_Create(ref systemraw);
            if (result != RESULT.OK)
            {
                return result;
            }

            systemnew = new System();
            systemnew.setRaw(systemraw);
            system = systemnew;

            return result;
        }


        #region importfunctions
  
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Create                      (ref IntPtr system);

        #endregion
    }


    /*
        'System' API
    */
    public class System
    {
        public RESULT release                ()
        {
            return FMOD_System_Release(systemraw);
        }


        // Pre-init functions.
        public RESULT getNumDrivers          (ref int numdrivers)
        {
            return FMOD_System_GetNumDrivers(systemraw, ref numdrivers);
        }
        public RESULT getDriverName          (int id, StringBuilder name, int namelen)
        {
            return FMOD_System_GetDriverName(systemraw, id, name, namelen);
        }
        public RESULT setDriver              (int driver)
        {
            return FMOD_System_SetDriver(systemraw, driver);
        }
        public RESULT getDriver              (ref int driver)
        {
            return FMOD_System_GetDriver(systemraw, ref driver);
        }
        public RESULT setHardwareChannels    (int min2d, int max2d, int min3d, int max3d)
        {
            return FMOD_System_SetHardwareChannels(systemraw, min2d, max2d, min3d, max3d);
        }
        public RESULT getHardwareChannels    (ref int num2d, ref int num3d, ref int total)
        {
            return FMOD_System_GetHardwareChannels(systemraw, ref num2d, ref num3d, ref total);
        }
        public RESULT setSoftwareChannels    (int numsoftwarechannels)
        {
            return FMOD_System_SetSoftwareChannels(systemraw, numsoftwarechannels);
        }
        public RESULT getSoftwareChannels    (ref int numsoftwarechannels)
        {
            return FMOD_System_GetSoftwareChannels(systemraw, ref numsoftwarechannels);
        }

       // Init/Close 
        public RESULT init                   (int maxchannels, INITFLAG flags, IntPtr extradata)
        {
            return FMOD_System_Init(systemraw, maxchannels, flags, extradata);
        }
        public RESULT close                  ()
        {
            return FMOD_System_Close(systemraw);
        }


        // General post-init system functions
        public RESULT update                 ()
        {
            return FMOD_System_Update(systemraw);
        }
        public RESULT updateFinished()
        {
            return FMOD_System_UpdateFinished(systemraw);
        }

        // System information functions.
        public RESULT getVersion             (ref uint version)
        {
            return FMOD_System_GetVersion(systemraw, ref version);
        }
        public RESULT getOutputHandle        (ref IntPtr handle)
        {
            return FMOD_System_GetOutputHandle(systemraw, ref handle);
        }
        public RESULT getChannelsPlaying     (ref int channels)
        {
            return FMOD_System_GetChannelsPlaying(systemraw, ref channels);
        }

       // Sound/DSP/Channel creation and retrieval. 
        public RESULT createSound            (string name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateSound(systemraw, name_or_data, mode, ref exinfo, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT createSound            (byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateSound(systemraw, data, mode, ref exinfo, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT createSound            (string name_or_data, MODE mode, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateSound(systemraw, name_or_data, mode, 0, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT createStream            (string name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateStream(systemraw, name_or_data, mode, ref exinfo, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT createStream            (byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateStream(systemraw, data, mode, ref exinfo, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT createStream            (string name_or_data, MODE mode, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            try
            {
                result = FMOD_System_CreateStream(systemraw, name_or_data, mode, 0, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }
        public RESULT playSound              (CHANNELINDEX channelid, Sound sound, bool paused, ref Channel channel)
        {
            RESULT result      = RESULT.OK;
            IntPtr      channelraw;
            Channel     channelnew  = null;

            if (channel != null)
            {
                channelraw = channel.getRaw();
            }
            else
            {
                channelraw  = new IntPtr();
            }

            try
            {
                result = FMOD_System_PlaySound(systemraw, channelid, sound.getRaw(), paused, ref channelraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (channel == null)
            {
                channelnew = new Channel();
                channelnew.setRaw(channelraw);
                channel = channelnew;
            }
            else
            {
                channel.setRaw(channelraw);
            }
                             
            return result;                                                                    
        }
        public RESULT getChannel             (int channelid, ref Channel channel)
        {
            RESULT result      = RESULT.OK;
            IntPtr      channelraw  = new IntPtr();
            Channel     channelnew  = null;

            try
            {
                result = FMOD_System_GetChannel(systemraw, channelid, ref channelraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (channel == null)
            {
                channelnew = new Channel();
                channelnew.setRaw(channelraw);
                channel = channelnew;
            }
            else
            {
                channel.setRaw(channelraw);
            }

            return result;
        }

       #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Release                (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetNumDrivers          (IntPtr system, ref int numdrivers);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetDriverName          (IntPtr system, int id, StringBuilder name, int namelen);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetDriver              (IntPtr system, int driver);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetDriver              (IntPtr system, ref int driver);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetHardwareChannels    (IntPtr system, int min2d, int max2d, int min3d, int max3d);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetHardwareChannels    (IntPtr system, ref int num2d, ref int num3d, ref int total);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetSoftwareChannels    (IntPtr system, int numsoftwarechannels);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetSoftwareChannels    (IntPtr system, ref int numsoftwarechannels);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Init                   (IntPtr system, int maxchannels, INITFLAG flags, IntPtr extradata);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Close                  (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Update                 (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_UpdateFinished         (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetVersion             (IntPtr system, ref uint version);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetOutputHandle        (IntPtr system, ref IntPtr handle);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetChannelsPlaying     (IntPtr system, ref int channels);
        [DllImport (VERSION.dll)]   
        private static extern RESULT FMOD_System_CreateSound            (IntPtr system, string name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_CreateStream           (IntPtr system, string name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]   
        private static extern RESULT FMOD_System_CreateSound            (IntPtr system, string name_or_data, MODE mode, int exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_CreateStream           (IntPtr system, string name_or_data, MODE mode, int exinfo, ref IntPtr sound);   
        [DllImport (VERSION.dll)]   
        private static extern RESULT FMOD_System_CreateSound            (IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_CreateStream           (IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]   
        private static extern RESULT FMOD_System_CreateSound            (IntPtr system, byte[] name_or_data, MODE mode, int exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_CreateStream           (IntPtr system, byte[] name_or_data, MODE mode, int exinfo, ref IntPtr sound);
        [DllImport (VERSION.dll)]                 
        private static extern RESULT FMOD_System_PlaySound              (IntPtr system, CHANNELINDEX channelid, IntPtr sound, bool paused, ref IntPtr channel);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetChannel             (IntPtr system, int channelid, ref IntPtr channel);
        #endregion

        #region wrapperinternal
        
        private IntPtr systemraw;

        public void setRaw(IntPtr system)
        {
            systemraw = new IntPtr();

            systemraw = system;
        }

        public IntPtr getRaw()
        {
            return systemraw;
        }

        #endregion
    }
    

    /*
        'Sound' API
    */
    public class Sound
    {
        public RESULT release                 ()
        {
            return FMOD_Sound_Release(soundraw);
        }
        public RESULT getSystemObject         (ref System system)
        {
            RESULT result   = RESULT.OK;
            IntPtr systemraw   = new IntPtr();
            System systemnew   = null;

            try
            {
                result = FMOD_Sound_GetSystemObject(soundraw, ref systemraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (system == null)
            {
                systemnew = new System();
                systemnew.setRaw(systemraw);
                system = systemnew;
            }
            else
            {
                system.setRaw(systemraw);
            }
            return result;  
        }
                     

        public RESULT @lock                   (uint offset, uint length, ref IntPtr ptr1, ref IntPtr ptr2, ref uint len1, ref uint len2)
        {
            return FMOD_Sound_Lock(soundraw, offset, length, ref ptr1, ref ptr2, ref len1, ref len2);
        }
        public RESULT unlock                  (IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2)
        {
            return FMOD_Sound_Unlock(soundraw, ptr1, ptr2, len1, len2);
        }
        public RESULT setDefaults             (float frequency, float volume, float pan, int priority)
        {
            return FMOD_Sound_SetDefaults(soundraw, frequency, volume, pan, priority);
        }
        public RESULT getDefaults             (ref float frequency, ref float volume, ref float pan, ref int priority)
        {
            return FMOD_Sound_GetDefaults(soundraw, ref frequency, ref volume, ref pan, ref priority);
        }
        public RESULT setSubSound             (int index, Sound subsound)
        {
            IntPtr subsoundraw = subsound.getRaw();

            return FMOD_Sound_SetSubSound(soundraw, index, subsoundraw);
        }
        public RESULT getSubSound             (int index, ref Sound subsound)
        {
            RESULT result       = RESULT.OK;
            IntPtr subsoundraw  = new IntPtr();
            Sound  subsoundnew  = null;

            try
            {
                result = FMOD_Sound_GetSubSound(soundraw, index, ref subsoundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (subsound == null)
            {
                subsoundnew = new Sound();
                subsoundnew.setRaw(subsoundraw);
                subsound = subsoundnew;
            }
            else
            {
                subsound.setRaw(subsoundraw);
            }

            return result;
        }
        public RESULT getName                 (StringBuilder name, int namelen)
        {
            return FMOD_Sound_GetName(soundraw, name, namelen);
        }
        public RESULT getLength               (ref uint length, TIMEUNIT lengthtype)
        {
            return FMOD_Sound_GetLength(soundraw, ref length, lengthtype);
        }
        public RESULT getFormat               (ref SOUND_TYPE type, ref SOUND_FORMAT format, ref int channels, ref int bits)
        {
            return FMOD_Sound_GetFormat(soundraw, ref type, ref format, ref channels, ref bits);
        }
        public RESULT getNumSubSounds         (ref int numsubsounds)
        {
            return FMOD_Sound_GetNumSubSounds(soundraw, ref numsubsounds);
        }
        public RESULT getNumTags              (ref int numtags, ref int numtagsupdated)
        {
            return FMOD_Sound_GetNumTags(soundraw, ref numtags, ref numtagsupdated);
        }
        public RESULT readData                (IntPtr buffer, uint lenbytes, ref uint read)
        {
            return FMOD_Sound_ReadData(soundraw, buffer, lenbytes, ref read);
        }
        public RESULT seekData                (uint pcm)
        {
            return FMOD_Sound_SeekData(soundraw, pcm);
        }


        public RESULT getNumSyncPoints        (ref int numsyncpoints)
        {
            return FMOD_Sound_GetNumSyncPoints(soundraw, ref numsyncpoints);
        }
        public RESULT getSyncPoint            (int index, ref IntPtr point)
        {
            return FMOD_Sound_GetSyncPoint(soundraw, index, ref point);
        }
        public RESULT getSyncPointInfo        (IntPtr point, StringBuilder name, int namelen, ref uint offset, TIMEUNIT offsettype)
        {
            return FMOD_Sound_GetSyncPointInfo(soundraw, point, name, namelen, ref offset, offsettype);
        }
        public RESULT addSyncPoint            (int offset, TIMEUNIT offsettype, string name, ref IntPtr point)
        {
            return FMOD_Sound_AddSyncPoint(soundraw, offset, offsettype, name, ref point);
        }
        public RESULT deleteSyncPoint         (IntPtr point)
        {
            return FMOD_Sound_DeleteSyncPoint(soundraw, point);
        }


        public RESULT setMode                 (MODE mode)
        {
            return FMOD_Sound_SetMode(soundraw, mode);
        }
        public RESULT getMode                 (ref MODE mode)
        {
            return FMOD_Sound_GetMode(soundraw, ref mode);
        }
        public RESULT setLoopCount            (int loopcount)
        {
            return FMOD_Sound_SetLoopCount(soundraw, loopcount);
        }
        public RESULT getLoopCount            (ref int loopcount)
        {
            return FMOD_Sound_GetLoopCount(soundraw, ref loopcount);
        }
        public RESULT setLoopPoints           (uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
        {
            return FMOD_Sound_SetLoopPoints(soundraw, loopstart, loopstarttype, loopend, loopendtype);
        }
        public RESULT getLoopPoints           (ref uint loopstart, TIMEUNIT loopstarttype, ref uint loopend, TIMEUNIT loopendtype)
        {
            return FMOD_Sound_GetLoopPoints(soundraw, ref loopstart, loopstarttype, ref loopend, loopendtype);
        }


        #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_Release                 (IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetSystemObject         (IntPtr sound, ref IntPtr system);
        [DllImport (VERSION.dll)]                   
        private static extern RESULT FMOD_Sound_Lock                   (IntPtr sound, uint offset, uint length, ref IntPtr ptr1, ref IntPtr ptr2, ref uint len1, ref uint len2);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_Unlock                  (IntPtr sound, IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SetDefaults             (IntPtr sound, float frequency, float volume, float pan, int priority);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetDefaults             (IntPtr sound, ref float frequency, ref float volume, ref float pan, ref int priority);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SetVariations           (IntPtr sound, float frequencyvar, float volumevar, float panvar);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetVariations           (IntPtr sound, ref float frequencyvar, ref float volumevar, ref float panvar);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SetSubSound             (IntPtr sound, int index, IntPtr subsound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetSubSound             (IntPtr sound, int index, ref IntPtr subsound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetName                 (IntPtr sound, StringBuilder name, int namelen);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetLength               (IntPtr sound, ref uint length, TIMEUNIT lengthtype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetFormat               (IntPtr sound, ref SOUND_TYPE type, ref SOUND_FORMAT format, ref int channels, ref int bits);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetNumSubSounds         (IntPtr sound, ref int numsubsounds);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetNumTags              (IntPtr sound, ref int numtags, ref int numtagsupdated);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_ReadData                (IntPtr sound, IntPtr buffer, uint lenbytes, ref uint read);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SeekData                (IntPtr sound, uint pcm);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetNumSyncPoints        (IntPtr sound, ref int numsyncpoints);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetSyncPoint            (IntPtr sound, int index, ref IntPtr point);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetSyncPointInfo        (IntPtr sound, IntPtr point, StringBuilder name, int namelen, ref uint offset, TIMEUNIT offsettype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_AddSyncPoint            (IntPtr sound, int offset, TIMEUNIT offsettype, string name, ref IntPtr point);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_DeleteSyncPoint         (IntPtr sound, IntPtr point);
        [DllImport (VERSION.dll)]                   
        private static extern RESULT FMOD_Sound_SetMode                 (IntPtr sound, MODE mode);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetMode                 (IntPtr sound, ref MODE mode);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SetLoopCount            (IntPtr sound, int loopcount);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetLoopCount            (IntPtr sound, ref int loopcount);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_SetLoopPoints           (IntPtr sound, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetLoopPoints           (IntPtr sound, ref uint loopstart, TIMEUNIT loopstarttype, ref uint loopend, TIMEUNIT loopendtype);
        [DllImport (VERSION.dll)]                                        
        private static extern RESULT FMOD_Sound_SetUserData             (IntPtr sound, IntPtr userdata);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_GetUserData             (IntPtr sound, ref IntPtr userdata);

        #endregion

        #region wrapperinternal

        private IntPtr soundraw;

        public void setRaw(IntPtr sound)
        {
            soundraw = new IntPtr();
            soundraw = sound;
        }

        public IntPtr getRaw()
        {
            return soundraw;
        }

        #endregion
    }


    /*
        'Channel' API
    */
    public class Channel
    {
        public RESULT getSystemObject       (ref System system)
        {
            RESULT result   = RESULT.OK;
            IntPtr systemraw   = new IntPtr();
            System systemnew   = null;

            try
            {
                result = FMOD_Channel_GetSystemObject(channelraw, ref systemraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (system == null)
            {
                systemnew = new System();
                systemnew.setRaw(systemraw);
                system = systemnew;
            }
            else
            {
                system.setRaw(systemraw);
            }

            return result;  
        }


        public RESULT stop                  ()
        {
            return FMOD_Channel_Stop(channelraw);
        }
        public RESULT setPaused             (bool paused)
        {
            return FMOD_Channel_SetPaused(channelraw, paused);
        }
        public RESULT getPaused             (ref bool paused)
        {
            return FMOD_Channel_GetPaused(channelraw, ref paused);
        }
        public RESULT setVolume             (float volume)
        {
            return FMOD_Channel_SetVolume(channelraw, volume);
        }
        public RESULT getVolume             (ref float volume)
        {
            return FMOD_Channel_GetVolume(channelraw, ref volume);
        }
        public RESULT setFrequency          (float frequency)
        {
            return FMOD_Channel_SetFrequency(channelraw, frequency);
        }
        public RESULT getFrequency          (ref float frequency)
        {
            return FMOD_Channel_GetFrequency(channelraw, ref frequency);
        }
        public RESULT setPan                (float pan)
        {
            return FMOD_Channel_SetPan(channelraw, pan);
        }
        public RESULT getPan                (ref float pan)
        {
            return FMOD_Channel_GetPan(channelraw, ref pan);
        }
        public RESULT setDelay              (uint startdelay, uint enddelay)
        {
            return FMOD_Channel_SetDelay(channelraw, startdelay, enddelay);
        }
        public RESULT getDelay              (ref uint startdelay, ref uint enddelay)
        {
            return FMOD_Channel_GetDelay(channelraw, ref startdelay, ref enddelay);
        }
        public RESULT setSpeakerMix         (float frontleft, float frontright, float center, float lfe, float backleft, float backright, float sideleft, float sideright)
        {
            return FMOD_Channel_SetSpeakerMix(channelraw, frontleft, frontright, center, lfe, backleft, backright, sideleft, sideright);
        }
        public RESULT getSpeakerMix         (ref float frontleft, ref float frontright, ref float center, ref float lfe, ref float backleft, ref float backright, ref float sideleft, ref float sideright)
        {
            return FMOD_Channel_GetSpeakerMix(channelraw, ref frontleft, ref frontright, ref center, ref lfe, ref backleft, ref backright, ref sideleft, ref sideright);
        }
        public RESULT setMute               (bool mute)
        {
            return FMOD_Channel_SetMute(channelraw, mute);
        }
        public RESULT getMute               (ref bool mute)
        {
            return FMOD_Channel_GetMute(channelraw, ref mute);
        }
        public RESULT setPriority           (int priority)
        {
            return FMOD_Channel_SetPriority(channelraw, priority);
        }
        public RESULT getPriority           (ref int priority)
        {
            return FMOD_Channel_GetPriority(channelraw, ref priority);
        }
        public RESULT setPosition           (uint position, TIMEUNIT postype)
        {
            return FMOD_Channel_SetPosition(channelraw, position, postype);
        }
        public RESULT getPosition           (ref uint position, TIMEUNIT postype)
        {
            return FMOD_Channel_GetPosition(channelraw, ref position, postype);
        }
        
        public RESULT setChannelGroup       (ChannelGroup channelgroup)
        {
            return FMOD_Channel_SetChannelGroup(channelraw, channelgroup.getRaw());
        }
        public RESULT getChannelGroup        (ref ChannelGroup channelgroup)
        {
            RESULT result = RESULT.OK;
            IntPtr channelgroupraw = new IntPtr();
            ChannelGroup    channelgroupnew = null;

            try
            {
                result = FMOD_Channel_GetChannelGroup(channelraw, ref channelgroupraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (channelgroup == null)
            {
                channelgroupnew = new ChannelGroup();
                channelgroupnew.setRaw(channelgroupraw);
                channelgroup = channelgroupnew;
            }
            else
            {
                channelgroup.setRaw(channelgroupraw);
            }
                             
            return result; 
        }

        public RESULT setCallback           (CHANNEL_CALLBACKTYPE type, CHANNEL_CALLBACK callback, int command)
        {
            return FMOD_Channel_SetCallback(channelraw, type, callback, command);
        }



        public RESULT isPlaying             (ref bool isplaying)
        {
            return FMOD_Channel_IsPlaying(channelraw, ref isplaying);
        }
        public RESULT isVirtual             (ref bool isvirtual)
        {
            return FMOD_Channel_IsVirtual(channelraw, ref isvirtual);
        }
        public RESULT getAudibility(ref float audibility)
        {
            return FMOD_Channel_GetAudibility(channelraw, ref audibility);
        }
        public RESULT getCurrentSound       (ref Sound sound)
        {
            RESULT result      = RESULT.OK;
            IntPtr soundraw    = new IntPtr();
            Sound  soundnew    = null;

            try
            {
                result = FMOD_Channel_GetCurrentSound(channelraw, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;  
        }
        public RESULT getWaveData           (float[] wavearray, int numvalues, int channeloffset)
        {
            return FMOD_Channel_GetWaveData(channelraw, wavearray, numvalues, channeloffset);
        }
        public RESULT getIndex(ref int index)
        {
            return FMOD_Channel_GetIndex(channelraw, ref index);
        }

        public RESULT setMode               (MODE mode)
        {
            return FMOD_Channel_SetMode(channelraw, mode);
        }
        public RESULT getMode               (ref MODE mode)
        {
            return FMOD_Channel_GetMode(channelraw, ref mode);
        }
        public RESULT setLoopCount          (int loopcount)
        {
            return FMOD_Channel_SetLoopCount(channelraw, loopcount);
        }
        public RESULT getLoopCount          (ref int loopcount)
        {
            return FMOD_Channel_GetLoopCount(channelraw, ref loopcount);
        }
        public RESULT setLoopPoints         (uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
        {
            return FMOD_Channel_SetLoopPoints(channelraw, loopstart, loopstarttype, loopend, loopendtype);
        }
        public RESULT getLoopPoints         (ref uint loopstart, TIMEUNIT loopstarttype, ref uint loopend, TIMEUNIT loopendtype)
        {
            return FMOD_Channel_GetLoopPoints(channelraw, ref loopstart, loopstarttype, ref loopend, loopendtype);
        }


        #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetSystemObject       (IntPtr channel, ref IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_Stop                  (IntPtr channel);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetPaused             (IntPtr channel, bool paused);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetPaused             (IntPtr channel, ref bool paused);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetVolume             (IntPtr channel, float volume);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetVolume             (IntPtr channel, ref float volume);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetFrequency          (IntPtr channel, float frequency);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetFrequency          (IntPtr channel, ref float frequency);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetPan                (IntPtr channel, float pan);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetPan                (IntPtr channel, ref float pan);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetDelay              (IntPtr channel, uint startdelay, uint enddelay);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetDelay              (IntPtr channel, ref uint startdelay, ref uint enddelay);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetSpeakerMix         (IntPtr channel, float frontleft, float frontright, float center, float lfe, float backleft, float backright, float sideleft, float sideright);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetSpeakerMix         (IntPtr channel, ref float frontleft, ref float frontright, ref float center, ref float lfe, ref float backleft, ref float backright, ref float sideleft, ref float sideright);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetMute               (IntPtr channel, bool mute);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetMute               (IntPtr channel, ref bool mute);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetPriority           (IntPtr channel, int priority);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetPriority           (IntPtr channel, ref int priority);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetChannelGroup       (IntPtr channel, IntPtr channelgroup);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetChannelGroup       (IntPtr channel, ref IntPtr channelgroup);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_IsPlaying             (IntPtr channel, ref bool isplaying);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_IsVirtual             (IntPtr channel, ref bool isvirtual);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetAudibility         (IntPtr channel, ref float audibility);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetCurrentSound       (IntPtr channel, ref IntPtr sound);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetWaveData           (IntPtr channel, [MarshalAs(UnmanagedType.LPArray)] float[] wavearray, int numvalues, int channeloffset);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetIndex              (IntPtr channel, ref int index);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetCallback           (IntPtr channel, CHANNEL_CALLBACKTYPE type, CHANNEL_CALLBACK callback, int command);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetPosition           (IntPtr channel, uint position, TIMEUNIT postype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetPosition           (IntPtr channel, ref uint position, TIMEUNIT postype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetDSPHead            (IntPtr channel, ref IntPtr dsp);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetMode               (IntPtr channel, MODE mode);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetMode               (IntPtr channel, ref MODE mode);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetLoopCount          (IntPtr channel, int loopcount);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetLoopCount          (IntPtr channel, ref int loopcount);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetLoopPoints         (IntPtr channel, uint  loopstart, TIMEUNIT loopstarttype, uint  loopend, TIMEUNIT loopendtype);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_GetLoopPoints         (IntPtr channel, ref uint loopstart, TIMEUNIT loopstarttype, ref uint loopend, TIMEUNIT loopendtype);
        #endregion
        
        #region wrapperinternal

        private IntPtr channelraw;

        public void setRaw(IntPtr channel)
        {
            channelraw = new IntPtr();

            channelraw = channel;
        }

        public IntPtr getRaw()
        {
            return channelraw;
        }

        #endregion
    }


    /*
        'ChannelGroup' API
    */
    public class ChannelGroup
    {
        public RESULT release                ()
        {
            return FMOD_ChannelGroup_Release(channelgroupraw);
        }
        public RESULT getSystemObject        (ref System system)
        {
            RESULT result = RESULT.OK;
            IntPtr systemraw = new IntPtr();
            System systemnew = null;

            try
            {
                result = FMOD_ChannelGroup_GetSystemObject(channelgroupraw, ref systemraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (system == null)
            {
                systemnew = new System();
                systemnew.setRaw(systemraw);
                system = systemnew;
            }
            else
            {
                system.setRaw(systemraw);
            }
                             
            return result; 
        }


        // Channelgroup scale values.  (scales the current volume or pitch of all channels and channel groups, DOESN'T overwrite)
        public RESULT setVolume              (float volume)
        {
            return FMOD_ChannelGroup_SetVolume(channelgroupraw, volume);
        }
        public RESULT getVolume              (ref float volume)
        {
            return FMOD_ChannelGroup_GetVolume(channelgroupraw, ref volume);
        }
        public RESULT setPitch               (float pitch)
        {
            return FMOD_ChannelGroup_SetPitch(channelgroupraw, pitch);
        }
        public RESULT getPitch               (ref float pitch)
        {
            return FMOD_ChannelGroup_GetPitch(channelgroupraw, ref pitch);
        }


        // Channelgroup override values.  (recursively overwrites whatever settings the channels had)
        public RESULT stop                   ()
        {
            return FMOD_ChannelGroup_Stop(channelgroupraw);
        }
        public RESULT overridePaused         (bool paused)
        {
            return FMOD_ChannelGroup_OverridePaused(channelgroupraw, paused);
        }
        public RESULT overrideVolume         (float volume)
        {
            return FMOD_ChannelGroup_OverrideVolume(channelgroupraw, volume);
        }
        public RESULT overrideFrequency      (float frequency)
        {
            return FMOD_ChannelGroup_OverrideFrequency(channelgroupraw, frequency);
        }
        public RESULT overridePan            (float pan)
        {
            return FMOD_ChannelGroup_OverridePan(channelgroupraw, pan);
        }
        public RESULT overrideMute           (bool mute)
        {
            return FMOD_ChannelGroup_OverrideMute(channelgroupraw, mute);
        }
        public RESULT overrideSpeakerMix(float frontleft, float frontright, float center, float lfe, float backleft, float backright, float sideleft, float sideright)
        {
            return FMOD_ChannelGroup_OverrideSpeakerMix(channelgroupraw, frontleft, frontright, center, lfe, backleft, backright, sideleft, sideright);
        }


        // Nested channel groups.
        public RESULT addGroup               (ChannelGroup group)
        {
            return FMOD_ChannelGroup_AddGroup(channelgroupraw, group.getRaw());
        }
        public RESULT getNumGroups           (ref int numgroups)
        {
            return FMOD_ChannelGroup_GetNumGroups(channelgroupraw, ref numgroups);
        }
        public RESULT getGroup               (int index, ref ChannelGroup group)
        {
            RESULT result = RESULT.OK;
            IntPtr channelraw = new IntPtr();
            ChannelGroup    channelnew = null;

            try
            {
                result = FMOD_ChannelGroup_GetGroup(channelgroupraw, index, ref channelraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (group == null)
            {
                channelnew = new ChannelGroup();
                channelnew.setRaw(channelraw);
                group = channelnew;
            }
            else
            {
                group.setRaw(channelraw);
            }
                             
            return result;
        }

        // Information only functions.
        public RESULT getName                (StringBuilder name, int namelen)
        {
            return FMOD_ChannelGroup_GetName(channelgroupraw, name, namelen);
        }
        public RESULT getNumChannels         (ref int numchannels)
        {
            return FMOD_ChannelGroup_GetNumChannels(channelgroupraw, ref numchannels);
        }
        public RESULT getChannel             (int index, ref Channel channel)
        {
            RESULT result = RESULT.OK;
            IntPtr channelraw = new IntPtr();
            Channel    channelnew = null;

            try
            {
                result = FMOD_ChannelGroup_GetChannel(channelgroupraw, index, ref channelraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (channel == null)
            {
                channelnew = new Channel();
                channelnew.setRaw(channelraw);
                channel = channelnew;
            }
            else
            {
                channel.setRaw(channelraw);
            }
                             
            return result;
        }
        public RESULT getWaveData            (float[] wavearray, int numvalues, int channeloffset)
        {
            return FMOD_ChannelGroup_GetWaveData(channelgroupraw, wavearray, numvalues, channeloffset);
        }

        #region importfunctions
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_Release          (IntPtr channelgroup);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetSystemObject  (IntPtr channelgroup, ref IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_SetVolume        (IntPtr channelgroup, float volume);
        [DllImport (VERSION.dll)]        
        private static extern RESULT FMOD_ChannelGroup_GetVolume        (IntPtr channelgroup, ref float volume);
        [DllImport (VERSION.dll)]       
        private static extern RESULT FMOD_ChannelGroup_SetPitch         (IntPtr channelgroup, float pitch);
        [DllImport (VERSION.dll)]       
        private static extern RESULT FMOD_ChannelGroup_GetPitch         (IntPtr channelgroup, ref float pitch);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_Stop             (IntPtr channelgroup);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverridePaused   (IntPtr channelgroup, bool paused);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverrideVolume   (IntPtr channelgroup, float volume);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverrideFrequency(IntPtr channelgroup, float frequency);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverridePan      (IntPtr channelgroup, float pan);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverrideMute     (IntPtr channelgroup, bool mute);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_OverrideSpeakerMix(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float backleft, float backright, float sideleft, float sideright);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_AddGroup         (IntPtr channelgroup, IntPtr group);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetNumGroups     (IntPtr channelgroup, ref int numgroups);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetGroup         (IntPtr channelgroup, int index, ref IntPtr group);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetName          (IntPtr channelgroup, StringBuilder name, int namelen);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetNumChannels   (IntPtr channelgroup, ref int numchannels);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetChannel       (IntPtr channelgroup, int index, ref IntPtr channel);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_ChannelGroup_GetWaveData      (IntPtr channelgroup, [MarshalAs(UnmanagedType.LPArray)] float[] wavearray, int numvalues, int channeloffset);
        #endregion

        #region wrapperinternal

        private IntPtr channelgroupraw;

        public void setRaw(IntPtr channelgroup)
        {
            channelgroupraw = new IntPtr();

            channelgroupraw = channelgroup;
        }

        public IntPtr getRaw()
        {
            return channelgroupraw;
        }

        #endregion
    }
}