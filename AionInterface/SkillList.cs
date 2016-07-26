// Decompiled with JetBrains decompiler
// Type: AionInterface.SkillList
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace AionInterface
{
  public class SkillList
  {
    protected Dictionary<Skill, XmlElement> _hSkillListElement = new Dictionary<Skill, XmlElement>();
    protected Dictionary<uint, Skill> _hSkillListID = new Dictionary<uint, Skill>();
    protected Dictionary<string, Skill> _hSkillListName = new Dictionary<string, Skill>();
    protected XmlDocument _hSkillDocument;
    private List<string> _levels;
    private Dictionary<string, bool> _map;
    protected string _zFile;

    public Skill this[uint iID]
    {
      get
      {
        return this.GetSkill(iID);
      }
    }

    public Skill this[string zName]
    {
      get
      {
        return this.GetSkill(zName);
      }
    }

    public SkillList(string zFile)
    {
      this._levels = new List<string>()
      {
        "I",
        "II",
        "III",
        "IV",
        "V",
        "VI",
        "VII",
        "VIII",
        "IX",
        "X"
      };
      this._zFile = zFile;
      this._LoadSkill();
    }

    protected string _GetElement(XmlElement hElement, string zKey)
    {
      XmlNodeList elementsByTagName = hElement.GetElementsByTagName(zKey);
      if (elementsByTagName.Count == 1)
        return elementsByTagName[0].InnerText;
      return (string) null;
    }

    public SkillList _LoadSkill()
    {
      Dictionary<uint, Skill> dictionary = this._hSkillListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        this._hSkillListID.Clear();
        try
        {
          XmlTextReader xmlTextReader = new XmlTextReader(this._zFile);
          this._hSkillDocument = new XmlDocument();
          this._hSkillDocument.Load((XmlReader) xmlTextReader);
          XmlElement documentElement = this._hSkillDocument.DocumentElement;
          for (uint index1 = 0; (long) index1 < (long) documentElement.ChildNodes.Count; ++index1)
          {
            XmlElement hElement = (XmlElement) documentElement.ChildNodes[(int) index1];
            uint iID = uint.Parse(this._GetElement(hElement, "iID"));
            string element = this._GetElement(hElement, "zName");
            Skill index2 = new Skill(iID, element);
            this._hSkillListID[iID] = index2;
            this._hSkillListName[element] = index2;
            this._hSkillListElement[index2] = hElement;
          }
          xmlTextReader.Close();
          foreach (KeyValuePair<string, Skill> keyValuePair in this._hSkillListName)
          {
            if (!new Regex("\\s(I|II|III|IV|V|VI|VII|VIII|IX|X|XI|XII|XIII)$").IsMatch(keyValuePair.Key))
            {
              Skill skill = this.GetSkill(keyValuePair.Key + " I");
              if (skill != null)
                keyValuePair.Value.SetActivate(skill.GetActivated());
            }
          }
          return this;
        }
        catch (Exception ex)
        {
          return this;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    protected Skill _ValidateSkill(Skill hSkill)
    {
      if (!hSkill.IsValid())
      {
        XmlElement hElement = this._hSkillListElement[hSkill];
        eDispell eDispell = (eDispell) int.Parse(this._GetElement(hElement, "iDispell"));
        eSkillType eType = (eSkillType) int.Parse(this._GetElement(hElement, "iType"));
        eSkillTypeSecondary eTypeSecondary = (eSkillTypeSecondary) int.Parse(this._GetElement(hElement, "iTypeSecondary"));
        bool bAcivated = bool.Parse(this._GetElement(hElement, "bActivated"));
        hSkill.Update(eDispell, eType, eTypeSecondary, bAcivated);
      }
      return hSkill;
    }

    internal bool GetActivated(string name)
    {
      if (this._map == null)
      {
        this._map = new Dictionary<string, bool>();
        foreach (Skill hSkill in Enumerable.Select<KeyValuePair<Skill, XmlElement>, Skill>((IEnumerable<KeyValuePair<Skill, XmlElement>>) this._hSkillListElement, (Func<KeyValuePair<Skill, XmlElement>, Skill>) (x => x.Key)))
        {
          this._ValidateSkill(hSkill);
          string key = this.ToBaseName(hSkill.GetName());
          this._map[key] = this._map.ContainsKey(key) ? this._map[key] || hSkill.GetActivated() : hSkill.GetActivated();
        }
      }
      string key1 = this.ToBaseName(name);
      if (!this._map.ContainsKey(key1))
        return false;
      return this._map[key1];
    }

    public Dictionary<uint, Skill> GetList()
    {
      Dictionary<uint, Skill> dictionary = this._hSkillListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return this._hSkillListID;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    public Skill GetSkill(string zName)
    {
      if (zName != null)
      {
        uint result;
        if (uint.TryParse(zName, out result))
          return this.GetSkill(result);
        Dictionary<uint, Skill> dictionary = this._hSkillListID;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object) dictionary, ref lockTaken);
          if (this._hSkillListName.ContainsKey(zName))
            return this._ValidateSkill(this._hSkillListName[zName]);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) dictionary);
        }
      }
      return (Skill) null;
    }

    public Skill GetSkill(uint iID)
    {
      Dictionary<uint, Skill> dictionary = this._hSkillListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (this._hSkillListID.ContainsKey(iID))
          return this._ValidateSkill(this._hSkillListID[iID]);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Skill) null;
    }

    public Skill GetSkillIndex(uint iIndex)
    {
      Dictionary<uint, Skill> dictionary = this._hSkillListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        if (iIndex >= 0U)
        {
          if ((long) iIndex < (long) this._hSkillListID.Count)
            return this._ValidateSkill(Enumerable.ElementAt<KeyValuePair<uint, Skill>>((IEnumerable<KeyValuePair<uint, Skill>>) this._hSkillListID, (int) iIndex).Value);
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return (Skill) null;
    }

    public uint GetSkillSize()
    {
      Dictionary<uint, Skill> dictionary = this._hSkillListID;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) dictionary, ref lockTaken);
        return (uint) this._hSkillListID.Count;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
    }

    private string ToBaseName(string name)
    {
      foreach (string str in this._levels)
      {
        if (name.EndsWith(str))
          return name.Substring(0, name.Length - str.Length - 1);
      }
      return name;
    }
  }
}
