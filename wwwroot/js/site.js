async function getSelectedEmployees(assignmentId){
    const url = `https://localhost:7147/api/EmployeesAssignments/Search?assignmentId=${assignmentId}`;
    let assignedEmployees =[];
    try {
        const response = await fetch(url);
        if (response.ok){
            const assignmentJson = await response.json();
            for (let i =0; i < assignmentJson.length; i++){
                assignedEmployees.push(assignmentJson[i].employee.employeeId);
            }
            return assignedEmployees;
        }
    }
    catch(error)
    {console.error(error.message)}
}
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


container.addEventListener("scroll", e => {
    if (container.scrollTop > 10) {
        scroller.style.visibility = "visible";
    } else {
        scroller.style.visibility = "hidden";
    }

});

function generateChipNumber() {
    document.getElementById("chip-number-input").value = Math.floor(Math.random() * (99999999 - 11111111 + 1) + 11111111);
}

async function addResourcesHelper(event, resourceType, itemId, inputValue){
    event.preventDefault();
    const url = `https://localhost:7147/api/resources/${resourceType}/${itemId}`;
    let updatedQuantity;
    if(inputValue < 1){
        throw new Error('The quantity has to be larger than 0.');
    }
    if (resourceType === 'accessories' || resourceType === 'beddings' || resourceType === 'diets' || resourceType === 'toys')
    {
        await fetch(url)
            .then(response => response.json())
            .then(data =>{
                if(resourceType === 'beddings' || resourceType === 'diets'){
                    updatedQuantity = data.quantity_kg + parseInt(inputValue);
                } else if (resourceType === 'accessories' || resourceType === 'toys') {
                    updatedQuantity = data.quantity + parseInt(inputValue);
                }
                })
            .catch(error => console.error(`Error fetching data: ${error}`));

        await fetch(url, {
            method: "PATCH",
            body: JSON.stringify([{
                "op": "replace",
                "path": resourceType === 'beddings' || resourceType === 'diets' ? 'quantity_kg' : 'quantity',
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
    else{
        throw new Error(`${resourceType} is not a valid resource type.`);
    }
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

