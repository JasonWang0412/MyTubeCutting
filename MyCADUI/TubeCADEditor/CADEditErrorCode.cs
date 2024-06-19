namespace MyCADUI
{
	public enum CADEditErrorCode
	{
		// process error
		NullParam = 0,
		NoMainTube = 1,
		NoSelectedObject = 2,

		// operation error
		InvalidParam = 100,
		MakeShapeFailed = 101,
		CanNotRemoveMainTube = 102,
	}
}
