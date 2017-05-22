using Windows.Storage.Streams;

namespace Nementis.SharedObjects.DataObjects {
	public class SessionStart {

		public IBuffer InitializationVector { get; set; }

		public IBuffer EncryptedData { get; set; }
	}
}
