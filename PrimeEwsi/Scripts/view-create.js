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

function DeleteFile(name) {
    var element = document.getElementById("filesContainer" + name);

    element.remove();
    index--;

     var elements = document.getElementsByClassName("fileInput");

     for (var i = 0, len = elements.length; i < len; i++) {
         elements[i].id = "Files_" + i + "_";
         elements[i].name = "Files[" + i + "]";
     }
}

function EditFile(index) {
    console.log(index);
    var fileName = document.getElementById("Files_" + index + "_").value;

    console.log(fileName);
    document.getElementById("FilesInput").value = fileName;
    DeleteFile(index);
}

var index = 0;

function AddFile() {
    var filesContainer = document.getElementById("files");
    var fileInput = document.getElementById("FilesInput");
    
    if (fileInput.value === "") {
        return;
    }

    filesContainer
        .innerHTML += "<div id='filesContainer"+index+"'><a target='_blank' href='" + fileInput.value + "'> " + fileInput.value + " <\a>" +
        " <input class='fileInput' id='Files_" + index + "_' name='Files[" + index + "]' type='hidden' value='" + fileInput.value + "' />" + "<i style='margin: 3px' class='fa fa-pencil' onclick='EditFile(" + index + ")' aria-hidden='true'></i>" + "<i style='margin: 3px' class='fa fa-trash-o fw' onclick='DeleteFile(" + index + ")' aria-hidden='true'></i></div>";
    index++;
   
    fileInput.value = "";
}

function AddFileFromValue(value) {

    var filesContainer = document.getElementById("files");
    
    filesContainer
        .innerHTML += "<div id='filesContainer" + index + "'><a target='_blank' href='" + value + "'> " + value + " <\a>" +
        " <input class='fileInput' id='Files_" + index + "_' name='Files[" + index + "]' type='hidden' value='" + value + "' />" + "<i style='margin: 3px' class='fa fa-pencil' onclick='EditFile(" + index + ")' aria-hidden='true'></i>" + "<i style='margin: 3px' class='fa fa-trash-o fw' onclick='DeleteFile(" + index + ")' aria-hidden='true'></i></div>";
    index++;
   
}

$(function () {
    document.getElementById("Teets").tabIndex = "1";
    document.getElementById("Component").tabIndex = "2";
    document.getElementById("ProjectId").tabIndex = "3";
    document.getElementById("TestEnvironment").tabIndex = "4";
    document.getElementById("FilesInput").tabIndex = "5";

 
    CreateAutoComplet("TestEnvironment");
    CreateAutoComplet("Component");
    CreateAutoComplet("Teets");
    CreateAutoComplet("ProjectId");
});

function CreateAutoComplet(name) {
    var availableTags = [];

    var liElements = document.getElementById(name + "-dropdown-items").getElementsByTagName("li");

    for (i = 0; i < liElements.length; i++) {

        availableTags.push(liElements[i].innerText.trim());
    }
        
    $("#"+name+"").autocomplete({
        source: availableTags,
        delay: 100

    }).data("ui-autocomplete")._renderMenu = function (ul, items) {
        var that = this;
        $.each(items, function (index, item) {
            that._renderItemData(ul, item);
        });
        $(ul).addClass("dropdown-menu");
    }
}
