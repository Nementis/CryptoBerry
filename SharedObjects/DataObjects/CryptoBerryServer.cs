using System.ComponentModel;
using System.Runtime.CompilerServices;
using Nementis.SharedObjects.Annotations;

namespace Nementis.SharedObjects.DataObjects {
	public class CryptoBerryServerInfo : INotifyPropertyChanged {
		public string Address { get; set; }

		public string Name { get; set; }

		public bool IsConfigured { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

	}
}
