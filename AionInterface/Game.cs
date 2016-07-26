// Decompiled with JetBrains decompiler
// Type: AionInterface.Game
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using Keiken.Executable;
using Keiken.Interoperability;
using Reflex.Memory.Resolving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AionInterface
{
  public class Game : Global
  {
    protected static bool _bInfinite = false;
    protected static bool _bLimited = true;
    protected static ArrayList _hListenerList = new ArrayList();
    protected static Thread _hThread = (Thread) null;
    protected static Thread _hThreadAuthorization = (Thread) null;
    protected static uint _iAuthorize = 0;
    protected static ulong _iBase = 0;
    protected static uint _iTime = 0;
    protected static string _zAccountPass = (string) null;
    public static AbilityList AbilityList = (AbilityList) null;
    public static DialogList DialogList = (DialogList) null;
    public static EntityList EntityList = (EntityList) null;
    public static ForceList ForceList = (ForceList) null;
    public static InventoryList InventoryList = (InventoryList) null;
    public static string Name = (string) null;
    public static Player Player = (Player) null;
    public static PlayerInput PlayerInput = (PlayerInput) null;
    public static ProcessCommunication Process = new ProcessCommunication();
    public static Reflex.Memory.Resolving.Resolver Resolver = (Reflex.Memory.Resolving.Resolver) null;
    public static SkillList SkillList = (SkillList) null;
    public static TravelList TravelList = (TravelList) null;

    public static event AionEventHandler OnAbility;

    public static event AionEventHandler OnCamera;

    public static event AionEventHandler OnClose;

    public static event AionEventHandler OnEntities;

    public static event AionEventHandler OnFrame;

    public static event AionEventHandler OnInitialize;

    public static event AionEventOpenHandler OnOpen;

    public static event AionEventHandler OnPlayer;

    public static event AionEventHandler OnPosition;

    protected static void _Active()
    {
      byte num1 = 0;
      uint num2 = 0;
      uint num3 = 0;
      uint num4 = 0;
      uint num5 = 0;
      uint num6 = 0;
      while (true)
      {
        DateTime now = new DateTime();
        uint num7;
        do
        {
          if (!Game.Process.Available())
          {
            HashSet<uint> hResultList;
            if ((hResultList = Game.Process.Open("aion.bin", ProcessAccessRights.CreateThread | ProcessAccessRights.QueryInformation | ProcessAccessRights.VMOperation | ProcessAccessRights.VMRead | ProcessAccessRights.VMWrite)) != null && hResultList.Count >= 2)
            {
              // ISSUE: reference to a compiler-generated field
              if (Game.OnOpen != null)
              {
                // ISSUE: reference to a compiler-generated field
                Game.OnOpen(Game._List(hResultList));
              }
              Game._Close(false);
              Thread.Sleep(250);
              continue;
            }
            if (hResultList.Count == 0)
            {
              Thread.Sleep(5000);
              continue;
            }
            Game.Process.Open(Enumerable.ElementAt<uint>((IEnumerable<uint>) hResultList, 0));
            if (!Game.LoadVersion())
            {
              Game.Process.Close();
              Thread.Sleep(5000);
              continue;
            }
          }
          if ((long) Game._iBase == 0L && (long) (Game._iBase = Game._Base(Game.Process)) == 0L)
          {
            Game._Close(false);
            Game._iBase = 0UL;
            num1 = (byte) 0;
            Thread.Sleep(1000);
          }
          else
          {
            if ((int) new Player(Game._iBase).GetID(Game._iBase) != 0 && (Game.Player != null || new Player(Game._iBase).GetName(Game._iBase).Length >= 1))
            {
              Game._iTime = Game.Time();
              if ((int) Game._iTime != 0 && Game.Name != null)
                Game._iAuthorize = Game._iTime + 600000U;
              if ((int) num2 == 0 || num2 + 500U < Game._iTime)
              {
                if (Game.AbilityList == null || Game.EntityList == null || (Game.InventoryList == null || Game.ForceList == null) || (Game.Player == null || Game.PlayerInput == null))
                {
                  Game.AbilityList = new AbilityList(Game._iBase);
                  Game.EntityList = new EntityList(Game._iBase);
                  Game.DialogList = new DialogList(Game._iBase);
                  Game.InventoryList = new InventoryList(Game._iBase);
                  Game.ForceList = new ForceList(Game._iBase);
                  Game.Player = new Player(Game._iBase).Update();
                  Game.PlayerInput = new PlayerInput(Game._iBase);
                  // ISSUE: reference to a compiler-generated field
                  if (Game.OnInitialize != null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    Game.OnInitialize();
                  }
                  try
                  {
                    if (File.ReadAllText(Path.Combine(Game.GetLocation(), "..\\cc.ini")).Contains("cc = \"1\""))
                    {
                      if (File.Exists("Setting\\Skill.NA.xml"))
                      {
                        Global.Write("[TEST] Aion NA detected. Switching Skill.xml to NA.");
                        Game.SkillList = new SkillList("Setting\\Skill.NA.xml");
                        Global.Write("[TEST] Skills switched successfully.");
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                  }
                }
                num3 = num2 = Game._iTime;
                Game.EntityList.Update();
                // ISSUE: reference to a compiler-generated field
                if (Game.OnEntities != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  Game.OnEntities();
                }
              }
              else if ((int) num3 == 0 || num3 + 100U < Game._iTime)
              {
                foreach (KeyValuePair<uint, Entity> keyValuePair in Game.EntityList.GetList())
                {
                  Entity entity = keyValuePair.Value;
                  if (entity.IsPlayer() || entity.IsMonster())
                  {
                    double num8 = (double) entity.UpdateRotation();
                    entity.UpdatePosition();
                  }
                }
                num3 = Game._iTime;
                // ISSUE: reference to a compiler-generated field
                if (Game.OnPosition != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  Game.OnPosition();
                }
              }
              if ((int) num4 == 0 || num4 + 250U < Game._iTime)
              {
                num4 = Game._iTime;
                Game.AbilityList.UpdateActivated();
                Game.DialogList.Update();
                Game.ForceList.Update();
                Game.InventoryList.Update();
                Game.Player.Update();
                if ((int) num1 == 0 || (int) num1 != (int) Game.Player.GetLevel() || num6 < Game.Time())
                {
                  num1 = Game.Player.GetLevel();
                  Game.AbilityList.Update();
                  num6 = Game.Time() + 5000U;
                  // ISSUE: reference to a compiler-generated field
                  if (Game.OnAbility != null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    Game.OnAbility();
                  }
                }
                // ISSUE: reference to a compiler-generated field
                if (Game.OnCamera != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  Game.OnCamera();
                }
                // ISSUE: reference to a compiler-generated field
                if (Game.OnPlayer != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  Game.OnPlayer();
                }
              }
              else if ((int) num5 == 0 || num5 + 50U < Game._iTime)
              {
                Game.Player.UpdateCamera();
                Game.Player.UpdatePosition();
                // ISSUE: reference to a compiler-generated field
                if (Game.OnCamera != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  Game.OnCamera();
                }
              }
              Game.Player.UpdateMove();
              Game.PlayerInput.Update();
              // ISSUE: reference to a compiler-generated field
              if (Game.OnFrame != null)
              {
                // ISSUE: reference to a compiler-generated field
                Game.OnFrame();
              }
            }
            else
            {
              Game._Close(false);
              Game._iBase = 0UL;
              num1 = (byte) 0;
            }
            now = DateTime.Now;
          }
        }
        while ((num7 = (uint) ((ulong) now.Ticks / 10000UL) - Game._iTime) >= 50U || num7 < 0U);
        Thread.Sleep(50 - (int) num7);
      }
    }

    public static ulong _Base(ProcessCommunication hProcess)
    {
      hProcess.ProcessEnvironment.Refresh();
      Image image = hProcess["Game.dll"];
      if (image != null)
        return image.Address;
      return 0;
    }

    protected static int _Close(bool bCloseMemory = false)
    {
      Game.AbilityList = (AbilityList) null;
      Game.DialogList = (DialogList) null;
      Game.EntityList = (EntityList) null;
      Game.InventoryList = (InventoryList) null;
      Game.ForceList = (ForceList) null;
      Game.Player = (Player) null;
      if (bCloseMemory)
        Game.Process.Close();
      // ISSUE: reference to a compiler-generated field
      if (Game.OnClose != null)
      {
        // ISSUE: reference to a compiler-generated field
        Game.OnClose();
      }
      return 0;
    }

    protected static ArrayList _List(HashSet<uint> hResultList)
    {
      ArrayList arrayList1 = new ArrayList();
      for (int index = 0; index < hResultList.Count; ++index)
      {
        uint iProcessId = Enumerable.ElementAt<uint>((IEnumerable<uint>) hResultList, index);
        ProcessCommunication hProcess = new ProcessCommunication();
        ProcessCommunication processCommunication = Game.Process;
        if (hProcess.Open(iProcessId))
        {
          ulong iBase = Game._Base(hProcess);
          if ((long) iBase != 0L)
          {
            Game.Process = hProcess;
            if (Game.LoadVersion())
            {
              Player player = new Player(iBase).Update();
              Game.Process = processCommunication;
              ArrayList arrayList2 = new ArrayList()
              {
                (object) iProcessId,
                (object) player.GetID(iBase),
                (object) player.GetName(0UL),
                (object) player.GetLevel()
              };
              arrayList1.Add((object) arrayList2);
            }
            else
              continue;
          }
          hProcess.Close();
        }
      }
      return arrayList1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ulong Base()
    {
      return Game._iBase;
    }

    public static string GetLocation()
    {
      try
      {
        return Path.GetDirectoryName(System.Diagnostics.Process.GetProcessById((int) Game.Process.ProcessId).MainModule.FileName);
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool LoadResolver()
    {
        try
        {
            Reflex.Memory.Resolving.Resolver resolver = new Reflex.Memory.Resolving.Resolver(Game.Process, Assembly.GetExecutingAssembly(), "AionInterface.AionInterface.xml", "0D65CB5B");
            if (resolver.Valid)
            {
                Game.Resolver = resolver;
                return true;
            }
        }
        catch (Exception ex)
        {
        }
        return false;
    }


    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool LoadVersion()
    {
      try
      {
        FileStream fileStream = File.Open(Path.GetDirectoryName(System.Diagnostics.Process.GetProcessById((int) Game.Process.ProcessId).MainModule.FileName) + Path.DirectorySeparatorChar.ToString() + "Game.dll", FileMode.Open, FileAccess.Read);
        byte[] hash = new CRC32().ComputeHash((Stream) fileStream);
        Reflex.Memory.Resolving.Resolver resolver = new Reflex.Memory.Resolving.Resolver(Game.Process, Assembly.GetExecutingAssembly(), "AionInterface.AionInterface.xml", BitConverter.ToString(hash).Replace("-", ""));
        fileStream.Close();
        if (resolver.Valid)
        {
          Game.Resolver = resolver;
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool Start(string zFileSkills)
    {
      Game.SkillList = new SkillList(zFileSkills);
      Game._hThread = new Thread(new ThreadStart(Game._Active));
      Game._hThread.Start();
      return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool Stop()
    {
      if (Game._hThread == null)
        return false;
      Game._hThread.Abort();
      Game._hThread = (Thread) null;
      Game._Close(true);
      return true;
    }

    public static uint Time()
    {
      return (uint) ((ulong) DateTime.Now.Ticks / 10000UL);
    }
  }
}
