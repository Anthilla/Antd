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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Antd.Sdp {
    public class AnnouncedSessionCollection : IReadOnlyList<SessionAnnouncement>, INotifyCollectionChanged, INotifyPropertyChanged {
        public SessionAnnouncement this[int index] {
            get { return _sessions.Values[index]; }
        }

        public int Count {
            get { return _sessions.Count; }
        }

        public IEnumerator<SessionAnnouncement> GetEnumerator() {
            return _sessions.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _sessions.Values.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        internal void Add(SessionAnnouncement announcement) {
            Session session = announcement.Session;
            _sessions.Add(session, announcement);
            int index = _sessions.IndexOfKey(announcement.Session);
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null) {
                var eventArgs = new SessionChangedEventArgs(SessionChange.New, announcement, index);
                CollectionChanged(this, eventArgs);
            }
        }

        internal void Remove(SessionAnnouncement announcement) {
            int index = _sessions.IndexOfKey(announcement.Session);
            _sessions.RemoveAt(index);

            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null) {
                var eventArgs = new SessionChangedEventArgs(SessionChange.Delete, announcement, index);
                CollectionChanged(this, eventArgs);
            }
        }

        internal void Replace(SessionAnnouncement oldAnnouncement, SessionAnnouncement newAnnouncement) {
            Session key = oldAnnouncement.Session;
            int index = _sessions.IndexOfKey(key);
            _sessions[key] = newAnnouncement;
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null) {
                var eventArgs = new SessionChangedEventArgs(SessionChange.Update, newAnnouncement, oldAnnouncement, index);
                CollectionChanged(this, eventArgs);
            }
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        private SortedList<Session, SessionAnnouncement> _sessions = new SortedList<Session, SessionAnnouncement>();
    }
}
