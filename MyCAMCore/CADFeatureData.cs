using OCC.TopoDS;
using System.Collections.Generic;

namespace MyCAMCore
{
	public class CADFeatureData
	{
		public CADFeatureData( List<TopoDS_Edge> outerWire, List<TopoDS_Edge> innerWire, List<TopoDS_Shape> featureShell )
		{
			OuterWire = outerWire;
			InnerWire = innerWire;
			FeatureShell = featureShell;
		}

		public List<TopoDS_Edge> OuterWire
		{
			get;
		}

		public List<TopoDS_Edge> InnerWire
		{
			get;
		}

		public List<TopoDS_Shape> FeatureShell
		{
			get;
		}
	}
}
