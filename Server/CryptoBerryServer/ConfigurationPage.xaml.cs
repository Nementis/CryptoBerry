using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nementis.CryptoBerryServer {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ConfigurationPage : Page {

		private const int DisplayTimeout = 60;
		private readonly DispatcherTimer _displayTimer;

		public ConfigurationPage() {
			InitializeComponent();

			_displayTimer = new DispatcherTimer();
			_displayTimer.Interval = TimeSpan.FromSeconds( DisplayTimeout );
			_displayTimer.Tick += DisplayTimerOnTick;
		}

		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );
			if ( e.Parameter != null ) {
				KeyText.Text = e.Parameter.ToString();
			}

			_displayTimer.Start();
		}

		private void DisplayTimerOnTick( object sender, object o ) {
			Frame rootFrame = Window.Current.Content as Frame;

			if ( rootFrame != null ) {
				if ( rootFrame.CanGoBack ) {
					rootFrame.GoBack();
				}
			}

			_displayTimer.Stop();
		}

	}
}
