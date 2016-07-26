// Decompiled with JetBrains decompiler
// Type: AionScript.Modelling.Scripting.Lua.LuaMachine64
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionScript.Modelling.Scripting;
using LuaInterface_X64;
using System;
using System.IO;

namespace AionScript.Modelling.Scripting.Lua2
{
  public class LuaMachine64 : LuaMachine, VirtualMachineEngine
  {
    private Lua _hLua;

    public LuaMachine64(Stream hStream, object[] hObjectList)
      : base(hStream, hObjectList)
    {
      this.Engine = (VirtualMachineEngine) this;
    }

    public override void Dispose()
    {
      this._hLua.Dispose();
    }

    public object GetFunction(string zFunction)
    {
      return (object) this._hLua.GetFunction(zFunction);
    }

    public object GetVariable(string zVariable)
    {
      return this._hLua[zVariable];
    }

    public object InitializeInclude(string zContent)
    {
      throw new NotImplementedException();
    }

    public override object InitializeInclude(string zName, string zObject, string zContent)
    {
      this._hLua.DoString(zContent, zName);
      return this._hLua[zObject];
    }

    public void InitializeMachine(string zContent)
    {
      this._hLua = new Lua();
      this._hLua.DoString(zContent);
    }

    public object SetFunction(object hFunction, object[] hArgumentList = null)
    {
      return (object) ((LuaFunction) hFunction).Call(hArgumentList);
    }

    public void SetVariable(string zVariable, object hResult)
    {
      this._hLua[zVariable] = hResult;
    }
  }
}
