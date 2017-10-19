namespace Antd.sync {
    internal class BlockLookup {
        private const long UNMATCHED_INDEX = -1;

        public long MatchedIndex { get; set; }

        /// <summary>
        /// The index of the next expected block
        /// </summary>
        public long NextMatchKey { get; set; }

        public int BlockLength { get; set; }

        public bool IsMatch {
            get {
                return MatchedIndex >= 0;
            }
        }

        public void ResetMatchIndex() {
            MatchedIndex = UNMATCHED_INDEX;
        }

        public BlockLookup(int initialBlockLength) {
            NextMatchKey = 0;
            MatchedIndex = UNMATCHED_INDEX;
            BlockLength = initialBlockLength;
        }

        public bool DoesSequenceFit {
            get {
                return MatchedIndex == NextMatchKey;
            }
        }
    }
}
