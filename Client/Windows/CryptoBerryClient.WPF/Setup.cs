using System.Windows.Threading;
using CryptoBerryClient.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.Wpf.Platform;
using MvvmCross.Wpf.Views;
using Windows.Storage.Streams;

namespace Nementis.CryptoBerryClient.UI.WPF {
	public class Setup : MvxWpfSetup {

		public Setup( Dispatcher uiThreadDispatcher, IMvxWpfViewPresenter presenter )
			: base( uiThreadDispatcher, presenter ) {}

		protected override IMvxApplication CreateApp() {
			return new AppCore();
		}
	}
}
