using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using CryptoBerryClient.Core.Models.Interfaces;

namespace CryptoBerryClient.Core.Models {
	class EncryptionEngineService : IEncryptionEngine {

		private const int IterationsCount = 10000;

		private const int ActualKeySize = 256;

		private CryptographicKey _serverSessionKey;

		public bool Initialize( string securityKey ) {

			KeyDerivationAlgorithmProvider keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm( KeyDerivationAlgorithmNames.Pbkdf2Sha256 );

			IBuffer bufferSecret = CryptographicBuffer.ConvertStringToBinary( securityKey, BinaryStringEncoding.Utf8 );
			IBuffer bufferSalt = CryptographicBuffer.GenerateRandom( 256 );

			KeyDerivationParameters keyParameters = KeyDerivationParameters.BuildForPbkdf2( bufferSalt, IterationsCount );

			CryptographicKey baseKey = keyDerivationProvider.CreateKey( bufferSecret );

			IBuffer serverSessionKeyMaterial = CryptographicEngine.DeriveKeyMaterial( baseKey, keyParameters, ActualKeySize );

			_serverSessionKey = keyDerivationProvider.CreateKey( serverSessionKeyMaterial );
			return true;
		}

		public IBuffer EncryptWithSessionKey( string message, out IBuffer initializationVector ) {

			IBuffer returnValue;

			SymmetricKeyAlgorithmProvider symmetricProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm( SymmetricAlgorithmNames.AesCbc );

			initializationVector = CryptographicBuffer.GenerateRandom( ActualKeySize );

			IBuffer bufferMessage = CryptographicBuffer.ConvertStringToBinary( message, BinaryStringEncoding.Utf8 );

			returnValue = CryptographicEngine.Encrypt( _serverSessionKey, bufferMessage, initializationVector );

			return returnValue;
		}
	}
}
