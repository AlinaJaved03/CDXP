$(function () {
    try {
        var $jsonstr = JSON.parse(decodeURIComponent($('body').attr('spoappcomponent')).replace(/\+/g, ' '));
        if ($jsonstr[0].DASHBOARD != "") {

            var options = {};
            $("#pageContainer").hide('slide', options, 300);
            $.ajax({
                type: "POST",
                url: "/" + $jsonstr[0].DASHBOARD + $(location).attr('search'),
                success: function (result) {
                    var options = {};
                    $('#pageContainer').html($(result).find('#pcontent').html());
                    $("#pageContainer").show('slide', options, 500);
                }
            });
        }

        $('#NameTitlebar').html($jsonstr[0].DISPLAY_NAME + ' ' + $jsonstr[0].DISPLAY_DESIGNATION);
        $.each($jsonstr, function (indexInArray, valueOfElement) {
            $('.cppag_m').append('<li>\
            <a '+ valueOfElement.DESCRIPTION + ' href="javascript:void(0);" data-toggle="collapse" data-target="#cppag_l' + valueOfElement.WP_SETUP_MENU_ID + '" class="collapsed" aria-expanded="false">\
            <div class="pull-left"><i class="'+ valueOfElement.CLASS_NAME + '"></i>\
            <span class="right-nav-text">'+ valueOfElement.MENU_NAME + '</span></div>\
            <div class="pull-right"><i class="ti-angle-down"></i></div>\
            <div class="clearfix"></div></a>\
            <ul id="cppag_l'+ valueOfElement.WP_SETUP_MENU_ID + '" spoappcomponent="' + encodeURIComponent(valueOfElement.S_MENU) + '" class="collapse-level-1 collapse" aria-expanded="false" style="height: 0px;"></ul>\
            </li>');
        });
        $('ul[id^="cppag_l"]').each(function (index, element) {
            var $jsonlis = eval("(" + decodeURIComponent($(this).attr('spoappcomponent')).replace(/\+/g, ' ') + ")");
            var $thisul = $(this);
            $.each($jsonlis, function (indexInArray, valueOfElement) {
                $thisul.append('<li><a  ' + valueOfElement._D + '  class="clsLink" href="#" controller="' + valueOfElement._CTRL + '" v="' + valueOfElement._CTRLV + '">' + valueOfElement._NAME + '</a></li>');
            });
        });
        $('body').removeAttr('spoappcomponent');
    } catch (e) {
        console.log(e.statusText);
    }
    try {
        $.ajax({
            type: "POST",
            url: "/Invoice/getWHours" + $(location).attr("search"),
            contenType: false,
            processData: false,
            async: false,
            success: function (result) {
               // $('#workingHours').text(result.split('½')[0] + ", " + result.split('½')[1] + "-" + result.split('½')[2])
                if (result.split('½')[1] == "" & result.split('½')[2] == "")
                    $('#workingHours').text(result.split('½')[0] + ", Off Day");
                else
                    $('#workingHours').text(result.split('½')[0] + ", " + result.split('½')[1] + "-" + result.split('½')[2])
            }
        });
    }
    catch (ex) {
        console.log(ex.statusText);
    }
    $('body').off('click', '.clsLink');
    $('body').on('click', '.clsLink', function (e) {
        e.preventDefault();
        var options = {};
        $("#pageContainer").hide('slide', options, 300);
        $.ajax({
            type: "POST",
            url: "/" + $(this).attr('controller') + "/" + $(this).attr('v') + $(location).attr('search'),
            success: function (result) {
                var options = {};
                $('#pageContainer').html($(result).find('#pcontent').html());
                $("#pageContainer").show('slide', options, 500);
            }
        });
    });
});

$('body').off('click', '#workingHoursDetails');
$('body').on('click', '#workingHoursDetails', function (e) {
    $.ajax({
        type: "POST",
        url: "/Invoice/getWHoursDetails" + $(location).attr("search"),
        //    data: formdata,
        contenType: false,
        processData: false,
        async: false,
        // data: { "vls": '29' },/**important*/
        success: function (result) {
            debugger;
            $('.tblContainerWH').html(result);
        }
    });


});
$('body').on('click', '.tab1', function (e) { $(this).parent().next().slideToggle('slow'); });


$(function () {

    var offsetX = 15;
    var offsetY = 15;
    var TooltipOpacity = 0.9;
    $('body').on('mouseenter', '[title]', function (e) {
        var Tooltip = $(this).attr('title');

        if ($.trim(Tooltip) !== '' || Tooltip !== undefined) {
            $(this).attr('customTooltip', Tooltip);
            $(this).attr('title', '');
        }
        else {
            $(this).removeattr('customTooltip', Tooltip);
        }
        var customTooltip = $(this).attr('customTooltip');
        if (customTooltip !== '' || customTooltip !== undefined) {
            $("body").append('<div id="tooltip">' + customTooltip + '</div>');
            $('#tooltip').css('left', e.pageX + offsetX);
            //$('#tooltip').css('top', e.pageY + offsetY);
            $('#tooltip').css('top', e.clientY + offsetY);
            $('#tooltip').fadeIn('500');
            $('#tooltip').fadeTo('10', TooltipOpacity);
        }
        else {
            $("body").children('div#tooltip').remove();
        }

    }).on('mousemove', '[title]', function (e) {
        var Tooltip = $(this).attr('title');
        debugger;
        if ($.trim(Tooltip) !== '' || Tooltip !== undefined) {

            var X = e.pageX;
            var Y = e.clientY;

            var tipToBottom, tipToRight;
            tipToRight = $(window).width() - (X + offsetX + $('#tooltip').outerWidth() + 5);
            if (tipToRight < offsetX) {
                X = e.pageX + tipToRight;
            }
            tipToBottom = $(window).height() - (Y + offsetY + $('#tooltip').outerHeight() + 5);
            if (tipToBottom < offsetY) {
                Y = e.pageY + tipToBottom;
            }
            $('#tooltip').css('left', X + offsetX);
            $('#tooltip').css('top', Y + offsetY);
        }
    }).on('mouseleave', '[title]', function () {
        $(this).attr('title', $(this).attr('customTooltip'));
        $("body").children('div#tooltip').remove();
    });
});



$('body').on('focus', ".dtDate", function () {
    $(this).datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd-M-yy'
    });
});

$('body').on('focus', ".dtDateTime", function () {
    $(this).datetimepicker({
        format: 'DD-MMM-YYYY HH:mm'
    });
});


$("body").on("keypress keyup blur", ".clsFloat", function (event) {
    //this.value = this.value.replace(/[^0-9\.]/g,'');

    //$(this).val($(this).val().replace(/[^0-9\.]/g, ''));
    $(this).val(
        $(this).val().replace(/(?!^-)[^0-9.]/g, '').replace(/(\..*)\./g, '$1')
    );


    if ((event.which !== 46 || $(this).val().indexOf('.') !== -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }


});

$("body").on("keypress keyup blur", ".clsNumber", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});


$("body").on("keypress keyup blur", ".clsNumberWithNegativeValues", function (event) {

    var value = $(this).val();
    if (value.length == 0) {
        if ((event.which != 45) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    }
    else if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
