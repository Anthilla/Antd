$(document).ready(function () {
    console.log('anthilla.selectize.js load');
    $.when(
        LoadTag(),
        LoadEventType(),
        LoadUserMono(),
        LoadUserMulti(),
        LoadCompanyMono(),
        LoadCompanyMulti(),
        LoadCompanyDefault(),
        LoadUserGroupMulti(),
        LoadUserGroupMono(),
        LoadFunctionGroupMono(),
        LoadFunctions(),
        LoadProjectMulti(),
        LoadProjectMono(),
        LoadLicense(),
        LoadSchema(),
        LoadResources(),
        LoadProjectForVfs(),
        LoadContacts(),
        LoadAuthEmails()
    ).then(
        console.log('loaded')
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

var $tagSelectizer = $('.tag-input').selectize({
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

function LoadTag() {
    if ($('.tag-input').size() > 0) {
        var tagSelectizer = $tagSelectizer[0].selectize;
        tagSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $eventTypeSelectizer = $('.event-type').selectize({
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

function LoadEventType() {
    if ($('.event-type').size() > 0) {
        var eventTypeSelectizer = $eventTypeSelectizer[0].selectize;
        eventTypeSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $userMonoSelectizer = $('#select-user').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'nameLabel',
    searchField: ['fname', 'lname', 'alias'],
    create: false,
    render: { option: UserSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: UserSelectizer.loadOptions
});

function LoadUserMono() {
    if ($('#select-user').size() > 0) {
        var userMonoSelectizer = $userMonoSelectizer[0].selectize;
        userMonoSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $userMultiSelectizer = $('#select-user-multi').selectize({
    valueField: 'guid',
    labelField: 'nameLabel',
    searchField: ['fname', 'lname', 'alias'],
    create: false,
    render: { option: UserSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: UserSelectizer.loadOptions
});

function LoadUserMulti() {
    if ($('#select-user-multi').size() > 0) {
        var userMultiSelectizer = $userMultiSelectizer[0].selectize;
        userMultiSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $companyMonoSelectizer = $('#select-company').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company',
    load: ItemSelectizer.loadOptions
});

function LoadCompanyMono() {
    if ($('#select-company').size() > 0) {
        var companyMonoSelectizer = $companyMonoSelectizer[0].selectize;
        companyMonoSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $companyMultiSelectizer = $('#select-company-multi').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company',
    load: ItemSelectizer.loadOptions
});

function LoadCompanyMulti() {
    if ($('#select-company-multi').size() > 0) {
        var companyMultiSelectizer = $companyMultiSelectizer[0].selectize;
        companyMultiSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $companyDefaultSelectizer = $('#select-company-default').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/company/default',
    load: ItemSelectizer.loadOptions
});

function LoadCompanyDefault() {
    if ($('#select-company-default').size() > 0) {
        var companyDefaultSelectizer = $companyDefaultSelectizer[0].selectize;
        companyDefaultSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $userGroupMultiSelectizer = $('#select-group').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/ugroup',
    load: ItemSelectizer.loadOptions
});

function LoadUserGroupMulti() {
    if ($('#select-group').size() > 0) {
        var userGroupMultiSelectizer = $userGroupMultiSelectizer[0].selectize;
        userGroupMultiSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $userGroupMonoSelectizer = $('#select-group-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/ugroup',
    load: ItemSelectizer.loadOptions
});

function LoadUserGroupMono() {
    if ($('#select-group-mono').size() > 0) {
        var userGroupMonoSelectizer = $userGroupMonoSelectizer[0].selectize;
        userGroupMonoSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $funcGroupMonoSelectizer = $('#select-funcgrp-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/funcgrp',
    load: ItemSelectizer.loadOptions
});

function LoadFunctionGroupMono() {
    if ($('#select-funcgrp-mono').size() > 0) {
        var funcGroupMonoSelectizer = $funcGroupMonoSelectizer[0].selectize;
        funcGroupMonoSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $functionsSelectizer = $('#select-funcs').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/func',
    load: ItemSelectizer.loadOptions
});

function LoadFunctions() {
    if ($('#select-funcs').size() > 0) {
        var functionsSelectizer = $functionsSelectizer[0].selectize;
        functionsSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $projectMultiSelectizer = $('#select-project').selectize({
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/project',
    load: ItemSelectizer.loadOptions
});

function LoadProjectMulti() {
    if ($('#select-project').size() > 0) {
        var projectMultiSelectizer = $projectMultiSelectizer[0].selectize;
        projectMultiSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $projectMonoSelectizer = $('#select-project-mono').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/project',
    load: ItemSelectizer.loadOptions
});

function LoadProjectMono() {
    if ($('#select-project-mono').size() > 0) {
        var projectMonoSelectizer = $projectMonoSelectizer[0].selectize;
        projectMonoSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $licenseSelectizer = $('#select-license').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/license',
    load: ItemSelectizer.loadOptions
});

function LoadLicense() {
    if ($('#select-license').size() > 0) {
        var licenseSelectizer = $licenseSelectizer[0].selectize;
        licenseSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $schemaSelectizer = $('#select-schema').selectize({
    maxItems: 1,
    valueField: 'guid',
    labelField: 'alias',
    searchField: 'alias',
    create: false,
    render: { option: ItemSelectizer.renderOptions },
    remoteUrl: '/rawdata/schemas',
    load: ItemSelectizer.loadOptions
});

function LoadSchema() {
    if ($('#select-schema').size() > 0) {
        var schemaSelectizer = $schemaSelectizer[0].selectize;
        schemaSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $resourceSelectizer = $('#select-resources').selectize({
    valueField: 'guid',
    labelField: 'data',
    searchField: ['data'],
    create: false,
    render: { option: ResourceSelectizer.renderOptions },
    remoteUrl: '/rawdata/resources',
    load: ResourceSelectizer.loadOptions
});

function LoadResources() {
    if ($('#select-resources').size() > 0) {
        var resourceSelectizer = $resourceSelectizer[0].selectize;
        resourceSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

var $projectFsSelectier = $('#select-project-mono-fs').selectize({
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

function LoadProjectForVfs() {
    if ($('#select-project-mono-fs').size() > 0) {
        var projectFsSelectier = $projectFsSelectier[0].selectize;
        projectFsSelectier.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

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

var $contactSelectizer = $('#select-user-contact').selectize({
    valueField: 'email',
    labelField: 'emailLabel',
    searchField: ['alias', 'email'],
    create: false,
    render: { option: ContactSelectizer.renderOptions },
    remoteUrl: '/rawdata/user',
    load: ContactSelectizer.loadOptions
});

function LoadContacts() {
    if ($('#select-user-contact').size() > 0) {
        var contactSelectizer = $contactSelectizer[0].selectize;
        contactSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}

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

var $authUserEmailSelectizer = $('#select-auth-user-emails').selectize({
    maxItems: 1,
    valueField: 'email',
    labelField: 'email',
    searchField: ['email'],
    create: false,
    render: { option: AuthMailsSelectizer.renderOptions },
    remoteUrl: '/rawdata/user/auth',
    load: AuthMailsSelectizer.loadOptions
});

function LoadAuthEmails() {
    if ($('#select-auth-user-emails').size() > 0) {
        var authUserEmailSelectizer = $authUserEmailSelectizer[0].selectize;
        authUserEmailSelectizer.load(function (callback) {
            Callback(callback, this.settings.remoteUrl);
        });
    }
}