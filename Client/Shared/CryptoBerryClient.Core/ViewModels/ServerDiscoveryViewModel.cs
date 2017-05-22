using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CryptoBerryClient.Core.Models.Interfaces;
using CryptoBerryClient.Core.Utility;
using MvvmCross.Core.ViewModels;
using Nementis.SharedObjects.DataObjects;
using Newtonsoft.Json;
using Sockets.Plugin.Abstractions;

namespace CryptoBerryClient.Core.ViewModels {
	public class ServerDiscoveryViewModel : MvxViewModel {

		private const int DiscoveryTimeout = 3;

		private readonly IUdpClientCommunication _udpCommunicationClient;

		private readonly DeepObservableCollection<CryptoBerryServerInfo> _discoveredServers;

		private CryptoBerryServerInfo _selectedServer;

		private bool _discoveryActive;

		private IMvxCommand _discoverServerCommand;

		private IMvxCommand _updateSelectedServerCommand;

		private IMvxCommand _configureServerCommand;

		public ServerDiscoveryViewModel( IUdpClientCommunication udpCommunicationCLient ) {

			_udpCommunicationClient = udpCommunicationCLient;

			_udpCommunicationClient.ConfigureListener( MessageHandler );

			_discoveredServers = new DeepObservableCollection<CryptoBerryServerInfo>();
		}

		public DeepObservableCollection<CryptoBerryServerInfo> DiscoveredServers => _discoveredServers;

		public CryptoBerryServerInfo SelectedServer {
			get => _selectedServer;
			set => SetProperty( ref _selectedServer, value );
		}

		public bool DiscoveryActive => _discoveryActive;

		public string DiscoveryStatus => _discoveryActive ? "Discovering Servers" : "Inactive";

		public IMvxCommand DiscoverServerCommand => _discoverServerCommand ?? ( _discoverServerCommand = new MvxAsyncCommand( ExecuteDiscoverServerCommandAsync ) );

		public IMvxCommand UpdateSelectedServerCommand => _updateSelectedServerCommand ?? (_updateSelectedServerCommand = new MvxCommand(ExecuteUpdateSelectedServerCommand) );

		public IMvxCommand ConfigureServerCommand => _configureServerCommand ?? (_configureServerCommand = new MvxAsyncCommand(ExecuteConfigureServerCommandAsync, CanExecuteConfigureServer));

		private async Task ExecuteConfigureServerCommandAsync() {
			ShowViewModel<ConfigureServerViewModel>( new { serverAddress = SelectedServer.Address } );
		}

		private bool CanExecuteConfigureServer() {
			return !_selectedServer?.IsConfigured ?? false;
		}

		private void ExecuteUpdateSelectedServerCommand() {
			ConfigureServerCommand.RaiseCanExecuteChanged();
		}

		private async Task ExecuteDiscoverServerCommandAsync() {

			_discoveredServers.Clear();

			_discoveryActive = true;

			//=== Force evaluation of properties to show to the user visual feedback on the discovery operation
			RaisePropertyChanged( nameof( DiscoveryActive ) );
			RaisePropertyChanged( nameof( DiscoveryStatus ) );

			await _udpCommunicationClient.SendDiscoveryMessageAsync();

			await Task.Delay( TimeSpan.FromSeconds( DiscoveryTimeout ) );

			_discoveryActive = false;

			RaisePropertyChanged( nameof( DiscoveryActive ) );
			RaisePropertyChanged( nameof( DiscoveryStatus ) );
		}

		private void MessageHandler( object sender, UdpSocketMessageReceivedEventArgs eventArgs ) {

			string messageString;
			try {
				messageString = Encoding.UTF8.GetString( eventArgs.ByteData, 0, eventArgs.ByteData.Length );
			}
			catch ( Exception ex ) {
				Debug.WriteLine( $"Error {ex.Message} while getting UDP message string from bytes");
				return;
			}

			CryptoBerryServerInfo serverInformation;
			try {
				serverInformation = JsonConvert.DeserializeObject<CryptoBerryServerInfo>( messageString );
			}
			catch ( Exception ex ) {
				Debug.WriteLine( $"Error {ex.Message} while deserializing CryptoBerryServer object from string" );
				return;
			}

			Dispatcher.RequestMainThreadAction( () => { _discoveredServers.Add( serverInformation); } );
		}
	}
}
