$(document).ready(async function () {
    await CefSharp.BindObjectAsync("state");
    state.NotifyBound();

    $("#window-size").text(`${window.innerWidth}x${window.innerHeight}`);
});