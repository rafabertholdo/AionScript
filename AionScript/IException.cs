// Decompiled with JetBrains decompiler
// Type: AionScript.IException
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AionScript
{
  public class IException : Form
  {
    private IContainer components;
    private TextBox Exception;
    private Label label1;
    private Panel Panel;

    public IException(string zException)
    {
      this.InitializeComponent();
      this.Exception.Text = zException;
      this.Exception.SelectAll();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.Exception = new TextBox();
      this.Panel = new Panel();
      this.label1 = new Label();
      this.Panel.SuspendLayout();
      this.SuspendLayout();
      this.Exception.BorderStyle = BorderStyle.None;
      this.Exception.Cursor = Cursors.Arrow;
      this.Exception.Dock = DockStyle.Fill;
      this.Exception.Location = new Point(0, 0);
      this.Exception.Multiline = true;
      this.Exception.Name = "Exception";
      this.Exception.ScrollBars = ScrollBars.Both;
      this.Exception.Size = new Size(306, 418);
      this.Exception.TabIndex = 0;
      this.Panel.BorderStyle = BorderStyle.FixedSingle;
      this.Panel.Controls.Add((Control) this.Exception);
      this.Panel.Location = new Point(4, 32);
      this.Panel.Name = "Panel";
      this.Panel.Size = new Size(308, 420);
      this.Panel.TabIndex = 1;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Calibri", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(8, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(293, 15);
      this.label1.TabIndex = 2;
      this.label1.Text = "Please post this error log in the 'Bug Report' section!";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.ClientSize = new Size(316, 456);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.Panel);
      this.Font = new Font("Calibri", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = "IException";
      this.Text = "Oops! An error occured..";
      this.Panel.ResumeLayout(false);
      this.Panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
