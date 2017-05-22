using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nementis.CryptoBerryClient.UI.WPF.Converters {
	public class IpAddressToStringConverter : MarkupExtension, IValueConverter {

		private static IpAddressToStringConverter _ipAddressToStringConverter;

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
			if ( value is IPAddress ) {
				string ipString;
				try {
					ipString = ( (IPAddress)value ).ToString();
				}
				catch ( Exception ) {
					ipString = string.Empty;
				}

				return ipString;
			}
			return Binding.DoNothing;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
			if ( targetType == typeof( IPAddress ) ) {
				IPAddress ipAddress;
				if ( IPAddress.TryParse( value.ToString(), out ipAddress ) ) {
					return ipAddress;
				}
			}
			return Binding.DoNothing;
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return _ipAddressToStringConverter ?? ( _ipAddressToStringConverter = new IpAddressToStringConverter() );
		}
	}
}
