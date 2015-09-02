//selectize
$(document).ready(function () {
    $.when(
        LoadSystemdUnits()
    ).then(
        console.log('.')
    );
});

function Callback(callback, url) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        data: {
            s: ' '
        },
        error: function () {
            callback();
            return false;
        },
        success: function (data) {
            callback(data);
        }
    });
}

var SelectizerOptions = function () {
    return {
        load: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
                data: {
                    s: query
                },
                error: function (input) {
                    callback();
                },
                success: function (data) {
                    callback(data);
                }
            });
        },
        render: function (data, escape) {
            return '<div>' +
                '<span data-name="' + escape(data.name) + '" class="button name bg-anthilla-violet">' +
                String(data.name) +
                '</span>' +
                '</div>';
        }
    };
}();

var $systemdUnitsSelectizer = $('#show-units').selectize({
    valueField: 'name',
    labelField: 'name',
    searchField: 'name',
    create: false,
    render: { option: SelectizerOptions.render },
    remoteUrl: '/units/list',
    load: SelectizerOptions.load
});

function LoadSystemdUnits() {
    if ($('#show-units').size() > 0) {
        var systemdUnitsSelectizer = $systemdUnitsSelectizer[0].selectize;
        systemdUnitsSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
            $('#show-units').hide();
        });
    }
}