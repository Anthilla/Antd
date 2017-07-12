"use strict";

app.controller("VfsController", ["$scope", "$http", "Upload", "$interval", VfsController]);

function VfsController($scope, $http, Upload, $interval) {

    $scope.NewDirectoryName = "";

    $scope.files = [];

    $scope.submit = function () {
        if ($scope.files.length) {
            for (var i = 0; i < $scope.files.length; i++) {
                $scope.upload($scope.files[i]);
            }
        }
    };

    $scope.uploadThis = function (index, file) {
        $scope.upload(file);
        $scope.removeFromUpload(index);
    }

    $scope.upload = function (file) {
        Upload.upload({
            url: $scope.GetPartialUrl("0/0"),
            data: {
                file: file,
                'ContainerPath': $scope.RequestedDirectory
            }
        }).then(function () {
            file.preupload = true;
            $scope.Share = {
                PackageCode: "",
                Destination: "",
                ProjectUnselected: true,
                ProjectStringPost: "",
                UserStringPost: "",
                TagStringPost: "",
                PackageName: "",
            }
            $scope.files = [];
            $scope.GetContainer($scope.RequestedDirectory);
            $scope.ShowResponseMessage("ok");
        }, function (resp) {
            switch (resp.status) {
                case 400:
                    $scope.ShowResponseMessage(resp.status + ": An error occured while processing your file");
                case 500:
                    $scope.ShowResponseMessage(resp.status + ": Requested folder does not exist on your File System");
                case 401:
                    $scope.ShowResponseMessage(resp.status + ": You don't have permissions to upload files in this folder");
                default:
                    $scope.ShowResponseMessage(resp.status);
            }
        }, function (evt) {
            var progressPercentage = parseInt(100 * evt.loaded / evt.total);
            file.progress = progressPercentage;
            console.log("progress: " + progressPercentage + "% " + evt.config.data.file.name);
        });
    };

    var baseUrl = "/vfs"
    var clientUrl = baseUrl + "/client"

    $scope.RequestedDirectory = "/";

    $scope.IsRoot = true;

    $scope.DownloadUrl = function (obj) {
        var api = $scope.GetPartialUrl("0/1");
        return api + "?path=" + $scope.RequestedDirectory.replace(/\//g, "%2F") + "&file=" + obj.Key;
    }

    Array.prototype.clean = function (deleteValue) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == deleteValue) {
                this.splice(i, 1);
                i--;
            }
        }
        return this;
    };

    $scope.EnterFolder = function (folderName) {
        var folderPath;
        if (folderName === "..") {
            var path = $scope.RequestedDirectory;
            var arr = path.split('/').clean("");
            if (arr.length === 1) {
                folderPath = "/";
            }
            else {
                arr = arr.slice(0, -1);
                folderPath = "/" + arr.join('/');
            }
        }
        else {
            if ($scope.RequestedDirectory === "/") {
                folderPath = $scope.RequestedDirectory + folderName;
            }
            else {
                folderPath = $scope.RequestedDirectory + "/" + folderName;
            }
        }
        $scope.IsRoot = folderPath === "/";
        $scope.RequestedDirectory = folderPath;
        $scope.GetContainer($scope.RequestedDirectory);
    }

    $scope.CurrentDirectory = {
        Url: "",
        UserGuid: "",
        ContainerName: "",
        Size: 0,
        NumObjects: 0,
        Created: "",
        LastUpdate: "",
        LastAccess: "",
        ContainerPath: [],
        ChildContainers: [],
        ObjectMetadata: [{
            Key: "",
            Size: 0,
            Created: "",
            LastUpdate: "",
            LastAccess: ""
        }]
    };

    $scope.DeleteContainer = function (containerPath) {
        var api = $scope.GetPartialUrl("1/5");
        var data = $.param({
            ContainerPath: $scope.RequestedDirectory + "/" + containerPath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () {
            $scope.EnterFolder("..");
            $scope.GetContainer($scope.RequestedDirectory);
        }, function (r) { console.log(r); });
    }

    $scope.RenameContainer = function (containerPath, newContainer) {
        console.log(newContainer)
        var api = $scope.GetPartialUrl("1/4");
        var data = $.param({
            ContainerPath: $scope.RequestedDirectory + "/" + containerPath,
            NewName: newContainer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.MoveContainer = function (containerPath, newContainer) {
        var api = $scope.GetPartialUrl("1/3");
        var data = $.param({
            ContainerPath: containerPath,
            NewContainer: newContainer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.VerifyContainer = function (containerPath) {
        var api = $scope.GetPartialUrl("1/2");
        var data = $.param({
            ContainerPath: containerPath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.GetContainer = function (containerPath) {
        var api = $scope.GetPartialUrl("1/1");
        var data = $.param({
            ContainerPath: containerPath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function (resp) {
            $scope.CurrentDirectory = resp.data;
        }, function (r) { console.log(r); });
    }

    $scope.CreateContainer = function () {
        var api = $scope.GetPartialUrl("1/0");
        var data = $.param({
            ContainerPath: $scope.RequestedDirectory + "/" + $scope.NewDirectoryName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () {
            $scope.NewDirectoryName = "";
            $scope.GetContainer($scope.RequestedDirectory);
        }, function (r) { console.log(r); });
    }

    $scope.DeleteObject = function (objectPath) {
        var api = $scope.GetPartialUrl("0/5");
        var data = $.param({
            ObjectPath: $scope.RequestedDirectory + "/" + objectPath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () {
            $scope.GetContainer($scope.RequestedDirectory);
        }, function (r) { console.log(r); });
    }

    $scope.RenameObject = function (objectPath, newName) {
        var api = $scope.GetPartialUrl("0/4");
        var data = $.param({
            ObjectPath: $scope.RequestedDirectory + "/" + objectPath,
            NewName: newName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.MoveObject = function (objectPath, newContainer) {
        var api = $scope.GetPartialUrl("0/3");
        var data = $.param({
            ObjectPath: objectPath,
            NewContainer: newContainer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.VerifyObject = function (objectPath) {
        var api = $scope.GetPartialUrl("0/2");
        var data = $.param({
            ObjectPath: objectPath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(api, data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    //$scope.CreateObject = function () {
    //    var api = $scope.GetPartialUrl("0/0");
    //}

    $scope.Connected = false;

    $scope.connect = function () {
        $scope.GetContainer($scope.RequestedDirectory);
        $scope.Connected = true;
    }

    $scope.disconnect = function () {
        $scope.RequestedDirectory = "/";
        $scope.Connected = false;
    }

    $scope.GetPartialUrl = function (actionCode) {
        // /client/{userguid}/{server}/{port}/0/0
        return clientUrl + "/" + $scope.Connection.User + "/" + $scope.Connection.Server + "/" + $scope.Connection.Port + "/" + actionCode;
    }

    $scope.Connection = {
        User: "default",
        Server: "127.0.0.1",
        Port: "8080"
    };

    $scope.saveUserMasters = function () {
        var json = angular.toKson($scope.UserMasters);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(baseUrl + "/save/users", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveApiKeyPermissions = function () {
        var json = angular.toKson($scope.ApiKeyPermissions);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(baseUrl + "/save/apikeypermissions", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveApiKeys = function () {
        var json = angular.toKson($scope.ApiKeys);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(baseUrl + "/save/apikeys", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveTopology = function () {
        var json = angular.toKson($scope.Topology);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(baseUrl + "/save/topology", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveSettings = function () {
        var json = angular.toKson($scope.Settings);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post(baseUrl + "/save/system", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Get = function () {
        $http.get(baseUrl).success(function (data) {
            $scope.Settings = data.Settings;
            $scope.Topology = data.Topology;
            $scope.ApiKeys = data.ApiKeys;
            $scope.ApiKeyPermissions = data.ApiKeyPermissions;
            $scope.UserMasters = data.UserMasters;
        });
    }
    $scope.Get();

    $scope.ResponseMessage = "";
    $scope.ResponseMessageHide = true;
    $scope.ShowResponseMessage = function (message) {
        $scope.ResponseMessageHide = false;
        $scope.ResponseMessage = message;
        $interval(function () {
            $scope.ResponseMessageHide = true;
        }, 1666);
        $scope.Get();
    }

    $scope.ResponseMessageHideSelf = function () {
        $scope.ResponseMessage = "";
        $scope.ResponseMessageHide = true;
    }
}

