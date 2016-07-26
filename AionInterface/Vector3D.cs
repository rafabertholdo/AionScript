// Decompiled with JetBrains decompiler
// Type: AionInterface.Vector3D
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;

namespace AionInterface
{
  public class Vector3D : ICloneable
  {
    public float X;
    public float Y;
    public float Z;

    public float Pitch
    {
      get
      {
        return this.Y;
      }
      set
      {
        this.Y = value;
      }
    }

    public float Roll
    {
      get
      {
        return this.Z;
      }
      set
      {
        this.Z = value;
      }
    }

    public float Yaw
    {
      get
      {
        return this.X;
      }
      set
      {
        this.X = value;
      }
    }

    public Vector3D(float X = 0.0f, float Y = 0.0f, float Z = 0.0f)
    {
      this.X = X;
      this.Y = Y;
      this.Z = Z;
    }

    public Vector3D CalculateCamera(Vector3D hPosition)
    {
      float num1 = this.Y - hPosition.Y;
      double num2 = (double) this.Z;
      double num3 = (double) hPosition.Z;
      double num4 = (double) this.X - (double) hPosition.X;
      double num5 = Math.Sqrt(num4 * num4 + (double) num1 * (double) num1);
      double num6 = -Math.Atan(((double) hPosition.Z - (double) this.Z) / num5) / Math.PI * 180.0;
      return new Vector3D((float) (Math.Atan2((double) hPosition.Y - (double) this.Y, (double) hPosition.X - (double) this.X) / Math.PI * 180.0 + 90.0), (float) num6, 0.0f);
    }

    public Vector3D CalculatePosition2D(Vector3D hPosition, Vector3D hCamera)
    {
      float num1 = this.X - hPosition.X;
      float num2 = this.Y - hPosition.Y;
      float Z = this.Z - hPosition.Z;
      double d = Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
      if (double.IsNaN(d))
        return new Vector3D(0.0f, 0.0f, Z);
      double num3 = (double) Math.Sign(num2) * Math.Acos((double) num1 / d) - (double) hCamera.X / 180.0 * Math.PI;
      return new Vector3D((float) (-Math.Cos(num3) * d), (float) (Math.Sin(num3) * d), Z);
    }

    public object Clone()
    {
      return (object) new Vector3D(this.X, this.Y, this.Z);
    }

    public double DistanceToPosition(Vector3D hPosition, double dClampDistance = 0.0)
    {
      double num1 = Math.Pow((double) hPosition.Y - (double) this.Y, 2.0);
      double num2 = Math.Pow((double) hPosition.Z - (double) this.Z, 2.0);
      double num3 = Math.Sqrt(Math.Pow((double) hPosition.X - (double) this.X, 2.0) + num1 + num2);
      if (dClampDistance > 0.0 && num3 > dClampDistance)
        return dClampDistance;
      return num3;
    }

    public override bool Equals(object obj)
    {
      if (obj is Vector3D)
      {
        Vector3D vector3D = (Vector3D) obj;
        if ((double) vector3D.X == (double) this.X && (double) vector3D.Y == (double) this.Y && (double) vector3D.Z == (double) this.Z)
          return true;
      }
      return false;
    }
  }
}
