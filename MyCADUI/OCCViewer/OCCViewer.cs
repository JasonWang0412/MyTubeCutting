// Created: 2006-06-22 
// 
// Copyright (c) 2006-2013 OPEN CASCADE SAS 
// 
// This file is part of commercial software by OPEN CASCADE SAS. 
// 
// This software is furnished in accordance with the terms and conditions 
// of the contract and with the inclusion of this copyright notice. 
// This software or any other copy thereof may not be provided or otherwise 
// be made available to any third party. 
// No ownership title to the software is transferred hereby. 
// 
// OPEN CASCADE SAS makes no representation or warranties with respect to the 
// performance of this software, and specifically disclaims any responsibility 
// for any damages, special or consequential, connected with its use. 

/* ----------------------------------------------------------------------------
 * This class demonstrates direct usage of wrapped Open CASCADE classes from 
 * C# code. Compare it with the C++ OCCViewer class in standard C# sample.
 * ----------------------------------------------------------------------------- */

using OCC.AIS;
using OCC.Aspect;
using OCC.BRep;
using OCC.BRepTools;
using OCC.gp;
using OCC.Graphic3d;
using OCC.IFSelect;
using OCC.IGESControl;
using OCC.OpenGl;
using OCC.Quantity;
using OCC.Standard;
using OCC.STEPControl;
using OCC.TopoDS;
using OCC.V3d;
using OCC.WNT;
using System;

namespace MyCADUI
{
	public class OCCViewer : IDisposable
	{
		private V3d_Viewer myViewer;
		private V3d_View myView;
		private AIS_InteractiveContext myAISContext;
		private Graphic3d_GraphicDriver myGraphicDriver;

		const int SELECT_PixelTolerance = 5;

		public bool InitViewer( IntPtr wnd )
		{
			Aspect_DisplayConnection aDisplayConnection = new Aspect_DisplayConnection();
			myGraphicDriver = new OpenGl_GraphicDriver( aDisplayConnection );
			myViewer = new V3d_Viewer( myGraphicDriver );

			myViewer.SetDefaultLights();
			myViewer.SetLightOn();
			myView = myViewer.CreateView();
			WNT_Window aWNTWindow = new WNT_Window( wnd );
			myView.SetWindow( aWNTWindow );
			if( !aWNTWindow.IsMapped() )
				aWNTWindow.Map();
			myAISContext = new AIS_InteractiveContext( myViewer );
			myAISContext.UpdateCurrentViewer();
			myView.Redraw();
			myView.MustBeResized();
			myView.SetBackgroundColor( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_BLACK ) );

			// LASER-1073 make an Axis triedron which fixed on panel LEFT_LOWER
			myView.ZBufferTriedronSetup( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_RED ),
										new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_GREEN ),
										new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_BLUE1 ), 0.8, 0.02, 20 );
			myView.TriedronDisplay( Aspect_TypeOfTriedronPosition.Aspect_TOTP_LEFT_LOWER,
									new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_WHITE ),
									0.2, V3d_TypeOfVisualization.V3d_ZBUFFER );

			// expand the range of clicking path
			myAISContext.SelectionManager().Selector().SetPixelTolerance( SELECT_PixelTolerance );

			Graphic3d_RenderingParams.CRef cref = myView.ChangeRenderingParams();
			cref.Ptr.NbMsaaSamples = 10000;
			return true;
		}

		~OCCViewer()
		{
			Dispose();
		}

		/// <summary>
		/// Clean up resources being used
		/// </summary>
		public virtual void Dispose()
		{
			if( myAISContext != null ) {
				myAISContext.Dispose();
			}
			myAISContext = null;
			if( myView != null ) {
				myView.Dispose();
			}
			myView = null;
			if( myViewer != null ) {
				myViewer.Dispose();
			}
			myViewer = null;
		}

		public void UpdateView()
		{
			if( myView != null ) {
				myView.MustBeResized();
			}
		}

		public void RedrawView()
		{
			if( myView != null ) {
				myView.Redraw();
			}
		}

		public void WindowFitAll( int Xmin, int Ymin, int Xmax, int Ymax )
		{
			if( myView != null ) {
				myView.WindowFitAll( Xmin, Ymin, Xmax, Ymax );
			}
		}

		public void Place( int x, int y, float zoomFactor )
		{
			if( myView != null ) {
				myView.Place( x, y, zoomFactor );
			}
		}

		public void Zoom( int x1, int y1, int x2, int y2 )
		{
			if( myView != null ) {
				myView.Zoom( x1, y1, x2, y2 );
			}
		}

		public void Pan( int x, int y )
		{
			if( myView != null ) {
				myView.Pan( x, y );
			}
		}

		public void Rotation( int x, int y )
		{
			if( myView != null ) {
				myView.Rotation( x, y );
			}
		}

		public void StartRotation( int x, int y )
		{
			if( myView != null ) {
				myView.StartRotation( x, y );
			}
		}

		public void Select( int x1, int y1, int x2, int y2 )
		{
			if( myView != null ) {
				myAISContext.SelectRectangle( new Graphic3d_Vec2i( x1, y1 ), new Graphic3d_Vec2i( x2, y2 ), myView );
			}
		}

		public void Select()
		{
			myAISContext.SelectDetected();
		}

		public void MoveTo( int x, int y )
		{
			if( myView != null ) {
				myAISContext.MoveTo( x, y, myView, true );
			}
		}

		public void SetAllowOverlapDetection( bool isAllowOverlapDetection )
		{
			myAISContext.SelectionManager().Selector().AllowOverlapDetection( isAllowOverlapDetection );
		}

		public void ShiftSelect( int x1, int y1, int x2, int y2 )
		{
			if( myView != null ) {
				myAISContext.SelectRectangle( new Graphic3d_Vec2i( x1, y1 ), new Graphic3d_Vec2i( x2, y2 ), myView, AIS_SelectionScheme.AIS_SelectionScheme_XOR );
			}
		}

		public void ShiftSelect()
		{
			myAISContext.SelectDetected( AIS_SelectionScheme.AIS_SelectionScheme_XOR );
		}

		public void BackgroundColor( ref int r, ref int g, ref int b )
		{
			if( myView != null ) {
				double R1 = 0, G1 = 0, B1 = 0;
				myView.BackgroundColor( Quantity_TypeOfColor.Quantity_TOC_RGB, ref R1, ref G1, ref B1 );
				r = (int)( R1 * 255 );
				g = (int)( G1 * 255 );
				b = (int)( B1 * 255 );
			}
		}

		public void UpdateCurrentViewer()
		{
			myAISContext.UpdateCurrentViewer();
		}

		public void FrontView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Ypos );
			}
		}

		public void BackView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Yneg );
			}
		}

		public void TopView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Zpos );
			}
		}

		public void BottomView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Zneg );
			}
		}

		public void RightView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Xpos );
			}
		}

		public void LeftView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_Xneg );
			}
		}

		public void IsometricView()
		{
			if( myView != null ) {
				myView.SetProj( V3d_TypeOfOrientation.V3d_XposYnegZpos );
			}
		}

		public void SetViewDir( gp_Dir dir )
		{
			if( myView != null ) {
				myView.SetProj( dir.X(), dir.Y(), dir.Z() );
			}
		}

		public void ZoomAllView()
		{
			if( myView != null ) {
				myView.FitAll();
				myView.ZFitAll();
			}
		}

		public double Scale()
		{
			if( myView != null ) {
				return myView.Scale();
			}
			else {
				return -1;
			}
		}

		public void ResetView()
		{
			if( myView != null ) {
				myView.Reset();
			}
		}

		public void SetBackgroundColor( int r, int g, int b )
		{
			if( myView != null ) {
				myView.SetBackgroundColor( Quantity_TypeOfColor.Quantity_TOC_RGB, r / 255.0, g / 255.0, b / 255.0 );
			}
		}

		public double GetVersion()
		{
			return Standard_Version.Number();
		}

		public bool ImportBRep( string filename )
		{
			TopoDS_Shape aShape = new TopoDS_Shape();
			BRep_Builder aBuilder = new BRep_Builder();
			bool result = BRepTools.Read( ref aShape, filename, aBuilder );
			if( !result )
				return false;
			myAISContext.Display( new AIS_Shape( aShape ), true );
			return true;
		}

		public bool ImportCsfdb( string filename )
		{
			return false;
		}

		public bool ImportIges( string filename )
		{
			bool res = false;
			IGESControl_Reader aReader = new IGESControl_Reader();
			IFSelect_ReturnStatus aStatus = aReader.ReadFile( filename );
			if( aStatus == IFSelect_ReturnStatus.IFSelect_RetDone ) {
				res = true;
				aReader.TransferRoots();
				TopoDS_Shape aShape = aReader.OneShape();
				myAISContext.Display( new AIS_Shape( aShape ), true );
			}
			return res;
		}

		public bool ImportStep( string filename )
		{
			bool res = false;
			STEPControl_Reader aReader = new STEPControl_Reader();
			IFSelect_ReturnStatus aStatus = aReader.ReadFile( filename );
			if( aStatus == IFSelect_ReturnStatus.IFSelect_RetDone ) {
				bool failsonly = false;
				aReader.PrintCheckLoad( failsonly, IFSelect_PrintCount.IFSelect_ItemsByEntity );

				int nbr = aReader.NbRootsForTransfer();
				aReader.PrintCheckTransfer( failsonly, IFSelect_PrintCount.IFSelect_ItemsByEntity );
				for( int r = 1; r <= nbr; r++ ) {
					bool ok = aReader.TransferRoot( r );
					int nbs = aReader.NbShapes();
					if( nbs > 0 ) {
						res = true;
						for( int s = 1; s <= nbs; s++ ) {
							TopoDS_Shape aShape = aReader.Shape( s );
							myAISContext.Display( new AIS_Shape( aShape ), true );
						}
					}
				}
			}
			return res;
		}

		public bool Dump( string filename )
		{
			if( myView != null ) {
				myView.Redraw();
				return myView.Dump( filename );
			}
			return false;
		}

		public void OnVertices()
		{
			myAISContext.Deactivate();

			// 1 means vertex select mode
			myAISContext.Activate( 1 );

			// set 0 selection pixel tolerance in order to be more accurate when selecting Vertex
			myAISContext.SelectionManager().Selector().SetPixelTolerance( 0 );
		}

		public void OnCloseAllContexts()
		{
			myAISContext.Deactivate();

			// active 0 means default select mode
			myAISContext.Activate( 0 );

			// expand selection pixel tolerance in normal selection mode
			myAISContext.SelectionManager().Selector().SetPixelTolerance( SELECT_PixelTolerance );
		}

		public void CreateNewView( IntPtr wnd )
		{
			myView = myAISContext.CurrentViewer().CreateView();
			if( myGraphicDriver == null ) {
				Aspect_DisplayConnection aDisplayConnection = new Aspect_DisplayConnection();
				myGraphicDriver = new OpenGl_GraphicDriver( aDisplayConnection );
			}
			Aspect_Window aWNTWindow = new Aspect_Window( wnd, true );
			myView.SetWindow( aWNTWindow );
			int w = 100, h = 100;
			aWNTWindow.Size( ref w, ref h );
			if( !aWNTWindow.IsMapped() )
				aWNTWindow.Map();
		}

		public bool SetAISContext( OCCViewer Viewer )
		{
			myAISContext = Viewer.GetAISContext();
			return myAISContext != null;
		}

		public AIS_InteractiveContext GetAISContext()
		{
			return myAISContext;
		}

		public void StartZoomAtPoint( int nPointX, int nPointY )
		{
			if( myView != null ) {
				myView.StartZoomAtPoint( nPointX, nPointY );
			}
		}

		public void ZoomAtPoint( int nMouseStartX, int nMouseStartY, int nMouseEndX, int nMouseEndY )
		{
			if( myView != null ) {
				myView.ZoomAtPoint( nMouseStartX, nMouseStartY, nMouseEndX, nMouseEndY );
			}
		}

		//Convert mouse position(pixel coordinate) to draft coordinate
		public void Convert( int Xp, int Yp, ref double X, ref double Y, ref double Z )
		{
			if( myView != null ) {
				myView.Convert( Xp, Yp, ref X, ref Y, ref Z );
			}
		}

		//Convert draft coordinate to pixel coordinate
		public void Convert( double X, double Y, double Z, ref int Xp, ref int Yp )
		{
			if( myView != null ) {
				myView.Convert( X, Y, Z, ref Xp, ref Yp );
			}
		}

		public void Eye( ref double eyeX, ref double eyeY, ref double eyeZ )
		{
			if( myView != null ) {
				myView.Eye( ref eyeX, ref eyeY, ref eyeZ );
			}
		}

		public void At( ref double atX, ref double atY, ref double atZ )
		{
			if( myView != null ) {
				myView.At( ref atX, ref atY, ref atZ );
			}
		}

		// to update Prs3d_Presentation instantly
		public void Redraw()
		{
			if( myView != null ) {
				myView.Redraw();
			}
		}
	}
}
