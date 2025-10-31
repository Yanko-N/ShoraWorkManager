// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function closePopUpMessage(id) {
    var div = document.getElementById(id);

    div.remove();
}

function ShowEditForm(clientId) {
    var editForm = document.getElementById(clientId + "-PartialEdit");
    var editBtn = document.getElementById(clientId + "-EditBtn");

    if (editForm.style.display === "none") {
        editForm.style.display = "block";
        editBtn.innerText = "Hide";
    }
    else {
        editForm.style.display = "none";
        editBtn.innerText = "Edit";
    }
}

function UpdateSortOrder(pageSize,search,sortBy) {

    var selectStuff = document.getElementById("orderBySelect");

    var url = `@Url.Action("Index", "Clients")??Page=1&PageSize=${pageSize}&Search=${search}&SortBy=${sortBy}&OrderBy=${selectStuff.innerText}"'`;

    window.location.href = url;
}



