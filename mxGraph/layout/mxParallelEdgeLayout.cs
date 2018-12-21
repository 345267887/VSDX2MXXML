using System;
using System.Collections.Generic;

namespace mxGraph.layout
{


	using mxCellPath = mxGraph.model.mxCellPath;
	using mxGeometry = mxGraph.model.mxGeometry;
	using mxICell = mxGraph.model.mxICell;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxPoint = mxGraph.util.mxPoint;
	using mxGraph = mxGraph.view.mxGraph;
	using mxGraphView = mxGraph.view.mxGraphView;

	public class mxParallelEdgeLayout : mxGraphLayout
	{

		/// <summary>
		/// Specifies the spacing between the edges. Default is 20.
		/// </summary>
		protected internal int spacing;

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxParallelEdgeLayout(mxGraph graph) : this(graph, 20)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxParallelEdgeLayout(mxGraph graph, int spacing) : base(graph)
		{
			this.spacing = spacing;
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			IDictionary<string, IList<object>> lookup = findParallels(parent);

			graph.Model.beginUpdate();
			try
			{
				IEnumerator<IList<object>> it = lookup.Values.GetEnumerator();

				while (it.MoveNext())
				{
					IList<object> parallels = it.Current;

					if (parallels.Count > 1)
					{
						layout(parallels);
					}
				}
			}
			finally
			{
				graph.Model.endUpdate();
			}
		}

		/// 
		protected internal virtual IDictionary<string, IList<object>> findParallels(object parent)
		{
			IDictionary<string, IList<object>> lookup = new Dictionary<string, IList<object>>();
			mxIGraphModel model = graph.Model;
			int childCount = model.getChildCount(parent);

			for (int i = 0; i < childCount; i++)
			{
				object child = model.getChildAt(parent, i);

				if (!isEdgeIgnored(child))
				{
					string id = getEdgeId(child);

					if (!string.ReferenceEquals(id, null))
					{
						if (!lookup.ContainsKey(id))
						{
							lookup[id] = new List<object>();
						}

						lookup[id].Add(child);
					}
				}
			}

			return lookup;
		}

		/// 
		protected internal virtual string getEdgeId(object edge)
		{
			mxGraphView view = graph.View;
			object src = view.getVisibleTerminal(edge, true);
			object trg = view.getVisibleTerminal(edge, false);

			if (src is mxICell && trg is mxICell)
			{
				string srcId = mxCellPath.create((mxICell) src);
				string trgId = mxCellPath.create((mxICell) trg);

				return (srcId.CompareTo(trgId) > 0) ? trgId + "-" + srcId : srcId + "-" + trgId;
			}

			return null;
		}

		/// 
		protected internal virtual void layout(IList<object> parallels)
		{
			object edge = parallels[0];
			mxIGraphModel model = graph.Model;
			mxGeometry src = model.getGeometry(model.getTerminal(edge, true));
			mxGeometry trg = model.getGeometry(model.getTerminal(edge, false));

			// Routes multiple loops
			if (src == trg)
			{
				double x0 = src.X + src.Width + this.spacing;
				double y0 = src.Y + src.Height / 2;

				for (int i = 0; i < parallels.Count; i++)
				{
					route(parallels[i], x0, y0);
					x0 += spacing;
				}
			}
			else if (src != null && trg != null)
			{
				// Routes parallel edges
				double scx = src.X + src.Width / 2;
				double scy = src.Y + src.Height / 2;

				double tcx = trg.X + trg.Width / 2;
				double tcy = trg.Y + trg.Height / 2;

				double dx = tcx - scx;
				double dy = tcy - scy;

				double len = Math.Sqrt(dx * dx + dy * dy);

				double x0 = scx + dx / 2;
				double y0 = scy + dy / 2;

				double nx = dy * spacing / len;
				double ny = dx * spacing / len;

				x0 += nx * (parallels.Count - 1) / 2;
				y0 -= ny * (parallels.Count - 1) / 2;

				for (int i = 0; i < parallels.Count; i++)
				{
					route(parallels[i], x0, y0);
					x0 -= nx;
					y0 += ny;
				}
			}
		}

		/// 
		protected internal virtual void route(object edge, double x, double y)
		{
			if (graph.isCellMovable(edge))
			{
				setEdgePoints(edge, (new List<mxPoint> {new mxPoint(x, y)}));
			}
		}

	}

}