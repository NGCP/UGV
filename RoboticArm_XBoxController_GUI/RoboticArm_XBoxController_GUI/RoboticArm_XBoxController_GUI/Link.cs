using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.IO
{
    public abstract class Link
    {
        /// <summary>
        /// Action to do when a new package was received
        /// </summary>
        public Action<byte[]> PackageReceived;
        /// <summary>
        /// If IO is active
        /// </summary>
        public abstract bool IsActive();
        /// <summary>
        /// Start a Link
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// Stop a Link
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Send(byte[] bytes) { }
        /// <summary>
        /// Send data with dynamic target
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Target"></param>
        public virtual void Send(byte[] bytes, object Target) { }
    }
}
