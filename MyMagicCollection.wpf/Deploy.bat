xcopy *.dll ..\..\..\..\..\MyMagicCollection /y /z
xcopy *.exe ..\..\..\..\..\MyMagicCollection /y /z
xcopy App_Data\ImageCache\Sets\* ..\..\..\..\..\MyMagicCollection\App_Data\ImageCache\Sets /s /y /z

del ..\..\..\..\..\MyMagicCollection\*.vshost.exe /Q
