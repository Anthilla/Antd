var $sl = jQuery.noConflict();

$sl(document).on("ready", function () {
    $sl.when(
        LoadNetworkIf()
    ).then();
});

function Callback(callback, url) {
    var aj = $sl.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        data: {
            s: " "
        },
        error: function () {
            callback();
            return false;
        },
        success: function (data) {
            callback(data);
        }
    });
    _requests.push(aj);
}

var SelectizerOptions = function () {
    return {
        load: function (query, callback) {
            if (!query.length) return callback();
            var aj = $sl.ajax({
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
        },
        render: function (data, escape) {
            return "<div>" +
                '<span data-name="' + escape(data.name) + '" class="button name bg-anthilla-violet">' +
                String(data.name) +
                "</span>" +
                "</div>";
        }
    };
}();

function InitNetworkIf(input) {
    input.selectize({
        valueField: "name",
        labelField: "name",
        searchField: "name",
        create: false,
        render: { option: SelectizerOptions.render },
        remoteUrl: "/network/list/if",
        load: SelectizerOptions.load
    });
}

function LoadNetworkIf() {
    if ($sl('[data-role="show-net-if"]').length > 0) {
        $sl('[data-role="show-net-if"]').each(function () {
            var init = InitNetworkIf($sl(this));
            if (init != undefined) {
                init.load(function (callback) {
                    Callback(callback, this.settings.remoteUrl);
                });
            }
            $sl(this).hide();
        });
    }
}
