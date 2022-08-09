del AppLauncher\*.* /Q
del AppLauncher\bin\*.* /Q
del AppLauncher-win-x64.zip
copy ..\src\AppLauncher\bin\publish\folder\*.* AppLauncher\bin
copy register.cmd AppLauncher
copy unregister.cmd AppLauncher
tar -a -c -f AppLauncher-win-x64.zip AppLauncher
pause