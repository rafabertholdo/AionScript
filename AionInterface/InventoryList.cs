// Decompiled with JetBrains decompiler
// Type: AionInterface.InventoryList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AionInterface
{
  public class InventoryList
  {
    protected Dictionary<uint, int> _hInventoryCacheID = new Dictionary<uint, int>();
    protected Dictionary<string, int> _hInventoryCacheName = new Dictionary<string, int>();
    protected List<Inventory> _hInventoryList = new List<Inventory>();
    protected ulong _iBase;
    protected uint _iInventoryCurrent;
    protected uint _iInventoryMaximum;

    public Inventory this[uint iID]
    {
      get
      {
        return this.GetInventory(iID);
      }
    }

    public Inventory this[string zName]
    {
      get
      {
        return this.GetInventory(zName);
      }
    }

    public InventoryList(ulong iBase)
    {
      this._iBase = iBase;
    }

    protected HashSet<ulong> _Node(ulong iNode, HashSet<ulong> hFound, ulong iInitialNode)
    {
      try
      {
        if ((long) iNode == (long) iInitialNode || hFound.Add(iNode))
        {
          ProcessCommunicationPointer communicationPointer = Game.Process[iNode].ToBuffered((ulong) (Game.Process.PointerSize * 3U));
          long num1 = 0;
          uint unsignedInteger1 = communicationPointer.GetUnsignedInteger((ulong) num1);
          long num2 = (long) Game.Process.PointerSize;
          uint unsignedInteger2 = communicationPointer.GetUnsignedInteger((ulong) num2);
          long num3 = (long) (Game.Process.PointerSize * 2U);
          uint unsignedInteger3 = communicationPointer.GetUnsignedInteger((ulong) num3);
          if ((int) unsignedInteger1 != 0)
            hFound = this._Node((ulong) unsignedInteger1, hFound, iInitialNode);
          if ((int) unsignedInteger2 != 0)
            hFound = this._Node((ulong) unsignedInteger2, hFound, iInitialNode);
          if ((int) unsignedInteger3 != 0)
            hFound = this._Node((ulong) unsignedInteger3, hFound, iInitialNode);
        }
        return hFound;
      }
      catch (Exception ex)
      {
        return hFound;
      }
    }

    public Inventory GetInventory(string zName)
    {
      if (zName != null)
      {
        uint result;
        if (uint.TryParse(zName, out result))
          return this.GetInventory(result);
        List<Inventory> list = this._hInventoryList;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) list, ref lockTaken);
          if (this._hInventoryCacheName.ContainsKey(zName))
            return this._hInventoryList[this._hInventoryCacheName[zName]];
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) list);
        }
      }
      return (Inventory) null;
    }

    public Inventory GetInventory(uint iID)
    {
      List<Inventory> list = this._hInventoryList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        if (this._hInventoryCacheID.ContainsKey(iID))
          return this._hInventoryList[this._hInventoryCacheID[iID]];
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
      return (Inventory) null;
    }

    public Inventory GetInventoryIndex(uint iIndex)
    {
      List<Inventory> list = this._hInventoryList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hInventoryList.Count)
            return this._hInventoryList[(int) iIndex];
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
      return (Inventory) null;
    }

    public uint GetInventorySize()
    {
      List<Inventory> list = this._hInventoryList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        return (uint) this._hInventoryList.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public List<Inventory> GetList()
    {
      List<Inventory> list = this._hInventoryList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        return this._hInventoryList;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public uint GetSlotCurrent()
    {
      if (this._iInventoryCurrent <= 1U)
        return 0;
      return this._iInventoryCurrent - 1U;
    }

    public uint GetSlotMaximum()
    {
      return this._iInventoryMaximum;
    }

    public InventoryList Update()
    {
      List<Inventory> list = this._hInventoryList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        ulong pointer1 = Game.Process.GetPointer(this._iBase + (ulong) Game.Resolver["InventoryCollection"]["PointerMap"].Value);
        uint num1 = Game.Resolver["InventoryCollection"]["PointerNode"].Value;
        ulong pointer2 = Game.Process.GetPointer(pointer1 + (ulong) num1);
        this._hInventoryCacheID.Clear();
        this._hInventoryCacheName.Clear();
        this._hInventoryList.Clear();
        this._iInventoryCurrent = 0U;
        this._iInventoryMaximum = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["InventoryCollection"]["MaximumSize"].Value);
        foreach (uint num2 in this._Node(pointer2, new HashSet<ulong>(), pointer1))
        {
          ulong pointer3 = Game.Process.GetPointer((ulong) (num2 + Game.Process.PointerSize * 4U));
          if ((long) pointer3 != 0L)
          {
            Inventory inventory = new Inventory(pointer3);
            if (inventory.GetName() != null && inventory.GetName().Length > 0)
            {
              this._hInventoryCacheID[inventory.GetID()] = this._hInventoryList.Count;
              this._hInventoryCacheName[inventory.GetName()] = this._hInventoryList.Count;
              this._hInventoryList.Add(inventory);
            }
            if (inventory.GetSlot() == eInventorySlotType.None)
              this._iInventoryCurrent = this._iInventoryCurrent + 1U;
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
