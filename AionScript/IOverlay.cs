// Decompiled with JetBrains decompiler
// Type: AionScript.IOverlay
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AionScript
{
  public class IOverlay : Form
  {
    private IContainer components;

    public IOverlay()
    {
      this.InitializeComponent();
      this.Text = Program.CurrentName;
      this.Height = SystemInformation.PrimaryMonitorSize.Height;
      this.Width = SystemInformation.PrimaryMonitorSize.Width;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.DarkGreen;
      this.ClientSize = new Size(640, 505);
      this.FormBorderStyle = FormBorderStyle.None;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "IOverlay";
      this.ShowInTaskbar = false;
      this.TopMost = true;
      this.TransparencyKey = Color.DarkGreen;
      this.WindowState = FormWindowState.Maximized;
      this.FormClosing += new FormClosingEventHandler(this.Overlay_FormClosing);
    }

    private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = true;
    }
  }
}
