// Decompiled with JetBrains decompiler
// Type: AionInterface.Travel
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;

namespace AionInterface
{
  public class Travel
  {
    protected bool _bAction;
    protected bool _bFlying;
    protected bool _bRest;
    protected Vector3D _hVector;
    protected string _zName;
    protected string _zParam;
    protected string _zType;

    public Travel(string zName, Vector3D hVector, bool bFlying, string zType, string zParam)
    {
      this._zName = zName;
      this._hVector = hVector;
      this._bFlying = bFlying;
      this._zType = zType;
      this._bAction = zType != null && zType.Equals("Action", StringComparison.InvariantCultureIgnoreCase);
      this._bRest = zType != null && zType.Equals("Rest", StringComparison.InvariantCultureIgnoreCase);
      this._zParam = zParam;
    }

    public string GetName()
    {
      return this._zName;
    }

    public string GetParam()
    {
      return this._zParam;
    }

    public Vector3D GetPosition()
    {
      return (Vector3D) this._hVector.Clone();
    }

    public string GetType()
    {
      return this._zType;
    }

    public bool IsAction()
    {
      return this._bAction;
    }

    public bool IsFlying()
    {
      return this._bFlying;
    }

    public bool IsMove()
    {
      if (!this._bAction)
        return !this._bRest;
      return false;
    }

    public bool IsRest()
    {
      return this._bRest;
    }
  }
}
