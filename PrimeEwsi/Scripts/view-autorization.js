var copyTextareaBtn = document.querySelector('.js-textareacopybtn');

copyTextareaBtn.addEventListener('click', function (event) {

    var element = document.getElementById("copyArea");
    var textNode = element.childNodes[0];

    var range = document.createRange();
    range.setStart(textNode, 0);
    range.setEnd(textNode, element.innerHTML.length);

    var selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
    

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Copying text command was ' + msg);
    } catch (err) {
        console.log('Oops, unable to copy');
    }
});