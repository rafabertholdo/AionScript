// Decompiled with JetBrains decompiler
// Type: AionScript.IManager
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using Microsoft.VisualBasic;
using Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AionScript
{
  public class IManager : Form
  {
    private static bool _bScript = false;
    private static Dictionary<string, IScripting> _hScriptingList = new Dictionary<string, IScripting>();
    private Dictionary<string, ITravel> _hNodeList = new Dictionary<string, ITravel>();
    private string _zFolderExtension;
    private string _zFolderNode;
    private string _zFolderScripting;
    private IContainer components;
    private PictureBox DialogPicture;
    private Label InfoLabel;
    private PictureBox InfoPicture;
    public ComboBox NodeList;
    public ContextMenuStrip NodeListContext;
    private ToolStripMenuItem NodeListContextClear;
    public ToolStripMenuItem NodeListContextCreate;
    public ToolStripMenuItem NodeListContextDelete;
    public ToolStripMenuItem NodeListContextEdit;
    public ToolStripMenuItem NodeListContextReload;
    private PictureBox OffsetsPicture;
    private Panel PanelUpper;
    public ComboBox ScriptList;
    public ContextMenuStrip ScriptListContext;
    private ToolStripMenuItem ScriptListContextClear;
    public ToolStripMenuItem ScriptListContextCreate;
    public ToolStripMenuItem ScriptListContextDelete;
    public ToolStripMenuItem ScriptListContextEdit;
    public ToolStripMenuItem ScriptListContextReload;
    public RichTextBox ScriptOutput;
    private ContextMenuStrip ScriptOutputContext;
    private ToolStripMenuItem ScriptOutputContextClear;
    public Panel ScriptOutputPanel;
    public Button ScriptToggle;
    private ToolTip ToolTip;
    private Panel panel1;
    public NotifyIcon TrayIcon;

    public IManager()
    {
      this.InitializeComponent();
      this.Text = Program.CurrentName;
      this.TrayIcon.Icon = this.Icon;
      this.TrayIcon.Text = this.Text + " (" + new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToLongDateString() + ")";
    }

    private void _OnClick(object sender, EventArgs e)
    {
      if (sender == this.InfoPicture)
      {
        if (!File.Exists(Program.CurrentDirectory + "AionScript.pdf"))
          return;
        Process.Start(Program.CurrentDirectory + "AionScript.pdf");
      }
      else if (sender == this.DialogPicture)
      {
        Program.Dialog.Show();
      }
      else
      {
        if (sender == this.OffsetsPicture)
          return;
        if (sender != this.NodeListContextCreate)
        {
          if (sender == this.NodeListContextClear)
            this.NodeList.SelectedIndex = -1;
          else if (sender == this.NodeListContextDelete)
          {
            if (MessageBox.Show("This file will be permanently deleted from the file system and cannot be restored later if you change your mind! Are you absolutely sure you wish to delete this node forever?", "Confirm Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
              return;
            string zName = (string) this.NodeList.SelectedItem;
            this.NodeClose(zName);
            File.Delete(this._zFolderNode + "\\" + zName);
            this.NodeList.Items.Remove((object) zName);
            this.NodeList.SelectedIndex = -1;
          }
          else if (sender == this.NodeListContextEdit)
          {
            string key = (string) this.NodeList.SelectedItem;
            if (this._hNodeList.ContainsKey(key) && !this._hNodeList[key].IsDisposed)
            {
              this._hNodeList[key].Show();
              this._hNodeList[key].Activate();
            }
            else
            {
              this._hNodeList[key] = new ITravel(this._zFolderNode + "\\" + key);
              this._hNodeList[key].Show();
            }
          }
          else if (sender == this.NodeListContextReload)
            this.SetFolderNode(this._zFolderNode);
          else if (sender != this.ScriptListContextCreate)
          {
            if (sender == this.ScriptListContextClear)
              this.ScriptList.SelectedIndex = -1;
            else if (sender == this.ScriptListContextDelete)
            {
              if (MessageBox.Show("This file will be permanently deleted from the file system and cannot be restored later if you change your mind! Are you absolutely sure you wish to delete this script forever?", "Confirm Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
              string zName = (string) this.ScriptList.SelectedItem;
              this.ScriptClose(zName);
              File.Delete(this._zFolderScripting + "\\" + zName);
              this.ScriptList.Items.Remove((object) zName);
              this.ScriptList.SelectedIndex = -1;
            }
            else if (sender == this.ScriptListContextEdit)
            {
              string key = (string) this.ScriptList.SelectedItem;
              if (IManager._hScriptingList.ContainsKey(key) && !IManager._hScriptingList[key].IsDisposed)
              {
                IManager._hScriptingList[key].Show();
                IManager._hScriptingList[key].Activate();
              }
              else if (File.Exists(this._zFolderScripting + "\\" + key))
              {
                IManager._hScriptingList[key] = new IScripting(this._zFolderScripting + "\\" + key);
                IManager._hScriptingList[key].Show();
              }
              else
              {
                int num = (int) MessageBox.Show("This file does not exist. Perhaps this is a project?");
              }
            }
            else if (sender == this.ScriptListContextReload)
              this.SetFolderScripting(this._zFolderScripting);
            else if (sender == this.ScriptOutputContextClear)
            {
              this.ScriptOutput.Text = "Clearing the console window...";
            }
            else
            {
              if (sender != this.ScriptToggle)
                return;
              if (IManager._bScript)
                this.ScriptExit();
              else if (Scripting.Load(this._zFolderScripting + "\\" + (string) this.ScriptList.SelectedItem))
              {
                this.ScriptList.Enabled = false;
                this.NodeList.Enabled = false;
                this.ScriptToggle.Text = "Disable";
                IManager._bScript = true;
              }
              else
                this.InfoLabel.Focus();
            }
          }
          else
          {
            string str = Interaction.InputBox("Please provide the name of the new script file.", string.Empty, string.Empty, -1, -1);
            if (str == null || str.Length == 0)
              return;
            if (!str.EndsWith(".lua") && !str.EndsWith(".cs") && !str.EndsWith(".vb"))
              str += ".lua";
            if (!this.ScriptList.Items.Contains((object) str))
              this.ScriptList.Items.Add((object) str);
            for (int index = 0; index < this.ScriptList.Items.Count; ++index)
            {
              if ((string) this.ScriptList.Items[index] == str)
              {
                this.ScriptList.SelectedIndex = index;
                break;
              }
            }
            if (!File.Exists(this._zFolderScripting + "\\" + str))
              File.Create(this._zFolderScripting + "\\" + str).Close();
            this.ScriptListContextEdit.PerformClick();
          }
        }
        else
        {
          string str = Interaction.InputBox("Please provide the name of the new node path.", string.Empty, string.Empty, -1, -1);
          if (str == null || str.Length == 0)
            return;
          if (!str.ToLower().EndsWith(".xml"))
            str += ".xml";
          if (!this.NodeList.Items.Contains((object) str))
            this.NodeList.Items.Add((object) str);
          for (int index = 0; index < this.NodeList.Items.Count; ++index)
          {
            if ((string) this.NodeList.Items[index] == str)
            {
              this.NodeList.SelectedIndex = index;
              break;
            }
          }
          if (!File.Exists(this._zFolderNode + "\\" + str))
            File.Create(this._zFolderNode + "\\" + str).Close();
          this.NodeListContextEdit.PerformClick();
        }
      }
    }

    private void _OnClickOverlay(object sender, EventArgs e)
    {
      ((Control) sender).BringToFront();
    }

    private void _OnClose(object sender, FormClosingEventArgs e)
    {
      Dictionary<string, ITravel> dictionary1 = new Dictionary<string, ITravel>();
      Dictionary<string, IScripting> dictionary2 = new Dictionary<string, IScripting>();
      foreach (KeyValuePair<string, ITravel> keyValuePair in this._hNodeList)
        dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
      foreach (KeyValuePair<string, IScripting> keyValuePair in IManager._hScriptingList)
        dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
      foreach (KeyValuePair<string, ITravel> keyValuePair in dictionary1)
        keyValuePair.Value.Close();
      foreach (KeyValuePair<string, IScripting> keyValuePair in dictionary2)
        keyValuePair.Value.Close();
      foreach (ArrayList arrayList in Extension.List())
      {
        if (arrayList[3] != null)
        {
          string str = ((string) arrayList[0]).Substring(((string) arrayList[0]).LastIndexOf("\\") + 1);
          Form form = (Form) arrayList[3];
          Dictionary<string, string> hValues = new Dictionary<string, string>();
          hValues["Left"] = form.Left.ToString();
          hValues["Top"] = form.Top.ToString();
          Program.Setting.SetDictionary("Window/" + str, hValues);
        }
      }
      Extension.Close(Program.Setting);
      Program.Close();
    }

    private void _OnDoubleClick(object sender, MouseEventArgs e)
    {
      if (sender != this.TrayIcon)
        return;
      this.Visible = !this.Visible;
    }

    private void _OnLinkClicked(object sender, LinkClickedEventArgs e)
    {
      if (e.LinkText.StartsWith("http://www.aionscript.com"))
      {
        Process.Start(e.LinkText);
      }
      else
      {
        if (MessageBox.Show("This link will take you of the AionScript domain. Do you wish to continue? This might pose a security risk!", Program.CurrentName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
          return;
        Process.Start(e.LinkText);
      }
    }

    private void _OnSelectedIndexChanged(object sender, EventArgs e)
    {
      if (sender == this.NodeList)
      {
        if (this.NodeList.SelectedIndex != -1)
        {
          this.NodeListContextDelete.Enabled = true;
          this.NodeListContextEdit.Enabled = true;
        }
        else
        {
          this.NodeListContextDelete.Enabled = false;
          this.NodeListContextEdit.Enabled = false;
        }
      }
      else
      {
        if (sender != this.ScriptList)
          return;
        if (this.ScriptList.SelectedIndex != -1)
        {
          this.ScriptListContextDelete.Enabled = true;
          this.ScriptListContextEdit.Enabled = true;
          this.ScriptToggle.Enabled = true;
        }
        else
        {
          this.ScriptListContextDelete.Enabled = false;
          this.ScriptListContextEdit.Enabled = false;
          this.ScriptToggle.Enabled = false;
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (IManager));
      this.ScriptListContext = new ContextMenuStrip(this.components);
      this.ScriptListContextCreate = new ToolStripMenuItem();
      this.ScriptListContextClear = new ToolStripMenuItem();
      this.ScriptListContextDelete = new ToolStripMenuItem();
      this.ScriptListContextEdit = new ToolStripMenuItem();
      this.ScriptListContextReload = new ToolStripMenuItem();
      this.TrayIcon = new NotifyIcon(this.components);
      this.ScriptToggle = new Button();
      this.ScriptList = new ComboBox();
      this.NodeList = new ComboBox();
      this.NodeListContext = new ContextMenuStrip(this.components);
      this.NodeListContextCreate = new ToolStripMenuItem();
      this.NodeListContextClear = new ToolStripMenuItem();
      this.NodeListContextDelete = new ToolStripMenuItem();
      this.NodeListContextEdit = new ToolStripMenuItem();
      this.NodeListContextReload = new ToolStripMenuItem();
      this.ToolTip = new ToolTip(this.components);
      this.ScriptOutput = new RichTextBox();
      this.ScriptOutputContext = new ContextMenuStrip(this.components);
      this.ScriptOutputContextClear = new ToolStripMenuItem();
      this.ScriptOutputPanel = new Panel();
      this.PanelUpper = new Panel();
      this.DialogPicture = new PictureBox();
      this.OffsetsPicture = new PictureBox();
      this.InfoPicture = new PictureBox();
      this.InfoLabel = new Label();
      this.panel1 = new Panel();
      this.ScriptListContext.SuspendLayout();
      this.NodeListContext.SuspendLayout();
      this.ScriptOutputContext.SuspendLayout();
      this.ScriptOutputPanel.SuspendLayout();
      this.PanelUpper.SuspendLayout();
      ((ISupportInitialize) this.DialogPicture).BeginInit();
      ((ISupportInitialize) this.OffsetsPicture).BeginInit();
      ((ISupportInitialize) this.InfoPicture).BeginInit();
      this.SuspendLayout();
      this.ScriptListContext.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.ScriptListContextCreate,
        (ToolStripItem) this.ScriptListContextClear,
        (ToolStripItem) this.ScriptListContextDelete,
        (ToolStripItem) this.ScriptListContextEdit,
        (ToolStripItem) this.ScriptListContextReload
      });
      this.ScriptListContext.Name = "hScriptListContext";
      this.ScriptListContext.Size = new Size(111, 114);
      this.ScriptListContextCreate.Name = "ScriptListContextCreate";
      this.ScriptListContextCreate.Size = new Size(110, 22);
      this.ScriptListContextCreate.Text = "Create";
      this.ScriptListContextCreate.Click += new EventHandler(this._OnClick);
      this.ScriptListContextClear.Name = "ScriptListContextClear";
      this.ScriptListContextClear.Size = new Size(110, 22);
      this.ScriptListContextClear.Text = "Clear";
      this.ScriptListContextClear.Click += new EventHandler(this._OnClick);
      this.ScriptListContextDelete.Enabled = false;
      this.ScriptListContextDelete.Name = "ScriptListContextDelete";
      this.ScriptListContextDelete.Size = new Size(110, 22);
      this.ScriptListContextDelete.Text = "Delete";
      this.ScriptListContextDelete.Click += new EventHandler(this._OnClick);
      this.ScriptListContextEdit.Enabled = false;
      this.ScriptListContextEdit.Name = "ScriptListContextEdit";
      this.ScriptListContextEdit.Size = new Size(110, 22);
      this.ScriptListContextEdit.Text = "Edit";
      this.ScriptListContextEdit.Click += new EventHandler(this._OnClick);
      this.ScriptListContextReload.Name = "ScriptListContextReload";
      this.ScriptListContextReload.Size = new Size(110, 22);
      this.ScriptListContextReload.Text = "Reload";
      this.ScriptListContextReload.Click += new EventHandler(this._OnClick);
      this.TrayIcon.MouseDoubleClick += new MouseEventHandler(this._OnDoubleClick);
      this.ScriptToggle.BackColor = SystemColors.Window;
      this.ScriptToggle.Enabled = false;
      this.ScriptToggle.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ScriptToggle.FlatStyle = FlatStyle.Flat;
      this.ScriptToggle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ScriptToggle.Location = new Point(177, 356);
      this.ScriptToggle.Name = "ScriptToggle";
      this.ScriptToggle.Size = new Size(226, 49);
      this.ScriptToggle.TabIndex = 3;
      this.ScriptToggle.Text = "Enable";
      this.ScriptToggle.UseVisualStyleBackColor = false;
      this.ScriptToggle.Click += new EventHandler(this._OnClick);
      this.ScriptList.BackColor = SystemColors.ControlLight;
      this.ScriptList.ContextMenuStrip = this.ScriptListContext;
      this.ScriptList.DropDownStyle = ComboBoxStyle.DropDownList;
      this.ScriptList.FlatStyle = FlatStyle.Flat;
      this.ScriptList.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ScriptList.FormattingEnabled = true;
      this.ScriptList.Location = new Point(8, 356);
      this.ScriptList.Name = "ScriptList";
      this.ScriptList.Size = new Size(163, 21);
      this.ScriptList.Sorted = true;
      this.ScriptList.TabIndex = 1;
      this.ToolTip.SetToolTip((Control) this.ScriptList, "Scripts allow you to write your own automated logic (such as a heal- or grind bot)!");
      this.ScriptList.SelectedIndexChanged += new EventHandler(this._OnSelectedIndexChanged);
      this.NodeList.BackColor = SystemColors.ControlLight;
      this.NodeList.ContextMenuStrip = this.NodeListContext;
      this.NodeList.DropDownStyle = ComboBoxStyle.DropDownList;
      this.NodeList.FlatStyle = FlatStyle.Flat;
      this.NodeList.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.NodeList.FormattingEnabled = true;
      this.NodeList.Location = new Point(8, 384);
      this.NodeList.Name = "NodeList";
      this.NodeList.Size = new Size(163, 21);
      this.NodeList.TabIndex = 2;
      this.ToolTip.SetToolTip((Control) this.NodeList, "Node paths allow scripts to move around naturally and automate tasks in different areas.");
      this.NodeList.SelectedIndexChanged += new EventHandler(this._OnSelectedIndexChanged);
      this.NodeListContext.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.NodeListContextCreate,
        (ToolStripItem) this.NodeListContextClear,
        (ToolStripItem) this.NodeListContextDelete,
        (ToolStripItem) this.NodeListContextEdit,
        (ToolStripItem) this.NodeListContextReload
      });
      this.NodeListContext.Name = "hScriptListContext";
      this.NodeListContext.Size = new Size(111, 114);
      this.NodeListContextCreate.Name = "NodeListContextCreate";
      this.NodeListContextCreate.Size = new Size(110, 22);
      this.NodeListContextCreate.Text = "Create";
      this.NodeListContextCreate.Click += new EventHandler(this._OnClick);
      this.NodeListContextClear.Name = "NodeListContextClear";
      this.NodeListContextClear.Size = new Size(110, 22);
      this.NodeListContextClear.Text = "Clear";
      this.NodeListContextClear.Click += new EventHandler(this._OnClick);
      this.NodeListContextDelete.Enabled = false;
      this.NodeListContextDelete.Name = "NodeListContextDelete";
      this.NodeListContextDelete.Size = new Size(110, 22);
      this.NodeListContextDelete.Text = "Delete";
      this.NodeListContextDelete.Click += new EventHandler(this._OnClick);
      this.NodeListContextEdit.Enabled = false;
      this.NodeListContextEdit.Name = "NodeListContextEdit";
      this.NodeListContextEdit.Size = new Size(110, 22);
      this.NodeListContextEdit.Text = "Edit";
      this.NodeListContextEdit.Click += new EventHandler(this._OnClick);
      this.NodeListContextReload.Name = "NodeListContextReload";
      this.NodeListContextReload.Size = new Size(110, 22);
      this.NodeListContextReload.Text = "Reload";
      this.NodeListContextReload.Click += new EventHandler(this._OnClick);
      this.ToolTip.IsBalloon = true;
      this.ScriptOutput.BackColor = SystemColors.Window;
      this.ScriptOutput.BorderStyle = BorderStyle.None;
      this.ScriptOutput.ContextMenuStrip = this.ScriptOutputContext;
      this.ScriptOutput.Dock = DockStyle.Fill;
      this.ScriptOutput.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ScriptOutput.Location = new Point(0, 0);
      this.ScriptOutput.Name = "ScriptOutput";
      this.ScriptOutput.ReadOnly = true;
      this.ScriptOutput.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
      this.ScriptOutput.Size = new Size(393, 312);
      this.ScriptOutput.TabIndex = 1;
      this.ScriptOutput.Text = "";
      this.ScriptOutput.WordWrap = false;
      this.ScriptOutput.LinkClicked += new LinkClickedEventHandler(this._OnLinkClicked);
      this.ScriptOutputContext.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.ScriptOutputContextClear
      });
      this.ScriptOutputContext.Name = "ScriptOutputContext";
      this.ScriptOutputContext.Size = new Size(102, 26);
      this.ScriptOutputContextClear.Name = "ScriptOutputContextClear";
      this.ScriptOutputContextClear.Size = new Size(101, 22);
      this.ScriptOutputContextClear.Text = "Clear";
      this.ScriptOutputContextClear.Click += new EventHandler(this._OnClick);
      this.ScriptOutputPanel.BorderStyle = BorderStyle.FixedSingle;
      this.ScriptOutputPanel.Controls.Add((Control) this.ScriptOutput);
      this.ScriptOutputPanel.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ScriptOutputPanel.Location = new Point(8, 36);
      this.ScriptOutputPanel.Name = "ScriptOutputPanel";
      this.ScriptOutputPanel.Size = new Size(395, 314);
      this.ScriptOutputPanel.TabIndex = 0;
      this.PanelUpper.BackgroundImage = (Image) Resources.BackgroundUpper;
      this.PanelUpper.Controls.Add((Control) this.DialogPicture);
      this.PanelUpper.Controls.Add((Control) this.OffsetsPicture);
      this.PanelUpper.Controls.Add((Control) this.InfoPicture);
      this.PanelUpper.Controls.Add((Control) this.InfoLabel);
      this.PanelUpper.Dock = DockStyle.Top;
      this.PanelUpper.Location = new Point(0, 0);
      this.PanelUpper.Name = "PanelUpper";
      this.PanelUpper.Size = new Size(411, 60);
      this.PanelUpper.TabIndex = 8;
      this.DialogPicture.BackColor = Color.Transparent;
      this.DialogPicture.BackgroundImage = (Image) Resources.Zoom_icon;
      this.DialogPicture.Location = new Point(386, 12);
      this.DialogPicture.Name = "DialogPicture";
      this.DialogPicture.Size = new Size(16, 16);
      this.DialogPicture.TabIndex = 8;
      this.DialogPicture.TabStop = false;
      this.DialogPicture.Click += new EventHandler(this._OnClick);
      this.OffsetsPicture.BackColor = Color.Transparent;
      this.OffsetsPicture.BackgroundImage = (Image) Resources.Offsets;
      this.OffsetsPicture.Enabled = false;
      this.OffsetsPicture.Location = new Point(342, 12);
      this.OffsetsPicture.Name = "OffsetsPicture";
      this.OffsetsPicture.Size = new Size(16, 16);
      this.OffsetsPicture.TabIndex = 7;
      this.OffsetsPicture.TabStop = false;
      this.OffsetsPicture.Visible = false;
      this.OffsetsPicture.Click += new EventHandler(this._OnClick);
      this.InfoPicture.BackColor = Color.Transparent;
      this.InfoPicture.BackgroundImage = (Image) Resources.Information;
      this.InfoPicture.Enabled = false;
      this.InfoPicture.Location = new Point(364, 12);
      this.InfoPicture.Name = "InfoPicture";
      this.InfoPicture.Size = new Size(16, 16);
      this.InfoPicture.TabIndex = 6;
      this.InfoPicture.TabStop = false;
      this.InfoPicture.Visible = false;
      this.InfoPicture.Click += new EventHandler(this._OnClick);
      this.InfoLabel.AutoSize = true;
      this.InfoLabel.BackColor = Color.Transparent;
      this.InfoLabel.Font = new Font("Segoe UI", 15.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.InfoLabel.Location = new Point(4, 4);
      this.InfoLabel.Name = "InfoLabel";
      this.InfoLabel.Size = new Size(0, 30);
      this.InfoLabel.TabIndex = 5;
      this.panel1.BackgroundImage = (Image) Resources.BackgroundBottom;
      this.panel1.Dock = DockStyle.Bottom;
      this.panel1.Location = new Point(0, 411);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(411, 48);
      this.panel1.TabIndex = 9;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(411, 459);
      this.Controls.Add((Control) this.NodeList);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.ScriptList);
      this.Controls.Add((Control) this.ScriptToggle);
      this.Controls.Add((Control) this.ScriptOutputPanel);
      this.Controls.Add((Control) this.PanelUpper);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.Name = "IManager";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Manager";
      this.FormClosing += new FormClosingEventHandler(this._OnClose);
      this.ScriptListContext.ResumeLayout(false);
      this.NodeListContext.ResumeLayout(false);
      this.ScriptOutputContext.ResumeLayout(false);
      this.ScriptOutputPanel.ResumeLayout(false);
      this.PanelUpper.ResumeLayout(false);
      this.PanelUpper.PerformLayout();
      ((ISupportInitialize) this.DialogPicture).EndInit();
      ((ISupportInitialize) this.OffsetsPicture).EndInit();
      ((ISupportInitialize) this.InfoPicture).EndInit();
      this.ResumeLayout(false);
    }

    public void NodeClose(string zName)
    {
      if (!this._hNodeList.ContainsKey(zName))
        return;
      this._hNodeList[zName].Close();
      this._hNodeList.Remove(zName);
    }

    public void ScriptClose(string zName)
    {
      if (!IManager._hScriptingList.ContainsKey(zName))
        return;
      IManager._hScriptingList[zName].Close();
      IManager._hScriptingList.Remove(zName);
    }

    public void ScriptExit()
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke((Delegate) new Action(this.ScriptExit));
      }
      else
      {
        if (!IManager._bScript)
          return;
        Scripting.Close();
        this.ScriptList.Enabled = true;
        this.NodeList.Enabled = true;
        this.ScriptToggle.Text = "Enable";
        IManager._bScript = false;
      }
    }

    public object ScriptInclude(string zFile)
    {
      return Scripting.Include(this._zFolderScripting + "\\" + zFile);
    }

    public void ScriptProcess(string zMessage)
    {
      this.ScriptWrite("> " + zMessage);
      if (!IManager._bScript)
        return;
      Scripting.Execute(zMessage + ";");
    }

    public void ScriptWrite(string zMessage)
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke((Delegate) new Program.DelegateString(this.ScriptWrite), (object) zMessage);
      }
      else
      {
        if (zMessage == null)
          return;
        this.ScriptOutput.AppendText("\n" + (zMessage.StartsWith(">") ? (string) null : "- ") + zMessage);
        this.ScriptOutput.SelectionStart = Program.Manager.ScriptOutput.Text.Length;
        this.ScriptOutput.ScrollToCaret();
      }
    }

    public void SetAccountName(string zName)
    {
      this.InfoLabel.Text = zName == null ? "Limited Demo" : "Welcome, " + zName;
    }

    public void SetCharacterName(string zName)
    {
      this.Text = "Manager" + (zName == null ? (string) null : " (" + zName + ")");
    }

    public void SetFolderExtension(string zFolder)
    {
      if (Directory.Exists(zFolder))
      {
        foreach (string zFile in Directory.GetFiles(zFolder))
        {
          if (zFile.EndsWith(".dll"))
            Extension.Load(zFile, Program.Setting);
        }
      }
      foreach (ArrayList arrayList in Extension.List())
      {
        if (arrayList[3] != null)
        {
          string str = ((string) arrayList[0]).Substring(((string) arrayList[0]).LastIndexOf("\\") + 1);
          Form form = (Form) arrayList[3];
          Dictionary<string, string> dictionary = Program.Setting.GetDictionary("Window/" + str);
          form.TopLevel = false;
          form.FormBorderStyle = form.FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.None : FormBorderStyle.FixedSingle;
          form.MaximizeBox = false;
          form.Click += new EventHandler(this._OnClickOverlay);
          form.ShowIcon = false;
          form.Visible = true;
          Program.Overlay.Controls.Add((Control) form);
          try
          {
            form.Left = int.Parse(dictionary["Left"]);
            form.Top = int.Parse(dictionary["Top"]);
            if (form.Left < 0)
              form.Left = 0;
            if (form.Top < 0)
              form.Top = 0;
            if (form.Left + form.Width > Program.Overlay.Width)
              form.Left = Program.Overlay.Width - form.Width;
            if (form.Top + form.Height > Program.Overlay.Height)
              form.Top = Program.Overlay.Height - form.Height;
          }
          catch (Exception ex)
          {
          }
        }
      }
      this._zFolderExtension = zFolder;
    }

    public void SetFolderNode(string zFolder)
    {
      this.NodeList.Items.Clear();
      if (Directory.Exists(zFolder))
      {
        foreach (string str in Directory.GetFiles(zFolder))
        {
          if (str.EndsWith(".xml"))
            this.NodeList.Items.Add((object) str.Substring(str.LastIndexOf("\\") + 1));
        }
      }
      this._zFolderNode = zFolder;
    }

    public void SetFolderScripting(string zFolder)
    {
      this.ScriptList.Items.Clear();
      if (Directory.Exists(zFolder))
      {
        foreach (string path in Directory.GetDirectories(zFolder))
        {
          if (File.Exists(path + Path.DirectorySeparatorChar.ToString() + Path.GetFileNameWithoutExtension(path) + ".csproj"))
            Program.Manager.ScriptList.Items.Add((object) Path.GetFileNameWithoutExtension(path));
        }
        foreach (string str in Directory.GetFiles(zFolder))
        {
          if (str.ToLower().EndsWith(".lua") || str.ToLower().EndsWith(".cs") || str.ToLower().EndsWith(".vb"))
            Program.Manager.ScriptList.Items.Add((object) str.Substring(str.LastIndexOf("\\") + 1));
        }
      }
      this._zFolderScripting = zFolder;
    }

    public void SetOffset(string zSkill)
    {
      Game.Stop();
      if (!Game.Start(zSkill))
        Program.Exception(new Exception("Skill file does not exist or is not valid!"), false);
      Program.Manager.Show();
    }

    public bool SetTravel(string zFile = null)
    {
      if (zFile != null && !zFile.EndsWith(".xml"))
        zFile += ".xml";
      if (zFile == null && this.NodeList.SelectedIndex == -1 || zFile != null && !File.Exists(this._zFolderNode + "\\" + zFile))
      {
        Game.TravelList = (TravelList) null;
        return false;
      }
      Game.TravelList = new TravelList(this._zFolderNode + (object) "\\" + (string) (zFile == null ? this.NodeList.SelectedItem : (object) zFile));
      return true;
    }
  }
}
