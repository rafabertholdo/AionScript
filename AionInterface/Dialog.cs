// Decompiled with JetBrains decompiler
// Type: AionInterface.Dialog
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AionInterface
{
  public class Dialog : DialogList, IComparable<Dialog>
  {
    protected bool _bEnabled;
    protected bool _bHTML;
    protected bool _bVisible;
    protected Vector2D _hPadding;
    protected Dialog _hParent;
    protected Vector2D _hPositionAbsolute;
    protected Vector2D _hPositionRelative;
    protected Vector2D _hSize;
    protected ulong _iDialog;
    protected uint _iHTML;
    protected uint _iIndex;
    protected string _zName;
    protected string _zText;

    public new Dialog this[uint iIndex]
    {
      get
      {
        return this.GetDialog(iIndex, true);
      }
    }

    public Dialog(ulong iBase, ulong iDialog, uint iIndex = 0, Dialog hParent = null)
      : base(iBase)
    {
      this._hPadding = new Vector2D(0U, 0U);
      this._hPositionAbsolute = new Vector2D(0U, 0U);
      this._hPositionRelative = new Vector2D(0U, 0U);
      this._hSize = new Vector2D(0U, 0U);
      if ((long) iDialog == 0L)
        return;
      uint unsignedInteger = Game.Process.GetUnsignedInteger(iDialog + (ulong) Game.Resolver["DialogSingle"]["BufferSize"].Value);
      this._zName = Game.Process.GetString(unsignedInteger >= 31U ? Game.Process.GetPointer(iDialog + (ulong) Game.Resolver["DialogSingle"]["Buffer"].Value) : iDialog + (ulong) Game.Resolver["DialogSingle"]["Buffer"].Value, 32U, MessageHandlerString.ASCII);
      this._bHTML = this._zName.Contains("html");
      this._iDialog = iDialog;
      this._iIndex = iIndex;
      this._hParent = hParent;
      this.UpdateDialog();
    }

    public bool Click()
    {
      if (!this.IsEnabled() || !this.IsVisible())
        return false;
      uint X = this._hPositionAbsolute.X + (uint) (0.5 * (double) this._hSize.X);
      uint Y = this._hPositionAbsolute.Y + (uint) (0.5 * (double) this._hSize.Y);
      return Game.PlayerInput.Click(X, Y);
    }

    public int CompareTo(Dialog hOther)
    {
      return this._zName.CompareTo(hOther.GetName());
    }

    public ulong GetAddress()
    {
      return this._iDialog;
    }

    public new Dialog GetDialog(uint iID, bool bChildUpdate = true)
    {
      return this.GetDialogIndex(iID, bChildUpdate);
    }

    public uint GetHTML()
    {
      return this._iHTML;
    }

    public uint GetIndex()
    {
      return this._iIndex;
    }

    public string GetName()
    {
      return this._zName;
    }

    public Vector2D GetPadding()
    {
      return (Vector2D) this._hPadding.Clone();
    }

    public Dialog GetParent()
    {
      return this._hParent;
    }

    public Vector2D GetPosition()
    {
      return (Vector2D) this._hPositionAbsolute.Clone();
    }

    public Vector2D GetSize()
    {
      return (Vector2D) this._hSize.Clone();
    }

    public string GetText()
    {
      return this._zText;
    }

    public bool IsEnabled()
    {
      return this._bEnabled;
    }

    public bool IsVisible()
    {
      return this._bVisible;
    }

    public void SetEnabled(bool bEnabled)
    {
      int num1 = (int) Game.Process.GetByte(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["State"].Value);
      int num2 = !bEnabled ? num1 & -3 : num1 | 2;
      Game.Process.SetByte(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["State"].Value, (byte) num2);
      this._bEnabled = bEnabled;
    }

    public void SetVisible(bool bVisible)
    {
      int num1 = (int) Game.Process.GetByte(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["State"].Value);
      int num2 = !bVisible ? num1 & -2 : num1 | 1;
      Game.Process.SetByte(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["State"].Value, (byte) num2);
      this._bVisible = bVisible;
    }

    public Dialog Update(bool bChildUpdate = true)
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        byte @byte = Game.Process.GetByte(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["State"].Value);
        Dialog dialog = this._hParent;
        if (this._bHTML)
          this._iHTML = Game.Process.GetUnsignedInteger(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["HtmlId"].Value);
        if (!bChildUpdate && this._hParent != null)
          this._hParent.Update(false);
        this._bEnabled = (this._hParent == null || this._hParent.IsEnabled()) && ((int) @byte & 2) == 2;
        this._bVisible = (this._hParent == null || this._hParent.IsVisible()) && ((int) @byte & 1) == 1;
        this._hPadding = new Vector2D((uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Padding"].Value), (uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Padding"].Value + 8UL));
        this._hPositionRelative = new Vector2D((uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Position"].Value), (uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Position"].Value + 8UL));
        this._hSize = new Vector2D((uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Size"].Value), (uint) Game.Process.GetDouble(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["Size"].Value + 8UL));
        this._hPositionAbsolute = this._hParent == null ? this._hPositionRelative : new Vector2D(this._hPositionRelative.X + this._hParent.GetPosition().X + this._hParent.GetPadding().X, this._hPositionRelative.Y + this._hParent.GetPosition().Y + this._hParent.GetPadding().Y);
        if (bChildUpdate)
          this.UpdateDialog();
        return this;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public Dialog UpdateDialog()
    {
      List<Dialog> list = this._hDialogList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        uint unsignedInteger = Game.Process.GetUnsignedInteger(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["ChildSize"].Value);
        uint iIndex = 0;
        ulong pointer1 = Game.Process.GetPointer(this._iDialog + (ulong) Game.Resolver["DialogSingle"]["ChildMap"].Value);
        if ((long) pointer1 != 0L)
        {
          this._hDialogList.Clear();
          for (uint index = 0; index < unsignedInteger && (long) pointer1 != 0L; ++index)
          {
            ulong pointer2;
            if ((long) (pointer1 = Game.Process.GetPointer(pointer1)) != 0L && (long) (pointer2 = Game.Process.GetPointer(pointer1 + (ulong) (Game.Process.PointerSize * 2U))) != 0L)
            {
              this._hDialogList.Add(new Dialog(this._iBase, pointer2, iIndex, this).Update(true));
              ++iIndex;
            }
          }
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
