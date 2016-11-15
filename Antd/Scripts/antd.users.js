

$(document).on("ready", function () {
    $.when(
        LoadUserEntitiesUnits()
    ).then();
});

var UserSelectizerOptions = function () {
    return {
        load: function (query, callback) {
            if (!query.length) return callback();
            var aj = $.ajax({
                url: this.settings.remoteUrl,
                type: "GET",
                dataType: "json",
                data: {
                    s: query
                },
                error: function () {
                    callback();
                },
                success: function (data) {
                    callback(data);
                }
            });
            _requests.push(aj);
            return callback;
        },
        render: function (data, escape) {
            return '<div><span data-guid="' + escape(data.guid) + '" class="button name bg-anthilla-violet">' + String(data.alias) + "</span></div>";
        }
    };
}();

var $userEntitiesSelectizer = $("#userEntities").selectize({
    delimiter: ",",
    persist: false,
    valueField: "guid",
    labelField: "alias",
    searchField: ["alias", "guid"],
    render: { option: UserSelectizerOptions.render },
    remoteUrl: "/users/json",
    load: UserSelectizerOptions.load,
    sortField: "alias"
});

function LoadUserEntitiesUnits() {
    if ($("#userEntities").size() > 0) {
        var userEntitiesSelectizer = $userEntitiesSelectizer[0].selectize;
        userEntitiesSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
            $("#userEntities").hide();
        });
    }
}

$('[data-role="UpdateUserPassword"]').on("click", function () {
    var user = $(this).attr("data-user");
    var container = $(this).parents('[data-role="UpdatePasswordPanel"]');
    var pass = container.find('[data-role="Password01"]').val();
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/users/change/password",
        type: "POST",
        data: {
            User: user,
            Password: pass
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});