/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Implements rules common to sprites and plans
/// </summary>
[Serializable()]
public abstract partial class GeneralRules
{
   protected static SaveUnit saveUnit = null;
   protected static System.Collections.Hashtable memorySaveSlots = new System.Collections.Hashtable();
   protected static System.Random randomGen = new System.Random();
   protected static long fpsStartTime;
   protected static long fpsFrameCount;

   /// <summary>
   /// Contains the last sprite created with <see cref="PlanBase.AddSpriteAtPlan"/>,
   /// <see cref="SpriteBase.TileAddSprite"/> or <see cref="SpriteBase.AddSpriteHere"/>.
   /// </summary>
   /// <remarks>This reference can be used to initialize various properties of a sprite
   /// that was just created. It can't be used to set parameters on the sprite that are
   /// specific to that sprite type unless it is cast to the correct type (which is not
   /// supported in the interface for defining rules). That should be done from within
   /// the sprite's rules by checking for a specific parameter value (like "IsInitialized")
   /// being 0 or 1, for example.</remarks>
   public static SpriteBase lastCreatedSprite;

   /// <summary>
   /// Retrieves the layer that contains this object.
   /// </summary>
   public abstract LayerBase ParentLayer
   {
      get;
   }

   /// <summary>
   /// Limit the frame rate of the game to the specified number of frames per second.  Call this only once per frame.
   /// </summary>
   /// <param name="fps">Frames per second.</param>
   /// <remarks>If this is called twice per frame, the effect would be to
   /// limit the frame rate to half the specified value, and more calls will
   /// make the game run even slower.</remarks>
   [Description("Limit the frame rate of the game to the specified number of frames per second.  Call this only once per frame.")]
   public virtual void LimitFrameRate(int fps)
   {
      long freq;
      long frame;
      freq = System.Diagnostics.Stopwatch.Frequency;
      frame = System.Diagnostics.Stopwatch.GetTimestamp();
      while ((frame - fpsStartTime) * fps < freq * fpsFrameCount)
      {
         int sleepTime = (int)((fpsStartTime * fps + freq * fpsFrameCount - frame * fps) * 1000 / (freq * fps));
         if (sleepTime > 0) System.Threading.Thread.Sleep(sleepTime);
         frame = System.Diagnostics.Stopwatch.GetTimestamp();
      }
      if (++fpsFrameCount > fps)
      {
         fpsFrameCount = 0;
         fpsStartTime = frame;
      }
   }


   /// <summary>
   /// Write a string to the debug output without moving to the next line.
   /// </summary>
   /// <param name="Label">String value to write to the debug output</param>
   /// <remarks><para>By default, the debug output is reset each frame, so in order to
   /// force a piece of debug output to be visible long enough to see it, it must be
   /// logged during each frame.</para>
   /// <para>Debug output is drawn to the display in <see cref="GameForm.OutputDebugInfo"/>,
   /// which is called from the main game loop in <see cref="GameForm.Run"/>. The behavior
   /// of debug output can be customized there.</para>
   /// <para>This function is often used in conjunction with <see cref="LogDebugValue"/>
   /// because LogDebugLabel leaves the position of the output on the same line allowing for a
   /// value to be appended to the end of the line.</para>
   /// <seealso cref="LogDebugValue"/>
   /// <seealso cref="GameForm.debugText"/></remarks>
   /// <example>This will add the string "Counter 1 value: " followed by the value
   /// contained in a counter named "Counter 1" to the debug output for the current frame.
   /// <code>LogDebugLabel("Counter 1 value: ");
   /// LogDebugValue(Counter.Counter_1.CurrentValue);</code></example>
   [Description("Write a string to the debug output without moving to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public virtual void LogDebugLabel(string Label)
   {
      Project.GameWindow.debugText.Write(Label);
   }

   /// <summary>
   /// Write a number to the debug output and move to the next line.
   /// </summary>
   /// <param name="DebugValue">Specifies a numeric value to be logged to the output</param>
   /// <remarks><para>By default, the debug output is reset each frame, so in order to
   /// force a piece of debug output to be visible long enough to see it, it must be
   /// logged during each frame.</para>
   /// <para>Debug output is drawn to the display in <see cref="GameForm.OutputDebugInfo"/>,
   /// which is called from the main game loop in <see cref="GameForm.Run"/>. The behavior
   /// of debug output can be customized there.</para>
   /// <para>This function is often used in conjunction with <see cref="LogDebugLabel"/>
   /// because <see cref="LogDebugLabel"/> leaves the position of the output on the same
   /// line allowing for a label to be provided on the same line as the value.</para>
   /// <seealso cref="LogDebugLabel"/>
   /// <seealso cref="GameForm.debugText"/></remarks>
   /// <example>See <see cref="LogDebugLabel"/> for an example</example>
   [Description("Write a number to the debug output and move to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public virtual void LogDebugValue(int DebugValue)
   {
      Project.GameWindow.debugText.WriteLine(DebugValue.ToString());
   }

   /// <summary>
   /// Sets a different map as the one to be drawn on the game display.
   /// </summary>
   /// <param name="MapType">Specifies the map to switch to.</param>
   /// <param name="UnloadCurrent">If true, the current map will be unloaded first</param>
   /// <remarks><para><paramref name="MapType"/> specifies a type of map rather than a specific
   /// instance of a map.
   /// This accomplishes two things. First, it allows the function to switch to a map that hasn't
   /// been initialized/created yet, thus allowing the map instance to only be created when
   /// necessary, and not before switching to it. This can improve performance because an instance
   /// of each map doesn't have to be created during game initialization. Secondly, it allows maps
   /// to be unloaded so that maps instances that don't need to remember their state can be released,
   /// freeing up whatever memory they were occupying. Each map defined in the IDE is compiled/generated as its
   /// own class when the project is compiled, and each map instance becomes an instance of the
   /// class. This conceivably allows multiple instances of the same map to exist, but this is not
   /// fully supported by default in the code provided with this environment.</para>
   /// <para>When <paramref name="UnloadCurrent"/> is true, the old map will be unloaded while
   /// switching to the new map, freeing up the memory used by the old map's sprites and tiles
   /// (if no other references to the map exist). Eliminating the map from memory also reduces the
   /// amount of data that would be written to a saved game file. Finally it causes the map to be
   /// recreated/reset when returning to it in the future. When <paramref name="UnloadCurrent"/>
   /// is false, the map is retained and re-used next time the game switches to the map, which
   /// means the state of all the tiles and sprites will be remembered as they were when switching
   /// away from the map.</para>
   /// <seealso cref="UnloadMap"/></remarks>
   [Description("Sets a different map as the one to be drawn on the game display.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public virtual void SwitchToMap([Editor("MapType", "UITypeEditor")] Type MapType, bool UnloadCurrent)
   {
      System.Type source = Project.GameWindow.CurrentMap.GetType();
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(source);
      (Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(MapType)).m_CameFromMapType = source;
   }

   /// <summary>
   /// Return to the map that was active before the last <see cref="SwitchToMap"/>.
   /// </summary>
   /// <param name="UnloadCurrent">If true, the current map will be unloaded first</param>
   /// <remarks><para>Each map remembers where the game was before it became the current map.
   /// Calling this will return to that map, recreating it if it was unloaded. Since each
   /// map can only remember one previous map, you can't keep multiple instances of the same
   /// map in the history. For example, if you switch from a map called "Level1" to "Level2"
   /// then to "Level3" and then back to "Level2", you won't be able to return back to
   /// Level1 using ReturnToPreviousMap. Level2 would switch back to Level3 each time
   /// ReturnToPreviousMap is called.</para>
   /// <para>If there is no previous map to return to, this function will have no effect.</para>
   /// <para>See <see cref="SwitchToMap"/> for more information about unloading maps.</para></remarks>
   [Description("Return to the map that was active before the last SwitchToMap.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public virtual void ReturnToPreviousMap(bool UnloadCurrent)
   {
      System.Type source = Project.GameWindow.CurrentMap.m_CameFromMapType;
      if (source == null)
         source = Project.GameWindow.CurrentMap.GetType();
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(Project.GameWindow.CurrentMap.GetType());
      Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(source);
   }

   /// <summary>
   /// Determines if there is a previous map to return to.
   /// </summary>
   /// <returns>True if calling <see cref="ReturnToPreviousMap"/> will have any effect,
   /// false otherwise.</returns>
   [Description("Determines if there is a previous map to return to.")]
   public virtual bool CanReturnToPreviousMap()
   {
      return Project.GameWindow.CurrentMap.m_CameFromMapType != null;
   }

   /// <summary>
   /// Unloads the specified map.
   /// </summary>
   /// <param name="MapType">Specifies the class of map whose instance will be unloaded if
   /// it is loaded.</param>
   /// <remarks>Unloading a map will free up any memory used by its tiles and sprites if no other
   /// code is referencing it. It also excludes it from the saved game data if the game is saved,
   /// which can significantly reduce the size of a saved game file. Finally it causes the tiles
   /// and sprites to be recreated/reset next time the map is loaded (switched to).
   /// <seealso cref="SwitchToMap"/>
   /// <seealso cref="ReturnToPreviousMap"/></remarks>
   [Description("Unloads the specified map, which will force it to be recreated/reset next time it is used.")]
   public virtual void UnloadMap([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      Project.GameWindow.UnloadMap(MapType);
   }

   /// <summary>
   /// Includes a specified set of objects in the <see cref="SaveUnit"/> that will be saved with the next call to <see cref="SaveGame"/>.
   /// </summary>
   /// <param name="Include">Specifies a category of objects that should be included</param>
   /// <remarks><para>There are many functions that can be used to specify what elements should be
   /// included in a saved game file.  This function is designed for setting up a very rough
   /// outline of what kind of data will be included in the save file.
   /// After this is called, the selection can be fine tuned with further calls to
   /// related functions in the See Also list.  If nothing is included in the <see cref="SaveUnit"/>
   /// before <see cref="SaveGame"/> is called, everything will be included by default.
   /// See <see cref="SaveUnitInclusion"/> for details about the meaning of different categories.</para>
   /// <para>By including maps in a <see cref="SaveUnit"/>, all the tiles and sprites are remembered exactly as they
   /// were when the game was saved, but only for those maps that are included.  Maps that aren't
   /// included will be reset to their initial state next time they become active after loading
   /// that game.</para>
   /// <para>By including only counters (and not maps) in a <see cref="SaveUnit"/>, you can store some
   /// general information in a <see cref="SaveUnit"/> in a significantly smaller file, and use that information
   /// to re-initialize some general properties of the game. For example, the number of lives
   /// and inventory owned by the player (assuming inventory is stored in counters) might be
   /// adequate for some games (if you don't have to worry about items being available again
   /// on maps that have been reset to their initial state, or if you have few enough items that
   /// you can initialize them based on counter values).</para>
   /// <seealso cref="IncludeCounterInSaveUnit"/>
   /// <seealso cref="ExcludeCounterFromSaveUnit"/>
   /// <seealso cref="IncludeMapInSaveUnit"/>
   /// <seealso cref="ExcludeMapFromSaveUnit"/>
   /// <seealso cref="SaveGame"/></remarks>
   [Description("Includes a specified set of objects in the SaveUnit that will be saved with the next call to SaveGame")]
   public virtual void IncludeInSaveUnit(SaveUnitInclusion Include)
   {
      if (saveUnit == null)
         saveUnit = new SaveUnit();

      switch (Include)
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
               foreach (System.Reflection.PropertyInfo counterProp in counterProps)
                  saveUnit.Counters.Add(new CounterRef((Counter)counterProp.GetValue(null, null)));
            }
            break;
         case SaveUnitInclusion.WhichMapIsCurrent:
            saveUnit.CurrentMapType = Project.GameWindow.CurrentMap.GetType();
            break;
         case SaveUnitInclusion.WhichMapIsOverlaid:
            if (Project.GameWindow.OverlayMap == null)
               saveUnit.OverlayMapType = typeof(System.DBNull);
            else
               saveUnit.OverlayMapType = Project.GameWindow.OverlayMap.GetType();
            break;
         case SaveUnitInclusion.PlayerOptions:
            saveUnit.PlayerOptions = Project.GameWindow.Players;
            break;
      }
   }

   /// <summary>
   /// Include the specified counter in the <see cref="SaveUnit"/> that will be saved with the next call to <see cref="SaveGame"/>.
   /// </summary>
   /// <param name="Counter">Specifies a counter whose value will be saved when <see cref="SaveGame"/>
   /// is called.</param>
   /// <remarks>If you want to include only a few select counters in a <see cref="SaveUnit"/>,
   /// you can use this function to select them before calling <see cref="SaveGame"/>.
   /// See <see cref="IncludeInSaveUnit"/> for more information about save units.
   /// <seealso cref="ExcludeCounterFromSaveUnit"/>
   /// <seealso cref="IncludeMapInSaveUnit"/>
   /// <seealso cref="ExcludeMapFromSaveUnit"/>
   /// <seealso cref="SaveGame"/></remarks>
   [Description("Include the specified counter in the SaveUnit that will be saved with the next call to SaveGame")]
   public virtual void IncludeCounterInSaveUnit(Counter Counter)
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
      foreach (CounterRef cr in saveUnit.Counters)
      {
         if (cr.instance == Counter)
            return;
      }
      saveUnit.Counters.Add(Counter);
   }

   /// <summary>
   /// Exclude the specified counter from the <see cref="SaveUnit"/> that will be saved with the next call to <see cref="SaveGame"/>.
   /// </summary>
   /// <param name="Counter">Specifies a counter to remove from the <see cref="SaveUnit"/></param>
   /// <remarks>If you want to save most counters, but exclude a few, you can call
   /// <see cref="IncludeInSaveUnit"/> to include all counters and then exclude a few
   /// with this function.
   /// See <see cref="IncludeInSaveUnit"/> for more information about save units.
   /// <seealso cref="IncludeCounterInSaveUnit"/>
   /// <seealso cref="IncludeMapInSaveUnit"/>
   /// <seealso cref="ExcludeMapFromSaveUnit"/>
   /// <seealso cref="SaveGame"/></remarks>
   [Description("Exclude the specified counter from the SaveUnit that will be saved with the next call to SaveGame")]
   public virtual void ExcludeCounterFromSaveUnit(Counter Counter)
   {
      if ((saveUnit == null) || (saveUnit.Counters == null))
         return;
      for (int i = 0; i < saveUnit.Counters.Count; i++)
      {
         if (((CounterRef)(saveUnit.Counters[i])).instance == Counter)
         {
            saveUnit.Counters.RemoveAt(i);
            return;
         }
      }
   }

   /// <summary>
   /// Include the specified map in the <see cref="SaveUnit"/> that will be saved with the next call to <see cref="SaveGame"/>.
   /// </summary>
   /// <param name="MapType">Specifies a map to include in the <see cref="SaveUnit"/></param>
   /// <remarks>If you only want to include a few maps in a <see cref="SaveUnit"/>, specify
   /// which maps to save with this function. Only loaded maps will be saved. If this function
   /// is called on a map type that refers to a map that is not loaded, it will be ignored, and
   /// the specified map type will still be reset next time it becomes active after loading
   /// the game.
   /// See <see cref="IncludeInSaveUnit"/> for more information about save units.
   /// <seealso cref="IncludeCounterInSaveUnit"/>
   /// <seealso cref="ExcludeCounterFromSaveUnit"/>
   /// <seealso cref="ExcludeMapFromSaveUnit"/>
   /// <seealso cref="SaveGame"/></remarks>
   [Description("Include the specified map in the SaveUnit that will be saved with the next call to SaveGame.")]
   public virtual void IncludeMapInSaveUnit([Editor("MapType", "UITypeEditor")] Type MapType)
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

   /// <summary>
   /// Remove the specified map in from the <see cref="SaveUnit"/> that will be saved with the next call to <see cref="SaveGame"/>.
   /// </summary>
   /// <param name="MapType">Specifies a map to exclude from the <see cref="SaveUnit"/></param>
   /// <remarks>
   /// If you want to save most maps, but not all, you can call <see cref="IncludeInSaveUnit"/>
   /// to include all maps, and then call this to exclude a few. Maps not included in the save
   /// unit (and maps that were not loaded when the game was saved, even if they are "included")
   /// will be reset next time they become active after loading that <see cref="SaveUnit"/>. Often times an
   /// overly map does not contain any important state information, so excluding an overlay map
   /// from a <see cref="SaveUnit"/> might be a good use of this function.
   /// See <see cref="IncludeInSaveUnit"/> for more information about save units.
   /// <seealso cref="IncludeCounterInSaveUnit"/>
   /// <seealso cref="ExcludeCounterFromSaveUnit"/>
   /// <seealso cref="IncludeMapInSaveUnit"/>
   /// <seealso cref="SaveGame"/></remarks>
   [Description("Remove the specified map in from the SaveUnit that will be saved with the next call to SaveGame.")]
   public virtual void ExcludeMapFromSaveUnit([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      if ((saveUnit == null) || (saveUnit.Maps == null))
         return;
      if (saveUnit.Maps.ContainsKey(MapType))
         saveUnit.Maps.Remove(MapType);
   }

   /// <summary>
   /// Save the current <see cref="SaveUnit"/> into the specified save slot.
   /// </summary>
   /// <param name="Slot">Specifies a number to uniquely identify this <see cref="SaveUnit"/>. If saved to a file, this number plue the ".sav" extension becomes the filename.</param>
   /// <param name="InMemory">If true, no file will be created; the current state of the game is stored in a "slot" in memory (and lost when the game exits). Otherwise the game data is saved to a file.</param>
   /// <remarks>After saving the game, the <see cref="SaveUnit"/> is cleared so that any further inclusions
   /// will start from an empty set.  When the game is saved, the state of the maps and counters
   /// are saved in their current state rather than the state they were in when
   /// <see cref="IncludeInSaveUnit"/> and related functions were called. One exception to this
   /// rule is <see cref="SaveUnitInclusion.WhichMapIsCurrent"/>, which will store the map that
   /// was current when <see cref="IncludeInSaveUnit"/> was called rather than than when the
   /// game is actually saved (if these happen to be different). It's possible to save a game
   /// in a memory slot as well as a file slot. These are considered different slots and can
   /// store different save images even if they use the same number. Otherwise, if a game
   /// already exists in a particular slot, it is overwritten.
   /// <seealso cref="IncludeInSaveUnit"/>
   /// <seealso cref="IncludeCounterInSaveUnit"/>
   /// <seealso cref="ExcludeCounterFromSaveUnit"/>
   /// <seealso cref="IncludeMapInSaveUnit"/>
   /// <seealso cref="ExcludeMapFromSaveUnit"/>
   /// <seealso cref="LoadGame"/>
   /// <seealso cref="DeleteSave"/>
   /// </remarks>
   [Description("Save the current save unit into the specified save slot, and reset the save unit. If InMemory is true, no file will be created, otherwise the game is saved to a file.")]
   public virtual void SaveGame(int Slot, bool InMemory)
   {
      System.IO.Stream stm;
      if (InMemory)
         stm = new System.IO.MemoryStream();
      else
         stm = new System.IO.FileStream(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"),
            System.IO.FileMode.Create, System.IO.FileAccess.Write);
      using (stm)
      {
         if (saveUnit == null)
         {
            IncludeInSaveUnit(SaveUnitInclusion.AllMaps);
            IncludeInSaveUnit(SaveUnitInclusion.AllCounters);
            IncludeInSaveUnit(SaveUnitInclusion.WhichMapIsCurrent);
            IncludeInSaveUnit(SaveUnitInclusion.WhichMapIsOverlaid);
            IncludeInSaveUnit(SaveUnitInclusion.PlayerOptions);
         }
         System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
         bf.Serialize(stm, saveUnit);
         if (InMemory)
            memorySaveSlots[Slot] = ((System.IO.MemoryStream)stm).ToArray();
         saveUnit = null;
      }
   }

   /// <summary>
   /// Restore the state of the objects contained in the specified save slot.
   /// </summary>
   /// <param name="Slot">Specifies a number that uniquely identifies which game to load</param>
   /// <param name="InMemory">If true, the memory slot is used, otherwise the file associated with the slot is loaded</param>
   /// <remarks>
   /// If there is no data in the specified slot, an error occurs. Use <see cref="SaveExists"/>
   /// to determine if data is available to load from a particular slot.
   /// Counters that were not included in the <see cref="SaveUnit"/> will retain the same value they
   /// had before the game was loaded. Similarly, maps that are not included in the save
   /// unit will retain the same state they had before the game was loaded instead of
   /// being reset or loaded from the file. One exception is if
   /// <see cref="SaveUnitInclusion.AllMaps"/> was ever included in the <see cref="SaveUnit"/> (even
   /// if some maps were later excluded). In this case, all maps that were not included
   /// in the <see cref="SaveUnit"/> will be reset next time they are visited.
   /// <seealso cref="SaveGame"/>
   /// <seealso cref="SaveExists"/>
   /// <seealso cref="DeleteSave"/>
   /// </remarks>
   [Description("Restore the state of the objects contained in the specified save slot. If InMemory is true, the memory slot is used, otherwise the file associated with the slot is loaded.")]
   public virtual void LoadGame(int Slot, bool InMemory)
   {
      System.IO.Stream stm;

      if (InMemory)
         stm = new System.IO.MemoryStream((byte[])memorySaveSlots[Slot], false);
      else
         stm = new System.IO.FileStream(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"),
            System.IO.FileMode.Open, System.IO.FileAccess.Read);

      using (stm)
      {
         System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
         SaveUnit unit = (SaveUnit)bf.Deserialize(stm);
         if (unit.Maps != null)
         {
            if (unit.AllMaps)
               Project.GameWindow.LoadedMaps = unit.Maps;
            else
               foreach (System.Collections.DictionaryEntry de in unit.Maps)
                  Project.GameWindow.LoadedMaps[de.Key] = de.Value;
            // If sprites exist on any layer of any map whose static state cache has not
            // been initialized, initialize them now.
            foreach (MapBase mb in Project.GameWindow.LoadedMaps.Values)
            {
               // Loop through each property of each map class
               foreach (System.Reflection.PropertyInfo lpi in mb.GetType().GetProperties())
               {
                  // If the property represents a map layer
                  if (lpi.PropertyType.IsSubclassOf(typeof(LayerBase)))
                  {
                     // Retrieve the layer object
                     LayerBase l = (LayerBase)lpi.GetValue(mb, null);
                     // Loop though each sprite in the layer's sprite collection
                     foreach (SpriteBase sp in l.m_Sprites)
                     {
                        Type spriteType = sp.GetType();
                        // Get the property containing the sprite type's cached list of states
                        System.Reflection.FieldInfo statesField = spriteType.GetField("m_SpriteStates",
                           System.Reflection.BindingFlags.GetField |
                           System.Reflection.BindingFlags.NonPublic |
                           System.Reflection.BindingFlags.Static);
                        // If the sprite has not initialized its states
                        if (statesField.GetValue(sp) == null)
                        {
                           // Call the static method that initializes the sprite's states.
                           System.Reflection.MethodInfo initMethod = spriteType.GetMethod("InitializeStates",
                              System.Reflection.BindingFlags.InvokeMethod |
                              System.Reflection.BindingFlags.NonPublic |
                              System.Reflection.BindingFlags.Static);
                           initMethod.Invoke(null, new object[] { Project.GameWindow.GameDisplay });
                        }
                     }
                  }
               }
            }
         }
         if (unit.CurrentMapType != null)
            Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(unit.CurrentMapType);
         else
            Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(Project.GameWindow.CurrentMap.GetType());
         if (unit.OverlayMapType != null)
         {
            if (unit.OverlayMapType == typeof(System.DBNull))
               Project.GameWindow.OverlayMap = null;
            else
               Project.GameWindow.OverlayMap = Project.GameWindow.GetMap(unit.OverlayMapType);
         }
         else if (Project.GameWindow.OverlayMap != null)
            Project.GameWindow.OverlayMap = Project.GameWindow.GetMap(Project.GameWindow.OverlayMap.GetType());
         if (unit.PlayerOptions != null)
            Project.GameWindow.Players = unit.PlayerOptions;
         // Counters auto-magically take care of themselves via CounterRef
      }
   }

   /// <summary>
   /// Determines if saved game data exists in the specified slot.
   /// </summary>
   /// <param name="Slot">Specifies a number that uniquely identifies the slot to check</param>
   /// <param name="InMemory">If true, checks to see if the specified memory slot has saved
   /// game data available, otherwise checks to see if a file with the specified number
   /// exists for loading game data.</param>
   /// <returns>True if data can be loaded from the specified slot, false otherwise.</returns>
   /// <remarks>
   /// Memory slots and file slots are distinct. If a memory slot has game data in it,
   /// a file slot with the same number may not, and vice versa.  Furthermore a memory slot may
   /// have different game data stored in it than a file slot with the same number.
   /// <seealso cref="LoadGame"/>
   /// <seealso cref="SaveGame"/>
   /// <seealso cref="DeleteSave"/></remarks>
   [Description("Determines if saved game data exists in the specified slot.  Checks for the existence of a file if InMemory is false.")]
   public virtual bool SaveExists(int Slot, bool InMemory)
   {
      if (InMemory)
         return memorySaveSlots.ContainsKey(Slot);
      return System.IO.File.Exists(System.IO.Path.Combine(
         System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"));
   }

   /// <summary>
   /// Empties the specified save slot.
   /// </summary>
   /// <param name="Slot">Specifies a number that uniquely identifies a saved game</param>
   /// <param name="InMemory">If false, a file is deleted, otherwise a memory slot is cleared.</param>
   /// <remarks>An error occurs if the specified slot is a file slot and there is no data to delete.
   /// Memory slots and file slots are distinct. If a memory slot has game data in it,
   /// a file slot with the same number may not, and vice versa.  Furthermore a memory slot may
   /// have different game data stored in it than a file slot with the same number.
   /// <seealso cref="LoadGame"/>
   /// <seealso cref="SaveGame"/>
   /// <seealso cref="SaveExists"/></remarks>
   [Description("Empties the specified save slot.  If InMemory is false, a file is deleted, otherwise a memory slot is cleared.")]
   public virtual void DeleteSave(int Slot, bool InMemory)
   {
      if (InMemory)
         memorySaveSlots.Remove(Slot);
      else
         System.IO.File.Delete(System.IO.Path.Combine(
            System.Windows.Forms.Application.UserAppDataPath, Slot.ToString() + ".sav"));
   }

   /// <summary>
   /// If multiple views are visible, this determines which view is currently active.
   /// </summary>
   /// <remarks>Only the currently active view is affected by functions like
   /// <see cref="PlanBase.ScrollSpriteIntoView"/>, and only the current view is
   /// drawn by <see cref="MapBase.Draw"/>. <see cref="MapBase.DrawAllViews"/> is called
   /// during the main loop to cycle through each visible view (changing this value)
   /// and draw it. The scope of this value is limited to the map, so setting it
   /// affects only the map containing this object.</remarks>
   [Browsable(false)]
   public virtual int CurrentView
   {
      get
      {
         return ParentLayer.ParentMap.CurrentViewIndex;
      }
      set
      {
         ParentLayer.ParentMap.CurrentViewIndex = (byte)value;
      }
   }

   /// <summary>
   /// Sets the layout of multiple views for the current map.
   /// </summary>
   /// <param name="Layout">Specifies the number and arrangement of views to be activated.</param>
   /// <remarks>The number of views designated by <paramref name="Layout"/> must not exceed
   /// <see cref="Project.MaxViews"/> defined by the project.</remarks>
   [Description("Sets the layout of multiple views for the current map.")]
   public virtual void SetViewLayout(ViewLayout Layout)
   {
      ParentLayer.ParentMap.ViewLayout = Layout;
   }

   /// <summary>
   /// Sets the current state of a sprite based on a category and index into the category.
   /// </summary>
   /// <param name="Category">Category containing the sprite to be affected</param>
   /// <param name="SpriteIndex">Zero-based index into the category, specifying a sprite</param>
   /// <param name="State">Numeric value referring to a state of the specified sprite.</param>
   [Description("Sets the current state of a sprite based on a category and index into the category.")]
   public virtual void SetCategorySpriteState(SpriteCollection Category, int SpriteIndex, int State)
   {
      Debug.Assert(Category[SpriteIndex].isActive, "SetCategorySpriteState attempted to set the state of an inactive sprite.");
      Category[SpriteIndex].state = State;
   }

   /// <summary>
   /// Turn off the overlay map.
   /// </summary>
   /// <remarks>This disables all drawing and rules in the overlay map.</remarks>
   [Description("Turn off the overlay map. This disables all drawing and rules in the overlay map.")]
   public virtual void ClearOverlay()
   {
      Project.GameWindow.OverlayMap = null;
   }

   /// <summary>
   /// Set the overlay map.
   /// </summary>
   /// <param name="MapType">Specifies a map that should be loaded into the overlay</param>
   /// <remarks>This is very similar to <see cref="SwitchToMap"/>, but it affects the overlay
   /// map instead of the main map.</remarks>
   [Description("Set the overlay map.")]
   public virtual void SetOverlay([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      Project.GameWindow.OverlayMap = Project.GameWindow.GetMap(MapType);
   }

   /// <summary>
   /// Turn on or off a flag associated with the current map.
   /// </summary>
   /// <param name="FlagIndex">A value from 0 to 30 indicating which flag to set or clear</param>
   /// <param name="Value">True to set the flag or false to clear it</param>
   /// <remarks>Each map has a built-in variable <see cref="MapBase.MapFlags"/> that can be used
   /// to store 30 boolean values associated with the map.  This could be used instead of counters
   /// to turn on or off major features of the map, for example, whether the map has been
   /// completed and will allow the player to exit the map without completing it. Such a task is
   /// more suited to a map flag than a counter because there must be exactly one flag per map,
   /// and the flag will automatically be available for each map that is created.
   /// <seealso cref="SetTargetMapFlag"/>
   /// <seealso cref="IsMapFlagOn"/></remarks>
   [Description("Turn on or off a flag associated with the current map.  FlagIndex must be a value from 0 through 30.")]
   public virtual void SetMapFlag(int FlagIndex, bool Value)
   {
      if (Value)
         ParentLayer.ParentMap.MapFlags |= 1 << FlagIndex;
      else
         ParentLayer.ParentMap.MapFlags &= ~(1 << FlagIndex);
   }

   /// <summary>
   /// Turn on or off a flag associated with the specified map.
   /// </summary>
   /// <param name="MapType">Specifies a map whose flag will be set</param>
   /// <param name="FlagIndex">A value from 0 to 30 specifying which flag to set or clear</param>
   /// <param name="Value">True to set the flag or false to clear it</param>
   /// <remarks>This operates like <see cref="SetMapFlag"/>, but can operate on any map
   /// rather than just the current map.  This could be used, for example, to set a flag that
   /// would cause all the sprites on any particular map (even if it is not the current map)
   /// to be frozen next time the player visits it, assuming all sprite definitions check
   /// the flag before allowing the sprite to move.
   /// <seealso cref="SetMapFlag"/>
   /// <seealso cref="IsMapFlagOn"/></remarks>
   [Description("Turn on or off a flag associated with the specified map. FlagIndex must be a value from 0 through 30.")]
   public virtual void SetTargetMapFlag([Editor("MapType", "UITypeEditor")] Type MapType, int FlagIndex, bool Value)
   {
      if (Value)
         Project.GameWindow.GetMap(MapType).MapFlags |= 1 << FlagIndex;
      else
         Project.GameWindow.GetMap(MapType).MapFlags &= ~(1 << FlagIndex);
   }

   /// <summary>
   /// Determine if the specified map-specific flag on the current map is on.
   /// </summary>
   /// <param name="FlagIndex">A number from 0 to 30 specifying which flag to check</param>
   /// <returns>True if the specified flag is set, or false if it is not.</returns>
   [Description("Determine if the specified map-specific flag on the current map is on.")]
   public virtual bool IsMapFlagOn(int FlagIndex)
   {
      return ((ParentLayer.ParentMap.MapFlags & (1 << FlagIndex)) != 0);
   }

   /// <summary>
   /// Unload all maps that aren't currently visible.
   /// </summary>
   /// <remarks>This unloads all maps except the current map and the overlay map.
   /// They will be reset next time they become active.</remarks>
   [Description("Unload all maps that aren't currently visible (as the current map or overlay map).")]
   public virtual void UnloadBackgroundMaps()
   {
      Project.GameWindow.UnloadBackgroundMaps();
   }

   /// <summary>
   /// Quit the game by closing the main window.
   /// </summary>
   [Description("Quit the game by closing the main window.")]
   public virtual void QuitGame()
   {
      Project.GameWindow.Quit();
   }

   /// <summary>
   /// Determines if a specified key is being pressed.
   /// </summary>
   /// <param name="key">Which key to check</param>
   /// <returns>True if the specified key is currently pressed, false otherwise</returns>
   [Description("Returns true if the specified key is currently pressed")]
   public virtual bool IsKeyPressed(Key key)
   {
      return Project.GameWindow.KeyboardState[key];
   }

   /// <summary>
   /// Return a random number within a specified range.
   /// </summary>
   /// <param name="Minimum">Minimum value that can be returned</param>
   /// <param name="Maximum">Maximum bound of the range; this value will never be returned</param>
   /// <returns>A random integer greater than or equal to Minimum and less than Maximum.
   /// This value is based on a random seed that was generated based on the current time when
   /// the program started.</returns>
   [Description("Return a random number greater than or equal to Minimum and less than Maximum.")]
   public virtual int GetRandomNumber(int Minimum, int Maximum)
   {
      return randomGen.Next(Minimum, Maximum);
   }

   /// <summary>
   /// Change a counter's value with a pre-defined operation.
   /// </summary>
   /// <param name="Operation">Specified a pre-defined operation to execute on a counter</param>
   /// <returns>True if the counter value hit a limit, false otherwise. For an operation that
   /// stops at a limit, true will only be returned if the counter was unable to change. For an
   /// operation that loops, true indicates that the counter looped. For operators that set the
   /// counter to a limit, true is returned if the counter was already at the limit value, false
   /// otherwise.</returns>
   /// <remarks>Counter values can be changed directly with the "=" function, but
   /// using a pre-defined operation, you can easily cause the counter to loop when
   /// it hits a limit, which is useful for counters linked to tile animations.</remarks>
   [Description("Change a counter's value with a pre-defined operation. Return true if the counter hits a limit or is left unchanged.")]
   public virtual bool ChangeCounter(Counter Counter, CounterOperation Operation)
   {
      switch (Operation)
      {
         case CounterOperation.IncrementAndStop:
            if (Counter.CurrentValue < Counter.MaxValue)
               Counter.CurrentValue += 1;
            else
               return true;
            return false;
         case CounterOperation.DecrementAndStop:
            if (Counter.CurrentValue > Counter.MinValue)
               Counter.CurrentValue -= 1;
            else
               return true;
            return false;
         case CounterOperation.IncrementAndLoop:
            if (Counter.CurrentValue < Counter.MaxValue)
            {
               Counter.CurrentValue += 1;
               return false;
            }
            Counter.CurrentValue = Counter.MinValue;
            return true;
         case CounterOperation.DecrementAndLoop:
            if (Counter.CurrentValue > Counter.MinValue)
            {
               Counter.CurrentValue -= 1;
               return false;
            }
            Counter.CurrentValue = Counter.MaxValue;
            return true;
         case CounterOperation.SetToMinimum:
            if (Counter.CurrentValue == Counter.MinValue)
               return true;
            Counter.CurrentValue = Counter.MinValue;
            return false;
         case CounterOperation.SetToMaximum:
            if (Counter.CurrentValue == Counter.MaxValue)
               return true;
            Counter.CurrentValue = Counter.MaxValue;
            return false;
      }
      return false;
   }

   /// <summary>
   /// Determines if the specified mouse button is pressed.
   /// </summary>
   /// <param name="Button">Specifies which button to check.</param>
   /// <returns>True if the button is pressed, false if it is not pressed.</returns>
   [Description("Determines if the specified mouse button is pressed.")]
   public virtual bool IsMouseButtonPressed(System.Windows.Forms.MouseButtons Button)
   {
      return 0 != (System.Windows.Forms.Control.MouseButtons & Button);
   }

   /// <summary>
   /// Determines if the specified mouse button is pressed.
   /// </summary>
   /// <param name="Button">Specifies which button to check.</param>
   /// <returns>True if the button is pressed, false if it is not pressed.</returns>
   [Description("Determines if touch was initiated or mouse was clicked this frame.")]
   public virtual bool Clicked()
   {
      return (0 != System.Windows.Forms.Control.MouseButtons) &&
         ((GameForm.oldMouseButtons & System.Windows.Forms.Control.MouseButtons) == 0);
   }

   /// <summary>
   /// When the mouse drags over the display, scroll the map along with it.
   /// </summary>
   [Description("When the mouse drags over the display, scroll the map along with it.")]
   public virtual void DragMap()
   {
      if (0 != (GameForm.oldMouseButtons & GameForm.curMouseButtons & System.Windows.Forms.MouseButtons.Left))
      {
         int mapX = (int)((ParentLayer.CurrentPosition.X - ParentLayer.AbsolutePosition.X) / ParentLayer.ScrollRate.Width);
         int mapY = (int)((ParentLayer.CurrentPosition.Y - ParentLayer.AbsolutePosition.Y) / ParentLayer.ScrollRate.Height);
         int mouseOffsetX = GameForm.curMousePosition.X - GameForm.oldMousePosition.X;
         int mouseOffsetY = Gameform.curMousePosition.Y - GameForm.oldMousePosition.Y;
         Project.GameWindow.CurrentMap.Scroll(new System.Drawing.Point(mapX + mouseOffsetX, mapY + mouseOffsetY));
      }
   }

   #region Collections
   protected static System.Collections.Specialized.HybridDictionary selectedSprites = new System.Collections.Specialized.HybridDictionary();

   /// <summary>
   /// Returns the sprite currently selected for the specified target name.
   /// </summary>
   /// <param name="TargetName">Which target name's currently selected sprite will be returned.</param>
   /// <returns>SpriteBase object or null if no sprite is selected for the specified target.</returns>
   public SpriteBase GetSelectedTargetFor(string TargetName)
   {
      if (selectedSprites.Contains(TargetName))
         return (SpriteBase)selectedSprites[TargetName];
      return null;
   }

   /// <summary>
   /// Selects a sprite within a collection, using a 0-based index, to be the target of <see cref="SetTargetParameter"/>.
   /// </summary>
   /// <param name="Sprites">Collection from which sprite is selected</param>
   /// <param name="Index">0-based index within the collection of the sprite to be selected</param>
   /// <returns>True if the specified index is within the bounds of the collection, False otherwise.</returns>
   /// <remarks>This function is provided for compatibility; see <see cref="SelectTargetSpriteFor"/>.
   /// This function in equivalent to <see cref="SelectTargetSpriteFor"/> with an empty string as TargetName.</remarks>
   [Description("Selects a sprite within a collection, using a 0-based index, to be the target of SetTargetParameter. This function is provided for compatibility; see SelectTargetSpriteFor.")]
   public virtual bool SelectTargetSprite(SpriteCollection Sprites, int Index)
   {
      return SelectTargetSpriteFor(Sprites, Index, String.Empty);
   }

   /// <summary>
   /// Selects a sprite within a collection, using a 0-based index, to be the target of <see cref="SetTargetParameterFor"/>.
   /// <paramref name="TargetName"/> allows any number of unique targets to be selected by assigning unique names.
   /// </summary>
   /// <param name="Sprites">Collection from which sprite is selected</param>
   /// <param name="Index">0-based index within the collection of the sprite to be selected</param>
   /// <param name="TargetName">A global name (quoted string) to indicate for what this sprite is being selected</param>
   /// <returns>True if the specified index is within the bounds of the collection, False otherwise.</returns>
   /// <remarks>If the Index is beyond the bounds of the collection, the target reference cleared.
   /// This function is useful in conjunction with <see cref="SetTargetParameterFor"/> and
   /// <see cref="GetTargetParameterFor"/> to set properties of arbitrary sprites within a
   /// collection to trigger various behaviors based on their rules, or get properties to affect
   /// the currently executing rules based on other sprites.
   /// <paramref name="TargetName"/> allows you to select any number of sprites by assigning each a globally
   /// unique name to be used by any plan or sprite rules. Only one sprite can ever be associated with that
   /// target name at a time. Assigning another to that name will replace the existing target with the new
   /// one. Specify -1 for <paramref name="Index"/> to clear selection for the specified target.
   /// <seealso cref="SelectLastCreatedSpriteFor"/>
   /// <seealso cref="SetTargetParameterFor"/>
   /// <seealso cref="GetTargetParameterFor"/>
   /// <seealso cref="DeactivateTargetSpriteFor"/></remarks>
   [Description("Selects a sprite within a collection, using a 0-based index, to be the target of SetTargetParameterFor. TargetName allows any number of unique targets to be selected by assigning unique names.")]
   public virtual bool SelectTargetSpriteFor(SpriteCollection Sprites, int Index, string TargetName)
   {
      if ((Index >= 0) && (Sprites.Count > Index))
      {
         selectedSprites[TargetName] = Sprites[Index];
         return true;
      }
      else
      {
         selectedSprites[TargetName] = null;
         return false;
      }
   }

   /// <summary>
   /// Select the most recently created sprite to be the target of <see cref="SetTargetParameter"/>.
   /// </summary>
   /// <remarks>This is the same as <see cref="SelectLastCreatedSpriteFor"/> using an empty string
   /// as the Target Name. This function is provided for compatibility; use SelectLastCreatedSpriteFor.
   /// </remarks>
   [Description("Selects the most recently created sprite to be the target of SetTargetParameter.")]
   public virtual void SelectLastCreatedSprite()
   {
      SelectLastCreatedSpriteFor(String.Empty);
   }

   /// <summary>
   /// Selects the most recently created sprite to be the target of <see cref="SetTargetParameterFor"/>
   /// or <see cref="GetTargetParameterFor"/>.
   /// </summary>
   /// <remarks><seealso cref="SetTargetParameterFor"/>
   /// <seealso cref="GetTargetParameterFor"/>
   /// <seealso cref="SelectTargetSpriteFor"/>
   /// <seealso cref="DeactivateTargetSpriteFor"/></remarks>
   [Description("Selects the most recently created sprite to be the target of SetTargetParameterFor.")]
   public virtual void SelectLastCreatedSpriteFor(string TargetName)
   {
      selectedSprites[TargetName] = lastCreatedSprite;
   }

   /// <summary>
   /// Determine if a sprite has been selected with <see cref="SelectLastCreatedSpriteFor"/> 
   /// or <see cref="SelectTargetSpriteFor"/>.
   /// </summary>
   /// <param name="TargetName">Check on whether a sprite has been selected for this target name.</param>
   /// <remarks><seealso cref="SelectTargetSpriteFor"/></remarks>
   [Description("Determines if a sprite has been selected with SelectLastCreatedSpriteFor or SelectTargetSpriteFor.")]
   public virtual bool IsTargetSpriteSelectedFor(string TargetName)
   {
      return selectedSprites.Contains(TargetName);
   }

   /// <summary>
   /// Set the value of a numeric property or parameter on a sprite selected with <see cref="SelectTargetSprite"/> to the specified value.
   /// </summary>
   /// <param name="ParameterName">Name of the parameter on the target sprite that will be affected.</param>
   /// <param name="Value">Numeric value that will be assigned to the specified parameter</param>
   /// <remarks>This function is provided for compatibility; see <see cref="SetTargetParameterFor"/>.
   /// This is equivalent to <see cref="SetTargetParameterFor"/> passing an empty string as Target Name.</remarks>
   [Description("Set the value of a numeric property or parameter (given its name as a string) on a sprite selected with SelectTargetSprite to the specified value. This function is provided for compatibility; see SetTargetParameterFor.")]
   public virtual void SetTargetParameter(string ParameterName, int Value)
   {
      SetTargetParameterFor(ParameterName, Value, String.Empty);
   }

   /// <summary>
   /// Set the value of a numeric property or parameter on a sprite selected with <see cref="SelectTargetSpriteFor"/> to the specified value.
   /// </summary>
   /// <param name="ParameterName">Name of the parameter on the target sprite that will be affected.</param>
   /// <param name="Value">Numeric value that will be assigned to the specified parameter.</param>
   /// <param name="TargetName">Determines which target's currently selected sprite will be affected.</param>
   /// <remarks>This is useful for a wide range of effects on sprites in collections. For example,
   /// after determining that some sprite in a collection is within a plan using
   /// <see cref="PlanBase.GetSpriteWithin"/>, you could use <see cref="SelectTargetSpriteFor"/> and
   /// this to set some parameter on that sprite to trigger its rules to react.
   /// Remember that ParameterName and TargetName are provided as strings,
   /// and must be quoted assuming the names are provided as literal values.
   /// <seealso cref="GetTargetParameterFor"/>
   /// <seealso cref="SelectTargetSpriteFor"/>
   /// <seealso cref="SelectLastCreatedSpriteFor"/></remarks>
   [Description("Set the value of a numeric property or parameter (given its name as a string) on a sprite selected with SelectTargetSpriteFor to the specified value.")]
   public virtual void SetTargetParameterFor(string ParameterName, int Value, string TargetName)
   {
      if (!selectedSprites.Contains(TargetName))
      {
         Debug.Fail("Tried to set parameter for non-existent target \"" + TargetName + "\".");
         return;
      }
      object selectedTarget = selectedSprites[TargetName];
      if (selectedTarget == null)
      {
         Debug.Fail("Tried to set parameter for target \"" + TargetName + "\" that was cleared.");
         return;
      }
      System.Reflection.PropertyInfo pi = selectedTarget.GetType().GetProperty(ParameterName);
      if (pi == null)
      {
         System.Reflection.FieldInfo fi = selectedTarget.GetType().GetField(ParameterName);
         Debug.Assert(fi != null, "Invalid property name in SetTargetParameter");
         if (fi != null)
            fi.SetValue(selectedTarget, Value);
      }
      else
         pi.SetValue(selectedTarget, Value, null);
   }

   /// <summary>
   /// Get the value of a numeric property or parameter on a sprite selected with <see cref="SelectTargetSpriteFor"/>.
   /// </summary>
   /// <param name="ParameterName">Name of the parameter on the target sprite that will be returned.</param>
   /// <param name="TargetName">Which target's currently selected sprite contains the parameter being retrieved.</param>
   /// <returns>Integer property value of specified property.</returns>
   /// <remarks>Remember that ParameterName and TargetName are provided as strings,
   /// and must be quoted assuming the names are provided directly.
   /// A target is an arbitrary name that distinguishes a sprite selected for one purpose from
   /// sprites selected for other purposes.  A target is defined by calling <see cref="SelectTargetSpriteFor"/>.
   /// <seealso cref="SetTargetParameterFor"/>
   /// <seealso cref="SelectTargetSpriteFor"/>
   /// <seealso cref="SelectLastCreatedSpriteFor"/></remarks>
   [Description("Get the value of a numeric property or parameter (given its name as a string) from a sprite selected with SelectTargetSprite.")]
   public virtual int GetTargetParameterFor(string ParameterName, string TargetName)
   {
      if (!selectedSprites.Contains(TargetName))
      {
         Debug.Fail("Tried to get parameter for non-existent target \"" + TargetName + "\".");
         return 0;
      }
      object selectedTarget = selectedSprites[TargetName];
      if (selectedTarget == null)
      {
         Debug.Fail("Tried to get parameter for target \"" + TargetName + "\" that was cleared.");
         return 0;
      }
      System.Reflection.PropertyInfo pi = selectedTarget.GetType().GetProperty(ParameterName);
      if (pi == null)
      {
         System.Reflection.FieldInfo fi = selectedTarget.GetType().GetField(ParameterName);
         Debug.Assert(fi != null, "Invalid property name in GetTargetParameter");
         if (fi != null)
            return (int)fi.GetValue(selectedTarget);
      }
      else
         return (int)pi.GetValue(selectedTarget, null);
      return 0;
   }

   /// <summary>
   /// Deactivate the sprite currently selected as the target for the specified name and clear the sprite selected for that target name.
   /// </summary>
   /// <param name="TargetName">For which target name the currently selected sprite should be terminated.</param>
   /// <returns>True if a sprite was deactivated, false otherwise.</returns>
   /// <remarks>Does nothing if the sprite is already inactive or no sprite is selected for the specified target name.
   /// <seealso cref="SelectTargetSpriteFor"/>
   /// <seealso cref="SelectLastCreatedSpriteFor"/></remarks>
   [Description("Deactivate the sprite currently selected as the target for the specified name.")]
   public virtual bool DeactivateTargetSpriteFor(string TargetName)
   {
      if (selectedSprites.Contains(TargetName))
      {
         object s = selectedSprites[TargetName];
         if (s == null)
            return false;
         bool result;
         SpriteBase sb = (SpriteBase)s;
         result = sb.isActive;
         sb.Deactivate();
         selectedSprites.Remove(TargetName);
         return result;
      }
      return false;
   }

   /// <summary>
   /// Determine if the sprite selected for the specified target (selected with SelectTargetSpriteFor)
   /// is of the specified type.
   /// </summary>
   /// <returns>
   /// True if the selected target is of the specified type, false otherwise.
   /// </returns>
   [Description("Determine if the sprite selected for the specified target (selected with SelectTargetSpriteFor) is of the specified type.")]
   public bool IsSpriteForTargetOfType(string TargetName, [Editor("SpriteDefinition", "UITypeEditor")] System.Type SpriteDefinition)
   {
      if (!selectedSprites.Contains(TargetName))
         return false;
      return SpriteDefinition.IsInstanceOfType(selectedSprites[TargetName]);
   }
   #endregion

   #region "Messages"
   /// <summary>
   /// Determines in which view(s) a message will appear.
   /// </summary>
   public enum MessageView
   {
      /// <summary>
      /// Display messages in the view that is active when the message is created.
      /// </summary>
      Current,
      /// <summary>
      /// Display messages in all views
      /// </summary>
      All,
      /// <summary>
      /// Display messages in the first (top or left) view
      /// </summary>
      First,
      /// <summary>
      /// Display messages in the second view: bottom or right in 2-view layout, top-right in 4-view layout.
      /// </summary>
      Second,
      /// <summary>
      /// Display messages in the bottom-left view
      /// </summary>
      Third,
      /// <summary>
      /// Display messages in the bottom-right view
      /// </summary>
      Fourth
   }
   /// <summary>
   /// Specifies a button or buttons on a player's input controller.
   /// </summary>
   [Flags()]
   public enum ButtonSpecifier
   {
      /// <summary>
      /// The first button as defined by the player options
      /// </summary>
      First = 1,
      /// <summary>
      /// The second button as defined by the player options
      /// </summary>
      Second = 2,
      /// <summary>
      /// The third button as defined by the player options
      /// </summary>
      Third = 4,
      /// <summary>
      /// The fourth button as defined by the player options
      /// </summary>
      Fourth = 8,
      /// <summary>
      /// Disable input from the player while waiting for a button;
      /// prevent it from affecting the player's sprite.
      /// </summary>
      FreezeInputs = 16
   }

   protected static Tileset FontTileset = null;
   protected const int maxMessages = 4;
   protected static MessageLayer[] activeMessages = new MessageLayer[maxMessages];
   protected static int activeMessageCount = 0;
   protected static System.Drawing.Color messageBackground = System.Drawing.Color.FromArgb(128, 64, 0, 255);
   protected static MessageView msgView = MessageView.Current;
   protected static RelativePosition msgPos = RelativePosition.CenterMiddle;
   protected const int messageMargin = 6;
   /// <summary>
   /// Zero-based player index that will be assigned to newly created messages
   /// </summary>
   protected static int currentPlayer = 0;
   protected static ButtonSpecifier dismissButton = ButtonSpecifier.First | ButtonSpecifier.FreezeInputs;
   protected static byte[] dismissPhase = null;

   /// <summary>
   /// Represents a message created and displayed by <see cref="ShowMessage"/>.
   /// </summary>
   public partial class MessageLayer : ByteLayer
   {
      public readonly System.Drawing.Color background;
      public MessageView view;
      public ButtonSpecifier dismissButton;
      /// <summary>
      /// 0-based player index whose controls affect this message
      /// </summary>
      public int player;

      /// <summary>
      /// Creates a message layer object
      /// </summary>
      /// <param name="Tileset">Each tile in this tileset represents a unicode character starting with
      /// tile number 0 representing unicode character 0.</param>
      /// <param name="Parent">Map that will host this layer.</param>
      /// <param name="nColumns">Number of columns of text this layer can represent.</param>
      /// <param name="nRows">Number of rows of text this layer can represent.</param>
      /// <param name="Position">Position of the top-left corner of this layer within the map.</param>
      /// <param name="background">Background color for the box containing this message.</param>
      /// <param name="player">0-based player number whose button can dismiss this message.</param>
      /// <param name="dismissButton">Which of the player's buttons can dismiss this message.</param>
      /// <param name="msgView">Which view(s) will the message appear in.</param>
      public MessageLayer(Tileset Tileset, MapBase Parent, int nColumns, int nRows,
         System.Drawing.Point Position, System.Drawing.Color background, int player,
         ButtonSpecifier dismissButton, MessageView msgView) :
         base(Tileset, Parent, 0, 0, 0, 0, nColumns, nRows, 0, 0, Position,
         new System.Drawing.SizeF(0, 0), 0, 0, null)
      {
         this.background = background;
         this.player = player;
         this.dismissButton = dismissButton;
         if (msgView == MessageView.Current)
         {
            switch (Parent.CurrentViewIndex)
            {
               case 0:
                  view = MessageView.First;
                  break;
               case 1:
                  view = MessageView.Second;
                  break;
               case 2:
                  view = MessageView.Third;
                  break;
               case 3:
                  view = MessageView.Fourth;
                  break;
            }
         }
         else
            view = msgView;
      }
   }

   /// <summary>
   /// Handles button pressses from a player with respect to displayed messages
   /// </summary>
   /// <param name="playerNumber">1-based player index</param>
   /// <param name="player">Player object providing the inputs.</param>
   /// <returns>True if input can be passed to the player or false if the player
   /// is "frozen" viewing a message.</returns>
   public static bool PlayerPressButton(int playerNumber, IPlayer player)
   {
      for (int i = 0; i < activeMessageCount; i++)
      {
         MessageLayer msg = activeMessages[i];
         if (msg.player == playerNumber - 1)
         {
            bool dismissPressed = false;
            if ((0 != (msg.dismissButton & ButtonSpecifier.First)) && player.Button1)
               dismissPressed = true;
            if ((0 != (msg.dismissButton & ButtonSpecifier.Second)) && player.Button2)
               dismissPressed = true;
            if ((0 != (msg.dismissButton & ButtonSpecifier.Third)) && player.Button3)
               dismissPressed = true;
            if ((0 != (msg.dismissButton & ButtonSpecifier.Fourth)) && player.Button4)
               dismissPressed = true;

            // dismissPhase[x]:
            // 0 = No frames have passed yet
            // 1 = Frames have passed and the dismiss button was initially pressed
            // 2 = Frames have passed and the dismiss button is not pressed
            // 3 = Dismiss button was not pressed, but now it is.

            if (dismissPhase == null)
               dismissPhase = new byte[Project.MaxPlayers];

            if (dismissPressed)
            {
               if ((dismissPhase[msg.player] == 0) || (dismissPhase[msg.player] == 2))
                  dismissPhase[msg.player]++;
            }
            else
            {
               if (dismissPhase[msg.player] < 2)
                  dismissPhase[msg.player] = 2;
               else if (dismissPhase[msg.player] > 2)
               {
                  DismissMessage(i);
                  dismissPhase[msg.player] = 0;
               }
            }

            if (0 != (msg.dismissButton & ButtonSpecifier.FreezeInputs))
            {
               return false;
            }
         }
      }
      return true;
   }

   private static void DismissMessage(int messageIndex)
   {
      for (int i = messageIndex; i < activeMessageCount - 1; i++)
         activeMessages[i] = activeMessages[i + 1];
      activeMessageCount--;
   }

   /// <summary>
   /// Sets the background for new messages added with <see cref="ShowMessage"/>.
   /// </summary>
   /// <param name="background">Names a color for the background of new messages.</param>
   /// <param name="alpha">Transparency level of the color: 255 = opaque, 128=50% transparent.</param>
   [Description("Sets the background for new messages added with ShowMessage. Alpha 255 = opaque, alpha 128=50% transparent.")]
   public virtual void SetMessageBackground(System.Drawing.KnownColor background, byte alpha)
   {
      System.Drawing.Color c = System.Drawing.Color.FromKnownColor(background);
      messageBackground = System.Drawing.Color.FromArgb(alpha, c.R, c.G, c.B);
   }

   /// <summary>
   /// Determines where newly created messages appear.
   /// </summary>
   /// <param name="ViewOption">Determines which view or views messages will appear in.</param>
   /// <param name="Position">Determines the area within the view in which the message appears.</param>
   [Description("Determines where newly created messages appear.")]
   public virtual void SetMessagePosition(MessageView ViewOption, RelativePosition Position)
   {
      msgView = ViewOption;
      msgPos = Position;
   }

   /// <summary>
   /// Determines which player and which button will dismiss newly created messages.
   /// </summary>
   /// <param name="DismissButton">Which of the player's buttons will dismiss the message</param>
   /// <param name="Player">Player number 1 to 4</param>
   [Description("Determines which player and which button will dismiss newly created messages. Player is a number 1 to 4.")]
   public virtual void SetMessageDismissal(ButtonSpecifier DismissButton, int Player)
   {
      dismissButton = DismissButton;
      currentPlayer = Player - 1;
   }

   /// <summary>
   /// Adds a message to the display.
   /// </summary>
   /// <param name="Message">Message text as a quoted string.  Use \r\n to insert new lines
   /// into the message.</param>
   /// <remarks>Up to 4 messages may be displayed.  No automatic word wrap or centering
   /// is performed.  All formatting is determined by the content of the string.</remarks>
   [Description("Adds a message to the display. Up to 4 messages may be displayed.")]
   public virtual void ShowMessage([Editor("Message", "UITypeEditor")] string Message)
   {
      if (activeMessageCount >= maxMessages)
         throw new InvalidOperationException("Maximum number of displayed messages exceeded");
      activeMessages[activeMessageCount++] = CreateMessage(Message);
   }

   /// <summary>
   /// Clears all active messages from the display.
   /// </summary>
   [Description("Clears all active messages from the display")]
   public virtual void ClearAllMessages()
   {
      activeMessageCount = 0;
   }

   /// <summary>
   /// Draws all active messages.
   /// </summary>
   /// <remarks>This function is called by the framework after drawing the overlay map.</remarks>
   public static void DrawMessages()
   {
      for (int i = 0; i < activeMessageCount; i++)
      {
         MessageLayer msg = activeMessages[i];
         Display disp = msg.ParentMap.Display;
         byte oldView = msg.ParentMap.CurrentViewIndex;
         switch (msg.view)
         {
            case MessageView.Current:
               DrawMessage(msg, disp);
               break;
            case MessageView.All:
               for (byte v = 0; v < Project.MaxViews; v++)
               {
                  msg.ParentMap.CurrentViewIndex = v;
                  DrawMessage(msg, disp);
               }
               break;
            case MessageView.First:
               msg.ParentMap.CurrentViewIndex = 0;
               DrawMessage(msg, disp);
               break;
            case MessageView.Second:
               msg.ParentMap.CurrentViewIndex = 1;
               DrawMessage(msg, disp);
               break;
            case MessageView.Third:
               msg.ParentMap.CurrentViewIndex = 2;
               DrawMessage(msg, disp);
               break;
            case MessageView.Fourth:
               msg.ParentMap.CurrentViewIndex = 3;
               DrawMessage(msg, disp);
               break;
         }
         msg.ParentMap.CurrentViewIndex = oldView;
      }
   }

   protected static void DrawMessage(MessageLayer msg, Display disp)
   {
      disp.Scissor(msg.ParentMap.CurrentView);
      System.Drawing.Rectangle messageRect = new System.Drawing.Rectangle(
         msg.CurrentPosition.X + msg.ParentMap.CurrentView.X,
         msg.CurrentPosition.Y + msg.ParentMap.CurrentView.Y,
         msg.VirtualColumns * msg.Tileset.TileWidth,
         msg.VirtualRows * msg.Tileset.TileHeight);
      messageRect.Inflate(messageMargin, messageMargin);
      disp.SetColor(msg.background);
      disp.FillRectangle(messageRect);
      disp.SetColor(System.Drawing.Color.White);
      disp.DrawRectangle(messageRect, 0);
      msg.Draw();
   }

   /// <summary>
   /// Set the tileset used as the source for characters in messages.
   /// </summary>
   /// <param name="Tileset">Tileset whose tiles will be used to represent characters
   /// for messages. The tile numbers correspond to ASCII values of the characters
   /// used in the messages.</param>
   [Description("Set the tileset used as the source for characters in messages")]
   public virtual void SetMessageFont(Tileset Tileset)
   {
      FontTileset = Tileset;
   }

   protected virtual MessageLayer CreateMessage(string Message)
   {
      if (FontTileset == null)
         FontTileset = (Tileset)(typeof(Tileset).GetProperties(
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public)
            [0].GetValue(null, null));

      byte[] charBytes = System.Text.Encoding.Unicode.GetBytes(Message);
      Display disp = ParentLayer.ParentMap.Display;
      int x = 0, y = 1;
      int maxWidth = 1;
      for (int charIdx = 0; charIdx < charBytes.Length; charIdx += 2)
      {
         if (Message[charIdx / 2] == '\n')
         {
            x = 0;
            y++;
         }
         else if (Message[charIdx / 2] != '\r')
         {
            if (++x > maxWidth)
               maxWidth = x;
         }
      }

      System.Drawing.Size messageSize = new System.Drawing.Size(
         maxWidth * FontTileset.TileWidth, y * FontTileset.TileHeight);
      System.Drawing.Size viewSize = ParentLayer.ParentMap.CurrentView.Size;

      System.Drawing.Point ptMessage = System.Drawing.Point.Empty;

      switch (msgPos)
      {
         case RelativePosition.TopLeft:
         case RelativePosition.LeftMiddle:
         case RelativePosition.BottomLeft:
            ptMessage.X = viewSize.Width / 4 - messageSize.Width / 2;
            if (ptMessage.X < messageMargin)
               ptMessage.X = messageMargin;
            break;
         case RelativePosition.TopCenter:
         case RelativePosition.CenterMiddle:
         case RelativePosition.BottomCenter:
            ptMessage.X = (viewSize.Width - messageSize.Width) / 2;
            break;
         case RelativePosition.TopRight:
         case RelativePosition.RightMiddle:
         case RelativePosition.BottomRight:
            ptMessage.X = viewSize.Width * 3 / 4 - messageSize.Width / 2;
            if (ptMessage.X + messageSize.Width > viewSize.Width - messageMargin)
               ptMessage.X = viewSize.Width - messageSize.Width - messageMargin;
            break;
      }

      switch (msgPos)
      {
         case RelativePosition.TopLeft:
         case RelativePosition.TopCenter:
         case RelativePosition.TopRight:
            ptMessage.Y = viewSize.Height / 4 - messageSize.Height / 2;
            if (ptMessage.Y <= 0)
               ptMessage.Y = 1;
            break;
         case RelativePosition.LeftMiddle:
         case RelativePosition.CenterMiddle:
         case RelativePosition.RightMiddle:
            ptMessage.Y = (viewSize.Height - messageSize.Height) / 2;
            break;
         case RelativePosition.BottomLeft:
         case RelativePosition.BottomCenter:
         case RelativePosition.BottomRight:
            ptMessage.Y = viewSize.Height * 3 / 4 - messageSize.Height / 2;
            if (ptMessage.Y + messageSize.Height >= viewSize.Height)
               ptMessage.Y = viewSize.Height - messageSize.Height - 1;
            break;
      }

      MessageLayer result = new MessageLayer(
         FontTileset, ParentLayer.ParentMap, maxWidth, y, ptMessage,
         messageBackground, currentPlayer, dismissButton, msgView);

      x = 0;
      y = 0;
      for (int charIdx = 0; charIdx < charBytes.Length; charIdx += 2)
      {
         if (Message[charIdx / 2] == '\n')
         {
            x = 0;
            y++;
         }
         else if (Message[charIdx / 2] != '\r')
         {
            result[x++, y] = charBytes[charIdx];
         }
      }

      return result;
   }
   #endregion
}

/// <summary>
/// Specifies an operation to perform on a counter.
/// </summary>
public enum CounterOperation
{
   /// <summary>
   /// Add 1 to the counter value. If the counter was at it's maximum value, leave it there.
   /// </summary>
   IncrementAndStop,
   /// <summary>
   /// Subtract 1 from the counter value. If the counter was at it's minimum value, leave it there.
   /// </summary>
   DecrementAndStop,
   /// <summary>
   /// Add 1 to the counter value. If the counter was at it's maximum value, set it to its minimum value.
   /// </summary>
   IncrementAndLoop,
   /// <summary>
   /// Subtract 1 from the counter value. If the counter was at it's minimum value, set it to its maximum value.
   /// </summary>
   DecrementAndLoop,
   /// <summary>
   /// Set the counter to its minimum value.
   /// </summary>
   SetToMinimum,
   /// <summary>
   /// Set the counter to its maximum value.
   /// </summary>
   SetToMaximum
}

/// <summary>
/// This is used to specify general categories of objects to be included
/// in a saved game (save unit).
/// </summary>
/// <remarks>This is used with <see cref="GeneralRules.IncludeInSaveUnit"/>.
/// <seealso cref="GeneralRules.IncludeInSaveUnit"/>
/// <seealso cref="GeneralRules.LoadGame"/></remarks>
public enum SaveUnitInclusion
{
   /// <summary>
   /// Includes all loaded maps.
   /// </summary>
   /// <remarks>Including all maps in a <see cref="SaveUnit"/> causes all maps to be loaded
   /// or reset when the game is loaded. Maps that were in memory when the game was saved
   /// will be loaded from the file, while the rest will be reset. In that sense, it is
   /// including even maps that aren't loaded because it causes all maps to be reset when the
   /// game is loaded, and then only the stored maps are restored from the file.</remarks>
   AllMaps,
   /// <summary>
   /// Includes the values of all counters defined in the project.
   /// </summary>
   AllCounters,
   /// <summary>
   /// Includes an indicator of which map was the currently active map when the game was saved.
   /// </summary>
   /// <remarks>If this is not included, the current map will be the same map that was active
   /// before the game was loaded, which may be reset if that map was not included in the
   /// <see cref="SaveUnit"/>. The indicator is determined at the time that <see cref="GeneralRules.IncludeInSaveUnit"/>
   /// is called, unlike most other members which are stored at the time <see cref="GeneralRules.SaveGame"/>
   /// is called.</remarks>
   WhichMapIsCurrent,
   /// <summary>
   /// Includes an indicator of which map was set as the overlay map when the game was saved.
   /// </summary>
   /// <remarks>If this is not included, the overlay map will remain unchanged when loading
   /// the game stored in this save unit. The indicator is determined at the time that
   /// <see cref="GeneralRules.IncludeInSaveUnit"/> is called, unlike most other members
   /// which are stored at the time <see cref="GeneralRules.SaveGame"/> is called.</remarks>
   WhichMapIsOverlaid,
   /// <summary>
   /// Includes player preferences that determine which input devices the players are using and
   /// the keyboad layout.
   /// </summary>
   PlayerOptions
}

/// <summary>
/// Specifies an arrangement for multiple scrolling views on a single display
/// </summary>
/// <remarks>This is used with <see cref="GeneralRules.SetViewLayout"/>.
/// There is no space between the views in any of these layouts, but an overlay layer could
/// be used to draw separators on top of the views.
/// <seealso cref="GeneralRules.SetViewLayout"/></remarks>
public enum ViewLayout
{
   /// <summary>
   /// A single view that fills the display
   /// </summary>
   Single,
   /// <summary>
   /// Two views side by side, equally split
   /// </summary>
   LeftRight,
   /// <summary>
   /// Two views, one above the other, equally split
   /// </summary>
   TopBottom,
   /// <summary>
   /// Four views occupying the four corners of the display, equally split
   /// </summary>
   FourCorners
}

/// <summary>
/// Retains information about what has been included for a saved game.
/// </summary>
/// <remarks>The information included in a SaveUnit only selects which information
/// will be saved, but does not actually store a copy of it. The data for these objects
/// is copied from the objects referenced in the SaveUnit at the time that
/// <see cref="GeneralRules.SaveGame"/> is called. One exception is <see cref="SaveUnitInclusion.WhichMapIsCurrent"/>,
/// which stores an indicator of the current map at the time <see cref="GeneralRules.IncludeInSaveUnit"/> is
/// called.</remarks>
[Serializable()]
public partial class SaveUnit
{
   public SaveUnit()
   {
   }
   /// <summary>
   /// Indicates whether <see cref="SaveUnitInclusion.AllMaps"/> was included.
   /// </summary>
   /// <remarks>This is significant because <see cref="GeneralRules.LoadGame"/> behaves differently
   /// with respect to maps that were not stored in the <see cref="SaveUnit"/> when this
   /// is true or false. If all maps were included, then maps that were not stored are
   /// reset while the game ie being loaded, otherwise they are left as they were before
   /// the game was loaded.</remarks>
   public bool AllMaps = false;
   /// <summary>
   /// Refers to all maps that will be saved
   /// </summary>
   /// <remarks>The key of this collection is the type of the map and the value refers to the
   /// instance of the map data.</remarks>
   public System.Collections.Hashtable Maps = null;
   /// <summary>
   /// This indicator remembers which map was current, or is null if the indicator is not saved
   /// </summary>
   public System.Type CurrentMapType = null;
   /// <summary>
   /// This indicator remembers which map was used as an overlay.  It's null if the indicator is
   /// not saved, and typeof(System.DBNull) if the overlay map is empty.
   /// </summary>
   public System.Type OverlayMapType = null;
   /// <summary>
   /// An array of <see cref="CounterRef"/> objects used to include counters in a <see cref="SaveUnit"/>
   /// </summary>
   public System.Collections.ArrayList Counters = null;
   /// <summary>
   /// Stores player preferenes.
   /// </summary>
   public IPlayer[] PlayerOptions = null;
}

/// <summary>
/// This class provides a kind of indirect reference to a counter for the purposes
/// of saving and loading counter values in a <see cref="SaveUnit"/>.
/// </summary>
/// <remarks>Since counters are global objects, it doesn't work well to add a counter
/// directly to the save unit because then loading the save unit will leave the
/// loaded counter values contained in the <see cref="SaveUnit"/> object. But by
/// providing this wrapper that has code specifically for loading ans storing counters,
/// the value of the global counters can automatically be linked to those in a
/// <see cref="SaveUnit"/>.</remarks>
[Serializable()]
public partial class CounterRef : System.Runtime.Serialization.ISerializable
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
      foreach (System.Reflection.PropertyInfo counterProp in counterProps)
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

/// <summary>
/// Refers to the position of a predefined point relative to a rectangle
/// </summary>
/// <remarks>Many operations involving the position of one object relative to another
/// object rely on RelativePosition to determine how the two objects' rectangles are
/// aligned. The positions defined in this enumeration generally refer to
/// a point inside the rectangle at the specified position. For example, RightMiddle
/// would refer to the point immediately within the middle of the right side of the
/// rectangle. In the case of aligning two rectangles, this means that the right
/// sides will match up exactly and the vertical center of one rectangle would be
/// aligned with the vertical center of the other.</remarks>
public enum RelativePosition
{
   TopLeft,
   TopCenter,
   TopRight,
   LeftMiddle,
   CenterMiddle,
   RightMiddle,
   BottomLeft,
   BottomCenter,
   BottomRight
}