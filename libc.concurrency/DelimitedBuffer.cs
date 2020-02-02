using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace libc.concurrency {
    /// <summary>
    ///     AAAA/BBB
    ///     اگر در مثال بالا کاراکتر اسلش، دلیمیتر باشه، این بافر مثال بالا رو به دو پیام پکت
    ///     تقسیم می کنه
    ///     و ما رو مطلع می کنه
    /// </summary>
    public class DelimitedBuffer : IDisposable {
        private readonly List<byte> buffer;
        private readonly List<byte> delimiter;
        private bool disposed;
        public DelimitedBuffer(string delimiter)
            : this(
                Encoding.UTF8.GetBytes(delimiter)) {
        }
        public DelimitedBuffer(IEnumerable<byte> delimiter) {
            this.delimiter = delimiter.ToList();
            buffer = new List<byte>();
        }
        public Action<byte[]> PacketRcv { get; set; }
        public void Dispose() {
            disposed = true;
            buffer.Clear();
            PacketRcv = null;
        }
        public void Add(byte newByte) {
            if (disposed) return;
            lock (buffer) {
                buffer.Add(newByte);
                if (buffer.Count < delimiter.Count) return;
                var index = buffer.Count - delimiter.Count;
                for (var i = index; i < buffer.Count; i++)
                    if (buffer[i] != delimiter[i - index])
                        return;
                var packet = new byte[buffer.Count - delimiter.Count];
                buffer.CopyTo(0, packet, 0, packet.Length);
                buffer.Clear();
                raiseEvent(packet);
            }
        }
        private void raiseEvent(byte[] packet) {
            PacketRcv?.Invoke(packet);
        }
    }
}