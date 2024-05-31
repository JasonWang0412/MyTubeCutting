using System;
using System.ComponentModel;
using System.Globalization;

namespace MyUIDisplayModel
{
	public class MyObjectConverter : ExpandableObjectConverter
	{
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			return string.Empty;
		}
	}
}
