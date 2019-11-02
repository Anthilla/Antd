namespace Antd.ProcFs {
    public enum ProcessState {
        /// <summary>
        /// R: Running
        /// </summary>
        Running,
        /// <summary>
        /// S: Sleeping in an interruptible wait
        /// </summary>
        Sleeping,
        /// <summary>
        /// D: Waiting in uninterruptible disk sleep
        /// </summary>
        Waiting,
        /// <summary>
        /// Z: Zombie
        /// </summary>
        Zombie,
        /// <summary>
        /// T: Stopped on a signal
        /// </summary>
        Stopped,
        /// <summary>
        /// t: Tracing stop
        /// </summary>
        TracingStop,
        /// <summary>
        /// X, x: Dead
        /// </summary>
        Dead,
        /// <summary>
        /// K: Wakekill
        /// </summary>
        WakeKill,
        /// <summary>
        /// W: Waking 
        /// </summary>
        Waking,
        /// <summary>
        /// P: Parked
        /// </summary>
        Parked,
        Unknown
    }
}