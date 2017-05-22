using MvvmCross.Core.ViewModels;

namespace CryptoBerryClient.Core.ViewModels {
	public class MainViewModel : MvxViewModel {

		private IMvxCommand _serverDisocveryCommand;

		public IMvxCommand ServerDiscoveryCommand => _serverDisocveryCommand ?? ( _serverDisocveryCommand = new MvxCommand( ServerDiscoverCommandAsync ) );

		private void ServerDiscoverCommandAsync() {
			ShowViewModel<ServerDiscoveryViewModel>();
		}
	}
}
