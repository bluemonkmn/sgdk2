using System;
using System.Collections;

namespace SGDK2
{
	/// <summary>
	/// Manages cached copies of sprite definitions
	/// </summary>
	public class SpriteCache
	{
		Hashtable m_cache = null;
      Display m_display;

      public SpriteCache(Display display)
		{
         m_display = display;
		}

      public void Clear()
      {
         m_cache = null;
      }

      public CachedSpriteDef this[string name]
      {
         get
         {
            CachedSpriteDef result;
            if (m_cache == null)
            {
               m_cache = new Hashtable();
            }
            else
            {
               if (m_cache.ContainsKey(name))
               {
                  WeakReference wr = (WeakReference)m_cache[name];
                  if (wr.IsAlive)
                     return (CachedSpriteDef)wr.Target;
               }
            }
            result = new CachedSpriteDef(ProjectData.GetSpriteDefinition(name), m_display);
            m_cache[name] = new WeakReference(result);
            return result;
         }
      }
	}

   public class StateInfo
   {
      public readonly FrameCache frameset;
      public readonly TileFrame[] frames;
      public readonly string name;
      private System.Drawing.Rectangle bounds = System.Drawing.Rectangle.Empty;

      public StateInfo(TileFrame[] frames, FrameCache frameset, string name)
      {
         this.frames = frames;
         this.frameset = frameset;
         this.name = name;
      }

      /// <summary>
      /// Returns a rectangle that bounds all the graphics possible in this state
      /// </summary>
      /// <returns>Rectangle relative to the origin of this sprite that can contain
      /// all graphics possible in this state</returns>
      public System.Drawing.Rectangle Bounds
      {
         get
         {
            if (bounds.IsEmpty)
            {
               foreach(TileFrame f in frames)
               {
                  for(int i=0; i<f.subFrames.Length; i++)
                  {
                     if (bounds.IsEmpty)
                        bounds = frameset[f.subFrames[i]].Bounds;
                     else
                        bounds = System.Drawing.Rectangle.Union(frameset[f.subFrames[i]].Bounds, bounds);
                  }
               }
            }
            return bounds;
         }
      }
   }

   public class CachedSpriteDef : System.Collections.DictionaryBase
   {
      private System.Collections.Specialized.StringCollection m_ParamNames;
      private System.Drawing.Rectangle m_bounds = System.Drawing.Rectangle.Empty;
      private readonly ProjectDataset.SpriteDefinitionRow m_SpriteDef;

      public CachedSpriteDef(ProjectDataset.SpriteDefinitionRow drSpriteDef, Display display)
      {
         ProjectDataset.SpriteParameterRow[] drParams = drSpriteDef.GetSpriteParameterRows();
         m_SpriteDef = drSpriteDef;

         m_ParamNames = new System.Collections.Specialized.StringCollection();
         for(int i=0; i<drParams.Length; i++)
            m_ParamNames.Add(drParams[i].Name);

         foreach(ProjectDataset.SpriteStateRow drState in drSpriteDef.GetSpriteStateRows())
         {
            RefreshState(drState, display);
         }
      }

      public string Name
      {
         get
         {
            return m_SpriteDef.Name;
         }
      }

      public bool ContainsState(string stateName)
      {
         return InnerHashtable.ContainsKey(stateName);
      }

      public System.Collections.Specialized.StringCollection ParamNames
      {
         get
         {
            return m_ParamNames;
         }
      }

      public StateInfo this[string stateName]
      {
         get
         {
            return (StateInfo)InnerHashtable[stateName];
         }
      }

      public void RefreshState(ProjectDataset.SpriteStateRow drState, Display display)
      {
         ProjectDataset.SpriteFrameRow[] frameRows = ProjectData.GetSortedSpriteFrames(drState);
         System.Collections.ArrayList frames = new System.Collections.ArrayList();
         System.Collections.ArrayList subFrames = new System.Collections.ArrayList();
         int frameValue = 0;
         for(int nFrameIdx = 0; nFrameIdx < frameRows.Length; nFrameIdx++)
         {
            subFrames.Add(frameRows[nFrameIdx].FrameValue);
            if (frameRows[nFrameIdx].Duration > 0)
            {
               frameValue += frameRows[nFrameIdx].Duration;
               frames.Add(new TileFrame(frameValue,(int[])subFrames.ToArray(typeof(int))));
               subFrames = new System.Collections.ArrayList();
            }
         }
         if (subFrames.Count > 0)
            // Frame sequence ends with a delay of 0 -- user error; add 1.
            frames.Add(new TileFrame(++frameValue,(int[])subFrames.ToArray(typeof(int))));

         InnerHashtable[drState.Name] = new StateInfo((TileFrame[])frames.ToArray(
            typeof(TileFrame)), FrameCache.GetFrameCache(drState.FramesetName, display), drState.Name);
      }

      public int[] this[string state, int frame]
      {
         get
         {
            int idx = GetIndexFromSequenceNumber(state, frame);
            if (idx < 0)
               return new int[] {};
            else
            {
               return this[state].frames[idx].subFrames;
            }
         }
      }

      /// <summary>
      /// Retrieve the index of the set of sub-frames to be used
      /// at a specified frame timer sequence value
      /// </summary>
      /// <param name="frameSequence">Timer/counter sequence value</param>
      /// <returns>Index into the specified state's frames array</returns>
      public int GetIndexFromSequenceNumber(string state, int frameSequence)
      {
         if (!this.InnerHashtable.ContainsKey(state))
            return -1;
         StateInfo si = this[state];
         if (si.frames.Length == 1)
            return 0;
         TileFrame[] tf = si.frames;
         if (tf.Length <= 0) return -1;
         frameSequence = frameSequence % tf[tf.Length-1].accumulatedDuration;
         int nFoundIdx = Array.BinarySearch(tf, frameSequence + 1);
         if ((nFoundIdx < 0) && (~nFoundIdx < tf.Length))
            return ~nFoundIdx;
         else if (nFoundIdx >= 0)
            return nFoundIdx;
         else
            throw new ApplicationException("Did not expect modded frame value beyond array bounds");
      }

      public System.Drawing.Rectangle Bounds
      {
         get
         {
            if (m_bounds.IsEmpty)
            {
               foreach(DictionaryEntry de in InnerHashtable)
                  if (m_bounds.IsEmpty)
                     m_bounds = ((StateInfo)de.Value).Bounds;
                  else
                     m_bounds = System.Drawing.Rectangle.Union(((StateInfo)de.Value).Bounds, m_bounds);
            }
            return m_bounds;
         }
      }
   }
}
