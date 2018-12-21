using System.Collections.Generic;

namespace mxGraph.io.gliffy.model
{

	public class Constraints
	{
		private IList<Constraint> constraints;

		private Constraint startConstraint;

		private Constraint endConstraint;

		public Constraints()
		{
		}

		public virtual IList<Constraint> getConstraints()
		{
			return constraints;
		}

		public virtual void setConstraints(IList<Constraint> constraints)
		{
			this.constraints = constraints;
		}

		public virtual Constraint StartConstraint
		{
			get
			{
				return startConstraint;
			}
			set
			{
				this.startConstraint = value;
			}
		}


		public virtual Constraint EndConstraint
		{
			get
			{
				return endConstraint;
			}
			set
			{
				this.endConstraint = value;
			}
		}


	}

}