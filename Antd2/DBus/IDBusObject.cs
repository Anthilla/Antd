// Copyright 2016 Tom Deseyn <tom.deseyn@gmail.com>
// This software is made available under the MIT License
// See COPYING for details

namespace Antd.DBus {
    /// <summary>
    /// Base interface for D-Bus objects.
    /// </summary>
    public interface IDBusObject {
        /// <summary>
        /// Path of the D-Bus object.
        /// </summary>
        ObjectPath ObjectPath { get; }
    }
}
