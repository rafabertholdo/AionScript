// Decompiled with JetBrains decompiler
// Type: AionInterface.State
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

namespace AionInterface
{
  public class State : Skill
  {
    public State(Skill hSkill)
      : base(hSkill.GetID(), hSkill.GetName())
    {
      this._bActivated = hSkill.GetActivated();
      this._bValid = true;
      this._eDispell = hSkill.GetDispell();
      this._eType = hSkill.GetType();
      this._eTypeSecondary = hSkill.GetTypeSecondary();
    }

    public State(uint iID)
      : base(iID, (string) null)
    {
      this._bValid = true;
    }
  }
}
