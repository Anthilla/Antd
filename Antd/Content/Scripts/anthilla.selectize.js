function SelectizeInput(guid) {
    $('#' + guid + 'TagsEdit').selectize({
        delimiter: ',',
        persist: false,
        create: function (input) {
            return {
                value: input,
                text: input
            }
        }
    });
    return false;
}

function SelectizeLeader(guid) {
    $('#' + guid + 'LeaderEdit').selectize({
        maxItems: 1,
        valueField: 'guid',
        labelField: 'alias',
        searchField: ['fname', 'lname', 'alias'],
        create: false,
        render: { option: UserSelectizer.renderOptions },
        remoteUrl: '/rawdata/user',
        load: UserSelectizer.loadOptions
    });
    return false;
}

function SelectizeCompany(guid) {
    $('#' + guid + 'CompanyEdit').selectize({
        maxItems: 1,
        valueField: 'guid',
        labelField: 'alias',
        searchField: 'alias',
        create: false,
        render: { option: ItemSelectizer.renderOptions },
        remoteUrl: '/rawdata/company',
        load: ItemSelectizer.loadOptions
    });
    return false;
}

function SelectizeProjects(guid) {
    $('#' + guid + 'ProjectEdit').selectize({
        valueField: 'guid',
        labelField: 'alias',
        searchField: 'alias',
        create: false,
        render: { option: ItemSelectizer.renderOptions },
        remoteUrl: '/rawdata/project',
        load: ItemSelectizer.loadOptions
    });
    return false;
}

function SelectizeGroups(guid) {
    $('#' + guid + 'UgroupEdit').selectize({
        valueField: 'guid',
        labelField: 'alias',
        searchField: 'alias',
        create: false,
        render: { option: ItemSelectizer.renderOptions },
        remoteUrl: '/rawdata/ugroup',
        load: ItemSelectizer.loadOptions
    });
    return false;
}

function SelectizeFunctions(guid) {
    $('#' + guid + 'FunctionsEdit').selectize({
        valueField: 'guid',
        labelField: 'alias',
        searchField: 'alias',
        create: false,
        render: { option: ItemSelectizer.renderOptions },
        remoteUrl: '/rawdata/func',
        load: ItemSelectizer.loadOptions
    });
    return false;
}

$('#UGroupTag').selectize({
    delimiter: ',',
    persist: false,
    create: function (input) {
        return {
            value: input,
            text: input
        }
    }
});

var ItemSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
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
        renderOptions: function (data, escape) {
            return '<div><span id="' + escape(data.guid) + '" class="button name bg-anthilla-violet">' + escape(data.alias) + '</span></div>';
        }
    };
}();

var UserSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
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
        },
        renderOptions: function (data, escape) {
            return '<div><span id="' + escape(data.guid) + '" class="button name bg-anthilla-violet">' + escape(data.fname) + ' ' + escape(data.lname) + '</span></div>';
        }
    };
}();

var ResourceSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
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
        },
        renderOptions: function (data, escape) {
            return '<div><span id="' + escape(data.guid) + '" class="button name bg-anthilla-violet">' + escape(data.data) + '</span></div>';
        }
    };
}();

$('.tag-input').selectize({
    createOnBlur: true,
    delimiter: ',',
    persist: false,
    create: function (input) {
        return {
            value: input,
            alias: input,
            guid: input,
            context: input,
            text: input
        }
    },
    sortField: {
        field: 'alias',
        direction: 'asc'
    },
    valueField: 'alias',
    labelField: 'alias',
    searchField: ['alias', 'context'],
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/tags',
    load: ItemSelectizer.loadOptions,
    sortField: 'input'
});

$('.event-type').selectize({
    maxItems: 1,
    createOnBlur: true,
    persist: false,
    create: function (input) {
        return {
            value: input,
            alias: input,
            guid: input,
            context: input,
            text: input
        }
    },
    sortField: {
        field: 'alias',
        direction: 'asc'
    },
    valueField: 'alias',
    labelField: 'alias',
    searchField: 'alias',
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/eventtypes',
    load: ItemSelectizer.loadOptions,
    sortField: 'input'
});

$('#select-user').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'nameLabel',
    searchField: ['fname', 'lname', 'alias'],
    create: false,
    render: { option: UserSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: UserSelectizer.loadOptions
});

$('#select-user-multi').selectize({
    valueField: 'guid',
    labelField: 'nameLabel',
    searchField: ['fname', 'lname', 'alias'],
    create: false,
    render: { option: UserSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: UserSelectizer.loadOptions
});

$('#select-company').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company',
    load: ItemSelectizer.loadOptions
});

$('#select-company-multi').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company',
    load: ItemSelectizer.loadOptions
});

$('#select-company-default').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company/default',
    load: ItemSelectizer.loadOptions
});

$('#select-group').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/ugroup',
    load: ItemSelectizer.loadOptions
});

$('#select-group-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/ugroup',
    load: ItemSelectizer.loadOptions
});

$('#select-funcgrp-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/funcgrp',
    load: ItemSelectizer.loadOptions
});

$('#select-funcs').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/func',
    load: ItemSelectizer.loadOptions
});

$('#select-project').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/project',
    load: ItemSelectizer.loadOptions
});

$('#select-project-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/project',
    load: ItemSelectizer.loadOptions
});

$('#select-license').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/license',
    load: ItemSelectizer.loadOptions
});

$('#select-schema').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/schemas',
    load: ItemSelectizer.loadOptions
});

$('#select-resources').selectize({
    valueField: 'guid',
    labelField: 'data',
    searchField: ['data'],
    create: false,
    render: { option: ResourceSelectizer.renderOptions },
    remoteUrl: '/rawdata/resources',
    load: ResourceSelectizer.loadOptions
});

$('#select-project-mono-fs').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/project',
    load: ItemSelectizer.loadOptions,
    onChange: function (value) {
        if (!value.length) return;
        var userUrl = '/rawdata/byproject/join/user/company/' + value;
        ShowProjectCode(value);
        $('#select-user-join-comp').selectize({
            valueField: 'userGuid',
            labelField: 'userNameLabel',
            searchField: ['userFname', 'userLname', 'userAlias', 'companyAlias'],
            create: false,
            render: { option: UserJoinSelectizer.renderOptions },
            remoteUrl: userUrl,
            load: UserJoinSelectizer.loadOptions,
        });
    }
});

//join selectizer

var UserJoinSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
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
        },
        renderOptions: function (data, escape) {
            return '<div>' +
                '<span id="' + escape(data.userGuid) + '" class="button name bg-anthilla-violet">' +
                escape(data.userFname) + ' ' + escape(data.userLname) + ' of ' + escape(data.companyAlias) +
                '</span>' +
                '</div>';
        }
    };
}();

var ContactSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
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
        },
        renderOptions: function (data, escape) {
            return '<div>' +
                '<span id="' + escape(data.guid) + '" class="button name bg-anthilla-violet">' +
                escape(data.fname) + ' ' + escape(data.lname) + ' - ' + escape(data.email) +
                '</span>' +
                '</div>';
        }
    };
}();

$('#select-user-contact').selectize({
    valueField: 'email',
    labelField: 'emailLabel',
    searchField: ['alias', 'email'],
    create: false,
    render: { option: ContactSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: ContactSelectizer.loadOptions
});

var AuthMailsSelectizer = function () {
    return {
        loadOptions: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: this.settings.remoteUrl,
                type: 'GET',
                dataType: 'json',
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
        },
        renderOptions: function (data, escape) {
            return '<div><span class="button name bg-anthilla-violet">' + escape(data.email) + '</span></div>';
        }
    };
}();

$('#select-auth-user-emails').selectize({
    maxItems: 1,
    valueField: 'email',
    labelField: 'email',
    searchField: ['email'],
    create: false,
    render: { option: AuthMailsSelectizer.renderOptions },
    remoteUrl: '/rawdata/user/auth',
    load: AuthMailsSelectizer.loadOptions
});