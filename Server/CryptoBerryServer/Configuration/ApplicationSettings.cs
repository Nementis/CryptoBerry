using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Nementis.CryptoBerryServer.Telemetry;

namespace Nementis.CryptoBerryServer.Configuration {
	public class ApplicationSettings {

		public static readonly StorageFolder SettingsFolder = ApplicationData.Current.LocalFolder;

		public static async Task<ApplicationSettings> LoadSettingsAsync( string fileName ) {
			try {
				StorageFile sttingsFile = await SettingsFolder.CreateFileAsync( fileName, CreationCollisionOption.OpenIfExists );
				if (sttingsFile == null) {
					return new ApplicationSettings();
				}
				IInputStream sessionInputStream = await sttingsFile.OpenReadAsync();
				var serializer = new XmlSerializer(typeof(ApplicationSettings));
				ApplicationSettings returnValue = (ApplicationSettings)serializer.Deserialize(sessionInputStream.AsStreamForRead());
				sessionInputStream.Dispose();

				return returnValue;
			}
			catch ( TaskCanceledException ex ) {
				
				Debug.WriteLine( "AppplicationSettings.LoadSettingsAsync(): " + ex.Message );

				var events = new Dictionary<string, string> { { "ApplicationSettings", ex.Message } };
				TelemetryHelper.TrackEvent( "FailedToRestoreSettings", events );

				return new ApplicationSettings();
			}
		}
	}
}
