namespace MyCADEditor
{
	public enum CADEditErrorCode
	{
		// OK
		OK = 0,

		// process error
		NullParam = 1,
		NoMainTube = 2,
		NoSelectedObject = 3,

		// operation error
		InvalidParam = 100,
		MakeShapeFailed = 101,
		CanNotRemoveMainTube = 102,
	}
}
