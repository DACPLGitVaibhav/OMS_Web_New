
function ShowModal(URL, strModelTitle, LargeModel) {
    //debugger;
   
    if (LargeModel == 'lg') {
        $('.modal-dialog').addClass('modal-lg');
    }
    else if (LargeModel == 'xl') {
        $('.modal-dialog').addClass('modal-xl');
    }
    else if (LargeModel == 'fullscreen') {
        $('.modal-dialog').addClass('modal-fullscreen');
    }
    else {
        $('.modal-dialog').addClass('');
    }
    $.get(URL,
        function (strHtml) {
           
            $(".modal-title").html(strModelTitle);
            $(".modal-body").html(strHtml);

        });
}


function ShowToaster(StrMessage) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-right',
        showConfirmButton: false,
        timer: 10000,

    });
    //debugger;
    if (StrMessage.toString().trim().includes("Something")) {
        Toast.fire({
            type: 'error',
            title: StrMessage
        })
    }
    else {
        Toast.fire({
            type: 'success',
            title: StrMessage
        })
    }
}
