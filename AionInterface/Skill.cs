// Decompiled with JetBrains decompiler
// Type: AionInterface.Skill
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

namespace AionInterface
{
  public class Skill
  {
    protected bool _bActivated;
    protected bool _bValid;
    protected eDispell _eDispell;
    protected eSkillType _eType;
    protected eSkillTypeSecondary _eTypeSecondary;
    protected uint _iID;
    protected string _zName;

    public Skill(uint iID, string zName)
    {
      this._iID = iID;
      this._zName = zName;
    }

    public bool GetActivated()
    {
      return this._bActivated;
    }

    public eDispell GetDispell()
    {
      return this._eDispell;
    }

    public uint GetID()
    {
      return this._iID;
    }

    public string GetName()
    {
      return this._zName;
    }

    public eSkillType GetType()
    {
      return this._eType;
    }

    public eSkillTypeSecondary GetTypeSecondary()
    {
      return this._eTypeSecondary;
    }

    public bool IsAttack()
    {
      return this._eTypeSecondary == eSkillTypeSecondary.Attack;
    }

    public bool IsBuff()
    {
      return this._eTypeSecondary == eSkillTypeSecondary.Buff;
    }

    public bool IsDebuff()
    {
      if (this._eDispell != eDispell.None)
        return this._eDispell != eDispell.Stun;
      return false;
    }

    public bool IsHeal()
    {
      return this._eTypeSecondary == eSkillTypeSecondary.Heal;
    }

    public bool IsMagical()
    {
      return this.IsDebuff() && this._eDispell == eDispell.Mental || !this.IsDebuff() && this._eType == eSkillType.Magical;
    }

    public bool IsPassive()
    {
      return this._eTypeSecondary == eSkillTypeSecondary.Passive;
    }

    public bool IsPhysical()
    {
      return this.IsDebuff() && this._eDispell == eDispell.Physical || !this.IsDebuff() && this._eType == eSkillType.Physical;
    }

    public bool IsStun()
    {
      return this._eDispell == eDispell.Stun;
    }

    public bool IsValid()
    {
      return this._bValid;
    }

    public void SetActivate(bool state)
    {
      this._bActivated = state;
    }

    public void Update(eDispell eDispell, eSkillType eType, eSkillTypeSecondary eTypeSecondary, bool bAcivated)
    {
      this._eDispell = eDispell;
      this._eType = eType;
      this._eTypeSecondary = eTypeSecondary;
      this._bActivated = bAcivated;
      this._bValid = true;
    }
  }
}
