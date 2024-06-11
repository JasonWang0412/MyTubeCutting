using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyUIDisplayModel
{
	public struct OriginalLayoutData
	{
		public OriginalLayoutData( Rectangle originalRect, float originalFontSize )
		{
			LayoutRect = originalRect;
			FontSize = originalFontSize;
		}

		public Rectangle LayoutRect
		{
			get;
			private set;
		}

		public float FontSize
		{
			get;
			private set;
		}
	}

	public partial class Relayout
	{
		// This should call right after initialize component
		public static Dictionary<Control, OriginalLayoutData> GetAllSubControlLayoutDic( Control c )
		{
			Dictionary<Control, OriginalLayoutData> Dic = new Dictionary<Control, OriginalLayoutData>();

			// Go through all subcontrols
			foreach( Control subControl in c.Controls ) {

				// Add default layout data into dictionary
				Dic.Add( subControl, new OriginalLayoutData( new Rectangle( subControl.Location.X, subControl.Location.Y, subControl.Width, subControl.Height ), subControl.Font.Size ) );

				// Recursive when subcontrol contains further subcontrols
				if( subControl is Panel && subControl.Controls.Count != 0 ) {
					foreach( KeyValuePair<Control, OriginalLayoutData> pair in GetAllSubControlLayoutDic( subControl ) ) {
						Dic.Add( pair.Key, pair.Value );
					}
				}
			}
			return Dic;
		}

		public static void RelayoutControl( Control c, OriginalLayoutData data, float fRatioX, float fRatioY, bool bResizeFont = false )
		{
			// Calculate new location
			int nNewLocation_x = (int)( data.LayoutRect.X * fRatioX );
			int nNewLocation_y = (int)( data.LayoutRect.Y * fRatioY );

			// Calculate new size
			int nNewWidth = (int)( data.LayoutRect.Width * fRatioX );
			int nNewHeight = (int)( data.LayoutRect.Height * fRatioY );

			// Relayout the control
			if( bResizeFont ) {
				c.Font = new Font( c.Font.FontFamily, data.FontSize * Math.Min( fRatioX, fRatioY ), c.Font.Style, c.Font.Unit, c.Font.GdiCharSet );
			}
			c.Location = new Point( nNewLocation_x, nNewLocation_y );
			c.Size = new Size( nNewWidth, nNewHeight );
		}
	}

	public partial class Relayout
	{
	}

	public partial class Relayout
	{
	}
}
