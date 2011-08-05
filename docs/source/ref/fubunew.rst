============
fubu new
============

.. versionadded:: 1.0

``fubu new`` is FubuMVC's command-line tool to create new FubuMVC projects.
This document outlines all it can do.

Usage
=====

``projectname`` should be the name of the new FubuMVC project you are about to create

``options`` which is optional, should be zero or more of the options available for the ``new`` command

Available Options
=================

.. versionadded:: 1.1
Git Repository (-g)
-------------------

Git repository for a FubuMVC template.

Example usage:

.. code-block:: bash

    fubu new -g https://github.com/myrepo/myfubumvctemplate

Zip File (-z)
-------------

A zip file containing a FubuMVC template.

Example usage:

.. code-block:: bash

    fubu new -z ../../template/myfubumvctemplate.zip
