using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Nementis.CryptoBerryServer.ApplicationGateway;
using Nementis.CryptoBerryServer.Telemetry;

namespace Nementis.CryptoBerryServer.Servers {
	public class TcpServer {

		private const string ConfigurationPort = "18975";

		private readonly MessageQueue _messageQueue;

		private StreamSocketListener _tcpListener;

		public TcpServer( MessageQueue messageQueue ) {
			_messageQueue = messageQueue;
		}

		public async Task StartTcpServer() {

			if ( _tcpListener == null ) {
				_tcpListener = new StreamSocketListener();
				_tcpListener.ConnectionReceived += TcpListenerOnConnectionReceived;

				_tcpListener.Control.KeepAlive = false;
			}

			try {
				await _tcpListener.BindServiceNameAsync( ConfigurationPort );
			}
			catch ( Exception ex ) {
				Debug.WriteLine( "TcpServer.StartTcpServer() Error: " + ex.Message );

				// Log telemetry event about this exception
				var events = new Dictionary<string, string> { { "TcpServer", ex.Message } };
				TelemetryHelper.TrackEvent( "FailedToStartTcpServer", events );
			}
		}

		public async Task StopTcpServer() {
			await _tcpListener.CancelIOAsync();
			_tcpListener.Dispose();
			_tcpListener = null;
		}
		private async void TcpListenerOnConnectionReceived( StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args ) {

			GatewayMessage newConnectionMessage = new GatewayMessage {
				Source = MessageSource.TcpServer,
				Type = MessageType.ClientConnected
			};

			_messageQueue.Enqueue( newConnectionMessage );

			DataReader clientReader = new DataReader( args.Socket.InputStream );

			while ( true ) {

				uint messageSizeReadBytes = await clientReader.LoadAsync( sizeof( uint ) );
				if ( messageSizeReadBytes != sizeof( uint ) ) {
					//=== The underlying socket was closed before we can read message data
					return;
				}

				//=== Read the message
				uint messageSize = clientReader.ReadUInt32();
				uint actualMessageSize = await clientReader.LoadAsync( messageSize );
				if ( messageSize != actualMessageSize ) {
					//=== The underlying socket was closed before we can read message data
					return;
				}

				byte[] encryptedMessage = new byte[messageSize];
				clientReader.ReadBytes( encryptedMessage );

				GatewayMessage newMessage = new GatewayMessage {
					Source = MessageSource.TcpServer,
					Type = MessageType.TcpEncryptedMessage,
					MessageData = encryptedMessage
				};

				_messageQueue.Enqueue( newMessage );
			}
		}
	}
}
