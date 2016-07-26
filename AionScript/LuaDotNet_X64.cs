// Decompiled with JetBrains decompiler
// Type: AionScript.LuaDotNet_X64
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using LuaInterface_X64;
using System.Dynamic;

namespace AionScript
{
  public class LuaDotNet_X64 : DynamicObject
  {
    public Lua LuaEngine { get; protected set; }

    public LuaDotNet_X64(Lua hLua)
    {
      this.LuaEngine = hLua;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = (object) this.LuaEngine.GetString(binder.Name);
      return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
      LuaFunction function = this.LuaEngine.GetFunction(binder.Name);
      if (function == null)
      {
        result = (object) null;
        return false;
      }
      result = (object) function.Call(args);
      return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      this.LuaEngine.DoString(binder.Name + "=" + value.ToString());
      return true;
    }
  }
}
