del "src\FubuMVC.HelloWorld\bin\FubuMVC.Diagnostics.dll"
rmdir /S "src\FubuMVC.HelloWorld\bin\fubu-packages"
"src\Fubu\bin\Debug\fubu.exe" create-pak -f diagnostics diagnostics.zip
"src\Fubu\bin\Debug\fubu.exe" install-pak diagnostics.zip fubu-hello