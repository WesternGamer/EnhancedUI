@echo off

REM Location of your SpaceEngineers.exe
mklink /J GameBinaries "D:\Games\steamapps\common\SpaceEngineers\Bin64"

REM Content folder linked to the Web developer's project or local build folder
mkdir GameBinaries\Plugins\Local\EnhancedUI 2>NUL
mklink /J GameBinaries\Plugins\Local\EnhancedUI\Content EnhancedUI\HtmlContent

pause