////-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace antdlib.common {
    public class MapToModel {

        public IEnumerable<T> FromFile<T>(string filePath, string separator = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) where T : new() {
            if(!File.Exists(filePath)) {
                return new List<T>();
            }
            var objects = new List<T>();
            var lines = File.ReadAllLines(filePath);
            foreach(var line in lines) {
                var arr = line.Split(new[] { separator }, 2, option);
                var obj = new T();
                var objProp = obj.GetType().GetProperties();
                foreach(var op in objProp) {
                    var propertyInfo = obj.GetType().GetProperty(op.Name);
                    var i = Array.IndexOf(objProp, op);
                    try {
                        propertyInfo.SetValue(obj, Convert.ChangeType(arr[i], propertyInfo.PropertyType), null);
                    }
                    catch(Exception) {
                        propertyInfo.SetValue(obj, Convert.ChangeType("", propertyInfo.PropertyType), null);
                    }
                }
                objects.Add(obj);
            }
            return objects;
        }

        public IEnumerable<T> FromCommand<T>(string command, string separator = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) where T : new() {
            var commandResult = Bash.Execute(command);
            if(commandResult.Length < 1) {
                return new List<T>();
            }
            var objects = new List<T>();
            var lines = commandResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines) {
                var arr = line.Split(new[] { separator }, option);
                var obj = new T();
                var objProp = obj.GetType().GetProperties();
                foreach(var op in objProp) {
                    var propertyInfo = obj.GetType().GetProperty(op.Name);
                    var i = Array.IndexOf(objProp, op);
                    try {
                        propertyInfo.SetValue(obj, Convert.ChangeType(arr[i], propertyInfo.PropertyType), null);
                    }
                    catch(Exception) {
                        propertyInfo.SetValue(obj, Convert.ChangeType("", propertyInfo.PropertyType), null);
                    }
                }
                objects.Add(obj);
            }
            return objects;
        }

        public IEnumerable<T> FromCommand<T>(string command, int length, string separator = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) where T : new() {
            var commandResult = Bash.Execute(command);
            if(commandResult.Length < 1) {
                return new List<T>();
            }
            var objects = new List<T>();
            var lines = commandResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines) {
                var arr = line.Split(new[] { separator }, length, option);
                var obj = new T();
                var objProp = obj.GetType().GetProperties();
                foreach(var op in objProp) {
                    var propertyInfo = obj.GetType().GetProperty(op.Name);
                    var i = Array.IndexOf(objProp, op);
                    try {
                        propertyInfo.SetValue(obj, Convert.ChangeType(arr[i], propertyInfo.PropertyType), null);
                    }
                    catch(Exception) {
                        propertyInfo.SetValue(obj, Convert.ChangeType("", propertyInfo.PropertyType), null);
                    }
                }
                objects.Add(obj);
            }
            return objects;
        }

        public IEnumerable<T> FromText<T>(string text, string separator = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) where T : new() {
            if(text.Length < 1) {
                return new List<T>();
            }
            var objects = new List<T>();
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines) {
                var arr = line.Split(new[] { separator }, option);
                var obj = new T();
                var objProp = obj.GetType().GetProperties();
                foreach(var op in objProp) {
                    var propertyInfo = obj.GetType().GetProperty(op.Name);
                    var i = Array.IndexOf(objProp, op);
                    try {
                        propertyInfo.SetValue(obj, Convert.ChangeType(arr[i], propertyInfo.PropertyType), null);
                    }
                    catch(Exception) {
                        propertyInfo.SetValue(obj, Convert.ChangeType("", propertyInfo.PropertyType), null);
                    }
                }
                objects.Add(obj);
            }
            return objects;
        }
    }
}
