#region Copyright (c) 2005 by Brian Gideon (briangideon@yahoo.com)
/* Shared Source License for NDde
 *
 * This license governs use of the accompanying software ('Software'), and your use of the Software constitutes acceptance of this license.
 *
 * You may use the Software for any commercial or noncommercial purpose, including distributing derivative works.
 * 
 * In return, we simply require that you agree:
 *  1. Not to remove any copyright or other notices from the Software. 
 *  2. That if you distribute the Software in source code form you do so only under this license (i.e. you must include a complete copy of this
 *     license with your distribution), and if you distribute the Software solely in object form you only do so under a license that complies with
 *     this license.
 *  3. That the Software comes "as is", with no warranties.  None whatsoever.  This means no express, implied or statutory warranty, including
 *     without limitation, warranties of merchantability or fitness for a particular purpose or any warranty of title or non-infringement.  Also,
 *     you must pass this disclaimer on whenever you distribute the Software or derivative works.
 *  4. That no contributor to the Software will be liable for any of those types of damages known as indirect, special, consequential, or incidental
 *     related to the Software or this license, to the maximum extent the law permits, no matter what legal theory it’s based on.  Also, you must
 *     pass this limitation of liability on whenever you distribute the Software or derivative works.
 *  5. That if you sue anyone over patents that you think may apply to the Software for a person's use of the Software, your license to the Software
 *     ends automatically.
 *  6. That the patent rights, if any, granted in this license only apply to the Software, not to any derivative works you make.
 *  7. That the Software is subject to U.S. export jurisdiction at the time it is licensed to you, and it may be subject to additional export or
 *     import laws in other places.  You agree to comply with all such laws and regulations that may apply to the Software after delivery of the
 *     software to you.
 *  8. That if you are an agency of the U.S. Government, (i) Software provided pursuant to a solicitation issued on or after December 1, 1995, is
 *     provided with the commercial license rights set forth in this license, and (ii) Software provided pursuant to a solicitation issued prior to
 *     December 1, 1995, is provided with “Restricted Rights” as set forth in FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013 
 *     (Oct 1988), as applicable.
 *  9. That your rights under this License end automatically if you breach it in any way.
 * 10. That all rights not expressly granted to you in this license are reserved.
 */
#endregion
namespace NDde.Server {
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Threading;
	using NDde.Advanced;
	using NDde.Foundation;
	using NDde.Foundation.Server;
	using NDde.Foundation.Advanced;
	using NDde.Properties;

	/// <summary>
	/// This represents the server side of DDE conversations.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	/// <remarks>
	/// <para>
	/// DDE conversations are established by specifying a service name and topic name pair.  The service name is usually the name of the application
	/// acting as a DDE server.  A DDE server can respond to multiple service names, but most servers usually only respond to one.  The topic name
	/// is a logical context for data and is defined by the server application.  A server can and usually does support many topic names.
	/// </para>
	/// <para>
	/// After this object has registered its service name by calling the <c>Register</c> method clients can connect to it by specifying the service
	/// name the server registered and a topic name that it supports.
	/// </para>
	/// <para>
	/// Event methods are invoked on the thread hosting the <c>DdeContext</c>.  All operations must be marshaled onto the thread hosting the 
	/// <c>DdeContext</c> associated with this object.  Method calls will block until that thread becomes available.  An exception will be generated
	/// if the thread does not become available in a timely manner.
	/// </para>
	/// <para>
	/// <note type="inheritinfo">
	/// The event methods must be overridden in a subclass as needed.
	/// </note>
	/// </para>
	/// </remarks>
	/// <include file='Documentation/Examples.xml' path='Comment/Member[@name="DdeServer"]/*'/>
	public abstract class DdeServer : IDisposable {
		private Object _LockObject = new Object();

		private DdemlServer _DdemlObject = null; // This has lazy initialization through a property.
		private DdeContext _Context = null;

		private bool _IsRegistered = false; // This is a cached DdemlServer property.
		private string _Service = "";    // This is a cached DdemlServer property.


		/// <overloads>
		/// <summary>
		/// </summary>
		/// </overloads>
		/// <summary>
		/// This initializes a new instance of the <c>DdeServer</c> class that can register the specified service name.
		/// </summary>
		/// <param name="service">
		/// The service name that this instance can register.
		/// </param>
		/// <exception cref="ArgumentException">
		/// This is thown when service exceeds 255 characters..
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when service is a null reference.
		/// </exception>
		public DdeServer(string service)
			: this(service, DdeContext.GetDefault()) {
		}

		/// <summary>
		/// This initializes a new instance of the <c>DdeServer</c> class that can register the specified service name and using the specified
		/// synchronizing object.
		/// </summary>
		/// <param name="service">
		/// The service name that this instance can register.
		/// </param>
		/// <param name="synchronizingObject">
		/// The synchronizing object to use for this instance.
		/// </param>
		/// <exception cref="ArgumentException">
		/// This is thown when service exceeds 255 characters..
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when service is a null reference.
		/// </exception>
		public DdeServer(string service, ISynchronizeInvoke synchronizingObject)
			: this(service, DdeContext.GetDefault(synchronizingObject)) {
		}

		/// <summary>
		/// This initializes a new instance of the <c>DdeServer</c> class that can register the specified service name and uses the specified
		/// context.
		/// </summary>
		/// <param name="service">
		/// The service name that this instance can register.
		/// </param>
		/// <param name="context">
		/// The context to use for execution.
		/// </param>
		/// <exception cref="ArgumentException">
		/// This is thown when service exceeds 255 characters..
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when service is a null reference.
		/// </exception>
		public DdeServer(string service, DdeContext context) {
			Service = service;
			Context = context;
		}

		/// <summary>
		/// This unregisters service name and releases all resources held by this instance.
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}

		/// <summary>
		/// This contains the implementation to release all resources held by this instance.
		/// </summary>
		/// <param name="disposing">
		/// True if called by Dispose, false otherwise.
		/// </param>
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				ThreadStart method = delegate() {
					DdemlObject.Dispose();
				};

				try {
					Context.Invoke(method);
				} catch {
					// Swallow any exception that occurs.
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal DdemlServer DdemlObject {
			get {
				lock (_LockObject) {
					if (_DdemlObject == null) {
						_DdemlObject = new MyDdemlServer(Service, Context.DdemlObject, this);
						_DdemlObject.StateChange += new EventHandler(this.OnStateChange);
					}
					return _DdemlObject;
				}
			}
		}

		/// <summary>
		/// This gets the context associated with his instance.
		/// </summary>
		public DdeContext Context {
			get {
				lock (_LockObject) {
					return _Context;
				}
			}
			private set {
				lock (_LockObject) {
					_Context = value;
				}
			}
		}

		/// <summary>
		/// This gets the service name associated with this server.
		/// </summary>
		public virtual string Service {
			get {
				lock (_LockObject) {
					return _Service;
				}
			}
			private set {
				lock (_LockObject) {
					_Service = value;
				}
			}
		}

		/// <summary>
		/// This gets a bool indicating whether the service name is registered.
		/// </summary>
		public virtual bool IsRegistered {
			get {
				lock (_LockObject) {
					return _IsRegistered;
				}
			}
		}

		/// <summary>
		/// This registers the service name.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is already registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the service name could not be registered.
		/// </exception>
		public virtual void Register() {
			ThreadStart method = delegate() {
				DdemlObject.Register();
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <summary>
		/// This unregisters the service name.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		public virtual void Unregister() {
			ThreadStart method = delegate() {
				DdemlObject.Unregister();
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <summary>
		/// This notifies all clients that data has changed for the specified topic name and item name pair.
		/// </summary>
		/// <param name="topic">
		/// A topic name supported by this server.
		/// </param>
		/// <param name="item">
		/// An item name supported by this server.
		/// </param>
		/// <exception cref="ArgumentException">
		/// This is thown when topic or item exceeds 255 characters..
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when topic or item is a null reference.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the notification could not be posted.
		/// </exception>
		/// <remarks>
		/// Use an asterix to indicate that the topic name, item name, or both should be wild.
		/// </remarks>
		public virtual void Advise(string topic, string item) {
			ThreadStart method = delegate() {
				DdemlObject.Advise(topic, item);
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ArgumentException e) {
				throw e;
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <overloads>
		/// <summary>
		/// </summary>
		/// <remarks>
		/// Pausing a conversation causes this server to queue events until the conversation resumes.
		/// </remarks>
		/// </overloads>
		/// <summary>
		/// This pauses the specified conversation.
		/// </summary>
		/// <param name="conversation">
		/// The conversation to pause.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when conversation is a null reference.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the conversation is already paused or when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversation could not be paused.
		/// </exception>
		public virtual void Pause(DdeConversation conversation) {
			ThreadStart method = delegate() {
				DdemlObject.Pause(conversation.DdemlObject);
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ArgumentException e) {
				throw e;
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <summary>
		/// This pauses all conversations.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversations could not be paused.
		/// </exception>
		/// <remarks>
		/// Pausing a conversation causes this object to queue events until the conversation resumes.
		/// </remarks>
		public virtual void Pause() {
			ThreadStart method = delegate() {
				DdemlObject.Pause();
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <overloads>
		/// <summary>
		/// </summary>
		/// </overloads>
		/// <summary>
		/// This resumes the specified conversation.
		/// </summary>
		/// <param name="conversation">
		/// The conversation to resume.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when conversation is a null reference.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the conversation is not paused or when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversation could not be resumed.
		/// </exception>
		public virtual void Resume(DdeConversation conversation) {
			ThreadStart method = delegate() {
				DdemlObject.Resume(conversation.DdemlObject);
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ArgumentException e) {
				throw e;
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <summary>
		/// This resumes all conversations.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversations could not be resumed.
		/// </exception>
		public virtual void Resume() {
			ThreadStart method = delegate() {
				DdemlObject.Resume();
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <overloads>
		/// <summary>
		/// </summary>
		/// </overloads>
		/// <summary>
		/// This terminates the specified conversation.
		/// </summary>
		/// <param name="conversation">
		/// The conversation to terminate.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// This is thrown when conversation is a null reference.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversation could not be terminated.
		/// </exception>
		public virtual void Disconnect(DdeConversation conversation) {
			ThreadStart method = delegate() {
				DdemlObject.Disconnect(conversation.DdemlObject);
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ArgumentException e) {
				throw e;
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		/// <summary>
		/// This terminates all conversations.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This is thrown when the server is not registered.
		/// </exception>
		/// <exception cref="DdeException">
		/// This is thrown when the conversations could not be terminated.
		/// </exception>
		public virtual void Disconnect() {
			ThreadStart method = delegate() {
				DdemlObject.Disconnect();
			};

			try {
				Context.Invoke(method);
			} catch (DdemlException e) {
				throw new DdeException(e);
			} catch (ObjectDisposedException e) {
				throw new ObjectDisposedException(this.GetType().ToString(), e);
			}
		}

		private void OnStateChange(object sender, EventArgs args) {
			lock (_LockObject) {
				_IsRegistered = _DdemlObject.IsRegistered;
				_Service = _DdemlObject.Service;
			}
		}

		/// <summary>
		/// This is invoked when a client attempts to initiate an advise loop.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		/// <param name="item">
		/// The item name associated with this event.
		/// </param>
		/// <param name="format">
		/// The format of the data.
		/// </param>
		/// <returns>
		/// True to allow the advise loop, false otherwise.
		/// </returns>
		/// <remarks>
		/// The default implementation accepts all advise loops.
		/// </remarks>
		protected virtual bool OnStartAdvise(DdeConversation conversation, string item, int format) {
			return true;
		}

		/// <summary>
		/// This is invoked when a client terminates an advise loop.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		/// <param name="item">
		/// The item name associated with this event.
		/// </param>
		protected virtual void OnStopAdvise(DdeConversation conversation, string item) {
		}

		/// <summary>
		/// This is invoked when a client attempts to establish a conversation.
		/// </summary>
		/// <param name="topic">
		/// The topic name associated with this event.
		/// </param>
		/// <returns>
		/// True to allow the connection, false otherwise.
		/// </returns>
		/// <remarks>
		/// The default implementation accepts all connections.
		/// </remarks>
		protected virtual bool OnBeforeConnect(string topic) {
			return true;
		}

		/// <summary>
		/// This is invoked when a client has successfully established a conversation.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		protected virtual void OnAfterConnect(DdeConversation conversation) {
		}

		/// <summary>
		/// This is invoked when a client terminates a conversation.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		protected virtual void OnDisconnect(DdeConversation conversation) {
		}

		/// <summary>
		/// This is invoked when a client sends a command.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		/// <param name="command">
		/// The command to be executed.
		/// </param>
		/// <returns>
		/// An <c>ExecuteResult</c> indicating the result.
		/// </returns>
		/// <remarks>
		/// The default implementation returns <c>ExecuteResult.NotProcessed</c> to the client.
		/// </remarks>
		protected virtual ExecuteResult OnExecute(DdeConversation conversation, string command) {
			return ExecuteResult.NotProcessed;
		}

		/// <summary>
		/// This is invoked when a client sends data.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		/// <param name="item">
		/// The item name associated with this event.
		/// </param>
		/// <param name="data">
		/// The data associated with this event.
		/// </param>
		/// <param name="format">
		/// The format of the data.
		/// </param>
		/// <returns>
		/// A <c>PokeResult</c> indicating the result.
		/// </returns>
		/// <remarks>
		/// The default implementation returns <c>PokeResult.NotProcessed</c> to the client.
		/// </remarks>
		protected virtual PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format) {
			return PokeResult.NotProcessed;
		}

		/// <overloads>
		/// <summary>
		/// </summary>
		/// </overloads>
		/// <summary>
		/// This is invoked when a client attempts to request data.
		/// </summary>
		/// <param name="conversation">
		/// The conversation associated with this event.
		/// </param>
		/// <param name="item">
		/// The item name associated with this event.
		/// </param>
		/// <param name="format">
		/// The format of the data.
		/// </param>
		/// <returns>
		/// A <c>RequestResult</c> indicating the result.
		/// </returns>
		/// <remarks>
		/// The default implementation returns <c>RequestResult.NotProcessed</c> to the client.
		/// </remarks>
		protected virtual RequestResult OnRequest(DdeConversation conversation, string item, int format) {
			return RequestResult.NotProcessed;
		}

		/// <summary>
		/// This is invoked when the server is performing a hot advise.
		/// </summary>
		/// <param name="topic">
		/// The topic name associated with this event.
		/// </param>
		/// <param name="item">
		/// The item name associated with this event.
		/// </param>
		/// <param name="format">
		/// The format of the data.
		/// </param>
		/// <returns>
		/// The data that will be sent to the clients.
		/// </returns>
		/// <remarks>
		/// The default implementation sends nothing to the clients.
		/// </remarks>
		protected virtual byte[] OnAdvise(string topic, string item, int format) {
			return null;
		}

		/// <summary>
		/// This is the return value of the <c>OnExecute</c> method.
		/// </summary>
		public struct ExecuteResult {
			/// <summary>
			/// Return this value if the command was executed successfully.
			/// </summary>
			public static readonly ExecuteResult Processed = new ExecuteResult(DdemlServer.ExecuteResult.Processed);

			/// <summary>
			/// Return this value if the command was not executed successfully.
			/// </summary>
			public static readonly ExecuteResult NotProcessed = new ExecuteResult(DdemlServer.ExecuteResult.NotProcessed);

			/// <summary>
			/// Return this value if the server is too busy.
			/// </summary>
			public static readonly ExecuteResult TooBusy = new ExecuteResult(DdemlServer.ExecuteResult.TooBusy);

			/// <summary>
			/// Return this value to pause the conversation and execute the command asynchronously.  After the conversation has been resumed the
			/// <c>OnExecute</c> method will run again.
			/// </summary>
			public static readonly ExecuteResult PauseConversation = new ExecuteResult(DdemlServer.ExecuteResult.PauseConversation);

			private DdemlServer.ExecuteResult _DdemlObject;

			private ExecuteResult(DdemlServer.ExecuteResult result) {
				_DdemlObject = result;
			}

			/// <summary>
			/// This determines whether two object instances are equal.
			/// </summary>
			/// <param name="o">
			/// The object to compare with the current object.
			/// </param>
			/// <returns>
			/// True if the specified object is equal to the current object, false otherwise.
			/// </returns>
			public override bool Equals(object o) {
				if (o is ExecuteResult) {
					ExecuteResult r = (ExecuteResult)o;
					return _DdemlObject == r._DdemlObject;
				}
				return false;
			}

			/// <summary>
			/// This returns a hash code for the object.
			/// </summary>
			/// <returns>
			/// A hash code for the object.
			/// </returns>
			public override int GetHashCode() {
				return _DdemlObject.GetHashCode();
			}

			/// <summary>
			/// This determines whether two <c>ExecuteResult</c> objects are equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are equal, false otherwise.
			/// </returns>
			public static bool operator ==(ExecuteResult lhs, ExecuteResult rhs) {
				return lhs._DdemlObject == rhs._DdemlObject;
			}

			/// <summary>
			/// This determines whether two <c>ExecuteResult</c> objects are not equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are not equal, false otherwise.
			/// </returns>
			public static bool operator !=(ExecuteResult lhs, ExecuteResult rhs) {
				return lhs._DdemlObject != rhs._DdemlObject;
			}

		} // struct

		/// <summary>
		/// This is the return value of the <c>OnPoke</c> method.
		/// </summary>
		public struct PokeResult {
			/// <summary>
			/// Return this value if the poke was successful.
			/// </summary>
			public static readonly PokeResult Processed = new PokeResult(DdemlServer.PokeResult.Processed);

			/// <summary>
			/// Return this value if the poke was not successful.
			/// </summary>
			public static readonly PokeResult NotProcessed = new PokeResult(DdemlServer.PokeResult.NotProcessed);

			/// <summary>
			/// Return this value if the server is too busy.
			/// </summary>
			public static readonly PokeResult TooBusy = new PokeResult(DdemlServer.PokeResult.TooBusy);

			/// <summary>
			/// Return this value to pause the conversation and execute the poke asynchronously.  After the conversation has been resumed the
			/// <c>OnPoke</c> method will run again.
			/// </summary>
			public static readonly PokeResult PauseConversation = new PokeResult(DdemlServer.PokeResult.PauseConversation);

			private DdemlServer.PokeResult _DdemlObject;

			private PokeResult(DdemlServer.PokeResult result) {
				_DdemlObject = result;
			}

			/// <summary>
			/// This determines whether two object instances are equal.
			/// </summary>
			/// <param name="o">
			/// The object to compare with the current object.
			/// </param>
			/// <returns>
			/// True if the specified object is equal to the current object, false otherwise.
			/// </returns>
			public override bool Equals(object o) {
				if (o is PokeResult) {
					PokeResult r = (PokeResult)o;
					return _DdemlObject == r._DdemlObject;
				}
				return false;
			}

			/// <summary>
			/// This returns a hash code for the object.
			/// </summary>
			/// <returns>
			/// A hash code for the object.
			/// </returns>
			public override int GetHashCode() {
				return _DdemlObject.GetHashCode();
			}

			/// <summary>
			/// This determines whether two <c>PokeResult</c> objects are equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are equal, false otherwise.
			/// </returns>
			public static bool operator ==(PokeResult lhs, PokeResult rhs) {
				return lhs._DdemlObject == rhs._DdemlObject;
			}

			/// <summary>
			/// This determines whether two <c>ExecuteResult</c> objects are not equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are not equal, false otherwise.
			/// </returns>
			public static bool operator !=(PokeResult lhs, PokeResult rhs) {
				return lhs._DdemlObject != rhs._DdemlObject;
			}

		} // struct

		/// <summary>
		/// This is the return value of the <c>OnRequest</c> method.
		/// </summary>
		public struct RequestResult {
			internal static readonly RequestResult Processed = new RequestResult(DdemlServer.RequestResult.Processed);

			/// <summary>
			/// Return this value if the request was not successful.
			/// </summary>
			public static readonly RequestResult NotProcessed = new RequestResult(DdemlServer.RequestResult.NotProcessed);

			/// <summary>
			/// Return this value to pause the conversation and execute the request asynchronously.  After the conversation has been resumed the
			/// <c>OnRequest</c> method will run again.
			/// </summary>
			public static readonly RequestResult PauseConversation = new RequestResult(DdemlServer.RequestResult.PauseConversation);

			private DdemlServer.RequestResult _DdemlObject;

			private RequestResult(DdemlServer.RequestResult result) {
				_DdemlObject = result;
			}

			/// <summary>
			/// This initializes the <c>RequestResult</c> struct with the data to return to the client.
			/// </summary>
			/// <param name="data">
			/// The data to return to the client.
			/// </param>
			public RequestResult(byte[] data) {
				_DdemlObject = DdemlServer.RequestResult.Processed;
				_DdemlObject.Data = data;
			}

			/// <summary>
			/// The data to send to the client application.
			/// </summary>
			public byte[] Data {
				get { return _DdemlObject.Data; }
				set { _DdemlObject.Data = value; }
			}

			/// <summary>
			/// This determines whether two object instances are equal.
			/// </summary>
			/// <param name="o">
			/// The object to compare with the current object.
			/// </param>
			/// <returns>
			/// True if the specified object is equal to the current object, false otherwise.
			/// </returns>
			public override bool Equals(object o) {
				if (o is RequestResult) {
					RequestResult r = (RequestResult)o;
					return _DdemlObject == r._DdemlObject;
				}
				return false;
			}

			/// <summary>
			/// This returns a hash code for the object.
			/// </summary>
			/// <returns>
			/// A hash code for the object.
			/// </returns>
			public override int GetHashCode() {
				return _DdemlObject.GetHashCode();
			}

			/// <summary>
			/// This determines whether two <c>RequestResult</c> objects are equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are equal, false otherwise.
			/// </returns>
			public static bool operator ==(RequestResult lhs, RequestResult rhs) {
				return lhs._DdemlObject == rhs._DdemlObject;
			}

			/// <summary>
			/// This determines whether two <c>ExecuteResult</c> objects are not equal.
			/// </summary>
			/// <param name="lhs">
			/// The left hand side object.
			/// </param>
			/// <param name="rhs"></param>
			/// The right hand side object.
			/// <returns>
			/// True if the two objects are not equal, false otherwise.
			/// </returns>
			public static bool operator !=(RequestResult lhs, RequestResult rhs) {
				return lhs._DdemlObject != rhs._DdemlObject;
			}

		} // struct

		private sealed class MyDdemlServer : DdemlServer {
			private DdeServer _Parent = null;

			public MyDdemlServer(string service, DdemlContext context, DdeServer parent)
				: base(service, context) {
				_Parent = parent;
			}

			protected override bool OnStartAdvise(DdemlConversation conversation, string item, int format) {
				return _Parent.OnStartAdvise((DdeConversation)conversation.Tag, item, format);
			}

			protected override void OnStopAdvise(DdemlConversation conversation, string item) {
				_Parent.OnStopAdvise((DdeConversation)conversation.Tag, item);
			}

			protected override bool OnBeforeConnect(string topic) {
				return _Parent.OnBeforeConnect(topic);
			}

			protected override void OnAfterConnect(DdemlConversation conversation) {
				DdeConversation c = new DdeConversation(conversation);
				conversation.Tag = c;
				_Parent.OnAfterConnect(c);
			}

			protected override void OnDisconnect(DdemlConversation conversation) {
				_Parent.OnDisconnect((DdeConversation)conversation.Tag);
			}

			protected override ExecuteResult OnExecute(DdemlConversation conversation, string command) {
				DdeServer.ExecuteResult result = _Parent.OnExecute((DdeConversation)conversation.Tag, command);
				if (result == DdeServer.ExecuteResult.NotProcessed) {
					return ExecuteResult.NotProcessed;
				}
				if (result == DdeServer.ExecuteResult.PauseConversation) {
					return ExecuteResult.PauseConversation;
				}
				if (result == DdeServer.ExecuteResult.Processed) {
					return ExecuteResult.Processed;
				}
				if (result == DdeServer.ExecuteResult.TooBusy) {
					return ExecuteResult.TooBusy;
				}
				return ExecuteResult.NotProcessed;
			}

			protected override PokeResult OnPoke(DdemlConversation conversation, string item, byte[] data, int format) {
				DdeServer.PokeResult result = _Parent.OnPoke((DdeConversation)conversation.Tag, item, data, format);
				if (result == DdeServer.PokeResult.NotProcessed) {
					return PokeResult.NotProcessed;
				}
				if (result == DdeServer.PokeResult.PauseConversation) {
					return PokeResult.PauseConversation;
				}
				if (result == DdeServer.PokeResult.Processed) {
					return PokeResult.Processed;
				}
				if (result == DdeServer.PokeResult.TooBusy) {
					return PokeResult.TooBusy;
				}
				return PokeResult.NotProcessed;
			}

			protected override RequestResult OnRequest(DdemlConversation conversation, string item, int format) {
				DdeServer.RequestResult result = _Parent.OnRequest((DdeConversation)conversation.Tag, item, format);
				if (result == DdeServer.RequestResult.NotProcessed) {
					return RequestResult.NotProcessed;
				}
				if (result == DdeServer.RequestResult.PauseConversation) {
					return RequestResult.PauseConversation;
				}
				if (result == DdeServer.RequestResult.Processed) {
					return new RequestResult(result.Data);
				}
				return RequestResult.NotProcessed;
			}

			protected override byte[] OnAdvise(string topic, string item, int format) {
				return _Parent.OnAdvise(topic, item, format);
			}

		} // class

	} // class

} // namespace