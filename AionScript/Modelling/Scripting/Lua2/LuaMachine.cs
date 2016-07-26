// Decompiled with JetBrains decompiler
// Type: AionScript.Modelling.Scripting.Lua.LuaMachine
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionScript.Modelling.Scripting;
using System.IO;
using System.Text.RegularExpressions;

namespace AionScript.Modelling.Scripting.Lua2
{
  public abstract class LuaMachine : VirtualMachine
  {
    public LuaMachine(Stream hStream, object[] hObjectList = null)
      : base(hStream, hObjectList)
    {
    }

    public override object Include(Stream hStream)
    {
      using (StreamReader streamReader = new StreamReader(hStream))
      {
        string input = streamReader.ReadToEnd();
        return this.InitializeInclude(input.Substring(input.IndexOf("\\") + 1), "AionScriptObject", "AionScriptObject = {};\r\n" + Regex.Replace(input, "function (.*?)\\(", "function AionScriptObject:$1("));
      }
    }

    public abstract object InitializeInclude(string zName, string zObject, string zContent);
  }
}
