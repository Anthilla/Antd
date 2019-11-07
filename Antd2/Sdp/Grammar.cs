//Copyright (C) 2014  Tom Deseyn

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System.Linq;

namespace Antd.Sdp {
    static class Grammar {
        public static void ValidateDigits(string s, bool pos, int minLength = 1) {
            if (s.Length >= minLength) {
                if ((!pos || s[0] != (char)0x30) &&
                    s.All(c => ((c >= (char)0x30) && (c <= (char)0x39)))) {
                    return;
                }
            }
            throw new SdpException("Invalid digits: '" + s + "'");
        }
        public static void ValidateAddress(string s) {
            ValidateNonWsString(s);
        }
        public static void ValidateText(string s) {
            ValidateByteString(s);
        }
        public static void ValidateNonWsString(string s) {
            if (s.Length != 0) {
                if (s.All(c => ((c >= (char)0x21) && (c <= (char)0x7e)) ||
                               ((c >= (char)0x80) && (c <= (char)0xff))
                         )) {
                    return;
                }
            }
            throw new SdpException("Invalid non-ws-string: '" + s + "'");
        }
        public static bool IsValidToken(string s) {
            if (s.Length != 0) {
                if (s.All(c => ((c >= (char)0x21) && (c <= (char)0x21)) ||
                               ((c >= (char)0x23) && (c <= (char)0x27)) ||
                               ((c >= (char)0x2a) && (c <= (char)0x2b)) ||
                               ((c >= (char)0x2d) && (c <= (char)0x2e)) ||
                               ((c >= (char)0x30) && (c <= (char)0x39)) ||
                               ((c >= (char)0x41) && (c <= (char)0x5a)) ||
                               ((c >= (char)0x5e) && (c <= (char)0x7e))
                    )) {
                    return true;
                }
            }
            return false;
        }
        public static void ValidateToken(string s) {
            if (IsValidToken(s)) {
                return;
            }
            throw new SdpException("Invalid token: '" + s + "'");
        }
        public static void ValidateByteString(string s) {
            if (s.Length != 0) {
                if (s.All(c => (c != '\0') && (c != '\r') && (c != '\n'))) {
                    return;
                }
            }
            throw new SdpException("Invalid byte-string: '" + s + "'");
        }
        public static void ValidateEMail(string value) {
            // TODO: more specific validation
            ValidateByteString(value);
        }
        public static void ValidatePhoneNumber(string value) {
            // TODO: more specific validation
            ValidateByteString(value);
        }
        public static void ValidateTime(string value) {
            if (value == "0") {
                return;
            }
            ValidateDigits(value, false, 10);
        }
        public static void ValidateProtocol(string value) {
            string[] parts = value.Split(new[] { '/' });
            foreach (string part in parts) {
                ValidateToken(part);
            }
        }
        public static void ValidateUnicastAddress(string s) {
            ValidateNonWsString(s);
        }
    }
}
