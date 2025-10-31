function SubmitEditMaterialForm(materialId)
{
    const form = document.getElementById("editPartialForm-" + materialId);
    const formData = new FormData(form);

    $.ajax({
        url: '/Materials/EditPartial/' + materialId,
        type: "POST",
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (result)
        {

            if (typeof result === "object" && result.isSuccess) {

                window.location = result.returnUrl;

            } else { //Significa que retornar a partial View
                $("#editPartial-" + materialId).html(result);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}

function GetEditMaterialForm(materialId) {
    $.ajax({
        url: '/Materials/EditPartial',
        type: "GET",
        data: {
            id: materialId
        },
        success: function (result) {
            $("#editPartial-" + materialId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}