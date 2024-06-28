using System.Windows.Forms;

namespace MyOCCViewer
{
	public class ViewerMouseAction
	{
		public static void MouseWheel( MouseEventArgs e, OCCViewer Viewer )
		{
			// zoom viewer at start point
			Viewer.StartZoomAtPoint( e.X, e.Y );

			int nEndX = (int)( e.X + e.X * e.Delta * ZOOM_Ratio );
			int nEndY = (int)( e.Y + e.Y * e.Delta * ZOOM_Ratio );

			// zoom viewer with mouse wheel delta and scaling ratio
			Viewer.ZoomAtPoint( e.X, e.Y, nEndX, nEndY );
		}

		public static void MouseDown( MouseEventArgs e, OCCViewer Viewer, ref int nXMousePosition, ref int nYMousePosition )
		{
			switch( e.Button ) {
				// press down middle button, then start translate the viewer
				case MouseButtons.Middle:
					nXMousePosition = e.X;
					nYMousePosition = e.Y;
					break;
				// press down right button, then start rotatae the viewer
				case MouseButtons.Right:
					Viewer.StartRotation( e.X, e.Y );
					break;
				default:
					break;
			}
		}

		public static void MouseMove( MouseEventArgs e, OCCViewer Viewer, ref int nXMousePosition, ref int nYMousePosition )
		{
			Viewer.MoveTo( e.X, e.Y );

			switch( e.Button ) {

				// translate the viewer
				case MouseButtons.Middle:
					Viewer.Pan( e.X - nXMousePosition, nYMousePosition - e.Y );
					nXMousePosition = e.X;
					nYMousePosition = e.Y;
					break;

				// rotate the viewer
				case MouseButtons.Right:
					Viewer.Rotation( e.X, e.Y );
					break;
				default:
					break;
			}
		}

		const double ZOOM_Ratio = 0.0002;
	}
}
