using System;
using System.Windows;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Wpf.Views;

namespace Nementis.CryptoBerryClient.UI.WPF {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		private bool _setupComplete;

		void DoSetup() {
			var presenter = new MvxSimpleWpfViewPresenter(MainWindow);

			var setup = new Setup(Dispatcher, presenter);
			setup.Initialize();

			var start = Mvx.Resolve<IMvxAppStart>();
			start.Start();

			_setupComplete = true;
		}

		protected override void OnActivated(EventArgs e) {
			if (!_setupComplete) {
				DoSetup();
			}

			base.OnActivated(e);
		}
	}
}
