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
    public class FormatParameter {
        public const string Config = "config";
        public const string Bitrate = "bitrate";
        public const string ProfileLevelId = "profile-level-id";
        public const string PacketizationMode = "packatization-mode";
        public const string Sampling = "sampling";
        public const string Width = "width";
        public const string Height = "height";
        public const string Depth = "depth";
        public const string Interlace = "interlace";
    }
    public class FormatParameterValue {
        public const string SamplingRGB = "RGB";
        public const string SamplingBGR = "BGR";
        public const string SamplingRGBA = "RGBA";
        public const string SamplingBGRA = "BGRA";
        public const string SamplingYUV444 = "YCbCr-4:4:4";
        public const string SamplingYUV422 = "YCbCr-4:2:2";
        public const string SamplingYUV420 = "YCbCr-4:2:0";
        public const string SamplingYUV411 = "YCbCr-4:1:1";
        public const string SamplingGrey = "GRAYSCALE";

        public const string Interlace = "1";
    }
}
