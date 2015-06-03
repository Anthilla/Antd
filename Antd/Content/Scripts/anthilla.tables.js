$(document).ready(function () {
    console.log('anthilla.tables.js load');
});

//Input Search
$('.search-field').keyup(function () {
    var queryString = $(this).val();
    var table = $('.searchable');
    var tableBody = table.children('tbody');
    var row = tableBody.children('tr');
    //todo controllare se ci sono tr già nascoste
    //todo feedback se non ci sono risultati
    row.each(function (index) {
        var thisRow = $(this);
        var queriedText = $(this).text();
        if (queriedText.indexOf(queryString) != -1) {
            thisRow.show();
        }
        if (queriedText.indexOf(queryString) < 0) {
            thisRow.hide();
        }
    });
});

$('.search-field-task').keyup(function () {
    var queryString = $(this).val();
    console.log(queryString);
    var row = $('.searchable-task');
    var field = row.find('searchable-task-alias');
    //todo controllare se ci sono tr già nascoste
    //todo feedback se non ci sono risultati
    row.each(function (index) {
        var thisRow = $(this);
        var queriedText = thisRow.find('searchable-task-alias').text();
        if (queriedText.indexOf(queryString) != -1) {
            thisRow.show();
        }
        if (queriedText.indexOf(queryString) < 0) {
            thisRow.hide();
        }
    });
});

//Table Management
function CheckEdit(guid) {
    var mainRow = $('tr[id="' + guid + '"]');
    var value = mainRow.attr('data-edit');
    return value;
}

function EnableEdit(guid) {
    var mainRow = $('tr[id="' + guid + '"]');
    mainRow.attr('data-edit', 'enabled')
}

function DisableEdit(guid) {
    var mainRow = $('tr[id="' + guid + '"]');
    mainRow.attr('data-edit', 'disabled')
}

function LockRowsExpansion(thisLink) {
    var lock = thisLink.find('.chk-lock')
    lock.toggleClass('icon-unlocked');
    lock.toggleClass('icon-locked');
    return false;
}

function ExpandAllTable() {
    var self = $(this);
    var rows = $('.cell');
    self.children('i').toggleClass('icon-arrow-up-4');
    self.children('i').toggleClass('icon-arrow-down-4');
    self.attr('onclick', 'CollapseAllTable()');
    rows.toggle();
    return false;
}

//Company Table Management
$('.CompanyTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadCompanyToEdit(guid);
    }
    return false;
});

$('.CompanyTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandCompanyRows(self);
});

$('.CompanyTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function LoadCompanyToEdit(guid) {
    DisableEdit(guid);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var emailRow = $('tr[row="' + guid + 'row2"]');
    var bankRow = $('tr[row="' + guid + 'row3"]');
    var addressRow = $('tr[row="' + guid + 'row4"]');
    var tagRow = $('tr[row="' + guid + 'row5"]');
    var miscRow = $('tr[row="' + guid + 'row6"]');

    var aliasCell = aliasRow.children('.value');
    var emailCell = emailRow.children('.value');
    var bankCell = bankRow.children('.value');
    var addressCell = addressRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var input = $(this).children('input');
    var val = input.val();
    var lock = $(document).find('.icon-unlocked');
    var isLocked = lock.attr('lock');

    if (isLocked != '1') {
        var alertMsg = '<i class="icon-warning fg-red on-left"></i> Inline edit is locked!';
        miscCell.html(alertMsg);
    }
    if (val == null && isLocked == '1') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/company/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (company) {
                var companyGuid;
                var companyAlias;
                var companyEmail;
                var companyBank;
                var companyStreetName;
                var companyStreetNumber;
                var companyPostalCode;
                var companyCity;
                var companyCountry;
                var companyTags;

                companyGuid = company.AnthillaGuid;
                companyAlias = company.AnthillaAlias;
                companyEmail = company.AnthillaEmail;
                companyBank = company.AnthillaBankCoord;
                companyStreetName = company.AnthillaStreetName;
                if (company.AnthillaStreetName == null)
                { companyStreetName = ""; }
                companyStreetNumber = company.AnthillaStreetNumber;
                if (company.AnthillaStreetNumber == null)
                { companyStreetNumber = ""; }
                companyPostalCode = company.AnthillaPostalCode;
                if (company.AnthillaPostalCode == null)
                { companyPostalCode = ""; }
                companyCity = company.AnthillaCity;
                if (company.AnthillaCity == null)
                { companyCity = ""; }
                companyCountry = company.AnthillaCountry;
                if (company.AnthillaCountry == null)
                { companyCountry = ""; }
                companyTags = company.AnthillaTags;

                var aliasHtml =
                '<input id="' + companyGuid + 'AliasEdit" style="height: 30px; width: 90%;" type="text" value="' + companyAlias + '" />';

                var emailHtml =
'<input id="' + companyGuid + 'EmailEdit" style="height: 30px; width: 90%;" type="text" value="' + companyEmail + '" />';

                var bankHtml =
'<input id="' + companyGuid + 'BankEdit" style="height: 30px; width: 90%;" type="text" value="' + companyBank + '" />';

                var addressHtml =
                '<input id="' + companyGuid + 'StreetEdit" style="height: 30px; width: 35%;" type="text" value="' + companyStreetName + '" />' +
                '<input id="' + companyGuid + 'NumbEdit" style="height: 30px; width: 5%;" type="text" value="' + companyStreetNumber + '" />' +
                '<input id="' + companyGuid + 'PcodEdit" style="height: 30px; width: 10%;" type="text" value="' + companyPostalCode + '" />' +
                '<input id="' + companyGuid + 'CityEdit" style="height: 30px; width: 35%;" type="text" value="' + companyCity + '" />' +
                '<input id="' + companyGuid + 'CountryEdit" style="height: 30px; width: 5%;" type="text" value="' + companyCountry + '" />';

                var tagHtml =
                '<input id="' + companyGuid + 'TagsEdit" type="text" value="' + companyTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditCompany(\'' + companyGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditCompany(\'' + companyGuid + '\')" ignore="yes" class="ignore bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteCompany(\'' + companyGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                aliasCell.html(aliasHtml);
                emailCell.html(emailHtml);
                bankCell.html(bankHtml);
                addressCell.html(addressHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
            }
        });
    }
    return false;
}

function EditCompany(guid) {
    jQuery.support.cors = true;
    var company = {
        Alias: $('#' + guid + 'AliasEdit').val(),
        Tags: $('#' + guid + 'TagsEdit').val(),
    };
    var info = {
        Email: $('#' + guid + 'EmailEdit').val(),
        Bank: $('#' + guid + 'BankEdit').val(),
    };
    var address = {
        Street: $('#' + guid + 'StreetEdit').val(),
        Numb: $('#' + guid + 'NumbEdit').val(),
        Pcod: $('#' + guid + 'PcodEdit').val(),
        City: $('#' + guid + 'CityEdit').val(),
        Country: $('#' + guid + 'CountryEdit').val(),
    };
    $.ajax({
        url: '/company/' + guid + '/' + company.Alias + '/' + company.Tags + '/edit',
        type: 'POST',
        data: JSON.stringify(company),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            $.ajax({
                url: '/company/address/' + guid + '/' + address.Street + '/' + address.Numb + '/' + address.Pcod + '/' + address.City + '/' + address.Country + '/edit',
                type: 'POST',
                data: JSON.stringify(address),
                contentType: "application/json;charset=utf-8",
                success: function () {
                    if (address.Street != '') {
                        $.ajax({
                            url: '/company/address/' + guid + '/' + address.Street + '/' + address.Numb + '/' + address.Pcod + '/' + address.City + '/' + address.Country + '/edit',
                            type: 'POST',
                            data: JSON.stringify(address),
                            contentType: "application/json;charset=utf-8",
                            success: function () {
                                return false;
                            }
                        });
                    }
                    RestoreCompanyStaticRow(guid);
                    return false;
                }
            });
            return false;
        }
    });
    return false;
}

function CancelEditCompany(guid) {
    if (guid != '') {
        RestoreCompanyStaticRow(guid);
        return false;
    }
}

function RestoreCompanyStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/company/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (company) {
            RestoreCompanyHtml(company);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreCompanyHtml(company) {
    var guid = company.AnthillaGuid;
    var companyName = company.AnthillaAlias;
    var companyEmail = company.AnthillaEmail;
    var companyBank = company.AnthillaBankCoord;
    var address = company.AnthillaStreetName + " " + company.AnthillaStreetNumber + ", " +
        company.AnthillaPostalCode + " " + company.AnthillaCity + " (" + company.AnthillaCountry + ")";
    var companyTags = company.AnthillaTags;
    var tags = "";
    $.each(companyTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    var parentRow = $('tr[id="' + guid + '"]');
    var newParentRow =
        "<td class='right'>" + companyName + "</td> \
                <td class='right'>" + address + "</td> \
                <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentRow.html(newParentRow);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var newAliasRow =
        "<td class='right'><label>Company</label></td> \
                <td class='right value'>" + companyName + "</td> \
                <td>&nbsp;</td>";
    aliasRow.html(newAliasRow);

    var emailRow = $('tr[row="' + guid + 'row2"]');
    var newEmailRow =
        "<td class='right'><label>Email</label></td> \
                <td class='right value'>" + companyEmail + "</td> \
                <td>&nbsp;</td>";
    emailRow.html(newEmailRow);

    var bankRow = $('tr[row="' + guid + 'row3"]');
    var newBankRow =
        "<td class='right'><label>Bank</label></td> \
                <td class='right value'>" + companyBank + "</td> \
                <td>&nbsp;</td>";
    bankRow.html(newBankRow);

    var addressRow = $('tr[row="' + guid + 'row4"]');
    var newAddressRow =
        "<td class='right'><label>Address</label></td> \
                <td class='right value'>" + address + "</td> \
                <td>&nbsp;</td>";
    addressRow.html(newAddressRow);

    var tagRow = $('tr[row="' + guid + 'row5"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row6"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteCompany(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/company/' + guid + '/delete/',
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[guid="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandCompanyRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    var nextRow_IV = nextRow_III.next();
    var nextRow_V = nextRow_IV.next();
    var nextRow_VI = nextRow_V.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    nextRow_IV.toggle();
    nextRow_V.toggle();
    nextRow_VI.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

//Project Table Management
$('.ProjectTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadProjectToEdit(guid);
    }
    return false;
});

$('.ProjectTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandProjectRows(self);
});

$('.ProjectTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function LoadProjectToEdit(guid) {
    DisableEdit(guid);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var leaderRow = $('tr[row="' + guid + 'row2"]');
    var tagRow = $('tr[row="' + guid + 'row3"]');
    var licenseRow = $('tr[row="' + guid + 'row4"]');
    var miscRow = $('tr[row="' + guid + 'row5"]');

    var aliasCell = aliasRow.children('.value');
    var leaderCell = leaderRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var input = $(this).children('input');
    var val = input.val();

    if (val == null) {
        jQuery.support.cors = true;
        $.ajax({
            url: '/project/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (project) {
                var projectGuid;
                var projectAlias;
                var projectLeader;
                var projectTags;

                projectGuid = project.AnthillaGuid;
                projectAlias = project.AnthillaAlias;
                projectLeader = project.AnthillaLeader;
                if (project.AnthillaLeader == null)
                { projectLeader = ""; }
                projectTags = project.AnthillaTags;

                var aliasHtml =
                '<input id="' + projectGuid + 'AliasEdit" style="height: 30px; width: 90%;" type="text" value="' + projectAlias + '" />';

                var orLeaderName = $('#' + guid).children('.leader').text();
                var leaderHtml =
                '<input id="' + projectGuid + 'LeaderEdit" style="height: 30px; width: 90%; " type="text" value="' + orLeaderName + '" />';

                var tagHtml =
                '<input id="' + projectGuid + 'TagsEdit" type="text" value="' + projectTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditProject(\'' + projectGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditProject(\'' + projectGuid + '\')" class="bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteProject(\'' + projectGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                aliasCell.html(aliasHtml);
                leaderCell.html(leaderHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
                SelectizeLeader(guid);
            }
        });
    }
    return false;
}

function EditProject(guid) {
    jQuery.support.cors = true;
    var project = {
        Alias: $('#' + guid + 'AliasEdit').val(),
        Tags: $('#' + guid + 'TagsEdit').val(),
    };
    var leader = {
        Guid: $('#' + guid + 'LeaderEdit').val()
    };
    $.ajax({
        url: '/project/' + guid + '/' + project.Alias + '/' + project.Tags + '/edit',
        type: 'POST',
        data: JSON.stringify(project),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            $.ajax({
                url: '/project/leader/' + guid + '/' + leader.Guid + '/e',
                type: 'POST',
                data: JSON.stringify(leader),
                contentType: "application/json;charset=utf-8",
                success: function (data) {
                    return false;
                }
            });
            RestoreProjectStaticRow(guid);
            return false;
        }
    });
    return false;
}

function CancelEditProject(guid) {
    if (guid != '') {
        RestoreProjectStaticRow(guid);
        return false;
    }
}

function RestoreProjectStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/project/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (project) {
            RestoreProjectHtml(project);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreProjectHtml(project) {
    var guid = project.AnthillaGuid;
    var projectName = project.AnthillaAlias + ' - ' + project.AnthillaProjectCode;
    var leader = project.AnthillaLeader;

    var projectTags = project.AnthillaTags;
    var tags = "";
    $.each(projectTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    var parentRow = $('tr[id="' + guid + '"]');
    var newParentRow =
        "<td class='right'>" + projectName + "</td> \
        <td class='right'>" + leader + "</td> \
        <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentRow.html(newParentRow);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var newAliasRow =
        "<td class='right'><label>Project</label></td> \
        <td class='right value'>" + projectName + "</td> \
        <td>&nbsp;</td>";
    aliasRow.html(newAliasRow);

    var leaderRow = $('tr[row="' + guid + 'row2"]');
    var newLeaderRow =
        "<td class='right'><label>Project Leader</label></td> \
        <td class='right value'> \
        <a class='bg-anthilla-blu fg-anthilla-gray' style='padding: 3px 10px;'> \
        <i class='icon-cone on-left fg-anthilla-gray'></i>" + leader + "</a></td> \
        <td>&nbsp;</td>";
    leaderRow.html(newLeaderRow);

    var tagRow = $('tr[row="' + guid + 'row3"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row4"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteProject(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/project/' + guid + '/delete/',
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[guid="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandProjectRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    var nextRow_IV = nextRow_III.next();
    var nextRow_V = nextRow_IV.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    nextRow_IV.toggle();
    nextRow_V.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

//User Table Management
$('.UserTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadUserToEdit(guid);
    }
    return false;
});

$('.UserTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandUserRows(self);
});

$('.UserTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function LoadUserToEdit(guid) {
    DisableEdit(guid);

    var usernameRow = $('tr[row="' + guid + 'row1"]');
    var emailRow = $('tr[row="' + guid + 'row2"]');
    var aliasRow = $('tr[row="' + guid + 'row3"]');
    var passwordRow = $('tr[row="' + guid + 'row4"]');
    var companyRow = $('tr[row="' + guid + 'row5"]');
    var projectRow = $('tr[row="' + guid + 'row6"]');
    var groupRow = $('tr[row="' + guid + 'row7"]');
    var tagRow = $('tr[row="' + guid + 'row8"]');
    var miscRow = $('tr[row="' + guid + 'row9"]');

    var usernameCell = usernameRow.children('.value');
    var emailCell = emailRow.children('.value');
    var aliasCell = aliasRow.children('.value');
    var passwordCell = passwordRow.children('.value');
    var companyCell = companyRow.children('.value');
    var projectCell = projectRow.children('.value');
    var groupCell = groupRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var companyText = companyCell.html().replace(/<a class="button bg-transparent">/g, '')
                                        .replace(/<i class="icon-earth on-left"><\/i> /g, '')
                                        .replace(/<\/a>/g, '')
                                        .replace(/&nbsp;/g, '')
                                        .replace(/\t/g, '')
                                        .replace(/\n/g, '');
    var companyText2 = $.trim(companyText);

    var projectText = projectCell.html().replace(/<a class="button bg-transparent">/g, '')
                                        .replace(/<i class="icon-cone on-left"><\/i> /g, '')
                                        .replace(/<\/a>/g, ',')
                                        .replace(/&nbsp;/g, '')
                                        .replace(/\t/g, '')
                                        .replace(/\n/g, '');
    var projectText2 = $.trim(projectText);

    var groupText = groupCell.html().replace(/<a class="button bg-transparent">/g, '')
                                    .replace(/<i class="icon-book on-left"><\/i> /g, '')
                                    .replace(/<\/a>/g, ',')
                                    .replace(/&nbsp;/g, '')
                                    .replace(/\t/g, '')
                                    .replace(/\n/g, '');
    var groupText2 = $.trim(groupText);

    var input = $(this).children('input');
    var val = input.val();

    if (val == null) {
        jQuery.support.cors = true;
        $.ajax({
            url: '/user/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (user) {
                var userGuid;
                var userFName;
                var userLName;
                var userEmail;
                var userAlias;
                var userPassword;
                var userCompany;
                var userProject;
                var userGroup;
                var userTags;

                userGuid = user.AnthillaGuid;
                userFName = user.AnthillaFirstName;
                userLName = user.AnthillaLastName;
                userEmail = user.AnthillaEmail;
                userAlias = user.AnthillaAlias;
                userPassword = user.AnthillaPassword;
                userCompany = user.AnthillaGuid;
                userProject = user.AnthillaGuid;
                userGroup = user.AnthillaGuid;
                userTags = user.AnthillaTags;

                var usernameHtml =
                '<input id="' + userGuid + 'fNameEdit" style="height: 30px; width: 45%;" type="text" value="' + userFName + '" />' +
                '<input id="' + userGuid + 'lNameEdit" style="height: 30px; width: 45%;" type="text" value="' + userLName + '" />';

                var emailHtml =
                '<input id="' + userGuid + 'EmailEdit" style="height: 30px; width: 90%;" type="text" value="' + userEmail + '" />';

                var aliasHtml =
                '<input id="' + userGuid + 'AliasEdit" style="height: 30px; width: 90%;" type="text" value="' + userAlias + '" readonly />';

                var passwordHtml =
'<input type="text" id="' + userGuid + 'NewPassword" style="display: none; height: 30px; width: 50.4%;">' +
'<a onclick="ConfirmPasswordChange(\'' + userGuid + '\')" id="' + userGuid + 'NewPasswordBtn" style="display: none; height: 30px; line-height: 15px;" class="button bg-anthilla-blu upcase"> change password</a>' +
'<a onclick="ResetPassword(\'' + userGuid + '\')" style="height: 30px; line-height: 15px;" class="button bg-anthilla-orange upcase"> reset password</a>';

                var compName0 = companyCell.text()
                var projName0 = projectCell.text()
                var grpName0 = groupCell.text()

                var companyHtml =
                '<input id="' + userGuid + 'CompanyEdit" style="height: 30px; width: 90%;" type="text" value="' + compName0 + '" />';

                var projectHtml =
                '<input id="' + userGuid + 'ProjectEdit" style="height: 30px; width: 90%;" type="text" value="' + projName0 + '" />';

                var groupHtml =
                '<input id="' + userGuid + 'UgroupEdit" style="height: 30px; width: 90%;" type="text" value="' + grpName0 + '" />';

                var tagHtml =
                '<input id="' + userGuid + 'TagsEdit" type="text" value="' + userTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditUser(\'' + userGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm Edit</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditUser(\'' + userGuid + '\')"  class="bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteUser(\'' + userGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                usernameCell.html(usernameHtml);
                emailCell.html(emailHtml);
                aliasCell.html(aliasHtml);
                passwordCell.html(passwordHtml);
                companyCell.html(companyHtml);
                projectCell.html(projectHtml);
                groupCell.html(groupHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
                SelectizeCompany(guid);
                SelectizeProjects(guid);
                SelectizeGroups(guid);
            }
        });
    }
    return false;
}

function EditUser(guid) {
    jQuery.support.cors = true;
    var element = {
        FNAME: $('#' + guid + 'fNameEdit').val(),
        LNAME: $('#' + guid + 'lNameEdit').val(),
        UserTag: $('#' + guid + 'TagsEdit').val(),
        UserEmail: $('#' + guid + 'EmailEdit').val(),
        Company: $('#' + guid + 'CompanyEdit').val(),
        Project: $('#' + guid + 'ProjectEdit').val(),
        Group: $('#' + guid + 'UgroupEdit').val(),
    };
    $.ajax({
        url: '/user/' + guid + '/' + element.FNAME + '/' + element.LNAME + '/reset/' + element.UserEmail + '/' + element.UserTag + '/edit',
        type: 'POST',
        data: JSON.stringify(element),
        success: function (data) {
            if (element.Company != null)
            { AssignCompanyToUser(guid, element.Company); }
            if (element.Company != null)
            { AssignProjectToUser(guid, element.Project); }
            if (element.Company != null)
            { AssignGroupToUser(guid, element.Group); }
            return false;
        }
    });
    return false;
}

function AssignCompanyToUser(userGuid, companyGuid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/user/assign/company/' + userGuid + '/' + companyGuid + '/edit',
        type: 'POST',
        data: JSON.stringify(element),
        success: function (data) {
            return false;
        }
    });
}

function AssignProjectToUser(userGuid, projectGuid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/user/assign/project/' + userGuid + '/' + projectGuid + '/edit',
        type: 'POST',
        data: JSON.stringify(element),
        success: function (data) {
            return false;
        }
    });
}

function AssignGroupToUser(userGuid, groupGuid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/user/assign/group/' + userGuid + '/' + groupGuid + '/edit',
        type: 'POST',
        data: JSON.stringify(element),
        success: function (data) {
            return false;
        }
    });
}

function CancelEditUser(guid) {
    if (guid != '') {
        RestoreUserStaticRow(guid);
        return false;
    }
}

function RestoreUserStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/user/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (user) {
            RestoreUserHtml(user);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreUserHtml(user) {
    var guid = user.AnthillaGuid;
    var userName = user.AnthillaFirstName + " " + user.AnthillaLastName;
    var email = user.AnthillaEmail;
    var alias = user.AnthillaAlias;
    var pwd = user.AnthillaPassword;
    var company = user.AnthillaCompanyId;
    var userprojects = user.AnthillaProjectIds;
    var projects = "";
    $.each(userprojects, function (index, value) {
        projects += "<a class='button bg-transparent bg-active-dark'><i class='icon-cone on-left'></i>" + value + "</a>"
    });
    var usergroups = user.AnthillaUserGroupIds;
    var groups = "";
    $.each(usergroups, function (index, value) {
        groups += "<a class='button bg-transparent bg-active-dark'><i class='icon-book on-left'></i>" + value + "</a>"
    });
    var userTags = user.AnthillaTags;
    var tags = "";
    $.each(userTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    var parentrow = $('tr[id="' + guid + '"]');
    var newparentrow =
        "<td class='right'>" + userName + "</td> \
        <td class='right'>" + email + "</td> \
        <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentrow.html(newparentrow);

    var usernamerow = $('tr[row="' + guid + 'row1"]');
    var newusernamerow =
        "<td class='right'><label>User</label></td> \
        <td class='right value'>" + userName + "</td> \
        <td>&nbsp;</td>";
    usernamerow.html(newusernamerow);

    var emailrow = $('tr[row="' + guid + 'row2"]');
    var newemailrowrow =
        "<td class='right'><label>Email</label></td> \
        <td class='right value'>" + email + "</td> \
        <td>&nbsp;</td>";
    emailrow.html(newemailrowrow);

    var aliasrow = $('tr[row="' + guid + 'row3"]');
    var newaliasrow =
        "<td class='right'><label>Alias</label></td> \
        <td class='right value'>" + alias + "</td> \
        <td>&nbsp;</td>";
    aliasrow.html(newaliasrow);

    var passwordrow = $('tr[row="' + guid + 'row4"]');
    var newpasswordrow =
        "<td class='right'><label>Password</label></td> \
        <td class='right value'>" + pwd + "</td> \
        <td>&nbsp;</td>";
    passwordrow.html(newpasswordrow);

    var companyrow = $('tr[row="' + guid + 'row5"]');
    var newcompanyrow =
    "<td class='right'><label>Company</label></td> \
        <td class='right value'> \
        <a class='bg-anthilla-blu fg-anthilla-gray' style='padding: 3px 10px;'> \
        <i class='icon-earth on-left fg-anthilla-gray'></i> " + company + "</a></td> \
        <td>&nbsp;</td>";
    companyrow.html(newcompanyrow);

    var projectrow = $('tr[row="' + guid + 'row6"]');
    var newprojectrow =
    "<td class='right'><label>Projects</label></td> \
        <td class='right value'>" + projects + "</td> \
        <td>&nbsp;</td>";
    projectrow.html(newprojectrow);

    var grouprow = $('tr[row="' + guid + 'row7"]');
    var newgrouprow =
    "<td class='right'><label>Groups</label></td> \
        <td class='right value'>" + groups + "</td> \
        <td>&nbsp;</td>";
    grouprow.html(newgrouprow);

    var tagRow = $('tr[row="' + guid + 'row8"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row9"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteUser(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/user/' + guid + '/delete/',
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[guid="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandUserRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    var nextRow_IV = nextRow_III.next();
    var nextRow_V = nextRow_IV.next();
    var nextRow_VI = nextRow_V.next();
    var nextRow_VII = nextRow_VI.next();
    var nextRow_VIII = nextRow_VII.next();
    var nextRow_IX = nextRow_VIII.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    nextRow_IV.toggle();
    nextRow_V.toggle();
    nextRow_VI.toggle();
    nextRow_VII.toggle();
    nextRow_VIII.toggle();
    nextRow_IX.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

function ResetPassword(guid) {
    var element = {
        input: $('#' + guid + 'NewPassword'),
        button: $('#' + guid + 'NewPasswordBtn')
    };
    element.input.show();
    element.button.show();
    return false;
}

function ConfirmPasswordChange(guid) {
    var element = {
        newp: $('#' + guid + 'NewPassword').val()
    };
    jQuery.support.cors = true;
    $.ajax({
        url: '/user/reset/password/' + guid + '/' + newp + '/edit',
        type: 'POST',
        data: JSON.stringify(element),
        success: function (data) {
            return false;
        },
        error: function (x, y, z, jqXHR, textStatus, errorThrown) {
            alert(x + '\n' + y + '\n' + z + textStatus + " - " + errorThrown);
        }
    });
}

//MyFiles Table Management
$('.MyFilesTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadMyFilesToEdit(guid);
    }
    return false;
});

$('.MyFilesTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandMyFilesRows(self);
});

$('.MyFilesTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function LoadMyFilesToEdit(guid) {
    DisableEdit(guid);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var tagRow = $('tr[row="' + guid + 'row5"]');
    var miscRow = $('tr[row="' + guid + 'row6"]');

    var aliasCell = aliasRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var input = $(this).children('input');
    var val = input.val();
    var lock = $(document).find('.icon-unlocked');
    var isLocked = lock.attr('lock');

    if (isLocked != '1') {
        var alertMsg = '<i class="icon-warning fg-red on-left"></i> Inline edit is locked!';
        miscCell.html(alertMsg);
    }
    if (val == null && isLocked == '1') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/vfs/getfile/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (file) {
                var fileGuid;
                var fileAlias;
                var fileTags;

                fileGuid = file.AnthillaGuid;
                fileAlias = file.AnthillaOriginalAlias;
                fileTags = file.AnthillaTags;

                var prefix = $('#FileNamingPrefix').val();
                var suffix = $('#FileNamingSuffix').val();

                var aliasHtml =
                    prefix +
                    '<input id="' + fileGuid + 'AliasEdit" style="height: 30px; width: 80%;" type="text" value="' + fileAlias + '" />' +
                    suffix;

                var tagHtml =
                '<input id="' + fileGuid + 'TagsEdit" type="text" value="' + fileTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditMyFiles(\'' + fileGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditMyFiles(\'' + fileGuid + '\')" ignore="yes" class="ignore bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteMyFiles(\'' + fileGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                aliasCell.html(aliasHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
            }
        });
    }
    return false;
}

function EditMyFiles(guid) {
    jQuery.support.cors = true;
    var myfiles = {
        Alias: $('#' + guid + 'AliasEdit').val(),
        Tags: $('#' + guid + 'TagsEdit').val(),
    };
    $.ajax({
        url: '/vfs/edit/fileinfo/' + guid + '/' + myfiles.Alias + '/' + myfiles.Tags + '/edit',
        type: 'POST',
        data: JSON.stringify(myfiles),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            RestoreMyFilesStaticRow(guid);
            return false;
        }
    });
    return false;
}

function CancelEditMyFiles(guid) {
    if (guid != '') {
        RestoreMyFilesStaticRow(guid);
        return false;
    }
}

function RestoreMyFilesStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/myfiles/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (myfiles) {
            RestoreMyFilesHtml(myfiles);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreMyFilesHtml(myfiles) {
    var guid = myfiles.AnthillaGuid;
    var myfilesName = myfiles.AnthillaOriginalAlias;
    var myfilesTags = myfiles.AnthillaTags;
    var tags = "";
    $.each(myfilesTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    var parentRow = $('tr[id="' + guid + '"]');
    var newParentRow =
        "<td class='right'>" + myfilesName + "</td> \
                <td class='right'>" + tags + "</td> \
                <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentRow.html(newParentRow);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var newAliasRow =
        "<td class='right'><label>File Name</label></td> \
                <td class='right value'>" + myfilesName + "</td> \
                <td>&nbsp;</td>";
    aliasRow.html(newAliasRow);

    var typeRow = $('tr[row="' + guid + 'row2"]');
    var newTypeRow =
        "<td class='right'><label>File Type</label></td> \
                <td class='right value'>" + myfiles.AnthillaExtension + "</td> \
                <td>&nbsp;</td>";
    typeRow.html(newTypeRow);

    var typeRow = $('tr[row="' + guid + 'row3"]');
    var newTypeRow =
        "<td class='right'><label>Dimension</label></td> \
                <td class='right value'>" + myfiles.AnthillaDimension + "</td> \
                <td>&nbsp;</td>";
    typeRow.html(newTypeRow);

    var typeRow = $('tr[row="' + guid + 'row4"]');
    var newTypeRow =
        "<td class='right'><label>Last Modified</label></td> \
                <td class='right value'>" + myfiles.AnthillaLastModified + "</td> \
                <td>&nbsp;</td>";
    typeRow.html(newTypeRow);

    var tagRow = $('tr[row="' + guid + 'row5"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row6"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteMyFiles(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/vfs/delete/' + guid,
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[guid="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandMyFilesRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    var nextRow_IV = nextRow_III.next();
    var nextRow_V = nextRow_IV.next();
    var nextRow_VI = nextRow_V.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    nextRow_IV.toggle();
    nextRow_V.toggle();
    nextRow_VI.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

//Log Table Management
$('.LogTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandLogRows(self);
});

function ExpandLogRows(thisRow) {
    var nextRow = thisRow.next();
    var nextRowK = nextRow.next();
    var nextRowKK = nextRowK.next();
    var nextRowKKK = nextRowKK.next();
    nextRow.toggle();
    nextRowK.toggle();
    nextRowKK.toggle();
    nextRowKKK.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-2');
    arrow.toggleClass('icon-arrow-down-2');
    return false;
}

//UserGroup Table Management
$('.UserGroupTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadUserGroupToEdit(guid);
    }
    return false;
});

$('.UserGroupTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandUserGroupRows(self);
});

$('.UserGroupTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function LoadUserGroupToEdit(guid) {
    DisableEdit(guid);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var tagRow = $('tr[row="' + guid + 'row2"]');
    var miscRow = $('tr[row="' + guid + 'row3"]');

    var aliasCell = aliasRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var input = $(this).children('input');
    var val = input.val();
    var lock = $(document).find('.icon-unlocked');
    var isLocked = lock.attr('lock');

    if (isLocked != '1') {
        var alertMsg = '<i class="icon-warning fg-red on-left"></i> Inline edit is locked!';
        miscCell.html(alertMsg);
    }
    if (val == null && isLocked == '1') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/usergroup/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (uGroup) {
                var uGroupGuid;
                var uGroupAlias;
                var uGroupTags;

                uGroupGuid = uGroup.AnthillaGuid;
                uGroupAlias = uGroup.AnthillaAlias;
                uGroupTags = uGroup.AnthillaTags;

                var aliasHtml =
                '<input id="' + uGroupGuid + 'AliasEdit" style="height: 30px; width: 90%;" type="text" value="' + uGroupAlias + '" />';

                var tagHtml =
                '<input id="' + uGroupGuid + 'TagsEdit" type="text" value="' + uGroupTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditUserGroup(\'' + uGroupGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditUserGroup(\'' + uGroupGuid + '\')" ignore="yes" class="ignore bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteUserGroup(\'' + uGroupGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                aliasCell.html(aliasHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
            }
        });
    }
    return false;
}

function EditUserGroup(guid) {
    jQuery.support.cors = true;
    var uGroup = {
        Alias: $('#' + guid + 'AliasEdit').val(),
        Tags: $('#' + guid + 'TagsEdit').val(),
    };
    $.ajax({
        url: '/edit/groups/users/' + guid + '/' + uGroup.Alias + '/' + uGroup.Tags + '/edit',
        type: 'POST',
        data: JSON.stringify(uGroup),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            RestoreUserGroupStaticRow(guid);
            return false;
        }
    });
    return false;
}

function CancelEditUserGroup(guid) {
    if (guid != '') {
        RestoreUserGroupStaticRow(guid);
        return false;
    }
}

function RestoreUserGroupStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/usergroup/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (uGroup) {
            RestoreUserGroupHtml(uGroup);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreUserGroupHtml(uGroup) {
    var guid = uGroup.AnthillaGuid;
    var uGroupName = uGroup.AnthillaAlias;
    var uGroupTags = uGroup.AnthillaTags;
    var tags = "";
    $.each(uGroupTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    var parentRow = $('tr[id="' + guid + '"]');
    var newParentRow =
        "<td class='right'>" + uGroupName + "</td> \
                <td class='right'>&nbsp</td> \
                <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentRow.html(newParentRow);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var newAliasRow =
        "<td class='right'><label>UserGroup</label></td> \
                <td class='right value'>" + uGroupName + "</td> \
                <td>&nbsp;</td>";
    aliasRow.html(newAliasRow);

    var tagRow = $('tr[row="' + guid + 'row2"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row3"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteUserGroup(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/delete/groups/users/' + guid,
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[id="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandUserGroupRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

//FunctionGroup Table Management
$('.FunctionGroupTableBody').on('click', '.value', function () {
    var parentRow = $(this).parent();
    var guid = parentRow.attr('guid');
    if (CheckEdit(guid) == "enabled") {
        LoadFunctionGroupToEdit(guid);
    }
    return false;
});

$('.FunctionGroupTableBody').on('click', '.exp', function () {
    var self = $(this);
    ExpandFunctionGroupRows(self);
    FillFunctionsRow(self);
});

$('.FunctionGroupTableBody').on('click', '.lock', function () {
    var self = $(this);
    LockRowsExpansion(self);
});

function FillFunctionsRow(thisRow) {
    var guid = thisRow.attr('guid');
    var funcsRow = $('tr[row="' + guid + 'row2"]');
    var funcsCell = funcsRow.children('.functions-cell');
    jQuery.support.cors = true;
    $.ajax({
        url: '/get/functions/alias/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (aliasList) {
            $.each(aliasList, function (i, alias) {
                var template = $('<a class="button bg-transparent bg-active-dark"><i class="icon-wrench fg-yellow on-left"></i></a>');
                template.append(alias);
                funcsCell.append(template);
            });
        }
    });
}

function LoadFunctionGroupToEdit(guid) {
    DisableEdit(guid);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var funcsRow = $('tr[row="' + guid + 'row2"]');
    var tagRow = $('tr[row="' + guid + 'row3"]');
    var miscRow = $('tr[row="' + guid + 'row4"]');

    var aliasCell = aliasRow.children('.value');
    var funcCell = funcsRow.children('.value');
    var tagCell = tagRow.children('.value');
    var miscCell = miscRow.children('.value');

    var input = $(this).children('input');
    var val = input.val();
    var lock = $(document).find('.icon-unlocked');
    var isLocked = lock.attr('lock');

    if (isLocked != '1') {
        var alertMsg = '<i class="icon-warning fg-red on-left"></i> Inline edit is locked!';
        miscCell.html(alertMsg);
    }
    if (val == null && isLocked == '1') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/functiongroup/' + guid,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (fGroup) {
                var fGroupGuid;
                var fGroupAlias;
                var fGroupFunctions;
                var fGroupTags;

                fGroupGuid = fGroup.AnthillaGuid;
                fGroupAlias = fGroup.AnthillaAlias;
                fGroupFunctions = fGroup.AnthillaFunctionGuids;
                fGroupTags = fGroup.AnthillaTags;

                var aliasHtml =
                '<input id="' + fGroupGuid + 'AliasEdit" style="height: 30px; width: 90%;" type="text" value="' + fGroupAlias + '" />';

                $.ajax({
                    url: '/get/functions/alias/' + fGroupGuid,
                    type: 'GET',
                    dataType: 'json',
                    contentType: 'application/json;charset=utf-8',
                    success: function (aliasList) {
                        var funcHtml = '<input id="' + fGroupGuid + 'FunctionsEdit" type="text" value="' + aliasList + '" />';
                        funcCell.html(funcHtml);
                        SelectizeFunctions(guid);
                    }
                });

                var tagHtml =
                '<input id="' + fGroupGuid + 'TagsEdit" type="text" value="' + fGroupTags + '" />';

                var miscHtml =
                '<button style="font-weight: bold;" href="#" onclick="EditFunctionGroup(\'' + fGroupGuid + '\')" class="bg-anthilla-green fg-anthilla-gray upcase"><i class="icon-checkmark on-left"></i> Confirm</button>' +
                '<button style="font-weight: bold;" href="#" onclick="CancelEditFunctionGroup(\'' + fGroupGuid + '\')" ignore="yes" class="ignore bg-anthilla-violet fg-anthilla-gray upcase"><i class="icon-undo on-left"></i> Close</button>' +
                '<button style="font-weight: bold;" href="#" onclick="DeleteFunctionGroup(\'' + fGroupGuid + '\')" class="bg-anthilla-orange fg-anthilla-gray upcase"><i class="icon-remove on-left"></i> Delete</button>';

                aliasCell.html(aliasHtml);
                tagCell.html(tagHtml);
                miscCell.html(miscHtml);

                SelectizeInput(guid);
            }
        });
    }
    return false;
}

function EditFunctionGroup(guid) {
    jQuery.support.cors = true;
    var fGroup = {
        Alias: $('#' + guid + 'AliasEdit').val(),
        Functions: $('#' + guid + 'FunctionsEdit').val(),
        Tags: $('#' + guid + 'TagsEdit').val()
    };
    $.ajax({
        url: '/edit/groups/function/' + guid + '/' + fGroup.Alias + '/' + fGroup.Tags + '/edit',
        type: 'POST',
        data: JSON.stringify(fGroup),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var arr = fGroup.Functions.split(',');
            $.each(fGroupTags, function (index, value) {
                $.ajax({
                    url: '/groups/functions/add/' + value + '/' + guid,
                    type: 'POST',
                    data: JSON.stringify(fGroup),
                    contentType: "application/json;charset=utf-8",
                    success: function () {
                        return false;
                    }
                });
            });
            RestoreFunctionGroupStaticRow(guid);
            return false;
        }
    });
    return false;
}

function CancelEditFunctionGroup(guid) {
    if (guid != '') {
        RestoreFunctionGroupStaticRow(guid);
        return false;
    }
}

function RestoreFunctionGroupStaticRow(guid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/functiongroup/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (fGroup) {
            RestoreFunctionGroupHtml(fGroup);
            EnableEdit(guid);
            return false;
        }
    });
    return false;
}

function RestoreFunctionGroupHtml(fGroup) {
    var guid = fGroup.AnthillaGuid;
    var fGroupName = fGroup.AnthillaAlias;
    var fGroupTags = fGroup.AnthillaTags;
    var tags = "";
    $.each(fGroupTags, function (index, value) {
        tags += "<a class='button bg-transparent bg-active-dark'><i class='icon-tag on-left'></i>" + value + "</a>"
    });

    $.ajax({
        url: '/get/functions/' + guid,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (functions) {
            var strFunctions = "";
            $.each(functions, function (i, fun) {
                strFunctions += "<a class='button bg-transparent bg-active-dark'><i class='icon-wrench fg-yellow on-left'></i>" + fun.AnthillaAlias + "</a>"
            });
            var funcsRow = $('tr[row="' + guid + 'row2"]');
            var newFuncsRow =
                "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + strFunctions + "</td> \
                <td>&nbsp;</td>";
            funcsRow.html(newFuncsRow);
        }
    });

    var parentRow = $('tr[id="' + guid + '"]');
    var newParentRow =
        "<td class='right'>" + fGroupName + "</td> \
                <td class='right'>&nbsp</td> \
                <td><i class='arr fg-anthilla-green icon-arrow-up-4'></i></td>";
    parentRow.html(newParentRow);

    var aliasRow = $('tr[row="' + guid + 'row1"]');
    var newAliasRow =
        "<td class='right'><label>FunctionGroup</label></td> \
                <td class='right value'>" + fGroupName + "</td> \
                <td>&nbsp;</td>";
    aliasRow.html(newAliasRow);

    //var funcsRow = $('tr[row="' + guid + 'row2"]');
    //var newFuncsRow =
    //    "<td class='right'><label>Tags</label></td> \
    //            <td class='right value'>" + strFunctions + "</td> \
    //            <td>&nbsp;</td>";
    //funcsRow.html(newFuncsRow);

    var tagRow = $('tr[row="' + guid + 'row3"]');
    var newTagRow =
        "<td class='right'><label>Tags</label></td> \
                <td class='right value'>" + tags + "</td> \
                <td>&nbsp;</td>";
    tagRow.html(newTagRow);

    var miscRow = $('tr[row="' + guid + 'row4"]');
    var newMiscRow =
        "<td>&nbsp;</td> \
                <td class='right value'>&nbsp;</td> \
                <td>&nbsp;</td>";
    miscRow.html(newMiscRow);
    return false;
}

function DeleteFunctionGroup(guid) {
    if (guid != '') {
        jQuery.support.cors = true;
        $.ajax({
            url: '/delete/groups/function/' + guid,
            type: 'POST',
            data: JSON.stringify(guid),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                var row = $('tr[id="' + guid + '"]');
                var empty = '';
                row.html(empty);
                row.hide();
                return false;
            }
        });
    }
    return false;
}

function ExpandFunctionGroupRows(thisRow) {
    var nextRow_I = thisRow.next();
    var nextRow_II = nextRow_I.next();
    var nextRow_III = nextRow_II.next();
    var nextRow_IV = nextRow_III.next();
    nextRow_I.toggle();
    nextRow_II.toggle();
    nextRow_III.toggle();
    nextRow_IV.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}

//Imap Table Management
$('.ImapSettingTableBody').on('click', '.exp', function () {
    var guid = $(this).attr('data-this');
    ExpandImapRows(guid);
});

function ExpandImapRows(guid) {
    var row = $('tr[data-row="' + guid + '"]');
    row.toggle();
    var arrow = $(this).find('.arr')
    arrow.toggleClass('icon-arrow-up-4');
    arrow.toggleClass('icon-arrow-down-4');
    return false;
}