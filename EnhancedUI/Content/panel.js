$(document).ready(function () {
    $("#window-size").text(`${window.innerWidth}x${window.innerHeight}`);
    loadContent().then(_ => null);
});

async function loadContent() {
    await CefSharp.BindObjectAsync("proxy");

    let result = await proxy.TestAdd(1, 2);
    $('#result').text(result.toString());
}