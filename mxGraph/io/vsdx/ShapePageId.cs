/// <summary>
/// Copyright (c) 2010-2016, JGraph Ltd
/// Copyright (c) 2010-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{

	/// <summary>
	/// Wraps the page and shape ID within that page to create a unique ID
	/// </summary>
	public class ShapePageId
	{
		private int pageNumber;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private int Id_Renamed;

		public ShapePageId(int pageNumber, int Id)
		{
			this.pageNumber = pageNumber;
			this.Id_Renamed = Id;
		}

		public virtual int Id
		{
			get
			{
				return Id_Renamed;
			}
		}

		public virtual int PageNumber
		{
			get
			{
				return pageNumber;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ShapePageId other = (ShapePageId) obj;
			ShapePageId other = (ShapePageId) obj;

			if (this.pageNumber != other.pageNumber || this.Id_Renamed != other.Id_Renamed)
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return 100000 * this.pageNumber + this.Id_Renamed;
		}
	}

}