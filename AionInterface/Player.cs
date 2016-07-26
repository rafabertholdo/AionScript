// Decompiled with JetBrains decompiler
// Type: AionInterface.Player
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken;
using Keiken.Messaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AionInterface
{
  public class Player : Entity
  {
    protected bool _bGravityLock;
    protected bool _bMoving;
    protected eAction _eAction;
    protected eAction _eActionPending;
    protected float _fAttackRange;
    protected Dictionary<uint, uint> _hBrand;
    protected Vector3D _hCamera;
    protected Vector3D _hMove;
    private Vector3D _hMoveCheck;
    protected Vector3D _hMoveClick;
    protected Vector3D _hMoveInitial;
    protected uint _iActionPending;
    protected ulong _iBase;
    protected uint _iExperienceCurrent;
    protected uint _iExperienceRecoverable;
    protected uint _iExperienceRequired;
    protected uint _iFlightCooldown;
    protected uint _iFlightTimeCurrent;
    protected uint _iFlightTimeMaximum;
    protected uint _iGravityPatchTimer;
    protected uint _iManaCurrent;
    protected uint _iManaMaximum;
    private uint _iMoveTime;
    protected int _iMovingAir;
    protected uint _iWorld;
    private uint _lastSkillTime;
    protected string _zMarked;

    public Player(ulong iBase)
      : base(iBase)
    {
      this._hBrand = new Dictionary<uint, uint>();
      this._iMovingAir = -1;
      this._iBase = iBase;
    }

    public ulong GetBaseAddress()
    {
      return this._iBase;
    }

    public float GetAttackRange()
    {
      return this._fAttackRange;
    }

    public uint GetBrand(uint iBrand)
    {
      if (this._hBrand.ContainsKey(iBrand))
        return this._hBrand[iBrand];
      return 0;
    }

    public Vector3D GetCamera()
    {
      return (Vector3D) this._hCamera.Clone();
    }

    public uint GetExperienceCurrent()
    {
      return this._iExperienceCurrent;
    }

    public uint GetExperienceRecoverable()
    {
      return this._iExperienceRecoverable;
    }

    public uint GetExperienceRequired()
    {
      return this._iExperienceRequired;
    }

    public uint GetFlightCooldown()
    {
      return this._iFlightCooldown;
    }

    public byte GetFlightTime()
    {
      return (byte) Math.Ceiling((double) this._iFlightTimeCurrent / (double) this._iFlightTimeMaximum * 100.0);
    }

    public uint GetFlightTimeCurrent()
    {
      return this._iFlightTimeCurrent;
    }

    public uint GetFlightTimeMaximum()
    {
      return this._iFlightTimeMaximum;
    }

    public uint GetID(ulong iBase = 0)
    {
      if (iBase > 0UL && ((int) (this._iID = Game.Process.GetUnsignedInteger(iBase + (ulong) Game.Resolver["ControllerSingle"]["Id"].Value)) == 248582069 || (int) this._iID == -842150451 || (int) this._iID == -1009696487))
        this._iID = 0U;
      return this._iID;
    }

    public byte GetMana()
    {
      return (byte) Math.Ceiling((double) this._iManaCurrent / (double) this._iManaMaximum * 100.0);
    }

    public uint GetManaCurrent()
    {
      return this._iManaCurrent;
    }

    public uint GetManaMaximum()
    {
      return this._iManaMaximum;
    }

    public string GetMarked()
    {
      return this._zMarked;
    }

    public Vector3D GetMove()
    {
      return this._hMove;
    }

    public string GetName(ulong iBase = 0)
    {
      if (iBase > 0UL)
        this._zName = Game.Process.GetString(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["Name"].Value, 64U, MessageHandlerString.Unicode);
      return this._zName;
    }

    public Vector3D GetPositionMove()
    {
      return (Vector3D) this._hMoveClick.Clone();
    }

    public uint GetWorld()
    {
      return this._iWorld;
    }

    public bool IsMoving()
    {
      if (!this._bMoving)
        return this._hMove != null;
      return true;
    }

    [DllImport("user32.dll")]
    private static extern bool PostMessage(uint hWnd, uint msg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    protected static extern bool SendMessage(uint hWnd, uint msg, uint wParam, uint lParam);

    public bool SetAction(eAction eNewAction)
    {
      if (this._eAction != eNewAction)
      {
        if (this._eAction != eAction.None && eNewAction != eAction.None)
        {
          Game.Process.SetByte(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Action"].Value, (byte) 0);
          this._eActionPending = eNewAction;
          this._iActionPending = Game.Time() + 500U;
          return true;
        }
        Game.Process.SetByte(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Action"].Value, (byte) eNewAction);
        this._eAction = eNewAction;
      }
      return true;
    }

    public bool SetAction(string zAction)
    {
      try
      {
        return this.SetAction((eAction) Enum.Parse(typeof (eAction), zAction, true));
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public void SetAttackRange(float fAttackRange)
    {
      Game.Process.SetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["AttackRange"].Value, fAttackRange);
      this._fAttackRange = fAttackRange;
    }

    public void SetCamera(Vector3D hPosition, bool bMoveBoth = true)
    {
      Vector3D vector3D = this._hPosition.CalculateCamera(hPosition);
      if (bMoveBoth)
        Game.Process.SetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["CameraPitch"].Value, vector3D.Y);
      Game.Process.SetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["CameraYaw"].Value, vector3D.X);
    }

    public bool SetMove(Vector3D hPosition, int iMovingAir = -1)
    {
      if (this.IsBusy() || Game.DialogList.GetDialog("skill_delay_dialog", true).IsVisible() || Game.DialogList.GetDialog("enchant_delay_dialog", true).IsVisible())
        return false;
      if (hPosition == null)
      {
        this.SetAction(eAction.None);
        this._hMove = (Vector3D) null;
        this._hMoveInitial = (Vector3D) null;
        this._bMoving = false;
      }
      else
      {
        if ((double) hPosition.X >= (double) this._hPosition.X - 2.0 && (double) hPosition.X <= (double) this._hPosition.X + 2.0 && ((double) hPosition.Y >= (double) this._hPosition.Y - 2.0 && (double) hPosition.Y <= (double) this._hPosition.Y + 2.0) && ((double) hPosition.Z >= (double) this._hPosition.Z - 2.0 && (double) hPosition.Z <= (double) this._hPosition.Z + 2.0))
          return true;
        this._hMove = (Vector3D) hPosition.Clone();
        this._hMoveInitial = (Vector3D) this._hPosition.Clone();
        this._bMoving = true;
        this._iMovingAir = iMovingAir;
        this._hMoveCheck = (Vector3D) this._hPosition.Clone();
        if (this._iMoveTime < Game.Time())
          this._iMoveTime = Game.Time() + 1000U;
        if (!this.UpdateMove())
        {
          this.SetMove((Vector3D) null, -1);
          return false;
        }
      }
      return true;
    }

    public bool SetMove(float X, float Y, float Z, int iMovingAir = -1)
    {
      return this.SetMove(new Vector3D(X, Y, Z), iMovingAir);
    }

    public void SetTarget(Entity hTarget)
    {
      if ((long) this._iEntity == 0L || hTarget == null)
        return;
      long num1 = (long) hTarget.GetAddress(0);
      ulong address = hTarget.GetAddress(2);
      Vector3D position = hTarget.GetPosition();
      double num2 = position.DistanceToPosition(this._hPosition, 0.0);
      HashSet<ulong> hashSet = new HashSet<ulong>();
      if (!hTarget.IsPlayer())
      {
        foreach (KeyValuePair<uint, Entity> keyValuePair in Game.EntityList.GetList())
        {
          Entity entity = keyValuePair.Value;
          if (entity.GetName() == hTarget.GetName() && (int) entity.GetID() != (int) hTarget.GetID())
          {
            Game.Process.SetString(entity.GetAddress(0) + (ulong) Game.Resolver["ActorSingle"]["Name"].Value, "", 64U, MessageHandlerString.Unicode);
            hashSet.Add(entity.GetAddress(0));
          }
        }
      }
      if (num2 > 30.0)
      {
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, this._hPosition.X);
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, this._hPosition.Y);
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, this._hPosition.Z);
      }
      Game.PlayerInput.Console("/Select " + hTarget.GetName());
      if (num2 > 30.0)
      {
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value, position.X);
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 4UL, position.Y);
        Game.Process.SetFloat(address + (ulong) Game.Resolver["ActorPosition"]["Position"].Value + 8UL, position.Z);
      }
      foreach (uint num3 in hashSet)
        Game.Process.SetString((ulong) (num3 + Game.Resolver["ActorSingle"]["Name"].Value), hTarget.GetName(), 64U, MessageHandlerString.Unicode);
    }

    public Player Update()
    {
      Dialog dialog = Game.DialogList == null ? (Dialog) null : Game.DialogList.GetDialog("target_dialog", true);
      ProcessCommunicationPointer communicationPointer = Game.Process[this._iBase + (ulong) Game.Resolver["ControllerSingle"]["Brand"].Value].ToBuffered(56UL);
      this.UpdateCamera();
      this.UpdatePosition();
      this._iID = this.GetID(this._iBase);
      if (this._zName == null)
        this._zName = this.GetName(this._iBase);
      if ((int) this._bLevel == 0)
        this._bLevel = Game.Process.GetByte(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["Level"].Value);
      Entity entity;
      if (Game.EntityList != null && (entity = Game.EntityList.GetEntity(this._iID)) != null)
      {
        this._iEntityNode = entity.GetAddress(1);
        base.Update();
        Game.DialogList.GetDialog("skill_delay_dialog", false);
        if (this.IsMoving() && this._iMoveTime < Game.Time())
        {
          if (this._hMoveCheck.Equals((object) this._hPosition))
          {
            this._eActionPending = this._eAction;
            this.SetAction(eAction.None);
            this._iActionPending = Game.Time() + 500U;
          }
          this._hMoveCheck = this._hPosition;
          this._iMoveTime = Game.Time() + 1000U;
        }
        this._eAction = (eAction) Game.Process.GetUnsignedInteger(this._iEntity + (ulong) Game.Resolver["ActorSingle"]["Action"].Value);
        if ((int) this._iActionPending != 0 && this._iActionPending < Game.Time())
        {
          if ((this._eAction == eAction.MoveForward && this._eActionPending == eAction.MoveForward || this._eAction == eAction.None) && this._eActionPending == eAction.MoveForward)
          {
            Player.PostMessage(Game.Process.ProcessWindowHandle, 256U, 144U, 0U);
            Player.PostMessage(Game.Process.ProcessWindowHandle, 257U, 144U, 0U);
          }
          else
            this.SetAction(this._eActionPending);
          this._iActionPending = 0U;
        }
      }
      try
      {
        uint num = Game.Resolver["ControllerSingleCheat"]["GravitySkills"].Value;
        if (Game.Resolver["ControllerSingleCheat"]["GravityFlying"] != null)
        {
          if (Game.Resolver["ControllerSingleCheat"]["GravitySkills"] != null)
          {
            if (this._iGravityPatchTimer >= Game.Time())
            {
              if (!this._bGravityLock)
              {
                if (Game.Player.GetStance() != eStance.Normal)
                {
                  if (Game.Player.GetStance() != eStance.Combat)
                    goto label_29;
                }
                Game.Process.SetUnsignedInteger(Game.Player.GetAddress(0) + (ulong) Game.Resolver["ActorSingle"]["Stance"].Value, Game.Player.GetStance() == eStance.Combat ? 5U : 4U);
                Game.Process.SetByte(Game.Base() + (ulong) Game.Resolver["ControllerSingleCheat"]["GravityFlying"].Value, (byte) 0);
                if ((int) num != 0)
                  Game.Process.SetUnsignedShort(Game.Base() + (ulong) num, (ushort) 37008);
                this._bGravityLock = true;
              }
            }
            else if (this._bGravityLock)
            {
              Game.Process.SetUnsignedInteger(Game.Player.GetAddress(0) + (ulong) Game.Resolver["ActorSingle"]["Stance"].Value, Game.Player.GetStance() == eStance.FlyingCombat ? 1U : 0U);
              Game.Process.SetByte(Game.Base() + (ulong) Game.Resolver["ControllerSingleCheat"]["GravityFlying"].Value, (byte) 1);
              if ((int) num != 0)
                Game.Process.SetUnsignedShort(Game.Base() + (ulong) num, (ushort) 49203);
              this._bGravityLock = false;
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
label_29:
      this._fAttackRange = Game.Process.GetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["AttackRange"].Value);
      this._hMoveClick = new Vector3D(0.0f, 0.0f, 0.0f);
      this._iManaMaximum = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["ManaMaximum"].Value);
      this._iManaCurrent = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["ManaCurrent"].Value);
      this._iFlightTimeCurrent = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["FlightTimeCurrent"].Value);
      this._iFlightTimeMaximum = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["FlightTimeMaximum"].Value);
      this._iExperienceCurrent = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["ExperienceCurrent"].Value);
      this._iExperienceRecoverable = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["ExperienceRecoverable"].Value);
      this._iExperienceRequired = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["ExperienceMaximum"].Value);
      this._iFlightCooldown = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["FlightCooldownRemainingTime"].Value);
      this._iWorld = Game.Process.GetUnsignedInteger(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["WorldId"].Value) / 10000U * 10000U;
      this._zMarked = Game.Process.GetString(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["NameSearch"].Value, 128U, MessageHandlerString.Unicode);
      for (uint index = 0; index < 14U; ++index)
        this._hBrand[index + 1U] = communicationPointer.GetUnsignedInteger((ulong) (index * 4U));
      if (dialog != null && dialog.IsVisible())
      {
        ulong address = dialog.GetAddress();
        this._iTargetID = Game.Process.GetUnsignedInteger(address + (ulong) Game.Resolver["ControllerSingle"]["TargetOffset"].Value);
      }
      return this;
    }

    public Vector3D UpdateCamera()
    {
      this._hCamera = new Vector3D(Game.Process.GetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["CameraYaw"].Value), Game.Process.GetFloat(this._iBase + (ulong) Game.Resolver["ControllerSingle"]["CameraPitch"].Value), 0.0f);
      return this._hCamera;
    }

    public void UpdateGravityLock(uint iTime)
    {
      this._iGravityPatchTimer = Game.Time() + iTime;
      this.Update();
    }

    public bool UpdateMove()
    {
      if (this.IsMoving())
      {
        float num1 = this._hMove.Y - this._hMoveInitial.Y;
        float num2 = this._hMove.Z - this._hMoveInitial.Z;
        double num3 = (double) this._hMove.X - (double) this._hMoveInitial.X;
        float num4 = (float) Math.Sqrt(num3 * num3 + (double) num1 * (double) num1 + (this.IsFlying() ? (double) num2 * (double) num2 : 0.0));
        double num5 = (double) num4;
        Vector3D vector3D1 = new Vector3D((float) (num3 / num5), num1 / num4, num2 / num4);
        Vector3D vector3D2 = new Vector3D(this._hMove.X - this._hPosition.X, this._hMove.Y - this._hPosition.Y, this._hMove.Z - this._hPosition.Z);
        float f = (float) ((double) vector3D1.X * (double) vector3D2.X + (double) vector3D1.Y * (double) vector3D2.Y + (this.IsFlying() ? (double) vector3D1.Z * (double) vector3D2.Z : 0.0));
        if ((double) f < 0.0 || float.IsNaN(f))
        {
          if (this._eAction == eAction.MoveForward)
            this.SetAction(eAction.None);
          this._hMove = (Vector3D) null;
          this._bMoving = false;
          return false;
        }
        if (this._eAction == eAction.None || this._eAction == eAction.Talking)
          this.SetAction(eAction.MoveForward);
        if ((int) this.GetFlightCooldown() == 0 && this._iMovingAir != -1 && (this.IsFlying() && this._iMovingAir == 0 || !this.IsFlying() && this._iMovingAir == 1))
          Game.PlayerInput.Ability("Toggle Flight/Landing");
        Game.Player.SetCamera(this._hMove, this.IsFlying());
        this._bMoving = false;
      }
      return true;
    }
  }
}
