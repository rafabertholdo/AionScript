// Decompiled with JetBrains decompiler
// Type: AionInterface.AbilityList
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
  public class AbilityList
  {
    protected List<uint> _hAbilityActive = new List<uint>();
    protected Dictionary<uint, Ability> _hAbilityListID = new Dictionary<uint, Ability>();
    protected Dictionary<string, Ability> _hAbilityListName = new Dictionary<string, Ability>();
    protected Dictionary<string, int> _hAbilityNumeric = new Dictionary<string, int>();
    protected ulong _iBase;

    public Ability this[uint iID]
    {
      get
      {
        return this.GetAbility(iID);
      }
    }

    public Ability this[string zName]
    {
      get
      {
        return this.GetAbility(zName);
      }
    }

    public AbilityList(ulong iBase)
    {
      this._hAbilityNumeric.Add("I", 1);
      this._hAbilityNumeric.Add("II", 2);
      this._hAbilityNumeric.Add("III", 3);
      this._hAbilityNumeric.Add("IV", 4);
      this._hAbilityNumeric.Add("V", 5);
      this._hAbilityNumeric.Add("VI", 6);
      this._hAbilityNumeric.Add("VII", 7);
      this._hAbilityNumeric.Add("VIII", 8);
      this._hAbilityNumeric.Add("IX", 9);
      this._hAbilityNumeric.Add("X", 10);
      this._iBase = iBase;
    }

    protected HashSet<ulong> _Node(ulong iNode, HashSet<ulong> hFound)
    {
      ProcessCommunicationPointer communicationPointer = Game.Process[iNode].ToBuffered((ulong) (Game.Process.PointerSize * 3U));
      if (!hFound.Contains(iNode))
        hFound.Add(iNode);
      for (uint index = 0; index < 3U; ++index)
      {
        ulong pointer;
        if ((long) (pointer = communicationPointer.GetPointer((ulong) (Game.Process.PointerSize * index))) != 0L && !hFound.Contains(pointer))
          hFound = this._Node(pointer, hFound);
      }
      return hFound;
    }

    protected HashSet<ulong> FindNodes48(ulong node, HashSet<ulong> nodes)
    {
      nodes.Add(node);
      ulong pointer = Game.Process.GetPointer(node + (ulong) (Game.Process.PointerSize * 3U));
      if ((long) pointer != 0L && !nodes.Contains(pointer))
        this.FindNodes48(pointer, nodes);
      return nodes;
    }

    public Ability GetAbility(string zName)
    {
      if (zName == null)
        return (Ability) null;
      uint result;
      if (uint.TryParse(zName, out result))
        return this.GetAbility(result);
      Ability ability = (Ability) null;
      int num = 0;
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (this._hAbilityListName.ContainsKey(zName))
          return this._hAbilityListName[zName];
        foreach (KeyValuePair<uint, Ability> keyValuePair in this._hAbilityListID)
        {
          string name;
          string key;
          if ((name = keyValuePair.Value.GetName()) != null && name.StartsWith(zName) && ((key = name.Substring(zName.Length + 1)) != null && this._hAbilityNumeric.ContainsKey(key)) && this._hAbilityNumeric[key] > num)
          {
            ability = keyValuePair.Value;
            num = this._hAbilityNumeric[key];
          }
        }
        return ability;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public Ability GetAbility(uint iID)
    {
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (this._hAbilityListID.ContainsKey(iID))
          return this._hAbilityListID[iID];
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Ability) null;
    }

    public Ability GetAbilityIndex(uint iIndex)
    {
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hAbilityListID.Count)
            return Enumerable.ElementAt<KeyValuePair<uint, Ability>>((IEnumerable<KeyValuePair<uint, Ability>>) this._hAbilityListID, (int) iIndex).Value;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Ability) null;
    }

    public uint GetAbilitySize()
    {
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return (uint) this._hAbilityListID.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public bool GetActivated(uint iID)
    {
      List<uint> list = this._hAbilityActive;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        return this._hAbilityActive.Contains(iID);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public Dictionary<uint, Ability> GetList()
    {
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return this._hAbilityListID;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public AbilityList Update()
    {
      Dictionary<uint, Ability> dictionary = this._hAbilityListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        ulong pointer1 = Game.Process.GetPointer(this._iBase + (ulong) Game.Resolver["AbilityCollection"]["PointerMap"].Value);
        uint num1 = Game.Resolver["AbilityCollection"]["PointerNode"].Value;
        ulong pointer2 = Game.Process.GetPointer(pointer1 + (ulong) num1);
        this._hAbilityListID.Clear();
        this._hAbilityListName.Clear();
        if (Game.Resolver["AbilityCollection"]["LeveledSkillProcessing"] != null)
        {
          foreach (ulong num2 in this._Node(Game.Process.GetPointer(pointer2), new HashSet<ulong>()))
          {
            Ability ability = new Ability(Game.Process.GetPointer(num2 + (ulong) (Game.Process.PointerSize * 4U)), true);
            if (ability.GetID() > 0U)
            {
              this._hAbilityListID[ability.GetID()] = ability;
              this._hAbilityListName[ability.GetName()] = ability;
            }
          }
        }
        else
        {
          foreach (uint num2 in this._Node(pointer2, new HashSet<ulong>()))
          {
            ulong pointer3 = Game.Process.GetPointer((ulong) (num2 + Game.Process.PointerSize * 5U));
            if ((long) pointer3 != 0L)
            {
              try
              {
                ulong pointer4 = Game.Process.GetPointer(pointer3);
                ulong pointer5 = Game.Process.GetPointer(pointer4 + (ulong) (Game.Process.PointerSize * 5U));
                ulong pointer6 = Game.Process.GetPointer(pointer5 + (ulong) Game.Process.PointerSize);
                Ability ability;
                if ((ability = new Ability(Game.Process.GetPointer(pointer6 + (ulong) (Game.Process.PointerSize * 2U)), false)) != null)
                {
                  if (ability.GetID() > 0U)
                  {
                    this._hAbilityListID[ability.GetID()] = ability;
                    this._hAbilityListName[ability.GetName()] = ability;
                  }
                }
              }
              catch (Exception ex)
              {
              }
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

    public AbilityList UpdateActivated()
    {
      List<uint> list = this._hAbilityActive;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        Dialog dialog = Game.DialogList.GetDialog("support_shortcut_dialog", true);
        if (dialog != null)
        {
          this._hAbilityActive.Clear();
          for (uint iID = 0; iID < dialog.GetDialogSize(); ++iID)
          {
            ulong address = dialog.GetDialog(iID, false).GetAddress();
            uint num = Game.Resolver["DialogSingle"]["SkillId"].Value;
            uint unsignedInteger = Game.Process.GetUnsignedInteger(address + (ulong) num);
            if ((int) unsignedInteger != 0)
              this._hAbilityActive.Add(unsignedInteger);
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
