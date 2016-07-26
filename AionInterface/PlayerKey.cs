// Decompiled with JetBrains decompiler
// Type: AionInterface.PlayerKey
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AionInterface
{
  public class PlayerKey : NativeWindow
  {
    protected static Dictionary<int, ArrayList> _hHotKey = new Dictionary<int, ArrayList>();
    protected static Thread _hThread = (Thread) null;
    protected static int _iHotKey = 0;

    public PlayerKey()
    {
      this.CreateHandle(new CreateParams());
    }

    ~PlayerKey()
    {
      this.DestroyHandle();
    }

    protected bool _Register(ArrayList hItem)
    {
      for (int index = 0; index < PlayerKey._hHotKey.Count; ++index)
      {
        if (PlayerKey._hHotKey[index] != null && (Keys) PlayerKey._hHotKey[index][1] == (Keys) hItem[1] && (KeysModifier) PlayerKey._hHotKey[index][2] == (KeysModifier) hItem[2])
          return false;
      }
      Keys keys = (Keys) hItem[1];
      KeysModifier keysModifier = (KeysModifier) hItem[2];
      if (!PlayerKey.RegisterHotKey(this.Handle, PlayerKey._iHotKey + 1, (uint) keysModifier, (uint) keys))
      {
        Marshal.GetLastWin32Error();
        return false;
      }
      PlayerKey._hHotKey.Add(PlayerKey._iHotKey, hItem);
      ++PlayerKey._iHotKey;
      return true;
    }

    [DllImport("user32.dll")]
    protected static extern uint GetForegroundWindow();

    [DllImport("user32.dll")]
    protected static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    protected static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public bool Register(AionEventHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      return this._Register(new ArrayList()
      {
        (object) hFunction,
        (object) hKey,
        (object) hModifier,
        (object) false
      });
    }

    public bool Register(AionEventKeyHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      return this._Register(new ArrayList()
      {
        (object) hFunction,
        (object) hKey,
        (object) hModifier,
        (object) true
      });
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    public void Unregister(Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      for (int index = 0; index < PlayerKey._hHotKey.Count; ++index)
      {
        if (PlayerKey._hHotKey[index] != null && (Keys) PlayerKey._hHotKey[index][1] == hKey && (KeysModifier) PlayerKey._hHotKey[index][2] == hModifier)
        {
          PlayerKey.UnregisterHotKey(this.Handle, index + 1);
          PlayerKey._hHotKey[index] = (ArrayList) null;
          break;
        }
      }
    }

    [DllImport("user32.dll")]
    protected static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 786)
      {
        Keys hKey = (Keys) ((int) m.LParam >> 16 & (int) ushort.MaxValue);
        KeysModifier hModifier = (KeysModifier) ((int) m.LParam & (int) ushort.MaxValue);
        uint foregroundWindow = PlayerKey.GetForegroundWindow();
        Dialog dialog = Game.DialogList == null ? (Dialog) null : Game.DialogList.GetDialog("chat_input_dialog", false);
        for (int index = 0; index < PlayerKey._hHotKey.Count; ++index)
        {
          if (PlayerKey._hHotKey[index] != null && (Keys) PlayerKey._hHotKey[index][1] == hKey && (KeysModifier) PlayerKey._hHotKey[index][2] == hModifier)
          {
            if ((int) foregroundWindow == (int) Game.Process.ProcessWindowHandle && (dialog == null || !dialog.IsVisible()))
            {
              if ((bool) PlayerKey._hHotKey[index][3])
              {
                ((AionEventKeyHandler) PlayerKey._hHotKey[index][0])(hKey, hModifier);
                break;
              }
              ((AionEventHandler) PlayerKey._hHotKey[index][0])();
              break;
            }
            PlayerKey.UnregisterHotKey(this.Handle, index + 1);
            PlayerKey.keybd_event((byte) hKey, (byte) 69, 1U, UIntPtr.Zero);
            PlayerKey.keybd_event((byte) hKey, (byte) 69, 3U, UIntPtr.Zero);
            PlayerKey.RegisterHotKey(this.Handle, index + 1, (uint) hModifier, (uint) hKey);
            break;
          }
        }
      }
      else
        base.WndProc(ref m);
    }
  }
}
