// Decompiled with JetBrains decompiler
// Type: AionScript.ProjectCompile
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AionScript
{
  public static class ProjectCompile
  {
    private static CompilerResults _Compile(Uri hFilePath, bool bGenerateInMemory, string zGlobalUniqueIdenitifer = null)
    {
      try
      {
        CodeDomProvider codeDomProvider = (CodeDomProvider) new CSharpCodeProvider();
        CompilerParameters options = new CompilerParameters();
        XDocument xdocument = XDocument.Load(hFilePath.LocalPath);
        List<string> hResultingFiles = new List<string>();
        options.GenerateExecutable = false;
        options.GenerateInMemory = bGenerateInMemory;
        if (!ProjectCompile._CompilePropertyGroup(hFilePath, xdocument.Root, zGlobalUniqueIdenitifer) || !ProjectCompile._CompileItemGroup(hFilePath, xdocument.Root, hResultingFiles, options.ReferencedAssemblies, options.EmbeddedResources))
          return (CompilerResults) null;
        options.ReferencedAssemblies.Add("System.dll");
        options.ReferencedAssemblies.Add("System.Core.dll");
        CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromFile(options, hResultingFiles.ToArray());
        if (compilerResults == null || compilerResults.Errors.HasErrors)
          return (CompilerResults) null;
        return compilerResults;
      }
      catch (Exception ex)
      {
        return (CompilerResults) null;
      }
    }

    private static bool _CompileItemGroup(Uri hFilePath, XElement hDocumentRoot, List<string> hResultingFiles, StringCollection hResultingReferences, StringCollection hResultingResources)
    {
      foreach (XElement xelement1 in Enumerable.Where<XElement>(hDocumentRoot.Elements(), (Func<XElement, bool>) (xelement_0 =>
      {
        if (xelement_0.HasElements)
          return xelement_0.Name.LocalName.Equals("ItemGroup");
        return false;
      })))
      {
        IEnumerable<XElement> enumerable1 = Enumerable.Where<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 =>
        {
          if (xelement_0.HasAttributes && xelement_0.Name.LocalName.Equals("Compile"))
            return xelement_0.Attribute((XName) "Include") != null;
          return false;
        }));
        IEnumerable<XElement> enumerable2 = Enumerable.Where<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 =>
        {
          if (xelement_0.HasAttributes && xelement_0.Name.LocalName.Equals("EmbeddedResource"))
            return xelement_0.Attribute((XName) "Include") != null;
          return false;
        }));
        IEnumerable<XElement> enumerable3 = Enumerable.Where<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 =>
        {
          if (xelement_0.HasAttributes && xelement_0.Name.LocalName.Equals("ProjectReference"))
            return xelement_0.Attribute((XName) "Include") != null;
          return false;
        }));
        IEnumerable<XElement> enumerable4 = Enumerable.Where<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 =>
        {
          if (xelement_0.HasAttributes && xelement_0.Name.LocalName.Equals("Reference"))
            return xelement_0.Attribute((XName) "Include") != null;
          return false;
        }));
        foreach (XElement xelement2 in enumerable1)
        {
          Uri uri = new Uri(hFilePath, xelement2.Attribute((XName) "Include").Value);
          if (!File.Exists(uri.LocalPath))
            return false;
          hResultingFiles.Add(uri.LocalPath);
        }
        foreach (XElement xelement2 in enumerable2)
        {
          Uri uri = new Uri(hFilePath, xelement2.Attribute((XName) "Include").Value);
          if (!File.Exists(uri.LocalPath))
            return false;
          hResultingResources.Add(uri.LocalPath);
        }
        foreach (XElement xelement2 in enumerable3)
        {
          XElement xelement3 = Enumerable.SingleOrDefault<XElement>(xelement2.Elements(), (Func<XElement, bool>) (xelement_0 => xelement_0.Name.LocalName.Equals("Project")));
          CompilerResults compilerResults = ProjectCompile._Compile(new Uri(hFilePath, xelement2.Attribute((XName) "Include").Value), false, xelement3 == null ? (string) null : xelement3.Value);
          if (compilerResults == null)
            return false;
          hResultingReferences.Add(compilerResults.PathToAssembly);
        }
        foreach (XElement xelement2 in enumerable4)
        {
          if (xelement2.HasElements)
          {
            bool flag = false;
            foreach (XElement xelement3 in xelement2.Elements())
            {
              if (xelement3.Name.LocalName.Equals("SpecificVersion"))
                flag = xelement3.Value.Equals("True", StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (XElement xelement3 in xelement2.Elements())
            {
              if (xelement3.Name.LocalName.Equals("HintPath"))
              {
                Uri uri = new Uri(hFilePath, xelement3.Value);
                if (!File.Exists(uri.LocalPath))
                  return false;
                if (flag)
                {
                  Assembly assembly = Assembly.ReflectionOnlyLoadFrom(uri.LocalPath);
                  if (!xelement2.Attribute((XName) "Include").Value.Equals(string.Format("{0}, processorArchitecture={1}", (object) Regex.Replace(assembly.FullName, "(.*), PublicKeyToken=(.*)", "$1"), (object) assembly.GetName().ProcessorArchitecture)))
                    return false;
                }
                hResultingReferences.Add(uri.LocalPath);
                break;
              }
            }
          }
          else
            hResultingReferences.Add(string.Format("{0}.dll", (object) xelement2.Attribute((XName) "Include").Value));
        }
      }
      return true;
    }

    private static bool _CompilePropertyGroup(Uri hFilePath, XElement hDocumentRoot, string zGlobalUniqueIdenitifer)
    {
      foreach (XElement xelement1 in Enumerable.Where<XElement>(hDocumentRoot.Elements(), (Func<XElement, bool>) (xelement_0 =>
      {
        if (xelement_0.HasElements)
          return xelement_0.Name.LocalName.Equals("PropertyGroup");
        return false;
      })))
      {
        XElement xelement2 = Enumerable.SingleOrDefault<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 => xelement_0.Name.LocalName.Equals("ProjectGuid")));
        XElement xelement3 = Enumerable.SingleOrDefault<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 => xelement_0.Name.LocalName.Equals("OutputType")));
        XElement xelement4 = Enumerable.SingleOrDefault<XElement>(xelement1.Elements(), (Func<XElement, bool>) (xelement_0 => xelement_0.Name.LocalName.Equals("TargetFrameworkVersion")));
        if (xelement2 != null && zGlobalUniqueIdenitifer != null && !xelement2.Value.Equals(zGlobalUniqueIdenitifer) || xelement3 != null && !xelement3.Value.Equals("Library") || xelement4 != null && !xelement4.Value.Equals("v4.0"))
          return false;
      }
      return true;
    }

    public static Assembly CompileToAssembly(Uri hFilePath)
    {
      CompilerResults compilerResults = ProjectCompile._Compile(hFilePath, true, (string) null);
      if (compilerResults == null)
        return (Assembly) null;
      return compilerResults.CompiledAssembly;
    }

    public static Uri CompileToFile(Uri hFilePath)
    {
      CompilerResults compilerResults = ProjectCompile._Compile(hFilePath, false, (string) null);
      if (compilerResults == null)
        return (Uri) null;
      return new Uri(compilerResults.PathToAssembly);
    }
  }
}
