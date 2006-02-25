/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2004 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;
using System.ComponentModel;

namespace SGDK2
{
	/// <summary>
	/// Collects a number of layers into a single parallax scrolling object
	/// </summary>
	public class Map
	{
      #region Embedded classes
      public class LayerCollection : System.Collections.CollectionBase
      {
         public int Add(Layer layer)
         {
            return List.Add(layer);
         }

         public void Insert(int index, Layer layer)
         {
            List.Insert(index, layer);
         }

         public void Remove(Layer layer)
         {
            List.Remove(layer);
         }

         public Layer this[int index]
         {
            get
            {
               return (Layer)List[index];
            }
            set
            {
               List[index] = value;
            }
         }

         public int IndexOf(Layer layer)
         {
            return List.IndexOf(layer);
         }

         public Boolean Contains(Layer layer)
         {
            return List.Contains(layer);
         }
      }
      #endregion

      #region Fields
      public LayerCollection m_Layers = new LayerCollection();
      #endregion

		public Map()
		{
		}

      #region Properties
      public LayerCollection Layers
      {
         get
         {
            return m_Layers;
         }
      }
      #endregion
	}
}
