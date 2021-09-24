@echo off

REM Location of your SpaceEngineers.exe
mklink /J GameBinaries "C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64"

REM Content folder linked to the Web developer's project or local build folder
mklink /J GameBinaries\Plugins\EnhancedUI EnhancedUI\Content

pause
