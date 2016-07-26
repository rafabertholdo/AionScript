// Decompiled with JetBrains decompiler
// Type: AionInterface.StateList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AionInterface
{
  public class StateList
  {
    protected Dictionary<uint, State> _hStateListID = new Dictionary<uint, State>();
    protected Dictionary<string, State> _hStateListName = new Dictionary<string, State>();
    protected ulong _iEntity;

    public State this[uint iID]
    {
      get
      {
        return this.GetState(iID);
      }
    }

    public State this[string zName]
    {
      get
      {
        return this.GetState(zName);
      }
    }

    public StateList(ulong iEntity)
    {
      this._iEntity = iEntity;
    }

    protected HashSet<ulong> _Node(ulong iNode, HashSet<ulong> hFound, ulong iInitialNode, uint iStateCount)
    {
      try
      {
        if (((long) iNode == (long) iInitialNode || hFound.Add(iNode)) && (long) hFound.Count <= (long) iStateCount)
        {
          ProcessCommunicationPointer communicationPointer = Game.Process[iNode].ToBuffered((ulong) (Game.Process.PointerSize * 3U));
          long num1 = 0;
          uint unsignedInteger1 = communicationPointer.GetUnsignedInteger((ulong) num1);
          long num2 = (long) Game.Process.PointerSize;
          uint unsignedInteger2 = communicationPointer.GetUnsignedInteger((ulong) num2);
          long num3 = (long) (Game.Process.PointerSize * 2U);
          uint unsignedInteger3 = communicationPointer.GetUnsignedInteger((ulong) num3);
          if ((int) unsignedInteger1 != 0)
            hFound = this._Node((ulong) unsignedInteger1, hFound, iInitialNode, iStateCount);
          if ((int) unsignedInteger2 != 0)
            hFound = this._Node((ulong) unsignedInteger2, hFound, iInitialNode, iStateCount);
          if ((int) unsignedInteger3 != 0)
            hFound = this._Node((ulong) unsignedInteger3, hFound, iInitialNode, iStateCount);
        }
        return hFound;
      }
      catch (Exception ex)
      {
        return hFound;
      }
    }

    public Dictionary<uint, State> GetList()
    {
      Dictionary<uint, State> dictionary = this._hStateListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return this._hStateListID;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public State GetState(string zName)
    {
      if (zName != null)
      {
        uint result;
        if (uint.TryParse(zName, out result))
          return this.GetState(result);
        Dictionary<uint, State> dictionary = this._hStateListID;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) dictionary, ref lockTaken);
          if (this._hStateListName.ContainsKey(zName))
            return this._hStateListName[zName];
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) dictionary);
        }
      }
      return (State) null;
    }

    public State GetState(uint iId)
    {
      Dictionary<uint, State> dictionary = this._hStateListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (this._hStateListID.ContainsKey(iId))
          return this._hStateListID[iId];
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (State) null;
    }

    public State GetStateIndex(uint iIndex)
    {
      Dictionary<uint, State> dictionary = this._hStateListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hStateListID.Count)
            return Enumerable.ElementAt<KeyValuePair<uint, State>>((IEnumerable<KeyValuePair<uint, State>>) this._hStateListID, (int) iIndex).Value;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (State) null;
    }

    public uint GetStateSize()
    {
      Dictionary<uint, State> dictionary = this._hStateListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return (uint) this._hStateListID.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public StateList Update()
    {
      Dictionary<uint, State> dictionary = this._hStateListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        ulong pointer = Game.Process.GetPointer(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["StateCollectionMap"].Value);
        uint unsignedInteger1 = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["StateCollectionSize"].Value);
        HashSet<ulong> hFound = new HashSet<ulong>();
        this._hStateListID.Clear();
        if ((int) unsignedInteger1 != 0 && (int) unsignedInteger1 != -842150451 && (unsignedInteger1 <= 32U && (long) pointer != 0L))
        {
          foreach (ulong num in this._Node(pointer, hFound, pointer, unsignedInteger1 * 4U))
          {
            uint unsignedInteger2 = Game.Process.GetUnsignedInteger(num + (ulong) Game.Resolver["ActorState"]["Id"].Value);
            Skill skill = Game.SkillList.GetSkill(unsignedInteger2);
            if (skill != null)
            {
              State state = new State(skill);
              this._hStateListID[state.GetID()] = state;
              this._hStateListName[state.GetName()] = state;
            }
            else
              this._hStateListID[unsignedInteger2] = new State(unsignedInteger2);
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
