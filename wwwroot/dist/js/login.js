function Validate() {
    $('#modal-loader').modal({ backdrop: 'static', keyboard: false });

    $('#modal-loader').modal('show')
    var url = $('#btn_login').data('request-url');
    const captchaUrl = $('#btn_login').data('captcha-url');
    var rem = $('#rem').prop("checked") ? 1 : 0;
    var dataToSend = {
        pno: $('#pno').val(),
        password: $('#password').val(),
        rem: rem,
        app: $('#app').children("option:selected").val(),
        captcha: $('#captcha').val()
    }
    $.ajax(
        {
            type: "POST",
            url: url,
            data: dataToSend,
            datatype: "json",
            error: function (result) {
                swal("There is a Problem, Try Again!", "Please re-enter!", "error");

            },
            success: function (result) {
                $('#modal-loader').modal('hide');
                if (result.status == true) {
                    swal("You are logged in!", "Please wait while we redirect you...", "success");

                    setTimeout(function () {// wait for 5 secs(2)
                        window.location.href = '../Home/Index';
                        // then reload the page.(3)
                    }, 2000);
                }
                else {
                    swal(result.message, "Please re-enter!", "error");
                    refreshCaptcha();
                }
            }
        });
}
function Validate2() {
    $('#modal-loader').modal({ backdrop: 'static', keyboard: false });

    $('#modal-loader').modal('show')

    var url = $('#btn_login2').data('request-url');
    var rem = $('#rem').prop("checked") ? 1 : 0;

    if ($('#pno').val()) {
        var dataToSend = {
            pno: $('#pno').val(),
            password: $('#password').val(),
            rem: rem,
            app: $('#app').children("option:selected").val(),
            captcha: $('#captcha').val()
        }
        $.ajax(
            {
                type: "POST",
                url: url,
                data: dataToSend,
                datatype: "json",
                error: function (result) {
                    $('#modal-loader').modal('hide')

                    swal("There is a Problem, Try Again!", "Please re-enter!", "error");

                },
                success: function (result) {
                    $('#modal-loader').modal('hide')
                    if (result.status == true) {
                        swal("Success", "Password has been sent to your Registered Email ID.", "success");
                    }
                }
            });
    }
    else {
        $('#modal-loader').modal('hide')

        swal("Fail", "Please Enter PNo.", "error");

    }
}


async function refreshCaptcha() {
    const url = $('#token-url').data('token-url');
    fetch(url)
        .then(res => res.text()
            .then(data => {
                $('#captcha-image').attr('src', "data:image/png;base64, " + data);
            }))
}


$(document).ready(async function () {
    await refreshCaptcha()
});