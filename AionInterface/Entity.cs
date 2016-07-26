// Decompiled with JetBrains decompiler
// Type: AionInterface.Entity
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using Keiken.Messaging;
using System;
using System.IO;
using System.Media;

namespace AionInterface
{
  public class Entity
  {
    protected byte _bHealth = byte.MaxValue;
    protected bool _bDuelling;
    protected bool _bElyos;
    protected byte _bLevel;
    protected bool _bUpdated;
    protected bool _bValid;
    protected eAttitude _eAttitude;
    protected eClass _eClass;
    protected eStance _eStance;
    protected eTypeA _eTypeA;
    protected eTypeC _eTypeC;
    protected float _fPositionX;
    protected float _fPositionY;
    protected float _fPositionZ;
    protected float _fRotation;
    protected float _fSpeed;
    protected Vector3D _hPosition;
    protected MessageHandler _hSkillTime;
    protected StateList _hStateList;
    protected uint _iAttack;
    protected uint _iDP;
    protected ulong _iEntity;
    protected ulong _iEntityExtended;
    protected ulong _iEntityNode;
    protected uint _iHealthCurrent;
    protected uint _iHealthMaximum;
    protected uint _iHide;
    protected uint _iHideView;
    protected uint _iID;
    protected uint _iOwnerID;
    protected uint _iRank;
    protected uint _iSkillDuration;
    protected uint _iSkillID;
    protected uint _iTargetID;
    protected uint _iTypeID;
    protected string _zLegion;
    protected string _zName;
    protected string _zOwnerName;

    public Entity(ulong iEntityNode)
    {
      this._iEntityNode = iEntityNode;
    }

    public ulong GetAddress(int iNodeSwitch = 0)
    {
      if (iNodeSwitch == 1)
        return this._iEntityNode;
      if (iNodeSwitch == 2)
        return this._iEntityExtended;
      return this._iEntity;
    }

    public uint GetAttackSpeed()
    {
      return this._iAttack;
    }

    public eAttitude GetAttitude()
    {
      return this._eAttitude;
    }

    public eClass GetClass()
    {
      return this._eClass;
    }

    public uint GetDP()
    {
      return this._iDP;
    }

    public byte GetHealth()
    {
      return this._bHealth;
    }

    public uint GetHealthCurrent()
    {
      return this._iHealthCurrent;
    }

    public uint GetHealthMaximum()
    {
      return this._iHealthMaximum;
    }

    public uint GetID()
    {
      return this._iID;
    }

    public string GetLegion()
    {
      return this._zLegion;
    }

    public byte GetLevel()
    {
      return this._bLevel;
    }

    public string GetName()
    {
      return this._zName;
    }

    public uint GetOwnerID()
    {
      return this._iOwnerID;
    }

    public string GetOwnerName()
    {
      return this._zOwnerName;
    }

    public Vector3D GetPosition()
    {
      if (this._hPosition == null)
        return new Vector3D(0.0f, 0.0f, 0.0f);
      return (Vector3D) this._hPosition.Clone();
    }

    public uint GetRank()
    {
      return this._iRank;
    }

    public float GetRotation()
    {
      return this._fRotation;
    }

    public uint GetSkillID()
    {
      return this._iSkillID;
    }

    public uint GetSkillTime()
    {
      uint num1 = (uint) Environment.TickCount;
      uint num2 = this._hSkillTime.GetUnsignedInteger(0UL) + this._iSkillDuration;
      if (num2 > num1)
        return num2 - num1;
      return 0;
    }

    public float GetSpeed()
    {
      return this._fSpeed;
    }

    public eStance GetStance()
    {
      return this._eStance;
    }

    public StateList GetState()
    {
      return this._hStateList;
    }

    public uint GetTargetID()
    {
      return this._iTargetID;
    }

    public uint GetTypeID()
    {
      return this._iTypeID;
    }

    public bool IsAsmodian()
    {
      return !this._bElyos;
    }

    public bool IsBusy()
    {
      return this._iSkillID > 0U;
    }

    public bool IsElyos()
    {
      return this._bElyos;
    }

    public bool IsFlying()
    {
      return (long) this._iEntity != 0L && (this._eStance == eStance.Flying || this._eStance == eStance.FlyingCombat);
    }

    public bool IsFriendly()
    {
      return (long) this._iEntity != 0L && !this.IsHostile() && (this._eAttitude == eAttitude.Friendly || this._eTypeC == eTypeC.Friendly || this.IsPlayer() && (int) this._iID == (int) Game.Player.GetID(0UL));
    }

    public bool IsGatherable()
    {
      if ((long) this._iEntity != 0L)
        return this._eTypeA == eTypeA.Gatherable;
      return false;
    }

    public bool IsGliding()
    {
      if ((long) this._iEntity != 0L)
        return this._eStance == eStance.Gliding;
      return false;
    }

    public bool IsHidden()
    {
      if (Game.Player == null)
        return false;
      return Game.Player._iHideView < this._iHide;
    }

    public bool InHide()
    {
      if (Game.Player == null || this._iHide <= 0U)
        return false;
      return this._iHide < 63U;
    }

    public bool IsHostile()
    {
      if ((long) this._iEntity == 0L || (int) Game.Player.GetID(0UL) == (int) this._iID || this.IsPet())
        return false;
      if (this.IsPlayer() && !this._bDuelling)
        return Game.Player.IsAsmodian() != this.IsAsmodian();
      if (this.IsMonster() && !this.IsKisk())
        return this._eAttitude == eAttitude.Hostile;
      return this._eTypeC == eTypeC.Attackable;
    }

    public bool IsKisk()
    {
      if ((long) this._iEntity == 0L || !this._zName.Contains("ника") && !this._zName.Contains("Ника") && !this._zName.Contains("Kisk") || ((int) this._iHealthMaximum != 4000 && (int) this._iHealthMaximum != 15000 && (int) this._iHealthMaximum != 100000 || this._eTypeA != eTypeA.NPC))
        return false;
      return this._eTypeC != eTypeC.Object;
    }

    public bool IsDead()
    {
      if ((long) this._iEntity == 0L || !this.IsPlayer() && !this.IsMonster())
        return false;
      return (int) this._bHealth == 0;
    }

    public bool IsMonster()
    {
      if ((long) this._iEntity != 0L && this._eTypeA == eTypeA.NPC)
        return this._eTypeC != eTypeC.Object;
      return false;
    }

    public bool IsObject()
    {
      if ((long) this._iEntity != 0L)
        return this._eTypeC == eTypeC.Object;
      return false;
    }

    public bool IsPet()
    {
      if ((long) this._iEntity != 0L)
        return this._eTypeA == eTypeA.Pet;
      return false;
    }

    public bool IsPlayer()
    {
      if ((long) this._iEntity != 0L)
        return this._eTypeA == eTypeA.Player;
      return false;
    }

    public bool IsResting()
    {
      if ((long) this._iEntity != 0L)
        return this._eStance == eStance.Resting;
      return false;
    }

    public bool IsValid()
    {
      return this._bValid;
    }

    public void SetAttackSpeed(uint iAttack)
    {
      if ((long) this._iEntity == 0L)
        return;
      Game.Process.SetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["AttackSpeed"].Value, (int) iAttack == 0 ? 1U : iAttack);
      this._iAttack = iAttack;
    }

    public void SetLegion(string zLegion)
    {
      Game.Process.SetString(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Legion"].Value, zLegion, 128U, MessageHandlerString.Unicode);
      this._zLegion = zLegion;
    }

    public void SetName(string setname)
    {
      Game.Process.SetString(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Name"].Value, setname, 64U, MessageHandlerString.Unicode);
      this._zName = setname;
    }

    public void SetExtraSense()
    {
      SoundPlayer soundPlayer;
      if ((int) this._iHideView == 0)
      {
        Game.Process.SetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["HideSeeing"].Value, (uint) byte.MaxValue);
        this._iHideView = (uint) byte.MaxValue;
        Global.Write("AntiHide just enabled!");
        soundPlayer = new SoundPlayer((Stream) ResoX.Activated);
      }
      else if ((int) this._iHideView == (int) byte.MaxValue)
      {
        Game.Process.SetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["HideSeeing"].Value, 0U);
        this._iHideView = 0U;
        Global.Write("AntiHide has been disabled!");
        soundPlayer = new SoundPlayer((Stream) ResoX.Deactivated);
      }
      else
      {
        Global.Write("You already used some AntiHide skill!");
        soundPlayer = new SoundPlayer((Stream) ResoX.Impossible);
      }
      soundPlayer.Play();
    }

    public void SetPosition(Vector3D hPosition)
    {
      if ((long) this._iEntityExtended == 0L || hPosition == null)
        return;
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, hPosition.X);
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, hPosition.Y);
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, hPosition.Z);
    }

    public void SetPosition(float X, float Y, float Z)
    {
      this.SetPosition(new Vector3D(X, Y, Z));
    }

    public void SetX(float paramX)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, paramX);
    }

    public void SetY(float paramY)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, paramY);
    }

    public void SetZ(float paramZ)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, paramZ);
    }

    public void IncreaseX(float incX)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, Game.Player.GetPosition().X + incX);
    }

    public void IncreaseY(float incY)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, Game.Player.GetPosition().Y + incY);
    }

    public void IncreaseZ(float incZ)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, Game.Player.GetPosition().Z + incZ);
    }

    public void DecreaseX(float decX)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, Game.Player.GetPosition().X - decX);
    }

    public void DecreaseY(float decY)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, Game.Player.GetPosition().Y - decY);
    }

    public void DecreaseZ(float decZ)
    {
      Game.Process.SetFloat(this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, Game.Player.GetPosition().Z - decZ);
    }

    public void SetSpeed(float fSpeed)
    {
      if ((long) this._iEntity == 0L)
        return;
      Game.Process.SetFloat(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Speed"].Value, fSpeed);
      this._fSpeed = fSpeed;
    }

    public void SetTypeID(uint iTypeID)
    {
      if ((long) this._iEntity == 0L || this.IsPlayer())
        return;
      Game.Process.SetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["TypeId"].Value, iTypeID);
      this._iTypeID = iTypeID;
    }

    public Entity Update()
    {
      this._iEntity = Game.Process.GetPointer(this._iEntityNode + (ulong) Game.Resolver["ActorCollection"]["PointerActorSingle"].Value);
      if ((long) this._iEntity != 0L)
      {
        byte byte1 = Game.Process.GetByte(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Health"].Value);
        byte byte2 = Game.Process.GetByte(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Level"].Value);
        uint unsignedInteger = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Id"].Value);
        uint num1 = this._iHealthCurrent;
        string @string = Game.Process.GetString(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Name"].Value, 64U, MessageHandlerString.Unicode);
        if ((int) byte1 > 100 || (int) byte2 > 85 || ((int) unsignedInteger == 0 || @string.Length == 0))
        {
          this._bValid = false;
          return this;
        }
        this._bLevel = byte2;
        this._bHealth = byte1;
        this._zName = @string;
        this._bDuelling = (double) Game.Process.GetFloat(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Duelling"].Value) == 0.0;
        this._eTypeA = (eTypeA) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Type"].Value);
        this._eTypeC = this._bDuelling ? eTypeC.Attackable : (eTypeC) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["TypeNpc"].Value);
        this._iTypeID = this.IsPlayer() ? 0U : Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["SkillTimeTotal"].Value);
        this._iEntityExtended = Game.Process.GetPointer(this._iEntityNode + (ulong) Game.Resolver["ActorCollection"]["PointerActorSinglePosition"].Value);
        if (this.IsMonster() || this.IsPlayer())
        {
          this._eAttitude = (eAttitude) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Attitude"].Value);
          this._fSpeed = Game.Process.GetFloat(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Speed"].Value);
          this._hSkillTime = (MessageHandler) Game.Process[this._iEntity + (ulong) Game.Resolver["ActorSingle"]["SkillTimeRemaining"].Value];
          this._hStateList = new StateList(this._iEntity).Update();
          this._iAttack = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["AttackSpeed"].Value);
          this._iHealthCurrent = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["HealthCurrent"].Value);
          this._iHealthMaximum = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["HealthMaximum"].Value);
          this._iSkillID = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["SkillId"].Value);
          this._iSkillDuration = (int) this._iSkillID == 0 ? 0U : Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["SkillTimeTotal"].Value);
          this._iTargetID = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["TargetId"].Value);
          this._iHide = Game.Resolver["ActorSingle"]["Hide"] == null ? 0U : Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Hide"].Value);
          this._iHideView = Game.Resolver["ActorSingle"]["HideSeeing"] == null ? 0U : (uint) Game.Process.GetByte(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["HideSeeing"].Value);
          double num2 = (double) this.UpdateRotation();
        }
        if (this.IsKisk() || this.IsMonster())
        {
          this._iOwnerID = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["OwnerId"].Value);
          this._zOwnerName = Game.Process.GetString(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["OwnerName"].Value, 32U, MessageHandlerString.Unicode);
          if (this._bUpdated && num1 <= 0U && (this._iHealthCurrent >= this._iHealthMaximum && this.IsMonster()))
            this._iHealthCurrent = 0U;
          this._bUpdated = true;
        }
        else if (this.IsPlayer())
        {
          this._eClass = (eClass) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Class"].Value);
          this._eStance = (int) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["StanceAerial"].Value) == 7 ? eStance.Gliding : (eStance) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Stance"].Value);
          this._iDP = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Dp"].Value);
          this._iRank = Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["RankId"].Value);
          this._zLegion = Game.Process.GetString(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Legion"].Value, 64U, MessageHandlerString.Unicode);
          this._eStance = this._eStance == (eStance) 8 || this._eStance == (eStance) 10 ? eStance.Dead : this._eStance;
          if ((int) this._iRank == 0)
          {
            this._bElyos = Game.Player.IsElyos();
          }
          else
          {
            this._bElyos = this._iRank >= 901215U && this._iRank <= 901232U;
            this._iRank = this._iRank >= 901233U ? this._iRank - 901233U : this._iRank - 901215U;
          }
        }
        this.UpdatePosition();
        this._iID = unsignedInteger;
        this._bValid = true;
      }
      return this;
    }

    public Vector3D UpdatePosition()
    {
      if ((long) this._iEntityExtended != 0L)
      {
        ProcessCommunicationPointer communicationPointer = Game.Process[this._iEntityExtended + (ulong) Game.Resolver["ActorPosition"]["Position"].Value].ToBuffered(12UL);
        this._hPosition = new Vector3D(communicationPointer.GetFloat(0UL), communicationPointer.GetFloat(4UL), communicationPointer.GetFloat(8UL));
      }
      return this._hPosition;
    }

    public float UpdateRotation()
    {
      if ((long) this._iEntity == 0L)
        return 0.0f;
      this._fRotation = Game.Process.GetFloat(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Rotation"].Value);
      return this._fRotation;
    }
  }
}
