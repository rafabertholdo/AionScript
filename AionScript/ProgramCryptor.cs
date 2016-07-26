// Decompiled with JetBrains decompiler
// Type: AionScript.ProgramCryptor
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

namespace AionScript
{
  public class ProgramCryptor
  {
    public static void Close(string zName)
    {
      Program.Manager.ScriptClose(zName);
    }

    public static object Include(string zFile)
    {
      return Program.Manager.ScriptInclude(zFile);
    }

    public static bool Register(string zFunction, string zKey, string zModifier = null)
    {
      return Scripting.Register(zFunction, zKey, zModifier);
    }

    public static bool Travel(string zFile)
    {
      return Scripting.Travel(zFile);
    }

    public static bool Unregister(string zKey, string zModifier = null)
    {
      return Scripting.Unregister(zKey, zModifier);
    }

    public static void Write(string zMessage)
    {
      Program.Manager.ScriptWrite(zMessage);
    }
  }
}
