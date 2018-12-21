using System;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{

    using Element = System.Xml.XmlElement;

	/// <summary>
	/// Wrapper for connect element
	/// See https://msdn.microsoft.com/en-us/library/office/ff768299%28v=office.14%29.aspx
	/// 
	/// </summary>
	public class mxVsdxConnect
	{
		protected internal Element endShape;

		/// <summary>
		/// ID of edge
		/// </summary>
		protected internal int? fromSheet = null;

		/// <summary>
		/// ID of source
		/// </summary>
		protected internal int? sourceToSheet = null;

		/// <summary>
		/// Where connection is made to source
		/// </summary>
		protected internal int? sourceToPart = -1;

		/// <summary>
		/// ID of target
		/// </summary>
		protected internal int? targetToSheet = null;

		/// <summary>
		/// Where connection is made to target
		/// </summary>
		protected internal int? targetToPart = -1;

		protected internal string fromCell = null;

		public mxVsdxConnect(Element connectElem)
		{
            string fromSheet = connectElem.GetAttribute(mxVsdxConstants.FROM_SHEET);
			this.fromSheet = (!string.ReferenceEquals(fromSheet, null) && fromSheet.Length > 0) ? Convert.ToInt32(fromSheet) : -1;

			string fromCell = connectElem.GetAttribute(mxVsdxConstants.FROM_CELL);
			addFromCell(connectElem, fromCell);
		}

		protected internal virtual void addFromCell(Element connectElem, string fromCell)
		{
			string toSheet = connectElem.GetAttribute(mxVsdxConstants.TO_SHEET);
			bool source = true;

			if (!string.ReferenceEquals(fromCell, null) && fromCell.Equals(mxVsdxConstants.BEGIN_X))
			{
				this.sourceToSheet = (!string.ReferenceEquals(toSheet, null) && toSheet.Length > 0) ? Convert.ToInt32(toSheet) : -1;
				source = true;
			}
			else if (!string.ReferenceEquals(fromCell, null) && fromCell.Equals(mxVsdxConstants.END_X))
			{
				this.targetToSheet = (!string.ReferenceEquals(toSheet, null) && toSheet.Length > 0) ? Convert.ToInt32(toSheet) : -1;
				source = false;
			}
			else if (this.sourceToSheet == null)
			{
				this.sourceToSheet = (!string.ReferenceEquals(toSheet, null) && toSheet.Length > 0) ? Convert.ToInt32(toSheet) : -1;
				source = true;
			}
			else if (this.targetToSheet == null)
			{
				this.targetToSheet = (!string.ReferenceEquals(toSheet, null) && toSheet.Length > 0) ? Convert.ToInt32(toSheet) : -1;
				source = false;
			}

			findToPart(connectElem, source);
		}

		protected internal virtual void findToPart(Element connectElem, bool source)
		{
			string toPartString = connectElem.GetAttribute(mxVsdxConstants.TO_PART);
			int? toPart = (!string.ReferenceEquals(toPartString, null) && toPartString.Length > 0) ? Convert.ToInt32(toPartString) : -1;

			if (source)
			{
				sourceToPart = toPart;
			}
			else
			{
				targetToPart = toPart;
			}
		}

		public virtual int? FromSheet
		{
			get
			{
				return this.fromSheet;
			}
		}

		public virtual int? SourceToSheet
		{
			get
			{
				return this.sourceToSheet;
			}
		}

		public virtual int? TargetToSheet
		{
			get
			{
				return this.targetToSheet;
			}
		}

		public virtual int? SourceToPart
		{
			get
			{
				return this.sourceToPart;
			}
		}

		public virtual int? TargetToPart
		{
			get
			{
				return this.targetToPart;
			}
		}

		/// 
		/// <param name="connectElem"> </param>
		public virtual void addConnect(Element connectElem)
		{
			this.endShape = connectElem;
			string fromCell = connectElem.GetAttribute(mxVsdxConstants.FROM_CELL);
			addFromCell(connectElem, fromCell);
		}

	}

}