=======================
Contribution Guidelines
=======================

  All Fubu projects are hosted on `github`_ using Git and run continuous
  integration builds on our `build server`_.

If there's one thing we like the most, it's `pull requests`_. We always welcome
contributions and love to see what we're missing. Of course, these projects run
in production so we have to try and ensure a little quality by looking over
anything that gets included. In order to make this nice and simple, we've come
up with a little process for contributors. If your'e new to Git and GitHub,
please check out GitHub's `bootcamp documentation`_.

The basic Fubu workflow goes roughly like so:

    * `Fork`_ on GitHub

    * Clone the fork locally (``$ git clone git@github.com:you/fubuproject.git``)

    * Create your local feature or bugfix branch (``$ git checkout -b
      MyFeatureOrBugFixBranch``)

    * Work on the Feature or Bugfix

    * Rebase your commits on top of the latest code (``$ git checkout master; 
      git fetch upstream; git checkout myFeature; git rebase master myFeature``)

    * Push your branch to GitHub (``$ push origin myFeature``)

    * Issue Pull Request on GitHub

Along with the Fubu workflow, there are a few *Dos* and *Don'ts* for
contributing to Fubu projects.

Don't make changes to your local or remote master branches
==========================================================

The master branch on the central repo (*e.g.*, ``DarthFubuMVC/fubumvc.git``) and
the master branch on your fork belongs to the framework team. Here's an example:

You have a fork of ``DarthFubuMVC/fubumvc``. At all times, your master branch
must be an identical replica of DarthFubuMVC's master. Of course, you may fall
behind and not push the latest changes to master. This is ok. (You can 
:ref:`synchronize`.) The point here is that you cannot have changes to your
master branch that are not in DarthFubuMVC's.

Another good rule here is that your local branches should match your GitHub
repo's (on ``origin``). In other words, don't save changes to your local master
that aren't on your GitHub master.

Pull requests should always come from remote branches
=====================================================

Since we are keeping the master branch clean, the only way to make changes is by
branching. Therefore, in order to make changes public, you need to create a
remote branch on your fork.

This makes it much easier to maintain commit history and ensure that the proper
commits are pulled in without the need (or at least decreasing the need) for
cherry-picking.

.. _synchronize:

Synchronize your origin/master with upstream/master
===================================================

When working with a locally cloned fork of a GitHub repository, the repository
will have one remote repository called ``origin``. This is the GitHub
repository and is the default when one does a ``git push`` or a ``git pull``.
Since we're working with a Forked repository of a Fubu project, the canonical
repository should be added as ``upstream``. This is done by using the command:
(for FubuMVC)

.. code-block:: bash

    $ git remote add upstream "https://github.com/DarthFubuMVC/fubumvc"

.. note::

    The value in quotes is case sensitive.  If you incorrectly specify it, then 
    when you attempt ``git pull upstream master`` you will receive a message 
    indicating 
    
    .../info/refs notfound: did you run git update-server-info on the server
    
    To resolve this message, just fix the casing to match the text above.
    
Once upstream is added, keeping up to date with changes is a few simple
commands:

.. code-block:: bash

    $ git checkout master
    $ git pull upstream master
    $ git push origin master

Write tests for your pull request
=================================

Whether a bug fix or new feature, it is considered good practice to provide
tests with your code changes. Our community loves working tests because it makes
it much easier to accept the pull request. Having tests for your code is one of
the first things the community will look at when a pull request is submitted.

Fubu projects use the same basic structure for tests: ``[Name].Tests``. We use
`NUnit`_ and RhinoMocks (Look through `our tests`_ for some good Rhino Mocks
examples). If you're having any troubles with writing tests or just how to go
about testing, ask! The `mailing list`_ is a great place to get help and we're
more than happy to help you work through getting your tests.

Keep pull request commit history clean
======================================

Pull requests are much more easily merged into ``master`` if the commit history
is clean and simple. To do that, there are a few things to keep in mind:

    * One branch per feature or bug fix

    * Do some work and commit it

    * Checkout ``master`` and pull the latest

    * Checkout your branch and rebase on top of ``master``

    * Run rake compile test

This ensures that your changes are applied on top of the very latest changes to
``master``. It makes it easier to read, easier to pull in, and increases your
chances of your pull request being accepted without needing changes to fix any
problems accidentally introduced.

.. _github: https://github.com/DarthFubuMVC/
.. _build server: http://build.fubu-project.org/
.. _pull requests: http://help.github.com/send-pull-requests/
.. _bootcamp documenation: http://help.github.com/#github_bootcamp
.. _Fork: http://help.github.com/fork-a-repo/
.. _NUnit: http://www.nunit.org/index.php?p=home
.. _our tests: https://github.com/DarthFubuMVC/fubumvc/tree/master/src/FubuMVC.Tests
.. _mailing list: http://groups.google.com/group/fubumvc-devel
