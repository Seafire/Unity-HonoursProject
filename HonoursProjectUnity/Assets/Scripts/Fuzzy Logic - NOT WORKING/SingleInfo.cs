/*

using System;
using System.Runtime.InteropServices;

namespace FuzzyLogic
{

	public struct SingleInfo
	{
		private float _value;
		private int _bits;

		public SingleInfo(float value)
		{
			_value = value;
			_bits = SingleInfo.SingleToInt32Bits(value);
		}

		public float Value
		{
			get { return _value; }
		}

		public int Bits
		{
			get { return _bits; }
		}

		public bool IsNegative
		{
			get { return (_bits < 0); }
		}

		public int Exponent
		{
			get { return (int)((_bits >> 23) & 0xff); }
		}

		public int Mantissa
		{
			get { return _bits & 0x7fffff; }
		}

		public static int SingleToInt32Bits(float value)
		{
			// This allows us to perform the same operations as BitConverter, but without allocating a new array of bytes.
			return new SingleUnion(value).IntValue;
		}
	}
}

*/