# app-launcher
AppLauncher is a Windows utility to allow 3rd-party applications to be called from within a web browser session, e.g. URL links following the registered protocol can be used to open applications on Windows.


/*
In Teams-Browser-App getestet:

mailto://frank.rabold@appsphere.com
file://c:\Tools\TLX.txt
file://c:\Tools\Report.pdf
file://Z:\Test\Test.xlsx
tlx://\\diskstation213\daten\Test\Test.xlsx
tlx://Z:/Test\Test.xlsx
tlx://Z:/Test\Filename with spaces.xlsx
tlx://Z:/Test\Folder with spaces\Test.xlsx
tlx://c:/Tools\Report.pdf
tlx://C:/Tools\The One And Only App Launcher\AppLauncher.exe?test?Hallo Welt!
tlx://C:/Tools\The One And Only App Launcher\AppLauncher.exe test x y z
tlx://c:/windows\notepad.exe

Besonderheiten in der Teams-Desktop-App:

bei UNC müssen alle Backslashes codiert werden
tlx://%5C%5Cdiskstation213%5Cdaten%5CTest%5CTest.xlsx

bei Laufwerk muss nach dem Doppelpunkt ein Slash stehen, kein Backslash
tlx://Z:/Test\Test.xlsx

Besonderheiten in der Teams-Desktop-App bei der IHK:
Doppelpunkt, alle Slashes und alle Leerzeichen müssen codiert werden
tlx://C%3A%5CTools%5CApp%20Launcher%5CAppLauncher.exe?test?Hallo%20Welt!
*/
