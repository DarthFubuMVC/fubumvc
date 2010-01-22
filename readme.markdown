FubuMVC ReadMe
===

How to Build
--

The build script requires Ruby with rake installed.

1. Run `InstallGems.bat` to get the ruby dependencies (only needs to be run once per computer)
1. open a command prompt to the root folder and type `rake` to execute rakefile.rb

If you do not have ruby:

1. You need to manually create a src\CommonAssemblyInfo.cs file 
  * type: `echo // > src\CommonAssemblyInfo.cs`
1. open src\FubuMVC.sln with Visual Studio and Build the solution