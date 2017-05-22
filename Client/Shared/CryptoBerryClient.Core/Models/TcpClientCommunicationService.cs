using System;
using System.Text;
using System.Threading.Tasks;
using CryptoBerryClient.Core.Models.Interfaces;
using Nementis.SharedObjects.Messages;
using Newtonsoft.Json;
using Sockets.Plugin;

namespace CryptoBerryClient.Core.Models {

	public class TcpClientCommunicationService : ITcpClientCommunication {

		private const string ServerConfigurationPort = "18975";

		private readonly TcpSocketClient _tcpClient;

		public TcpClientCommunicationService() {
			_tcpClient = new TcpSocketClient();
		}

		public async Task ConnectServerAsync( string serverAddress ) {
			await _tcpClient.ConnectAsync( serverAddress, ServerConfigurationPort );
		}

		public async Task SendAsync( TcpDataMessge message ) {

			string messsageString = JsonConvert.SerializeObject( message );
			byte[] messageBytes = Encoding.UTF8.GetBytes( messsageString );
			byte[] messageLengthBytes = BitConverter.GetBytes( messageBytes.Length );
			await _tcpClient.WriteStream.WriteAsync( messageLengthBytes, 0, messageLengthBytes.Length );
			await _tcpClient.WriteStream.WriteAsync( messageBytes, 0, messageBytes.Length );
			await _tcpClient.WriteStream.FlushAsync();
		}
	}
}
