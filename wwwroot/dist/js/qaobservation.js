$('input[id=ystandard]').click(function () {
    $('input[id=nbasics]').prop('checked', true)
    $('input[id=ybasics]').prop('checked', false)

});
$('input[id=nstandard]').click(function () {
    $('input[id=ybasics]').prop('checked', true)
    $('input[id=nbasics]').prop('checked', false)

});

var img_array = [];
function getBase64(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        img_array.push(reader.result);
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

$(function () {
    $('#fileInput').change(function () {
        img_array = [];
        var files = document.getElementById('fileInput').files;
        for (var i = 0; i < files.length; i++) {
            if (files.length > 0) {

                var ext = files[i].name.substr(-3);
                if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "pdf" || ext == "doc" || ext == "docx" || ext == "xls" || ext == "xlsx" || ext == "ppt" || ext == "pptx") {
                    getBase64(files[i]);
                }


            }
        }
        console.log(img_array);
    });
});


$(document).ready(function () {
    /*$('.select2').select2();*/
    //Date range picker
    //$('.good').css('display', 'none');
    //$('.non').css('display', 'none');




    //$('#type_of_observation').on('change', function () {
    //    if (this.value == "Non Confirmnace") {
    //        $('.non').css('display', 'block');
    //        $('.good').css('display', 'none');
    //        $('.placeholder_confirmnace').css('display', 'none');
    //    }
    //    else if (this.value == "Good Observation") {
    //        $('.non').css('display', 'none');
    //        $('.good').css('display', 'block');
    //        $('.placeholder_confirmnace').css('display', 'none');
    //    }
    //    else {
    //        $('.non').css('display', 'none');
    //        $('.good').css('display', 'none');
    //        $('.placeholder_confirmnace').css('display', 'block');
    //    }
    //});


    //$('#compliance_target_date').datepicker({
    //    format: 'L',
    //    minDate: new Date()
    //});
    //$('#observation_date_picker').datepicker({
    //    format: 'L',
    //    minDate: new Date()

    //});
    //Jquery validation
    //$('#myform').validate({
    //    rules: {
    //        division_id: {
    //            required: true
    //        },
    //        department: {
    //            required: true
    //        },
    //        site: {
    //            required: true
    //        },
    //        location: {
    //            required: true
    //        },
    //        observation_remarks: {
    //            required: true
    //        },
    //        log_non_confirmance: {
    //            required: true
    //        },
    //        compliance_target_date: {
    //            required: true
    //        },
    //        type_of_observation: {
    //            required: true
    //        },
    //        type_of_confirmance: {
    //            required: function (element) {
    //                return $("#type_of_observation").val() == "Non Confirmnace";
    //            }
    //        },
    //        nature_of_confirmance: {
    //            required: function (element) {
    //                return $("#type_of_observation").val() == "Non Confirmnace";
    //            }
    //        },
    //        vendor_code: {
    //            required: true
    //        },
    //        vendor_name: {
    //            required: true
    //        },
    //        p_o_no: {
    //            required: true
    //        },
    //        observation_by: {
    //            required: true
    //        },
    //        observation_date: {
    //            required: true
    //        }
    //    },
    //    messages: {
    //        division_id: {
    //            required: "This field is required"
    //        },
    //        department: {
    //            required: "This field is required"
    //        },
    //        site: {
    //            required: "This field is required"
    //        },
    //        location: {
    //            required: "This field is required"
    //        },
    //        observation_remarks: {
    //            required: "This field is required"
    //        },
    //        log_non_confirmance: {
    //            required: "This field is required"
    //        },
    //        compliance_target_date: {
    //            required: "This field is required"
    //        },
    //        vendor_code: {
    //            required: "This field is required"
    //        },
    //        vendor_name: {
    //            required: "This field is required"
    //        },
    //        p_o_no: {
    //            required: "This field is required"
    //        },
    //        observation_by: {
    //            required: "This field is required"
    //        },
    //        observation_date: {
    //            required: "This field is required"
    //        }
    //    },
    //    errorElement: 'span',
    //    errorPlacement: function (error, element) {
    //        error.addClass('invalid-feedback');
    //        element.closest('.form-group').append(error);
    //    },
    //    highlight: function (element, errorClass, validClass) {
    //        $(element).addClass('is-invalid');
    //    },
    //    unhighlight: function (element, errorClass, validClass) {
    //        $(element).removeClass('is-invalid');
    //    }
    //});
})

//function onCritical(value) {
//    var fields = document.getElementsByClassName("critical_fileds");
//    if (value == "Critical") {
//        var i;
//        for (i = 0; i < fields.length; i++) {
//            fields[i].style.display = 'block';
//        }
//    } else {
//        var i;
//        for (i = 0; i < fields.length; i++) {
//            fields[i].style.display = 'none';
//        }
//    }
//}

function SubmitQA() {
    if ($('#myform').valid()) {
        $('#modal-loader').modal('show')
        $('#submit_request').prop('disabled', true)

        let myData = new FormData();
        myData.append("division_id", $('#division_id').val());
        myData.append("department", $('#department').val());
        myData.append("site", $('#site').val());
        myData.append("location", $('#location').val());
        
        myData.append("nature_of_work", $('#nature_of_work').val())
        myData.append("type_of_observation", $('#type_of_observation').val())
        myData.append("observation_remarks", $('#observation_remarks').val())
        myData.append("log_non_confirmance", $('#log_non_confirmance').val())
        myData.append("log_confirmance", $('#log_confirmance').val())
        myData.append("compliance_target_date", $('#compliance_target_date').val())
        myData.append("type_of_confirmance", $('#type_of_confirmance').val())
        myData.append("nature_of_confirmance", $('#nature_of_confirmance').val())
        myData.append("standard", $("[name=standard]:checked").val())
        myData.append("basics", $("[name=basics]:checked").val())
        myData.append("job", $("[name=jobStopped]:checked").val())
        myData.append("vendor_code", $('#vendor_code').val())
        myData.append("vendor_name", $('#vendor_name').val())
        myData.append("p_o_no", $('#p_o_no').val())
        myData.append("site_incharge", $('#site_incharge').val())
        myData.append("head_detp", $('#head_detp').val())
        myData.append("project_incharge", $('#project_incharge').val())
        myData.append("department_head", $('#department_head').val())
        myData.append("business_head", $('#business_head').val())
        myData.append("qa_officer", $('#qa_officer').val())
        myData.append("attachments", imgs)
        myData.append("observation_by", $('#observation_by').val())
        myData.append("observation_date", $('#observation_date').val())

        imgs = [];
        $('.image-upload').each(function () {
            imgs.push($(this).data('src'));
        });

        var dataToSend = {
            department: $('#department').val(),
            site: $('#site').val(),
            location: $('#location').val(),
            nature_of_work: $('#nature_of_work').val(),
            type_of_observation: $('#type_of_observation').val(),
            observation_remarks: $('#observation_remarks').val(),
            log_non_confirmance: $('#log_non_confirmance').val(),
            log_confirmance: $('#log_confirmance').val(),
            compliance_target_date: $('#compliance_target_date').val(),
            type_of_confirmance: $('#type_of_confirmance').val(),
            nature_of_confirmance: $('#nature_of_confirmance').val(),
            standard: $("[name=standard]:checked").val(),
            basics: $("[name=basics]:checked").val(),
            job: $("[name=jobStopped]:checked").val(),
            vendor_code: $('#vendor_code').val(),
            vendor_name: $('#vendor_name').val(),
            p_o_no: $('#p_o_no').val(),
            site_incharge: $('#site_incharge').val(),
            head_detp: $('#head_detp').val(),
            project_incharge: $('#project_incharge').val(),
            department_head: $('#department_head').val(),
            business_head: $('#business_head').val(),
            qa_officer: $('#qa_officer').val(),
            attachments: imgs,
            observation_by: $('#observation_by').val(),
            observation_date: $('#observation_date').val(),
        }

        if (dataToSend.site_incharge == dataToSend.qa_officer) {
            alert("Site incharge and QA Officer can't be same.");
            $('#modal-loader').modal('hide');
            return;
        }

        if (dataToSend.department_head == dataToSend.qa_officer) {
            alert("Department Head and QA Officer can't be same.");
            $('#modal-loader').modal('hide');
            return;
        }

        if (dataToSend.business_head == dataToSend.qa_officer) {
            alert("Business Head and QA Officer can't be same.");
            $('#modal-loader').modal('hide');
            return;
        }
        
        if (dataToSend.project_incharge == dataToSend.qa_officer) {
            alert("Project incharge and QA Officer can't be same.");
            $('#modal-loader').modal('hide');
            return;
        }

        $.ajax(
            {
                type: "POST",
                url: $('#submit_observation').data('request-url'),
                data: myData,
                contentType: false,
                processData: false,
                error: function (result) {
                    alert("There is a Problem, Try Again!");
                },
                success: function (result) {
                    console.log(result);
                    if (result.status == true) {
                        $('#modal-loader').modal('hide')
                        $('#submit_request').prop('disabled', false)
                        $('#myform').trigger('reset')
                        $('#visit_no').text(result.visit_no)
                        $('#modal-success').modal('show')
                        setTimeout(function () {// wait for 5 secs(2)
                            window.location.href = $('#actionurl').val();
                            // then reload the page.(3)
                        }, 3000);
                    }
                    else {
                        alert(result.message);
                    }
                }
            });
    }
}