// Decompiled with JetBrains decompiler
// Type: AionInterface.AbilityLazy
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;

namespace AionInterface
{
  public class AbilityLazy : LazySynch
  {
    protected string Test
    {
      get
      {
        return this._GetValue<string>((Action<string>) (x => this.Test = (string) null), 0UL);
      }
      set
      {
      }
    }

    public AbilityLazy(ulong iAddress)
    {
    }
  }
}
