namespace MyTubeCutting
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
		CommandErrorCode Do();

		void Undo();

		event CADEditFinishEventHandler EditFinished;
	}
}
