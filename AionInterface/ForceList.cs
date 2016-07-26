// Decompiled with JetBrains decompiler
// Type: AionInterface.ForceList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AionInterface
{
  public class ForceList
  {
    protected Dictionary<uint, Force> _hForceListID = new Dictionary<uint, Force>();
    protected Dictionary<string, Force> _hForceListName = new Dictionary<string, Force>();
    protected ulong _iBase;
    protected uint _iForceLeader;

    public Force this[uint iID]
    {
      get
      {
        return this.GetForce(iID);
      }
    }

    public Force this[string zName]
    {
      get
      {
        return this.GetForce(zName);
      }
    }

    public ForceList(ulong iBase)
    {
      this._iBase = iBase;
    }

    public Force GetForce(string zName)
    {
      if (zName != null)
      {
        uint result;
        if (uint.TryParse(zName, out result))
          return this.GetForce(result);
        Dictionary<uint, Force> dictionary = this._hForceListID;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) dictionary, ref lockTaken);
          if (this._hForceListName.ContainsKey(zName))
            return this._hForceListName[zName];
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) dictionary);
        }
      }
      return (Force) null;
    }

    public Force GetForce(uint iID)
    {
      Dictionary<uint, Force> dictionary = this._hForceListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (this._hForceListID.ContainsKey(iID))
          return this._hForceListID[iID];
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Force) null;
    }

    public Force GetForceIndex(uint iIndex)
    {
      Dictionary<uint, Force> dictionary = this._hForceListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hForceListID.Count)
            return Enumerable.ElementAt<KeyValuePair<uint, Force>>((IEnumerable<KeyValuePair<uint, Force>>) this._hForceListID, (int) iIndex).Value;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Force) null;
    }

    public Force GetForceLeader()
    {
      return this.GetForce(this._iForceLeader);
    }

    public uint GetForceSize()
    {
      Dictionary<uint, Force> dictionary = this._hForceListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return (uint) this._hForceListID.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public Dictionary<uint, Force> GetList()
    {
      Dictionary<uint, Force> dictionary = this._hForceListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return this._hForceListID;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public ForceList Update()
    {
      Dictionary<uint, Force> dictionary = this._hForceListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        uint unsignedInteger1 = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["GroupCollection"]["PartySize"].Value);
        uint unsignedInteger2 = Game.Process.GetUnsignedInteger(this._iBase + ((int) unsignedInteger1 == 0 ? (ulong) Game.Resolver["GroupCollection"]["AllianceSize"].Value : (ulong) Game.Resolver["GroupCollection"]["PartySize"].Value));
        ulong lAddress = (ulong) Game.Process.GetUnsignedInteger(this._iBase + ((int) unsignedInteger1 == 0 ? (ulong) Game.Resolver["GroupCollection"]["AllianceMap"].Value : (ulong) Game.Resolver["GroupCollection"]["PartyMap"].Value));
        this._hForceListID.Clear();
        this._hForceListName.Clear();
        this._iForceLeader = Game.Process.GetUnsignedInteger(this._iBase + ((int) unsignedInteger1 == 0 ? (ulong) Game.Resolver["GroupCollection"]["AllianceLeaderId"].Value : (ulong) Game.Resolver["GroupCollection"]["PartyLeaderId"].Value));
        for (int index = 0; (long) index < (long) unsignedInteger2; ++index)
        {
          ulong pointer;
          if ((long) (lAddress = Game.Process.GetPointer(lAddress)) != 0L && (long) (pointer = Game.Process.GetPointer(lAddress + (ulong) (Game.Process.PointerSize * 2U))) != 0L)
          {
            Force force = new Force(pointer, this._iForceLeader).Update();
            this._hForceListID[force.GetID()] = force;
            this._hForceListName[force.GetName()] = force;
          }
        }
        return this;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }
  }
}
