using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class Ellipse : Row
	{
		public Ellipse(int index, double? x, double? y, double? a, double? b, double? c, double? d) : base(index, x, y)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
			{
				double h = shape.Height;
				double w = shape.Width;

				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				y = h - y;
				double a = this.a.Value * mxVsdxUtils.conversionFactor;
				double b = this.b.Value * mxVsdxUtils.conversionFactor;
				b = h - b;
				double c = this.c.Value * mxVsdxUtils.conversionFactor;
				double d = this.d.Value * mxVsdxUtils.conversionFactor;
				d = h - d;

				double dx1 = Math.Abs(a - x);
				double dy1 = Math.Abs(b - y);
				double r1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);

				double dx2 = Math.Abs(c - x);
				double dy2 = Math.Abs(d - y);
				double r2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);
				double newX = x * 100 / w;
				double newY = y * 100 / h;
				double newW = (r1 * 100 / w) / 2;
				double newH = (r2 * 100 / h) / 2;
				newH = Math.Round(newH * 100.0) / 100.0;
				newW = Math.Round(newW * 100.0) / 100.0;
				double newX1 = Math.Round((newX - 2 * newW) * 100.0) / 100.0;
				double newX2 = Math.Round((newX + 2 * newW) * 100.0) / 100.0;
				newY = Math.Round(newY * 100.0) / 100.0;


				return "<move x=\"" + newX1.ToString() + "\" y=\"" + newY.ToString() + "\"/>" + "<arc" + " rx=\"" + newW.ToString() + "\" ry=\"" + newH.ToString() + "\" x=\"" + newX2.ToString() + "\" y=\"" + newY.ToString() + "\" x-axis-rotation=\"0\" large-arc-flag=\"1\" sweep-flag=\"0\"/>" + "<arc" + " rx=\"" + newW.ToString() + "\" ry=\"" + newH.ToString() + "\" x=\"" + newX1.ToString() + "\" y=\"" + newY.ToString() + "\" x-axis-rotation=\"0\" large-arc-flag=\"1\" sweep-flag=\"0\"/>";
			}

			return "";
		}

	//	public String handle1(mxPoint p, Shape1 shape)
	//	{
	//		if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
	//		{
	//			double h = shape.getHeight();
	//			double w = shape.getWidth();
	//
	//			double x = this.x * mxVsdxUtils.conversionFactor;
	//			double y = this.y * mxVsdxUtils.conversionFactor;
	//			y = h - y;
	//			double a = this.a * mxVsdxUtils.conversionFactor;
	//			double b = this.b * mxVsdxUtils.conversionFactor;
	//			b = h - b;
	//			double c = this.c * mxVsdxUtils.conversionFactor;
	//			double d = this.d * mxVsdxUtils.conversionFactor;
	//			d = h - d;
	//			
	//			double dx1 = Math.abs(a - x);
	//			double dy1 = Math.abs(b - y);
	//			double r1 = Math.sqrt(dx1 * dx1 + dy1 * dy1);
	//
	//			double dx2 = Math.abs(c - x);
	//			double dy2 = Math.abs(d - y);
	//			double r2 = Math.sqrt(dx2 * dx2 + dy2 * dy2);
	//			double newX = x * 100 / w;
	//			double newY = y * 100 / h;
	//			double newW = (r1 * 100 / w)/2;
	//			double newH = (r2 * 100 / h)/2;
	//			newH = Math.round(newH * 100.0) / 100.0;
	//			newW = Math.round(newW * 100.0) / 100.0;
	//			double newX1 = Math.round((newX - 2 * newW) * 100.0) / 100.0;
	//			double newX2 = Math.round((newX + 2 * newW) * 100.0) / 100.0;
	//			newY = Math.round(newY * 100.0) / 100.0;
	//			
	//			
	//			return  "<move x=\"" + String.valueOf(newX1) + "\" y=\"" + String.valueOf(newY) + "\"/>" +
	//								"<arc" +
	//								" rx=\"" + String.valueOf(newW) + 
	//								"\" ry=\"" + String.valueOf(newH) + 
	//								"\" x=\"" + String.valueOf(newX2) + 
	//								"\" y=\"" + String.valueOf(newY) + 
	//								"\" x-axis-rotation=\"0\" large-arc-flag=\"1\" sweep-flag=\"0\"/>" +
	//								
	//								"<arc" +
	//								" rx=\"" + String.valueOf(newW) + 
	//								"\" ry=\"" + String.valueOf(newH) + 
	//								"\" x=\"" + String.valueOf(newX1) + 
	//								"\" y=\"" + String.valueOf(newY) + 
	//								"\" x-axis-rotation=\"0\" large-arc-flag=\"1\" sweep-flag=\"0\"/>";
	//		}
	//		
	//		return "";
	//	}
	//
	//	
	//	public String handle2(mxPoint p, Shape1 shape)
	//	{
	//		if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
	//		{
	//			double h = shape.getHeight();
	//			double w = shape.getWidth();
	//
	//			double x = this.x * mxVsdxUtils.conversionFactor;
	//			double y = this.y * mxVsdxUtils.conversionFactor;
	//			y = h - y;
	//			double a = this.a * mxVsdxUtils.conversionFactor;
	//			double b = this.b * mxVsdxUtils.conversionFactor;
	//			b = h - b;
	//			double c = this.c * mxVsdxUtils.conversionFactor;
	//			double d = this.d * mxVsdxUtils.conversionFactor;
	//			d = h - d;
	//			
	//			double dx1 = Math.abs(a - x);
	//			double dy1 = Math.abs(b - y);
	//			double r1 = Math.sqrt(dx1 * dx1 + dy1 * dy1);
	//
	//			double dx2 = Math.abs(c - x);
	//			double dy2 = Math.abs(d - y);
	//			double r2 = Math.sqrt(dx2 * dx2 + dy2 * dy2);
	//			double newX = (x - r1) * 100 / w;
	//			double newY = (y - r2) * 100 / h;
	//			double newW = 2 * r1 * 100 / w;
	//			double newH = 2 * r2 * 100 / h;
	//			newH = Math.round(newH * 100.0) / 100.0;
	//			newW = Math.round(newW * 100.0) / 100.0;
	//			newX = Math.round(newX * 100.0) / 100.0;
	//			newY = Math.round(newY * 100.0) / 100.0;
	//			
	//			return "<ellipse" + 
	//					" x=\"" + String.valueOf(newX) + 
	//					"\" y=\"" + String.valueOf(newY) + 
	//					"\" w=\"" + String.valueOf(newW) + 
	//					"\" h=\"" + String.valueOf(newH) + 
	//					"\"/>";
	//		}
	//		
	//		return "";
	//	}
	//	
	//	
	//	public static class Shape1 {
	//		double width, height;
	//
	//		public double getWidth() {
	//			return width;
	//		}
	//
	//		public double getHeight() {
	//			return height;
	//		}
	//
	//		public void setWidth(double width) {
	//			this.width = width;
	//		}
	//
	//		public void setHeight(double height) {
	//			this.height = height;
	//		}
	//
	//		public Shape1(double width, double height) {
	//			this.width = width;
	//			this.height = height;
	//		}
	//		
	//	}
	//	public static void main(String[] args) {
	//		Shape1 shape = new Shape1(160.91, 76.2);
	//		Ellipse ellipse = new Ellipse(0, 0.5, 0.1, 0.93, 0.1, 0.90, 0.50);
	//		System.out.println(ellipse.handle1(null, shape));
	//		System.out.println(ellipse.handle2(null, shape));
	//	}
	}

}