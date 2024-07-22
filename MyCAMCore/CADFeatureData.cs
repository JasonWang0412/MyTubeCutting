using OCC.TopoDS;
using System.Collections.Generic;
using System.Linq;

namespace MyCAMCore
{
	public class CADFeatureData
	{
		public CADFeatureData( List<TopoDS_Shape> outerWire, List<TopoDS_Shape> innerWire, List<TopoDS_Shape> featureShell )
		{
			OuterWire = outerWire;
			InnerWire = innerWire;
			FeatureShell = featureShell;
		}

		public CADFeatureData( List<TopoDS_Edge> outerWire, List<TopoDS_Edge> innerWire, List<TopoDS_Shape> featureShell )
		{
			OuterWire = outerWire.Cast<TopoDS_Shape>().ToList();
			InnerWire = innerWire.Cast<TopoDS_Shape>().ToList();
			FeatureShell = featureShell;
		}

		public List<TopoDS_Shape> OuterWire
		{
			get;
		}

		public List<TopoDS_Shape> InnerWire
		{
			get;
		}

		public List<TopoDS_Shape> FeatureShell
		{
			get;
		}
	}
}
