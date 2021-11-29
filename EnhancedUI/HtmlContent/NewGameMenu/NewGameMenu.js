const {
    CefSharp,
} = window; // this is equivalent to const CefSharp = window.CefSharp

const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("NewGameMenuViewModel");
}

let continueButton = document.getElementById("continueButton");
let newGameButton = document.getElementById("newGameButton");
let loadGameButton = document.getElementById("loadGameButton");
let joinGameButton = document.getElementById("joinGameButton");
let optionsButton = document.getElementById("optionsButton");
let characterButton = document.getElementById("characterButton");
let exitButton = document.getElementById("exitButton");

//startCefSharp()
//    .then(() => { continueButton.addEventListener("click", (e) => window.NewGameMenuViewModel.ContinueLastGame()) })
//    .then(() => { newGameButton.addEventListener("click", (e) => window.NewGameMenuViewModel.NewGame()) })
//    .then(() => { loadGameButton.addEventListener("click", (e) => window.NewGameMenuViewModel.LoadGame()) })
//    .then(() => { joinGameButton.addEventListener("click", (e) => window.NewGameMenuViewModel.JoinGame()) })
//    .then(() => { optionsButton.addEventListener("click", (e) => window.NewGameMenuViewModel.Options()) })
//    .then(() => { characterButton.addEventListener("click", (e) => window.NewGameMenuViewModel.Character()) })
//    .then(() => { exitButton.addEventListener("click", (e) => window.NewGameMenuViewModel.Exit()) });
    
    