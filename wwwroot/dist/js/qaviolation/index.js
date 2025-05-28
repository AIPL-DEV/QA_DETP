$(document).ready(function () {
    $.validator.addMethod("atLeastOneRequired", function (value, element, params) {
        return $('#FinancialYear').val() || $('#Month').val() || $('#VendorCode').val();
    }, "At least one of these fields is required");
    $("#SearchForm").validate({
        rules: {
            FinancialYear: {
                atLeastOneRequired: true

            },
            Month: {
                atLeastOneRequired: true
            },
            VendorCode: {
                atLeastOneRequired: true
            }
        },
        submitHandler: async function (form, event) {
            event.preventDefault();


            updateTable();
        }
    });
});

var violationTable = null;
function updateTable() {
    if (violationTable != null) {

        violationTable.ajax.reload();
        return;
    }
    violationTable = $('#violation-table').DataTable({
        serverSide: true,
        ajax: {
            url: 'QaViolations/Search',
            type: 'POST',
            data: function (data) {
                data.Year = $('#FinancialYear').val();
                data.Month = $('#Month').val();
                data.VendorCode = $('#VendorCode').val();
            }
        },
        columns: [
            {
                data: "visitNo",
            },
            {
                data: "division.name"
            },
            {
                data: "dept.name"
            },
            {
                data: "loggedDate"
            },
            {
                data: "vendorCode"
            },
            {
                data: "vendorName"
            },
            {
                data: "logNonConfirmance"
            },
            {
                data: "numberOfObservation"
            },
            {
                data: "site"
            },
            {
                data: "siteIncharge.name"
            },
            {
                data: "projectIncharge.name"
            },
            {
                data: "businessHead.name"
            },
            {
                data: "deptHod.name"
            },
            {
                render: (value, isDiaplay, row) => {
                    
                    return `<a href="QaViolations/Create/${row.serialNo}" class="btn btn-warning">Create Consecuence Request</button>`
                }
            }
        ],

        preDrawCallback: function () {
            let el = $('div.dataTables_filter label');
            if (!el.parent('form').length) {
                el.wrapAll('<form></form>').parent()
                    .attr('autocomplete', false)
                    .attr('autofill', false);
            }
        }
        
    });
}