using System.Threading.Tasks;
using Windows.Storage.Streams;
using MvvmCross.Core.ViewModels;
using CryptoBerryClient.Core.Models.Interfaces;
using Nementis.SharedObjects.DataObjects;
using Nementis.SharedObjects.Messages;
using Newtonsoft.Json;

namespace CryptoBerryClient.Core.ViewModels {
	public class ConfigureServerViewModel : MvxViewModel {

		private readonly IEncryptionEngine _encryptionEngine;

		private readonly ITcpClientCommunication _tcpClientCommunication;

		private string _securityKey;

		private string _serverAddress;

		public ConfigureServerViewModel( IEncryptionEngine encryptionEngine,
			ITcpClientCommunication tcpClientCommunication ) {

			_encryptionEngine = encryptionEngine;

			_tcpClientCommunication = tcpClientCommunication;
		}

		public void Init( string serverAddress ) {
			_serverAddress = serverAddress;
		}

		private IMvxCommand _connectServerCommnad;

		public string SecurityKey {
			get => _securityKey;
			set => SetProperty( ref _securityKey, value );
		}

		public IMvxCommand ConnectServerCommand => _connectServerCommnad ?? ( _connectServerCommnad = new MvxAsyncCommand( ExecuteConnectServerCommnadAsync ) );

		private async Task ExecuteConnectServerCommnadAsync() {
			_encryptionEngine.Initialize( _securityKey );
			await _tcpClientCommunication.ConnectServerAsync( _serverAddress );

			IBuffer initializationVector;
			IBuffer encryptedData = _encryptionEngine.EncryptWithSessionKey( _securityKey, out initializationVector );

			SessionStart sessionStartData = new SessionStart { EncryptedData = encryptedData, InitializationVector = initializationVector };

			string sessionStartDataString = JsonConvert.SerializeObject( sessionStartData );

			TcpDataMessge message = new TcpDataMessge { MessageType = TcpDataMessageType.StartSession, MessageData = sessionStartDataString };

			await _tcpClientCommunication.SendAsync( message );
		}
	}
}
