// Decompiled with JetBrains decompiler
// Type: AionScript.IDialog
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
  public class IDialog : Form
  {
    protected IDialogNode _hTreeNode;
    private Button ButtonClick;
    private Button ButtonCopy;
    private Button ButtonEnable;
    private Button ButtonShow;
    private ComboBox ComboBox;
    private IContainer components;
    private Label Label;
    private Panel PanelLanguage;
    private Panel PanelLower;
    private Panel PanelUpper;
    private TreeView TreeView;

    public IDialog()
    {
      this.InitializeComponent();
      this.ComboBox.SelectedIndex = 1;
    }

    protected void _OnAfterSelect(object sender, TreeViewEventArgs e)
    {
      if (this._hTreeNode != null && (e.Node.Parent == null || e.Node.Parent != this._hTreeNode))
      {
        if (e.Node == null || e.Node.Parent != this._hTreeNode.Parent)
          this._hTreeNode.CollapseAll();
        this._hTreeNode.Update(false, false);
        this._hTreeNode.Nodes.Clear();
      }
      if (e.Node == null)
        return;
      this._hTreeNode = (IDialogNode) e.Node;
      this.ButtonCopy.Enabled = true;
      this.Update();
      this._hTreeNode.Expand();
    }

    private void _OnClick(object sender, EventArgs e)
    {
      if (sender == this.ButtonClick)
      {
        if (this._hTreeNode != null)
          this._hTreeNode.Dialog((Dialog) null).Click();
      }
      else if (sender == this.ButtonCopy)
      {
        if (this._hTreeNode != null)
        {
          IDialogNode idialogNode = this._hTreeNode;
          string str = (string) null;
          for (; idialogNode != null; idialogNode = (IDialogNode) idialogNode.Parent)
          {
            string name = idialogNode.Dialog((Dialog) null).GetName();
            str = (name == null || name.Length == 0 ? idialogNode.Dialog((Dialog) null).GetIndex().ToString() : name) + (str == null ? string.Empty : "/" + str);
          }
          Clipboard.SetText(this.ComboBox.SelectedIndex != 1 ? "Game.DialogList.GetDialog( \"" + str + "\" )" : "DialogList:GetDialog( \"" + str + "\" )");
          this.ButtonCopy.Enabled = false;
        }
      }
      else if (sender == this.ButtonEnable)
      {
        if (this._hTreeNode != null)
          this._hTreeNode.Dialog((Dialog) null).SetEnabled(!this._hTreeNode.Dialog((Dialog) null).IsEnabled());
        this.Update();
      }
      else if (sender == this.ButtonShow)
      {
        if (this._hTreeNode != null)
          this._hTreeNode.Dialog((Dialog) null).SetVisible(!this._hTreeNode.Dialog((Dialog) null).IsVisible());
        this.Update();
      }
      this.Label.Focus();
    }

    private void _OnClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
      this.Clear();
    }

    private void _OnShown(object sender, EventArgs e)
    {
      this.Label.Focus();
    }

    public void Clear()
    {
      this.ButtonClick.Enabled = false;
      this.ButtonCopy.Enabled = false;
      this.ButtonEnable.Enabled = false;
      this.ButtonShow.Enabled = false;
      this.TreeView.Nodes.Clear();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      //ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (IDialog));
      this.PanelUpper = new Panel();
      this.PanelLanguage = new Panel();
      this.ComboBox = new ComboBox();
      this.Label = new Label();
      this.PanelLower = new Panel();
      this.ButtonShow = new Button();
      this.ButtonEnable = new Button();
      this.ButtonCopy = new Button();
      this.ButtonClick = new Button();
      this.TreeView = new TreeView();
      this.PanelUpper.SuspendLayout();
      this.PanelLanguage.SuspendLayout();
      this.PanelLower.SuspendLayout();
      this.SuspendLayout();
      this.PanelUpper.BackgroundImage = (Image) Resources.BackgroundUpper;
      this.PanelUpper.Controls.Add((Control) this.PanelLanguage);
      this.PanelUpper.Controls.Add((Control) this.Label);
      this.PanelUpper.Dock = DockStyle.Top;
      this.PanelUpper.Location = new Point(0, 0);
      this.PanelUpper.Name = "PanelUpper";
      this.PanelUpper.Size = new Size(268, 60);
      this.PanelUpper.TabIndex = 14;
      this.PanelLanguage.BorderStyle = BorderStyle.FixedSingle;
      this.PanelLanguage.Controls.Add((Control) this.ComboBox);
      this.PanelLanguage.Location = new Point(180, 16);
      this.PanelLanguage.Name = "PanelLanguage";
      this.PanelLanguage.Size = new Size(80, 20);
      this.PanelLanguage.TabIndex = 7;
      this.ComboBox.Dock = DockStyle.Fill;
      this.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.ComboBox.FlatStyle = FlatStyle.Flat;
      this.ComboBox.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ComboBox.FormattingEnabled = true;
      this.ComboBox.Items.AddRange(new object[3]
      {
        (object) "C#",
        (object) "Lua",
        (object) "VB"
      });
      this.ComboBox.Location = new Point(0, 0);
      this.ComboBox.Name = "ComboBox";
      this.ComboBox.Size = new Size(78, 21);
      this.ComboBox.TabIndex = 6;
      this.Label.AutoSize = true;
      this.Label.BackColor = Color.Transparent;
      this.Label.Font = new Font("Segoe UI", 15.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Label.Location = new Point(4, 8);
      this.Label.Name = "Label";
      this.Label.Size = new Size(165, 30);
      this.Label.TabIndex = 5;
      this.Label.Text = "Dialog Inspector";
      this.PanelLower.BackgroundImage = (Image) Resources.BackgroundBottom;
      this.PanelLower.Controls.Add((Control) this.ButtonShow);
      this.PanelLower.Controls.Add((Control) this.ButtonEnable);
      this.PanelLower.Controls.Add((Control) this.ButtonCopy);
      this.PanelLower.Controls.Add((Control) this.ButtonClick);
      this.PanelLower.Dock = DockStyle.Bottom;
      this.PanelLower.Location = new Point(0, 392);
      this.PanelLower.Name = "PanelLower";
      this.PanelLower.Size = new Size(268, 60);
      this.PanelLower.TabIndex = 13;
      this.ButtonShow.BackColor = SystemColors.Window;
      this.ButtonShow.Enabled = false;
      this.ButtonShow.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonShow.FlatStyle = FlatStyle.Flat;
      this.ButtonShow.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonShow.Location = new Point(200, 8);
      this.ButtonShow.Name = "ButtonShow";
      this.ButtonShow.Size = new Size(60, 44);
      this.ButtonShow.TabIndex = 19;
      this.ButtonShow.Text = "Show";
      this.ButtonShow.UseVisualStyleBackColor = false;
      this.ButtonShow.Click += new EventHandler(this._OnClick);
      this.ButtonEnable.BackColor = SystemColors.Window;
      this.ButtonEnable.Enabled = false;
      this.ButtonEnable.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonEnable.FlatStyle = FlatStyle.Flat;
      this.ButtonEnable.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonEnable.Location = new Point(136, 8);
      this.ButtonEnable.Name = "ButtonEnable";
      this.ButtonEnable.Size = new Size(60, 44);
      this.ButtonEnable.TabIndex = 18;
      this.ButtonEnable.Text = "Enable";
      this.ButtonEnable.UseVisualStyleBackColor = false;
      this.ButtonEnable.Click += new EventHandler(this._OnClick);
      this.ButtonCopy.BackColor = SystemColors.Window;
      this.ButtonCopy.Enabled = false;
      this.ButtonCopy.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonCopy.FlatStyle = FlatStyle.Flat;
      this.ButtonCopy.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonCopy.Location = new Point(72, 8);
      this.ButtonCopy.Name = "ButtonCopy";
      this.ButtonCopy.Size = new Size(60, 44);
      this.ButtonCopy.TabIndex = 17;
      this.ButtonCopy.Text = "Copy";
      this.ButtonCopy.UseVisualStyleBackColor = false;
      this.ButtonCopy.Click += new EventHandler(this._OnClick);
      this.ButtonClick.BackColor = SystemColors.Window;
      this.ButtonClick.Enabled = false;
      this.ButtonClick.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonClick.FlatStyle = FlatStyle.Flat;
      this.ButtonClick.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonClick.Location = new Point(8, 8);
      this.ButtonClick.Name = "ButtonClick";
      this.ButtonClick.Size = new Size(60, 44);
      this.ButtonClick.TabIndex = 16;
      this.ButtonClick.Text = "Click";
      this.ButtonClick.UseVisualStyleBackColor = false;
      this.ButtonClick.Click += new EventHandler(this._OnClick);
      this.TreeView.BorderStyle = BorderStyle.FixedSingle;
      this.TreeView.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.TreeView.Location = new Point(8, 40);
      this.TreeView.Name = "TreeView";
      this.TreeView.ShowPlusMinus = false;
      this.TreeView.ShowRootLines = false;
      this.TreeView.Size = new Size(252, 356);
      this.TreeView.TabIndex = 15;
      this.TreeView.AfterSelect += new TreeViewEventHandler(this._OnAfterSelect);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(268, 452);
      this.Controls.Add((Control) this.TreeView);
      this.Controls.Add((Control) this.PanelUpper);
      this.Controls.Add((Control) this.PanelLower);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      //this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = "IDialog";
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Dialog Inspector";
      this.FormClosing += new FormClosingEventHandler(this._OnClosing);
      this.Shown += new EventHandler(this._OnShown);
      this.PanelUpper.ResumeLayout(false);
      this.PanelUpper.PerformLayout();
      this.PanelLanguage.ResumeLayout(false);
      this.PanelLower.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    public new void Update()
    {
      this.TreeView.SuspendLayout();
      if ((long) Game.DialogList.GetDialogSize() != (long) this.TreeView.Nodes.Count)
      {
        this.TreeView.Nodes.Clear();
        for (int index = 0; (long) index < (long) Game.DialogList.GetDialogSize(); ++index)
          this.TreeView.Nodes.Add((TreeNode) new IDialogNode(Game.DialogList.GetDialogIndex((uint) index, false)));
      }
      if (this._hTreeNode != null)
      {
        this.ButtonClick.Enabled = this._hTreeNode.Dialog((Dialog) null).IsVisible() && this._hTreeNode.Dialog((Dialog) null).IsEnabled();
        this.ButtonEnable.Enabled = true;
        this.ButtonShow.Enabled = true;
        this._hTreeNode.Update(true, false);
        this.ButtonEnable.Text = this._hTreeNode.Dialog((Dialog) null).IsEnabled() ? "Disable" : "Enable";
        this.ButtonShow.Text = this._hTreeNode.Dialog((Dialog) null).IsVisible() ? "Hide" : "Show";
      }
      else
      {
        this.ButtonClick.Enabled = false;
        this.ButtonCopy.Enabled = false;
        this.ButtonEnable.Enabled = false;
        this.ButtonShow.Enabled = false;
      }
      this.TreeView.ResumeLayout();
    }
  }
}
