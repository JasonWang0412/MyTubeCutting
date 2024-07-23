using OCC.TopoDS;

namespace MyCAMCore
{
	public class CADFeatureData
	{
		public CADFeatureData( TopoDS_Shape outerWire, TopoDS_Shape innerWire, TopoDS_Shape featureShell )
		{
			OuterWire = outerWire;
			InnerWire = innerWire;
			FeatureShell = featureShell;
		}

		public TopoDS_Shape OuterWire
		{
			get;
		}

		public TopoDS_Shape InnerWire
		{
			get;
		}

		public TopoDS_Shape FeatureShell
		{
			get;
		}
	}
}
