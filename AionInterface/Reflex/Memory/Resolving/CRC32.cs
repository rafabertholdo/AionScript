// Decompiled with JetBrains decompiler
// Type: Reflex.Memory.Resolving.CRC32
// Assembly: AionInterface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B485CA48-C877-4FBB-8075-0A890B655697
// Assembly location: C:\Users\rafaelgb\Downloads\aioninterface.dll

using System;
using System.Collections;
using System.Security.Cryptography;

namespace Reflex.Memory.Resolving
{
  public class CRC32 : HashAlgorithm
  {
    protected static Hashtable _iCachedCRC32TableList = new Hashtable();
    private uint _iCRC32;
    protected uint[] _iCRC32Table;

    public static uint DefaultPolynomial
    {
      get
      {
        return 3988292384;
      }
    }

    public CRC32()
      : this(CRC32.DefaultPolynomial)
    {
    }

    public CRC32(uint iPolynomial)
    {
      this.HashSizeValue = 32;
      if (CRC32._iCachedCRC32TableList.Contains((object) iPolynomial))
      {
        this._iCRC32Table = (uint[]) CRC32._iCachedCRC32TableList[(object) iPolynomial];
      }
      else
      {
        this._iCRC32Table = CRC32._BuildCRC32Table(iPolynomial);
        CRC32._iCachedCRC32TableList.Add((object) iPolynomial, (object) this._iCRC32Table);
      }
      this.Initialize();
    }

    protected static uint[] _BuildCRC32Table(uint iPolynomial)
    {
      uint[] numArray = new uint[256];
      for (uint index1 = 0; index1 < 256U; ++index1)
      {
        numArray[(int) index1] = index1;
        for (uint index2 = 8; index2 > 0U; --index2)
          numArray[(int) index1] = ((int) numArray[(int) index1] & 1) != 1 ? numArray[(int) index1] >> 1 : numArray[(int) index1] >> 1 ^ iPolynomial;
      }
      return numArray;
    }

    protected override void HashCore(byte[] bBuffer, int iOffset, int iCount)
    {
      for (int index = iOffset; index < iCount; ++index)
      {
        ulong num = (ulong) (this._iCRC32 & (uint) byte.MaxValue ^ (uint) bBuffer[index]);
        this._iCRC32 = this._iCRC32 >> 8;
        this._iCRC32 = this._iCRC32 ^ this._iCRC32Table[(int) (IntPtr) ((long) num)];
      }
    }

    protected override byte[] HashFinal()
    {
      ulong num = (ulong) (this._iCRC32 ^ uint.MaxValue);
      return new byte[4]
      {
        (byte) (num >> 24 & (ulong) byte.MaxValue),
        (byte) (num >> 16 & (ulong) byte.MaxValue),
        (byte) (num >> 8 & (ulong) byte.MaxValue),
        (byte) (num & (ulong) byte.MaxValue)
      };
    }

    public override void Initialize()
    {
      this._iCRC32 = uint.MaxValue;
    }
  }
}
