using System;

namespace Antd.sync {
    public class WrongPathTypeException : Exception {

        public WrongPathTypeException() {

        }

        public WrongPathTypeException(string path) {
            Path = path;
        }

        public string Path { get; set; }
    }
}
