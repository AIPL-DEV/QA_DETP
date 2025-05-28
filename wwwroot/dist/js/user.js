var url = $('#delete-user').data('request-url');
var url2 = $('#edit-user').data('request-url');
var url3 = $('#resetbtn').data('request-url');
var url4 = $('#submit_request3').data('request-url');


function ResetPwd(user_id) {
    $('#modal-loader').modal('show')


    var dataToSend = {
        user_id: user_id
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
                console.log(result);
                if (result.status == true) {
                    console.log(result);
                    $('#modal-loader').modal('hide')
                    $('#modal-success2').modal('show')
                }
                else {
                    alert(result.message);
                }
            }
        });
}

function Update(user_id) {


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
                     $("#id").val(result.id);
                    $("#pno").val(result.pno);
                $("#name").val(result.name);
                $("#email").val(result.email);
                $('select[name="dept"]:first').val(result.dept) 
                $('select[name="role"]:first').val(result.role) 

                    $('#modal-edit').modal('show')
                
            }
        });
}


function DoUpdate() {

    $('#modal-edit').modal('hide')

    $('#modal-loader').modal('show')


    var dataToSend = {
        id: $('#id').val(),
        pno: $('#pno').val(),
        name: $('#name').val(),
        email: $('#email').val(),
        dept: $('#dept').val(),
        role: $('#role').val()
    }
    $.ajax(
        {
            type: "POST",
            url: url4,
            data: dataToSend,
            datatype: "json",
            error: function (result) {
                alert("There is a Problem, Try Again!");
            },
            success: function (result) {
                $('#modal-loader').modal('hide')
                $('#modal-success3').modal('show')
                setTimeout(function () {// wait for 5 secs(2)
                    location.reload(); // then reload the page.(3)
                }, 3000);

            }
        });
}


function Submit(user_id) {

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

