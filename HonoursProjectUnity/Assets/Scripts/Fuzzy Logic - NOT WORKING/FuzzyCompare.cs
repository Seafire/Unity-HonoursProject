/*

using System;

namespace FuzzyLogic
{

	public static class FuzzyCompare
	{
		public const float MaxFloatULP = 2.0282409603651670423947251286016E+31f;
		public const double MaxDoubleULP = 1.9958403095347198116563727130368E+292;

		public static bool AreEqual(double first, double second, double marginOfError, int ulpTolerance)
		{
			// When numbers are very close to zero, this initial check of absolute values is needed. Otherwise 
			// we can safely use the ULP difference.
			double absoluteDiff = Math.Abs(first - second);

			if (absoluteDiff <= marginOfError)
			{
				return true;
			}

			DoubleInfo firstInfo = new DoubleInfo(first);
			DoubleInfo secondInfo = new DoubleInfo(second);

			// Different signs mean the numbers don't match, period.
			if (firstInfo.IsNegative != secondInfo.IsNegative)
			{
				return false;
			}

			// Find the difference in ULPs (unit of least precision).
			long ulpDiff = Math.Abs(firstInfo.Bits - secondInfo.Bits);

			if (ulpDiff <= ulpTolerance)
			{
				return true;
			}

			return false;
		}


		public static int Compare(double first, double second, double marginOfError, int ulpTolerance)
		{
			if (AreEqual(first, second, marginOfError, ulpTolerance))
			{
				return 0;
			}
			else
			{
				return first < second ? -1 : 1;
			}
		}

		public static bool AreEqual(float first, float second, float marginOfError, int ulpTolerance)
		{
			// When numbers are very close to zero, this initial check of absolute values is needed. Otherwise 
			// we can safely use the ULP difference.
			float absoluteDiff = Math.Abs(first - second);

			if (absoluteDiff <= marginOfError)
			{
				return true;
			}

			SingleInfo firstInfo = new SingleInfo(first);
			SingleInfo secondInfo = new SingleInfo(second);

			// Different signs mean the numbers don't match, period.
			if (firstInfo.IsNegative != secondInfo.IsNegative)
			{
				return false;
			}

			// Find the difference in ULPs (unit of least precision).
			int ulpDiff = Math.Abs(firstInfo.Bits - secondInfo.Bits);

			if (ulpDiff <= ulpTolerance)
			{
				return true;
			}

			return false;
		}

		public static int Compare(float first, float second, float marginOfError, int ulpTolerance)
		{
			if (AreEqual(first, second, marginOfError, ulpTolerance))
			{
				return 0;
			}
			else
			{
				return first < second ? -1 : 1;
			}
		}

		public static double ULP(double value)
		{
			if (Double.IsNaN(value))
			{
				return Double.NaN;
			}
			else if (Double.IsPositiveInfinity(value) || Double.IsNegativeInfinity(value))
			{
				return Double.PositiveInfinity;
			}
			else if (value == 0.0)
			{
				return Double.Epsilon;
			}
			else if (Math.Abs(value) == Double.MaxValue)
			{
				return MaxDoubleULP;
			}

			DoubleInfo info = new DoubleInfo(value);
			return Math.Abs((double)(info.Bits + 1) - value);
		}

		public static long ULPDistance(double first, double second)
		{
			DoubleInfo firstInfo = new DoubleInfo(first);
			DoubleInfo secondInfo = new DoubleInfo(second);

			if (firstInfo.IsNegative != secondInfo.IsNegative)
			{
				throw new ArgumentException("Numbers have mixed signs; cannot calculate the ULP distance across the 0 boundary.");
			}

			return Math.Abs(firstInfo.Bits - secondInfo.Bits);
		}

		public static float ULP(float value)
		{
			if (Single.IsNaN(value))
			{
				return Single.NaN;
			}
			else if (Single.IsPositiveInfinity(value) || Single.IsNegativeInfinity(value))
			{
				return Single.PositiveInfinity;
			}
			else if (value == 0.0)
			{
				return Single.Epsilon;
			}
			else if (Math.Abs(value) == Single.MaxValue)
			{
				return MaxFloatULP;
			}

			SingleInfo info = new SingleInfo(value);
			return Math.Abs((float)(info.Bits + 1) - value);
		}

		public static int ULPDistance(float first, float second)
		{
			SingleInfo firstInfo = new SingleInfo(first);
			SingleInfo secondInfo = new SingleInfo(second);

			if (firstInfo.IsNegative != secondInfo.IsNegative)
			{
				throw new ArgumentException("Numbers have mixed signs; cannot calculate the ULP distance across the 0 boundary.");
			}

			return Math.Abs(firstInfo.Bits - secondInfo.Bits);
		}

		public static long RoundToBoundary(double value, long boundary)
		{
			return (new DoubleInfo(value).Bits / boundary) * boundary;
		}

		public static long Boundary(double value, double marginOfError, int ulpTolerance, int boundaryScale)
		{
			DoubleInfo valueInfo = new DoubleInfo(Math.Abs(value));
			DoubleInfo valueWithErrorInfo = new DoubleInfo(Math.Abs(value) + marginOfError);

			long ulpBoundary = ulpTolerance * boundaryScale;
			long marginBoundary = (valueWithErrorInfo.Bits - valueInfo.Bits) * boundaryScale;

			return Math.Max(ulpBoundary, marginBoundary);
		}

		public static bool InSameBoundary(double first, double second, double marginOfError, int ulpTolerance, int boundaryScale)
		{
			long boundary = Boundary(first, marginOfError, ulpTolerance, boundaryScale);

			long firstAlongBoundary = RoundToBoundary(first, boundary);
			long secondAlongBoundary = RoundToBoundary(second, boundary);

			return firstAlongBoundary == secondAlongBoundary;
		}

		public static int RoundToBoundary(float value, int boundary)
		{
			return (new SingleInfo(value).Bits / boundary) * boundary;
		}

		public static int Boundary(float value, float marginOfError, int ulpTolerance, int boundaryScale)
		{
			SingleInfo valueInfo = new SingleInfo(Math.Abs(value));
			SingleInfo valueWithErrorInfo = new SingleInfo(Math.Abs(value) + marginOfError);

			int ulpBoundary = ulpTolerance * boundaryScale;
			int marginBoundary = (valueWithErrorInfo.Bits - valueInfo.Bits) * boundaryScale;

			return Math.Max(ulpBoundary, marginBoundary);
		}

		public static bool InSameBoundary(float first, float second, float marginOfError, int ulpTolerance, int boundaryScale)
		{
			int boundary = Boundary(first, marginOfError, ulpTolerance, boundaryScale);

			int firstAlongBoundary = RoundToBoundary(first, boundary);
			int secondAlongBoundary = RoundToBoundary(second, boundary);

			return firstAlongBoundary == secondAlongBoundary;
		}

		#endregion
	}
}
*/