// Decompiled with JetBrains decompiler
// Type: AionInterface.EntityList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AionInterface
{
  public class EntityList
  {
    protected Dictionary<uint, Entity> _hEntityListID = new Dictionary<uint, Entity>();
    protected Dictionary<string, Entity> _hEntityListName = new Dictionary<string, Entity>();
    protected ulong _iBase;

    public Entity this[uint iID]
    {
      get
      {
        return this.GetEntity(iID);
      }
    }

    public Entity this[string zName]
    {
      get
      {
        return this.GetEntity(zName);
      }
    }

    public EntityList(ulong iBase)
    {
      this._iBase = iBase;
    }

    protected HashSet<ulong> _Node(ulong iNode, ref HashSet<ulong> hEntityList, ref HashSet<ulong> hLinkedList, uint iDeep = 0)
    {
      try
      {
        if (iDeep < 5000U)
        {
          ulong pointer1 = Game.Process.GetPointer(iNode + (ulong) (Game.Process.PointerSize * 3U));
          if ((long) pointer1 != 0L)
            hEntityList.Add(pointer1);
          ulong pointer2;
          if ((long) (pointer2 = Game.Process.GetPointer(iNode)) != 0L && !hLinkedList.Contains(pointer2))
          {
            hLinkedList.Add(pointer2);
            this._Node(pointer2, ref hEntityList, ref hLinkedList, iDeep + 1U);
          }
        }
        return hEntityList;
      }
      catch (Exception ex)
      {
        return hEntityList;
      }
    }

    public Entity GetEntity(string zName)
    {
      if (zName != null)
      {
        uint result;
        if (uint.TryParse(zName, out result))
          return this.GetEntity(result);
        Dictionary<uint, Entity> dictionary = this._hEntityListID;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) dictionary, ref lockTaken);
          if (this._hEntityListName.ContainsKey(zName))
            return this._hEntityListName[zName];
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) dictionary);
        }
      }
      return (Entity) null;
    }

    public Entity GetEntity(uint iID)
    {
      if ((int) iID != 0)
      {
        Dictionary<uint, Entity> dictionary = this._hEntityListID;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) dictionary, ref lockTaken);
          if (this._hEntityListID.ContainsKey(iID))
            return this._hEntityListID[iID];
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) dictionary);
        }
      }
      return (Entity) null;
    }

    public Entity GetEntityIndex(uint iIndex)
    {
      Dictionary<uint, Entity> dictionary = this._hEntityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) this._hEntityListID.Count > (long) iIndex)
            return Enumerable.ElementAt<KeyValuePair<uint, Entity>>((IEnumerable<KeyValuePair<uint, Entity>>) this._hEntityListID, (int) iIndex).Value;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Entity) null;
    }

    public uint GetEntitySize()
    {
      Dictionary<uint, Entity> dictionary = this._hEntityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return (uint) this._hEntityListID.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public Dictionary<uint, Entity> GetList()
    {
      Dictionary<uint, Entity> dictionary = this._hEntityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return this._hEntityListID;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public EntityList Update()
    {
      Dictionary<uint, Entity> dictionary = this._hEntityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        ulong pointer1 = Game.Process.GetPointer(this._iBase + (ulong) Game.Resolver["ActorCollection"]["PointerMap"].Value);
        ulong pointer2 = Game.Process.GetPointer(pointer1 + (ulong) Game.Resolver["ActorCollection"]["PointerNode"].Value);
        this._hEntityListID.Clear();
        this._hEntityListName.Clear();
        if ((long) pointer2 != 0L)
        {
          HashSet<ulong> hEntityList = new HashSet<ulong>();
          HashSet<ulong> hLinkedList = new HashSet<ulong>();
          this._Node(pointer2, ref hEntityList, ref hLinkedList, 0U);
          foreach (ulong iEntityNode in hEntityList)
          {
            Entity entity;
            if ((entity = new Entity(iEntityNode).Update()) != null && entity.IsValid())
            {
              this._hEntityListID[entity.GetID()] = entity;
              this._hEntityListName[entity.GetName()] = entity;
            }
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
