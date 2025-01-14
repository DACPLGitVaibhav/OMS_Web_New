var reloadInterval;
var currentPage = 0;
var rowsPerPage = 50; // Set the number of rows per page
var totalData = [];
var LineId = 1;
$(document).ready(function () {
    $('#Table').DataTable({
        dom: 'Blfrtip',
        "ordering": false,
        paging: false,
        info: false,
        buttons: [],
        "autoWidth": false,
        "drawCallback": function (settings) {
            $(".Chenge").each(function () {
                if ($(this).attr("data-id") == 0) {
                    $(this).css('background-color', '#337ab7').text("Schedule");///blue Schedule

                }
                if ($(this).attr("data-id") == 1) {
                    $(this).css('background-color', '#999966').text("Received");// militry Received

                }
                if ($(this).attr("data-id") == 2) {
                    $(this).css('background-color', '#777777').text("Started");//gray Started

                }

                if ($(this).attr("data-id") == 3) {
                    $(this).css('background-color', '#f0ad4e').text("Processed");//orange process

                }
                if ($(this).attr("data-id") == 4) {
                    $(this).css('background-color', '#5cb85c').text("Transmitted");//green Transmitted

                }
                if ($(this).attr("data-id") == 100) {
                    $(this).css('background-color', '#5bc0de').text("Breakpoint");//skyblue Breakpoint

                }
                if ($(this).attr("data-id") == 101) {
                    $(this).css('background-color', '#d9534f').text("Suspended");//red Suspended

                }
                if ($(this).attr("data-id") == 102) {
                    $(this).css('background-color', '#000000').text("Abounded");//Black Abounded

                }
            });
        }
    });


    //$('#pageButtons').on('click', '.page-button', function () {
    //    currentPage = parseInt($(this).data('page'), 10);
    //    renderTable();
    //});
    //// Listen for changes in the dropdown
    //$('#rowsPerPage').on('change', function () {

    //    rowsPerPage = parseInt($(this).val(), 10);
    //    if (rowsPerPage != 100) {
    //        currentPage = 0;
    //    }
    //    reloadTable(rowsPerPage);

    //});


    // Initial call to generate page buttons
   // generatePageButtons();
    startReload();


});
function startReload() {

    reloadTable(rowsPerPage);
    reloadInterval = setInterval(function () {
        reloadTable(rowsPerPage);
    }, 10000);
}
function stopReload() {
    clearInterval(reloadInterval);
}
$('.aaa').click(function (e) {
    e.preventDefault();
    currentPage = 0;
    $('.aaa').removeClass('active');
    $(this).addClass('active');
    LineId = $(this).data('id');
    reloadTable(rowsPerPage);
});
function reloadTable(rowsPerPage) {
    if (LineId != 0) {
        $.ajax({
            url: '/LineMaster/Get_Baretail_Log_ByLineId',
            type: 'GET',
            data: { LineId: LineId },
            success: function (result) {
                totalData = result;
                renderTable();
                //  console.log(result);
            },
            error: function (xhr, status, error) {
                // Handle errors
               // console.error(xhr.responseText);
            }
        });
    }
    else {
        $.ajax({
            url: '/LineMaster/ReloadBaretail_Log',
            type: 'GET',
            success: function (result) {
                totalData = result;
                renderTable();
                // console.log(result);
            },
            error: function (xhr, status, error) {
                // Handle errors
               // console.error(xhr.responseText);
            }
        });
    }
}
function renderTable() {
   // debugger;
    const tableBody = $('#Table tbody');
    tableBody.empty();

    //var startIndex = currentPage * rowsPerPage;
    //var endIndex = startIndex + rowsPerPage;
    //var dataToDisplay = totalData.slice(startIndex, endIndex);

    var dataToDisplay = totalData;
    var sno = 1;
    ///Append Row/////
    dataToDisplay.forEach(function (item) {
        var ID = item.id;
        var TimeStamp = forDateTime(item.timeStamp);
        var LineName = item.lineName;
        var SeqNo = item.seqNo;
        var StatusID = item.statusID;
        var Itemid = item.itemid;
        var biwno = item.biwno;
        tableBody.append("<tr class='data-row'>" +

            "<td>" + sno + "</td>" +
           
            "<td>" + LineName + "</td>" +
            "<td>" + SeqNo + "</td>" +
            "<td>" + Itemid + "</td>" +
            "<td>" + biwno + "</td>" +
            "<td> <button class='Chenge' data-id='" + StatusID + "' style='color: white; border:none;width: 110px;'></button></td>" +
            "<td>" + TimeStamp + "</td>" +
            "</tr>");

        $(".Chenge").each(function () {
            if ($(this).attr("data-id") == 0) {
                $(this).css('background-color', '#337ab7').text("Schedule");///blue Schedule

            }
            if ($(this).attr("data-id") == 1) {
                $(this).css('background-color', '#999966').text("Received");// militry Received

            }
            if ($(this).attr("data-id") == 2) {
                $(this).css('background-color', '#777777').text("Started");//gray Started

            }

            if ($(this).attr("data-id") == 3) {
                $(this).css('background-color', '#f0ad4e').text("Processed");//orange process

            }
            if ($(this).attr("data-id") == 4) {
                $(this).css('background-color', '#5cb85c').text("Transmitted");//green Transmitted

            }
            if ($(this).attr("data-id") == 100) {
                $(this).css('background-color', '#5bc0de').text("Breakpoint");//skyblue Breakpoint

            }
            if ($(this).attr("data-id") == 101) {
                $(this).css('background-color', '#d9534f').text("Suspended");//red Suspended

            }
            if ($(this).attr("data-id") == 102) {
                $(this).css('background-color', '#000000').text("Abounded");//Black Abounded

            }
        });

        //totalCountFromTotalData = totalData.reduce(function (count, item) {
        //    if (item.ff === 0 && item.fe === 0 && item.rf === 0 && item.bslh === 0 && item.bsrh === 0) {
        //        return count + 1;
        //    }
        //    return count;
        //}, 0);
        sno++;
    });


    //generatePageButtons();
}
function generatePageButtons() {

    //const pageButtons = $('#pageButtons');
    //pageButtons.empty();

    //var totalPages = Math.ceil(totalData.length / rowsPerPage);

    //for (var i = 0; i < totalPages; i++) {
    //    var page = i;
    //    var button = $('<li><a href="#" class="page-button" data-page="' + page + '">' + (page + 1) + '</a></li>');
    //    pageButtons.append(button);
    //}
    //pageButtons.find('a[data-page="' + currentPage + '"]').addClass('current-page');
}
function forDateTime(date) {
    // debugger;
    var fromorTodate;
    var d = new Date(date.split("/").reverse().join("-"));
    var dd = `${(d.getDate())}`.padStart(2, '0');
    var mm = `${(d.getMonth() + 1)}`.padStart(2, '0');
    var yy = d.getFullYear();
    var h = `${d.getHours()}`.padStart(2, '0');

    var m = `${d.getMinutes()}`.padStart(2, '0');
    var s = `${d.getSeconds()}`.padStart(2, '0');
    fromorTodate = dd + "/" + mm + "/" + yy + " " + h + ":" + m + ":" + s;

    return fromorTodate;
}