﻿var reloadInterval;
var totalData = [];
$(document).ready(function () {
   
    startReload()
    

    $('#Table').DataTable({
        dom: 'Blfrtip',
        "ordering": false,
        // dom: '<"top"i>rt<"bottom"flp><"clear">',
        buttons: [
                                                                                                                                                                                                                                                                                                                                                                            /*'copy', 'csv',*/ 'excel'
        ],
        //lengthMenu: [
        //    [100, 200, 500, -1],
        //    [100, 200, 500, 'All']
        //],
       
        columnDefs: [{
            orderable: false,
            className: 'select-checkbox',
            targets: 0
        }],
        select: {
            style: 'multi',
            selector: 'td:first-child'
            /* selector: 'td:first-child input[type="checkbox"]'*/
        },

        "paging": false

    });

    $('#select-all-checkbox').on('click', function () {
        if ($(this).is(':checked')) {

            table.rows({ page: 'current' }).select();
        } else {

            table.rows().deselect();
        }
    });

    var selectedData = [];
    var customDataArray = [];
    var table = $('#Table').DataTable();
    var buttonContainer = $('#buttonContainer');
    var divDelete = $('#DivDelete');


    // #region Code update by Vaibhav 27/12/2024  as per after Demo
    

    $('#btnselect').click(function () {

        if (($('#fromPpseq').val() == 0 || $('#fromPpseq').val() == '') || ($('#toPpseq').val() == 0 || $('#toPpseq').val() == '')) {
            alert('Enter Valid PPSeqNo!');
            location.reload();
            return;
        }

        //if ($('#fromPpseq').val() > $('#toPpseq').val()) {
        //    alert('From PPSeqNo should not be greater than To PPSeqNo.');
        //    return;
        //}

        let startSRNo = $('#fromPpseq').val();
        let endSRNo = $('#toPpseq').val();

        $('#Table tbody tr').each(function (index, item) {   
            $(this).removeClass('selected');


            let x = parseInt($(this).find('#PPSeqNo').text());
            //debugger;
            if (x >= startSRNo && x <= endSRNo) {
                $(this).addClass('selected').data();               
            }

        });

        SelectedRowCountToDisplay();
        
        if (table.rows('.selected').count() > 0) {
            //debugger;
            var selectedRows = table.rows('.selected').data(); // Get selected rows data
            selectedData = [];
            // Iterate through the selected rows and add them to the selectedData array
            selectedRows.each(function (index, data) {
                // Check if the data is not already in the selectedData array
                if (!isSelected(data)) {
                    selectedData.push(index);
                }
            });

            updateCustomDataArray();

            var btntrfProdcuction = $('<button>', {
                class: 'btn btn-primary',
                text: 'TRF Production',
                click: function () {
                    var confirmed = confirm('Are you sure you want to Transfer to Production this orders?');
                    if (confirmed) {
                        $.ajax({
                            type: 'POST',
                            url: '/DataVisulization/TRFtoProduction',
                            dataType: 'json',
                            contentType: 'application/json',
                            data: JSON.stringify(customDataArray),
                            success: function (data) {
                                if (data.status === "DataRecived") {
                                    alert('Production Start');
                                    /*table.rows().deselect();*/
                                    window.location.reload();
                                    window.location.href = '/DataVisulization/Index';
                                }
                                if (data.status === "error") {
                                    alert('Refresh Page ');
                                    location.reload();
                                }
                                if (data.status === "Unauthorized") {
                                    alert(data.message);
                                    location.reload();
                                }

                            },
                        });
                    } else {
                        alert('Order Transfer to Production canceled.');
                        location.reload();
                    }
                },
            });

            buttonContainer.empty().append(btntrfProdcuction);
        } else {
            buttonContainer.empty();
        }

    })

    //#endregion

    
    function updateCustomDataArray() {

        customDataArray = selectedData.map(function (rowData) {
            return {
                PPSeqNo: parseInt(rowData[1]),
                ItemId: rowData[2],
                BiwNo: rowData[3],
                Vcode: rowData[4],
                ModelCode: rowData[5],
            };
        });
    }

    table.on('select', function (e, dt, type, indexes) {
        if (type === 'row') {

            SelectedRowCountToDisplay();

            var selectedRows = table.rows('.selected').data(); // Get selected rows data
            selectedData = [];
            // Iterate through the selected rows and add them to the selectedData array
            selectedRows.each(function (index, data) {
                // Check if the data is not already in the selectedData array
                if (!isSelected(data)) {
                    selectedData.push(index);
                }
            });

            //var selectedRowData = table.row(indexes).data();
            //if (!selectedData.includes(selectedRowData)) {
            //    selectedData.push(selectedRowData);
            //}

            updateCustomDataArray();
           // console.log(selectedData);
           // console.log(customDataArray);
        }

        if (selectedData.length > 0) {
            var btntrfProdcuction = $('<button>', {
                class: 'btn btn-primary',
                text: 'TRF Production',
                click: function () {
                    var confirmed = confirm('Are you sure you want to Transfer to Production this orders?');
                    if (confirmed) {
                        $.ajax({
                            type: 'POST',
                            url: '/DataVisulization/TRFtoProduction',
                            dataType: 'json',
                            contentType: 'application/json',
                            data: JSON.stringify(customDataArray),
                            success: function (data) {
                                if (data.status === "DataRecived") {
                                    alert('Production Start');
                                    /*table.rows().deselect();*/
                                    window.location.reload();
                                    window.location.href = '/DataVisulization/Index';
                                }
                                if (data.status === "error") {
                                    alert('Error Refresh Page ');
                                    location.reload();
                                }
                                if (data.status === "Unauthorized") {
                                    alert(data.message);
                                    location.reload();
                                }

                            },
                        });
                    } else {
                        alert('Order Transfer to Production canceled.');
                        location.reload();
                    }
                },
            });

            buttonContainer.empty().append(btntrfProdcuction);
        }

        // #region Abonded Button
        //if (selectedData.length > 0) {
        //    buttonAbound = $('<button>', {
        //        class: 'btnAbound',
        //        html: '<i class="fa fa-trash"></i>',
        //        // text: 'TRF Production',
        //        click: function () {

        //            var confirmed = confirm('Are you sure you want to Abound this order?');
        //            if (confirmed) {
        //                $.ajax({
        //                    type: "POST",
        //                    url: "",
        //                    dataType: 'json',
        //                    contentType: 'application/json',
        //                    data: JSON.stringify(),
        //                    success: function (data) {
        //                        if (data.status === "Set Abandoned") {
        //                            alert('Order Abandoned.');

        //                        }
        //                    }
        //                });
        //            }
        //            else {
        //                alert('Order abandonment canceled.');
        //                startReload();
        //            }


        //        }
        //    });

        //    divDelete.empty().append(buttonAbound);
        //}
        //#endregion

    });

    table.on('deselect', function (e, dt, type, indexes) {

        if (type === 'row') {
            SelectedRowCountToDisplay();
            var unselectedRows = table.rows('.selected').data();
            selectedData = [];
            unselectedRows.each(function (index, data) {

                // Check if the data is not already in the selectedData array
                if (!isSelected(data)) {

                    selectedData.push(index);
                }
            });
           // console.log(selectedData);
            updateCustomDataArray();

            if (selectedData.length === 0) {
                buttonContainer.empty();
            }
        }
    });



    function isSelected(data) {
        for (var i = 0; i < selectedData.length; i++) {
            if (JSON.stringify(selectedData[i]) === JSON.stringify(data)) {
                return true;
            }
        }
        return false;
    }

    function SelectedRowCountToDisplay() {
        var selectedRowCount = table.rows('.selected').count();
        $("#RowcountDisplay").text(`Selected ${selectedRowCount}`).append(' of total '+ $('#Table tbody tr').length);
    }
    preback();



   

});

function startReload() {

    reloadTable();
    reloadInterval = setInterval(function () {
        reloadTable();
    }, 5000);
}
function reloadTable() {
    $.ajax({
        url: '/DataVisulization/CheckIsAutoMode',
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            if (data == true) {
                $('#divFilter').hide();

                


                $.ajax({
                    url: '/DataVisulization/ReloadPreProOrder',
                    type: 'GET',
                    dataType: 'json',
                    success: function (dataToDisplay) {
                       // debugger;

                        totalData = dataToDisplay
                        //renderTable();

                        const tableBody = $('#Table tbody');
                        tableBody.empty();

                        var dataToDisplay = totalData
                        dataToDisplay.forEach(function (item) {

                            tableBody.append("<tr><td></td>" +
                                "<td id='PPSeqNo'>" + item.ppSeqNo + "</td>" +
                                "<td>" + item.itemId + "</td>" +
                                "<td>" + item.biwNo + "</td>" +
                                "<td>" + item.vcode + "</td>" +
                                "<td>" + item.modelCode + "</td>" +
                                "<td>" + item.fileName + "</td>" +
                                "<td>" + forDateTime(item.dateIimport) + "</td>" +

                                "</tr>");

                            //if (item.ppSeqNo > 20) {
                            //    tableBody.append("<tr style='background-color:red;'><td></td>" +
                            //        "<td id='PPSeqNo'>" + item.ppSeqNo + "</td>" +
                            //        "<td>" + item.itemId + "</td>" +
                            //        "<td>" + item.biwNo + "</td>" +
                            //        "<td>" + item.vcode + "</td>" +
                            //        "<td>" + item.modelCode + "</td>" +
                            //        "<td>" + item.fileName + "</td>" +
                            //        "<td>" + forDateTime(item.dateIimport) + "</td>" +

                            //        "</tr>");
                            //} else {
                            //    tableBody.append("<tr><td></td>" +
                            //        "<td id='PPSeqNo'>" + item.ppSeqNo + "</td>" +
                            //        "<td>" + item.itemId + "</td>" +
                            //        "<td>" + item.biwNo + "</td>" +
                            //        "<td>" + item.vcode + "</td>" +
                            //        "<td>" + item.modelCode + "</td>" +
                            //        "<td>" + item.fileName + "</td>" +
                            //        "<td>" + forDateTime(item.dateIimport) + "</td>" +

                            //        "</tr>");
                            //}

                          

                        });
                    },

                    error: function (error) {
                       // console.error('Error fetching data:', error);
                    }
                });


            } else {
                $('#divFilter').show();
                clearInterval(reloadInterval);
            }
        },

        error: function (error) {
           // console.error('Error fetching data:', error);
        }
    });
}

function forDateTime(date) {
    var fromorTodate;
    var d = new Date(date.split("/").reverse().join("-"));
    var dd = `${(d.getDate())}`.padStart(2, '0');
    var mm = `${(d.getMonth() + 1)}`.padStart(2, '0');
    var yy = d.getFullYear();
    var h = d.getHours();
    var m = d.getMinutes();
    var s = d.getSeconds();
    fromorTodate = dd + "/" + mm + "/" + yy + " " + h + ":" + m + ":" + s;

    return fromorTodate;
}
function preback() {
    function preback() { window.history.forward(); }
    setTimeout("preback()", 0);
    window.onunload = function () { null };
}