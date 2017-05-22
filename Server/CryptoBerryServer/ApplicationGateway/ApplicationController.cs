using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Nementis.CryptoBerryServer.Configuration;
using Nementis.CryptoBerryServer.Encryption;
using Nementis.CryptoBerryServer.Servers;
using Nementis.CryptoBerryServer.Telemetry;

namespace Nementis.CryptoBerryServer.ApplicationGateway {
	public class ApplicationController {

		private const int MessageTimeout = 500;

		private const int CodeLength = 15;

		private readonly EncryptionEngine _encryptionEngine;

		private readonly UdpServer _udpServer;

		private readonly TcpServer _tcpServer;

		private readonly BackgroundWorker _applicationGateway;

		private readonly MessageQueue _messageQueue;

		private Window _mainWindow;

		private string _currentKeyCode;

		public ApplicationController() {

			_messageQueue = new MessageQueue();

			_encryptionEngine = new EncryptionEngine();

			_udpServer = new UdpServer( this, _messageQueue );

			_tcpServer = new TcpServer( _messageQueue );

			_applicationGateway = new BackgroundWorker();

			_applicationGateway.WorkerSupportsCancellation = true;

			_applicationGateway.DoWork += ApplicationGatewayWork;
			_applicationGateway.RunWorkerCompleted += ApplicationGatewayShutdown;
		}

		/// <summary>
		/// Configuration settings for app
		/// </summary>
		public ApplicationSettings XmlSettings;

		/// <summary>
		/// Provides status if the controller hae been initialized or not
		/// </summary>
		public bool IsInitialized { get; protected set; }

		public async Task Initialize( Window currentWindow ) {

			_mainWindow = currentWindow;

			try {

				//=== Check il server RSA key is created
				if ( CngKey.Exists( "CryptoberryKey", CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.None ) ) {
					//=== Load application settings from file system
					XmlSettings = await ApplicationSettings.LoadSettingsAsync( "settings.xml" );

					IsInitialized = true;
				}

				//=== Start UDP Service
				await _udpServer.StartUdpServer();

				//=== Start TP Server
				await _tcpServer.StartTcpServer();

				//=== Start internal message listener
				_applicationGateway.RunWorkerAsync();
			}
			catch (Exception ex) {
				Debug.WriteLine("Controller.Initialize() Error: " + ex.Message);

				// Log telemetry event about this exception
				var events = new Dictionary<string, string> { { "Controller", ex.Message } };
				TelemetryHelper.TrackEvent("FailedToInitialize", events);
			}
		}
		private void ApplicationGatewayWork( object sender, DoWorkEventArgs e ) {

			BackgroundWorker currentWorker = (BackgroundWorker) sender;
			do {
				GatewayMessage currentMessage = _messageQueue.GetMessage( MessageTimeout );

				switch ( currentMessage?.Source ) {
					case MessageSource.CloudStorageManager:
						ProcessCloudStorageMessage( currentMessage );
						break;

					case MessageSource.EncryptionEngine:
						ProcessEncryptionEngineMessage( currentMessage );
						break;

					case MessageSource.FileSystemWatcher:
						ProcessFileSystemWatcherMessage( currentMessage );
						break;

					case MessageSource.TcpServer:
						ProcessTcpServerMessge( currentMessage );
						break;

					case MessageSource.UdpServer:
						ProcessUdpServerMessge( currentMessage );
						break;
				}
			} while ( !currentWorker.CancellationPending );
		}

		private void ProcessUdpServerMessge( GatewayMessage currentMessage ) {

			switch ( currentMessage.Type ) {
				case MessageType.ServerDiscovered:
					ProcessServerDiscoveredMessage();
					break;
			}
		}

		private async void ProcessServerDiscoveredMessage() {
			await _mainWindow.Dispatcher.RunAsync( CoreDispatcherPriority.Normal, AgileCallback );
		}

		private void AgileCallback() {
			Frame rootFrame = Window.Current.Content as Frame;
			if ( rootFrame == null ) {
				rootFrame = new Frame();
				Window.Current.Content = rootFrame;
			}

			_currentKeyCode = GenerateString( CodeLength );

			rootFrame.Navigate( typeof( ConfigurationPage ), _currentKeyCode );
		}

		private void ApplicationGatewayShutdown( object sender, RunWorkerCompletedEventArgs e ) {
			throw new NotImplementedException();
		}

		private void ProcessTcpServerMessge( GatewayMessage currentMessage ) {

			switch ( currentMessage.Type ) {
				case MessageType.ClientConnected:
					ProcessClientConnectedMessage( currentMessage );
					break;
			}
		}

		private void ProcessFileSystemWatcherMessage( GatewayMessage currentMessage ) {
			throw new NotImplementedException();
		}

		private void ProcessEncryptionEngineMessage( GatewayMessage currentMessage ) {
			throw new NotImplementedException();
		}

		private void ProcessCloudStorageMessage( GatewayMessage currentMessage ) {
			throw new NotImplementedException();
		}

		private void ProcessClientConnectedMessage( GatewayMessage currentMessage ) {
			if ( !IsInitialized ) {
				
				//=== Initialize Encryption Engine
				_encryptionEngine.Initialize( _currentKeyCode );
			}
		}
	public string GenerateString( int size ) {
		Random rand = new Random();

		string alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			char[] chars = new char[size];
			for ( int i = 0; i < size; i++ ) {
				chars[i] = alphabet[rand.Next( alphabet.Length )];
			}
			return new string( chars );
		}
	}
}
