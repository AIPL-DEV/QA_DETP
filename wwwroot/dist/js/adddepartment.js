var url = $('#submit_request').data('request-url');
var url3 = $('#submit_request2').data('request-url');
var url2 = $('#edit-btn').data('request-url');


function DoUpdate() {
    $('#modal-edit').modal('hide')

    $('#modal-loader').modal('show')


    var dataToSend = {
        abb: $('#abb').val(),
        name: $('#name').val(),
        id: $('#id').val()
    }
    $.ajax(
        {
            type: "POST",
            url: url3,
            data: dataToSend,
            datatype: "json",
            error: function (result) {
                alert("There is a Problem, Try Again!");
            },
            success: function (result) {
                $('#modal-loader').modal('hide')
                $('#modal-success2').modal('show')
                setTimeout(function () {// wait for 5 secs(2)
                    location.reload(); // then reload the page.(3)
                }, 3000);
            }
        });
}


function Update(user_id) {
    $('#modal-loader').modal('show')


    var dataToSend = {
        user_id: user_id
    }
    $.ajax(
        {
            type: "POST",
            url: url2,
            data: dataToSend,
            datatype: "json",
            error: function (result) {
                alert("There is a Problem, Try Again!");
            },
            success: function (result) {

                console.log(result);
                $("#abb").val(result.department_abbr);
                $("#name").val(result.department_name);
                $("#id").val(result.department_id);
                $('#modal-loader').modal('hide')
                $('#modal-edit').modal('show')

            }
        });
}

$(document).ready(function () {
    //Jquery validation
    $('#myform').validate({
        rules: {
            division_id: {
                required: true
            },
            department_abbr: {
                required: true
            },
            department_name: {
                required: true
            }
        },
        messages: {
            division_id: {
                required: "This field is required"
            },
            department_abbr: {
                required: "This field is required"
            },
            department_name: {
                required: "This field is required"
            }
        },
        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass('invalid-feedback');
            element.closest('.form-group').append(error);
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass('is-invalid');
        }
    });
})

function Submit() {
    if ($('#myform').valid()) {
        $('#modal-loader').modal('show')
        $('#submit_request').prop('disabled', true)

        var dataToSend = {
            division_id: $('#division_id').val(),
            department_abbr: $('#department_abbr').val(),
            department_name: $('#department_name').val(),
        }
        console.log(dataToSend);
        return;
        $.ajax(
            {
                type: "POST",
                url: url,
                data: dataToSend,
                datatype: "json",
                error: function (result) {
                    alert("There is a Problem, Try Again!");
                },
                success: function (result) {
                    if (result.status == true) {
                        $('#modal-loader').modal('hide')
                        $('#submit_request').prop('disabled', false)
                        $('#myform').trigger('reset')
                        $('#visit_no').text(result.visit_no)
                        $('#modal-success').modal('show')
                    }
                    else {
                        alert(result.message);
                    }
                }
            });
    }
}
function Delete(user_id) {

    $('#modal-loader').modal('show')
    $('#submit_request').prop('disabled', true)

    var dataToSend = {
        user_id: user_id
    }
    $.ajax(
        {
            type: "POST",
            url: url,
            data: dataToSend,
            datatype: "json",
            error: function (result) {
                alert("There is a Problem, Try Again!");
            },
            success: function (result) {
                if (result.status == true) {
                    $('#modal-loader').modal('hide')
                    $('#modal-success').modal('show')
                    setTimeout(function () {// wait for 5 secs(2)
                        location.reload(); // then reload the page.(3)
                    }, 3000);
                }
                else {
                    alert(result.message);
                }
            }
        });
}