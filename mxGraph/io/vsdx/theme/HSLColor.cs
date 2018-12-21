using System;

namespace mxGraph.io.vsdx.theme
{

	public class HSLColor
	{
		private double hue, sat, lum;

		public HSLColor(double hue, double sat, double lum)
		{
			this.hue = hue;
			this.sat = sat;
			this.lum = lum;
		}

		public virtual double Hue
		{
			get
			{
				return hue;
			}
			set
			{
				this.hue = value;
			}
		}






		public virtual double Sat
		{
			get
			{
				return sat;
			}
			set
			{
				this.sat = value;
			}
		}






		public virtual double Lum
		{
			get
			{
				return lum;
			}
			set
			{
				this.lum = value;
			}
		}




		private double hue2rgb(double p, double q, double t)
		{
			if (t < 0)
			{
				t += 1;
			}
			if (t > 1)
			{
				t -= 1;
			}
			if (t < 1 / 6.0)
			{
				return p + (q - p) * 6 * t;
			}
			if (t < 0.5)
			{
				return q;
			}
			if (t < 2 / 3.0)
			{
				return p + (q - p) * (2 / 3.0 - t) * 6;
			}
			return p;
		}

		public virtual Color toRgb()
		{
			double r, g, b;

			double h = this.hue;
			double s = this.sat;
			double l = this.lum;


			if (s == 0)
			{
				r = g = b = l; // achromatic
			}
			else
			{
				double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
				double p = 2 * l - q;
				r = hue2rgb(p, q, h + 1 / 3.0);
				g = hue2rgb(p, q, h);
				b = hue2rgb(p, q, h - 1 / 3.0);
			}

			return new Color((int)(r * 255), (int)(g * 255), (int)(b * 255));
		}

		// Force a number between 0 and 1
		private double clamp01(double val)
		{
			return Math.Min(1, Math.Max(0, val));
		}

		//lighten or tint
		public virtual HSLColor tint(int amount)
		{
	//	    HSLColor hsl = color.toHsl();
			this.lum *= (1 + (amount / 100.0));
			this.lum = clamp01(this.lum);
			return this;
		}

		//darken or shade
		public virtual HSLColor shade(int amount)
		{
			this.lum *= amount / 100.0;
			this.lum = clamp01(this.lum);
			return this;
		}

		public virtual HSLColor satMod(int amount)
		{
			this.sat *= amount / 100.0;
			this.sat = clamp01(this.sat);
			return this;
		}

		public virtual HSLColor lumMod(int amount)
		{
			this.lum *= amount / 100.0;
			this.lum = clamp01(this.lum);
			return this;
		}

	}

}