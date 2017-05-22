using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nementis.SharedObjects.DataObjects;
using Sockets.Plugin.Abstractions;

namespace CryptoBerryClient.Core.Models.Interfaces {
	public interface IUdpClientCommunication {

		Task ConfigureListener( EventHandler<UdpSocketMessageReceivedEventArgs> messageHandler );

		Task SendDiscoveryMessageAsync();

		Task ShutdownListener();

	}
}
