// Decompiled with JetBrains decompiler
// Type: AionScript.Modelling.Scripting.VirtualMachine
// Assembly: AionScript, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F0A67B5-C555-4E0D-A1E1-0CB1EB82850F
// Assembly location: C:\Users\rafaelgb\Downloads\aionscript\AionScript.exe

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace AionScript.Modelling.Scripting
{
  public abstract class VirtualMachine : DynamicObject, IDisposable
  {
    private Dictionary<string, object> _hVirtualMachineEngineEvent = new Dictionary<string, object>();
    private object[] _hObjectList;
    private Stream _hStream;
    private VirtualMachineEngine _hVirtualMachineEngine;

    public VirtualMachineEngine Engine
    {
      get
      {
        return this._hVirtualMachineEngine;
      }
      set
      {
        using (StreamReader streamReader = new StreamReader(this._hStream))
        {
          this._hVirtualMachineEngine = value;
          this._hVirtualMachineEngine.InitializeMachine(streamReader.ReadToEnd());
          if (this._hObjectList != null)
          {
            foreach (object obj in this._hObjectList)
            {
              if (obj is string)
                this.Include((string) obj);
              else if (obj is Stream)
                this.Include((Stream) obj);
            }
          }
          foreach (VirtualMachineEngineEvent machineEngineEvent in Enum.GetValues(typeof (VirtualMachineEngineEvent)))
          {
            object function = this._hVirtualMachineEngine.GetFunction(machineEngineEvent.ToString());
            if (function != null)
              this._hVirtualMachineEngineEvent[machineEngineEvent.ToString()] = function;
          }
        }
      }
    }

    public object this[string zName]
    {
      get
      {
        return this._hVirtualMachineEngine.GetVariable(zName);
      }
      set
      {
        this._hVirtualMachineEngine.SetVariable(zName, value);
      }
    }

    public VirtualMachine(Stream hStream, object[] hObjectList = null)
    {
      this._hObjectList = hObjectList;
      this._hStream = hStream;
    }

    public virtual void Dispose()
    {
    }

    public virtual object Include(Stream hStream)
    {
      using (StreamReader streamReader = new StreamReader(hStream))
        return this._hVirtualMachineEngine.InitializeInclude(streamReader.ReadToEnd());
    }

    public virtual object Include(string zFile)
    {
      if (File.Exists(zFile))
        return this.Include((Stream) File.Open(zFile, FileMode.Open, FileAccess.Read, FileShare.Read));
      return (object) null;
    }

    public object Raise(VirtualMachineEngineEvent hLuaMachineEvent)
    {
      if (this._hVirtualMachineEngineEvent.ContainsKey(hLuaMachineEvent.ToString()))
        return this._hVirtualMachineEngine.SetFunction(this._hVirtualMachineEngineEvent[hLuaMachineEvent.ToString()], (object[]) null);
      return (object) null;
    }

    public object Raise(string zFunction, object[] hArgumentList = null)
    {
      if (!this._hVirtualMachineEngineEvent.ContainsKey(zFunction))
        this._hVirtualMachineEngineEvent[zFunction] = this._hVirtualMachineEngine.GetFunction(zFunction);
      if (this._hVirtualMachineEngineEvent[zFunction] != null)
        return this._hVirtualMachineEngine.SetFunction(this._hVirtualMachineEngineEvent[zFunction], hArgumentList);
      return (object) null;
    }

    public override bool TryGetMember(GetMemberBinder hGetMemberBinder, out object hResult)
    {
      hResult = this[hGetMemberBinder.Name];
      return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder hInvokeMemberBinder, object[] hArgumentList, out object hResult)
    {
      hResult = this.Raise(hInvokeMemberBinder.Name, hArgumentList);
      return true;
    }

    public override bool TrySetMember(SetMemberBinder hSetMemberBinder, object hValue)
    {
      this[hSetMemberBinder.Name] = hValue;
      return true;
    }
  }
}
