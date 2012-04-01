.. _usingfubunew:

==================
Using fubu new
==================

Using :program:`fubu.exe`'s :ref:`fubu-new` command to create template FubuMVC
projects and to create your own custom templates.

Creating a default FubuMVC project
----------------------------------

:program:`fubu.exe` has a default template zip included when invoking the
``new`` command. After :ref:`getting FubuMVC <getting-fubumvc>` you should add
the :program:`fubu.exe` to the :envvar:`PATH`.  Now that :program:`fubu.exe` is
on the path, open a terminal and navigate to a writable directory that you would
like the new FubuMVC project, and type:

.. code-block:: bash

    $ fubu new My.NewProject

This will create the folder `NewProject` with the default FubuMVC project using
My.NewProject as the project Namespace. Lets have a quick look at what the
``new`` command generated::

    NewProject/
        NewProject.sln
        src/
            NewProject/
                Controllers/
                Models/
                Properties/
                Views/
                NewProject.csproj
                NewProjectHtmlConventions.cs
                NewProjectRegistry.cs
                Global.asax
                Web.config

And there you have it! A new FubuMVC project ready to go.

.. _fubunew-using-zip:

Using a different Fubu Template zip
-----------------------------------

:program:`fubu.exe` by default uses a zip file placed in the same directory as
itself, but if you have other template zip files, you can use the `-z` flag on
:ref:`fubu-new`.

.. code-block:: bash

    $ fubu new -z ../MyTemplate.zip My.NewTemplateProject

This will unzip the contents of the ``MyTemplate.zip`` to the folder
``NewTemplateProject`` and run the template transformation on the contents of
the zip file.

.. _fubunew-zip:

Creating your own template zip
------------------------------

Creating your own template zip for :ref:`fubu new <fubu-new>` is a very simple process. The
easiest way to start is to unzip The :file:`fubuTemplate.zip` file that resides with
:ref:`fubu.exe <fubuexe>`.

Once you've extracted the :file:`fubuTemplate.zip` you will see this structure::

      FUBUPROJECTNAME.sln
      src/
          FUBUPROJECTNAME/
              Controllers/
              Models/
              Properties/
              Views/
              FUBUPROJECTNAME.csproj
              FUBUPROJECTNAMEHtmlConventions.cs
              FUBUPROJECTNAMERegistry.cs
              Global.asax
              Web.config

Inside of the files will look much like this:

.. literalinclude:: ../../../fubuTemplate/src/FUBUPROJECTNAME\FUBUPROJECTSHORTNAMERegistry.cs
   :language: csharp
   :linenos:

From this, we can see that there are a few keywords that get replaced when using
:ref:`fubu new <fubu-new>`. This is the list of keywords and corresponding
replacements:

==================================== =========================================
Keyword                              Replacement
==================================== =========================================
FUBUPROJECTNAME                      Name put in the fubu new command
FUBUPROJECTSHORTNAME                 Last part of fubu new name (after last .)
FACEFACE-FACE-FACE-FACE-FACEFACEFACE Guid.NewGuid()
==================================== =========================================

Modifying the template can be as simple as adding extra classes to adding
entirely new Projects.

When you're happy with the results of your template, you simply need to zip up
the directory. You can then use the ``-z`` flag to use the new template, for
example:

.. code-block:: bash

    $ fubu new -z ../../Templates/MyNewTemplate.zip My.NewProject

.. _fubunew-using-git:

Using a Git repository as a Fubu Template
-----------------------------------------

:program:`fubu.exe` can also use a git repository as a template source as well
as using a zip file. If you have a Git repository that has a fubu template, you can
use the `-g` flag on :ref:`fubu-new`.

.. code-block:: bash

    $ fubu new -g https://github.com/MyGithubUsername/MyFubuTempmlate.git My.NewGitBasedProject

This will clone the git repository `MyFubuTemplate` to the folder
``NewGitBasedproject`` and run the template transformation on the contents of
the cloned repository.

.. _fubunew-git:

Creating your own template with git
-----------------------------------

Creating a template using Git is easy and fun to do, it is very much like
creating a `template zip file.<fubunew-zip>`. The main difference when creating
a template git repository is that you initialize a git repository and add the
code to the repository.

.. code-block:: bash

   $ git init
   Initialized empty Git repository in c:/
   $ git add .
   $ git commit -m "New Fubu Template project"

Once you have committed your git repository, you can push the code up to a
central place such as `Github <https://github.com>`_ or `Gitorious
<https://gitorious.org>`_ if that helps sharing the template project. Once
you've got your git repository set up, `using the repository <fubunew-using-git>` is
simple.
