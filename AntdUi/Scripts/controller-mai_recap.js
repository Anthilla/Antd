"use strict";

app.controller("MaiRecapControllerDashboard", ["$rootScope", "$scope", "$http", MaiRecapControllerDashboard]);

function MaiRecapControllerDashboard($rootScope, $scope, $http) {
    $scope.Create = function (el) {
        var data = $.param({
            Ward: $scope.Ward,
            Nurse: $scope.Nurse,
            Month: $scope.Month,
            TotalBed: $scope.TotalBed,
            TotalAvailableSupport: $scope.TotalAvailableSupport

        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap", data).then(
            function () {
                $scope.Ward = null;
                $scope.Nurse = null;
                $rootScope.$broadcast("MaiRecapDashboardSave");
            },
        function (response) {
            console.log(response);
        });
        $(el).hide();
    };

    $scope.ResetDashboard = function (el) {
        $(el).find("input").each(function () {
            $(this).val("");
        });
    };

    $scope.CloseDashboard = function (el) {
        $(el).find("input").each(function () {
            $(this).val("");
        });
        $(el).hide();
    };

    $scope.Ward = null;

    $http.get("/cache/ward").success(function (data) {
        $scope.WardOptions = data;
        $http.get("/rawdata/project").success(function (rawdata) {
            angular.forEach(rawdata, function (v) {
                var found = $scope.WardOptions.some(function (el) {
                    return el.Value.toUpperCase() === v.Alias.toUpperCase();
                });
                if (!found) {
                    $scope.WardOptions.push({ Id: v.Guid, Value: v.Alias });
                }
            });
        });
    });

    $scope.Nurse = null;
    $scope.HideNurse = false;

    $http.get("/auth/user").success(function (data) {
        $scope.Nurse = data.FirstName + " " + data.LastName;
        $scope.HideNurse = true;
    });

    $http.get("/cache/nurse").success(function (data) {
        $scope.NurseOptions = data;
    });

    $scope.Month = null;

    $scope.MonthOptions = [
        { Key: "201612", Value: "Dicembre 2016" },
        { Key: "201701", Value: "Gennaio 2017" },
        { Key: "201702", Value: "Febbraio 2017" },
        { Key: "201703", Value: "Marzo 2017" },
        { Key: "201704", Value: "Aprile 2017" },
        { Key: "201705", Value: "Maggio 2017" },
        { Key: "201706", Value: "Giugno 2017" },
        { Key: "201707", Value: "Luglio 2017" },
        { Key: "201708", Value: "Agosto 2017" },
        { Key: "201709", Value: "Settembre 2017" },
        { Key: "201710", Value: "Ottobre 2017" },
        { Key: "201711", Value: "Novembre 2017" },
        { Key: "201712", Value: "Dicembre 2017" }
    ];

    $scope.CacheConfig = {
        valueField: "Value",
        labelField: "Value",
        searchField: ["Value"],
        persist: false,
        create: true,
        maxItems: 1
    };

    $scope.HideIfExist = function (v1, v2) {
        return v1 === null || v2 === null;
    }
}

app.controller("MaiRecapControllerContent", ["$rootScope", "$scope", "$http", "$window", MaiRecapControllerContent]);

function MaiRecapControllerContent($rootScope, $scope, $http, $window) {

    $scope.SaveFile = function (file) {
        var gsts = "";
        angular.forEach(file.Guests, function (gst) {
            gsts += gst.Gender + "," + gst.BedIndex + "," + gst.GuestName + "," +
                gst.TotalBath + "," + gst.TotalNail + "," + gst.TotalFemaleShave + "," + gst.TotalMaleShave + "," +
                gst.TotalBarber + "," + gst.TotalSupport + "," + gst.TotalBedClean + "," + gst.TotalNotMobilized + "," + gst.TotalRest + "," +
                gst.Note + "," + gst.Exit + ";";
        });
        var data = $.param({
            Guid: file.Guid,
            TotalKitchenClean: file.TotalKitchenClean,
            TotalNightDiaperChange: file.TotalNightDiaperChange,
            Guests: gsts
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap/save", data).then(
            function () {
                $http.get("/repo/mairecap").success(function (data) {
                    $scope.files = data;
                });
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SubmitFile = function (file) {
        var gsts = "";
        angular.forEach(file.Guests, function (gst) {
            gsts += gst.Gender + "," + gst.BedIndex + "," + gst.GuestName + "," +
                gst.TotalBath + "," + gst.TotalNail + "," + gst.TotalFemaleShave + "," + gst.TotalMaleShave + "," +
                gst.TotalBarber + "," + gst.TotalSupport + "," + gst.TotalBedClean + "," + gst.TotalNotMobilized + "," + gst.TotalRest + "," +
                gst.Note + "," + gst.Exit + ";";
        });
        var data = $.param({
            Guid: file.Guid,
            TotalKitchenClean: file.TotalKitchenClean,
            TotalNightDiaperChange: file.TotalNightDiaperChange,
            Guests: gsts
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap/save", data).then(
            function () {
                var data = $.param({
                    Guid: file.Guid
                });
                $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                $http.post("/repo/mairecap/submit", data).then(
                    function () {
                        $http.get("/repo/mairecap").success(function (data) {
                            $scope.files = data;
                        });
                    },
                function (response) {
                    console.log(response);
                });
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.Increase = function (file) {
        file.Guests.push({
            BedIndex: 0,
            Gender: "",
            GuestName: "",
            Exit: "",
            Note: "",
            TotalBath: 0,
            TotalNail: 0,
            TotalFemaleShave: 0,
            TotalMaleShave: 0,
            TotalBarber: 0,
            TotalSupport: 0,
            TotalBedClean: 0,
            TotalNotMobilized: 0,
            TotalRest: 0
        });
    }

    $http.get("/cache/guest_name").success(function (data) {
        $scope.GuestNameOptions = data;
    });

    $scope.CacheConfig = {
        valueField: "Value",
        labelField: "Value",
        searchField: ["Value"],
        persist: false,
        create: true,
        maxItems: 1
    };

    $scope.HideIfMinor = function (v1, v2) {
        return v1 < v2;
    }

    $scope.$on("MaiRecapDashboardSave", function () {
        $http.get("/repo/mairecap").success(function (data) {
            $scope.files = data;
        });
    });

    //get
    $http.get("/repo/mairecap").success(function (data) {
        $scope.files = data;
    });

    $scope.greaterThan = function (prop, val) {
        return function (item) {
            return item[prop] > val;
        }
    }

    $scope.GetStatusString = function (index) {
        return RequestStatus[parseInt(index + 1)];
    }

    $scope.ExpandRows = function (guid) {
        $('[data-icon="' + guid + '"]').toggleClass("mif-chevron-right").toggleClass("mif-expand-more");
        $('[data-row="' + guid + '"]').toggle();
    };

    $scope.CompleteTask = function (file) {
        var data = $.param({
            Guid: file.Guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap/archive", data).then(
            function () {
                $http.get("/repo/mairecap").success(function (data) {
                    $scope.files = data;
                });
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.GetElementPosition = function (arr, el) {
        return arr.indexOf(el) + 1;
    }

    $scope.DeleteFile = function (file) {
        var data = $.param({
            Guid: file.Guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap/delete", data).then(
            function () {
                var index = $scope.files.indexOf(file);
                $scope.files.splice(index, 1);
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.hide = true;

    $scope.Show = function (el) {
        var isChecked = $(el).find("input:checkbox").is(":checked");
        if (isChecked) {
            $(el).find("span").addClass("bg-anthilla-green");
            $scope.hide = false;

        } else {
            $(el).find("span").removeClass("bg-anthilla-green");
            $scope.hide = true;
        }
    }

    $scope.RejectFile = function () {
        var sourceModel = this.file;
        sourceModel.Status = -1;
        var data = $.param({
            Guid: sourceModel.Guid,
            Status: sourceModel.Status
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/mairecap/edit/status", data).then(
            function () {
                $http.get("/repo/mairecap").success(function (data) {
                    $scope.files = data;
                });
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.PrintPreview = function (file, isNewWindow) {
        if (isNewWindow) {
            $window.open($window.location.origin + $window.location.pathname + "" + "#/mai_recap_print?g=" + file.Guid, "_blank");
        }
        else {
            $window.location.hash = "#/mai_recap_print?g=" + file.Guid;
        }
    };
}