﻿using System;
using Antd.DBus.CodeGen;

namespace Antd.DBus {
    /// <summary>
    /// Overrides how the property is handled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyAttribute : Attribute {
        /// <summary>
        /// If not null, used to override the autogenerated name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies the mutability of the property.
        /// </summary>
        public PropertyAccess Access { get; set; } = PropertyAccess.ReadWrite;
    }
}
