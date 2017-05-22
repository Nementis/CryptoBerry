using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Nementis.CryptoBerryServer.ApplicationGateway {
	public class MessageQueue : IDisposable {

		private readonly ConcurrentQueue<GatewayMessage> _messageQueue;

		/// <summary>
		/// Manual reset event used to wakeup dispatcher queue, signaled when a new message is enqueued
		/// </summary>
		private readonly ManualResetEvent _messageReadyEvent;

		/// <summary>
		/// Flag used in IDisposable implementation to avoid multiple deallocations or resources
		/// </summary>
		private bool _disposed;

		public MessageQueue() {

			_messageReadyEvent = new ManualResetEvent( false );

			_messageQueue = new ConcurrentQueue<GatewayMessage>();
		}

		public bool Enqueue( GatewayMessage message ) {

			bool returnValue = true;

			_messageQueue.Enqueue( message );

			try {
				_messageReadyEvent.Set();
			}
			catch ( Exception ) {
				returnValue = false;
			}

			return returnValue;
		}

		public GatewayMessage GetMessage( int timeout ) {

			GatewayMessage returnValue;

			if ( !_messageReadyEvent.WaitOne( timeout ) ) {
				return null;
			}

			try {
				_messageReadyEvent.Reset();
			}
			catch ( Exception ) {
				return null;
			}

			if ( !_messageQueue.TryDequeue( out returnValue ) ) {
				returnValue = null;
			}

			return returnValue;
		}

		#region IDisposable Implementation

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_messageReadyEvent" )]
		protected virtual void Dispose( bool disposing ) {
			if ( !_disposed ) {
				if ( disposing ) {
					//=== Free managed objects
					_messageReadyEvent?.Dispose();
				}

				//=== Free unmanaged objects
				_disposed = true;
			}
		}

		~MessageQueue() {
			Dispose( false );
		}

		#endregion
	}
}
