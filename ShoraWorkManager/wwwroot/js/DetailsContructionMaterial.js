
function GetAddNewMaterial(contructionId) {
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

function ReturnToIndexMaterialMovement(contructionId) {
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

function SubmitCreateConstructionMaterialForm(constructionId) {
    const form = document.getElementById("createFormMaterialMovement");
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
