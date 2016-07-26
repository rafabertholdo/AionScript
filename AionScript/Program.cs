// Decompiled with JetBrains decompiler
// Type: AionScript.Program
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AionScript
{
  public class Program : ApplicationContext
  {
    private static bool _bClose = false;
    public static string CurrentDirectory = (string) null;
    public static string CurrentName = "AionScript";
    public static IDialog Dialog = (IDialog) null;
    public static IManager Manager = (IManager) null;
    public static IOverlay Overlay = (IOverlay) null;
    public static Setting Setting = (Setting) null;

    public Program(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler) ((sender, resolveEventArgs_0) =>
      {
        string str = string.Format("{0}{1}.dll", (object) Path.GetTempPath(), (object) resolveEventArgs_0.Name.Substring(0, resolveEventArgs_0.Name.IndexOf(",")));
        if (File.Exists(str))
          return Assembly.LoadFrom(str);
        return (Assembly) null;
      });
      Program.Dialog = new IDialog();
      Program.Manager = new IManager();
      Program.Overlay = new IOverlay();
      Program.Setting = new Setting(Program.CurrentDirectory + "Setting/Setting.xml");
      Program.Manager.SetFolderExtension(Program.CurrentDirectory + "Extension");
      Program.Manager.SetFolderNode(Program.CurrentDirectory + "Node");
      Program.Manager.SetFolderScripting(Program.CurrentDirectory + "Scripting");
      Game.OnClose += new AionEventHandler(Event.Close);
      Game.OnEntities += new AionEventHandler(Event.Entities);
      Global.OnGlobalClose += new AionEventHandler(Program.Manager.ScriptExit);
      Global.OnGlobalInclude += new AionEventIncludeHandler(Program.Manager.ScriptInclude);
      Global.OnGlobalRegister += new AionEventRegisterHandler(Scripting.RegisterCompiled);
      Global.OnGlobalUnregister += new AionEventUnregisterHandler(Scripting.UnregisterCompiled);
      Global.OnGlobalTravel += new AionEventTravelHandler(Program.Manager.SetTravel);
      Global.OnGlobalWrite += new AionEventStringHandler(Program.Manager.ScriptWrite);
      Game.OnInitialize += new AionEventHandler(Event.Initialize);
      Game.OnFrame += new AionEventHandler(Event.Frame);
      Game.OnOpen += new AionEventOpenHandler(Event.Open);
      Game.OnPlayer += new AionEventHandler(Event.Player);
      Program.Manager.SetOffset(Program.CurrentDirectory + "Setting\\Skill.xml");    
    }

    public static void Close()
    {
      if (Program._bClose)
        return;
      Program._bClose = true;
      Game.Stop();
      Program.Setting.Save();
      Program.Manager.Close();
      Program.Overlay.Close();
      Environment.Exit(0);
    }

    public static void Exception(Exception hException, bool bLuaException = false)
    {
      if (bLuaException)
      {
        Program.Manager.ScriptExit();
        Program.Manager.ScriptWrite(hException.Message);
      }
      else
      {
        int num = (int) MessageBox.Show(hException.Message + "\n" + hException.StackTrace);
        Program.Close();
      }
    }

    [STAThread]
    public static void Main(string[] args)
    {
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.UnhandledExceptions);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((ApplicationContext) new Program(args));
    }

    public static void UnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
    {
      int num = (int) new IException(e.ExceptionObject.ToString()).ShowDialog();
    }

    public delegate void DelegateArrayList(ArrayList hList);

    public delegate void DelegateBoolean(bool bBoolean);

    public delegate void DelegateException(Exception ex);

    public delegate bool DelegateSimpleBoolean();

    public delegate void DelegateString(string zString);
  }
}
