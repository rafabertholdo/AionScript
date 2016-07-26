// Decompiled with JetBrains decompiler
// Type: Reflex.Memory.Resolving.ResolverElement
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Reflex.Memory.Resolving
{
  public class ResolverElement
  {
    protected XElement _hElement;
    protected Dictionary<XElement, ResolverElement> _hResolverList;
    protected uint _iValue;

    public ResolverElement this[string zName]
    {
      get
      {
        if (this._hElement == null)
          return (ResolverElement) null;
        XElement index = this._hElement.Element((XName) zName);
        if (index == null)
          return (ResolverElement) null;
        if (this._hResolverList == null || !this._hResolverList.ContainsKey(index))
        {
          if (this._hResolverList == null)
            this._hResolverList = new Dictionary<XElement, ResolverElement>();
          this._hResolverList[index] = new ResolverElement(index);
        }
        return this._hResolverList[index];
      }
    }

    public uint Value
    {
      get
      {
        if (this._hElement == null || this._hElement.HasElements)
          return 0;
        if ((int) this._iValue == 0)
          this._iValue = uint.Parse(this._hElement.Value, NumberStyles.HexNumber);
        return this._iValue;
      }
    }

    public ResolverElement(XElement hElement)
    {
      this._hElement = hElement;
    }
  }
}
