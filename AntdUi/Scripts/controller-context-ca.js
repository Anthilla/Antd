"use strict";

app.controller("CaController", ["$scope", "$http", CaController]);

function CaController($scope, $http) {
    $scope.createScCert = function (el) {
        var data = $.param({
            Name: el.Name,
            Passphrase: el.Passphrase,
            Upn: el.Upn,
            Email: el.Email,
            CountryName: el.CountryName,
            StateOrProvinceName: el.StateOrProvinceName,
            LocalityName: el.LocalityName,
            OrganizationName: el.OrganizationName,
            OrganizationalUnitName: el.OrganizationalUnitName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/certificate/sc", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.createDcCert = function (el) {
        var data = $.param({
            Name: el.Name,
            Passphrase: el.Passphrase,
            Guid: el.Guid,
            Dns: el.Dns,
            Email: el.Email,
            CountryName: el.CountryName,
            StateOrProvinceName: el.StateOrProvinceName,
            LocalityName: el.LocalityName,
            OrganizationName: el.OrganizationName,
            OrganizationalUnitName: el.OrganizationalUnitName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/certificate/dc", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.createServerCert = function (el) {
        var data = $.param({
            Name: el.Name,
            Passphrase: el.Passphrase,
            Email: el.Email,
            CountryName: el.CountryName,
            StateOrProvinceName: el.StateOrProvinceName,
            LocalityName: el.LocalityName,
            OrganizationName: el.OrganizationName,
            OrganizationalUnitName: el.OrganizationalUnitName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/certificate/server", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.createUserCert = function (el) {
        var data = $.param({
            Name: el.Name,
            Passphrase: el.Passphrase,
            Email: el.Email,
            CountryName: el.CountryName,
            StateOrProvinceName: el.StateOrProvinceName,
            LocalityName: el.LocalityName,
            OrganizationName: el.OrganizationName,
            OrganizationalUnitName: el.OrganizationalUnitName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/certificate/user", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (ca) {
        var data = $.param({
            KeyPassout: ca.KeyPassout,
            RootCountryName: ca.RootCountryName,
            RootStateOrProvinceName: ca.RootStateOrProvinceName,
            RootLocalityName: ca.RootLocalityName,
            RootOrganizationName: ca.RootOrganizationName,
            RootOrganizationalUnitName: ca.RootOrganizationalUnitName,
            RootCommonName: ca.RootCommonName,
            RootEmailAddress: ca.RootEmailAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ca/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/ca").success(function (data) {
        $scope.Ca = data;
    });
}