$(document).ready(function () {
    $("#window-size").text(`${window.innerWidth}x${window.innerHeight}`);
    initializeProxy().then(_ => null);
});

async function initializeProxy() {
    await CefSharp.BindObjectAsync("model");
    model.MarkLoaded();
}