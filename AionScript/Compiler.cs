// Decompiled with JetBrains decompiler
// Type: AionScript.Compiler
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AionScript
{
  public class Compiler
  {
    public static List<string> _OnResolve(string zFile, Uri hBase = null, List<string> hFiles = null)
    {
      if (hBase == (Uri) null)
      {
        string currentDirectory = Environment.CurrentDirectory;
        char ch = Path.DirectorySeparatorChar;
        string str1 = ch.ToString();
        hBase = new Uri(currentDirectory + str1);
        Uri baseUri = hBase;
        string directoryName = Path.GetDirectoryName(zFile);
        ch = Path.DirectorySeparatorChar;
        string str2 = ch.ToString();
        string relativeUri = directoryName + str2;
        hBase = new Uri(baseUri, relativeUri);
      }
      Uri uri = new Uri(hBase, Path.GetFileName(zFile));
      hFiles = hFiles == null ? new List<string>() : hFiles;
      if (hFiles.Contains(uri.LocalPath) || !File.Exists(uri.LocalPath))
        return (List<string>) null;
      hFiles.Add(uri.LocalPath);
      foreach (Match match in Regex.Matches(File.ReadAllText(uri.LocalPath), "Game\\.Include\\((\\s*)\"(.*)\"(\\s*)\\)"))
      {
        if (match.Groups[2].Value.EndsWith(Path.GetExtension(zFile)))
          Compiler._OnResolve(match.Groups[2].Value, hBase, hFiles);
      }
      return hFiles;
    }

    public static CompileResult Compile(string zTargetFile, bool bIsVisualBasic = false)
    {
      List<string> list = Compiler._OnResolve(zTargetFile, (Uri) null, (List<string>) null);
      CodeDomProvider codeDomProvider = bIsVisualBasic ? (CodeDomProvider) new VBCodeProvider() : (CodeDomProvider) new CSharpCodeProvider();
      CompilerParameters options = new CompilerParameters();
      if (list != null)
      {
        options.GenerateExecutable = false;
        options.GenerateInMemory = true;
        Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        AppDomain.CurrentDomain.GetAssemblies();
        foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        {
          string str = string.Format("{0}.dll", (object) assemblyName.Name);
          options.ReferencedAssemblies.Add(str);
        }
        options.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
        options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
        CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromFile(options, list.ToArray());
        if (!compilerResults.Errors.HasErrors && !compilerResults.Errors.HasWarnings)
        {
          Type[] types = compilerResults.CompiledAssembly.GetTypes();
          foreach (Type type in types)
          {
            try
            {
              if (type.IsClass)
                return new CompileResult()
                {
                  hInterface = (IAionInterface) Activator.CreateInstance(type),
                  hTypes = types
                };
            }
            catch (Exception ex)
            {
            }
          }
          return (CompileResult) null;
        }
        Program.Exception(new Exception((string) (object) compilerResults.Errors[0].Line + (object) ": " + compilerResults.Errors[0].ErrorText), true);
      }
      return (CompileResult) null;
    }
  }
}
