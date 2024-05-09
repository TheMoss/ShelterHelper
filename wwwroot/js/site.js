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
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    document.getElementById(typeName).style.display = "block";
    event.currentTarget.className += " active";
}

const scroller = document.querySelector("#scroller");
const container = document.querySelector(".container");
scroller.style.visibility = "hidden";
container.addEventListener("scroll", e => {
    if (container.scrollTop > 10) {
        scroller.style.visibility = "visible";
    }
    else {
        scroller.style.visibility = "hidden";
    }
    
});

scroller.addEventListener("click", function () {
    container.scrollTo({
        top: 0,
    left: 0
    });
});
