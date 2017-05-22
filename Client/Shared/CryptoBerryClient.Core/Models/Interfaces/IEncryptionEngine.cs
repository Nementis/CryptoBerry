using Windows.Storage.Streams;

namespace CryptoBerryClient.Core.Models.Interfaces {
	public interface IEncryptionEngine {

		bool Initialize( string securityKey );

		IBuffer EncryptWithSessionKey( string messageout, out IBuffer initializationVector );

	}
}
