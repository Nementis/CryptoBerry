using System;
using System.Text;
using System.Threading.Tasks;
using CryptoBerryClient.Core.Models.Interfaces;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;

namespace CryptoBerryClient.Core.Models {
	public class UdpClientCommunicationService : IUdpClientCommunication {

		private const string DiscoveryMessage = @"CryptoBerry-ServerDiscovery";

		/// <summary>
		/// Local port used for UDP communications
		/// </summary>
		private const int LocalUdpPort = 13688;

		/// <summary>
		/// Server port used for UDP communicaions
		/// </summary>
		private const int ServerUdpPort = 13687;

		/// <summary>
		/// UdpClient object used to receive messages from the server
		/// </summary>
		private UdpSocketReceiver _udpListener;

		/// <summary>
		/// UdpClient object used to send messages to the server
		/// </summary>
		private UdpSocketClient _udpSender;


		public async Task ConfigureListener( EventHandler<UdpSocketMessageReceivedEventArgs> messageHandler) {
		
			_udpListener = new UdpSocketReceiver();

			_udpListener.MessageReceived += messageHandler;

			_udpSender = new UdpSocketClient();

			await _udpListener.StartListeningAsync( LocalUdpPort );
		}

		public async Task SendDiscoveryMessageAsync() {

			if ( _udpListener == null || _udpSender == null ) {
				return;
			}

			var messageBytes = Encoding.UTF8.GetBytes( DiscoveryMessage );

			await _udpSender.SendToAsync( messageBytes, "255.255.255.255", ServerUdpPort );
		}

		public async Task ShutdownListener() {

			await _udpListener.StopListeningAsync();
			_udpListener.Dispose();
			_udpListener = null;

			await _udpSender.DisconnectAsync();
			_udpSender.Dispose();
			_udpSender = null;
		}
	}
}
