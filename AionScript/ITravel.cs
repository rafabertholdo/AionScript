// Decompiled with JetBrains decompiler
// Type: AionScript.ITravel
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AionScript
{
  public class ITravel : Form
  {
    protected bool _bClosing;
    protected bool _bInitialized;
    protected bool _bRequestSave;
    protected ITravel.TravelRecorder _hTravelRecord;
    protected string _zFile;
    private IContainer components;
    private Button ButtonDelete;
    public Button ButtonAdd;
    private Button ButtonRun;
    private Panel PanelUpper;
    private Panel PanelLower;
    private Label Label;
    private MyDataGridView TravelList;
    private Panel TravelListPanel;
    private Panel TravelTypePanel;
    private ComboBox TravelType;
    private Button ButtonUp;
    private Button ButtonDown;
    private DataGridViewTextBoxColumn TravelListName;
    private DataGridViewTextBoxColumn TravelListX;
    private DataGridViewTextBoxColumn TravelListY;
    private DataGridViewTextBoxColumn TravelListZ;
    private DataGridViewCheckBoxColumn TravelListFlying;
    private DataGridViewComboBoxColumn TravelListType;
    private DataGridViewTextBoxColumn TravelListParam;
    public Button ButtonRecord;

    public ITravel(string zFile)
    {
      this.InitializeComponent();
      this.Text = Program.CurrentName;
      this._hTravelRecord = new ITravel.TravelRecorder(this);
      AionInterface.TravelList travelList = new AionInterface.TravelList(zFile);
      this.TravelType.SelectedIndex = travelList.IsReverse() ? 1 : 0;
      foreach (Travel travel in travelList.GetList())
        this.Add(travel.GetName(), travel.GetPosition().X, travel.GetPosition().Y, travel.GetPosition().Z, travel.IsFlying(), travel.GetType(), travel.GetParam());
      this.Text = this.Text + " (" + zFile.Substring(zFile.LastIndexOf("\\") + 1) + ")";
      this._zFile = zFile;
      this._bInitialized = true;
    }

    protected void _Changed()
    {
      if (!this._bInitialized || this._bRequestSave)
        return;
      this.Text = this.Text + " *";
      this._bRequestSave = true;
    }

    protected void _OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (sender != this.TravelList)
        return;
      this._Changed();
    }

    private void _OnClick(object sender, EventArgs e)
    {
      if (sender == this.ButtonAdd)
      {
        if (Game.Player != null)
        {
          Vector3D position = Game.Player.GetPosition();
          this.Add((string) null, position.X, position.Y, position.Z, Game.Player.IsFlying(), "Move", (string) null);
          this._Changed();
        }
      }
      else if (sender == this.ButtonDelete)
      {
        if (this.TravelList.SelectedRows != null)
        {
          this.TravelList.Rows.RemoveAt(this.TravelList.SelectedRows[0].Index);
          this._Changed();
        }
      }
      else if (sender == this.ButtonDown)
      {
        if (this.TravelList.SelectedRows != null && this.TravelList.SelectedRows[0].Index < this.TravelList.Rows.Count - 1)
        {
          DataGridViewRow dataGridViewRow = this.TravelList.SelectedRows[0];
          int count = this.TravelList.Rows.Count;
          int index = dataGridViewRow.Index;
          this.TravelList.Rows.RemoveAt(index);
          this.TravelList.Rows.Insert(index + 1, dataGridViewRow);
          dataGridViewRow.Selected = true;
          this._Changed();
        }
      }
      else if (sender == this.ButtonRecord)
      {
        if (!this._hTravelRecord.Running())
        {
          this.ButtonRecord.Text = "Stop";
          this._hTravelRecord.Start();
        }
        else
        {
          this.ButtonRecord.Text = "Record";
          this._hTravelRecord.Stop();
        }
      }
      else if (sender == this.ButtonRun)
      {
        if (this.TravelList.SelectedRows != null && Game.Player != null)
        {
          AionInterface.TravelList travelList = new AionInterface.TravelList((string) null);
          for (int index = 0; index < this.TravelList.Rows.Count; ++index)
          {
            float X = float.Parse((string) this.TravelList.Rows[index].Cells[1].Value);
            float Y = float.Parse((string) this.TravelList.Rows[index].Cells[2].Value);
            float Z = float.Parse((string) this.TravelList.Rows[index].Cells[3].Value);
            bool bFlying = bool.Parse((string) this.TravelList.Rows[index].Cells[4].Value);
            travelList.Modify(new Travel((string) null, new Vector3D(X, Y, Z), bFlying, (string) null, (string) null), -1);
          }
          for (int index = 0; index < this.TravelList.SelectedRows[0].Index; ++index)
            travelList.GetNext();
          travelList.Move();
        }
      }
      else if (sender == this.ButtonUp && this.TravelList.SelectedRows != null && this.TravelList.SelectedRows[0].Index > 0)
      {
        DataGridViewRow dataGridViewRow = this.TravelList.SelectedRows[0];
        int index = dataGridViewRow.Index;
        this.TravelList.Rows.RemoveAt(index);
        this.TravelList.Rows.Insert(index - 1, dataGridViewRow);
        dataGridViewRow.Selected = true;
        this._Changed();
      }
      this.Label.Focus();
    }

    private void _OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      if (sender != this.TravelList || this.TravelList.CurrentCell.ColumnIndex < 1 || this.TravelList.CurrentCell.ColumnIndex > 3)
        return;
      TextBox textBox = e.Control as TextBox;
      if (textBox == null)
        return;
      textBox.KeyPress += new KeyPressEventHandler(this._OnKeyPress);
    }

    protected void _OnFormClosing(object sender, FormClosingEventArgs e)
    {
      if (this._bRequestSave)
      {
        switch (MessageBox.Show("Do you wish to save the changes you made?", this._zFile.Substring(this._zFile.IndexOf("\\") + 1), this._bClosing ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk))
        {
          case DialogResult.Cancel:
            e.Cancel = true;
            break;
          case DialogResult.Yes:
            this.Save();
            break;
        }
      }
      if (e.Cancel)
        return;
      if (this._hTravelRecord.Running())
        this._hTravelRecord.Stop();
      Program.Manager.NodeClose(this._zFile);
    }

    protected void _OnKeyPress(object sender, KeyPressEventArgs e)
    {
      TextBox textBox = (TextBox) sender;
      if (char.IsNumber(e.KeyChar) || (int) e.KeyChar == 8 || !textBox.Text.Contains(".") && (int) e.KeyChar == 46)
        return;
      e.Handled = true;
    }

    private void _OnSelectionChanged(object sender, EventArgs e)
    {
      if (sender != this.TravelList)
        return;
      if (this.TravelList.SelectedRows.Count == 0)
      {
        this.ButtonDown.Enabled = false;
        this.ButtonDelete.Enabled = false;
        this.ButtonUp.Enabled = false;
        this.ButtonRun.Enabled = false;
      }
      else
      {
        this.ButtonDown.Enabled = this.TravelList.SelectedRows.Count != 0 && this.TravelList.SelectedRows[0].Index < this.TravelList.Rows.Count - 1;
        this.ButtonDelete.Enabled = true;
        this.ButtonUp.Enabled = this.TravelList.SelectedRows.Count != 0 && this.TravelList.SelectedRows[0].Index > 0;
        this.ButtonRun.Enabled = true;
      }
    }

    private void _OnShown(object sender, EventArgs e)
    {
      if (sender != this)
        return;
      this.Label.Focus();
    }

    public void Add(string zName, float float_0, float float_1, float float_2, bool bFlying, string zType, string zParam)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Delegate) new ITravel.DelegateAdd(this.Add), (object) zName, (object) float_0, (object) float_1, (object) float_2, (object) (bool) (bFlying ? true : false), (object) zType, (object) zParam);
      }
      else
      {
        int index = this.TravelList.Rows.Add();
        this.TravelList.Rows[index].Cells[0].Value = zName == null || zName.Length < 1 ? (object) "No name" : (object) zName;
        this.TravelList.Rows[index].Cells[1].Value = (object) float_0.ToString("0.00");
        this.TravelList.Rows[index].Cells[2].Value = (object) float_1.ToString("0.00");
        this.TravelList.Rows[index].Cells[3].Value = (object) float_2.ToString("0.00");
        this.TravelList.Rows[index].Cells[4].Value = bFlying ? (object) "True" : (object) "False";
        this.TravelList.Rows[index].Cells[5].Value = zType == null ? (object) "Move" : (object) zType;
        this.TravelList.Rows[index].Cells[6].Value = (object) zParam;
      }
    }

    public new void Close()
    {
      this._bClosing = true;
      base.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle4 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle5 = new DataGridViewCellStyle();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ITravel));
      this.ButtonAdd = new Button();
      this.ButtonDelete = new Button();
      this.ButtonRun = new Button();
      this.PanelUpper = new Panel();
      this.Label = new Label();
      this.PanelLower = new Panel();
      this.ButtonRecord = new Button();
      this.ButtonUp = new Button();
      this.ButtonDown = new Button();
      this.TravelTypePanel = new Panel();
      this.TravelType = new ComboBox();
      this.TravelListPanel = new Panel();
      this.TravelList = new MyDataGridView();
      this.TravelListName = new DataGridViewTextBoxColumn();
      this.TravelListX = new DataGridViewTextBoxColumn();
      this.TravelListY = new DataGridViewTextBoxColumn();
      this.TravelListZ = new DataGridViewTextBoxColumn();
      this.TravelListFlying = new DataGridViewCheckBoxColumn();
      this.TravelListType = new DataGridViewComboBoxColumn();
      this.TravelListParam = new DataGridViewTextBoxColumn();
      this.PanelUpper.SuspendLayout();
      this.PanelLower.SuspendLayout();
      this.TravelTypePanel.SuspendLayout();
      this.TravelListPanel.SuspendLayout();
      //this.TravelList.BeginInit();
      this.SuspendLayout();
      this.ButtonAdd.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonAdd.FlatStyle = FlatStyle.Flat;
      this.ButtonAdd.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonAdd.Location = new Point(184, 28);
      this.ButtonAdd.Name = "ButtonAdd";
      this.ButtonAdd.Size = new Size(56, 24);
      this.ButtonAdd.TabIndex = 0;
      this.ButtonAdd.Text = "Add";
      this.ButtonAdd.UseVisualStyleBackColor = true;
      this.ButtonAdd.Click += new EventHandler(this._OnClick);
      this.ButtonDelete.Enabled = false;
      this.ButtonDelete.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonDelete.FlatStyle = FlatStyle.Flat;
      this.ButtonDelete.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonDelete.Location = new Point(368, 28);
      this.ButtonDelete.Name = "ButtonDelete";
      this.ButtonDelete.Size = new Size(28, 24);
      this.ButtonDelete.TabIndex = 3;
      this.ButtonDelete.Text = "X";
      this.ButtonDelete.UseVisualStyleBackColor = true;
      this.ButtonDelete.Click += new EventHandler(this._OnClick);
      this.ButtonRun.Enabled = false;
      this.ButtonRun.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonRun.FlatStyle = FlatStyle.Flat;
      this.ButtonRun.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonRun.Location = new Point(244, 28);
      this.ButtonRun.Name = "ButtonRun";
      this.ButtonRun.Size = new Size(56, 24);
      this.ButtonRun.TabIndex = 9;
      this.ButtonRun.Text = "Run";
      this.ButtonRun.UseVisualStyleBackColor = true;
      this.ButtonRun.Click += new EventHandler(this._OnClick);
      this.PanelUpper.BackgroundImage = (Image) Resources.BackgroundUpper;
      this.PanelUpper.Controls.Add((Control) this.Label);
      this.PanelUpper.Dock = DockStyle.Top;
      this.PanelUpper.Location = new Point(0, 0);
      this.PanelUpper.Name = "PanelUpper";
      this.PanelUpper.Size = new Size(402, 60);
      this.PanelUpper.TabIndex = 12;
      this.Label.AutoSize = true;
      this.Label.BackColor = Color.Transparent;
      this.Label.Font = new Font("Segoe UI", 15.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Label.Location = new Point(4, 8);
      this.Label.Name = "Label";
      this.Label.Size = new Size(126, 30);
      this.Label.TabIndex = 5;
      this.Label.Text = "Travel Editor";
      this.PanelLower.BackgroundImage = (Image) Resources.BackgroundBottom;
      this.PanelLower.Controls.Add((Control) this.ButtonRecord);
      this.PanelLower.Controls.Add((Control) this.ButtonUp);
      this.PanelLower.Controls.Add((Control) this.ButtonDown);
      this.PanelLower.Controls.Add((Control) this.TravelTypePanel);
      this.PanelLower.Controls.Add((Control) this.ButtonAdd);
      this.PanelLower.Controls.Add((Control) this.ButtonRun);
      this.PanelLower.Controls.Add((Control) this.ButtonDelete);
      this.PanelLower.Dock = DockStyle.Bottom;
      this.PanelLower.Location = new Point(0, 196);
      this.PanelLower.Name = "PanelLower";
      this.PanelLower.Size = new Size(402, 60);
      this.PanelLower.TabIndex = 11;
      this.ButtonRecord.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonRecord.FlatStyle = FlatStyle.Flat;
      this.ButtonRecord.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonRecord.Location = new Point(8, 28);
      this.ButtonRecord.Name = "ButtonRecord";
      this.ButtonRecord.Size = new Size(56, 24);
      this.ButtonRecord.TabIndex = 17;
      this.ButtonRecord.Text = "Record";
      this.ButtonRecord.UseVisualStyleBackColor = true;
      this.ButtonRecord.Click += new EventHandler(this._OnClick);
      this.ButtonUp.Enabled = false;
      this.ButtonUp.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonUp.FlatStyle = FlatStyle.Flat;
      this.ButtonUp.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonUp.Location = new Point(304, 28);
      this.ButtonUp.Name = "ButtonUp";
      this.ButtonUp.Size = new Size(28, 24);
      this.ButtonUp.TabIndex = 12;
      this.ButtonUp.Text = "↑";
      this.ButtonUp.UseVisualStyleBackColor = true;
      this.ButtonUp.Click += new EventHandler(this._OnClick);
      this.ButtonDown.Enabled = false;
      this.ButtonDown.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
      this.ButtonDown.FlatStyle = FlatStyle.Flat;
      this.ButtonDown.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ButtonDown.Location = new Point(336, 28);
      this.ButtonDown.Name = "ButtonDown";
      this.ButtonDown.Size = new Size(29, 24);
      this.ButtonDown.TabIndex = 16;
      this.ButtonDown.Text = "↓";
      this.ButtonDown.UseVisualStyleBackColor = true;
      this.ButtonDown.Click += new EventHandler(this._OnClick);
      this.TravelTypePanel.BorderStyle = BorderStyle.FixedSingle;
      this.TravelTypePanel.Controls.Add((Control) this.TravelType);
      this.TravelTypePanel.Location = new Point(68, 28);
      this.TravelTypePanel.Name = "TravelTypePanel";
      this.TravelTypePanel.Size = new Size(112, 24);
      this.TravelTypePanel.TabIndex = 11;
      this.TravelType.Dock = DockStyle.Fill;
      this.TravelType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.TravelType.FlatStyle = FlatStyle.Flat;
      this.TravelType.FormattingEnabled = true;
      this.TravelType.Items.AddRange(new object[2]
      {
        (object) "Circular Travel Path",
        (object) "Reversed Travel Path"
      });
      this.TravelType.Location = new Point(0, 0);
      this.TravelType.Name = "TravelType";
      this.TravelType.Size = new Size(110, 21);
      this.TravelType.TabIndex = 10;
      this.TravelListPanel.BorderStyle = BorderStyle.FixedSingle;
      this.TravelListPanel.Controls.Add((Control) this.TravelList);
      this.TravelListPanel.Location = new Point(8, 40);
      this.TravelListPanel.Name = "TravelListPanel";
      this.TravelListPanel.Size = new Size(388, 178);
      this.TravelListPanel.TabIndex = 15;
      this.TravelList.AllowUserToAddRows = false;
      this.TravelList.AllowUserToDeleteRows = false;
      this.TravelList.AllowUserToResizeColumns = false;
      this.TravelList.AllowUserToResizeRows = false;
      this.TravelList.BackgroundColor = SystemColors.Window;
      this.TravelList.BorderStyle = BorderStyle.None;
      this.TravelList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
      this.TravelList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.TravelList.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.TravelList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TravelList.Columns.AddRange((DataGridViewColumn) this.TravelListName, (DataGridViewColumn) this.TravelListX, (DataGridViewColumn) this.TravelListY, (DataGridViewColumn) this.TravelListZ, (DataGridViewColumn) this.TravelListFlying, (DataGridViewColumn) this.TravelListType, (DataGridViewColumn) this.TravelListParam);
      this.TravelList.Dock = DockStyle.Fill;
      this.TravelList.Location = new Point(0, 0);
      this.TravelList.MultiSelect = false;
      this.TravelList.Name = "TravelList";
      this.TravelList.RowHeadersVisible = false;
      this.TravelList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(224, 224, 224);
      gridViewCellStyle2.SelectionForeColor = Color.Black;
      this.TravelList.RowsDefaultCellStyle = gridViewCellStyle2;
      this.TravelList.ScrollBars = ScrollBars.Vertical;
      this.TravelList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.TravelList.Size = new Size(386, 176);
      this.TravelList.TabIndex = 14;
      this.TravelList.CellValueChanged += new DataGridViewCellEventHandler(this._OnCellValueChanged);
      this.TravelList.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(this._OnEditingControlShowing);
      this.TravelList.SelectionChanged += new EventHandler(this._OnSelectionChanged);
      this.TravelListName.HeaderText = "Name";
      this.TravelListName.Name = "TravelListName";
      this.TravelListName.Resizable = DataGridViewTriState.False;
      this.TravelListName.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.TravelListName.Width = 60;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.TravelListX.DefaultCellStyle = gridViewCellStyle3;
      this.TravelListX.HeaderText = "X";
      this.TravelListX.Name = "TravelListX";
      this.TravelListX.Resizable = DataGridViewTriState.False;
      this.TravelListX.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.TravelListX.Width = 50;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.TravelListY.DefaultCellStyle = gridViewCellStyle4;
      this.TravelListY.HeaderText = "Y";
      this.TravelListY.Name = "TravelListY";
      this.TravelListY.Resizable = DataGridViewTriState.False;
      this.TravelListY.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.TravelListY.Width = 50;
      gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.TravelListZ.DefaultCellStyle = gridViewCellStyle5;
      this.TravelListZ.HeaderText = "Z";
      this.TravelListZ.Name = "TravelListZ";
      this.TravelListZ.Resizable = DataGridViewTriState.False;
      this.TravelListZ.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.TravelListZ.Width = 50;
      this.TravelListFlying.FalseValue = (object) "False";
      this.TravelListFlying.HeaderText = "Fly";
      this.TravelListFlying.Name = "TravelListFlying";
      this.TravelListFlying.Resizable = DataGridViewTriState.False;
      this.TravelListFlying.TrueValue = (object) "True";
      this.TravelListFlying.Width = 35;
      this.TravelListType.HeaderText = "Type";
      this.TravelListType.Items.AddRange((object) "Action", (object) "Move", (object) "Rest");
      this.TravelListType.Name = "TravelListType";
      this.TravelListType.Resizable = DataGridViewTriState.False;
      this.TravelListType.Width = 60;
      this.TravelListParam.HeaderText = "Param";
      this.TravelListParam.Name = "TravelListParam";
      this.TravelListParam.Width = 60;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(402, 256);
      this.Controls.Add((Control) this.TravelListPanel);
      this.Controls.Add((Control) this.PanelUpper);
      this.Controls.Add((Control) this.PanelLower);
      this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = "ITravel";
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Travel Editor";
      this.FormClosing += new FormClosingEventHandler(this._OnFormClosing);
      this.Shown += new EventHandler(this._OnShown);
      this.PanelUpper.ResumeLayout(false);
      this.PanelUpper.PerformLayout();
      this.PanelLower.ResumeLayout(false);
      this.TravelTypePanel.ResumeLayout(false);
      this.TravelListPanel.ResumeLayout(false);
      //this.TravelList.EndInit();
      this.ResumeLayout(false);
    }

    public bool Save()
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("TravelList");
      element1.SetAttribute("Reverse", this.TravelType.SelectedIndex == 0 ? "False" : "True");
      for (int index = 0; index < this.TravelList.Rows.Count; ++index)
      {
        XmlElement element2 = xmlDocument.CreateElement("Travel");
        XmlElement element3 = xmlDocument.CreateElement("Name");
        XmlElement element4 = xmlDocument.CreateElement("X");
        XmlElement element5 = xmlDocument.CreateElement("Y");
        XmlElement element6 = xmlDocument.CreateElement("Z");
        XmlElement element7 = xmlDocument.CreateElement("Flying");
        XmlElement element8 = xmlDocument.CreateElement("Type");
        XmlElement element9 = xmlDocument.CreateElement("Param");
        element3.InnerText = (string) this.TravelList.Rows[index].Cells[0].Value;
        float num1 = float.Parse((string) this.TravelList.Rows[index].Cells[1].Value);
        element4.InnerText = num1.ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture);
        float num2 = float.Parse((string) this.TravelList.Rows[index].Cells[2].Value);
        element5.InnerText = num2.ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture);
        float num3 = float.Parse((string) this.TravelList.Rows[index].Cells[3].Value);
        element6.InnerText = num3.ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture);
        element7.InnerText = (string) this.TravelList.Rows[index].Cells[4].Value;
        element8.InnerText = (string) this.TravelList.Rows[index].Cells[5].Value;
        element9.InnerText = (string) this.TravelList.Rows[index].Cells[6].Value;
        element2.AppendChild((XmlNode) element3);
        element2.AppendChild((XmlNode) element4);
        element2.AppendChild((XmlNode) element5);
        element2.AppendChild((XmlNode) element6);
        element2.AppendChild((XmlNode) element7);
        element2.AppendChild((XmlNode) element8);
        element2.AppendChild((XmlNode) element9);
        element1.AppendChild((XmlNode) element2);
      }
      xmlDocument.AppendChild((XmlNode) element1);
      xmlDocument.Save(this._zFile);
      if (this.Text.EndsWith("*"))
      {
        this.Text = this.Text.Substring(0, this.Text.Length - 2);
        this._bRequestSave = false;
      }
      return true;
    }

    protected delegate void DelegateAdd(string zName, float float_0, float float_1, float float_2, bool bFlying, string zType, string zParam);

    protected class TravelRecorder
    {
      protected bool _bFlying;
      protected Thread _hThread;
      protected ITravel _hTravel;
      protected Vector3D _hPosition;

      public TravelRecorder(ITravel hTravel)
      {
        this._hTravel = hTravel;
      }

      protected void _Active()
      {
        this._hPosition = (Vector3D) null;
        while (true)
        {
          while (Game.Player == null)
            Thread.Sleep(250);
          if (this._hPosition == null || this._bFlying != Game.Player.IsFlying() || Game.Player.GetPosition().DistanceToPosition(this._hPosition, 0.0) >= 5.0)
          {
            this._hPosition = Game.Player.GetPosition();
            this._bFlying = Game.Player.IsFlying();
            this._hTravel.Add((string) null, this._hPosition.X, this._hPosition.Y, this._hPosition.Z, Game.Player.IsFlying(), "Action", (string) null);
          }
          Thread.Sleep(250);
        }
      }

      public bool Running()
      {
        return this._hThread != null;
      }

      public bool Start()
      {
        if (this._hThread != null)
          return false;
        this._hThread = new Thread(new ThreadStart(this._Active));
        this._hThread.Start();
        return true;
      }

      public bool Stop()
      {
        if (this._hThread == null)
          return false;
        this._hThread.Abort();
        this._hThread = (Thread) null;
        return true;
      }
    }
  }
}
