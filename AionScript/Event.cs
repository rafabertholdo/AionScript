// Decompiled with JetBrains decompiler
// Type: AionScript.Event
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using System;
using System.Collections;
using System.Windows.Forms;

namespace AionScript
{
  public class Event
  {
    public static IProcess _hProcess;

    public static void Close()
    {
      if (Program.Manager.InvokeRequired)
      {
        Program.Manager.Invoke((Delegate) new Action(Event.Close));
      }
      else
      {
        Program.Dialog.Clear();
        Program.Overlay.Hide();
      }
    }

    public static void Entities()
    {
      if (Program.Manager.InvokeRequired)
      {
        Program.Manager.Invoke((Delegate) new Action(Event.Entities));
      }
      else
      {
        if (!Program.Dialog.Visible)
          return;
        Program.Dialog.Update();
      }
    }

    public static void Frame()
    {
      if (Program.Manager.InvokeRequired)
        Program.Manager.Invoke((Delegate) new Action(Event.Frame));
      else
        Scripting.Event(eScripting.OnFrame);
    }

    public static void Initialize()
    {
      if (Program.Manager.InvokeRequired)
      {
        Program.Manager.Invoke((Delegate) new Action(Event.Initialize));
      }
      else
      {
        Game.PlayerInput.RegisterThread();
        Program.Overlay.Show();
        Scripting.Update();
      }
    }

    public static void Open(ArrayList hResultList)
    {
      if (Event._hProcess != null)
        return;
      if (Program.Manager.InvokeRequired)
      {
        object[] objArray = new object[1]
        {
          (object) hResultList
        };
        Program.Manager.Invoke((Delegate) new Program.DelegateArrayList(Event.Open), objArray);
      }
      else
      {
        if (hResultList.Count <= 0)
          return;
        Event._hProcess = new IProcess();
        foreach (ArrayList arrayList in hResultList)
          Event._hProcess.Add((uint) arrayList[0], (uint) arrayList[1], (string) arrayList[2], (byte) arrayList[3]);
        int num = (int) Event._hProcess.ShowDialog((IWin32Window) Program.Manager);
        Event._hProcess = (IProcess) null;
      }
    }

    public static void Player()
    {
      if (Program.Manager.InvokeRequired)
        Program.Manager.Invoke((Delegate) new Action(Event.Player));
      else
        Scripting.Event(eScripting.OnRun);
    }
  }
}
