// Decompiled with JetBrains decompiler
// Type: AionScript.IScripting
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AionScript
{
  public class IScripting : Form
  {
    private bool _bChanged;
    private bool _bClosing;
    private bool _bLoaded;
    private string _zExtension;
    private string _zFile;
    private IContainer components;
    private Scintilla _hEditor;

    public override sealed string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
      }
    }

    public IScripting(string zFile)
    {
      try
      {
        this.InitializeComponent();
        this.Text = Program.CurrentName;
        this._zExtension = zFile.Substring(zFile.LastIndexOf(".", StringComparison.Ordinal) + 1);
        this._hEditor.ConfigurationManager.Language = this._zExtension == "vb" ? "vbscript" : this._zExtension;
        StreamReader streamReader = new StreamReader(zFile);
        this._hEditor.Text = streamReader.ReadToEnd();
        streamReader.Close();
        this.Text = this.Text + " (" + zFile.Substring(zFile.LastIndexOf("\\", StringComparison.Ordinal) + 1) + ")";
        this._zFile = zFile;
      }
      catch (Exception ex)
      {
        int num = 0U > 0U ? 1 : 0;
        Program.Exception(ex, num != 0);
      }
    }

    public new void Close()
    {
      this._bClosing = true;
      base.Close();
    }

    private void Editor_DocumentChange(object sender, NativeScintillaEventArgs e)
    {
      if (!this._bLoaded || this.Text.EndsWith("*"))
        return;
      this.Text = this.Text + " *";
      this._bChanged = true;
    }

    private void Editor_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
        this.Save();
      this._bLoaded = true;
    }

    private void Editor_FormClosing(object sender, CancelEventArgs e)
    {
      try
      {
        if (this._bChanged)
        {
          switch (MessageBox.Show("Do you wish to save the changes you made?", this._zFile.Substring(this._zFile.IndexOf("\\", StringComparison.Ordinal) + 1), this._bClosing ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk))
          {
            case DialogResult.Cancel:
              e.Cancel = true;
              return;
            case DialogResult.Yes:
              if (!this.Save())
              {
                e.Cancel = true;
                return;
              }
              break;
          }
          this._bChanged = false;
        }
        Program.Manager.ScriptClose(this._zFile);
      }
      catch (Exception ex)
      {
        int num = 0U > 0U ? 1 : 0;
        Program.Exception(ex, num != 0);
      }
    }

    private void Editor_Load(object sender, EventArgs e)
    {
      List<object> list = new List<object>();
      list.Add((object) new Ability(0UL, false));
      list.Add((object) new AbilityList(0UL));
      list.Add((object) new Dialog(0UL, 0UL, 0U, (Dialog) null));
      list.Add((object) new DialogList(0UL));
      list.Add((object) new Entity(0UL));
      list.Add((object) new EntityList(0UL));
      list.Add((object) new StateList(0UL));
      list.Add((object) new Inventory(0UL));
      list.Add((object) new InventoryList(0UL));
      list.Add((object) new Force(0UL, 0U));
      list.Add((object) new ForceList(0UL));
      list.Add((object) new Player(0UL));
      list.Add((object) new PlayerInput(0UL));
      list.Add((object) new Vector2D(0U, 0U));
      list.Add((object) new Vector3D(0.0f, 0.0f, 0.0f));
      list.Add((object) new Skill(0U, (string) null));
      list.Add((object) new SkillList((string) null));
      list.Add((object) new Travel((string) null, (Vector3D) null, false, (string) null, (string) null));
      list.Add((object) new TravelList((string) null));
      this._hEditor.AutoComplete.FillUpCharacters = " (";
      foreach (object obj in list)
      {
        Type type = obj.GetType();
        string str = (string) null;
        foreach (MethodInfo methodInfo in Enumerable.Where<MethodInfo>((IEnumerable<MethodInfo>) type.GetMethods(), (Func<MethodInfo, bool>) (methodInfo =>
        {
          if (!methodInfo.IsPrivate && !methodInfo.IsVirtual && (methodInfo.Name != "GetType" && !methodInfo.Name.StartsWith("Update")) && (!(type == typeof (PlayerInput)) || methodInfo.Name != "Register" && methodInfo.Name != "RegisterThread" && methodInfo.Name != "Unregister"))
            return methodInfo.Name != "get_item";
          return false;
        })))
        {
          foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            str = str == null ? parameterInfo.Name : str + "," + parameterInfo.Name;
          this._hEditor.AutoComplete.List.Add((this._zExtension == "lua" ? (string) null : "Game.") + type.Name + (this._zExtension == "lua" ? ":" : ".") + methodInfo.Name + "(" + str + ")");
          str = (string) null;
        }
      }
    }

    private bool Save()
    {
      try
      {
        StreamWriter streamWriter = new StreamWriter(this._zFile);
        string text = this._hEditor.Text;
        streamWriter.Write(text);
        streamWriter.Close();
      }
      catch (Exception ex)
      {
        int num = 0U > 0U ? 1 : 0;
        Program.Exception(ex, num != 0);
        return false;
      }
      if (this.Text.EndsWith("*"))
      {
        this.Text = this.Text.Substring(0, this.Text.Length - 2);
        this._bChanged = false;
      }
      return true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IContainer container = this.components;
        if (container != null)
          container.Dispose();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this._hEditor = new Scintilla();
      this._hEditor.BeginInit();
      this.SuspendLayout();
      this._hEditor.ConfigurationManager.Language = "lua";
      this._hEditor.Dock = DockStyle.Fill;
      this._hEditor.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this._hEditor.Location = new Point(0, 0);
      this._hEditor.Name = "_hEditor";
      this._hEditor.Scrolling.HorizontalWidth = 800;
      this._hEditor.Size = new Size(632, 613);
      this._hEditor.Styles.BraceBad.FontName = "Verdana";
      this._hEditor.Styles.BraceLight.FontName = "Verdana";
      this._hEditor.Styles.ControlChar.FontName = "Verdana";
      this._hEditor.Styles.Default.FontName = "Verdana";
      this._hEditor.Styles.IndentGuide.FontName = "Verdana";
      this._hEditor.Styles.LastPredefined.FontName = "Verdana";
      this._hEditor.Styles.LineNumber.FontName = "Verdana";
      this._hEditor.Styles.Max.FontName = "Verdana";
      this._hEditor.TabIndex = 0;
      this._hEditor.DocumentChange += new EventHandler<NativeScintillaEventArgs>(this.Editor_DocumentChange);
      this._hEditor.KeyDown += new KeyEventHandler(this.Editor_KeyDown);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(632, 613);
      this.Controls.Add((Control) this._hEditor);
      this.Name = "IEditor";
      this.Text = "Editor";
      this.FormClosing += new FormClosingEventHandler(this.Editor_FormClosing);
      this.Load += new EventHandler(this.Editor_Load);
      this._hEditor.EndInit();
      this.ResumeLayout(false);
    }
  }
}
