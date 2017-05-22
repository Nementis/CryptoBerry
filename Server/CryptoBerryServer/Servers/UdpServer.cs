using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Nementis.CryptoBerryServer.ApplicationGateway;
using Nementis.CryptoBerryServer.Extensions;
using Nementis.CryptoBerryServer.Telemetry;
using Nementis.SharedObjects.DataObjects;
using Newtonsoft.Json;

namespace Nementis.CryptoBerryServer.Servers {
	public class UdpServer {

		private const string DiscoveryMessage = @"CryptoBerry-ServerDiscovery";

		private const string ListenPort = "13687";

		private const string ClientPort = "13688";

		private readonly ApplicationController _applicationController;

		private readonly MessageQueue _messageQueue;

		private DatagramSocket _udpSocket;

		public UdpServer( ApplicationController applicationController, MessageQueue messageQueue ) {

			_applicationController = applicationController;

			_messageQueue = messageQueue;
		}

		public async Task StartUdpServer() {
			try {
				if ( _udpSocket == null ) {

					_udpSocket = new DatagramSocket();
					_udpSocket.MessageReceived += SocketOnMessageReceived;

					await _udpSocket.BindServiceNameAsync( ListenPort );
				}
			}
			catch (Exception ex) {
				Debug.WriteLine("UdpServer.StartUdpServer() Error: " + ex.Message);

				// Log telemetry event about this exception
				var events = new Dictionary<string, string> { { "UdpServer", ex.Message } };
				TelemetryHelper.TrackEvent("FailedToStartUdpServer", events);
			}
		}

		private async void SocketOnMessageReceived( DatagramSocket datagramSocket, DatagramSocketMessageReceivedEventArgs args ) {
			try {
				//===Read the message that was received from the UDP echo client.
				Stream streamIn = args.GetDataStream().AsStreamForRead();
				StreamReader reader = new StreamReader( streamIn );
				string message = await reader.ReadLineAsync();

				if ( message.Equals( DiscoveryMessage ) ) {
					DatagramSocket replySocket = new DatagramSocket();
					Stream streamOut = ( await replySocket.GetOutputStreamAsync( args.RemoteAddress, ClientPort ) ).AsStreamForWrite();
					StreamWriter writer = new StreamWriter( streamOut );
					EasClientDeviceInformation eas = new EasClientDeviceInformation();
					CryptoBerryServerInfo currentServer =
						new CryptoBerryServerInfo {
																				Address = GetServerAddress( args.RemoteAddress ),
																				Name = eas.FriendlyName,
																				IsConfigured = _applicationController.IsInitialized
																			};
					string messageString = JsonConvert.SerializeObject( currentServer );
					await writer.WriteLineAsync( messageString );
					await writer.FlushAsync();

					GatewayMessage serverDiscoveredMessage = new GatewayMessage { Source = MessageSource.UdpServer, Type = MessageType.ServerDiscovered };
					_messageQueue.Enqueue( serverDiscoveredMessage );
				}
			}
			catch (Exception ex) {
				Debug.WriteLine("UdpServer.SocketOnMessageReceived() Error: " + ex.Message);

				// Log telemetry event about this exception
				var events = new Dictionary<string, string> { { "UdpServer", ex.Message } };
				TelemetryHelper.TrackEvent("FailedToSendDiscoveryReply", events);
			}
		}

		public async Task StopUdpServer() {
			await _udpSocket.CancelIOAsync();
			_udpSocket.Dispose();
			_udpSocket = null;
		}

		private string GetServerAddress( HostName clientAddress ) {

			string returnValue = string.Empty;

			IPAddress clientIpAddress = null;

			try {
				clientIpAddress = IPAddress.Parse( clientAddress.CanonicalName );
			}
			catch ( Exception ex ) {
				Debug.WriteLine( "UdpServer.GetServerAddress() Error: " + ex.Message );

				// Log telemetry event about this exception
				var events = new Dictionary<string, string> { { "UdpServer", ex.Message } };
				TelemetryHelper.TrackEvent( "FailedToConvertClientIp", events );
			}

			var hostNames = NetworkInformation.GetHostNames();
			foreach ( var hostName in hostNames ) {
				if ( hostName.Type == HostNameType.Ipv4 ) {
					IPAddress serverIpAddress = null;
					try {
						serverIpAddress = IPAddress.Parse( hostName.CanonicalName );
					}
					catch ( Exception ex ) {
						Debug.WriteLine( "UdpServer.GetServerAddress() Error: " + ex.Message );

						// Log telemetry event about this exception
						var events = new Dictionary<string, string> { { "UdpServer", ex.Message } };
						TelemetryHelper.TrackEvent( "FailedToConvertServerIp", events );
					}

					IPAddress subnetMask = IpAddressExtension.CreateByNetBitLength( hostName.IPInformation.PrefixLength ?? 24 );
					if ( clientIpAddress.IsInSameSubnet( serverIpAddress, subnetMask ) ) {
						returnValue = hostName.CanonicalName;
						break;
					}
				}
			}

			return returnValue;
		}
	}
}
