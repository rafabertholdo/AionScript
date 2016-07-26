// Decompiled with JetBrains decompiler
// Type: AionInterface.Setting
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace AionInterface
{
  public class Setting
  {
    protected XmlDocument _hDocument;
    protected string _zFile;

    public Setting(string zFile = null)
    {
      if (zFile == null)
        return;
      this.Open(zFile);
    }

    protected XmlNode _CreateNode(string zName, XmlNode hNewRoot = null)
    {
      if (this._hDocument == null || this._hDocument.DocumentElement == null)
        return (XmlNode) null;
      if (hNewRoot == null)
        hNewRoot = (XmlNode) this._hDocument.DocumentElement;
      this._DeleteNode(zName, (XmlNode) null);
      zName = zName.Replace("\\", "/");
      if (zName.Contains("/"))
      {
        XmlNode hNewRoot1 = hNewRoot;
        char[] chArray = new char[1]
        {
          '/'
        };
        foreach (string zName1 in zName.Split(chArray))
        {
          if (hNewRoot1 == null || (hNewRoot1 = this._FindNode(zName1, hNewRoot1)) == null)
          {
            if (zName1.Contains(":"))
            {
              int num = (int) MessageBox.Show(zName);
            }
            XmlElement element = this._hDocument.CreateElement("", zName1.Replace(" ", ""), "");
            hNewRoot.AppendChild((XmlNode) element);
            hNewRoot = (XmlNode) element;
          }
          else
            hNewRoot = hNewRoot1;
        }
        return hNewRoot;
      }
      XmlElement element1 = this._hDocument.CreateElement("", zName, "");
      hNewRoot.AppendChild((XmlNode) element1);
      return (XmlNode) element1;
    }

    protected bool _DeleteNode(string zName, XmlNode hNewRoot = null)
    {
      if (this._hDocument != null && this._hDocument.DocumentElement != null)
      {
        XmlNode node = this._FindNode(zName, hNewRoot);
        if (node != null && node.ParentNode != null)
        {
          node.ParentNode.RemoveChild(node);
          return true;
        }
      }
      return false;
    }

    protected XmlNode _FindNode(string zName, XmlNode hNewRoot = null)
    {
      if (this._hDocument != null && this._hDocument.DocumentElement != null)
      {
        if (hNewRoot == null)
          hNewRoot = (XmlNode) this._hDocument.DocumentElement;
        zName = zName.Replace("\\", "/");
        if (zName.Contains("/"))
        {
          char[] chArray = new char[1]
          {
            '/'
          };
          foreach (string zName1 in zName.Split(chArray))
          {
            if ((hNewRoot = this._FindNode(zName1, hNewRoot)) == null)
              return (XmlNode) null;
          }
          return hNewRoot;
        }
        for (int index = 0; index < hNewRoot.ChildNodes.Count; ++index)
        {
          if (hNewRoot.ChildNodes[index].Name == zName)
            return hNewRoot.ChildNodes[index];
        }
      }
      return (XmlNode) null;
    }

    public Dictionary<string, string> GetDictionary(string zName)
    {
      if (this._hDocument == null || this._hDocument.DocumentElement == null)
        return (Dictionary<string, string>) null;
      XmlNode node = this._FindNode(zName, (XmlNode) null);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (node == null)
        return (Dictionary<string, string>) null;
      for (int index = 0; index < node.ChildNodes.Count; ++index)
      {
        XmlNode xmlNode = node.ChildNodes[index];
        dictionary[xmlNode.Name] = xmlNode.InnerText;
      }
      if (dictionary.Count <= 0)
        return (Dictionary<string, string>) null;
      return dictionary;
    }

    public XmlNode GetNode(string zName, XmlNode hXmlNode = null)
    {
      XmlNode xmlNode = hXmlNode == null ? (XmlNode) this._hDocument.DocumentElement : hXmlNode;
      for (int index = 0; index < xmlNode.ChildNodes.Count; ++index)
      {
        if (xmlNode.ChildNodes[index].Name == zName)
          return xmlNode.ChildNodes[index];
      }
      return (XmlNode) null;
    }

    public string GetValue(string zName, string zKey)
    {
      if (this._hDocument != null && this._hDocument.DocumentElement != null)
      {
        if (zName == null)
        {
          XmlNode node = this._FindNode(zKey, (XmlNode) null);
          if (node != null)
            return node.InnerText;
          return (string) null;
        }
        XmlNode node1 = this._FindNode(zName, (XmlNode) null);
        if (node1 != null)
        {
          for (int index = 0; index < node1.ChildNodes.Count; ++index)
          {
            if (node1.ChildNodes[index].Name == zKey)
              return node1.ChildNodes[index].InnerText;
          }
        }
      }
      return (string) null;
    }

    public bool Open(string zFile)
    {
      if (this._hDocument != null && this._hDocument.DocumentElement != null)
        return false;
      try
      {
        this._zFile = zFile;
        XmlTextReader xmlTextReader = new XmlTextReader(zFile);
        xmlTextReader.Read();
        this._hDocument = new XmlDocument();
        this._hDocument.Load((XmlReader) xmlTextReader);
        return true;
      }
      catch (Exception ex)
      {
        this._hDocument = new XmlDocument();
        this._hDocument.AppendChild((XmlNode) this._hDocument.CreateElement("", "Setting", ""));
        return false;
      }
    }

    public bool Save()
    {
      if (this._hDocument != null)
      {
        if (this._hDocument.DocumentElement != null)
        {
          try
          {
            if (this._zFile == null)
              return false;
            this._hDocument.Save(this._zFile);
            return true;
          }
          catch (Exception ex)
          {
            return false;
          }
        }
      }
      return false;
    }

    public bool SetDictionary(string zName, Dictionary<string, string> hValues)
    {
      if (this._hDocument == null || this._hDocument.DocumentElement == null)
        return false;
      XmlNode node = this._CreateNode(zName, (XmlNode) null);
      foreach (KeyValuePair<string, string> keyValuePair in hValues)
      {
        try
        {
          XmlNode newChild = (XmlNode) this._hDocument.CreateElement(keyValuePair.Key);
          newChild.InnerText = keyValuePair.Value;
          node.AppendChild(newChild);
        }
        catch (Exception ex)
        {
          return false;
        }
      }
      return true;
    }

    public bool SetValue(string zName, string zKey, string zValue)
    {
      if (this._hDocument == null || this._hDocument.DocumentElement == null)
        return false;
      XmlNode node = this._FindNode(zName, (XmlNode) null);
      if (node == null)
        node = this._CreateNode(zName, (XmlNode) null);
      foreach (XmlNode xmlNode in node.ChildNodes)
      {
        if (xmlNode.Name == zKey)
        {
          xmlNode.InnerText = zValue;
          return true;
        }
      }
      try
      {
        if (zKey == null)
        {
          node.InnerText = zValue;
          return true;
        }
        XmlNode newChild = (XmlNode) this._hDocument.CreateElement(zKey);
        newChild.InnerText = zValue;
        node.AppendChild(newChild);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
