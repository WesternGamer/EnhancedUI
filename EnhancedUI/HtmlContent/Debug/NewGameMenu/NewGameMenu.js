const {
    CefSharp,
} = window; // this is equivalent to const CefSharp = window.CefSharp

const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("NewGameMenuViewModel");
}



    
    
    