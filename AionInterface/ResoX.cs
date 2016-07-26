// Decompiled with JetBrains decompiler
// Type: ResoX
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class ResoX
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (ResoX.resourceMan == null)
        ResoX.resourceMan = new ResourceManager("ResoX", typeof (ResoX).Assembly);
      return ResoX.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get
    {
      return ResoX.resourceCulture;
    }
    set
    {
      ResoX.resourceCulture = value;
    }
  }

  internal static UnmanagedMemoryStream Activated
  {
    get
    {
      return ResoX.ResourceManager.GetStream("Activated", ResoX.resourceCulture);
    }
  }

  internal static UnmanagedMemoryStream Deactivated
  {
    get
    {
      return ResoX.ResourceManager.GetStream("Deactivated", ResoX.resourceCulture);
    }
  }

  internal static UnmanagedMemoryStream Impossible
  {
    get
    {
      return ResoX.ResourceManager.GetStream("Impossible", ResoX.resourceCulture);
    }
  }

  internal ResoX()
  {
  }
}
