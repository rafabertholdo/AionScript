// Decompiled with JetBrains decompiler
// Type: AionScript.Extension
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace AionScript
{
  public class Extension
  {
    private static ArrayList _hExtensionList = new ArrayList();
    private static Dictionary<string, Dictionary<string, string>> _hSettingList = new Dictionary<string, Dictionary<string, string>>();

    public static bool Close(Setting hSetting)
    {
      if (Extension._hExtensionList.Count <= 0)
        return false;
      foreach (ArrayList arrayList in Extension._hExtensionList)
      {
        string str = (string) arrayList[0];
        object[] args = new object[1]
        {
          (object) Extension._hSettingList[(string) arrayList[0]]
        };
        ((Type) arrayList[1]).InvokeMember("OnClose", BindingFlags.InvokeMethod, (Binder) null, arrayList[2], args);
        hSetting.SetDictionary(str.Substring(str.LastIndexOf("\\") + 1), Extension._hSettingList[(string) arrayList[0]]);
      }
      foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in Extension._hSettingList)
      {
        string key = keyValuePair.Key;
        hSetting.SetDictionary(key.Substring(key.LastIndexOf("\\") + 1), keyValuePair.Value);
      }
      Extension._hExtensionList.Clear();
      Extension._hSettingList.Clear();
      return true;
    }

    public static bool Event(string zName)
    {
      if (Extension._hExtensionList.Count <= 0)
        return false;
      foreach (ArrayList arrayList in Extension._hExtensionList)
      {
        try
        {
          ((Type) arrayList[1]).InvokeMember(zName, BindingFlags.InvokeMethod, (Binder) null, arrayList[2], (object[]) null);
        }
        catch (Exception ex)
        {
        }
      }
      return true;
    }

    public static object Invoke(Type hType, object hInstance, string zName, object[] mParameter = null)
    {
      try
      {
        return hType.InvokeMember(zName, BindingFlags.InvokeMethod, (Binder) null, hInstance, mParameter);
      }
      catch (Exception ex)
      {
        return (object) null;
      }
    }

    public static ArrayList List()
    {
      return Extension._hExtensionList;
    }

    public static bool Load(string zFile, Setting hSetting)
    {
      Assembly assembly;
      try
      {
        assembly = Assembly.LoadFile(zFile);
      }
      catch (Exception ex1)
      {
        try
        {
          string location = Assembly.GetExecutingAssembly().Location;
          assembly = Assembly.LoadFile(location.Substring(0, location.LastIndexOf("\\") + 1) + zFile);
        }
        catch (Exception ex2)
        {
          int num = (int) MessageBox.Show(ex2.Message + "\n\n" + ex2.StackTrace);
          return false;
        }
      }
      foreach (Type type in assembly.GetTypes())
      {
        if (type.Name == "AionExtension")
        {
          object instance = Activator.CreateInstance(type);
          zFile = zFile.Contains("\\") ? zFile.Substring(zFile.LastIndexOf("\\") + 1) : zFile;
          Extension._hSettingList[zFile] = hSetting.GetDictionary(zFile);
          Extension._hSettingList[zFile] = Extension._hSettingList[zFile] == null ? new Dictionary<string, string>() : Extension._hSettingList[zFile];
          ArrayList arrayList = new ArrayList()
          {
            (object) zFile,
            (object) type,
            instance
          };
          object[] mParameter = new object[2]
          {
            (object) Program.CurrentDirectory,
            (object) Extension._hSettingList[zFile]
          };
          arrayList.Add(Extension.Invoke(type, instance, "OnLoad", mParameter));
          Extension._hExtensionList.Add((object) arrayList);
          return true;
        }
      }
      return false;
    }
  }
}
