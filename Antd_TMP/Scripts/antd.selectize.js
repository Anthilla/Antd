//selectize
$(document).ready(function () {
    $.when(
        LoadSystemdUnits()
    ).then();
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

var $valueBundleTagSelectizer = $('#valueBundleTag').selectize({
    maxItems: 1,
    createOnBlur: true,
    delimiter: ',',
    persist: false,
    create: function (input) {
        return {
            value: input,
            name: input,
            text: input
        }
    },
    sortField: {
        field: 'name',
        direction: 'asc'
    },
    valueField: 'name',
    labelField: 'name',
    searchField: 'name',
    render: { option: SelectizerOptions.render },
    remoteUrl: '/cfg/tags',
    load: SelectizerOptions.load,
    sortField: 'name'
});

function LoadSystemdUnits() {
    if ($('#valueBundleTag').size() > 0) {
        var valueBundleTagSelectizer = $valueBundleTagSelectizer[0].selectize;
        valueBundleTagSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
            $('#valueBundleTag').hide();
        });
    }
}

var $commandBundleLayoutSelectizer = $('#commandBundleLayout').selectize({
    maxItems: 1,
    createOnBlur: true,
    delimiter: ',',
    persist: false,
    create: function (input) {
        return {
            value: input,
            name: input,
            text: input
        }
    },
    sortField: {
        field: 'name',
        direction: 'asc'
    },
    valueField: 'name',
    labelField: 'name',
    searchField: 'name',
    render: { option: SelectizerOptions.render },
    remoteUrl: '/cfg/layouts',
    load: SelectizerOptions.load,
    sortField: 'name'
});

function LoadSystemdUnits() {
    if ($('#commandBundleLayout').size() > 0) {
        var commandBundleLayoutSelectizer = $commandBundleLayoutSelectizer[0].selectize;
        commandBundleLayoutSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
            $('#commandBundleLayout').hide();
        });
    }
}