$(document).ready(async function () {
    // For debugging only
    // $("#window-size").text(`${window.innerWidth}x${window.innerHeight}`);

    await CefSharp.BindObjectAsync("state");
    state.NotifyBound();
});