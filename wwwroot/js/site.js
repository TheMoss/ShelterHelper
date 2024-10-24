function closeModal(selector) {
    $(selector).modal("hide");
}

function openModal(selector) {
    $(selector).modal("show");
}

const defaultTab = document.getElementById('openDefault');
if (defaultTab != null) {
    defaultTab.click();
}
;

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
if (scroller != null) {
    scroller.style.visibility = "hidden";

    scroller.addEventListener("click", function () {
        container.scrollTo({
            top: 0,
            left: 0
        });
    });

}

const completedTrue = document.getElementById("completedTrue");
completedTrue.addEventListener("click", function () {

    var inProgressTrue = document.getElementById("inProgressTrue");
    var inProgressFalse = document.getElementById("inProgressFalse");

    if (completedTrue.checked) {
        inProgressFalse.checked = true;

        inProgressFalse.disabled = true;
        inProgressTrue.disabled = true;
    }
})

const completedFalse = document.getElementById("completedFalse");
completedFalse.addEventListener("click", function () {
    var inProgressTrue = document.getElementById("inProgressTrue");
    var inProgressFalse = document.getElementById("inProgressFalse");

    if (completedFalse.checked) {

        inProgressFalse.disabled = false;
        inProgressTrue.disabled = false;
    }
})

container.addEventListener("scroll", e => {
    if (container.scrollTop > 10) {
        scroller.style.visibility = "visible";
    } else {
        scroller.style.visibility = "hidden";
    }

});

function generateChipNumber() {
    document.getElementById("chip-number-input").value = Math.floor(Math.random() * (99999999 - 11111111 + 1) + 11111111);
};


function submitAddResources(event, itemId, targetCategory) {
    event.preventDefault();

    let itemName = event.currentTarget.querySelector("p").innerHTML;
    let input = event.currentTarget.querySelector("input")
    let inputValue = input.value;
    console.log(itemId, itemName, inputValue);

    switch (targetCategory) {
        case 'diet':
            addResourcesDiet(event, itemId, inputValue);
            break;
        case 'bedding':
            break;
        case 'toy':
            break;
        case 'accessory':
            break;
    }
    input.value = '';
}

async function addResourcesDiet(event, itemId, inputValue) {
    const dietUrl = `https://localhost:7147/api/resources/diets/${itemId}`;
    let updatedQuantity = 0;
    await fetch(dietUrl)
        .then(response => {
            return response.json()
        })
        .then(data => {
            updatedQuantity = data.quantity_kg + parseInt(inputValue);
            console.log(`Updated quantity: ${updatedQuantity}`);
        })
        .catch(error => console.error(`Error fetching data: ${error}`));

    await fetch(dietUrl, {
        method: "PATCH",
        body: JSON.stringify([{
            "op": "replace",
            "path": "quantity_kg",
            "value": updatedQuantity.toString()
        }]),
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        }
    })
        .then(response => response.json())
        .then(json => console.log(json))
        .catch(error => console.error(`Error putting data: ${error}`));

}

async function moveToDoing(assignmentId){
    const assignmentsUrl = `https://localhost:7147/api/assignments/${assignmentId}`;
    await fetch(assignmentsUrl, {
        method: "PATCH",
        body: JSON.stringify([{
            "op" : "replace",
            "path" : "isInProgress",
            "value" : "true"
        }]),
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        }
    });
    
    location.reload();
}

async function moveToDone(assignmentId){
    const assignmentsUrl = `https://localhost:7147/api/assignments/${assignmentId}`;
    await fetch(assignmentsUrl, {
        method: "PATCH",
        body: JSON.stringify([{
            "op" : "replace",
            "path" : "isInProgress",
            "value" : "false"
        },
            {
                "op" : "replace",
                "path" : "isCompleted",
                "value" : "true"  
            }]),
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        }
    });
    location.reload();
}
