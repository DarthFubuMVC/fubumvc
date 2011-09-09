=====================
Writing documentation
=====================

Good documentation is critical to FubuMVC, so we place a high importance on
readable and consistent documentation. We also believe our documentation is as
important as our code, and deserves to be improved just as often.

This section explains some of the basics of crafting documentation changes.

Getting started with Sphinx
---------------------------

FubuMVC's documentation, and all other projects under the Fubu banner, use the
Sphinx__ documentation system. The idea behind Sphinx and other systems such as
Markdown is that formatted plain-text documentation can be transformed into
multiple target formats such as HTML, PDF, ePub, and many other formats.

__ http://sphinx.pocoo.org

Installing Sphinx on Windows
''''''''''''''''''''''''''''

The Sphinx documentation system is built in Python, so to get Sphinx running on
Windows there are a few steps that need to be taken.

    #. Install Python 2.7 from python.org__

    #. Add the Python directory to the :envvar:`PATH` (i.e. ``C:\Python27``)

    #. Add the Python /Scripts directory to the :envvar:`PATH` (i.e.
       ``C:\Python27\Scripts``)

    #. From the command prompt type ``C:\> easy_install -U Sphinx``

__ http://python.org/download

.. warning::

   On 64-bit Windows, follow `these instructions to get easy_install intalled
   <http://pypi.python.org/pypi/setuptools#windows>`_ then run ``easy_install -U
   Sphinx``

Once Sphinx is installed, building locally is easy; just ``rake docs:html`` from
any directory.

.. note::

   You will also notice that under the /docs directory there are two files named
   ``Makefile`` and ``make.bat``. These are used by ReadTheDocs__ which hosts
   the generated documentation. You do not need to run either of these files.

__ http://readthedocs.org

To get started, you'll want to read the :ref:`reStructuredText Primer
<sphinx:rst-primer>`.  After you've gotten an understanding of reStructuredText,
you'll want to read about the :ref:`Sphinx-specific markup
<sphinx:sphinxmarkup>` that's used to manage metadata, indexing and
cross-references.

Commonly used terms
-------------------

Here are some style guidelines on commonly used terms in the documentation.

    * **FubuMVC** - Always capitalize the first letter and the MVC.

    * **.NET** - Always uppercase.

    * **HTTP** - Always uppercase.

reStructuredText file format
----------------------------

Our documentation should follow these reStructuredText guidelines.

    * Capitalize only the inital word and proper nouns in section titles.

    * Wrap the documentation at 80 characters unless an example is significantly
      less readable over two lines.

    * Add as much semantic markup as possible. So::

          Use ``fubu.exe alias`` for adding an alias

      Is not nearly as useful as::

          Use :ref:`fubu-alias` for adding an alias
