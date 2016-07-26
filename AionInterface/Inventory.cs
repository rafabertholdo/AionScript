// Decompiled with JetBrains decompiler
// Type: AionInterface.Inventory
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken.Messaging;
using System;

namespace AionInterface
{
  public class Inventory
  {
    protected eInventorySlotType _eSlot;
    protected eInventoryType _eType;
    protected MessageHandler _hCooldown;
    protected uint _iAmount;
    protected uint _iID;
    protected ulong _iInventory;
    protected uint _iReuse;
    protected string _zName;

    public Inventory(ulong iInventory)
    {
      if ((long) iInventory == 0L)
        return;
      uint unsignedInteger = Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["BufferSize"].Value);
      this._iID = Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["Id"].Value);
      this._iAmount = Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["Amount"].Value);
      this._iReuse = Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["CooldownTime"].Value);
      this._eType = (eInventoryType) Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["Type"].Value);
      this._zName = Game.Process.GetString(unsignedInteger > 7U ? Game.Process.GetPointer(iInventory + (ulong) Game.Resolver["InventorySingle"]["Buffer"].Value) : iInventory + (ulong) Game.Resolver["InventorySingle"]["Buffer"].Value, 128U, MessageHandlerString.Unicode);
      this._hCooldown = (MessageHandler) Game.Process[iInventory + (ulong) Game.Resolver["InventorySingle"]["CooldownTimeRemaining"].Value];
      this._eSlot = (eInventorySlotType) Game.Process.GetUnsignedInteger(iInventory + (ulong) Game.Resolver["InventorySingle"]["Slot"].Value);
      this._iInventory = iInventory;
    }

    public ulong GetAddress()
    {
      return this._iInventory;
    }

    public uint GetAmount()
    {
      return this._iAmount;
    }

    public uint GetCooldown()
    {
      uint num = (uint) Environment.TickCount;
      uint unsignedInteger = this._hCooldown.GetUnsignedInteger(0UL);
      if (unsignedInteger > num)
        return unsignedInteger - num;
      return 0;
    }

    public uint GetID()
    {
      return this._iID;
    }

    public string GetName()
    {
      return this._zName;
    }

    public uint GetReuse()
    {
      return this._iReuse;
    }

    public eInventorySlotType GetSlot()
    {
      return this._eSlot;
    }

    public eInventoryType GetType()
    {
      return this._eType;
    }
  }
}
