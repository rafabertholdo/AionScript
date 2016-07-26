// Decompiled with JetBrains decompiler
// Type: AionScript.Modelling.Scripting.VirtualMachineEngine
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

namespace AionScript.Modelling.Scripting
{
  public interface VirtualMachineEngine
  {
    object GetFunction(string zFunction);

    object GetVariable(string zVariable);

    object InitializeInclude(string zContent);

    void InitializeMachine(string zContent);

    object SetFunction(object hFunction, object[] hArgumentList = null);

    void SetVariable(string zVariable, object hResult);
  }
}
