namespace mxGraph.io.vsdx.theme
{

	public abstract class OoxmlColor
	{
	//	a:tint    Tint
		private int tint = 0;
	//	a:shade    Shade
		private int shade = 0;
	//	a:comp    Complement
		private int comp = 0;
	//	a:inv    Inverse
		private int inv = 0;
	//	a:gray    Gray
		private int gray = 0;
	//	a:alpha    Alpha
		private int alpha = 0;
	//	a:alphaOff    Alpha Offset
		private int alphaOff = 0;
	//	a:alphaMod    Alpha Modulation
		private int alphaMod = 0;
	//	a:hue    Hue
		private int hue = 0;
	//	a:hueOff    Hue Offset
		private int hueOff = 0;
	//	a:hueMod    Hue Modulate
		private int hueMod = 0;
	//	a:sat    Saturation
		private int sat = 0;
	//	a:satOff    Saturation Offset
		private int satOff = 0;
	//	a:satMod    Saturation Modulation
		private int satMod = 0;
	//	a:lum    Luminance
		private int lum = 0;
	//	a:lumOff    Luminance Offset
		private int lumOff = 0;
	//	a:lumMod    Luminance Modulation
		private int lumMod = 0;
	//	a:red    Red
		private int red = 0;
	//	a:redOff    Red Offset
		private int redOff = 0;
	//	a:redMod    Red Modulation
		private int redMod = 0;
	//	a:green    Green
		private int green = 0;
	//	a:greenOff    Green Offset
		private int greenOff = 0;
	//	a:greenMod    Green Modification
		private int greenMod = 0;
	//	a:blue    Blue
		private int blue = 0;
	//	a:blueOff    Blue Offset
		private int blueOff = 0;
	//	a:blueMod    Blue Modification
		private int blueMod = 0;
	//	a:gamma    Gamma
		private int gamma = 0;
	//	a:invGamma    Inverse Gamma
		private int invGamma = 0;

		protected internal Color color;

		protected internal bool isDynamic = false;

		protected internal bool isInitialized = false;

		protected internal bool hasEffects = false;

		protected internal virtual void calcColor(int styleColor, mxVsdxTheme theme)
		{
			if (hasEffects)
			{
				//TODO complete the list of effects
				//currently we support tint, shade, satMod, lumMod 
	//			HSLColor hslColor = color.toHsl();
	//			if (tint != 0)
	//			{
	//				hslColor.tint(tint);
	//			}
	//			if (shade != 0)
	//			{
	//				hslColor.shade(shade);
	//			}
	//			if (satMod != 0)
	//			{
	//				hslColor.satMod(satMod);
	//			}
	//			if (lumMod != 0)
	//			{
	//				hslColor.lumMod(lumMod);
	//			}
	//			color = hslColor.toRgb();


				HSVColor hsvColor = color.toHsv();
				if (tint != 0)
				{
					hsvColor.tint(tint);
				}
				if (shade != 0)
				{
					hsvColor.shade(shade);
				}
				if (satMod != 0)
				{
					hsvColor.satMod(satMod);
				}
				if (lumMod != 0)
				{
					//TODO this may be better done in HSL color format
					hsvColor.lumMod(lumMod);
				}
				if (hueMod != 0)
				{
					hsvColor.hueMod(hueMod);
				}
				color = hsvColor.toRgb();
			}
		}

		public virtual Color getColor(int styleColor, mxVsdxTheme theme)
		{
			if (isDynamic || !isInitialized)
			{
				calcColor(styleColor, theme);
				isInitialized = true;
			}
			return color;
		}

		public virtual Color getColor(mxVsdxTheme theme)
		{
			return getColor(-1, theme);
		}

		public virtual int Tint
		{
			set
			{
				this.tint = value;
				hasEffects = true;
			}
		}

		public virtual int Shade
		{
			set
			{
				this.shade = value;
				hasEffects = true;
			}
		}

		public virtual int Comp
		{
			set
			{
				this.comp = value;
				hasEffects = true;
			}
		}

		public virtual int Inv
		{
			set
			{
				this.inv = value;
				hasEffects = true;
			}
		}

		public virtual int Gray
		{
			set
			{
				this.gray = value;
				hasEffects = true;
			}
		}

		public virtual int Alpha
		{
			set
			{
				this.alpha = value;
				hasEffects = true;
			}
		}

		public virtual int AlphaOff
		{
			set
			{
				this.alphaOff = value;
				hasEffects = true;
			}
		}

		public virtual int AlphaMod
		{
			set
			{
				this.alphaMod = value;
				hasEffects = true;
			}
		}

		public virtual int Hue
		{
			set
			{
				this.hue = value;
				hasEffects = true;
			}
		}

		public virtual int HueOff
		{
			set
			{
				this.hueOff = value;
				hasEffects = true;
			}
		}

		public virtual int HueMod
		{
			set
			{
				this.hueMod = value;
				hasEffects = true;
			}
		}

		public virtual int Sat
		{
			set
			{
				this.sat = value;
				hasEffects = true;
			}
		}

		public virtual int SatOff
		{
			set
			{
				this.satOff = value;
				hasEffects = true;
			}
		}

		public virtual int SatMod
		{
			set
			{
				this.satMod = value;
				hasEffects = true;
			}
		}

		public virtual int Lum
		{
			set
			{
				this.lum = value;
				hasEffects = true;
			}
		}

		public virtual int LumOff
		{
			set
			{
				this.lumOff = value;
				hasEffects = true;
			}
		}

		public virtual int LumMod
		{
			set
			{
				this.lumMod = value;
				hasEffects = true;
			}
		}

		public virtual int Red
		{
			set
			{
				this.red = value;
				hasEffects = true;
			}
		}

		public virtual int RedOff
		{
			set
			{
				this.redOff = value;
				hasEffects = true;
			}
		}

		public virtual int RedMod
		{
			set
			{
				this.redMod = value;
				hasEffects = true;
			}
		}

		public virtual int Green
		{
			set
			{
				this.green = value;
				hasEffects = true;
			}
		}

		public virtual int GreenOff
		{
			set
			{
				this.greenOff = value;
				hasEffects = true;
			}
		}

		public virtual int GreenMod
		{
			set
			{
				this.greenMod = value;
				hasEffects = true;
			}
		}

		public virtual int Blue
		{
			set
			{
				this.blue = value;
				hasEffects = true;
			}
		}

		public virtual int BlueOff
		{
			set
			{
				this.blueOff = value;
				hasEffects = true;
			}
		}

		public virtual int BlueMod
		{
			set
			{
				this.blueMod = value;
				hasEffects = true;
			}
		}

		public virtual int Gamma
		{
			set
			{
				this.gamma = value;
				hasEffects = true;
			}
		}

		public virtual int InvGamma
		{
			set
			{
				this.invGamma = value;
				hasEffects = true;
			}
		}
	}

}