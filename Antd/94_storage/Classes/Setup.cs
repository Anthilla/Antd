﻿using anthilla.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kvpbase {
    public class Setup {
        public Setup() {
            RunSetup();
        }

        private void RunSetup() {
            #region Variables

            DateTime timestamp = DateTime.Now;
            Settings currSettings = new Settings();
            string separator = "";

            ApiKey currApiKey = new ApiKey();
            List<ApiKey> apiKeys = new List<ApiKey>();

            ApiKeyPermission currPerm = new ApiKeyPermission();
            List<ApiKeyPermission> permissions = new List<ApiKeyPermission>();

            UserMaster currUser = new UserMaster();
            List<UserMaster> users = new List<UserMaster>();

            Topology currTopology = new Topology();
            Node currNode = new Node();

            #endregion

            #region Welcome

            ConsoleLogger.Log("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            ConsoleLogger.Log(@"   _             _                    ");
            ConsoleLogger.Log(@"  | |____ ___ __| |__  __ _ ___ ___   ");
            ConsoleLogger.Log(@"  | / /\ V / '_ \ '_ \/ _` (_-</ -_)  ");
            ConsoleLogger.Log(@"  |_\_\ \_/| .__/_.__/\__,_/__/\___|  ");
            ConsoleLogger.Log(@"           |_|                        ");
            ConsoleLogger.Log(@"                                      ");
            Console.ResetColor();

            ConsoleLogger.Log("");
            ConsoleLogger.Log("kvpbase storage server");
            ConsoleLogger.Log("");
            //          1         2         3         4         5         6         7
            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            ConsoleLogger.Log("Thank you for using kvpbase!  We'll put together a basic system configuration");
            ConsoleLogger.Log("so you can be up and running quickly.  You'll want to modify the System.json");
            ConsoleLogger.Log("file after to ensure a more secure operating environment.");
            ConsoleLogger.Log("");
            ConsoleLogger.Log("Press ENTER to get started.");
            ConsoleLogger.Log("");
            Console.ReadLine();

            #endregion

            #region Initial-Settings

            currSettings.ProductName = "kvpbase";
            currSettings.ProductVersion = "2.0.1";
            currSettings.DocumentationUrl = "http://www.kvpbase.com/docs/";
            currSettings.LogoUrl = "http://kvpbase.com/Content/Images/cloud-only_25px.png";
            currSettings.HomepageUrl = "http://www.kvpbase.com";
            currSettings.SupportEmail = "support@maraudersoftware.com";

            int platform = (int)Environment.OSVersion.Platform;
            if((platform == 4) || (platform == 6) || (platform == 128)) {
                currSettings.Environment = "linux";
            }
            else {
                currSettings.Environment = "windows";
            }

            currSettings.EnableConsole = 1;

            #endregion

            #region Set-Defaults-for-Config-Sections

            separator = Common.GetPathSeparator(currSettings.Environment);

            #region Files

            currSettings.Files = new Settings.SettingsFiles();
            currSettings.Files.ApiKey = "." + separator + "ApiKey.json";
            currSettings.Files.Permission = "." + separator + "ApiKeyPermission.json";
            currSettings.Files.Topology = "." + separator + "Topology.json";
            currSettings.Files.UserMaster = "." + separator + "UserMaster.json";

            #endregion

            #region Server

            currSettings.Server = new Settings.SettingsServer();
            currSettings.Server.HeaderApiKey = "x-api-key";
            currSettings.Server.HeaderEmail = "x-email";
            currSettings.Server.HeaderPassword = "x-password";
            currSettings.Server.HeaderToken = "x-token";
            currSettings.Server.HeaderVersion = "x-version";
            currSettings.Server.AdminApiKey = "kvpbaseadmin";
            currSettings.Server.TokenExpirationSec = 86400;
            currSettings.Server.FailedRequestsIntervalSec = 60;

            #endregion

            #region Redirection

            currSettings.Redirection = new Settings.SettingsRedirection();
            currSettings.Redirection.DeleteRedirectHttpStatus = 301;
            currSettings.Redirection.DeleteRedirectString = "Moved Permanently";
            currSettings.Redirection.DeleteRedirectionMode = "proxy";
            currSettings.Redirection.ReadRedirectHttpStatus = 301;
            currSettings.Redirection.ReadRedirectString = "Moved Permanently";
            currSettings.Redirection.ReadRedirectionMode = "proxy";
            currSettings.Redirection.WriteRedirectHttpStatus = 301;
            currSettings.Redirection.WriteRedirectString = "Moved Permanently";
            currSettings.Redirection.WriteRedirectionMode = "proxy";
            currSettings.Redirection.SearchRedirectHttpStatus = 301;
            currSettings.Redirection.SearchRedirectString = "Moved Permanently";
            currSettings.Redirection.SearchRedirectionMode = "proxy";

            #endregion

            #region Topology

            currSettings.Topology = new Settings.SettingsTopology();
            currSettings.Topology.RefreshSec = 10;

            #endregion

            #region Perfmon

            currSettings.Perfmon = new Settings.SettingsPerfmon();
            currSettings.Perfmon.Enable = 1;
            currSettings.Perfmon.RefreshSec = 10;
            currSettings.Perfmon.Syslog = 0;

            #endregion

            #region Storage

            currSettings.Storage = new Settings.SettingsStorage();
            currSettings.Storage.Directory = "." + separator + "storage" + separator;
            currSettings.Storage.MaxObjectSize = 512000000;
            ConsoleLogger.Log("");
            //          1         2         3         4         5         6         7
            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            ConsoleLogger.Log("Is this a gateway-mode kvpbase node (true)?  Gateway-mode is used when the node");
            ConsoleLogger.Log("uses storage directories that live on other systems or contain data that can");
            ConsoleLogger.Log("be accessed through means other than kvpbase, such as a file share.  Enabling");
            ConsoleLogger.Log("gateway mode allows accessing the data through means outside of kvpbase by");
            ConsoleLogger.Log("disabling encryption and compression.");
            ConsoleLogger.Log("");
            bool isGatewayMode = Common.InputBoolean("Configure this node for gateway-mode", true);
            ConsoleLogger.Log("");

            if(isGatewayMode) {
                currSettings.Storage.GatewayMode = 1;
                currSettings.Storage.DefaultCompress = 0;
                currSettings.Storage.DefaultEncrypt = 0;
            }
            else {
                currSettings.Storage.GatewayMode = 0;
                currSettings.Storage.DefaultCompress = 1;
                currSettings.Storage.DefaultEncrypt = 1;
            }

            #endregion

            #region Messages

            currSettings.Messages = new Settings.SettingsMessages();
            currSettings.Messages.Directory = "." + separator + "messages" + separator;
            currSettings.Messages.RefreshSec = 10;

            #endregion

            #region Expiration

            currSettings.Expiration = new Settings.SettingsExpiration();
            currSettings.Expiration.Directory = "." + separator + "expiration" + separator;
            currSettings.Expiration.RefreshSec = 10;
            currSettings.Expiration.DefaultExpirationSec = 0;

            #endregion

            #region Replication

            currSettings.Replication = new Settings.SettingsReplication();
            currSettings.Replication.Directory = "." + separator + "replication" + separator;
            currSettings.Replication.RefreshSec = 10;
            currSettings.Replication.ReplicationMode = "sync";

            #endregion

            #region Bunker

            currSettings.Bunker = new Settings.SettingsBunker();
            currSettings.Bunker.Directory = "." + separator + "bunker" + separator;
            currSettings.Bunker.Enable = 0;
            currSettings.Bunker.RefreshSec = 30;
            currSettings.Bunker.Nodes = null;

            #endregion

            #region Pubfiles

            currSettings.PublicObj = new Settings.SettingsPublicObj();
            currSettings.PublicObj.Directory = "." + separator + "pubfiles" + separator;
            currSettings.PublicObj.RefreshSec = 600;
            currSettings.PublicObj.DefaultExpirationSec = 7776000;

            #endregion

            #region Tasks

            currSettings.Tasks = new Settings.SettingsTasks();
            currSettings.Tasks.Directory = "." + separator + "tasks" + separator;
            currSettings.Tasks.RefreshSec = 10;

            #endregion

            #region Logger

            currSettings.Logger = new Settings.SettingsLogger();
            currSettings.Logger.RefreshSec = 10;

            #endregion

            #region Syslog

            currSettings.Syslog = new Settings.SettingsSyslog();
            currSettings.Syslog.ConsoleLogging = 1;
            currSettings.Syslog.Header = "kvpbase";
            currSettings.Syslog.ServerIp = "127.0.0.1";
            currSettings.Syslog.ServerPort = 514;
            currSettings.Syslog.LogHttpRequests = 0;
            currSettings.Syslog.LogHttpResponses = 0;
            currSettings.Syslog.MinimumLevel = 1;

            #endregion

            #region Email

            currSettings.Email = new Settings.SettingsEmail();

            #endregion

            #region Encryption

            currSettings.Encryption = new Settings.SettingsEncryption();
            currSettings.Encryption.Mode = "local";
            currSettings.Encryption.Iv = "0000000000000000";
            currSettings.Encryption.Passphrase = "0000000000000000";
            currSettings.Encryption.Salt = "0000000000000000";

            #endregion

            #region REST

            currSettings.Rest = new Settings.SettingsRest();
            currSettings.Rest.AcceptInvalidCerts = 0;
            currSettings.Rest.UseWebProxy = 0;

            #endregion

            #region Debug

            currSettings.Debug = new Settings.SettingsDebug();
            currSettings.Debug.DebugCompression = 0;
            currSettings.Debug.DebugEncryption = 0;

            #endregion

            #region Mailgun

            currSettings.Mailgun = new Settings.SettingsMailgun();

            #endregion

            #endregion

            #region System-Config

            if(
                Common.FileExists("System.json")
                ) {
                ConsoleLogger.Log("System configuration file already exists.");
                if(Common.InputBoolean("Do you wish to overwrite this file", true)) {
                    Common.DeleteFile("System.json");
                    if(!Common.WriteFile("System.json", Common.SerializeJson(currSettings), false)) {
                        Common.ExitApplication("setup", "Unable to write System.json", -1);
                        return;
                    }
                }
            }
            else {
                if(!Common.WriteFile("System.json", Common.SerializeJson(currSettings), false)) {
                    Common.ExitApplication("setup", "Unable to write System.json", -1);
                    return;
                }
            }

            #endregion

            #region Users-API-Keys-and-Permissions

            if(
                Common.FileExists(currSettings.Files.ApiKey)
                || Common.FileExists(currSettings.Files.Permission)
                || Common.FileExists(currSettings.Files.UserMaster)
                ) {
                ConsoleLogger.Log("Configuration files already exist for API keys, users, and/or permissions.");
                if(Common.InputBoolean("Do you wish to overwrite these files", true)) {
                    Common.DeleteFile(currSettings.Files.ApiKey);
                    Common.DeleteFile(currSettings.Files.Permission);
                    Common.DeleteFile(currSettings.Files.UserMaster);

                    ConsoleLogger.Log("Creating new configuration files for API keys, users, and permissions.");

                    currApiKey = new ApiKey();
                    currApiKey.Active = 1;
                    currApiKey.ApiKeyId = 1;
                    currApiKey.Created = timestamp;
                    currApiKey.LastUpdate = timestamp;
                    currApiKey.Expiration = timestamp.AddYears(100);
                    currApiKey.Guid = "default";
                    currApiKey.Notes = "Created by setup script";
                    currApiKey.UserMasterId = 1;
                    apiKeys.Add(currApiKey);

                    currPerm = new ApiKeyPermission();
                    currPerm.Active = 1;
                    currPerm.AllowDeleteContainer = 1;
                    currPerm.AllowDeleteObject = 1;
                    currPerm.AllowReadContainer = 1;
                    currPerm.AllowReadObject = 1;
                    currPerm.AllowSearch = 1;
                    currPerm.AllowWriteContainer = 1;
                    currPerm.AllowWriteObject = 1;
                    currPerm.ApiKeyId = 1;
                    currPerm.ApiKeyPermissionId = 1;
                    currPerm.Created = timestamp;
                    currPerm.LastUpdate = timestamp;
                    currPerm.Expiration = timestamp.AddYears(100);
                    currPerm.Guid = "default";
                    currPerm.Notes = "Created by setup script";
                    currPerm.UserMasterId = 1;
                    permissions.Add(currPerm);

                    currUser = new UserMaster();
                    currUser.Active = 1;
                    currUser.Address1 = "123 Some Street";
                    currUser.Cellphone = "408-555-1212";
                    currUser.City = "San Jose";
                    currUser.CompanyName = "Default Company";
                    currUser.Country = "USA";
                    currUser.FirstName = "First";
                    currUser.LastName = "Last";
                    currUser.Email = "default@default.com";
                    currUser.IsAdmin = 1;
                    currUser.NodeId = 0;
                    currUser.Password = "default";
                    currUser.PostalCode = "95128";
                    currUser.State = "CA";
                    currUser.UserMasterId = 1;
                    currUser.Guid = "default";
                    currUser.Created = timestamp;
                    currUser.LastUpdate = timestamp;
                    currUser.Expiration = timestamp.AddYears(100);
                    users.Add(currUser);

                    if(!Common.WriteFile(currSettings.Files.ApiKey, Common.SerializeJson(apiKeys), false)) {
                        Common.ExitApplication("setup", "Unable to write " + currSettings.Files.ApiKey, -1);
                        return;
                    }

                    if(!Common.WriteFile(currSettings.Files.Permission, Common.SerializeJson(permissions), false)) {
                        Common.ExitApplication("setup", "Unable to write " + currSettings.Files.Permission, -1);
                        return;
                    }

                    if(!Common.WriteFile(currSettings.Files.UserMaster, Common.SerializeJson(users), false)) {
                        Common.ExitApplication("setup", "Unable to write " + currSettings.Files.UserMaster, -1);
                        return;
                    }

                    ConsoleLogger.Log("We have created your first user account and permissions.");
                    ConsoleLogger.Log("  Email    : " + currUser.Email);
                    ConsoleLogger.Log("  Password : " + currUser.Password);
                    ConsoleLogger.Log("  GUID     : " + currUser.Guid);
                    ConsoleLogger.Log("  API Key  : " + currApiKey.Guid);
                    ConsoleLogger.Log("");
                    ConsoleLogger.Log("This was done by creating the following files:");
                    ConsoleLogger.Log("  " + currSettings.Files.UserMaster);
                    ConsoleLogger.Log("  " + currSettings.Files.ApiKey);
                    ConsoleLogger.Log("  " + currSettings.Files.Permission);
                    ConsoleLogger.Log("");
                }
                else {
                    ConsoleLogger.Log("Existing files were left in tact.");
                }
            }
            else {
                currApiKey = new ApiKey();
                currApiKey.Active = 1;
                currApiKey.ApiKeyId = 1;
                currApiKey.Created = timestamp;
                currApiKey.LastUpdate = timestamp;
                currApiKey.Expiration = timestamp.AddYears(100);
                currApiKey.Guid = "default";
                currApiKey.Notes = "Created by setup script";
                currApiKey.UserMasterId = 1;
                apiKeys.Add(currApiKey);

                currPerm = new ApiKeyPermission();
                currPerm.Active = 1;
                currPerm.AllowDeleteContainer = 1;
                currPerm.AllowDeleteObject = 1;
                currPerm.AllowReadContainer = 1;
                currPerm.AllowReadObject = 1;
                currPerm.AllowSearch = 1;
                currPerm.AllowWriteContainer = 1;
                currPerm.AllowWriteObject = 1;
                currPerm.ApiKeyId = 1;
                currPerm.ApiKeyPermissionId = 1;
                currPerm.Created = timestamp;
                currPerm.LastUpdate = timestamp;
                currPerm.Expiration = timestamp.AddYears(100);
                currPerm.Guid = "default";
                currPerm.Notes = "Created by setup script";
                currPerm.UserMasterId = 1;
                permissions.Add(currPerm);

                currUser = new UserMaster();
                currUser.Active = 1;
                currUser.Address1 = "123 Some Street";
                currUser.Cellphone = "408-555-1212";
                currUser.City = "San Jose";
                currUser.CompanyName = "Default Company";
                currUser.Country = "USA";
                currUser.FirstName = "First";
                currUser.LastName = "Last";
                currUser.Email = "default@default.com";
                currUser.IsAdmin = 1;
                currUser.NodeId = 0;
                currUser.Password = "default";
                currUser.PostalCode = "95128";
                currUser.State = "CA";
                currUser.UserMasterId = 1;
                currUser.Guid = "default";
                currUser.Created = timestamp;
                currUser.LastUpdate = timestamp;
                currUser.Expiration = timestamp.AddYears(100);
                users.Add(currUser);

                if(!Common.WriteFile(currSettings.Files.ApiKey, Common.SerializeJson(apiKeys), false)) {
                    Common.ExitApplication("setup", "Unable to write " + currSettings.Files.ApiKey, -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Files.Permission, Common.SerializeJson(permissions), false)) {
                    Common.ExitApplication("setup", "Unable to write " + currSettings.Files.Permission, -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Files.UserMaster, Common.SerializeJson(users), false)) {
                    Common.ExitApplication("setup", "Unable to write " + currSettings.Files.UserMaster, -1);
                    return;
                }

                ConsoleLogger.Log("We have created your first user account and permissions.");
                ConsoleLogger.Log("  Email    : " + currUser.Email);
                ConsoleLogger.Log("  Password : " + currUser.Password);
                ConsoleLogger.Log("  GUID     : " + currUser.Guid);
                ConsoleLogger.Log("  API Key  : " + currApiKey.Guid);
                ConsoleLogger.Log("");
                ConsoleLogger.Log("This was done by creating the following files:");
                ConsoleLogger.Log("  " + currSettings.Files.UserMaster);
                ConsoleLogger.Log("  " + currSettings.Files.ApiKey);
                ConsoleLogger.Log("  " + currSettings.Files.Permission);
                ConsoleLogger.Log("");
            }

            #endregion

            #region Topology

            if(Common.FileExists(currSettings.Files.Topology)) {
                #region File-Exists

                ConsoleLogger.Log("Configuration file already exists for topology.");
                if(Common.InputBoolean("Do you wish to overwrite this file", true)) {
                    #region Overwrite

                    Common.DeleteFile(currSettings.Files.Topology);

                    currTopology = new Topology();
                    currTopology.CurrNodeId = 1;

                    currTopology.Nodes = new List<Node>();
                    currNode = new Node();
                    currNode.DnsHostname = "localhost";
                    currNode.Name = "localhost";
                    currNode.Neighbors = null;
                    currNode.NodeId = 1;
                    currNode.Port = 8080;
                    currNode.Ssl = 0;

                    switch(currSettings.Environment) {
                        case "linux":
                            ConsoleLogger.Log("");
                            //          1         2         3         4         5         6         7
                            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                            ConsoleLogger.Log("IMPORTANT: for Linux and Mac environments, kvpbase can only receive requests on");
                            ConsoleLogger.Log("one hostname.  The hostname you set here must either be the hostname in the URL");
                            ConsoleLogger.Log("used by the requestor, or, set in the HOST header of each request.");
                            ConsoleLogger.Log("");
                            ConsoleLogger.Log("If you set the hostname to 'localhost', this node will ONLY receive and handle");
                            ConsoleLogger.Log("requests destined to 'localhost', i.e. it will only handle local requests.");
                            ConsoleLogger.Log("");

                            currNode.DnsHostname = Common.InputString("On which hostname shall this node listen?", "localhost", false);
                            break;

                        case "windows":
                            currNode.DnsHostname = "+";
                            break;
                    }

                    ConsoleLogger.Log("");
                    currNode.Port = Common.InputInteger("On which port should this node listen?", 8080, true, false);
                    ConsoleLogger.Log("");

                    currTopology.Nodes = new List<Node>();
                    currTopology.Nodes.Add(currNode);
                    currTopology.Replicas = null;

                    if(!Common.WriteFile(currSettings.Files.Topology, Common.SerializeJson(currTopology), false)) {
                        Common.ExitApplication("setup", "Unable to write " + currSettings.Files.Topology, -1);
                        return;
                    }
                    //          1         2         3         4         5         6         7
                    // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    ConsoleLogger.Log("We've created your topology file.  This node is configured to");
                    ConsoleLogger.Log("use HTTP (not HTTPS) and is accessible at the following URL:");
                    ConsoleLogger.Log("");
                    ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port);

                    if(String.Compare(currSettings.Environment, "windows") == 0)
                        ConsoleLogger.Log("  Windows: '+' indicates accessibility on any IP or hostname");

                    ConsoleLogger.Log("");
                    ConsoleLogger.Log("Be sure to install an SSL certificate and modify your Topology.json file to");
                    ConsoleLogger.Log("use SSL to maximize security and set the correct hostname.");
                    ConsoleLogger.Log("");

                    #endregion
                }

                #endregion
            }
            else {
                #region New-File

                currTopology = new Topology();
                currTopology.CurrNodeId = 1;

                currTopology.Nodes = new List<Node>();
                currNode = new Node();
                currNode.DnsHostname = "localhost";
                currNode.Name = "localhost";
                currNode.Neighbors = null;
                currNode.NodeId = 1;
                currNode.Port = 8080;
                currNode.Ssl = 0;

                switch(currSettings.Environment) {
                    case "linux":
                        ConsoleLogger.Log("");
                        //          1         2         3         4         5         6         7
                        // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ConsoleLogger.Log("IMPORTANT: for Linux and Mac environments, kvpbase can only receive requests on");
                        ConsoleLogger.Log("one hostname.  The hostname you set here must either be the hostname in the URL");
                        ConsoleLogger.Log("used by the requestor, or, set in the HOST header of each request.");
                        ConsoleLogger.Log("");
                        ConsoleLogger.Log("If you set the hostname to 'localhost', this node will ONLY receive and handle");
                        ConsoleLogger.Log("requests destined to 'localhost', i.e. it will only handle local requests.");
                        ConsoleLogger.Log("");

                        currNode.DnsHostname = Common.InputString("On which hostname shall this node listen?", "localhost", false);
                        break;

                    case "windows":
                        currNode.DnsHostname = "+";
                        break;
                }

                ConsoleLogger.Log("");
                currNode.Port = Common.InputInteger("On which port should this node listen?", 8080, true, false);
                ConsoleLogger.Log("");

                currTopology.Nodes = new List<Node>();
                currTopology.Nodes.Add(currNode);
                currTopology.Replicas = null;

                if(!Common.WriteFile(currSettings.Files.Topology, Common.SerializeJson(currTopology), false)) {
                    Common.ExitApplication("setup", "Unable to write " + currSettings.Files.Topology, -1);
                    return;
                }

                //          1         2         3         4         5         6         7
                // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                ConsoleLogger.Log("We've created your topology file.  This node is configured to");
                ConsoleLogger.Log("use HTTP (not HTTPS) and is accessible at the following URL:");
                ConsoleLogger.Log("");
                ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port);

                if(String.Compare(currSettings.Environment, "windows") == 0)
                    ConsoleLogger.Log("  Windows: '+' indicates accessibility on any IP or hostname");

                ConsoleLogger.Log("");
                ConsoleLogger.Log("Be sure to install an SSL certificate and modify your Topology.json file to");
                ConsoleLogger.Log("use SSL to maximize security and set the correct hostname.");
                ConsoleLogger.Log("");

                #endregion
            }

            #endregion

            #region Create-Directories

            currSettings.Storage.Directory = "." + separator + "storage" + separator;
            currSettings.Messages.Directory = "." + separator + "messages" + separator;
            currSettings.Expiration.Directory = "." + separator + "expiration" + separator;
            currSettings.Replication.Directory = "." + separator + "replication" + separator;
            currSettings.Bunker.Directory = "." + separator + "bunker" + separator;
            currSettings.PublicObj.Directory = "." + separator + "pubfiles" + separator;
            currSettings.Tasks.Directory = "." + separator + "tasks" + separator;

            if(!Common.CreateDirectory(currSettings.Storage.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Storage.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.Messages.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Messages.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.Expiration.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Expiration.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.Replication.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Replication.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.Bunker.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Bunker.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.PublicObj.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.PublicObj.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory(currSettings.Tasks.Directory)) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Tasks.Directory, -1);
                return;
            }

            if(!Common.CreateDirectory("." + separator + "actions" + separator)) {
                Common.ExitApplication("setup", "Unable to create directory actions subdirectory", -1);
                return;
            }

            #endregion

            #region Create-Sample-Objects

            string htmlFile = SampleHtmlFile(currSettings.DocumentationUrl, "http://www.kvpbase.com/support", "http://github.com/kvpbase");
            string textFile = SampleTextFile(currSettings.DocumentationUrl, "http://www.kvpbase.com/support", "http://github.com/kvpbase");
            string jsonFile = SampleJsonFile(currSettings.DocumentationUrl, "http://www.kvpbase.com/support", "http://github.com/kvpbase");

            if(!Common.CreateDirectory(currSettings.Storage.Directory + "default")) {
                Common.ExitApplication("setup", "Unable to create directory " + currSettings.Storage.Directory + "default", -1);
                return;
            }

            Obj htmlObj = new Obj();
            htmlObj.IsCompressed = 0;
            htmlObj.IsContainer = 0;
            htmlObj.ContainerPath = null;
            htmlObj.ContentType = "text/html";
            htmlObj.Created = DateTime.Now;
            htmlObj.IsEncrypted = 0;
            htmlObj.Key = "hello.html";
            htmlObj.LastAccess = DateTime.Now;
            htmlObj.LastUpdate = DateTime.Now;
            htmlObj.PrimaryNode = currNode;
            htmlObj.PrimaryUrlWithQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.html?x-api-key=default";
            htmlObj.PrimaryUrlWithoutQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.html";
            htmlObj.Replicas = null;
            htmlObj.ReplicationMode = "none";
            htmlObj.Tags = null;
            htmlObj.UserGuid = "default";
            htmlObj.Value = Encoding.UTF8.GetBytes(htmlFile);
            htmlObj.Md5Hash = Common.Md5(htmlObj.Value);
            htmlObj.DiskPath = currSettings.Storage.Directory + "default" + separator + "hello.html";

            Obj textObj = new Obj();
            textObj.IsCompressed = 0;
            textObj.IsContainer = 0;
            textObj.ContainerPath = null;
            textObj.ContentType = "text/plain";
            textObj.Created = DateTime.Now;
            textObj.IsEncrypted = 0;
            textObj.Key = "hello.txt";
            textObj.LastAccess = DateTime.Now;
            textObj.LastUpdate = DateTime.Now;
            textObj.PrimaryNode = currNode;
            textObj.PrimaryUrlWithQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.txt?x-api-key=default";
            textObj.PrimaryUrlWithoutQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.txt";
            textObj.Replicas = null;
            textObj.ReplicationMode = "none";
            textObj.Tags = null;
            textObj.UserGuid = "default";
            textObj.Value = Encoding.UTF8.GetBytes(textFile);
            textObj.Md5Hash = Common.Md5(textObj.Value);
            textObj.DiskPath = currSettings.Storage.Directory + "default" + separator + "hello.txt";

            Obj jsonObj = new Obj();
            jsonObj.IsCompressed = 0;
            jsonObj.IsContainer = 0;
            jsonObj.ContainerPath = null;
            jsonObj.ContentType = "application/json";
            jsonObj.Created = DateTime.Now;
            jsonObj.IsEncrypted = 0;
            jsonObj.Key = "hello.json";
            jsonObj.LastAccess = DateTime.Now;
            jsonObj.LastUpdate = DateTime.Now;
            jsonObj.PrimaryNode = currNode;
            jsonObj.PrimaryUrlWithQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.json?x-api-key=default";
            jsonObj.PrimaryUrlWithoutQs = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.json";
            jsonObj.Replicas = null;
            jsonObj.ReplicationMode = "none";
            jsonObj.Tags = null;
            jsonObj.UserGuid = "default";
            jsonObj.Value = Encoding.UTF8.GetBytes(jsonFile);
            jsonObj.Md5Hash = Common.Md5(jsonObj.Value);
            jsonObj.DiskPath = currSettings.Storage.Directory + "default" + separator + "hello.json";

            if(Common.IsTrue(currSettings.Storage.GatewayMode)) {
                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.html", htmlFile, false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.html", -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.txt", textFile, false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.txt", -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.json", jsonFile, false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.json", -1);
                    return;
                }
            }
            else {
                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.html", Common.SerializeJson(htmlObj), false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.html", -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.txt", Common.SerializeJson(textObj), false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.txt", -1);
                    return;
                }

                if(!Common.WriteFile(currSettings.Storage.Directory + "default" + separator + "hello.json", Common.SerializeJson(jsonObj), false)) {
                    Common.ExitApplication("setup", "Unable to create sample file storage/default/hello.json", -1);
                    return;
                }
            }

            #endregion

            #region Wrap-Up

            //          1         2         3         4         5         6         7         8
            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890

            ConsoleLogger.Log("");
            ConsoleLogger.Log("All finished!");
            ConsoleLogger.Log("");
            ConsoleLogger.Log("If you ever want to return to this setup wizard, just re-run the application");
            ConsoleLogger.Log("from the terminal with the 'setup' argument.");
            ConsoleLogger.Log("");
            ConsoleLogger.Log("Press ENTER to start.");
            ConsoleLogger.Log("");
            Console.ReadLine();

            ConsoleLogger.Log("");
            ConsoleLogger.Log("We created a couple of sample files for you so that you can see your node in");
            ConsoleLogger.Log("action.  Go to the following URLs in your browser and see what happens!");
            ConsoleLogger.Log("");

            switch(currSettings.Environment) {
                case "linux":
                    ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.html?x-api-key=default");
                    ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.html?x-api-key=default&metadata=true");
                    ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.txt?x-api-key=default");
                    ConsoleLogger.Log("  http://" + currNode.DnsHostname + ":" + currNode.Port + "/default/hello.json?x-api-key=default");
                    break;

                case "windows":
                    ConsoleLogger.Log("  http://localhost:" + currNode.Port + "/default/hello.html?x-api-key=default");
                    ConsoleLogger.Log("  http://localhost:" + currNode.Port + "/default/hello.html?x-api-key=default&metadata=true");
                    ConsoleLogger.Log("  http://localhost:" + currNode.Port + "/default/hello.txt?x-api-key=default");
                    ConsoleLogger.Log("  http://localhost:" + currNode.Port + "/default/hello.json?x-api-key=default");
                    break;
            }

            ConsoleLogger.Log("");

            #endregion
        }

        private string SampleHtmlFile(string docLink, string supportLink, string sdkLink) {
            string html =
                "<html>" + Environment.NewLine +
                "   <head>" + Environment.NewLine +
                "      <title>Welcome to kvpbase!</title>" + Environment.NewLine +
                "      <style>" + Environment.NewLine +
                "      	body {" + Environment.NewLine +
                "  		  font-family: arial;" + Environment.NewLine +
                "      	}" + Environment.NewLine +
                "      	h3 {" + Environment.NewLine +
                "  		  background-color: grey;" + Environment.NewLine +
                "         color: white; " + Environment.NewLine +
                "  		  padding: 16px;" + Environment.NewLine +
                "  		  border: 16px;" + Environment.NewLine +
                "      	}" + Environment.NewLine +
                "      	p {" + Environment.NewLine +
                "      	  padding: 4px;" + Environment.NewLine +
                "      	  border: 4px;" + Environment.NewLine +
                "      	}" + Environment.NewLine +
                "      	a {" + Environment.NewLine +
                "      	  background-color: green;" + Environment.NewLine +
                "      	  color: white;" + Environment.NewLine +
                "      	  padding: 4px;" + Environment.NewLine +
                "      	  border: 4px;" + Environment.NewLine +
                "      	}" + Environment.NewLine +
                "      	li {" + Environment.NewLine +
                "      	  padding: 6px;" + Environment.NewLine +
                "      	  border: 6px;" + Environment.NewLine +
                "      	}" + Environment.NewLine +
                "      </style>" + Environment.NewLine +
                "   </head>" + Environment.NewLine +
                "   <body>" + Environment.NewLine +
                "      <h3>Welcome to kvpbase!</h3>" + Environment.NewLine +
                "      <p>If you can see this file, your kvpbase node is running!</p>" + Environment.NewLine +
                "      <p>If you opened this file using your browser, it should have been rendered as HTML. That's because kvpbase preserves the content-type when you write an object, meaning you can use your kvpbase nodes as an extension of your web servers!" + Environment.NewLine +
                "      </p>" + Environment.NewLine +
                "      <p>Remember these helpful links!</p>" + Environment.NewLine +
                "      	<ul>" + Environment.NewLine +
                "      	  <li><a href='" + docLink + "' target='_blank'>API Documentation</a></li>" + Environment.NewLine +
                "      	  <li><a href='" + supportLink + "' target='_blank'>Support Portal</a></li>" + Environment.NewLine +
                "      	  <li><a href='" + sdkLink + "' target='_blank'>Download SDKs</a></li>" + Environment.NewLine +
                "      	</ul>" + Environment.NewLine +
                "   </body>" + Environment.NewLine +
                "</html>";

            return html;
        }

        private string SampleJsonFile(string docLink, string supportLink, string sdkLink) {
            string json =
                "{" + Environment.NewLine +
                "  \"title\": \"Welcome to kvpbase!\", " + Environment.NewLine +
                "  \"data\": \"If you can see this file, your kvpbase node is running!\", " + Environment.NewLine +
                "  \"other_urls\": [" + Environment.NewLine +
                "    \"http://localhost:8080/default/hello.html?x-api-key=default\", " + Environment.NewLine +
                "    \"http://localhost:8080/default/hello.html?x-api-key=default&metadata=true\", " + Environment.NewLine +
                "    \"http://localhost:8080/default/hello.txt?x-api-key=default\" " + Environment.NewLine +
                "  ]," + Environment.NewLine +
                "  \"documentation\": \"" + docLink + "\"," + Environment.NewLine +
                "  \"support\": \"" + supportLink + "\"," + Environment.NewLine +
                "  \"sdks\": \"" + sdkLink + "\"" + Environment.NewLine +
                "}";

            return json;
        }

        private string SampleTextFile(string docLink, string supportLink, string sdkLink) {
            string text =
                "Welcome to kvpbase!" + Environment.NewLine + Environment.NewLine +
                "If you can see this file, your kvpbase node is running!  Now try " +
                "accessing this same URL in your browser, but use the .html extension!" + Environment.NewLine + Environment.NewLine +
                "Remember - documentation is available here: " + docLink + Environment.NewLine + Environment.NewLine +
                "And, our support portal is available here: " + supportLink + Environment.NewLine + Environment.NewLine +
                "Finally, download SDKs here: " + sdkLink + Environment.NewLine + Environment.NewLine;

            return text;
        }
    }
}