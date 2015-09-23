# Building FubuMVC

The FubuMVC codebase still uses Rake for build automation, but as of September 2015, you don't **have** to use Rake to develop with FubuMVC if you don't want to. You **will need 
to have Node.js or Io.js and npm installed** in order to build the client side assets for FubuMVC's diagnostics package before working with the C# code.

## With Rake

Assuming you have Ruby 2.1+ installed on your computer, go to a command line and type...

1. bundle install
1. rake


## Visual Studio.Net Only

There is a small command file called `build.cmd` that can be executed once to bring down nuget and npm dependencies and build the client side assets that FubuMVC 
needs for its embedded. diagnostics. Run this command at least once before opening Visual Studio.Net.

From there, open the solution file at `src/FubuMVC.sln` and go to town.


# Working with Storyteller

* `rake open_st` -- Opens the Storyteller test suite in the Storyteller client for interactive editing and execution
* `rake storyteller` -- Runs all the Storyteller specifications

# Working with Diagnostics

Open the diagnostics harness application to the browser with the command `rake diagnostics`. This command will start webpack in a new window against the client side
attributes in the `javascript` folder in "watched" mode. This command also compiles and starts the `DiagnosticsHarness` application in a NOWIN server before opening a browser
window to the newly launched application. The browser will auto-refresh whenever a new version of the webpack `bundle.js` file is saved. You will have to stop and restart
the FubuMVC application to see any changes to the server side.
 

