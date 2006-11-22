using System;
using System.ComponentModel;

/// <summary>
/// Implements rules common to sprites and plans
/// </summary>
[Serializable()]
public abstract class GeneralRules
{
   private static SaveUnit saveUnit = null;
   private static System.Collections.Hashtable memorySaveSlots = new System.Collections.Hashtable();

   public abstract LayerBase ParentLayer
   {
      get;
   }

   [Description("Write a string to the debug output without moving to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugLabel(string Label)
   {
      Project.GameWindow.debugText.Write(Label);
   }

   [Description("Write a number to the debug output and move to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugValue(int DebugValue)
   {
      Project.GameWindow.debugText.WriteLine(DebugValue.ToString());
   }

   [Description("Sets a different map as the one to be drawn on the game display.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public void SwitchToMap([Editor("MapType", "UITypeEditor")] Type MapType, bool UnloadCurrent)
   {
      System.Type source = Project.GameWindow.CurrentMap.GetType();
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(source);
      (Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(MapType)).m_CameFromMapType = source;
   }

   [Description("Return to the map that was active before the last SwitchToMap.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public void ReturnToPreviousMap(bool UnloadCurrent)
   {
      System.Type source = Project.GameWindow.CurrentMap.m_CameFromMapType;
      if (source == null)
         return;
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(source);
      Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(source);
   }

   [Description("Unloads the specified map, which will force it to be recreated/reset next time it is used.")]
   public void UnloadMap([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      Project.GameWindow.UnloadMap(MapType);
   }

   [Description("Includes a specified set of objects in the SaveUnit that will be saved with the next call to a Save function")]
   public void IncludeInSaveUnit(SaveUnitInclusion Include)
   {
      if (saveUnit == null)
         saveUnit = new SaveUnit();

      switch(Include)
      {
         case SaveUnitInclusion.AllMaps:
            saveUnit.Maps = Project.GameWindow.LoadedMaps;
            saveUnit.AllMaps = true;
            break;
         case SaveUnitInclusion.AllCounters:
         {
            saveUnit.Counters = new System.Collections.ArrayList();
            System.Reflection.PropertyInfo[] counterProps = typeof(Counter).GetProperties(
               System.Reflection.BindingFlags.Public |
               System.Reflection.BindingFlags.GetProperty |
               System.Reflection.BindingFlags.Static);
            foreach(System.Reflection.PropertyInfo counterProp in counterProps)
               saveUnit.Counters.Add(new CounterRef((Counter)counterProp.GetValue(null, null)));
         }
            break;
         case SaveUnitInclusion.WhichMapIsCurrent:
            saveUnit.CurrentMapType = Project.GameWindow.CurrentMap.GetType();
            break;
      }
   }

   [Description("Include the specified counter in the SaveUnit that will be saved with the next call to a Save function")]
   public void IncludeCounterInSaveUnit(Counter Counter)
   {
      if (saveUnit == null)
      {
         saveUnit = new SaveUnit();
      }
      if (saveUnit.Counters == null)
      {
         saveUnit.Counters = new System.Collections.ArrayList();
         saveUnit.Counters.Add(new CounterRef(Counter));
         return;
      }
      foreach(CounterRef cr in saveUnit.Counters)
      {
         if (cr.instance == Counter)
            return;
      }
      saveUnit.Counters.Add(Counter);
   }

   [Description("Exclude the specified counter from the SaveUnit that will be saved with the next call to a Save function")]
   public void ExcludeCounterFromSaveUnit(Counter Counter)
   {
      if ((saveUnit == null) || (saveUnit.Counters == null))
         return;
      for(int i = 0; i < saveUnit.Counters.Count; i++)
      {
         if (((CounterRef)(saveUnit.Counters[i])).instance == Counter)
         {
            saveUnit.Counters.RemoveAt(i);
            return;
         }
      }      
   }

   [Description("Include the specified map in the SaveUnit that will be saved with the next call to a Save function.")]
   public void IncludeMapInSaveUnit([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      if (Project.GameWindow.LoadedMaps.ContainsKey(MapType))
      {
         if (saveUnit == null)
            saveUnit = new SaveUnit();
         if (saveUnit.Maps == null)
            saveUnit.Maps = new System.Collections.Hashtable();
         saveUnit.Maps[MapType] = Project.GameWindow.LoadedMaps[MapType];
      }
   }

   [Description("Remove the specified map in from the SaveUnit that will be saved with the next call to a Save function.")]
   public void ExcludeMapFromSaveUnit([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      if ((saveUnit == null) || (saveUnit.Maps == null))
         return;
      if (saveUnit.Maps.ContainsKey(MapType))
         saveUnit.Maps.Remove(MapType);
   }

   [Description("Save the current SaveUnit into the specified save slot, and reset SaveUnit. If InMemory is true, no file will be created, otherwise the game is saved to a file.")]
   public void SaveGame(int Slot, bool InMemory)
   {
      System.IO.Stream stm;
      if (InMemory)
         stm = new System.IO.MemoryStream();
      else
         stm = new System.IO.FileStream(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"),
            System.IO.FileMode.Create, System.IO.FileAccess.Write);
      using(stm)
      {
         if (saveUnit == null)
         {
            IncludeInSaveUnit(SaveUnitInclusion.AllMaps);
            IncludeInSaveUnit(SaveUnitInclusion.AllCounters);
            IncludeInSaveUnit(SaveUnitInclusion.WhichMapIsCurrent);
         }
         System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
         bf.Serialize(stm, saveUnit);
         if (InMemory)
            memorySaveSlots[Slot] = ((System.IO.MemoryStream)stm).ToArray();
         saveUnit = null;
      }
   }

   [Description("Restore the state of the objects contained in the specified save slot. If InMemory is true, the memory slot is used, otherwise the file associated with the slot is loaded.")]
   public void LoadGame(int Slot, bool InMemory)
   {
      System.IO.Stream stm;

      if (InMemory)
         stm = new System.IO.MemoryStream((byte[])memorySaveSlots[Slot], false);
      else
         stm = new System.IO.FileStream(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"),
            System.IO.FileMode.Open, System.IO.FileAccess.Read);

      using(stm)
      {
         System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
         SaveUnit unit = (SaveUnit)bf.Deserialize(stm);
         if (unit.Maps != null)
         {
            if (unit.AllMaps)
               Project.GameWindow.LoadedMaps = unit.Maps;
            else
               foreach(System.Collections.DictionaryEntry de in unit.Maps)
                  Project.GameWindow.LoadedMaps[de.Key] = de.Value;
         }
         if (unit.CurrentMapType != null)
            Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(unit.CurrentMapType);
         else
            Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(Project.GameWindow.CurrentMap.GetType());
         // Counters auto-magically take care of themselves via CounterRef
      }
   }

   [Description("Determines if saved game data exists in the specified slot.  Checks for the existence of a file if InMemory is false.")]
   public bool SaveExists(int Slot, bool InMemory)
   {
      if (InMemory)
         return memorySaveSlots.ContainsKey(Slot);
      return System.IO.File.Exists(System.IO.Path.Combine(
         System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"));
   }

   [Description("Empties the specified save slot.  If InMemory is false, a file is deleted, otherwise a memory slot is cleared.")]
   public void DeleteSave(int Slot, bool InMemory)
   {
      if (InMemory)
         memorySaveSlots.Remove(Slot);
      else
         System.IO.File.Delete(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"));
   }
}

public enum SaveUnitInclusion
{
   AllMaps,
   AllCounters,
   WhichMapIsCurrent
}

[Serializable()]
public class SaveUnit
{
   public SaveUnit()
   {
   }
   public bool AllMaps = false;
   public System.Collections.Hashtable Maps = null;
   public System.Type CurrentMapType = null;
   public System.Collections.ArrayList Counters = null;
}

[Serializable()]
public class CounterRef : System.Runtime.Serialization.ISerializable
{
   public string counterName;
   public Counter instance;

   public CounterRef(Counter counter)
   {
      instance = counter;
      System.Reflection.PropertyInfo[] counterProps = typeof(Counter).GetProperties(
         System.Reflection.BindingFlags.Public |
         System.Reflection.BindingFlags.GetProperty |
         System.Reflection.BindingFlags.Static);
      foreach(System.Reflection.PropertyInfo counterProp in counterProps)
      {
         Counter inst = (Counter)counterProp.GetValue(null, null);
         if (inst == counter)
         {
            counterName = counterProp.Name;
            break;
         }
      }
   }

   private CounterRef(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      counterName = info.GetString("CounterName");
      instance = (Counter)(typeof(Counter).GetProperty(counterName,
         System.Reflection.BindingFlags.Public |
         System.Reflection.BindingFlags.GetProperty |
         System.Reflection.BindingFlags.Static).GetValue(null, null));
      instance.CurrentValue = info.GetInt32("CounterValue");
   }

   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      info.AddValue("CounterName", counterName);
      info.AddValue("CounterValue", instance.CurrentValue);
   }
}