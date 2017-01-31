"use strict";

app.controller("RequestMaterialGenericControllerContent", ["$rootScope", "$scope", "$http", RequestMaterialGenericControllerContent]);

function RequestMaterialGenericControllerContent($rootScope, $scope, $http) {

    $scope.Create = function (el) {
        var data = $.param({
            Ward: $scope.Ward,
            Nurse: $scope.Nurse,
            Materials: [
                {
                    MaterialName: $scope.MaterialName,
                    MaterialQty: $scope.MaterialQty,
                    MaterialFormat: $scope.MaterialFormat
                }
            ]
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/requestmaterialgeneric", data).then(
            function () {
                $scope.Ward = null;
                $scope.Nurse = null;
                $rootScope.$broadcast("RequestMaterialGenericDashboardSave");
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

    $scope.SaveFile = function (file) {
        var mats = "";
        angular.forEach(file.Materials, function (mat) {
            mats += mat.MaterialName + "," + mat.MaterialQty + "," + mat.MaterialFormat + "," + mat.Status + "," + mat.Note + ";";
        });
        var data = $.param({
            Guid: file.Guid,
            Materials: mats
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/requestmaterialgeneric/save", data).then(
            function () {
                $http.get("/repo/requestmaterialgeneric").success(function (data) {
                    $scope.files = data;
                });
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SubmitFile = function (file) {
        var mats = "";
        angular.forEach(file.Materials, function (mat) {
            mats += mat.MaterialName + "," + mat.MaterialQty + "," + mat.MaterialFormat + "," + mat.Status + "," + mat.Note + ";";
        });
        var data = $.param({
            Guid: file.Guid,
            Materials: mats
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/requestmaterialgeneric/save", data).then(
            function () {
                var data = $.param({
                    Guid: file.Guid
                });
                $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                $http.post("/repo/requestmaterialgeneric/submit", data).then(
                    function () {
                        $http.get("/repo/requestmaterialgeneric").success(function (data) {
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
        file.Materials.push({ MaterialName: "", MaterialQty: 0, MaterialFormat: "" });
    }

    $http.get("/cache/material_generic").success(function (data) {
        $scope.MaterialNameOptions = data;
    });

    $scope.MaterialNameConfig = {
        valueField: "Articolo",
        labelField: "Articolo",
        searchField: ["Articolo"],
        persist: false,
        create: true,
        maxItems: 1
    };

    $scope.HideIfMinor = function (v1, v2) {
        return v1 < v2;
    }

    $scope.$on("RequestMaterialGenericDashboardSave", function () {
        $http.get("/repo/requestmaterialgeneric").success(function (data) {
            $scope.files = data;
        });
    });

    //get
    $http.get("/repo/requestmaterialgeneric").success(function (data) {
        $scope.files = data;
    });

    $scope.greaterThan = function (prop, val) {
        return function (item) {
            return item[prop] > val;
        }
    }

    $scope.smallerThan = function (prop, val) {
        return function (item) {
            return item[prop] < val;
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
        $http.post("/repo/requestmaterialgeneric/archive", data).then(
            function () {
                $http.get("/repo/requestmaterialgeneric").success(function (data) {
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
        $http.post("/repo/requestmaterialgeneric/delete", data).then(
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
        $http.post("/repo/requestmaterialgeneric/edit/status", data).then(
            function () {
                $http.get("/repo/requestmaterialgeneric").success(function (data) {
                    $scope.files = data;
                });
            },
        function (response) {
            console.log(response);
        });
    }
}
