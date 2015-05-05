$('.upload-post-to').click(function (ev) {
    ev.preventDefault();
    var self = $(this);
    var context = self.attr('data-context');
    var dbContent = $('#DashboardContent');
    SetBorders(context);
    dbContent.show();
    ShowDashboard(context);
    $('#InfoCont').show();
    $('#Info').text(context);
    return false;
});

function ResetBorders() {
    var myfilesBtn = $('a[data-context="myfiles"]');
    myfilesBtn.removeClass('border-2-anthilla-green');
    myfilesBtn.removeClass('border-2-anthilla-gray');
    var sharingBtn = $('a[data-context="sharing"]');
    sharingBtn.removeClass('border-2-anthilla-green');
    sharingBtn.removeClass('border-2-anthilla-gray');
    var archiveBtn = $('a[data-context="archive"]');
    archiveBtn.removeClass('border-2-anthilla-green');
    archiveBtn.removeClass('border-2-anthilla-gray');
}

function SetBorders(context) {
    var myfilesBtn = $('a[data-context="myfiles"]');
    var sharingBtn = $('a[data-context="sharing"]');
    var archiveBtn = $('a[data-context="archive"]');
    ResetBorders();
    if (context == "myfiles") {
        myfilesBtn.addClass('border-2-anthilla-green');
        sharingBtn.addClass('border-2-anthilla-gray');
        archiveBtn.addClass('border-2-anthilla-gray');
    }
    if (context == "sharing") {
        myfilesBtn.addClass('border-2-anthilla-gray');
        sharingBtn.addClass('border-2-anthilla-green');
        archiveBtn.addClass('border-2-anthilla-gray');
    }
    if (context == "archive") {
        myfilesBtn.addClass('border-2-anthilla-gray');
        sharingBtn.addClass('border-2-anthilla-gray');
        archiveBtn.addClass('border-2-anthilla-green');
    }
}

var $projectRow = $('#ProjectRow');
var $userRow = $('#UserRow');
var $packageRow = $('#PackageRow');
var $tagsRow = $('#TagsRow');
var $descriptionRow = $('#DescriptionRow');

function ShowAll() {
    $projectRow.fadeTo("fast", 1);
    $userRow.fadeTo("fast", 1);
    $packageRow.fadeTo("fast", 1);
    $tagsRow.fadeTo("fast", 1);
    $descriptionRow.fadeTo("fast", 1);
}

function ShowForMyfiles() {
    ShowAll();
    $projectRow.fadeTo("fast", 0.33);
    $userRow.fadeTo("fast", 0.33);
    $packageRow.fadeTo("fast", 0.33);
}

function ShowForSharing() {
    ShowAll();
}

function ShowForArchive() {
    ShowAll();
    $userRow.fadeTo("fast", 0.33);
}

function ClearAllInputs() {
    $('input:text').val('');
    projectControl.clear();
    userControl.clear();
}

function ShowDashboard(dashboard) {
    if (dashboard == "myfiles") {
        ClearAllInputs();
        $('#Context').val('/vfs/myfiles');
        $('#ContextType').val('myfiles');
        ShowForMyfiles();
        DisableAll();
    }
    if (dashboard == "sharing") {
        ClearAllInputs();
        $('#Context').val('/vfs/sharing');
        $('#ContextType').val('sharing');
        ShowForSharing();
        EnableAll();
    }
    if (dashboard == "archive") {
        ClearAllInputs();
        $('#Context').val('/vfs/archive');
        $('#ContextType').val('archive');
        ShowForArchive();
        EnableAll();
        DisableUserInput();
    }
}

function EnableUserInput(userUrl) {
    $selectUser = $('#vfs-select-user').selectize({
        valueField: 'userGuid',
        labelField: 'userNameLabel',
        searchField: ['userFname', 'userLname', 'userAlias', 'companyAlias'],
        create: false,
        render: { option: UserJoinSelectizer.renderOptions },
        remoteUrl: userUrl,
        load: UserJoinSelectizer.loadOptions,
    });
    userControl = $selectUser[0].selectize;
}

var $selectUser = $('#vfs-select-user').selectize();

var $selectProject = $('#vfs-select-project').selectize({
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
        userControl.destroy();
        EnableUserInput(userUrl);
    }
});

var projectControl = $selectProject[0].selectize;

var userControl = $selectUser[0].selectize;

function EnableAll() {
    projectControl.enable();
    userControl.enable();
}

function DisableAll() {
    projectControl.disable();
    userControl.disable();
}

function DisableUserInput() {
    userControl.disable();
}