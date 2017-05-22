using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace Nementis.CryptoBerryServer.Telemetry {
	internal static class TelemetryHelper {

		internal static TelemetryClient TelemetryClient = new TelemetryClient();

		private static void AddCommonProperties( ref IDictionary<string, string> properties ) {
			// add common properties as long as they don't already exist in the original properties passed in
			if ( !properties.ContainsKey( "Custom_AppVersion" ) ) {
				properties.Add( "Custom_AppVersion", EnvironmentSettings.GetAppVersion() );
			}
			if ( !properties.ContainsKey( "Custom_OSVersion" ) ) {
				properties.Add( "Custom_OSVersion", EnvironmentSettings.GetOSVersion() );
			}
		}
		/// <summary>
		/// Log and track custom app insights metrics with global common properities
		/// See https://azure.microsoft.com/en-us/documentation/articles/app-insights-api-custom-events-metrics/#api-summary
		/// </summary>
		/// <param name="metricName"></param>
		/// <param name="value"></param>
		internal static void TrackMetric(string metricName, double value) {
			Dictionary<string, string> properties = new Dictionary<string, string>();
			// call overloaded internal helper method
			TrackMetric(metricName, value, properties);
		}

		/// <summary>
		/// Log and track custom app insights metrics with global common properities
		/// See https://azure.microsoft.com/en-us/documentation/articles/app-insights-api-custom-events-metrics/#api-summary
		/// </summary>
		/// <param name="metricName"></param>
		/// <param name="value"></param>
		/// <param name="properties"></param>
		internal static void TrackMetric(string metricName, double value, IDictionary<string, string> properties) {
			// add common properties
			AddCommonProperties(ref properties);
			TelemetryClient.TrackMetric(metricName, value, properties);
		}

		/// <summary>
		/// Log and track custom app insights event with global common properities
		/// See https://azure.microsoft.com/en-us/documentation/articles/app-insights-api-custom-events-metrics/#api-summary
		/// </summary>
		/// <param name="eventName"></param>
		internal static void TrackEvent(string eventName) {
			Dictionary<string, string> properties = new Dictionary<string, string>();
			// call overloaded internal helper method
			TrackEvent(eventName, properties);
		}

		/// <summary>
		/// Log and track custom app insights event with global common properities
		/// See https://azure.microsoft.com/en-us/documentation/articles/app-insights-api-custom-events-metrics/#api-summary
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="properties"></param>
		internal static void TrackEvent(string eventName, IDictionary<string, string> properties) {
			// add common properties
			AddCommonProperties(ref properties);
			TelemetryClient.TrackEvent(eventName, properties);
		}

	}
}
