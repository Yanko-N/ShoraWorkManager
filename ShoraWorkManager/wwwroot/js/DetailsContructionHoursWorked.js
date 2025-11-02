
function GetAddNewWorkedHours(contructionId) {
    $.ajax({
        url: '/HoursWorkers/Create/' + contructionId,
        type: "GET",
        data: {

        },
        success: function (result) {
            $("#workedHoursForm").html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });

}

function ReturnToIndexWorkedHours(contructionId) {
    $.ajax({
        url: '/HoursWorkers/GetTheWorkedHoursIndex',
        type: "GET",
        data: {
            id: contructionId
        },
        success: function (result) {
            $("#workedHoursForm").html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });

}

function SubmitCreateConstructionWorkedHoursForm(constructionId) {
    const form = document.getElementById("createFormWorkedHours");
    const formData = new FormData(form);

    $.ajax({
        url: '/HoursWorkers/Create/' + constructionId,
        type: "POST",
        data: formData,               
        processData: false,           
        contentType: false, 
        success: function (result) {
            $("#workedHoursForm").html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}
