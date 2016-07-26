// Decompiled with JetBrains decompiler
// Type: AionInterface.LazySynch
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections.Generic;

namespace AionInterface
{
  public class LazySynch
  {
    private Dictionary<string, ulong> _hTimeTable = new Dictionary<string, ulong>();

    public T _GetValue<T>(Action<string> x, ulong iOffset = 0)
    {
      return default (T);
    }
  }
}
