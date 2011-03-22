rmdir "src\FubuMVC.HelloWorld\bin\fubu-packages" /S
"src\Fubu\bin\Debug\Fubu.exe" create-pak -f diagnostics diagnostics.zip
"src\Fubu\bin\Debug\Fubu.exe" install-pak diagnostics.zip fubu-hello