using CryptoBerryClient.Core.Models;
using CryptoBerryClient.Core.Models.Interfaces;
using CryptoBerryClient.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace CryptoBerryClient.Core {
	public class AppCore : MvxApplication {

		public AppCore() {
			Mvx.RegisterType<IUdpClientCommunication, UdpClientCommunicationService>();
			Mvx.RegisterType<ITcpClientCommunication, TcpClientCommunicationService>();
			Mvx.RegisterType<IEncryptionEngine, EncryptionEngineService>();
			Mvx.RegisterSingleton<IMvxAppStart>( new MvxAppStart<MainViewModel>() );
		}
	}
}
