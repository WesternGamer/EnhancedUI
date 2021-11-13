let {
    CefSharp,
} = window; // this is equivalent to let CefSharp = window.CefSharp

let WebPageViewModel;

const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("WebPageViewModel");
    const { WebPageViewModel } = window;
}

startCefSharp()
    .then(() => { WebPageViewModel.Exit() })