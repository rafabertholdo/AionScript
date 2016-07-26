// Decompiled with JetBrains decompiler
// Type: AionInterface.Vector2D
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;

namespace AionInterface
{
  public class Vector2D : ICloneable
  {
    public uint X;
    public uint Y;

    public uint Height
    {
      get
      {
        return this.Y;
      }
      set
      {
        this.Y = value;
      }
    }

    public uint Width
    {
      get
      {
        return this.X;
      }
      set
      {
        this.X = value;
      }
    }

    public Vector2D(uint X = 0, uint Y = 0)
    {
      this.X = X;
      this.Y = Y;
    }

    public object Clone()
    {
      return (object) new Vector2D(this.X, this.Y);
    }
  }
}
