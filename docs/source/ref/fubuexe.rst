
.. _fubuexe:

=====================
Working With fubu.exe
=====================

:program:`fubu.exe` is FubuMVC's command-line tool for working with FubuMVC applications and deployments.
This document outlines all it can do.

Usage
=====

.. code-block:: bash

    fubu.exe <command> [options]

``command`` should be one of the commands listed in this document.

``options`` should be zero or more of the options available for the given command.

Getting runtime help
--------------------

:option:`help`

Run ``fubu.exe help`` to display a list of all available commands.

Run ``fubu.exe <command> help`` to display a description of the given command
and a list of its available options.

Available Commands
==================

.. _fubu-add-directive:

add-directive
-------------

Adds a directive to an existing /deployment/recipe/host

Usages for ``add-directive``

.. code-block:: bash

    add-directive <recipe> <host> <directive> [-deployment <deployment>] [-open]

**Arguments**

``recipe`` - The recipe to add the directive to.

``host`` - The host in the recipe to add the directive to.

``directive`` - The directive to add.

**Flags**

``-deployment <deployment>`` - The deployment directory

``-open`` - Open the directive file when done.

.. _fubu-add-profile:

add-profile
-----------

Adds a deployment profile

.. code-block:: bash

    add-profile <name> [-recipes <recipes1 recipes2 recipes3 ...>] [-deployment <deployment>]

**Arguments**

``name`` - The name of the profile

``recipes`` - List of recipes to include

**Flags**

``-deployment <deployment>`` - Where the ~/deployment folder is

.. _fubu-add-recipe:

add-recipe
----------

Adds a recipe to a deployment

.. code-block:: bash

    add-recipe <name> [-deployment <deployment>]

**Arguments**

``name`` - The name of the recipe

**Flags**

``-deployment <deployment>`` - Where the ~/deployment folder is

.. _fubu-alias:

alias
-----

Manage folder aliases


**Usages**

List all the aliases for this solution folder

.. code-block:: bash

    alias

Removes an alias

.. code-block:: bash

    alias <name> [-remove]

Creates a new alias for a folder

.. code-block:: bash

    alias <name> <folder>

**Arguments**

``name`` - The name of the alias

``folder`` - The path to the actual folder

**Flags**

``-remove`` - Removes the alias

.. _fubu-appendvdir:

appendvdir
----------

Appends to virtual directories in IIS

.. code-block:: bash

    appendvdir <virtualdirectoryfolder> <folder> <virtualdirectory>

**Arguments**

``virtualdirectoryfolder`` - The subfolder pathing underneath the virtual directory, i.e., '/content/images'

``folder`` - Physical file path

``virtualdirectory`` - The name of the virtual directory in IIS

.. _fubu-assemblies:

assemblies
----------

Adds assemblies to a given manifest

Remove or adds all assemblies to the manifest file

.. code-block:: bash

     assemblies add|remove|list <directory> [-file <filename>] [-open] [-target debug|release]

Removes or adds a single assembly name to the manifest file

.. code-block:: bash

    assemblies add|remove|list <directory> <assemblyname> [-file <filename>] [-open] [-target debug|release]

**Arguments**

``mode`` - Add, remove, or list the assemblies for this manifest

``directory`` - The package or application directory

``assemblyname`` - Add or removes the named assembly

**Flags**

``-file <filename>`` - Overrides the name of the manifest file if it's not the default .package-manifest or .fubu-manifest

``-open`` - Opens the manifest file in your editor

``-target debug|release`` - Choose the compilation target for any assemblies.  Default is debug


.. _fubu-assembly-pak:

assembly-pak
------------

Bundle up the content and data files for a self contained assembly package

.. code-block:: bash

    assembly-pak <rootfolder> [-projfile <projfile>]

**Arguments**

``rootfolder`` - The root folder for the project if different from the project file's folder

**Flags**

``-projfile <projfile>`` - Name of the csproj file.  If set, this command attempts to add the zip files as embedded resources

.. _fubu-bundle:

bundle
------

Bundles up designated deployment options to a folder

Bundle with only the environment settings in the deployment folder

.. code-block:: bash

    bundle <destination> [-create-bottles] [-profile <profile>] [-deployment <deployment>] [-recipe <recipe>] [-report <report>]

Bundle with imported folders

.. code-block:: bash

    bundle <destination> [-importedfolders <importedfolders1 importedfolders2 importedfolders3 ...>] [-create-bottles] [-profile <profile>] [-deployment <deployment>] [-recipe <recipe>] [-report <report>]

**Arguments**

``destination`` - The directory name where the deployment artifacts are going to be written

``importedfolders`` - Import any other ~/deployment folders for this deployment

**Flags**

``-create-bottles`` - CreateBottlesFlag

``-profile <profile>`` - The profile to execute.  'default' is the default.

``-deployment <deployment>`` - Path to where the deployment folder is ~/deployment

``-recipe <recipe>`` - Tacks on ONE additional recipie. Great for including tests.

``-report <report>`` - File where the installation report should be written.  Default is installation_report.htm

.. _fubu-copy:

copy
----

Copies all of the deployment structure to another folder with all the necessary bottle support

.. code-block:: bash

    copy <destination> [-create-bottles] [-deployment <deployment>]

**Arguments**

``destination`` - The directory name where the deployment artifacts are going to be written

**Flags**

``-create-bottles`` - CreateBottlesFlag

``-deployment <deployment>`` - Path to where the deployment folder is ~/deployment

.. _fubu-create-all:

create-all
----------

Creates all the packages for the directories / manifests listed in the bottles.manifest file and puts the new packages into the deployment/bottles directory

.. code-block:: bash

    create-all [-directory <directory>] [-deployment <deployment>] [-pdb] [-target debug|release] [-clean]

**Flags**

``-directory <directory>`` - Overrides the top level directory to begin searching for package manifests

``-deployment <deployment>`` - Overrides the deployment directory ~/deployment

``-pdb`` - Includes any matching .pdb files for the package assemblies

``-target debug|release`` - Overrides the compilation target.  The default is debug

``-clean`` - Directs the command to remove all bottle files before creating new files.  **Can be destructive**


.. _fubu-create-deployment:

create-deployment
-----------------

Creates or seeds a new website deployment

.. code-block:: bash

    create-deployment <applicationbottlename> [-recipe <recipe>] [-host <host>] [-virtualdir <virtualdir>] [-deployment <deployment>] [-open]

**Arguments**

``applicationbottlename`` - Declares the main application bottle

**Flags**

``recipe <recipe>`` - Recipe name for the FubuWebsite.  Default is 'baseline'

``-host <host>`` - Host name of the FubuWebsite directive.  Default is 'web'

``-virtualdir <virtualdir>`` - Override the virtual directory name

``-deployment <deployment>`` - Overrides the location of the ~/deployment directory

``-open`` - Open the directive file when done.

.. _fubu-create-pak:

create-pak
----------

Create a package file from a package directory

.. code-block:: bash

    create-pak <packagefolder> <zipfile> [-pdb] [-f] [-target debug|release] [-file <manifestfilename>]

**Arguments**

``packagefolder`` - The root physical folder (or valid alias) of the package

``zipfile`` - The filepath where the zip file for the package will be written ie. ./blue/my-pak.zip

**Flags**

``-pdb`` - Includes any matching .pdb files for the package assemblies

``-f`` - Forces the command to delete any existing zip file first

``-target debug|release`` - Choose the compilation target for any assemblies

``-file <manifestfilename>`` - Overrides the name of the manifest file

.. _fubu-createvdir:

createvdir
----------

Creates virtual directories in IIS

.. code-block:: bash

    createvdir <folder> <virtualdirectory>

**Arguments**

``folder`` - Physical file path

``virtualdirectory`` - The name of the virtual directory in IIS

.. _fubu-deploy:

deploy
------

Deploys the given profile

Deploy with only the environment settings in the deployment folder

.. code-block:: bash

    deploy [-override <override>] [-profile <profile>] [-deployment <deployment>] [-recipe <recipe>] [-report <report>]

Deploy with imported folders

.. code-block:: bash

    deploy [-importedfolders <importedfolders1 importedfolders2 importedfolders3 ...>] [-override <override>] [-profile <profile>] [-deployment <deployment>] [-recipe <recipe>] [-report <report>]

**Arguments**

``importedfolders`` - Import any other ~/deployment folders for this deployment

**Flags**

``-override <override>`` - Override any profile settings in form arg1:value1;arg2:value2;arg3:value3

``-profile <profile>`` - The profile to execute.  'default' is the default.

``-deployment <deployment>`` - Path to where the deployment folder is ~/deployment

``-recipe <recipe>`` - Tacks on ONE additional recipie. Great for including tests.

``-report <report>`` - File where the installation report should be written.  Default is installation_report.htm

.. _fubu-help:

help
----

lists all the available commands as well as providing help for individual commands.

List all the available commands

.. code-block:: bash

    help

Show all the valid usages for a command

.. code-block:: bash

    help <name>

**Arguments**

``name`` - A command name

.. _fubu-init:

init
----

Seeds the /deployment folder structure underneath the root directory of a codebase

.. code-block:: bash

    init [-deployment <deployment>] [-f]

**Flags**

``-deployment <deployment>`` - Physical folder (or valid alias) of the application

``-f`` - ForceFlag

.. _fubu-init-pak:

init-pak
--------

Initialize a package manifest

.. code-block:: bash

    init-pak <path> <name> [-role <role>] [-alias <alias>] [-open] [-noweb] [-f]

**Arguments**

``path`` - The physical path to the new package

``name`` - The name of the new package

**Flags**

``-role <role>`` - What role should this pak play - Options: module (default), binaries, config, application

``-alias <alias>`` - Creates a folder alias for the package folder.  Equivalent to fubu alias <folder> <alias>

``-open`` - Opens the package manifest file in notepad

``-noweb`` - There is no web content to include

``-f`` - Force the command to overwrite any existing manifest file if using the -create flag

.. _fubu-install:

install
-------

Runs installer actions and/or environment checks for an application

.. code-block:: bash

    install <appfolder> [-mode install|all|check] [-logfile <logfile>] [-open] [-class <environmentclassname>] [-assembly <environmentassembly>] [-config-file <configfile>]

**Arguments**

``appfolder`` - Root folder (or alias) of the fubu application

**Flags**

``-mode install|all|check`` - Determines what actions are executed for each installer.  'install' is the default

``-logfile <logfile>`` - Overrides the location of the log file produced, otherwise 'installation.htm' is the default

``-open`` - When specified, opens the resulting log file in the default web browser

``-class <environmentclassname>`` - The IEnvironment class to run during an install

``-assembly <environmentassembly>`` - The assembly containing the IEnvironment class to run during an install

``-config-file <configfile>`` - The name of the .Net AppDomain config file to use while running the installers

.. _fubu-install-pak:

install-pak
-----------

Install a package zip file to the specified application

Install a package zip file to an application

.. code-block:: bash

    install-pak <packagefile> <appfolder>

Remove a package zip file from an application

.. code-block:: bash

    install-pak <packagefile> <appfolder> [-u]

**Arguments**

``packagefile`` - The package zip file location to be installed.  If un-installing, just use the zip file name

``appfolder`` - The physical folder (or valid alias) of the main application

**Flags**

``-u`` - Uninstalls the named package from an application folder

.. _fubu-link:

link
----

Links a package folder to an application folder in development mode

List the current links for the application

.. code-block:: bash

    link <appfolder>

Remove any and all existing links from the application to any package folder

.. code-block:: bash

    link <appfolder> [-cleanall]

Create a new link for the application to the package

.. code-block:: bash

    link <appfolder> <packagefolder>

Remove any existing link for the application to the package

.. code-block:: bash

    link <appfolder> <packagefolder> [-remove]


**Arguments**

``appfolder`` - The physical folder (or valid alias) of the main application

``packagefolder`` - The physical folder (or valid alias) of a package

**Flags**

``-remove`` - Remove the package folder link from the application

``-cleanall`` - Remove all links from an application manifest file

.. _fubu-list:

list
----

Lists all discovered manifests

List manifests

.. code-block:: bash

    list [-point <point>] [-deployment <deployment>]

list something specific

.. code-block:: bash

    list manifests|directives|recipes|profiles|bottles|hosts|all [-point <point>] [-deployment <deployment>]


**Arguments**

``mode`` - What to list

**Flags**

``-point <point>`` - Where to scan

``-deployment <deployment>`` - The directory where the deployment settings are stored

.. _fubu-new:

new
---

Creates a new FubuMVC solution (see :ref:`usingfubunew`)

**Arguments**

``projectname`` - This should be the name of the new FubuMVC project you are about to create.

**Flags**

``-g`` - Git repository URL for a FubuMVC template.


``-z`` - The path to a zip file containing a FubuMVC template.

Example usage using a Git repository:

.. code-block:: bash

    fubu new -g https://github.com/myrepo/myfubumvctemplate mynewfubumvcproject

To learn how to create your own template in a git repository, see :ref:`fubunew-git`

Example usage using a Zip file:

.. code-block:: bash

    fubu new -z /../template/myfubumvctemplate.zip mynewfubumvcproject

To learn how to create your own template in a zip file, see :ref:`fubunew-zip`

.. _fubu-packages:

packages
--------

Display and modify the state of package zip files in an application folder

.. _fubu-preview:

preview
-------

Generates a preview of the deployment

.. code-block:: bash

    packages <appfolder> [-cleanall] [-explode] [-removeall]

**Arguments**

``appfolder`` - Physical root folder (or valid alias) of the application

**Flags**

``-cleanall`` - Removes all 'exploded' package folders out of the application folder

``-explode`` - 'Explodes' all the zip files underneath <appfolder>/bin/fubu-packages

``-removeall`` - Removes all package zip files and exploded directories from the application folder

.. _fubu-ref-bottle:

ref-bottle
----------

Adds a bottles reference to the specified host

.. code-block:: bash

    ref-bottle <recipe> <host> <bottle> [-deployment <deployment>]

**Arguments**

``recipe`` - The recipe that the host is in

``host`` - The host to add the reference to

``bottle`` - The name of the bottle to link

**Flags**

``-deployment <deployment>`` - Path to the deployment folder (~/deployment)

.. _fubu-ref-recipe:

ref-recipe
----------

Adds a bottles reference to the specified host

.. code-block:: bash

    ref-recipe <profile> <recipe> [-deployment <deployment>]

**Arguments**

``profile`` - The name of the profile

``recipe`` - The name of the recipe to reference

**Flags**

``-deployment <deployment>`` - Path to the deployment folder (~/deployment)

.. _fubu-restart:

restart
-------

Restarts a web application by 'touching' the web.config file

.. code-block:: bash

    restart <appfolder>

**Arguments**

``appfolder`` - Physical folder (or valid alias) of the web application

.. _fubu-set-env-prop:

set-env-prop
------------

Writes or overwrites a single directive property in the environment.settings file

.. code-block:: bash

    set-env-prop <propertyvalue> [-deployment <deployment>]

**Arguments**

``propertyvalue`` - Property=Value declaration of the property to write out

**Flags**

``-deployment <deployment>`` - Overrides the location of the ~/deployment directory

.. _fubu-set-host-prop:

set-host-prop
-------------

Writes or overwrites a single directive property in a Recipe/Host file

.. code-block:: bash

    set-host-prop <recipe> <host> <propertyvalue> [-deployment <deployment>]

**Arguments**

``recipe`` - Name of the recipe

``host`` - Name of the host

``propertyvalue`` - Property=Value declaration of the property to write out

**Flags**

``-deployment <deployment>`` - Overrides the location of the ~/deployment directory

.. _fubu-set-profile-prop:

set-profile-prop
----------------

Writes or overwrites a single directive property in a profile file

.. code-block:: bash

    set-profile-prop <profile> <propertyvalue> [-deployment <deployment>]

**Arguments**

``profile`` - Name of the profile
``propertyvalue`` - Property=Value declaration of the property to write out

**Flags**

``-deployment <deployment>`` - Overrides the location of the ~/deployment directory

