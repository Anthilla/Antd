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

using System;
using System.Collections.Specialized;

namespace Antd.Sdp {
    public class SessionChangedEventArgs : NotifyCollectionChangedEventArgs {
        public SessionAnnouncement OldAnnouncement {
            get {
                if (OldItems != null && OldItems.Count == 1) {
                    return OldItems[0] as SessionAnnouncement;
                }
                else {
                    return null;
                }
            }
        }

        public SessionAnnouncement NewAnnouncement {
            get {
                if (NewItems != null && NewItems.Count == 1) {
                    return NewItems[0] as SessionAnnouncement;
                }
                else {
                    return null;
                }
            }
        }

        public SessionAnnouncement Announcement {
            get {
                if (NewAnnouncement != null) {
                    return NewAnnouncement;
                }
                else {
                    return OldAnnouncement;
                }
            }
        }

        public Session Session {
            get {
                return Announcement.Session;
            }
        }

        public SessionChange Change {
            get {
                return ToSessionChange(Action);
            }
        }

        public SessionChangedEventArgs(SessionChange change, SessionAnnouncement newAnnouncement, SessionAnnouncement oldAnnouncement, int index) :
            base(NotifyCollectionChangedAction.Replace, newAnnouncement, oldAnnouncement) {
            if (change != SessionChange.Update) {
                throw new Exception("Invalid SessionChange for this constructor");
            }
        }

        public SessionChangedEventArgs(SessionChange change, SessionAnnouncement announcement, int index) :
            base(ToCollectionChangedAction(change), announcement, index) {
            if (change == SessionChange.Update) {
                throw new Exception("Invalid SessionChange for this constructor");
            }
        }

        private static NotifyCollectionChangedAction ToCollectionChangedAction(SessionChange change) {
            switch (change) {
                case SessionChange.New:
                    return NotifyCollectionChangedAction.Add;
                case SessionChange.Delete:
                    return NotifyCollectionChangedAction.Remove;
                case SessionChange.Update:
                    return NotifyCollectionChangedAction.Replace;
            }
            throw new Exception("Unknown SessionChange value");
        }
        private static SessionChange ToSessionChange(NotifyCollectionChangedAction action) {
            switch (action) {
                case NotifyCollectionChangedAction.Add:
                    return SessionChange.New;
                case NotifyCollectionChangedAction.Remove:
                    return SessionChange.Delete;
                case NotifyCollectionChangedAction.Replace:
                    return SessionChange.Update;
            }
            throw new Exception("Invalid NotifyCollectionChangedAction value");
        }
    }
}
