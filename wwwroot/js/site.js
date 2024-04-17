// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



function closeModal(selector) {
    $(selector).modal("hide");
}

function openModal(selector) {
    $(selector).modal("show");
}
document.getElementById('openDefault').click();
function openType(event, typeName) {
    var i, tabcontent, tablinks;

    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace("active", "");

    }

    document.getElementById(typeName).style.display = "block";
    event.currentTarget.className += "active";
}

function cloneRow(rowsId) {
    var row = document.getElementById(rowsId).firstElementChild;
    var clone = row.cloneNode(true);

    clone.prepend(document.createElement("hr"));

    let deleteButton = document.createElement('button');
    deleteButton.classList.add('btn', 'btn-danger', 'deleteBtn');
    deleteButton.textContent = "Delete";
    deleteButton.addEventListener('click', deleteRow);
    clone.appendChild(deleteButton);

    document.querySelector('input').value = "";
    document.getElementById(rowsId).appendChild(clone);

    function deleteRow() {
        document.getElementById(rowsId).removeChild(clone);
    }
};

