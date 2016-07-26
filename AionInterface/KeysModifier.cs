// Decompiled with JetBrains decompiler
// Type: AionInterface.KeysModifier
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;

namespace AionInterface
{
  [Flags]
  public enum KeysModifier : uint
  {
    Alt = 1,
    Control = 2,
    None = 0,
    Shift = 4,
    Super = 8,
  }
}
