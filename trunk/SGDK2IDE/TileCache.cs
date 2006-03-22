/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2005 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;

namespace SGDK2
{
	/// <summary>
	/// Pre-processed cache of tiles extracted from a TileSetRow in the project
	/// </summary>
	public class TileCache
	{
      class TileInfo
      {
         public readonly TileFrame[] frames;
         public readonly ProjectDataset.CounterRow counterRow;
         public TileInfo(TileFrame[] frames, ProjectDataset.CounterRow counterRow)
         {
            this.frames = frames;
            this.counterRow = counterRow;
         }
      }

      class TileFrame : IComparable
      {
         /// <summary>
         /// Represents counter value, and is used to optimize frame searching
         /// </summary>
         public readonly int accumulatedDuration;
         /// <summary>
         /// Composite tiles may have multiple frames drawn on top of each other
         /// </summary>
         public readonly int[] subFrames;


         public TileFrame(int accumulatedDuration, int[] subFrames)
         {
            this.accumulatedDuration = accumulatedDuration;
            this.subFrames = subFrames;
         }
         
         #region IComparable Members

         public int CompareTo(object obj)
         {
            if (obj is TileFrame)
               return accumulatedDuration.CompareTo(((TileFrame)obj).accumulatedDuration);
            else
               return accumulatedDuration.CompareTo(obj);
         }

         #endregion
      }
      private readonly TileInfo[] m_tiles;

		public TileCache(ProjectDataset.TilesetRow tr)
		{
         ProjectDataset.TileRow[] tiles = ProjectData.GetSortedTileRows(tr);
         int nFrameCount = tr.FramesetRow.GetFrameRows().Length;
         int nTileCount;
         if (tiles.Length > 0)
            nTileCount = Math.Max(tiles[tiles.Length-1].TileValue+1, nFrameCount);
         else
            nTileCount = nFrameCount;
         m_tiles = new TileInfo[nTileCount];
         int nCurRowIdx = 0;
         for (int nTileIdx = 0; nTileIdx < nTileCount; nTileIdx++)
         {
            if ((nCurRowIdx < tiles.Length) && (tiles[nCurRowIdx].TileValue == nTileIdx))
            {
               RefreshTile(tiles[nCurRowIdx]);
               nCurRowIdx++;
            }
            else
               m_tiles[nTileIdx] = new TileInfo(
                  new TileFrame[1]
                  {
                     new TileFrame(1,new int[] {nTileIdx % nFrameCount})
                  }, null);
         }
		}

      public void RefreshTile(ProjectDataset.TileRow tr)
      {
         ProjectDataset.TileFrameRow[] tfr = ProjectData.GetSortedTileFrames(tr);
         System.Collections.ArrayList frames = new System.Collections.ArrayList();
         System.Collections.ArrayList subFrames = new System.Collections.ArrayList();
         int counterValue = 0;
         for(int nFrameIdx = 0; nFrameIdx < tfr.Length; nFrameIdx++)
         {
            subFrames.Add(tfr[nFrameIdx].FrameValue);
            if (tfr[nFrameIdx].Duration > 0)
            {
               counterValue += tfr[nFrameIdx].Duration;
               frames.Add(new TileFrame(counterValue,(int[])subFrames.ToArray(typeof(int))));
               subFrames = new System.Collections.ArrayList();
            }
         }
         if (subFrames.Count > 0)
            // Frame sequence ends with a delay of 0 -- user error; add 1.
            frames.Add(new TileFrame(++counterValue,(int[])subFrames.ToArray(typeof(int))));

         m_tiles[tr.TileValue] = new TileInfo((TileFrame[])frames.ToArray(
            typeof(TileFrame)), tr.CounterRow);
      }

      public void ResetTile(int nTile, int nFrame)
      {
         m_tiles[nTile] = new TileInfo(
            new TileFrame[1]
               {
                  new TileFrame(1,new int[] {nFrame})
               }, null);
      }

      public int[] this[int nTileValue]
      {
         get
         {
            if (nTileValue >= m_tiles.Length)
               nTileValue = nTileValue % m_tiles.Length;
            if (m_tiles[nTileValue].frames.Length == 1)
               return m_tiles[nTileValue].frames[0].subFrames;
            TileFrame[] tf = m_tiles[nTileValue].frames;
            int nCounterValue;
            if (tf.Length <= 0) return new int[] {};
            if (m_tiles[nTileValue].counterRow != null)
               nCounterValue = m_tiles[nTileValue].counterRow.Value % tf[tf.Length-1].accumulatedDuration;
            else
               nCounterValue = 0;
            int nFoundIdx = Array.BinarySearch(tf, nCounterValue+1);
            if ((nFoundIdx < 0) && (~nFoundIdx < tf.Length))
               return tf[~nFoundIdx].subFrames;
            else if (nFoundIdx >= 0)
               return tf[nFoundIdx].subFrames;
            else
               throw new ApplicationException("Did not expect modded counter value beyond array bounds");
         }
      }

      public int[] this[int nTileValue, int nCounterValue]
      {
         get
         {
            if (nTileValue >= m_tiles.Length)
               nTileValue = nTileValue % m_tiles.Length;
            if (m_tiles[nTileValue].frames.Length == 1)
               return m_tiles[nTileValue].frames[0].subFrames;
            TileFrame[] tf = m_tiles[nTileValue].frames;
            if (tf.Length <= 0) return new int[] {};
            if (m_tiles[nTileValue].counterRow != null)
               nCounterValue = nCounterValue % tf[tf.Length-1].accumulatedDuration;
            else
               nCounterValue = 0;
            int nFoundIdx = Array.BinarySearch(tf, nCounterValue+1);
            if ((nFoundIdx < 0) && (~nFoundIdx < tf.Length))
               return tf[~nFoundIdx].subFrames;
            else if (nFoundIdx >= 0)
               return tf[nFoundIdx].subFrames;
            else
               throw new ApplicationException("Did not expect modded counter value beyond array bounds");
         }
      }

      public short GetFrameCount(int nTileIndex)
      {
         if (nTileIndex >= m_tiles.Length)
            nTileIndex = nTileIndex % m_tiles.Length;
         return (short)m_tiles[nTileIndex].frames.Length;
      }

      public int Count
      {
         get
         {
            return m_tiles.Length;
         }
      }

   }
}
