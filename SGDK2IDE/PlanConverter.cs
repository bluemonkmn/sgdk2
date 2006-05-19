using System;

namespace SGDK2
{
	/// <summary>
	/// Exposes properties of a SpritePlanRow to a property browser
	/// </summary>
   public class PlanProvider
   {
      private ProjectDataset.SpritePlanRow m_Plan;

      public PlanProvider(ProjectDataset.SpritePlanRow plan)
      {
         m_Plan = plan;
      }

      public string Name
      {
         get
         {
            return m_Plan.Name;
         }
         set
         {
            m_Plan.Name = value;
         }
      }

      public int Priority
      {
         get
         {
            return m_Plan.Priority;
         }
         set
         {
            m_Plan.Priority = value;
         }
      }
   }

   public class CoordProvider
   {
      private ProjectDataset.CoordinateRow m_Coord;

      public CoordProvider(ProjectDataset.CoordinateRow coord)
      {
         m_Coord = coord;
      }

      public int X
      {
         get
         {
            return m_Coord.X;
         }
         set
         {
            m_Coord.X = value;
         }
      }

      public int Y
      {
         get
         {
            return m_Coord.Y;
         }
         set
         {
            m_Coord.Y = value;
         }
      }

      public int Weight
      {
         get
         {
            return m_Coord.Weight;
         }
         set
         {
            m_Coord.Weight = value;
         }
      }

      public int Sequence
      {
         get
         {
            return m_Coord.Sequence;
         }
      }

      public string Plan
      {
         get
         {
            return m_Coord.SpritePlanRowParent.Name;
         }
      }

      public override string ToString()
      {
         return X.ToString() + ", " + Y.ToString();
      }

      [System.ComponentModel.Browsable(false)]
      public ProjectDataset.CoordinateRow CoordinateRow
      {
         get
         {
            return m_Coord;
         }
      }
   }
}
