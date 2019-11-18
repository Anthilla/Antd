using System;
using System.Security.Cryptography;
using System.Text;

namespace antd.core {
    public class CommonRandom {

        #region [    Private Methods    ]
        private static int Roll(int length) {
            var randomNumber = new byte[1];
            using(var rngCsp = new RNGCryptoServiceProvider()) {
                rngCsp.GetBytes(randomNumber);
            }
            while(!IsFairValue(randomNumber[0], length))
                ;
            return randomNumber[0] % length;
        }

        private static bool IsFairValue(byte roll, int length) {
            return roll < length * (byte.MaxValue / length);
        }
        #endregion

        private static readonly char[] SrcChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] SrcCharsExtended = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        public static string Alphanumeric(int length, bool controlOutput = false) {
            var selectedChars = new char[length];
            if(controlOutput) {
                for(var i = 0; i < length; i++) {
                    if(i < 2) {
                        selectedChars[i] = SrcChars[Roll(36)];
                    }
                    else {
                        var c = SrcChars[Roll(36)];
                        //const bool isSequential = false;
                        var isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                        var isValid = /*!isSequential && */!isRepeated;
                        while(!isValid) {
                            c = SrcChars[Roll(36)];
                            //isSequential = (c - (selectedChars[i - 1])) == (selectedChars[i - 1] - (selectedChars[i - 2]));
                            isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                            isValid = /*!isSequential &&*/ !isRepeated;
                        }
                        selectedChars[i] = c;
                    }
                }
            }
            else {
                for(var i = 0; i < length; i++) {
                    selectedChars[i] = SrcChars[Roll(36)];
                }
            }
            return new string(selectedChars);
        }

        public static string AlphanumericExtended(int length, bool controlOutput = false) {
            var selectedChars = new char[length];
            if(controlOutput) {
                for(var i = 0; i < length; i++) {
                    if(i < 2) {
                        selectedChars[i] = SrcChars[Roll(62)];
                    }
                    else {
                        var c = SrcChars[Roll(62)];
                        //const bool isSequential = false;
                        var isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                        var isValid = /*!isSequential && */!isRepeated;
                        while(!isValid) {
                            c = SrcChars[Roll(62)];
                            //isSequential = (c - (selectedChars[i - 1])) == (selectedChars[i - 1] - (selectedChars[i - 2]));
                            isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                            isValid = /*!isSequential &&*/ !isRepeated;
                        }
                        selectedChars[i] = c;
                    }
                }
            }
            else {
                for(var i = 0; i < length; i++) {
                    selectedChars[i] = SrcChars[Roll(62)];
                }
            }
            return new string(selectedChars);
        }

        public static string Numbers(int length, bool controlOutput = false) {
            var selectedChars = new char[length];
            if(controlOutput) {
                for(var i = 0; i < length; i++) {
                    if(i < 2) {
                        selectedChars[i] = SrcChars[Roll(10)];
                    }
                    else {
                        var c = SrcChars[Roll(10)];
                        var isSequential = (c - (selectedChars[i - 1])) == (selectedChars[i - 1] - (selectedChars[i - 2]));
                        var isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                        var isValid = !isSequential && !isRepeated;
                        while(!isValid) {
                            c = SrcChars[Roll(10)];
                            isSequential = (c - (selectedChars[i - 1])) == (selectedChars[i - 1] - (selectedChars[i - 2]));
                            isRepeated = c == (selectedChars[i - 1]) && c == (selectedChars[i - 2]);
                            isValid = !isSequential && !isRepeated;
                        }
                        selectedChars[i] = c;
                    }
                }
            }
            else {
                for(var i = 0; i < length; i++) {
                    selectedChars[i] = SrcChars[Roll(10)];
                }
            }
            return new string(selectedChars);
        }

        public static string ShortGuid() {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 22).Replace("/", "_").Replace("+", "-");
        }

        public static int Number(int max) {
            return Roll(max);
        }

    }
}
