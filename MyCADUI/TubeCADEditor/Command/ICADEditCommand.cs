namespace MyCADUI
{
	delegate void CADEditFinishEventHandler( EditType type, string szObjectName );

	internal enum EditType
	{
		AddMainTube,
		RemoveMainTube,
		ModifyMainTube,
		AddCADFeature,
		RemoveCADFeature,
		ModifyCADFeature,
	}

	internal interface ICADEditCommand
	{
		void Do();

		void Undo();

		event CADEditFinishEventHandler CommandFinished;
	}
}
