"use strict";

app.controller("TextFileControllerDashboard", ["$scope", "$http", TextFileControllerDashboard]);

function TextFileControllerDashboard($scope, $http) {
    $scope.Create = function (el) {
        var data = $.param({
            Name: $scope.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/textfile", data).then(
            function (response) {
                console.log(response);
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
}

app.controller("TextFileControllerContent", ["$scope", "$http", TextFileControllerContent]);

function TextFileControllerContent($scope, $http) {
    $http.get("/repo/textfile").success(function (data) {
        $scope.files = data;
    });

    $scope.ExpandRows = function (guid) {
        $('[data-icon="' + guid + '"]').toggleClass("mif-chevron-right").toggleClass("mif-expand-more");
        $('[data-row="' + guid + '"]').toggle();
    };

    $scope.EnableEdit = function (file) {
        $('[data-editable="' + file.Guid + '"]').each(function () {
            $(this).show();
        });
        $('[data-static="' + file.Guid + '"]').each(function () {
            $(this).hide();
        });

        $scope.content = file.Content;
        $scope.guid = file.Guid;
        $scope.filename = file.Name;
    };

    $scope.UpdateName = function (value) {
        this.file.Name = value;
    }

    $scope.UpdateContent = function () {
        this.file.Content = value.Content;
        $scope.content = value.Content;
    }

    $scope.content = function (value) {
        return value;
    }

    $scope.SaveFile = function () {
        var data = $.param({
            Guid: $scope.guid,
            Name: $scope.filename,
            Content: $scope.content
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/textfile/edit", data).then(
            function (response) {
                console.log(response);
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.EditFiles = function (guid) {
        var sourceModel = this.file;
        var data = $.param({
            Guid: sourceModel.Guid,
            Name: sourceModel.Name,
            Content: sourceModel.Content
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/textfile/edit", data).then(
            function (response) {
                console.log(response);
            },
        function (response) {
            console.log(response);
        });

        $('[data-editable="' + guid + '"]').each(function () {
            $(this).hide();
        });
        $('[data-static="' + guid + '"]').each(function () {
            $(this).show();
        });
        $('[data-row="' + guid + '"]').each(function () {
            $(this).hide();
        });
    };

    $scope.CloseFilesEditing = function (guid) {
        $('[data-editable="' + guid + '"]').each(function () {
            $(this).hide();
        });
        $('[data-static="' + guid + '"]').each(function () {
            $(this).show();
        });
        $('[data-row="' + guid + '"]').each(function () {
            $(this).hide();
        });
    };

    $scope.DeleteFiles = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/textfile/delete", data).then(
            function (response) {
                console.log(response);
            },
        function (response) {
            console.log(response);
        });
        $('[data-user-table="' + guid + '"]').remove();
    };
}
