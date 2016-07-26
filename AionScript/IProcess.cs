// Decompiled with JetBrains decompiler
// Type: AionScript.IProcess
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AionScript
{
  public class IProcess : Form
  {
    private System.Collections.Generic.List<uint> _hProcessList = new System.Collections.Generic.List<uint>();
    public Button Button;
    private IContainer components;
    private Label Label;
    private ComboBox List;
    private Panel Panel;
    private Panel PanelUpper;

    public IProcess()
    {
      this.InitializeComponent();
    }

    private void _OnClick(object sender, EventArgs e)
    {
      if (sender != this.Button || this.List.SelectedIndex == -1 || this._hProcessList.Count <= this.List.SelectedIndex)
        return;
      Game.Process.Open(this._hProcessList[this.List.SelectedIndex]);
      Game.LoadVersion();
      this.Close();
    }

    private void _OnSelectedIndexChanged(object sender, EventArgs e)
    {
      if (sender != this.List)
        return;
      this.Button.Enabled = true;
    }

    private void _OnShown(object sender, EventArgs e)
    {
      if (sender != this)
        return;
      this.Label.Focus();
    }

    public void Add(uint iProcessID, uint iCharacterID, string zName, byte iLevel)
    {
      ComboBox.ObjectCollection items = this.List.Items;
      // ISSUE: variable of a boxed type
      var local = (ValueType) iProcessID;
      string str1 = ": ";
      string str2;
      if (zName != null && zName.Length != 0)
        str2 = string.Concat(new object[4]
        {
          (object) zName,
          (object) " (",
          (object) iLevel,
          (object) ")"
        });
      else
        str2 = "No character information.";
      string str3 = (string) (object) local + (object) str1 + str2;
      items.Add((object) str3);
      this._hProcessList.Add(iProcessID);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (IProcess));
      this.PanelUpper = new Panel();
      this.Label = new Label();
      this.Panel = new Panel();
      this.List = new ComboBox();
      this.Button = new Button();
      this.PanelUpper.SuspendLayout();
      this.Panel.SuspendLayout();
      this.SuspendLayout();
      this.PanelUpper.BackgroundImage = (Image) Resources.BackgroundUpper;
      this.PanelUpper.Controls.Add((Control) this.Label);
      this.PanelUpper.Controls.Add((Control) this.Panel);
      this.PanelUpper.Dock = DockStyle.Top;
      this.PanelUpper.Location = new Point(0, 0);
      this.PanelUpper.Name = "PanelUpper";
      this.PanelUpper.Size = new Size(250, 60);
      this.PanelUpper.TabIndex = 10;
      this.Label.AutoSize = true;
      this.Label.BackColor = Color.Transparent;
      this.Label.Font = new Font("Segoe UI", 15.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Label.Location = new Point(4, 1);
      this.Label.Name = "Label";
      this.Label.Size = new Size(126, 30);
      this.Label.TabIndex = 0;
      this.Label.Text = "Pick Process";
      this.Panel.BorderStyle = BorderStyle.FixedSingle;
      this.Panel.Controls.Add((Control) this.List);
      this.Panel.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Panel.Location = new Point(8, 32);
      this.Panel.Name = "Panel";
      this.Panel.Size = new Size(232, 24);
      this.Panel.TabIndex = 8;
      this.List.Dock = DockStyle.Fill;
      this.List.DropDownStyle = ComboBoxStyle.DropDownList;
      this.List.FlatStyle = FlatStyle.Flat;
      this.List.FormattingEnabled = true;
      this.List.Location = new Point(0, 0);
      this.List.Name = "List";
      this.List.Size = new Size(230, 21);
      this.List.TabIndex = 1;
      this.List.SelectedIndexChanged += new EventHandler(this._OnSelectedIndexChanged);
      this.Button.BackColor = SystemColors.Window;
      this.Button.Enabled = false;
      this.Button.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.Button.FlatStyle = FlatStyle.Flat;
      this.Button.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Button.Location = new Point(188, 64);
      this.Button.Name = "Button";
      this.Button.Size = new Size(52, 24);
      this.Button.TabIndex = 2;
      this.Button.Text = "Accept";
      this.Button.UseVisualStyleBackColor = false;
      this.Button.Click += new EventHandler(this._OnClick);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(250, 96);
      this.Controls.Add((Control) this.PanelUpper);
      this.Controls.Add((Control) this.Button);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = "IProcess";
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Pick Process";
      this.Shown += new EventHandler(this._OnShown);
      this.PanelUpper.ResumeLayout(false);
      this.PanelUpper.PerformLayout();
      this.Panel.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
