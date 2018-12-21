using System.Collections.Generic;
using System.Text;

namespace mxGraph.view
{


	using Element = System.Xml.XmlElement;

	using mxIGraphModel = model.mxIGraphModel;
	using mxUtils = util.mxUtils;

	public class mxMultiplicity
	{

		/// <summary>
		/// Defines the type of the source or target terminal. The type is a string
		/// passed to mxUtils.isNode together with the source or target vertex
		/// value as the first argument.
		/// </summary>
		protected internal string type;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal string attr;

		/// <summary>
		/// Optional string that specifies the value of the attribute to be passed
		/// to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal string value;

		/// <summary>
		/// Boolean that specifies if the rule is applied to the source or target
		/// terminal of an edge.
		/// </summary>
		protected internal bool source;

		/// <summary>
		/// Defines the minimum number of connections for which this rule applies.
		/// Default is 0.
		/// </summary>
		protected internal int min = 0;

		/// <summary>
		/// Defines the maximum number of connections for which this rule applies.
		/// A value of 'n' means unlimited times. Default is 'n'. 
		/// </summary>
		protected internal string max = "n";

		/// <summary>
		/// Holds an array of strings that specify the type of neighbor for which
		/// this rule applies. The strings are used in mxCell.is on the opposite
		/// terminal to check if the rule applies to the connection.
		/// </summary>
		protected internal ICollection<string> validNeighbors;

		/// <summary>
		/// Boolean indicating if the list of validNeighbors are those that are allowed
		/// for this rule or those that are not allowed for this rule.
		/// </summary>
		protected internal bool validNeighborsAllowed = true;

		/// <summary>
		/// Holds the localized error message to be displayed if the number of
		/// connections for which the rule applies is smaller than min or greater
		/// than max.
		/// </summary>
		protected internal string countError;

		/// <summary>
		/// Holds the localized error message to be displayed if the type of the
		/// neighbor for a connection does not match the rule.
		/// </summary>
		protected internal string typeError;

		/// 
		public mxMultiplicity(bool source, string type, string attr, string value, int min, string max, ICollection<string> validNeighbors, string countError, string typeError, bool validNeighborsAllowed)
		{
			this.source = source;
			this.type = type;
			this.attr = attr;
			this.value = value;
			this.min = min;
			this.max = max;
			this.validNeighbors = validNeighbors;
			this.countError = countError;
			this.typeError = typeError;
			this.validNeighborsAllowed = validNeighborsAllowed;
		}

		/// <summary>
		/// Function: check
		/// 
		/// Checks the multiplicity for the given arguments and returns the error
		/// for the given connection or null if the multiplicity does not apply.
		/// 
		/// Parameters:
		/// 
		/// graph - Reference to the enclosing graph instance.
		/// edge - Cell that represents the edge to validate.
		/// source - Cell that represents the source terminal.
		/// target - Cell that represents the target terminal.
		/// sourceOut - Number of outgoing edges from the source terminal.
		/// targetIn - Number of incoming edges for the target terminal.
		/// </summary>
		public virtual string check(mxGraph graph, object edge, object source, object target, int sourceOut, int targetIn)
		{
			StringBuilder error = new StringBuilder();

			if ((this.source && checkTerminal(graph, source, edge)) || (!this.source && checkTerminal(graph, target, edge)))
			{
				if (!Unlimited)
				{
					int m = MaxValue;

					if (m == 0 || (this.source && sourceOut >= m) || (!this.source && targetIn >= m))
					{
						error.Append(countError + "\n");
					}
				}

				if (validNeighbors != null && !string.ReferenceEquals(typeError, null) && validNeighbors.Count > 0)
				{
					bool isValid = checkNeighbors(graph, edge, source, target);

					if (!isValid)
					{
						error.Append(typeError + "\n");
					}
				}
			}

			return (error.Length > 0) ? error.ToString() : null;
		}

		/// <summary>
		/// Checks the type of the given value.
		/// </summary>
		public virtual bool checkNeighbors(mxGraph graph, object edge, object source, object target)
		{
			mxIGraphModel model = graph.Model;
			object sourceValue = model.getValue(source);
			object targetValue = model.getValue(target);
			bool isValid = !validNeighborsAllowed;
			IEnumerator<string> it = validNeighbors.GetEnumerator();

			while (it.MoveNext())
			{
				string tmp = it.Current;

				if (this.source && checkType(graph, targetValue, tmp))
				{
					isValid = validNeighborsAllowed;
					break;
				}
				else if (!this.source && checkType(graph, sourceValue, tmp))
				{
					isValid = validNeighborsAllowed;
					break;
				}
			}

			return isValid;
		}

		/// <summary>
		/// Checks the type of the given value.
		/// </summary>
		public virtual bool checkTerminal(mxGraph graph, object terminal, object edge)
		{
			object userObject = graph.Model.getValue(terminal);

			return checkType(graph, userObject, type, attr, value);
		}

		/// <summary>
		/// Checks the type of the given value.
		/// </summary>
		public virtual bool checkType(mxGraph graph, object value, string type)
		{
			return checkType(graph, value, type, null, null);
		}

		/// <summary>
		/// Checks the type of the given value.
		/// </summary>
		public virtual bool checkType(mxGraph graph, object value, string type, string attr, string attrValue)
		{
			if (value != null)
			{
				if (value is Element)
				{
					return mxUtils.isNode(value, type, attr, attrValue);
				}
				else
				{
					return value.Equals(type);
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if max is "n" (unlimited).
		/// </summary>
		public virtual bool Unlimited
		{
			get
			{
				return string.ReferenceEquals(max, null) || string.ReferenceEquals(max, "n");
			}
		}

		/// <summary>
		/// Returns the numeric value of max.
		/// </summary>
		public virtual int MaxValue
		{
			get
			{
				try
				{
					return int.Parse(max);
				}
				catch (System.FormatException)
				{
					// ignore
				}
    
				return 0;
			}
		}

	}

}