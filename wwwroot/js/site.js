// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Swiper
try {
    const swiper = new Swiper(".swiper", {
        loop: true,
        centeredSlides: true,
        direction: 'horizontal',
        spaceBetween: 100,
        slidersPerView: 1,
        breakpoints: {
            1024: {
                slidesPerView: 3,
                spaceBetween: 50,
            }
        },
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        }
    });

    const swiper2 = new Swiper(".swiper2", {
        loop: true,
        slidersPerView: 1,
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
    });

}
catch (error) {
    console.log("no swiper found");
}

//Table List
function toggle(source) {
    checkboxes = $("tbody tr input");
    console.log(checkboxes);
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        checkboxes[i].checked = source.checked;
    }
}

function deleteSelected(event) {
    selected = $("tbody input:checkbox:checked");
    if (selected.length == 0) {

        $("#empty-delete-error").modal("show");

        $(".close-popup").click(function () {
            $("#empty-delete-error").modal("hide");
            location.reload();
        });
    }
    else {
        var deleteArr = [];
        selected.each(function (index, value) {
            deleteArr.push($(value).val());
        });
        console.log(deleteArr.join(","));
        var idsString = deleteArr.join(",");
        var serviceURL = "/Posts/MultipleDelete";
        $.ajax({
            type: "GET",
            url: serviceURL,
            data: { idsStr: idsString },
            success: function (data) {
                $("#success-message").modal("show");
                $(".close-popup").click(function () {
                    $("#success-message").modal("hide");
                    location.reload();
                });
            },
            error: function (data) {
                errorFunc();
            },
        });
    }

}
function deleteMedia(event) {

    var serviceURL = "/MediaItem/DeleteMediaItem/" + event;
    var mediaId = event;

    $.ajax({
        type: "DELETE",
        url: serviceURL,
        data: mediaId,
        success: function (data) {
            /*alert("success");*/
            window.location.reload()
        },
        error: function (data) {
            alert("error");
        },
    })

}

function successFunc(data, status) {
    alert(data);
}

function errorFunc() {
    $("#error-message").modal("show");
    $(".close-popup").click(function () {
        $("#success-message").modal("hide");
        location.reload();
    });
}

function publishSelected() {
    selected = $("tbody input:checkbox:checked");
    var publishArr = [];
    selected.each(function (index, value) {
        publishArr.push($(value).val());
    });
    console.log(publishArr.join(","));
    var idsString = publishArr.join(",");
    var serviceURL = "/Posts/MultiplePublish";
    $.ajax({
        type: "GET",
        url: serviceURL,
        data: { idsStr: idsString },
        success: function (data) {
            $("#success-message-published").modal("show");
            $(".close-popup").click(function () {
                $("#success-message-published").modal("hide");
                location.reload();
            });
        },
        error: function (data) {
            errorFunc();
        },
    });
    console.log(publishArr);
}


function deleteMedia(event) {

    var serviceURL = "/MediaItem/DeleteMediaItem/" + event;
    var mediaId = event;

    $.ajax({
        type: "DELETE",
        url: serviceURL,
        data: mediaId,
        success: function (data) {
            /*alert("success");*/
            window.location.reload()
        },
        error: function (data) {
            alert("error");
        },
    })

}



/*function submitShareForm() {

    if ($("input[type=checkbox]:checked").length == 0) {


        e.preventDefault();
        alert("select at least one channel")
    }

    else {

        var serviceURL = "/Social/Publish/Send" + e;
        var shareForm = $(':input').serializeArray();
        var imgSrc = $('.shareImage').attr('src');
        shareForm.push(({ name: 'src', value: imgSrc }));

        $.ajax({
            type: "POST",
            url: shareForm,
            data: mediaId,
            success: function (data) {
                window.location.reload()
            },
            error: function (data) {
                alert("error");
            },
        })
    }

}*/



try {
    var calendarInstance = new calendarJs("myCalendar", {
        exportEventsEnabled: true,
        manualEditingEnabled: true,
        showTimesInMainCalendarEvents: false,
        fullScreenModeEnabled: true,
        minimumDayHeight: 0,
        organizerName: "Your Name",
        organizerEmailAddress: "your@email.address",
        visibleDays: [0, 1, 2, 3, 4, 5, 6],
        showExtraToolbarButtons: true,
        events: getEvents(),
        onEventDragStart: onDragEventStart,
        onEventDragStop: onDragEventStop,
        onEventDragDrop: onDragEventDrop,
        onBeforeEventAddEdit: null,
        addYearButtonsOnMainDisplay: false,
        useAmPmForTimeDisplays: false,
    });

    document.title += " v" + calendarInstance.getVersion();
    document.getElementById("header").innerText +=
        " v" + calendarInstance.getVersion();

    function turnOnEventNotifications() {
        calendarInstance.setOptions({
            eventNotificationsEnabled: true,
        });
    }

    function addEventType() {
        console.log(
            "Event type added: " +
            calendarInstance.addEventType(5, "A New Event Type")
        );
    }

    function removeEventType() {
        console.log("Event type removed: " + calendarInstance.removeEventType(5));
    }

    function setEvents() {
        calendarInstance.setEvents(getEvents());
    }

    function removeEvent() {
        calendarInstance.removeEvent(new Date(), "Test Title 2");
    }

    function daysInMonth(year, month) {
        return new Date(year, month + 1, 0).getDate();
    }

    function setOptions() {
        calendarInstance.setOptions({
            minimumDayHeight: 70,
            manualEditingEnabled: false,
            exportEventsEnabled: false,
            showDayNumberOrdinals: false,
            fullScreenModeEnabled: false,
            maximumEventsPerDayDisplay: 0,
            showTimelineArrowsOnViews: false,
            maximumEventTitleLength: 10,
            maximumEventDescriptionLength: 10,
            maximumEventLocationLength: 10,
            maximumEventGroupLength: 10,
            showDayNamesInMainDisplay: false,
            tooltipsEnabled: true,
            visibleDays: [0, 1, 2, 3, 4],
            allowEventScrollingOnMainDisplay: true,
            showExtraToolbarButtons: false,
            hideEventsWithoutGroupAssigned: true,
            showHolidays: false,
            allowHtmlInDisplay: true,
            workingDays: [],
            startOfWeekDay: 0,
        });
    }

    function setSearchOptions() {
        calendarInstance.setSearchOptions({
            left: 10,
            top: 10,
        });
    }

    function onlyDotsDisplay() {
        calendarInstance.setOptions({
            useOnlyDotEventsForMainDisplay: true,
        });
    }

    function setCurrentDisplayDate() {
        var newDate = new Date();
        newDate.setMonth(newDate.getMonth() + 3);

        calendarInstance.setCurrentDisplayDate(newDate);
    }

    function getEvents() {
        var previousDay = new Date(),
            today9 = new Date(),
            today11 = new Date(),
            tomorrow = new Date(),
            firstDayInNextMonth = new Date(),
            lastDayInNextMonth = new Date(),
            today = new Date(),
            today3HoursAhead = new Date(),
            previousYear = new Date(),
            nextYear = new Date(),
            overlappingEvent1 = new Date(),
            overlappingEventTo1 = new Date(),
            overlappingEvent2 = new Date(),
            overlappingEventTo2 = new Date(),
            overlappingEvent3 = new Date(),
            overlappingEventTo3 = new Date(),
            overlappingEvent4 = new Date(),
            overlappingEventTo4 = new Date(),
            overlappingEvent5 = new Date(),
            overlappingEventTo5 = new Date();

        previousDay.setDate(previousDay.getDate() - 1);
        today11.setHours(11);
        tomorrow.setDate(today11.getDate() + 1);
        today9.setHours(9);

        firstDayInNextMonth.setDate(1);
        firstDayInNextMonth.setDate(
            firstDayInNextMonth.getDate() +
            daysInMonth(
                firstDayInNextMonth.getFullYear(),
                firstDayInNextMonth.getMonth()
            )
        );

        lastDayInNextMonth.setDate(1);
        lastDayInNextMonth.setMonth(lastDayInNextMonth.getMonth() + 1);
        lastDayInNextMonth.setDate(
            lastDayInNextMonth.getDate() +
            daysInMonth(
                lastDayInNextMonth.getFullYear(),
                lastDayInNextMonth.getMonth()
            ) -
            1
        );

        today.setHours(21, 59, 0, 0);
        today.setDate(today.getDate() + 3);
        today3HoursAhead.setHours(23, 59, 0, 0);
        today3HoursAhead.setDate(today3HoursAhead.getDate() + 3);

        previousYear.setFullYear(previousYear.getFullYear() - 1);
        nextYear.setFullYear(nextYear.getFullYear() + 1);

        overlappingEvent1.setDate(overlappingEvent1.getDate() - 3);
        overlappingEventTo1.setDate(overlappingEventTo1.getDate() - 3);
        overlappingEvent2.setDate(overlappingEvent2.getDate() - 3);
        overlappingEventTo2.setDate(overlappingEventTo2.getDate() - 3);
        overlappingEvent3.setDate(overlappingEvent3.getDate() - 3);
        overlappingEventTo3.setDate(overlappingEventTo3.getDate() - 3);
        overlappingEvent4.setDate(overlappingEvent4.getDate() - 3);
        overlappingEventTo4.setDate(overlappingEventTo4.getDate() - 3);
        overlappingEvent5.setDate(overlappingEvent5.getDate() - 3);
        overlappingEventTo5.setDate(overlappingEventTo5.getDate() - 3);
        overlappingEvent1.setHours(0, 10, 0, 0);
        overlappingEventTo1.setHours(1, 10, 0, 0);
        overlappingEvent2.setHours(0, 35, 0, 0);
        overlappingEventTo2.setHours(1, 35, 0, 0);
        overlappingEvent3.setHours(1, 20, 0, 0);
        overlappingEventTo3.setHours(2, 20, 0, 0);
        overlappingEvent4.setHours(2, 0, 0, 0);
        overlappingEventTo4.setHours(3, 0, 0, 0);
        overlappingEvent5.setHours(3, 30, 0, 0);
        overlappingEventTo5.setHours(4, 40, 0, 0);

        /*   return [
                   {
                       from: overlappingEvent1,
                       to: overlappingEventTo1,
                       title: "Overlapping Event 1",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       group: "Group 1",
                       type: 1,
                       customTags: {
                           testTag: true
                       }
                   },
                   {
                       from: overlappingEvent2,
                       to: overlappingEventTo2,
                       title: "Overlapping Event 2",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       group: "Group 1",
                       type: 1,
                       customTags: true
                   },
                   {
                       from: overlappingEvent3,
                       to: overlappingEventTo3,
                       title: "Overlapping Event 3",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       group: "Group 1",
                       type: 1,
                       customTags: [1]
                   },
                   {
                       from: overlappingEvent4,
                       to: overlappingEventTo4,
                       title: "Overlapping Event 4",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       group: "Group 1",
                       type: 1,
                       customTags: 1
                   },
                   {
                       from: overlappingEvent5,
                       to: overlappingEventTo5,
                       title: "Overlapping Event 5",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       group: "Group 1",
                       type: 1,
                       customTags: "Test Tag"
                   },
                   {
                       from: previousYear,
                       to: previousYear,
                       title: "Previous Year",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       group: "Group 2",
                       type: 2
                   },
                   {
                       from: nextYear,
                       to: nextYear,
                       title: "Next Year",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       group: "Group 2",
                       type: 2
                   },
                   {
                       from: previousDay,
                       to: previousDay,
                       title: "Previous Day",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       color: "#FF0000",
                       colorText: "#FFFF00",
                       colorBorder: "#00FF00",
                       repeatEvery: 5,
                       id: "1234-5678-9",
                       group: "Group 1",
                       locked: true,
                       type: 3
                   },
                   {
                       from: today11,
                       to: tomorrow,
                       title: "Title 1",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: false,
                       group: "group 1"
                   },
                   {
                       from: tomorrow,
                       to: today11,
                       title: "Title Bad (should not show)",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: false,
                       group: "group 1",
                       type: 0
                   },
                   {
                       from: today9,
                       to: today9,
                       title: "Title 2",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       group: "Group 1",
                       url: "https://www.google.com/",
                       type: 4
                   },
                   {
                       from: firstDayInNextMonth,
                       to: firstDayInNextMonth,
                       title: "First Day 1",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       color: "#00FF00",
                       colorText: "#FF0000",
                       repeatEvery: 4,
                       type: 0
                   },
                   {
                       from: firstDayInNextMonth,
                       to: firstDayInNextMonth,
                       title: "First Day 2",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       color: "#00FF00",
                       colorText: "#FF0000",
                       repeatEvery: 4,
                       type: 0
                   },
                   {
                       from: lastDayInNextMonth,
                       to: lastDayInNextMonth,
                       title: "Last Day 1",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       location: "Teams Meeting",
                       isAllDay: true,
                       color: "#0000FF",
                       repeatEvery: 2,
                       type: 0
                   },
                   {
                       from: today,
                       to: today3HoursAhead,
                       title: "Regular Event",
                       description: "This is a another <b>description</b> of the event that has been added, so it can be shown in the pop-up dialog.",
                       repeatEvery: 1,
                       repeatEveryExcludeDays: [6, 0],
                       repeatEnds: new Date(today.getFullYear() + 1, 0, 1),
                       group: "Group 1",
                       type: 0
                   }
               ];*/
    }

    function onDragEventStart(eventDetails) {
        console.log("Event drag started: " + eventDetails.id);
    }

    function onDragEventStop(eventDetails) {
        console.log("Event drag stopped: " + eventDetails.id);
    }

    function onDragEventDrop(eventDetails, dropDate) {
        console.log(
            "Event drag dropped: " + eventDetails.id + ". Date dropped: " + dropDate
        );
    }

    function getCopiedEvent() {
        var today = new Date(),
            todayPlus1Hour = new Date();

        todayPlus1Hour.setHours(today.getHours() + 1);

        return {
            from: today,
            to: todayPlus1Hour,
            title: "Copied Event",
            description:
                "This is a another description of the event that has been added, so it can be shown in the pop-up dialog.",
            group: "Group 1",
        };
    }

    function addNewHolidays() {
        var today = new Date();

        var holiday1 = {
            day: today.getDate(),
            month: today.getMonth() + 1,
            year: today.getFullYear(),
            title: "Google Day",
            onClick: function () {
                window.open("https://www.google.com/", "_blank");
            },
        };

        var holiday2 = {
            day: today.getDate(),
            month: today.getMonth() + 1,
            title: "Calendar.js Day",
            backgroundColor: "lightblue",
            textColor: "maroon",
            onClick: function () {
                window.open("https://github.com/williamtroup/Calendar.js", "_blank");
            },
        };

        calendarInstance.addHolidays([holiday1, holiday2]);
    }

    function removeNewHolidays() {
        calendarInstance.removeHolidays(["Google Day", "Calendar.js Day"]);
    }
} catch {
    console.log("no calendar");
}

$("#details").ready(function () {
    $(function () {
        // some json data
        var FJsonData = {
            //pass defualt data to form inputs
            /*      name: "facebook",
                        id: "123",
                        enabled: true,*/
        };

        // initialize the form, prefix is optional and defaults to data

        /*        $("#configsList").jsForm({
            
                    });
            */

        try {
            $("#facebookConfigs").jsForm({
                /*data: FJsonData,*/
            });

            $("#twitterConfigs").jsForm({});

            $("#telegramConfigs").jsForm({});

            $("#instagramConfigs").jsForm({});

            $("#whatsappConfigs").jsForm({});
        } catch {
            console.log("no formjs");
        }

        /*        $("#show").click(function () {
                        // show the json data
                        alert(JSON.stringify($("#instagramConfigs").jsForm("get"), null, " ") + JSON.stringify($("#facebookConfigs").jsForm("get"), null, " "));
                    });*/
    });
});

