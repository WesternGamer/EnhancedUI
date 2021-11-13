// Invoked from C#
// noinspection JSUnusedGlobalSymbols
function Exit() {
    window.Exit();
}

export const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("WebPageViewModel");
    const { WebPageViewModel.Exit() } = window;
}

let {
    CefSharp,
} = window;