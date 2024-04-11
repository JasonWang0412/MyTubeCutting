namespace MyTubeCutting
{
	delegate void CADEditFinishEventHandler( EditType type, string szObjectName );

	internal enum EditType
	{
		AddMainTube,
		ModifyMainTube,
		AddCADFeature,
		RemoveCADFeature,
		ModifyCADFeature,
	}

	internal interface ICADEditCommand
	{
		void Do();

		void Undo();

		event CADEditFinishEventHandler EditFinished;
	}
}
