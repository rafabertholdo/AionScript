// Decompiled with JetBrains decompiler
// Type: AionScript.IDialogNode
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using AionInterface;
using System.Windows.Forms;

namespace AionScript
{
  public class IDialogNode : TreeNode
  {
    protected Dialog _hDialog;

    public IDialogNode(Dialog hDialog)
    {
      this.Dialog(hDialog);
    }

    protected string _Validate(string zName)
    {
      if (zName != null && zName.Length != 0)
        return zName;
      return "[Nameless]";
    }

    public void CollapseAll()
    {
      this.Collapse();
      if (this.Parent == null)
        return;
      this.Parent.Collapse();
    }

    public Dialog Dialog(Dialog hDialog = null)
    {
      if (hDialog != null)
      {
        this.Text = this._Validate(hDialog.GetName());
        this._hDialog = hDialog;
      }
      return this._hDialog;
    }

    public void Update(bool bFullUpdate = true, bool bForceParentUpdate = false)
    {
      if (this._hDialog == null)
        return;
      if (bFullUpdate | bForceParentUpdate)
      {
        if (this.Parent != null)
          this._hDialog = ((IDialogNode) this.Parent).Dialog((Dialog) null).GetDialog((uint) this.Index, true);
        else
          this._hDialog.Update(true);
        if (this._hDialog == null)
          return;
        if (!bForceParentUpdate && this.Parent != null)
          ((IDialogNode) this.Parent).Update(false, true);
        this.Text = this._Validate(this._hDialog.GetName()) + (object) " (" + (string) (object) this._hDialog.GetPosition().X + ", " + (string) (object) this._hDialog.GetPosition().Y + (this._hDialog.GetName().Contains("html") ? ", #" + (object) this._hDialog.GetHTML() : (string) null) + ")";
        for (int index = 0; (long) index < (long) this._hDialog.GetDialogSize(); ++index)
        {
          Dialog dialog = this._hDialog.GetDialog((uint) index, false);
          if (this.Nodes.Count > index)
            ((IDialogNode) this.Nodes[index]).Dialog(dialog);
          else
            this.Nodes.Add((TreeNode) new IDialogNode(dialog));
        }
        for (int index = this.Nodes.Count - 1; (long) index >= (long) this._hDialog.GetDialogSize(); --index)
          this.Nodes.RemoveAt(index);
      }
      if (bFullUpdate)
        return;
      if (this.Parent != null)
        ((IDialogNode) this.Parent).Update(false, false);
      this.Dialog(this._hDialog);
    }
  }
}
