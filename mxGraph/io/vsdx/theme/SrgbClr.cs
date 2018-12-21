namespace mxGraph.io.vsdx.theme
{


	public class SrgbClr : OoxmlColor
	{
		private string hexVal;
		public SrgbClr(string hexVal)
		{
			this.hexVal = hexVal;
			color = Color.decodeColorHex(hexVal);
		}
	}

}