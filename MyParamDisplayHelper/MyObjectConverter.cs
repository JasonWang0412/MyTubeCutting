using System;
using System.ComponentModel;
using System.Globalization;

namespace MyParamDisplayHelper
{
	public class MyObjectConverter : ExpandableObjectConverter
	{
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			return string.Empty;
		}
	}
}
