// Decompiled with JetBrains decompiler
// Type: AionInterface.PlayerInput
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken.Interoperability;
using Keiken.Messaging;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AionInterface
{
  public class PlayerInput
  {
    protected static PlayerKey _hPlayerKey;
    protected uint _iAbilityID;
    protected uint _iAbilityTime;
    protected ulong _iBase;
    protected uint _iClickTime;
    protected ulong _iConsolePointer;
    protected uint _iConsoleTime;
    protected uint _iInventoryID;
    protected uint _iInventoryTime;

    public PlayerInput(ulong iBase)
    {
      this._iBase = iBase;
    }

    public bool Ability(string zName)
    {
      uint result;
      if (uint.TryParse(zName, out result))
        return this.Ability(result);
      Ability ability = Game.AbilityList.GetAbility(zName);
      if (ability == null)
        return false;
      return this.Ability(ability.GetID());
    }

    public bool Ability(uint iID)
    {
      Ability ability = Game.AbilityList.GetAbility(iID);
      if (ability == null || (int) ability.GetCooldown(false) != 0 || Game.Player.IsBusy())
        return false;
      if ((int) this._iAbilityID == (int) ability.GetID() && this._iAbilityTime >= Game.Time())
        return true;
      this._iAbilityID = ability.GetID();
      this._iAbilityTime = Game.Time() + 500U;
      return this.Console("/Skill " + ability.GetRealName());
    }

    public bool Click(uint X, uint Y)
    {
      if ((int) this._iClickTime != 0)
        return false;
      Game.Process.SetByte(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickDisabler"].Value, (byte) 0);
      Game.Process.SetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickX1"].Value, X);
      Game.Process.SetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickX2"].Value, X);
      Game.Process.SetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickY1"].Value, Y);
      Game.Process.SetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickY2"].Value, Y);
      PlayerInput.SendMessage(Game.Process.ProcessWindowHandle, 513U, 1U, 0U);
      PlayerInput.SendMessage(Game.Process.ProcessWindowHandle, 514U, 1U, 0U);
      Game.Process.SetByte(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickDisabler"].Value, (byte) 2);
      return true;
    }

    public bool Console(string zMessage)
    {
      Dialog dialog = Game.DialogList.GetDialog("chat_input_dialog", false);
      long ticks = DateTime.Now.Ticks;
      if (dialog == null || dialog.IsVisible())
        return false;
      if ((long) this._iConsolePointer == 0L)
      {
        ulong address = dialog.GetAddress();
        uint num = Game.Resolver["ControllerSingleInput"]["ConsoleMap"].Value;
        ulong pointer = Game.Process.GetPointer(address + (ulong) num);
        if ((int) Game.Process.GetUnsignedInteger(pointer + (ulong) Game.Resolver["ControllerSingleInput"]["ConsoleMapLength"].Value) == 256)
        {
          this._iConsolePointer = Game.Process.GetPointer(pointer + (ulong) Game.Resolver["ControllerSingleInput"]["ConsoleMapPointer"].Value);
        }
        else
        {
          this._iConsolePointer = Game.Process.MemoryAllocate(512UL, MemoryProtection.ExecuteReadWrite, MemoryAllocationType.Commit);
          if ((long) this._iConsolePointer == 0L)
            return false;
          Game.Process.SetPointer(pointer + (ulong) Game.Resolver["ControllerSingleInput"]["ConsoleMapPointer"].Value, this._iConsolePointer);
          Game.Process.SetUnsignedInteger(pointer + (ulong) Game.Resolver["ControllerSingleInput"]["ConsoleMapLength"].Value, 256U);
        }
      }
      Game.Process.SetString(this._iConsolePointer, zMessage, 512U, MessageHandlerString.Unicode);
      PlayerInput.PostMessage(Game.Process.ProcessWindowHandle, 256U, 13U, 0U);
      PlayerInput.PostMessage(Game.Process.ProcessWindowHandle, 256U, 13U, 0U);
      while ((int) Game.Process.GetByte(this._iConsolePointer) != 0)
      {
        if ((DateTime.Now.Ticks - ticks) / 10000L >= 500L)
        {
          this._iConsoleTime = Game.Time() + 30000U;
          break;
        }
        Thread.Sleep(10);
      }
      return true;
    }

    public void Escape()
    {
      PlayerInput.SendMessage(Game.Process.ProcessWindowHandle, 256U, 27U, 0U);
    }

    public bool Inventory(string zName)
    {
      uint result;
      if (uint.TryParse(zName, out result))
        return this.Inventory(result);
      Inventory inventory = Game.InventoryList.GetInventory(zName);
      if (inventory == null)
        return false;
      return this.Inventory(inventory.GetID());
    }

    public bool Inventory(uint iID)
    {
      Inventory inventory = Game.InventoryList.GetInventory(iID);
      if (inventory == null || (int) inventory.GetCooldown() != 0 || Game.Player.IsBusy())
        return false;
      if ((int) this._iInventoryTime == (int) inventory.GetID() && this._iInventoryTime >= Game.Time())
        return true;
      this._iInventoryID = inventory.GetID();
      this._iInventoryTime = Game.Time() + 1000U;
      return this.Console("/Use " + inventory.GetName());
    }

    [DllImport("user32.dll")]
    protected static extern bool PostMessage(uint hWnd, uint msg, uint wParam, uint lParam);

    public bool Register(AionEventHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      if (PlayerInput._hPlayerKey != null)
        return PlayerInput._hPlayerKey.Register(hFunction, hKey, hModifier);
      return false;
    }

    public bool Register(AionEventKeyHandler hFunction, Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      if (PlayerInput._hPlayerKey != null)
        return PlayerInput._hPlayerKey.Register(hFunction, hKey, hModifier);
      return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RegisterThread()
    {
      if (PlayerInput._hPlayerKey != null)
        return;
      PlayerInput._hPlayerKey = new PlayerKey();
    }

    [DllImport("user32.dll")]
    protected static extern bool SendMessage(uint hWnd, uint msg, uint wParam, uint lParam);

    public void Unregister(Keys hKey, KeysModifier hModifier = KeysModifier.None)
    {
      if (PlayerInput._hPlayerKey == null)
        return;
      PlayerInput._hPlayerKey.Unregister(hKey, hModifier);
    }

    public void Update()
    {
      if ((int) this._iConsoleTime != 0 && this._iConsoleTime >= Game.Time())
      {
        Dialog dialog = Game.DialogList.GetDialog("chat_input_dialog", false);
        Thread.Sleep(500);
        if (dialog == null)
          return;
        if (!dialog.IsVisible())
        {
          this._iConsoleTime = 0U;
          return;
        }
        PlayerInput.PostMessage(Game.Process.ProcessWindowHandle, 256U, 13U, 0U);
      }
      if ((int) this._iClickTime == 0 || this._iClickTime >= Game.Time())
        return;
      PlayerInput.SendMessage(Game.Process.ProcessWindowHandle, 514U, 1U, 0U);
      Game.Process.SetByte(this._iBase + (ulong) Game.Resolver["ControllerSingleInput"]["ClickDisabler"].Value, (byte) 2);
      this._iClickTime = 0U;
    }
  }
}
