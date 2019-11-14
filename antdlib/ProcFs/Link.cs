using System;
using System.Buffers.Text;

namespace antdlib.ProcFs {
    public struct Link {
        public LinkType Type { get; }
        public string Path { get; }
        public int INode { get; }

        private Link(LinkType type, string path, int iNode) {
            Type = type;
            Path = path;
            INode = iNode;
        }

        private static ReadOnlyMemory<byte> socketLinkStart = "socket:[".ToUtf8();
        private static ReadOnlyMemory<byte> pipeLinkStart = "pipe:[".ToUtf8();
        private static ReadOnlyMemory<byte> anonLinkStart = "anon_inode:[".ToUtf8();

        public static Link Read(string linkPath) {
            using (var linkTextBuffer = Native.ReadLink(linkPath)) {
                var linkText = linkTextBuffer.Span;
                if (linkText.StartsWith(socketLinkStart.Span)) {
                    Utf8Parser.TryParse(linkText.Slice(socketLinkStart.Length, linkText.Length - socketLinkStart.Length - 1), out int iNode, out _);
                    return new Link(LinkType.Socket, null, iNode);
                }

                if (linkText.StartsWith(pipeLinkStart.Span)) {
                    Utf8Parser.TryParse(linkText.Slice(pipeLinkStart.Length, linkText.Length - pipeLinkStart.Length - 1), out int iNode, out _);
                    return new Link(LinkType.Pipe, null, iNode);
                }

                if (linkText.StartsWith(anonLinkStart.Span))
                    return new Link(LinkType.Anon, linkText.Slice(anonLinkStart.Length, linkText.Length - anonLinkStart.Length - 1).ToUtf8String(), 0);

                return new Link(LinkType.File, linkText.ToUtf8String(), 0);
            }
        }

        public override string ToString() => $"{Type}:[{Path ?? INode.ToString()}]";
    }

    public enum LinkType {
        File,
        Socket,
        Pipe,
        Anon
    }
}