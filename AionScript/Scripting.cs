// Decompiled with JetBrains decompiler
// Type: AionScript.Scripting
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AionScript
{
  public class Scripting
  {
    private static bool _bLoaded = false;
    private static CompileResult _hCompiled = (CompileResult) null;
    private static Dictionary<string, ArrayList> _hHotKey = new Dictionary<string, ArrayList>();
    private static Dictionary<AionEventKeyHandler, ArrayList> _hHotKeyCompiled = new Dictionary<AionEventKeyHandler, ArrayList>();
    private static LuaDotNet_X64 _hLua_X64 = (LuaDotNet_X64) null;
    private static LuaDotNet_X86 _hLua_X86 = (LuaDotNet_X86) null;

    private static LuaDotNet_X64 _Create_X64(string zFile)
    {
      LuaInterface_X64.Lua hLua = new LuaInterface_X64.Lua();
      ProgramCryptor programCryptor = new ProgramCryptor();
      Scripting.Update(hLua);
      hLua.RegisterFunction("Close", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Close"));
      hLua.RegisterFunction("Include", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Include"));
      hLua.RegisterFunction("Time", (object) new Game(), (MethodBase) new Game().GetType().GetMethod("Time"));
      hLua.RegisterFunction("Travel", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Travel"));
      hLua.RegisterFunction("Register", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Register"));
      hLua.RegisterFunction("Unregister", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Unregister"));
      hLua.RegisterFunction("Write", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Write"));
      if (File.Exists(zFile))
        hLua.DoString(File.ReadAllText(zFile), Path.GetFileName(zFile));
      return new LuaDotNet_X64(hLua);
    }

    private static LuaDotNet_X86 _Create_X86(string zFile)
    {
      LuaInterface_X86.Lua hLua = new LuaInterface_X86.Lua();
      ProgramCryptor programCryptor = new ProgramCryptor();
      Scripting.Update(hLua);
      hLua.RegisterFunction("Close", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Close"));
      hLua.RegisterFunction("Include", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Include"));
      hLua.RegisterFunction("Time", (object) new Game(), (MethodBase) new Game().GetType().GetMethod("Time"));
      hLua.RegisterFunction("Travel", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Travel"));
      hLua.RegisterFunction("Register", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Register"));
      hLua.RegisterFunction("Unregister", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Unregister"));
      hLua.RegisterFunction("Write", (object) programCryptor, (MethodBase) programCryptor.GetType().GetMethod("Write"));
      if (File.Exists(zFile))
        hLua.DoString(File.ReadAllText(zFile), Path.GetFileName(zFile));
      return new LuaDotNet_X86(hLua);
    }

    private static void _OnHotKey(Keys hKey, KeysModifier hModifier)
    {
      if (Scripting._hLua_X64 == null && Scripting._hLua_X86 == null)
        return;
      foreach (KeyValuePair<string, ArrayList> keyValuePair in Scripting._hHotKey)
      {
        try
        {
          if ((Keys) keyValuePair.Value[0] == hKey)
          {
            if ((KeysModifier) keyValuePair.Value[1] == hModifier)
            {
              if (Scripting._hLua_X64 != null)
              {
                LuaInterface_X64.LuaFunction function = Scripting._hLua_X64.LuaEngine.GetFunction(keyValuePair.Key);
                if (function == null)
                  break;
                function.Call();
                break;
              }
              LuaInterface_X86.LuaFunction function1 = Scripting._hLua_X86.LuaEngine.GetFunction(keyValuePair.Key);
              if (function1 == null)
                break;
              function1.Call();
              break;
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static string _Read(string zFile)
    {
      try
      {
        StreamReader streamReader = File.OpenText(zFile);
        string str = streamReader.ReadToEnd();
        streamReader.Close();
        return str;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public static bool Close()
    {
      if (Scripting._hLua_X64 == null && Scripting._hLua_X86 == null && Scripting._hCompiled == null)
        return false;
      foreach (KeyValuePair<string, ArrayList> keyValuePair in Scripting._hHotKey)
        Game.PlayerInput.Unregister((Keys) keyValuePair.Value[0], (KeysModifier) keyValuePair.Value[1]);
      foreach (KeyValuePair<AionEventKeyHandler, ArrayList> keyValuePair in Scripting._hHotKeyCompiled)
        Game.PlayerInput.Unregister((Keys) keyValuePair.Value[0], (KeysModifier) keyValuePair.Value[1]);
      Scripting._hHotKey.Clear();
      Scripting._hHotKeyCompiled.Clear();
      Scripting.Event(eScripting.OnClose);
      Scripting._hLua_X86 = (LuaDotNet_X86) null;
      Scripting._hLua_X64 = (LuaDotNet_X64) null;
      Scripting._hCompiled = (CompileResult) null;
      return true;
    }

    public static bool Event(eScripting eScripting)
    {
      if (Scripting._hLua_X64 != null || Scripting._hLua_X86 != null || Scripting._hCompiled != null)
      {
        if (!Scripting._bLoaded && (eScripting == eScripting.OnRun || eScripting == eScripting.OnFrame))
        {
          Scripting._bLoaded = true;
          Scripting.Event(eScripting.OnLoad);
        }
        try
        {
          if (Scripting._hCompiled != null)
          {
            switch (eScripting)
            {
              case eScripting.OnClose:
                Scripting._hCompiled.hInterface.OnClose();
                break;
              case eScripting.OnLoad:
                Scripting._hCompiled.hInterface.OnLoad();
                break;
              case eScripting.OnRun:
                Scripting._hCompiled.hInterface.OnRun();
                break;
            }
          }
          else if (Scripting._hLua_X64 != null)
          {
            LuaInterface_X64.LuaFunction function = Scripting._hLua_X64.LuaEngine.GetFunction(eScripting.ToString());
            if (function == null)
              return false;
            function.Call();
          }
          else
          {
            LuaInterface_X86.LuaFunction function = Scripting._hLua_X86.LuaEngine.GetFunction(eScripting.ToString());
            if (function == null)
              return false;
            function.Call();
          }
          return true;
        }
        catch (Exception ex)
        {
          int num = 1;
          Program.Exception(ex, num != 0);
          Scripting.Close();
        }
      }
      return false;
    }

    public static bool Execute(string zMessage)
    {
      if (Scripting._hLua_X64 == null && Scripting._hLua_X86 == null)
        return false;
      try
      {
        if (Scripting._hLua_X64 != null)
          Scripting._hLua_X64.LuaEngine.DoString(zMessage);
        else if (Scripting._hLua_X86 != null)
          Scripting._hLua_X86.LuaEngine.DoString(zMessage);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static object Include(string zFile)
    {
      if (!zFile.EndsWith(".lua") && !zFile.EndsWith(".cs") && !zFile.EndsWith(".vb"))
      {
        if (Scripting._hLua_X64 == null && Scripting._hLua_X86 == null)
          return (object) null;
        zFile += ".lua";
      }
      if (!File.Exists(zFile))
        return (object) null;
      return Scripting.IncludeString(zFile, Scripting._Read(zFile));
    }

    public static object IncludeString(string zFile, string zString)
    {
      if (Scripting._hLua_X64 == null && Scripting._hLua_X86 == null && Scripting._hCompiled == null)
        return (object) null;
      try
      {
        if (zFile != null && !zFile.EndsWith(".lua"))
        {
          if (Scripting._hCompiled.hTypes != null)
          {
            foreach (Type type in Scripting._hCompiled.hTypes)
            {
              if (type.IsClass && Path.GetFileNameWithoutExtension(zString).Equals(type.Name))
                return Activator.CreateInstance(type);
            }
          }
          return (object) null;
        }
        if (Scripting._hLua_X64 != null)
        {
          if (Scripting._hCompiled != null)
            return (object) Scripting._Create_X64(zFile);
          Scripting._hLua_X64.LuaEngine.DoString("AionScriptObject = {};\r\n" + Regex.Replace(zString, "function (.*?)\\(", "function AionScriptObject:$1("), Path.GetFileName(zFile));
          return Scripting._hLua_X64.LuaEngine["AionScriptObject"];
        }
        if (Scripting._hCompiled != null)
          return (object) Scripting._Create_X86(zFile);
        Scripting._hLua_X86.LuaEngine.DoString("AionScriptObject = {};\r\n" + Regex.Replace(zString, "function (.*?)\\(", "function AionScriptObject:$1("), Path.GetFileName(zFile));
        return Scripting._hLua_X86.LuaEngine["AionScriptObject"];
      }
      catch (Exception ex)
      {
        int num = 1;
        Program.Exception(ex, num != 0);
        Scripting.Close();
        return (object) null;
      }
    }

    public static bool Load(string zFile)
    {
      try
      {
        bool bIsVisualBasic = false;
        Scripting._bLoaded = false;
        if (Game.PlayerInput == null)
          throw new Exception("Please start the game before starting a script.");
        Program.Manager.SetTravel((string) null);
        string path = zFile + Path.DirectorySeparatorChar.ToString() + Path.GetFileNameWithoutExtension(zFile) + ".csproj";
        if (File.Exists(path))
        {
          Assembly assembly = ProjectCompile.CompileToAssembly(new Uri(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + path));
          // ISSUE: variable of the null type
          Assembly local = null;
          if (assembly == (Assembly) local)
            throw new Exception("Unable to compile project; does it work in Visual Studio?");
          foreach (Type type in assembly.GetTypes())
          {
            try
            {
              if (!type.IsAbstract)
              {
                if (type.IsClass)
                {
                  if (type.GetInterface(typeof (IAionInterface).Name) != (Type) null)
                  {
                    Scripting._hCompiled = new CompileResult()
                    {
                      hInterface = (IAionInterface) Activator.CreateInstance(type),
                      hTypes = (Type[]) null
                    };
                    return true;
                  }
                }
              }
            }
            catch (Exception ex)
            {
              throw new Exception("Unable to instantiate project entry point!");
            }
          }
          throw new Exception("Unable to find project entry point!");
        }
        if (!zFile.EndsWith(".cs") && !(bIsVisualBasic = zFile.EndsWith(".vb")))
        {
          if (zFile.EndsWith(".lua"))
          {
            if (UIntPtr.Size == 4)
            {
              Scripting._hLua_X86 = Scripting._Create_X86(zFile);
              if (Scripting._hLua_X86 == null)
                return false;
            }
            else
            {
              Scripting._hLua_X64 = Scripting._Create_X64(zFile);
              if (Scripting._hLua_X64 == null)
                return false;
            }
            if (File.Exists("AionScript.lua"))
            {
              if (Scripting._hLua_X64 != null)
                Scripting._hLua_X64.LuaEngine.DoFile("AionScript.lua");
              else if (Scripting._hLua_X86 != null)
                Scripting._hLua_X86.LuaEngine.DoFile("AionScript.lua");
            }
            foreach (string index in new List<string>()
            {
              "AsCircularMagic",
              "AsTest"
            })
            {
              string zString = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AionScript.Resources.Library." + index + ".lua")).ReadToEnd();
              if (Scripting._hLua_X64 != null)
                Scripting._hLua_X64.LuaEngine[index] = Scripting.IncludeString((string) null, zString);
              else
                Scripting._hLua_X86.LuaEngine[index] = Scripting.IncludeString((string) null, zString);
            }
            return true;
          }
        }
        else
        {
          Scripting._hCompiled = Compiler.Compile(zFile, bIsVisualBasic);
          return Scripting._hCompiled != null;
        }
      }
      catch (Exception ex)
      {
        int num = 1;
        Program.Exception(ex, num != 0);
        Scripting.Close();
      }
      return false;
    }

    public static bool Register(string zFunction, string zKey, string zModifier = null)
    {
      try
      {
        if (Scripting._hLua_X86 == null && Scripting._hLua_X64 == null)
          return false;
        int result = 0;
        Keys hKey = (Keys) Enum.Parse(typeof (Keys), zKey);
        KeysModifier hModifier = zModifier == null ? KeysModifier.None : (int.TryParse(zModifier, out result) ? (KeysModifier) result : (KeysModifier) Enum.Parse(typeof (KeysModifier), zModifier));
        if (Scripting._hHotKey.ContainsKey(zFunction) || !Game.PlayerInput.Register(new AionEventKeyHandler(Scripting._OnHotKey), hKey, hModifier))
          return false;
        ArrayList arrayList = new ArrayList()
        {
          (object) hKey,
          (object) hModifier
        };
        Scripting._hHotKey[zFunction] = arrayList;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool RegisterCompiled(AionEventKeyHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      try
      {
        if (Scripting._hCompiled == null || Scripting._hHotKeyCompiled.ContainsKey(hFunction) || !Game.PlayerInput.Register(hFunction, hKey, hModifier))
          return false;
        ArrayList arrayList = new ArrayList()
        {
          (object) hKey,
          (object) hModifier
        };
        Scripting._hHotKeyCompiled[hFunction] = arrayList;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool Travel(string zFile)
    {
      if (Program.Manager.SetTravel(zFile))
      {
        if (Scripting._hLua_X64 != null)
          Scripting._hLua_X64.LuaEngine["TravelList"] = (object) Game.TravelList;
        else if (Scripting._hLua_X86 != null)
          Scripting._hLua_X86.LuaEngine["TravelList"] = (object) Game.TravelList;
        return true;
      }
      if (Scripting._hLua_X64 != null)
        Scripting._hLua_X64.LuaEngine["TravelList"] = (object) null;
      else if (Scripting._hLua_X86 != null)
        Scripting._hLua_X86.LuaEngine["TravelList"] = (object) null;
      return false;
    }

    public static bool Unregister(string zKey, string zModifier = null)
    {
      try
      {
        if (Scripting._hLua_X86 == null && Scripting._hLua_X64 == null)
          return false;
        int result = 0;
        Keys hKey = (Keys) Enum.Parse(typeof (Keys), zKey);
        KeysModifier hModifier = zModifier == null ? KeysModifier.None : (int.TryParse(zModifier, out result) ? (KeysModifier) result : (KeysModifier) Enum.Parse(typeof (KeysModifier), zModifier));
        foreach (KeyValuePair<string, ArrayList> keyValuePair in Scripting._hHotKey)
        {
          if ((Keys) keyValuePair.Value[0] == hKey && (KeysModifier) keyValuePair.Value[1] == hModifier)
          {
            Game.PlayerInput.Unregister(hKey, hModifier);
            Scripting._hHotKey.Remove(keyValuePair.Key);
            return true;
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool UnregisterCompiled(Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      try
      {
        if (Scripting._hCompiled == null)
          return false;
        foreach (KeyValuePair<AionEventKeyHandler, ArrayList> keyValuePair in Scripting._hHotKeyCompiled)
        {
          if ((Keys) keyValuePair.Value[0] == hKey && (KeysModifier) keyValuePair.Value[1] == hModifier)
          {
            Game.PlayerInput.Unregister(hKey, hModifier);
            Scripting._hHotKeyCompiled.Remove(keyValuePair.Key);
            return true;
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool Update()
    {
      if (Scripting._hLua_X64 != null)
        return Scripting.Update(Scripting._hLua_X64.LuaEngine);
      if (Scripting._hLua_X86 != null)
        return Scripting.Update(Scripting._hLua_X86.LuaEngine);
      return false;
    }

    public static bool Update(LuaInterface_X64.Lua hLua)
    {
      if (hLua == null)
        return false;
      hLua["AbilityList"] = (object) Game.AbilityList;
      hLua["DialogList"] = (object) Game.DialogList;
      hLua["EntityList"] = (object) Game.EntityList;
      hLua["InventoryList"] = (object) Game.InventoryList;
      hLua["Memory"] = (object) Game.Process;
      hLua["ForceList"] = (object) Game.ForceList;
      hLua["Player"] = (object) Game.Player;
      hLua["PlayerInput"] = (object) Game.PlayerInput;
      hLua["SkillList"] = (object) Game.SkillList;
      hLua["TravelList"] = (object) Game.TravelList;
      return true;
    }

    public static bool Update(LuaInterface_X86.Lua hLua)
    {
      if (hLua == null)
        hLua = Scripting._hLua_X86 == null ? (LuaInterface_X86.Lua) null : Scripting._hLua_X86.LuaEngine;
      if (hLua == null)
        return false;
      hLua["AbilityList"] = (object) Game.AbilityList;
      hLua["DialogList"] = (object) Game.DialogList;
      hLua["EntityList"] = (object) Game.EntityList;
      hLua["InventoryList"] = (object) Game.InventoryList;
      hLua["Memory"] = (object) Game.Process;
      hLua["ForceList"] = (object) Game.ForceList;
      hLua["Player"] = (object) Game.Player;
      hLua["PlayerInput"] = (object) Game.PlayerInput;
      hLua["SkillList"] = (object) Game.SkillList;
      hLua["TravelList"] = (object) Game.TravelList;
      return true;
    }
  }
}
