using System;

namespace Raytracing.Maths
{
	public class MathHelpers
	{
		public static double Scale(double value, double min, double max, double minScaled, double maxScaled)
		{
			double scaled = minScaled + (maxScaled - minScaled) * ((value - min) / (max - min));
			return scaled;
		}

		public static double DegreesToRadians(double degrees)
		{
			return degrees * Math.PI / 180.0;
		}
	}
}