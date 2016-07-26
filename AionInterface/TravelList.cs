// Decompiled with JetBrains decompiler
// Type: AionInterface.TravelList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;

namespace AionInterface
{
  public class TravelList
  {
    protected List<Travel> _hTravelList = new List<Travel>();
    protected bool _bBackward;
    protected bool _bReverse;
    protected Travel _hPrevious;
    protected XmlDocument _hTravelDocument;
    protected int _iCurrent;

    public TravelList(string zFile = null)
    {
      if (zFile == null)
        return;
      this._Open(zFile);
    }

    protected bool _Close()
    {
      List<Travel> list = this._hTravelList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        if (this._hTravelList.Count <= 0)
          return false;
        this._hTravelList.Clear();
        this._bReverse = false;
        this._iCurrent = 0;
        this._hPrevious = (Travel) null;
        return true;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    protected string _GetElement(XmlElement hElement, string zKey)
    {
      XmlNodeList elementsByTagName = hElement.GetElementsByTagName(zKey);
      if (elementsByTagName.Count == 1)
        return elementsByTagName[0].InnerText;
      return (string) null;
    }

    protected bool _Open(string zFile)
    {
      List<Travel> list = this._hTravelList;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) list, ref lockTaken);
        this._hTravelList.Clear();
        try
        {
          XmlTextReader xmlTextReader = new XmlTextReader(zFile);
          this._hTravelDocument = new XmlDocument();
          this._hTravelDocument.Load((XmlReader) xmlTextReader);
          XmlElement documentElement = this._hTravelDocument.DocumentElement;
          this._bReverse = documentElement.HasAttribute("Reverse") && documentElement.GetAttribute("Reverse").Equals("True", StringComparison.InvariantCultureIgnoreCase);
          this._iCurrent = 0;
          this._hPrevious = (Travel) null;
          for (int index = 0; index < documentElement.ChildNodes.Count; ++index)
          {
            XmlElement hElement = (XmlElement) documentElement.ChildNodes[index];
            this._hTravelList.Add(new Travel(this._GetElement(hElement, "Name"), new Vector3D(float.Parse(this._GetElement(hElement, "X"), (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(this._GetElement(hElement, "Y"), (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(this._GetElement(hElement, "Z"), (IFormatProvider) CultureInfo.InvariantCulture)), bool.Parse(this._GetElement(hElement, "Flying")), this._GetElement(hElement, "Type"), this._GetElement(hElement, "Param")));
          }
          xmlTextReader.Close();
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) list);
      }
    }

    public Travel GetCurrent()
    {
      if (this._hTravelList.Count == 0)
        return (Travel) null;
      return this._hTravelList[this._iCurrent];
    }

    public List<Travel> GetList()
    {
      return this._hTravelList;
    }

    public Travel GetNext()
    {
      if (this._hTravelList.Count == 0)
        return (Travel) null;
      this._hPrevious = this._hTravelList[this._iCurrent];
      if (this._bBackward)
      {
        if (this._iCurrent == 1)
          this._bBackward = false;
        this._iCurrent = this._iCurrent - 1;
      }
      else if (this._iCurrent == this._hTravelList.Count - 1)
      {
        if (this._bReverse)
        {
          this._iCurrent = this._iCurrent - 1;
          this._bBackward = true;
        }
        else
          this._iCurrent = 0;
      }
      else
        this._iCurrent = this._iCurrent + 1;
      if (this._iCurrent == -1)
        this._iCurrent = 0;
      return this._hTravelList[this._iCurrent];
    }

    public Travel GetPrevious()
    {
      return this._hPrevious;
    }

    public bool IsReverse()
    {
      return this._bReverse;
    }

    public bool Modify(Travel hTravel, int iIndex = -1)
    {
      if (iIndex == -1)
      {
        this._hTravelList.Add(hTravel);
        return true;
      }
      if (iIndex < 0 || iIndex >= this._hTravelList.Count)
        return false;
      if (hTravel == null)
        this._hTravelList.RemoveAt(iIndex);
      else
        this._hTravelList.Insert(iIndex, hTravel);
      return true;
    }

    public bool Move()
    {
      int iMovingAir = -1;
      if (Game.PlayerInput == null || Game.Player == null || Game.Player.IsBusy())
        return false;
      if (Game.Player.IsFlying() && !this._hTravelList[this._iCurrent].IsFlying() && (this.GetPrevious() != null && !this.GetPrevious().IsFlying()))
        iMovingAir = 0;
      if (!Game.Player.IsFlying() && this._hTravelList[this._iCurrent].IsFlying())
        iMovingAir = 1;
      Game.Player.SetMove(this._hTravelList[this._iCurrent].GetPosition(), iMovingAir);
      return true;
    }
  }
}
