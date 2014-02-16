var delay;

$(document).ready(function () {
    window.external.notify("onReady");
});


function slide(interval) {
    clearInterval(delay);
    delay = setInterval( function () {
        window.scrollBy(0, 2)
    }, interval);
}

function stopSlide() {
    clearInterval(delay);
}

function pullTabContent(content) {
    document.getElementById('textContainer').innerHTML = content;
}

