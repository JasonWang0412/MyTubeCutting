using OCC.STEPControl;
using OCC.TopoDS;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyUtility
{
	public class ExportHelper
	{
		public static void ExportBrep( List<TopoDS_Shape> ShapeList, string szType, int nCount = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			for( int i = 0; i < ShapeList.Count; i++ ) {

				// 儲存新檔案
				string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + "_" + i.ToString( GetFormat( ShapeList.Count ) ) + ".brep" );
				OCC.BRepTools.BRepTools.Write( ShapeList[ i ], filePath );
			}
		}

		public static void ExportBrep( TopoDS_Shape shape, string szType, int nCount = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			// 儲存新檔案
			string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + ".brep" );
			OCC.BRepTools.BRepTools.Write( shape, filePath );
		}

		public static void ExportBrep( TopoDS_Shape shape, string szType, int nCount = 0, int i = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			// 儲存新檔案
			string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + "_" + i.ToString() + ".brep" );
			OCC.BRepTools.BRepTools.Write( shape, filePath );
		}

		public static void ExportBrep( TopoDS_Shape shape, string szType, int nCount = 0, int i = 0, int j = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			// 儲存新檔案
			string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + "_" + i.ToString() + "_" + j.ToString() + ".brep" );
			OCC.BRepTools.BRepTools.Write( shape, filePath );
		}

		public static void ExportBrep( List<List<TopoDS_Edge>> ShapeListList, string szType, int nCount = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			for( int i = 0; i < ShapeListList.Count; i++ ) {
				for( int j = 0; j < ShapeListList[ i ].Count; j++ ) {
					// 儲存新檔案
					string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + "_" +
						i.ToString( GetFormat( ShapeListList.Count ) ) + "_" + j.ToString( GetFormat( ShapeListList[ i ].Count ) ) + ".brep" );
					OCC.BRepTools.BRepTools.Write( ShapeListList[ i ][ j ], filePath );
				}
			}
		}

		public static void ExportBrep( List<TopoDS_Edge> ShapeList, string szType, int nCount = 0 )
		{
			string combinedPath = MakeDir( szType, nCount );

			for( int i = 0; i < ShapeList.Count; i++ ) {
				// 儲存新檔案
				string filePath = Path.Combine( combinedPath, "_" + nCount.ToString() + "_" + szType + "_" + i.ToString( GetFormat( ShapeList.Count ) ) + ".brep" );
				OCC.BRepTools.BRepTools.Write( ShapeList[ i ], filePath );
			}
		}

		public static void ExportStep( TopoDS_Shape shape, string szType )
		{
			string combinedPath = MakeDir( szType );

			// 儲存新檔案
			string filePath = Path.Combine( combinedPath, szType + ".stp" );
			STEPControl_Writer writer = new STEPControl_Writer();
			writer.Transfer( shape, STEPControl_StepModelType.STEPControl_AsIs );
			writer.Write( filePath );
		}

		static string MakeDir( string szType, int nCount = 0 )
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string newDirectory = "OutPut" + "\\" + nCount.ToString() + "\\" + szType; // 欲新增的目錄名稱
			string combinedPath = Path.Combine( baseDirectory, newDirectory );
			Console.WriteLine( "合併後的目錄路徑: " + combinedPath );

			// 創建新目錄
			Directory.CreateDirectory( combinedPath );
			Console.WriteLine( "已新增目錄。" );

			return combinedPath;
		}

		static string GetFormat( int nCount )
		{
			if( nCount > 1000 ) {
				return "D4";
			}
			else if( nCount > 100 ) {
				return "D3";
			}
			else if( nCount > 10 ) {
				return "D2";
			}
			return "D1";
		}
	}
}
