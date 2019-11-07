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

namespace Antd.Sdp {
    public class RtpEncoding {
        // video
        public const string Raw = "raw";
        public const string H264 = "H264";
        public const string H264Svc = "H264-SVC";
        public const string Jpeg2000 = "jpeg2000";
        public const string Mpeg4Visual = "MP4V-ES";

        // audio
        public const string L8 = "L8";
        public const string L16 = "L16";
        public const string L24 = "L24";
        public const string Pcmu = "PCMU";
        public const string AC3 = "ac3";
        public const string EnhancedAC3 = "eac3";
        public const string Midi = "rtp-midi";
        public const string Vorbis = "vorbis";
        public const string Speex = "speex";
        public const string Mpeg4Audio = "MP4A-LATM";

        // multi
        public const string Mpeg4ES = "mpeg4-generic";
    }
}
