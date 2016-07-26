// Decompiled with JetBrains decompiler
// Type: AionInterface.Global
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System.Windows.Forms;

namespace AionInterface
{
  public class Global
  {
    public static event AionEventHandler OnGlobalClose;

    public static event AionEventIncludeHandler OnGlobalInclude;

    public static event AionEventRegisterHandler OnGlobalRegister;

    public static event AionEventTravelHandler OnGlobalTravel;

    public static event AionEventUnregisterHandler OnGlobalUnregister;

    public static event AionEventStringHandler OnGlobalWrite;

    public static void Close()
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalClose == null)
        return;
      // ISSUE: reference to a compiler-generated field
      Global.OnGlobalClose();
    }

    public static object Include(string zFile)
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalInclude != null)
      {
        // ISSUE: reference to a compiler-generated field
        return Global.OnGlobalInclude(zFile);
      }
      return (object) null;
    }

    public static bool Register(AionEventKeyHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalRegister != null)
      {
        // ISSUE: reference to a compiler-generated field
        return Global.OnGlobalRegister(hFunction, hKey, hModifier);
      }
      return false;
    }

    public static bool Travel(string zFile)
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalTravel != null)
      {
        // ISSUE: reference to a compiler-generated field
        return Global.OnGlobalTravel(zFile);
      }
      return true;
    }

    public static bool Unregister(Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalUnregister != null)
      {
        // ISSUE: reference to a compiler-generated field
        return Global.OnGlobalUnregister(hKey, hModifier);
      }
      return false;
    }

    public static void Write(string zMessage)
    {
      // ISSUE: reference to a compiler-generated field
      if (Global.OnGlobalWrite == null)
        return;
      // ISSUE: reference to a compiler-generated field
      Global.OnGlobalWrite(zMessage);
    }
  }
}
