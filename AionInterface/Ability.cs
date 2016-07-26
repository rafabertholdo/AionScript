// Decompiled with JetBrains decompiler
// Type: AionInterface.Ability
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken.Messaging;
using System;
using System.Collections.Generic;

namespace AionInterface
{
  public class Ability
  {
    protected MessageHandler _hCooldown;
    protected Skill _hSkill;
    protected ulong _iAbility;
    protected uint _iID;
    private uint _iLevel;
    protected uint _iReuse;
    protected string _zName;
    protected string _zRealName;

    public Ability(ulong iAbility, bool attemptNormalization = false)
    {
      if ((long) iAbility == 0L)
        return;
      this._iID = Game.Process.GetUnsignedInteger(iAbility + (ulong) Game.Resolver["AbilitySingle"]["Id"].Value);
      this._zName = Game.Process.GetString(Game.Process.GetUnsignedInteger(iAbility + (ulong) Game.Resolver["AbilitySingle"]["BufferSize"].Value) > 7U ? (ulong) Game.Process.GetUnsignedInteger(iAbility + (ulong) Game.Resolver["AbilitySingle"]["Buffer"].Value) : iAbility + (ulong) Game.Resolver["AbilitySingle"]["Buffer"].Value, 128U, MessageHandlerString.Unicode);
      this._iReuse = Game.Process.GetUnsignedInteger(iAbility + (ulong) Game.Resolver["AbilitySingle"]["CooldownTime"].Value);
      this._hCooldown = (MessageHandler) Game.Process[iAbility + (ulong) Game.Resolver["AbilitySingle"]["CooldownTimeRemaining"].Value];
      this._hSkill = Game.SkillList.GetSkill(this._zName) ?? Game.SkillList.GetSkill(this._iID);
      this._iAbility = iAbility;
      if (!attemptNormalization)
        return;
      this._iLevel = Game.Process.GetUnsignedInteger(iAbility + (ulong) Game.Resolver["AbilitySingle"]["Level"].Value);
    }

    public bool GetActivated()
    {
      if (Game.SkillList.GetActivated(this._zName))
        return Game.AbilityList.GetActivated(this._iID);
      return true;
    }

    public ulong GetAddress()
    {
      return this._iAbility;
    }

    public uint GetCooldown(bool inner = false)
    {
      if (this._hCooldown != null)
      {
        uint num = (uint) Environment.TickCount;
        uint unsignedInteger = this._hCooldown.GetUnsignedInteger(0UL);
        if (unsignedInteger > num)
          return unsignedInteger - num;
      }
      if (!inner)
      {
        foreach (KeyValuePair<uint, Ability> keyValuePair in Game.AbilityList.GetList())
        {
          if (keyValuePair.Value.GetName().StartsWith(this._zName))
          {
            uint cooldown = keyValuePair.Value.GetCooldown(true);
            if (cooldown > 0U)
              return cooldown;
          }
        }
      }
      return 0;
    }

    public uint GetID()
    {
      return this._iID;
    }

    public uint GetLevel()
    {
      return this._iLevel;
    }

    public string GetName()
    {
      return this._zName;
    }

    public string GetRealName()
    {
      return this._zRealName ?? this._zName;
    }

    public uint GetReuse()
    {
      return this._iReuse;
    }
  }
}
