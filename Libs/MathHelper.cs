// MathHelper.cs
// A math helper utility to generate a few random numbers or so.

using System;

namespace RicaBotpaw.Libs
{
	public class MathHelper
	{
		public static int GetRandomIntegerInRange(Random random, int min, int max)
		{
			return min >= max ? min : random.Next(max - min + 1) + min;
		}
	}
}