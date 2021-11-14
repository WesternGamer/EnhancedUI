const {
    CefSharp,
} = window; // this is equivalent to const CefSharp = window.CefSharp

const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("WebPageViewModel");
}

let continueButton = document.getElementById("continueButton");
let newGameButton = document.getElementById("newGameButton");
let loadGameButton = document.getElementById("loadGameButton");
let joinGameButton = document.getElementById("joinGameButton");
let optionsButton = document.getElementById("optionsButton");
let characterButton = document.getElementById("characterButton");
let exitButton = document.getElementById("exitButton");

startCefSharp()
    .then(() => { continueButton.addEventListener("click", (e) => window.WebPageViewModel.ContinueLastGame()) })
    .then(() => { newGameButton.addEventListener("click", (e) => window.WebPageViewModel.NewGame()) })
    .then(() => { loadGameButton.addEventListener("click", (e) => window.WebPageViewModel.LoadGame()) })
    .then(() => { joinGameButton.addEventListener("click", (e) => window.WebPageViewModel.JoinGame()) })
    .then(() => { optionsButton.addEventListener("click", (e) => window.WebPageViewModel.Options()) })
    .then(() => { characterButton.addEventListener("click", (e) => window.WebPageViewModel.Character()) })
    .then(() => { exitButton.addEventListener("click", (e) => window.WebPageViewModel.Exit()) });
    
    