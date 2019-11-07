using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Antd.Varlink {
    [Flags]
    public enum CallFlags {
        None = 0,
        ThrowOnError = 1
    }

    public interface IConnection : IDisposable {
        Task<MethodReply<TResult>> CallMethodAsync<TResult>(string method, Func<string, System.Type> errorParametersType, object parameters, CallFlags flags);
    }

    public static class ConnectionExtensions {
        public static async Task<TResult> CallAsync<TResult>(this IConnection connection, string method, Func<string, System.Type> errorParametersType, object parameters) {
            MethodReply<TResult> reply = await connection.CallMethodAsync<TResult>(method, errorParametersType, parameters, CallFlags.ThrowOnError);
            return reply.parameters;
        }

        public static async Task<TResult> CallAsync<TResult>(this IConnection connection, string method, Func<string, System.Type> errorParametersType) {
            MethodReply<TResult> reply = await connection.CallMethodAsync<TResult>(method, errorParametersType, parameters: null, CallFlags.ThrowOnError);
            return reply.parameters;
        }

        public static Task CallAsync(this IConnection connection, string method, Func<string, System.Type> errorParametersType, object parameters)
            => connection.CallMethodAsync<VoidType>(method, errorParametersType, parameters, CallFlags.ThrowOnError);

        public static Task CallAsync(this IConnection connection, string method, Func<string, System.Type> errorParametersType)
            => connection.CallMethodAsync<VoidType>(method, errorParametersType, parameters: null, CallFlags.ThrowOnError);

        private class VoidType { }
    }

    public sealed class Connection : IConnection {
        private const int StateNone = 0;
        private const int StateBusy = 1;
        private const int StateDisposed = 2;

        private readonly MemoryStream _memoryStream;
        private readonly StreamReader _streamReader;
        private readonly byte[] _readBuffer;
        private readonly JsonSerializer _serializer;
        private bool _connected;
        private string _address;
        private int _state;
        private EndPoint _endPoint;
        private Socket _socket;

        public Connection(string address) {
            if (address == null) {
                throw new ArgumentNullException(nameof(address));
            }
            _address = address;
            _memoryStream = new MemoryStream();
            _streamReader = new StreamReader(_memoryStream, Encoding.UTF8);
            _readBuffer = new byte[1024];
            _serializer = _serializer = new JsonSerializer();
        }

        private static void ParseAddress(string address, out Socket socket, out EndPoint endPoint) {
            try {
                int charPos = address.IndexOf(':');
                if (charPos != -1) {
                    string scheme = address.Substring(0, charPos);
                    address = address.Substring(charPos + 1);
                    if (scheme == "tcp") {
                        charPos = address.IndexOf(':');
                        if (charPos != -1) {
                            string host = address.Substring(0, charPos);
                            int port = int.Parse(address.Substring(charPos + 1));
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            endPoint = new DnsEndPoint(host, port);
                            return;
                        }
                    }
                    else if (scheme == "unix") {
                        if (address.StartsWith('@')) {
                            address = '\0' + address.Substring(1);
                        }
                        socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                        endPoint = new UnixDomainSocketEndPoint(address);
                        return;
                    }
                }
            }
            catch { }
            throw new NotSupportedException($"Address '{address}' not supported.");
        }

        public async Task<MethodReply<TResult>> CallMethodAsync<TResult>(string method, Func<string, System.Type> errorParametersType, object parameters, CallFlags flags) {
            try {
                int prevState = Interlocked.CompareExchange(ref _state, StateBusy, StateNone);
                if (prevState == StateBusy) {
                    throw new InvalidOperationException("Concurrent operations are not supported.");
                }
                else if (prevState == StateDisposed) {
                    throw new ObjectDisposedException(typeof(Connection).FullName);
                }
                if (!_connected) {
                    ParseAddress(_address, out _socket, out _endPoint);
                    await _socket.ConnectAsync(_endPoint);
                    _connected = true;
                }
                MethodCall<object> call = new MethodCall<object> {
                    parameters = parameters,
                    method = method,
                    oneway = false,
                    upgrade = false
                };
                string serializedCall = JsonConvert.SerializeObject(call);
                await _socket.SendAsync(Encoding.UTF8.GetBytes(serializedCall), SocketFlags.None); // TODO: why didn't this throw without ConnectAsync
                await _socket.SendAsync(new byte[] { 0 }, SocketFlags.None);
                _memoryStream.SetLength(0);
                bool eom = false;
                do {
                    int length = await _socket.ReceiveAsync(_readBuffer.AsMemory(), SocketFlags.None);
                    if (length == 0) {
                        throw new IOException("Unexpected end of stream.");
                    }
                    _memoryStream.Write(_readBuffer, 0, length);
                    for (int i = 0; i < length; i++) {
                        if (_readBuffer[i] == 0) {
                            eom = true;
                        }
                    }
                } while (!eom);
                _memoryStream.Position = 0;
                _streamReader.DiscardBufferedData();
                MethodReply reply = (MethodReply)_serializer.Deserialize(_streamReader, typeof(MethodReply));

                _memoryStream.Position = 0;
                _streamReader.DiscardBufferedData();
                MethodReply<TResult> result;
                if (reply.error != null) {
                    result = new MethodReply<TResult>(reply);
                    System.Type errorType = errorParametersType?.Invoke(reply.error);
                    if (errorType != null) {
                        var replyWithErrorParameters = (MethodReply)_serializer.Deserialize(_streamReader, typeof(MethodReply<>).MakeGenericType(errorType));
                        result.ErrorParameters = replyWithErrorParameters.GetParameters();
                    }
                    if ((flags & CallFlags.ThrowOnError) != CallFlags.None) {
                        result.ThrowOnError();
                    }
                }
                else {
                    result = (MethodReply<TResult>)_serializer.Deserialize(_streamReader, typeof(MethodReply<TResult>));
                }
                return result;
            }
            catch {
                _socket?.Dispose();
                _connected = false;
                throw;
            }
            finally {
                int prevState = Interlocked.CompareExchange(ref _state, StateNone, StateBusy);
                if (prevState == StateDisposed) {
                    _socket?.Dispose();
                }
            }
        }

        public void Dispose() {
            Volatile.Write(ref _state, StateDisposed);
            _socket?.Dispose();
        }
    }

    class MethodCall<T> : MethodCall {
        public T parameters { get; set; }

        public bool ShouldSerializeparameters() => parameters != null;
    }

    class MethodCall {
        public string method { get; set; }
        public bool oneway { get; set; }
        public bool more { get; set; }
        public bool upgrade { get; set; }

        public bool ShouldSerializeoneway() => oneway != true;
        public bool ShouldSerializemore() => more != true;
        public bool ShouldSerializeupgrade() => upgrade != true;
    }

    public class MethodReply<T> : MethodReply {
        public MethodReply() { }

        internal MethodReply(MethodReply reply) : base(reply) { }

        public T parameters { get; set; }

        internal override object GetParameters() => parameters;
    }

    public class MethodReply {
        public MethodReply() { }

        internal MethodReply(MethodReply reply) {
            continues = reply.continues;
            error = reply.error;
            upgraded = reply.upgraded;
            ErrorParameters = reply.ErrorParameters;
        }
        public bool continues { get; set; }
        public string error { get; set; }
        public bool upgraded { get; set; }
        public object ErrorParameters { get; set; }

        public void ThrowOnError() {
            if (error != null) {
                throw new VarlinkErrorException(error, ErrorParameters);
            }
        }

        internal virtual object GetParameters() => null;
    }

    [System.Serializable]
    public class VarlinkErrorException : System.Exception {
        public VarlinkErrorException() { }
        public VarlinkErrorException(string name) : base(name) { }
        public VarlinkErrorException(string name, object parameters) : base(name) {
            ErrorParameters = parameters;
        }

        protected VarlinkErrorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string ErrorName { get => Message; }
        public object ErrorParameters { get; }
    }
}