using System.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Nementis.CryptoBerryServer.Encryption {
	public class EncryptionEngine {

		private SymmetricKeyAlgorithmProvider _simmetricKeyProvider;

		private RSACng _rsaCryptoServiceProvider;

		public void Initialize( string keyCode ) {

			//=== Initialize the key algorythm to AES CBC
			_simmetricKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm( SymmetricAlgorithmNames.AesCbc );
		}
	}
}
