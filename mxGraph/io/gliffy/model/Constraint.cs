using System.Collections.Generic;

namespace mxGraph.io.gliffy.model
{

	//using SerializedName = com.google.gson.annotations.SerializedName;

	public class Constraint
	{

		public sealed class ConstraintType
		{
			public static readonly ConstraintType START_POSITION_CONSTRAINT = new ConstraintType("START_POSITION_CONSTRAINT", InnerEnum.START_POSITION_CONSTRAINT);
			public static readonly ConstraintType END_POSITION_CONSTRAINT = new ConstraintType("END_POSITION_CONSTRAINT", InnerEnum.END_POSITION_CONSTRAINT);
			public static readonly ConstraintType HEIGHT_CONSTRAINT = new ConstraintType("HEIGHT_CONSTRAINT", InnerEnum.HEIGHT_CONSTRAINT);

			private static readonly IList<ConstraintType> valueList = new List<ConstraintType>();

			static ConstraintType()
			{
				valueList.Add(START_POSITION_CONSTRAINT);
				valueList.Add(END_POSITION_CONSTRAINT);
				valueList.Add(HEIGHT_CONSTRAINT);
			}

			public enum InnerEnum
			{
				START_POSITION_CONSTRAINT,
				END_POSITION_CONSTRAINT,
				HEIGHT_CONSTRAINT
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			private ConstraintType(string name, InnerEnum innerEnum)
			{
				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public override string ToString()
			{
				return this.nameValue;
			}

			public static IList<ConstraintType> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public static ConstraintType valueOf(string name)
			{
				foreach (ConstraintType enumInstance in ConstraintType.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		public class ConstraintData
		{
			internal int nodeId;

			internal float px;

			internal float py;

			public virtual int NodeId
			{
				get
				{
					return nodeId;
				}
				set
				{
					this.nodeId = value;
				}
			}


			public virtual float Px
			{
				get
				{
					return px;
				}
				set
				{
					this.px = value;
				}
			}


			public virtual float Py
			{
				get
				{
					return py;
				}
				set
				{
					this.py = value;
				}
			}


		}

		private ConstraintType type;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ConstraintData StartPositionConstraint_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ConstraintData EndPositionConstraint_Renamed;

		public virtual ConstraintType Type
		{
			get
			{
				return type;
			}
			set
			{
				this.type = value;
			}
		}


		public virtual ConstraintData StartPositionConstraint
		{
			get
			{
				return StartPositionConstraint_Renamed;
			}
			set
			{
				StartPositionConstraint_Renamed = value;
			}
		}


		public virtual ConstraintData EndPositionConstraint
		{
			get
			{
				return EndPositionConstraint_Renamed;
			}
			set
			{
				EndPositionConstraint_Renamed = value;
			}
		}


	}

}