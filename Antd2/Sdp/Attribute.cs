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
    public class Attribute {
        public const string Category = "cat";
        public const string Keywords = "keywds";
        public const string PacketTime = "ptime";
        public const string MaxPacketTime = "maxptime";
        public const string RtpEncoding = "rtpmap";
        public const string ReceiveOnly = "recvonly";
        public const string SendReceive = "sendrecv";
        public const string SendOnly = "sendonly";
        public const string Inactive = "inactive";
        public const string Orientation = "orient";
        public const string ConferenceType = "type";
        public const string CharacterSet = "charset";
        public const string SdpLanguage = "sdplang";
        public const string Language = "lang";
        public const string Framerate = "framerate";
        public const string Quality = "quality";
        public const string FormatParameters = "fmtp";
        public const string Label = "label";
    }
    public class AttributeValue {
        public const string OrientationPortrait = "portrait";
        public const string OrientationLandscape = "landscape";
        public const string OrientationSeascape = "seascape";

        public const string ConferenceTypeBroadcast = "broadcast";
        public const string ConferenceTypeMeeting = "meeting";
        public const string ConferenceTypeModerated = "moderated";
        public const string ConferenceTypeTest = "test";
        public const string ConferenceTypeH332 = "H332";

        public const int QualityBest = 10;
        public const int QualityDefault = 5;
        public const int QualityWorst = 0;
    }
}
