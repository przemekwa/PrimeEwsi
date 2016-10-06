function AddText(elementName, value) {
    var element = document.getElementById(elementName);
    if (element.value === "") {
        element.value = value;
    } else {
        element.value += "," + value;
    }
}

function UpdateText(elementName, value) {
    var element = document.getElementById(elementName);
    element.value = value;
}

function UpdateInnerHTML(elementName) {
    var element = document.getElementById(elementName);
    element.innerHTML = "";
}

var index = 0;

function AddFile() {
    var filesContainer = document.getElementById("files");
    var fileInput = document.getElementById("FilesInput");

    console.log(index);


    if (fileInput.value === "") {
        return;
    }

    filesContainer
        .innerHTML += "<a target='_blank' href='" + fileInput.value + "'> " + fileInput.value + " <\a>" +
        " <input class='text-box single-line' id='Files_" + index + "_' name='Files[" + index + "]' type='hidden' value='" + fileInput.value + "' /><br/>";
    index++;
}
