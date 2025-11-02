
function GetAddNewWorkedHours(contructionId) {
    $.ajax({
        url: '/MaterialMovements/Create/' + contructionId,
        type: "GET",
        data: {

        },
        success: function (result) {
            $("#materialForm").html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });

}

function ReturnToIndexWorkedHours(contructionId) {
    $.ajax({
        url: '/MaterialMovements/GetTheMaterialMovementIndex',
        type: "GET",
        data: {
            id: contructionId
        },
        success: function (result) {
            $("#materialForm").html(result);
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
        url: '/MaterialMovements/Create/' + constructionId,
        type: "POST",
        data: formData,               
        processData: false,           
        contentType: false, 
        success: function (result) {
            $("#materialForm").html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}
