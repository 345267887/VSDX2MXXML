using System;
using System.Collections.Generic;

namespace mxGraph.io.gliffy.model
{


	//using PostDeserializable = mxGraphio.gliffy.importer.PostDeserializer.PostDeserializable;
	using mxCell = mxGraph.model.mxCell;

	/// <summary>
	/// Class representing Gliffy diagram object
	/// 
	/// </summary>
	public class GliffyObject //: PostDeserializable
	{
		private static ISet<string> GRAPHICLESS_SHAPES = new HashSet<string>();

		private static ISet<string> GROUP_SHAPES = new HashSet<string>();

		private static ISet<string> MINDMAP_SHAPES = new HashSet<string>();

		public float x;

		public float y;

		public int id;

		public float width;

		public float height;

		public float rotation;

		public string uid;

		public string tid;

		public string order;

		public bool lockshape;

		public Graphic graphic;

		public List<GliffyObject> children;

		public Constraints constraints;

		public List<LinkMap> linkMap;

		public mxCell mxObject; // the mxCell this gliffy object got converted into

		public GliffyObject parent = null;

		static GliffyObject()
		{
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.package");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.class");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.simple_class");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.object_timeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.lifeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.use_case");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.actor");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.use_case");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.self_message");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.message");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.activation");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.dependency");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.dependency");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.composition");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.aggregation");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v1.default.association");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.package");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.simple_class");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.class");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.class2");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.interface");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.class.enumeration");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.sequence.lifeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.sequence.boundary_lifeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.sequence.control_lifeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.sequence.entity_lifeline");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.deployment.package");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.component.package");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.uml.uml_v2.use_case.package");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.erd.erd_v1.default.entity_with_attributes");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.erd.erd_v1.default.entity_with_multiple_attributes");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.bpmn.bpmn_v1.data_artifacts.annotation");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.erd.erd_v1.default.entity_with_multiple_attributes");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.navigation.navbar");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.forms_controls.combo_box");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.containers_content.tooltip_top");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.containers_content.popover_bottom");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.forms_controls.selector");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.icon_symbols.annotate_left");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.icon_symbols.annotate_right");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.icon_symbols.annotate_top");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.ui.ui_v3.containers_content.popover_top");

			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.sitemap.sitemap_v2.page");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.sitemap.sitemap_v2.home");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.sitemap.sitemap_v2.gliffy");
			GRAPHICLESS_SHAPES.Add("com.gliffy.shape.sitemap.sitemap_v2.form");

			/*GRAPHICLESS_SHAPES.add("com.gliffy.shape.sitemap.sitemap_v2.page");
			*/

			GROUP_SHAPES.Add("com.gliffy.shape.basic.basic_v1.default.group");

			MINDMAP_SHAPES.Add("com.gliffy.shape.mindmap.mindmap_v1.default.main_topic");
			MINDMAP_SHAPES.Add("com.gliffy.shape.mindmap.mindmap_v1.default.subtopic");
			MINDMAP_SHAPES.Add("com.gliffy.shape.mindmap.mindmap_v1.default.child_node");

		}

		public GliffyObject()
		{
		}

		public virtual Graphic Graphic
		{
			get
			{
				if (graphic != null)
				{
					return graphic;
				}
				else if (Uml || GRAPHICLESS_SHAPES.Contains(uid))
				{
					return FirstChildGraphic;
				}
				else
				{
					return null;
				}
			}
		}

		public virtual mxCell MxObject
		{
			get
			{
				return mxObject;
			}
		}

		/// <summary>
		/// Returns the object that represents the caption for this object
		/// 
		/// @return
		/// </summary>
		public virtual GliffyObject TextObject
		{
			get
			{
    
				if (IsText)
				{
					return this;
				}
				if (children == null)
				{
					return null;
				}
    
				foreach (GliffyObject child in children)
				{
					if (child.Graphic != null && child.Graphic.getType().Equals(Graphic.Type.TEXT))
					{
						return child;
					}
					else
					{
						return child.TextObject;
					}
				}
    
				return null;
			}
		}

		//public virtual string Text
		//{
		//	get
		//	{
		//		return graphic.Text.Html;
		//	}
		//}

		public virtual string Link
		{
			get
			{
				if (linkMap != null && linkMap.Count > 0)
				{
					return linkMap[0].url;
				}
    
				return null;
			}
		}

		/// <summary>
		/// Some shapes like UML package, class and interface do not have a graphic object but instead rely on graphic of their children.
		/// In that case, graphic is the same for all children </summary>
		/// <returns> graphic of the first child or null of there are no children </returns>
		public virtual Graphic FirstChildGraphic
		{
			get
			{
				return children.Count > 0 ? children[0].graphic : null;
			}
		}

		public virtual bool Group
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && GROUP_SHAPES.Contains(uid);
			}
		}

		public virtual bool Mindmap
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && MINDMAP_SHAPES.Contains(uid);
			}
		}

		public virtual bool Line
		{
			get
			{
				return graphic != null && graphic.getType().Equals(Graphic.Type.LINE);
			}
		}

		private bool Uml
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && (uid.StartsWith("com.gliffy.shape.uml.uml", StringComparison.Ordinal));
			}
		}

		public virtual bool Shape
		{
			get
			{
				if (graphic != null)
				{
					return graphic.getType().Equals(Graphic.Type.SHAPE) || graphic.getType().Equals(Graphic.Type.MINDMAP);
				}
				else
				{
					//some UML shapes do not have a graphic,instead their graphic type is determined by their first child
					Graphic g = FirstChildGraphic;
					return g != null && g.getType().Equals(Graphic.Type.SHAPE);
				}
			}
		}

		public virtual bool Svg
		{
			get
			{
				return graphic != null && graphic.type.Equals(Graphic.Type.SVG);
			}
		}

		public virtual bool Swimlane
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && uid.Contains("com.gliffy.shape.swimlanes");
			}
		}

		public virtual bool IsText
		{
			get
			{
				return graphic != null && graphic.getType().Equals(Graphic.Type.TEXT);
			}
		}

		public virtual bool Image
		{
			get
			{
				return graphic != null && graphic.getType().Equals(Graphic.Type.IMAGE);
			}
		}

		public virtual bool VennCircle
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && uid.StartsWith("com.gliffy.shape.venn", StringComparison.Ordinal);
			}
		}

		public virtual string GradientColor
		{
			get
			{
				string gradientColor = "#FFFFFF";
    
				// Gradient colors are lighter version of the fill color except for radial
				// venn shapes, where white is used with a radial gradient (we use linear)
				if (graphic != null && graphic.Shape_Renamed != null && !string.ReferenceEquals(uid, null) && !uid.StartsWith("com.gliffy.shape.radial", StringComparison.Ordinal))
				{
					string hex = graphic.Shape_Renamed.fillColor;
    
					if (!string.ReferenceEquals(hex, null) && hex.Length == 7 && hex[0] == '#')
					{
                        //long clr = long.Parse(hex.Substring(1), 16);

                        long clr = Convert.ToInt64(hex.Substring(1), 16);

                        long r = Math.Min(0xFF0000, ((clr & 0xFF0000) + 0xAA0000)) & 0xFF0000;
						long g = Math.Min(0x00FF00, ((clr & 0x00FF00) + 0x00AA00)) & 0x00FF00;
						long b = Math.Min(0x0000FF, ((clr & 0x0000FF) + 0x0000AA)) & 0x0000FF;
    
						gradientColor = string.Format("#{0:X6}", 0xFFFFFF & (r + g + b));
					}
				}
    
				return gradientColor;
			}
		}

		/// <summary>
		/// LATER: Add more cases where gradient is ignored.
		/// </summary>
		public virtual bool GradientIgnored
		{
			get
			{
				return !string.ReferenceEquals(uid, null) && (uid.StartsWith("com.gliffy.shape.venn.outline", StringComparison.Ordinal) || uid.StartsWith("com.gliffy.shape.venn.flat", StringComparison.Ordinal));
			}
		}

		/// <summary>
		/// Returns a boolean indicating if this object is a subroutine </summary>
		/// <returns> true if subroutine, false otherwise </returns>
		public virtual bool SubRoutine
		{
			get
			{
				return uid.Equals("com.gliffy.shape.flowchart.flowchart_v1.default.subroutine");
			}
		}

		public virtual bool UnrecognizedGraphicType
		{
			get
			{
				return graphic != null && graphic.type == null;
			}
		}

		public virtual Constraints Constraints
		{
			get
			{
				return constraints;
			}
		}

		public virtual bool hasChildren()
		{
			return children != null && children.Count > 0;
		}

		public override string ToString()
		{
			return !string.ReferenceEquals(uid, null) ? uid : tid;
		}

		public virtual void postDeserialize()
		{
			if (Group)
			{
				normalizeChildrenCoordinates();
			}
		}

		/// <summary>
		/// Some Gliffy diagrams have groups whose children have negative coordinates.
		/// This is a problem in draw.io as they get set to 0.
		/// This method expands the groups left and up and adjusts children's coordinates so that they are never less than zero.
		/// </summary>
		private void normalizeChildrenCoordinates()
		{
			//sorts the list to find the leftmost child and it's X
			IComparer<GliffyObject> cx = new ComparatorAnonymousInnerClass(this);

			children.Sort(cx);
			float xMin = children[0].x;

			if (xMin < 0)
			{
				width += -xMin; //increase width
				x += xMin;

				foreach (GliffyObject child in children) //increase x
				{
					child.x += -xMin;
				}
			}

			//sorts the list to find the uppermost child and it's Y
			IComparer<GliffyObject> cy = new ComparatorAnonymousInnerClass2(this);

			children.Sort(cy);
			float yMin = children[0].y;

			if (yMin < 0)
			{
				height += -yMin; //increase height
				y += yMin;

				foreach (GliffyObject child in children) //increase y
				{
					child.y += -yMin;
				}
			}
		}

		private class ComparatorAnonymousInnerClass : IComparer<GliffyObject>
		{
			private readonly GliffyObject outerInstance;

			public ComparatorAnonymousInnerClass(GliffyObject outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(GliffyObject o1, GliffyObject o2)
			{
				return (int)(o1.x - o2.x);
			}
		}

		private class ComparatorAnonymousInnerClass2 : IComparer<GliffyObject>
		{
			private readonly GliffyObject outerInstance;

			public ComparatorAnonymousInnerClass2(GliffyObject outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(GliffyObject o1, GliffyObject o2)
			{
				return (int)(o1.y - o2.y);
			}
		}
	}

}