// Decompiled with JetBrains decompiler
// Type: AionInterface.Force
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using Keiken.Messaging;
using System;

namespace AionInterface
{
  public class Force
  {
    protected byte _bLevel;
    protected bool _bMentor;
    protected eClass _eClass;
    protected Vector3D _hPosition;
    protected uint _iFlightTimeCurrent;
    protected uint _iFlightTimeMaximum;
    protected ulong _iForce;
    protected uint _iForceLeader;
    protected uint _iHealthCurrent;
    protected uint _iHealthMaximum;
    protected uint _iID;
    protected uint _iManaCurrent;
    protected uint _iManaMaximum;
    protected uint _iTeam;
    protected uint _iWorld;
    protected string _zName;

    public Force(ulong iForce, uint iForceLeader)
    {
      this._iForce = iForce;
      this._iForceLeader = iForceLeader;
    }

    public ulong GetAddress()
    {
      return this._iForce;
    }

    public eClass GetClass()
    {
      return this._eClass;
    }

    public Entity GetEntity()
    {
      return Game.EntityList.GetEntity(this._iID);
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

    public byte GetHealth()
    {
      return (byte) Math.Ceiling((double) this._iHealthCurrent / (double) this._iHealthMaximum * 100.0);
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

    public byte GetLevel()
    {
      return this._bLevel;
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

    public string GetName()
    {
      return this._zName;
    }

    public Vector3D GetPosition()
    {
      return (Vector3D) this._hPosition.Clone();
    }

    public uint GetTeam()
    {
      return this._iTeam;
    }

    public uint GetWorld()
    {
      return this._iWorld;
    }

    public bool IsLeader()
    {
      return (int) this._iID == (int) this._iForceLeader;
    }

    public bool IsMentor()
    {
      return this._bMentor;
    }

    public void SetName(string zName)
    {
      Game.Process.SetString(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Name"].Value, zName, 64U, MessageHandlerString.Unicode);
      this._zName = zName;
    }

    public Force Update()
    {
      this.UpdatePosition();
      this._bLevel = Game.Process.GetByte(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Level"].Value);
      this._bMentor = Game.Resolver["GroupSingle"]["IsMentor"] != null && (int) Game.Process.GetByte(this._iForce + (ulong) Game.Resolver["GroupSingle"]["IsMentor"].Value) == 1;
      this._eClass = (eClass) Game.Process.GetByte(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Class"].Value);
      this._iTeam = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["TeamId"].Value);
      this._iID = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Id"].Value);
      this._iHealthMaximum = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["HealthMaximum"].Value);
      this._iHealthCurrent = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["HealthCurrent"].Value);
      this._iManaMaximum = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["ManaMaximum"].Value);
      this._iManaCurrent = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["ManaCurrent"].Value);
      this._iFlightTimeMaximum = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["FlightTimeMaximum"].Value);
      this._iFlightTimeCurrent = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["FlightTimeCurrent"].Value);
      this._iWorld = Game.Process.GetUnsignedInteger(this._iForce + (ulong) Game.Resolver["GroupSingle"]["WorldId"].Value) / 10000U * 10000U;
      this._zName = Game.Process.GetString(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Name"].Value, 64U, MessageHandlerString.Unicode);
      return this;
    }

    public Vector3D UpdatePosition()
    {
      this._hPosition = new Vector3D(Game.Process.GetFloat(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Position"].Value), Game.Process.GetFloat(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Position"].Value + 4UL), Game.Process.GetFloat(this._iForce + (ulong) Game.Resolver["GroupSingle"]["Position"].Value + 8UL));
      return this._hPosition;
    }
  }
}
