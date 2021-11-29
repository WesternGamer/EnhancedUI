const {
    CefSharp,
} = window; // this is equivalent to const CefSharp = window.CefSharp

const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("MainMenuViewModel");
}

let continueButton = document.getElementById("continueButton");
let newGameButton = document.getElementById("newGameButton");
let loadGameButton = document.getElementById("loadGameButton");
let joinGameButton = document.getElementById("joinGameButton");
let optionsButton = document.getElementById("optionsButton");
let characterButton = document.getElementById("characterButton");
let exitButton = document.getElementById("exitButton");

startCefSharp()
    .then(() => { continueButton.addEventListener("click", (e) => OnContinueButtonClick()) })
    .then(() => { newGameButton.addEventListener("click", (e) => OnNewGameButtonClick()) })
    .then(() => { loadGameButton.addEventListener("click", (e) => OnLoadGameButtonClick()) })
    .then(() => { joinGameButton.addEventListener("click", (e) => OnJoinGameButtonClick()) })
    .then(() => { optionsButton.addEventListener("click", (e) => OnOptionsButtonClick()) })
    .then(() => { characterButton.addEventListener("click", (e) => OnCharacterButtonClick()) })
    .then(() => { exitButton.addEventListener("click", (e) => OnExitButtonClick()) });

//To prevent user from clicking button twice.
function OnContinueButtonClick(){
    window.MainMenuViewModel.ContinueLastGame()
    continueButton.disabled = true
    setTimeout(function () { continueButton.disabled = false; }, 2000);
}

function OnNewGameButtonClick() {
    window.MainMenuViewModel.NewGame()
    newGameButton.disabled = true
    setTimeout(function () { newGameButton.disabled = false; }, 2000);
}

function OnLoadGameButtonClick() {
    window.MainMenuViewModel.LoadGame()
    loadGameButton.disabled = true
    setTimeout(function () { loadGameButton.disabled = false; }, 2000);
}

function OnJoinGameButtonClick() {
    window.MainMenuViewModel.JoinGame()
    joinGameButton.disabled = true
    setTimeout(function () { joinGameButton.disabled = false; }, 2000);
}

function OnOptionsButtonClick() {
    window.MainMenuViewModel.Options()
    optionsButton.disabled = true
    setTimeout(function () { optionsButton.disabled = false; }, 2000);
}

function OnCharacterButtonClick() {
    window.MainMenuViewModel.Character()
    characterButton.disabled = true
    setTimeout(function () { characterButton.disabled = false; }, 2000);

}

function OnExitButtonClick() {
    window.MainMenuViewModel.Exit()
    exitButton.disabled = true
    setTimeout(function () { exitButton.disabled = false; }, 2000);
}
    
    