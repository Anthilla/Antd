$('input[data-role="command-place"]').keyup(function () {
    var thisValue = $(this).val();
    if (thisValue.indexOf("[") > 0) {
        //todo: in questo caso prendere la stringa che si sta per scrivere dopo la [
        //      fare una query, tipo selectize, nei ValueBundle e mostrare tutt i valori appartenenti a quel tag
        //      con relativo indice
        //      in questo modo puoi già scrivere il tag esatto senza dover andare a filtrare la tabella dei valori
        //      OPPURE, questa query la si fa sulla tabella, senza fare chiamate ajax
        //      tipo il campo di input di 'Search
        //      e in base a quello che scrivi ti mostra solo le righe della tabella interessate
    }
    if (thisValue.length > 0) {
        jQuery.support.cors = true;
        $.ajax({
            url: "/cfg/layouts",
            type: "GET",
            success: function (layoutJson) {
                var results = [];
                $.each(layoutJson, function (i, item) {
                    if (item.name.indexOf(thisValue) === 0) {
                        results.push(item.name);
                    }
                });
                if (results.length > 0) {
                    DisplayResults(results);
                }
                else {
                    CleanResults();
                    HideDisplayer();
                }
            }
        });
    }
    else {
        CleanResults();
        HideDisplayer();
    }
});

function ShowDisplayer() {
    $("#ResultsContainer").show();
}

function HideDisplayer() {
    $("#ResultsContainer").hide();
}

function DisplayResults(objectsToDisplay) {
    ShowDisplayer();
    CleanResults();
    $.each(objectsToDisplay, function (i, item) {
        $("#ResultsContainer").append('<li data-role="query-result" class="bg-anthilla-violet" style="display: inline-block; padding: 3px 15px; margin: 5px;">' + item + "</li>");
        SelectLayoutToEdit();
    });
}

function CleanResults() {
    var container = $("#ResultsContainer");
    container.html("");
}

function SelectLayoutToEdit() {
    $('[data-role="query-result"]').dblclick(function () {
        $('input[data-role="command-place"]').val($(this).text());
    });
}

$('i[data-role="show-all-command"]').mousedown(function () {
    $('[data-role="command-saved"]').each(function () { $(this).hide(); });
    $('[data-role="command-exd"]').each(function () { $(this).show(); });
});

$('i[data-role="show-all-command"]').mouseup(function () {
    $('[data-role="command-saved"]').each(function () { $(this).show(); });
    $('[data-role="command-exd"]').each(function () { $(this).hide(); });
});

$('i[data-role="show-command"]').mousedown(function () {
    var container = $(this).parents("li");
    container.find('[data-role="command-saved"]').hide();
    container.find('[data-role="command-exd"]').show();
});

$('i[data-role="show-command"]').mouseup(function () {
    var container = $(this).parents("li");
    container.find('[data-role="command-saved"]').show();
    container.find('[data-role="command-exd"]').hide();
});

$("#sortable").sortable({
    //sort: function () {
    //    RefreshCommandIndexes();
    //    ReindexCheck();
    //},
    stop: function () {
        RefreshCommandIndexes();
        ReindexCheck();
    },
}).disableSelection();

$(document).on("click", "body", function () {
    RefreshCommandIndexes();
    ReindexCheck();
});

$(document).ready(function () {
    $("#valueBundleTag").hide();
});

function ReindexCheck() {
    $('[data-role="control"]').each(function (index) {
        var i = index;
        $(this).find('[data-role="DisplayIndex"]').val(i);
        $(this).find("input").each(function () {
            var name = $(this).attr("data-name");
            var nn = name + "_" + i;
            $(this).attr("name", nn);
        });
    });
}

function RefreshCommandIndexes() {
    $("input[name=Index]").each(function (index) {
        $(this).val("");
        $(this).val(index);
    });
}

$('i[data-role="remove-command"]').click(function () {
    var g = $(this).attr("data-guid");
    jQuery.support.cors = true;
    $.ajax({
        url: "/cfg/delcommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('i[data-role="disable-command"]').click(function () {
    var g = $(this).attr("data-guid");
    jQuery.support.cors = true;
    $.ajax({
        url: "/cfg/disablecommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('i[data-role="enable-command"]').click(function () {
    var g = $(this).attr("data-guid");
    jQuery.support.cors = true;
    $.ajax({
        url: "/cfg/enablecommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('i[data-role="play-command"]').click(function () {
    var g = $(this).attr("data-guid");
    jQuery.support.cors = true;
    $.ajax({
        url: "/cfg/launchcommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
});

$('input[data-role="command-place"]').focusin(function () {
    $("#Tip").show();
});

$('input[data-role="command-place"]').focusout(function () {
    $("#Tip").hide();
});
