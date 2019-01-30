using anthilla.core;
using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse GetLogin(RequestMetadata md) {
            #region Variables

            string email = "";
            string password = "";
            UserMaster currUser = new UserMaster();

            #endregion

            #region Get-Values-from-Querystring

            email = md.CurrentHttpRequest.RetrieveHeaderValue(CurrentSettings.Server.HeaderEmail);
            password = md.CurrentHttpRequest.RetrieveHeaderValue(CurrentSettings.Server.HeaderPassword);

            if(String.IsNullOrEmpty(email)) {
                ConsoleLogger.Warn("GetLogin email not found in querystring under key " + CurrentSettings.Server.HeaderEmail);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Incomplete authentication material supplied.", null).ToJson(),
                    true);
            }

            if(String.IsNullOrEmpty(password)) {
                ConsoleLogger.Warn("GetLogin password not found in querystring under key " + CurrentSettings.Server.HeaderPassword);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Incomplete authentication material supplied.", null).ToJson(),
                    true);
            }

            #endregion

            #region Retrieve-User-Master-by-Email

            currUser = Users.GetUserByEmail(email);
            if(currUser == null) {
                ConsoleLogger.Warn("GetLogin unable to find user while attempting to authenticate user " + email);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Check-Password

            if(String.Compare(password, currUser.Password) != 0) {
                ConsoleLogger.Warn("GetLogin incorrect password supplied for user " + email);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Check-User-Active

            if(!Common.IsTrue(currUser.Active)) {
                ConsoleLogger.Warn("GetLogin user " + email + " marked inactive");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Check-User-Not-Expired

            if(!Common.IsLaterThanNow(currUser.Expiration)) {
                ConsoleLogger.Warn("GetLogin user " + email + " marked as expired at " + currUser.Expiration);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Respond

            currUser.Password = null;
            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(currUser), true);

            #endregion
        }
    }
}