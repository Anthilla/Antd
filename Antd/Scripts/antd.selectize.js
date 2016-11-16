$('[data-input="selectize"]').each(function () {
    var value = $(this).val();
    $(this).selectize({
        create: true,
        render: { option: SelectizerOptions.render },
        load: SelectizerOptions.load,
        onInitialize: function () {
            var s = this;
            var actualValue = value;
            if (actualValue) {
                var arr = actualValue.split(",");
                $.each(arr, function (k, v) {
                    s.addOption(v);
                    s.setValue(actualValue);
                    s.blur();
                });
            }
        }
    });
    $(this).hide();
});

//selectize
$(document).ready(function () {
    $.when(
        LoadNetworkIf()
    ).then();
});

function Callback(callback, url) {
    var aj = $.ajax({
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
    if ($('[data-role="show-net-if"]').length > 0) {
        $('[data-role="show-net-if"]').each(function () {
            var init = InitNetworkIf($(this));
            if (init != undefined) {
                init.load(function (callback) {
                    Callback(callback, this.settings.remoteUrl);
                });
            }
            $(this).hide();
        });
    }
}
