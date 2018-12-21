using System;

namespace mxGraph.io.vsdx.theme
{

	public class Color
	{
		//Special none color
		public static readonly Color NONE = new Color(-1, -1, -1);

		private int red, green, blue;
		private Color gradientClr;

		public Color(int red, int green, int blue)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public virtual int Red
		{
			get
			{
				return red;
			}
			set
			{
				this.red = value;
			}
		}


		public virtual int Green
		{
			get
			{
				return green;
			}
			set
			{
				this.green = value;
			}
		}


		public virtual int Blue
		{
			get
			{
				return blue;
			}
			set
			{
				this.blue = value;
			}
		}


		public virtual HSLColor toHsl()
		{
			double r = this.Red / 255.0, g = this.Green / 255.0, b = this.Blue / 255.0;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			double l = (max + min) / 2.0;
			double h, s;

			if (max == min)
			{
				h = s = 0; // achromatic
			}
			else
			{
				double d = max - min;
				s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
				if (max == r)
				{
					h = (g - b) / d + (g < b ? 6 : 0);
				}
				else if (max == g)
				{
					h = (b - r) / d + 2;
				}
				else
				{
					h = (r - g) / d + 4;
				}

				h /= 6;
			}
			return new HSLColor(h, s, l);
		}

		public virtual HSVColor toHsv()
		{
			double r = this.Red / 255.0, g = this.Green / 255.0, b = this.Blue / 255.0;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			double h , s , v = max;

			double d = max - min;
			s = max == 0 ? 0 : d / max;

			if (max == min)
			{
				h = 0; // achromatic
			}
			else
			{
				if (max == r)
				{
					h = (g - b) / d + (g < b ? 6 : 0);
				}
				else if (max == g)
				{
					h = (b - r) / d + 2;
				}
				else
				{
					h = (r - g) / d + 4;
				}
				h /= 6;
			}
			return new HSVColor(h, s, v);
		}

		public static Color decodeColorHex(string hex)
		{
            int color = Convert.ToInt32(hex,16);// int.Parse(hex, 16);
			return new Color((color >> 16) & 0xff, (color >> 8) & 0xff, color & 0xff);
		}

		public virtual string toHexStr()
		{
			return string.Format("#{0:x2}{1:x2}{2:x2}", red, green, blue);
		}

		public virtual Color GradientClr
		{
			get
			{
				return gradientClr;
			}
			set
			{
				this.gradientClr = value;
			}
		}

	}

}