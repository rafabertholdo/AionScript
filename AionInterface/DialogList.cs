// Decompiled with JetBrains decompiler
// Type: AionInterface.DialogList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using Keiken.Messaging;
using System.Collections.Generic;
using System.Threading;

namespace AionInterface
{
  public class DialogList
  {
    protected List<ulong> _hDialogAddress = new List<ulong>();
    protected Dictionary<string, uint> _hDialogCache = new Dictionary<string, uint>();
    protected List<Dialog> _hDialogList = new List<Dialog>();
    protected ulong _iBase;
    protected uint _iUpdateTime;

    public Dialog this[string zName]
    {
      get
      {
        return this.GetDialog(zName, true);
      }
    }

    public Dialog this[uint iID]
    {
      get
      {
        return this.GetDialog(iID, true);
      }
    }

    public DialogList(ulong iBase)
    {
      this._iBase = iBase;
    }

    public Dialog GetDialog(string zName, bool bChildUpdate = true)
    {
      int result = 0;
      if (zName == null)
        return (Dialog) null;
      if (!zName.Contains("/") && !zName.Contains("\\"))
      {
        if (int.TryParse(zName, out result))
          return this.GetDialog((uint) result, bChildUpdate);
        List<Dialog> list = this._hDialogList;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) list, ref lockTaken);
          foreach (Dialog dialog in this._hDialogList)
          {
            if (dialog.GetName() == zName || dialog.GetDialog(zName, true) != null)
              return dialog.Update(bChildUpdate);
          }
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) list);
        }
        return (Dialog) null;
      }
      DialogList dialogList = this;
      char[] chArray = new char[1]
      {
        '/'
      };
      foreach (string str in zName.Replace('\\', '/').Split(chArray))
      {
        if (dialogList != null)
          dialogList = !int.TryParse(str, out result) ? (DialogList) dialogList.GetDialog(str, true) : (DialogList) dialogList.GetDialog((uint) result, true);
        else
          break;
      }
      if (dialogList == this)
        return (Dialog) null;
      return (Dialog) dialogList;
    }

    public Dialog GetDialog(uint iID, bool bChildUpdate = true)
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        foreach (Dialog dialog in this._hDialogList)
        {
          if ((int) dialog.GetIndex() == (int) iID)
            return dialog.Update(bChildUpdate);
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
      return (Dialog) null;
    }

    public Dialog GetDialogIndex(uint iIndex, bool bChildUpdate = true)
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hDialogList.Count)
            return this._hDialogList[(int) iIndex].Update(true);
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
      return (Dialog) null;
    }

    public uint GetDialogSize()
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        return (uint) this._hDialogList.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public List<Dialog> GetList()
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        return this._hDialogList;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public DialogList Update()
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        if (this == Game.DialogList && (this._hDialogList.Count == 0 || this._iUpdateTime < Game.Time()))
        {
          ProcessCommunicationPointer communicationPointer = Game.Process[this._iBase + (ulong) Game.Resolver["DialogCollection"]["Array"].Value].ToBuffered((ulong) Game.Resolver["DialogCollection"]["Size"].Value);
          for (uint iIndex = 0; iIndex < Game.Resolver["DialogCollection"]["Size"].Value / Game.Process.PointerSize; ++iIndex)
          {
            ulong pointer = communicationPointer.GetPointer((ulong) (Game.Process.PointerSize * iIndex));
            if (!this._hDialogAddress.Contains(pointer) && (long) pointer != 0L && (Game.Process.GetString(pointer + (ulong) Game.Resolver["DialogCollection"]["Validate"].Value, 4U, MessageHandlerString.ASCII) == "Vera" || Game.Process.GetString(pointer + (ulong) Game.Resolver["DialogCollection"]["Validate"].Value, 8U, MessageHandlerString.ASCII) == "MYRIADPR"))
            {
              this._hDialogList.Add(new Dialog(this._iBase, pointer, iIndex, (Dialog) null));
              this._hDialogAddress.Add(pointer);
            }
          }
          this._hDialogList.Sort();
          this._iUpdateTime = Game.Time() + 1000U;
        }
        return this;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }
  }
}
