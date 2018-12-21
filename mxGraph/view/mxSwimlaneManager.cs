namespace mxGraph.view
{

	using mxGeometry = model.mxGeometry;
	using mxIGraphModel = model.mxIGraphModel;
	using mxEvent = util.mxEvent;
	using mxEventObject = util.mxEventObject;
	using mxEventSource = util.mxEventSource;
	using mxRectangle = util.mxRectangle;

	public class mxSwimlaneManager : mxEventSource
	{

		/// <summary>
		/// Defines the type of the source or target terminal. The type is a string
		/// passed to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal mxGraph graph;

		/// <summary>
		/// Optional string that specifies the value of the attribute to be passed
		/// to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool enabled;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool horizontal;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool siblings;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool bubbling;

		/// 
		protected internal mxIEventListener addHandler = new mxIEventListenerAnonymousInnerClass();

		private class mxIEventListenerAnonymousInnerClass : mxIEventListener
		{
			public mxIEventListenerAnonymousInnerClass()
			{
			}

			public virtual void invoke(object source, mxEventObject evt)
			{
				//if (outerInstance.Enabled)
				//{
				//	outerInstance.cellsAdded((object[]) evt.getProperty("cells"));
				//}


			}
		}

		/// 
		protected internal mxIEventListener resizeHandler = new mxIEventListenerAnonymousInnerClass2();

		private class mxIEventListenerAnonymousInnerClass2 : mxIEventListener
		{
			public mxIEventListenerAnonymousInnerClass2()
			{
			}

			public virtual void invoke(object source, mxEventObject evt)
			{
				//if (outerInstance.Enabled)
				//{
				//	outerInstance.cellsResized((object[]) evt.getProperty("cells"));
				//}
			}
		}

		/// 
		public mxSwimlaneManager(mxGraph graph)
		{
			Graph = graph;
		}

		/// 
		public virtual bool isSwimlaneIgnored(object swimlane)
		{
			return !Graph.isSwimlane(swimlane);
		}

		/// <returns> the enabled </returns>
		public virtual bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}


		/// <returns> the bubbling </returns>
		public virtual bool Horizontal
		{
			get
			{
				return horizontal;
			}
			set
			{
				horizontal = value;
			}
		}


		/// <returns> the bubbling </returns>
		public virtual bool Siblings
		{
			get
			{
				return siblings;
			}
			set
			{
				siblings = value;
			}
		}


		/// <returns> the bubbling </returns>
		public virtual bool Bubbling
		{
			get
			{
				return bubbling;
			}
			set
			{
				bubbling = value;
			}
		}


		/// <returns> the graph </returns>
		public virtual mxGraph Graph
		{
			get
			{
				return graph;
			}
			set
			{
				if (this.graph != null)
				{
					this.graph.removeListener(addHandler);
					this.graph.removeListener(resizeHandler);
				}
    
				this.graph = value;
    
				if (this.graph != null)
				{
					this.graph.addListener(mxEvent.ADD_CELLS, addHandler);
					this.graph.addListener(mxEvent.CELLS_RESIZED, resizeHandler);
				}
			}
		}


		/// 
		protected internal virtual void cellsAdded(object[] cells)
		{
			if (cells != null)
			{
				mxIGraphModel model = Graph.Model;

				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (!isSwimlaneIgnored(cells[i]))
						{
							swimlaneAdded(cells[i]);
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void swimlaneAdded(object swimlane)
		{
			mxIGraphModel model = Graph.Model;

			// Tries to find existing swimlane for dimensions
			// TODO: Use parent geometry - header if inside
			// parent swimlane
			mxGeometry geo = null;
			object parent = model.getParent(swimlane);
			int childCount = model.getChildCount(parent);

			for (int i = 0; i < childCount; i++)
			{
				object child = model.getChildAt(parent, i);

				if (child != swimlane && !isSwimlaneIgnored(child))
				{
					geo = model.getGeometry(child);
					break;
				}
			}

			// Applies dimension to new child
			if (geo != null)
			{
				model.beginUpdate();
				try
				{
					resizeSwimlane(swimlane, geo.Width, geo.Height);
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void cellsResized(object[] cells)
		{
			if (cells != null)
			{
				mxIGraphModel model = Graph.Model;

				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (!isSwimlaneIgnored(cells[i]))
						{
							swimlaneResized(cells[i]);
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void swimlaneResized(object swimlane)
		{
			mxIGraphModel model = Graph.Model;
			mxGeometry geo = model.getGeometry(swimlane);

			if (geo != null)
			{
				double w = geo.Width;
				double h = geo.Height;

				model.beginUpdate();
				try
				{
					object parent = model.getParent(swimlane);

					if (Siblings)
					{
						int childCount = model.getChildCount(parent);

						for (int i = 0; i < childCount; i++)
						{
							object child = model.getChildAt(parent, i);

							if (child != swimlane && !isSwimlaneIgnored(child))
							{
								resizeSwimlane(child, w, h);
							}
						}
					}

					if (Bubbling && !isSwimlaneIgnored(parent))
					{
						resizeParent(parent, w, h);
						swimlaneResized(parent);
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void resizeSwimlane(object swimlane, double w, double h)
		{
			mxIGraphModel model = Graph.Model;
			mxGeometry geo = model.getGeometry(swimlane);

			if (geo != null)
			{
				geo = (mxGeometry) geo.clone();

				if (Horizontal)
				{
					geo.Width = w;
				}
				else
				{
					geo.Height = h;
				}

				model.setGeometry(swimlane, geo);
			}
		}

		/// 
		protected internal virtual void resizeParent(object parent, double w, double h)
		{
			mxIGraphModel model = Graph.Model;
			mxGeometry geo = model.getGeometry(parent);

			if (geo != null)
			{
				geo = (mxGeometry) geo.clone();
				mxRectangle size = graph.getStartSize(parent);

				if (Horizontal)
				{
					geo.Width = w + size.Width;
				}
				else
				{
					geo.Height = h + size.Height;
				}

				model.setGeometry(parent, geo);
			}
		}

		/// 
		public virtual void destroy()
		{
			Graph = null;
		}

	}

}