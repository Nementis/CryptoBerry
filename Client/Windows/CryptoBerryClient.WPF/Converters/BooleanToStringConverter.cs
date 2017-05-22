using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nementis.CryptoBerryClient.UI.WPF.Converters {
	public class BooleanToStringConverter : MarkupExtension, IValueConverter {

		private static BooleanToStringConverter _booleanToStringConverter;

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
			if ( value is bool ) {
				return (bool)value ? "Yes" : "No";
			}
			return Binding.DoNothing;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
			return Binding.DoNothing;
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return _booleanToStringConverter ?? ( _booleanToStringConverter = new BooleanToStringConverter() );
		}
	}
}
