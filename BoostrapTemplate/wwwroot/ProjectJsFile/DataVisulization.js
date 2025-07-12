
/*<----#region DataLoad---->*/
var reloadInterval;
var currentPage = 0;
var rowsPerPage = 100; // Set the number of rows per page
var totalData = [];
$(document).ready(function () {
   $('#Table').DataTable({
        dom: 'Blfrtip',
        "ordering": false,
        paging: false,
        info: false,
        buttons: [/*'copy', 'csv', 'excel'*/],
        //lengthMenu: [
        //    [10, 25, 50, '-1'],
        //    [10, 25, 50, 'All']
        //],

        // pageLength: 5,

        columnDefs: [{
            //"defaultContent": "-",
            //"targets": "_all",
            //orderable: false,
            //className: 'select-checkbox',
            //targets: 0
        }],
        //select: {
        //    style: 'multi',
        //    selector: 'td:first-child'
        //},
        columns: [
            { data: "" },
            { data: 'biwNo' },
            { data: 'bslh' },
            { data: 'bsrh' },
            { data: 'erpSeqNo' },
            { data: 'fe' },
            { data: 'ff' },
            { data: 'itemId' },

            // { data: 'lineID' },
            { data: 'vcode' },
            { data: 'rf' },
            // { data: 'status' },
            // ...
        ],
        "autoWidth": false,
        "drawCallback": function (settings) {
            $(".Chenge").each(function () {
                if ($(this).attr("data-id") == 0) {
                    $(this).css('background-color', '#337ab7');///blue Schedule

                }
                if ($(this).attr("data-id") == 1) {
                    $(this).css('background-color', '#999966');// militry Received

                }
                if ($(this).attr("data-id") == 2) {
                    $(this).css('background-color', '#777777');//gray started

                }

                if ($(this).attr("data-id") == 3) {
                    $(this).css('background-color', '#f0ad4e');//orange process

                }
                if ($(this).attr("data-id") == 4) {
                    $(this).css('background-color', '#5cb85c');//green Transmitted

                }
                if ($(this).attr("data-id") == 100) {
                    $(this).css('background-color', '#5bc0de');//skyblue Breakpoint

                }
                if ($(this).attr("data-id") == 101) {
                    $(this).css('background-color', '#d9534f');//red Suspended

                }
                if ($(this).attr("data-id") == 102) {
                    $(this).css('background-color', '#000000');//Black Abounded

                }
            });
        }
    });
    $('#pageButtons').on('click', '.page-button', function () {
        currentPage = parseInt($(this).data('page'), 10);
        renderTable();
    });
    // Listen for changes in the dropdown
    $('#rowsPerPage').on('change', function () {
        $('#TRFBtnContainer').empty();
        $('#buttonContainer').empty();
        $('#RowcountDisplay').empty();
        rowsPerPage = parseInt($(this).val(), 10);
        if (rowsPerPage != 100) {
            currentPage = 0;
        }
        reloadTable(rowsPerPage);
    });
    // Initial call to generate page buttons
    generatePageButtons();
    startReload();

    CheckLineStatus();
    setInterval(CheckLineStatus, 3000);

   
});

//#region LineStatus updated by Vaibhav at 21-10-2024.
function CheckLineStatus() {
    $.ajax({
        type: 'GET',
        url: '/DataVisulization/GetLineStatus',
        dataType: 'json',
        success: function (data) {
            //debugger;
            if (data == true) {
                $('.alert-danger').show()
            }
            else {
                $('.alert-danger').hide()
            }
        }
    });
}
// #endregion
function startReload() {

    reloadTable(rowsPerPage);
    reloadInterval = setInterval(function () {
        reloadTable(rowsPerPage);
    }, 5000);
}
function stopReload() {
    clearInterval(reloadInterval);
}
function reloadTable(rowsPerPage) {
    $.ajax({
        url: '/DataVisulization/ReLoadData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            totalData = data;
            renderTable();
        },

        error: function (error) {
           // console.error('Error fetching data:', error);
        }
    });
}
function renderTable() {
    const tableBody = $('#Table tbody');
    tableBody.empty();

    var startIndex = currentPage * rowsPerPage;
    var endIndex = startIndex + rowsPerPage;
    var dataToDisplay = totalData.slice(startIndex, endIndex);
    var totalCountFromTotalData = 0;
    ///Append Row/////
    dataToDisplay.forEach(function (item) {
        var ErpSeqNo = item.erpSeqNo;
        var ItemId = item.itemId;
        var BiwNo = item.biwNo;
        var Vcode = item.vcode;
        var LineID = item.lineID;
        var Status = item.status;
        var FF = item.ff;
        var FE = item.fe;
        var RF = item.rf;
        var BSLH = item.bslh;
        var BSRH = item.bsrh;
        tableBody.append("<tr class='data-row'>" +
            "<td><input type='checkbox' class='checkbox'></td>" +
            "<td>" + ErpSeqNo + "</td>" +
            "<td>" + ItemId + "</td>" +
            "<td>" + BiwNo + "</td>" +
            "<td>" + Vcode + "</td>" +
            "<td> <button class='Chenge' data-id='" + FF + "' style='color: white; border:none'>0</button> </td>" +
            "<td><button class='Chenge' data-id='" + FE + "' style='color: white; border:none'>0</button> </td>" +
            "<td><button class='Chenge' data-id='" + RF + "' style='color: white; border:none'>0</button> </td>" +
            "<td> <button class='Chenge' data-id='" + BSRH + "' style='color: white; border:none'>0</button></td>" +
            "<td> <button class='Chenge' data-id='" + BSLH + "' style='color: white; border:none'>0</button></td>" +
            "</tr>");

        $(".Chenge").each(function () {
            if ($(this).attr("data-id") == 0) {
                $(this).css('background-color', '#337ab7'); // blue Schedule
            }
            if ($(this).attr("data-id") == 1) {
                $(this).css('background-color', '#999966'); // militry Received
            }
            if ($(this).attr("data-id") == 2) {
                $(this).css('background-color', '#777777'); // gray started
            }
            if ($(this).attr("data-id") == 3) {
                $(this).css('background-color', '#f0ad4e'); // orange process
            }
            if ($(this).attr("data-id") == 4) {
                $(this).css('background-color', '#5cb85c'); // green Transmitted
            }
            if ($(this).attr("data-id") == 100) {
                $(this).css('background-color', '#5bc0de'); // skyblue Breakpoint
            }
            if ($(this).attr("data-id") == 101) {
                $(this).css('background-color', '#d9534f'); // red Suspended
            }
            if ($(this).attr("data-id") == 102) {
                $(this).css('background-color', '#000000'); // Black Abounded
            }
        });

        totalCountFromTotalData = totalData.reduce(function (count, item) {
            if (item.ff === 0 && item.fe === 0 && item.rf === 0 && item.bslh === 0 && item.bsrh === 0) {
                return count + 1;
            }
            return count;
        }, 0);
    });
   // debugger;
    $("#countDisplay").text(`Pending Orders: ${totalCountFromTotalData}`);
    generatePageButtons();
}

function generatePageButtons() {
    $('#TRFBtnContainer').empty();
    $('#buttonContainer').empty();
    $('#RowcountDisplay').empty();
    const pageButtons = $('#pageButtons');
    pageButtons.empty();

    var totalPages = Math.ceil(totalData.length / rowsPerPage);

    for (var i = 0; i < totalPages; i++) {
        var page = i;
        var button = $('<li><a href="#" class="page-button" data-page="' + page + '">' + (page + 1) + '</a></li>');
        pageButtons.append(button);
    }
    pageButtons.find('a[data-page="' + currentPage + '"]').addClass('current-page');
}
////#endregion//
///






////#region SelectRow Configuration
var ErpSeqArray = [];
var ErpSeqArray2 = [];
$(document).ready(function () {
    var table = $('#Table').DataTable();
    var selectedRowCount = 0;
    var btnTRFToPreproduction;
    var buttonAbound;
    var btnSwap;
    var btnMultipleSwap;
    var btnSwap2;
    var enableMultiSwap = false;

    $('#Table tbody').on('click', 'tr', function () {
        stopReload();
        //console.log(ErpSeqArray);
        if (event.target.type !== 'checkbox') {
            var checkbox = $(this).find('.checkbox');
            checkbox.prop('checked', !checkbox.prop('checked'));
        }
        $(this).toggleClass('selected');

        selectedRowCount = $('#Table tbody tr.selected').length;
        $("#RowcountDisplay").text(`Order Selected: ${selectedRowCount}`);
      //  console.log(selectedRowCount);

       
        // #region transferToPre-production.


        btnTRFToPreproduction = $('<button/>', {
            class: 'btn btn-primary',
            text: 'Transfer To PreProduction',
            click: function () {
                var confirmed = confirm('Are you sure you want to Transfer to PreProduction these orders?');
                if (confirmed) {
                    $.ajax({
                        type: 'GET',
                        url: '/DataVisulization/GetLineStatus',
                        dataType: 'json',
                        success: function (data) {
                            var linestatus = data;
                            //console.log(linestatus);

                            if (linestatus == true) {
                                $.ajax({
                                    type: 'POST',
                                    url: '/DataVisulization/TRFToPreProduction',
                                    dataType: 'json',
                                    contentType: 'application/json',
                                    data: JSON.stringify(ErpSeqArray),
                                    success: function (data) {
                                        if (data.status === "Executed") {
                                            alert('Selected orders has been revert back to pre-production.');

                                            window.location.href = '/DataVisulization/PreProductionOrders';
                                        }
                                        if (data.status === "error") {
                                            alert('Order Already Executed You Can Not Revert Back');
                                            alert('Refresh Page ');
                                            location.reload();
                                        }

                                        if (data.status === "Unauthorized") {
                                            alert(data.message);
                                            location.reload();
                                        }
                                    },
                                });
                            }
                            else {
                                alert('You Need To Stop All Lines');
                                $('#TRFBtnContainer').empty();
                                $('#buttonContainer').empty();
                                $('#RowcountDisplay').empty();
                            }
                            startReload();
                        }
                    });
                }
                else {
                    alert('Order Transfer to PreProduction canceled.');
                    startReload();
                }
            }
        });
                      //#endregion
        


        buttonAbound = $('<button>', {
            class: 'btnAbound',
            html: '<i class="fa fa-trash"></i>',
            click: function () {

                var confirmed = confirm('Are you sure you want to Abound this order?');
                if (confirmed) {
                    $.ajax({
                        type: "POST",
                        url: "/DataVisulization/SetAbandoned",
                        dataType: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify(ErpSeqArray),
                        success: function (data) {
                            if (data.status === "Set Abandoned") {
                                alert('Order Abandoned.');

                            }
                            if (data.status === "error") {
                                alert('Refresh Page ');
                                location.reload();
                            }
                            if (data.status === "error1") {
                                alert('Order Already Executed You Can Not Abandon Order');
                                location.reload();
                            }
                            if (data.status === "Unauthorized") {
                                alert(data.message);
                                location.reload();
                            }
                            startReload();
                        },

                    });

                    // alert('Order Abandoned.');
                }
                else {
                    alert('Order abandonment canceled.');
                    startReload();
                }
                $('#buttonContainer').empty();
                $('#TRFBtnContainer').empty();
                $('#RowcountDisplay').empty();

            }
        });

        //As discuss with gandhar hide swap button on 18/06/2025
        btnSwap = $('<button/>', {
            class: 'swp',
            html: 'SWAP',
            /* html: '<span style="font-size: 17px;">&#x1F504;</span> Swap',*/
            click: function () {
                var confirmed = confirm('Are you sure you want to Swap this orders?');
                if (confirmed) {
                    $.ajax({
                        type: "POST",
                        url: "/DataVisulization/SwapOrder",
                        dataType: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify(ErpSeqArray),
                        success: function (data) {
                            if (data.status === "Executed") {
                                alert('Order Swap Successfully.');
                            }
                            if (data.status === "error") {
                                alert('Refresh Page ');
                                location.reload();
                            }
                            if (data.status === "error1") {
                                alert('Order Already Executed You Can Not Swap Order');
                                location.reload();
                            }
                            if (data.status === "Unauthorized") {
                                alert(data.message);
                                location.reload();
                            }
                            startReload();
                        },

                    });
                }
                else {
                    alert('Order Swap canceled.');
                    startReload();
                }
                $('#buttonContainer').empty();
                $('#TRFBtnContainer').empty();
                $('#RowcountDisplay').empty();
            }
        });




        if (selectedRowCount === 1) {
            deselectRowsAndRemoveFromErpSeqArray();

            var selectedRow = $('#Table tbody tr.selected');

            var ErpSeqNo = selectedRow.find('td:eq(1)').text();
            var ItemId = selectedRow.find('td:eq(2)').text();
            var BiwNo = selectedRow.find('td:eq(3)').text();
            var FF = selectedRow.find('td:eq(5) button').attr('data-id');
            var FE = selectedRow.find('td:eq(6) button').attr('data-id');
            var RF = selectedRow.find('td:eq(7) button').attr('data-id');
            var BSRH = selectedRow.find('td:eq(8) button').attr('data-id');
            var BSLH = selectedRow.find('td:eq(9) button').attr('data-id');

            //  var selectedRows = dataTable.rows('.selected').data();
            if (!ErpSeqArray.includes(ErpSeqNo)) {
                ErpSeqArray.push(ErpSeqNo);
            }

            if (
                (FF === '0' || FF === '100' || FF === '101') &&
                (FE === '0' || FE === '100' || FE === '101') &&
                (RF === '0' || RF === '100' || RF === '101') &&
                (BSLH === '0' || BSLH === '100' || BSLH === '101') &&
                (BSRH === '0' || BSRH === '100' || BSRH === '101')

            ) {

                // Create a button element with  attributes
                var buttonBrkPoint = $('<button>', {
                    text: '\u2719 BREAKPOINT',
                    class: 'brkbutton',
                    click: function () {
                        
                        var confirmed = '';
                        if ($(buttonBrkPoint).html() == '✙ BREAKPOINT') {
                            confirmed = confirm('Are you sure, you want to Perform Breakpoint Operation?');
                        } else {                            
                            confirmed = confirm('Are you sure, you want to release Breakpoint Operation?');
                        }
                        if (confirmed) {
                            $.ajax({
                                type: "POST",
                                dataType: "json",
                                data: {
                                    "ErpSeqNo": ErpSeqNo,
                                    "ItemId": ItemId,
                                    "BiwNo": BiwNo,
                                    "FF": FF,
                                    "FE": FE,
                                    "RF": RF,
                                    "BSLH": BSLH,
                                    "BSRH": BSRH
                                },
                                url: "/DataVisulization/SetBreakPoint",
                                success: function (data) {
                                    if (data.status === "SetBreakpoint") {
                                        alert('Set Breakpoint');
                                    }
                                    if (data.status === "ReleseBreakPoint") {
                                        alert('Relese Breakpoint');
                                    }
                                    if (data.status === "error") {
                                        alert('Refresh Page ');
                                        location.reload();
                                    }
                                    if (data.status === "error1") {
                                        alert('Order Already Executed You Can Not Set BREAKPOINT');
                                        location.reload();
                                    } if (data.status === "Unauthorized") {
                                        alert(data.message);
                                        location.reload();
                                    }

                                    startReload();
                                },

                            });
                            // alert('Set Breakpoint');
                            $('#buttonContainer').empty();
                            $('#TRFBtnContainer').empty();
                            $('#RowcountDisplay').empty();
                        }
                        else {
                            alert('Breakpoint Operation canceled.');
                            startReload();
                        }

                    }
                });
                if (FF === '100' && FE === '100' && RF === '100' && BSLH === '100' && BSRH === '100') {

                    $(buttonBrkPoint).text('\u2212 RELEASE BREAKPOINT');
                } else {

                    $(buttonBrkPoint).text('\u2719 BREAKPOINT');
                }

                //var buttonAbound = $('<button>', {
                //    class: 'btnAbound',
                //    html: '<i class="fa fa-trash"></i>',
                //    click: function () {

                //        var confirmed = confirm('Are you sure you want to Abound this order?');
                //        if (confirmed) {
                //            $.ajax({
                //                type: "POST",
                //                url: "/DataVisulization/SetAbandoned",
                //                dataType: "json",
                //                data: {
                //                    "ErpSeqNo": ErpSeqNo,
                //                    "ItemId": ItemId,
                //                    "BiwNo": BiwNo,
                //                    "FF": FF,
                //                    "FE": FE,
                //                    "RF": RF,
                //                    "BSLH": BSLH,
                //                    "BSRH": BSRH
                //                },
                //                success: function (data) {
                //                    if (data.status === "Set Abandoned") {
                //                        alert('Order Abandoned.');
                //                    }
                //                    if (data.status === "error") {
                //                        alert('Refresh Page ');
                //                        location.reload();
                //                    }
                //                    if (data.status === "error1") {
                //                        alert('Order Already Executed You Can Not Abandon Order');
                //                        location.reload();
                //                    }
                //                    startReload();
                //                },

                //            });

                //            // alert('Order Abandoned.');
                //        }

                //        $('#buttonContainer').empty();
                //        $('#TRFBtnContainer').empty();
                //        $('#RowcountDisplay').empty();
                //    }
                //});



               // debugger;
                // Append the button to a div outside the table
                if (FF === '100' && FE === '100' && RF === '100' && BSRH === '100' && BSLH === '100') {
                    $('#buttonContainer').empty().append(buttonBrkPoint);
                }
                else if (FF === '101' && FE === '101' && RF === '101' && BSRH === '101' && BSLH === '101') {
                    $('#buttonContainer').empty().append(buttonAbound);
                    $('#TRFBtnContainer').empty().append(btnTRFToPreproduction);
                }
                else {
                    $('#buttonContainer').empty().append(buttonAbound, buttonBrkPoint);
                    $('#TRFBtnContainer').empty().append(btnTRFToPreproduction);
                }
            } else {

                $('#buttonContainer').empty();
            }


        }

        else if (selectedRowCount >= 1) {
            //if (enableMultiSwap == false) {
            //    $('#MultswpBtnContainer').empty().append(btnMultipleSwap);
            //}


            var allZeroValues = true;
            var checkValueForAbound = true;

            var rows = $('#Table tbody tr.selected');
            //console.log(rows);

            if (selectedRowCount >= 1) {
                var selectedRows = $('#Table tbody tr.selected');

                selectedRows.each(function () {
                    var ErpSeqNo = $(this).find('td:eq(1)').text();
                    //var ItemId = $(this).find('td:eq(2)').text();
                    //var BiwNo = $(this).find('td:eq(3)').text();
                    var FF = $(this).find('td:eq(5) button').attr('data-id');
                    var FE = $(this).find('td:eq(6) button').attr('data-id');
                    var RF = $(this).find('td:eq(7) button').attr('data-id');
                    var BSRH = $(this).find('td:eq(8) button').attr('data-id');
                    var BSLH = $(this).find('td:eq(9) button').attr('data-id');


                    if (FF !== '0' || FE !== '0' || RF !== '0' || BSRH !== '0' || BSLH !== '0') {
                        allZeroValues = false;

                    }
                    if (FF !== '101' || FE !== '101' || RF !== '101' || BSRH !== '101' || BSLH !== '101') {
                        checkValueForAbound = false;

                    }


                    if (!ErpSeqArray.includes(ErpSeqNo)) {
                        ErpSeqArray.push(ErpSeqNo);
                    }
                });

            }

            deselectRowsAndRemoveFromErpSeqArray();


            if (allZeroValues == true) {
                $('#TRFBtnContainer').empty().append(btnTRFToPreproduction);
                $('#buttonContainer').empty().append(buttonAbound);
                //if (selectedRowCount === 2) {
                //     $('#buttonContainer').append(btnSwap);
                //}
            }
            else {
                $('#TRFBtnContainer').empty();
                $('#buttonContainer').empty();
            }

            if (checkValueForAbound == true) {
                $('#buttonContainer').empty().append(buttonAbound);
                $('#TRFBtnContainer').empty().append(btnTRFToPreproduction);
            }


        }
        else if (selectedRowCount === 0) {
            $('#TRFBtnContainer').empty();
            $('#buttonContainer').empty();
            $('#RowcountDisplay').empty();
            startReload();
        }
        //   console.log(selectedRowCount);

    });

});
function deselectRowsAndRemoveFromErpSeqArray() {
    var deselectedRows = $('#Table tbody tr').not('.selected');
    deselectedRows.each(function () {
        var ErpSeqNo1 = $(this).find('td:eq(1)').text();
        var index = ErpSeqArray.indexOf(ErpSeqNo1);
        if (index !== -1) {
            ErpSeqArray.splice(index, 1);
        }
    });
}
        //// #endregion