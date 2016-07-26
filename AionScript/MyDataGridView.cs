// Decompiled with JetBrains decompiler
// Type: AionScript.MyDataGridView
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace AionScript
{
  public class MyDataGridView : DataGridView
  {
    private const int BORDERWIDTH = 2;

    public MyDataGridView()
    {
      this.VerticalScrollBar.Visible = true;
      this.VerticalScrollBar.VisibleChanged += new EventHandler(this.ShowScrollBars);
    }

    private void ShowScrollBars(object sender, EventArgs e)
    {
      if (this.VerticalScrollBar.Visible)
        return;
      int width = this.VerticalScrollBar.Width;
      this.VerticalScrollBar.Location = new Point(this.ClientRectangle.Width - width - 2, this.ColumnHeadersHeight);
      this.VerticalScrollBar.Size = new Size(width, this.ClientRectangle.Height - this.ColumnHeadersHeight - 2);
      this.VerticalScrollBar.VisibleChanged -= new EventHandler(this.ShowScrollBars);
      this.VerticalScrollBar.Show();
      this.VerticalScrollBar.VisibleChanged += new EventHandler(this.ShowScrollBars);
    }
  }
}
