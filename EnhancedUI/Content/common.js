$(document).ready(async function () {
    await CefSharp.BindObjectAsync("TerminalViewModel");
    console.log("Bound")
});