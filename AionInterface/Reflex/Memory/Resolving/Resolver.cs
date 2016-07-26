// Decompiled with JetBrains decompiler
// Type: Reflex.Memory.Resolving.Resolver
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Reflex.Memory.Resolving
{
  public class Resolver : ResolverElement
  {
    protected Assembly _hAssembly;
    protected ProcessCommunication _hProcess;

    public bool Valid
    {
      get
      {
        return this._hProcess != null;
      }
    }

    public Resolver(ProcessCommunication hProcess, Assembly hAssembly, string zFile, string zCRC32)
      : base((XElement) null)
    {
      XElement xelement1 = (XElement) null;
      Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
      using (Stream manifestResourceStream = hAssembly.GetManifestResourceStream(zFile))
      {
        if (manifestResourceStream == null)
          return;
        xelement1 = XElement.Load(manifestResourceStream);
      }
      foreach (XElement xelement2 in xelement1.Elements())
      {
        XAttribute xattribute = xelement2.Attribute((XName) "Hash");
        if (xattribute != null && zCRC32 == xattribute.Value)
        {
          using (Stream manifestResourceStream = hAssembly.GetManifestResourceStream(xelement2.Value))
          {
            if (manifestResourceStream == null)
              break;
            this._hElement = XElement.Load(manifestResourceStream);
            this._hAssembly = hAssembly;
            this._hProcess = hProcess;
            break;
          }
        }
      }
    }
  }
}
