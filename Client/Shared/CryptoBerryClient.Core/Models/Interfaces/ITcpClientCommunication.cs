using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nementis.SharedObjects.Messages;

namespace CryptoBerryClient.Core.Models.Interfaces {
	public interface ITcpClientCommunication {

		Task ConnectServerAsync( string serverAddress );

		Task SendAsync( TcpDataMessge message );

	}
}
