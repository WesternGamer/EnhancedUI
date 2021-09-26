# Enhanced UI

Highly functional Terminal UI based on Web technology.

## Install the client plugin

1. Install the [Plugin Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2407984968)
2. Start the game
3. Open the Plugins menu (should be in the Main Menu)
4. Enable the Enhanced UI plugin from the GitHub source
5. Click on Save, then Restart (below the plugin list)

*Enjoy!*

## Development

TODO: Separate this section into a file as it grows.

### Prerequisites

- .NET SDK 5.0
- .NET Framework 4.8
- [Space Engineers](https://spaceengineersgame.com)
- [JetBrains Rider](https://jetbrains.com) or Visual Studio
- [Plugin Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2407984968)

### Steps

1. Clone the repository to have a local working copy
2. Edit the game's path in batch file, then run it: `Edit-and-run-before-opening-solution.bat`
3. Open the solution
4. Build the solution
5. Deploy locally and run
  - JetBrains: Select a run configuration for local deployment
  - Visual Studio: See the batch files in the projects

### Debugging

TODO

### References

- [CefSharp](https://github.com/cefsharp/) browser component
- [C#-JS integration](https://github.com/cefsharp/CefSharp/wiki/General-Usage#javascript-integration)