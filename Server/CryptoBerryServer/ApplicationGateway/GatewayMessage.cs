namespace Nementis.CryptoBerryServer.ApplicationGateway {
	public class GatewayMessage {
		public MessageSource Source { get; set; }

		public MessageType Type { get; set; }

		public byte[] MessageData { get; set; }
	}
}
