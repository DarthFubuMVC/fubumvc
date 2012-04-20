=====================
Writing documentation
=====================

Good documentation is critical to FubuMVC, so we place a high importance on
readable and consistent documentation. We also believe our documentation is as
important as our code, and deserves to be improved just as often.

This section explains some of the basics of crafting documentation changes.

Getting started with Sphinx
===========================

FubuMVC's documentation, and all other projects under the Fubu banner, use the
Sphinx__ documentation system. The idea behind Sphinx and other systems such as
Markdown is that formatted plain-text documentation can be transformed into
multiple target formats such as HTML, PDF, ePub, and many other formats while
the source documentation is easily stored in version control.

__ http://sphinx.pocoo.org

Installing Sphinx on Windows
----------------------------

The Sphinx documentation system is built in Python, so to get Sphinx running on
Windows there are a few steps that need to be taken.

    #. Install Python 2.7 from python.org__

    #. Add the Python directory to the :envvar:`PATH` (i.e., ``C:\Python27``)

    #. **On 32-bit Windows skip this step.** Install ``easy_install`` following
       `these instructions <http://pypi.python.org/pypi/setuptools#windows>`_

    #. From the command prompt type ``C:\> easy_install -U Sphinx``

    #. Add the Python /Scripts directory to the :envvar:`PATH` (i.e.,
       ``C:\Python27\Scripts``)

    #. Test ``sphinx-build`` is available by typing ``c:\> sphinx-build``


__ http://python.org/download

Once Sphinx is installed, building locally is easy; just ``rake docs:html`` from
any directory.

.. note::

    You will also notice that under the /docs directory there are two files 
    named ``Makefile`` and ``make.bat``. These are used by ReadTheDocs__ which 
    hosts the generated documentation. You do not need to run either of these 
    files.

__ http://readthedocs.org

To get started, you'll want to read the :ref:`reStructuredText Primer
<sphinx:rst-primer>`.  After you've gotten an understanding of reStructuredText,
you'll want to read about the :ref:`Sphinx-specific markup
<sphinx:sphinxmarkup>` that's used to manage metadata, indexing and
cross-references.

.. warning::

    In Sphinx and reStructuredText white space is significant. The :ref:`TOC
    tree <sphinx:toctree-directive>` for example. Will not work unless the items
    underneath are spaced properly. When coming from a non-Python background
    this can be confusing at first.

Commonly used terms
-------------------

Here are some style guidelines on commonly used terms in the documentation.

    * **FubuMVC** - Always capitalize the first letter and the MVC.

    * **NuGet** - Capitalize the G.

    * **.NET**

    * **HTTP**

    * **HTML**

reStructuredText file format
----------------------------

Since this documentation will be written and read by many people consistency in
formatting and style is a top priority. With that in mind, our documentation
should follow these reStructuredText guidelines.

    * Capitalize only the inital word and proper nouns in section titles.

    * Wrap the documentation at 80 characters unless an example is significantly
      less readable over two lines.

    * Add as much semantic markup as possible. So::

          Use ``fubu.exe alias`` for adding an alias

      Is not nearly as useful as::

          Use :ref:`fubu-alias` for adding an alias

    * Use spaces instead of tabs. The formatting of the source is as important
      as the output and since reStructuredText and Sphinx are whitespace
      sensitive it makes it much easier to keep the formatting of each file
      consistent.

Documentation folder layout
---------------------------

The documentation folder layout FubuMVC uses is borrowed from the Django__
project documentation. The structure is laid out into four directories:

__ http://djangoproject.org

intro
'''''

This folder is for documentation directed at complete beginners to FubuMVC. This
is where the initial tutorial for getting started with Fubu and a series of 
sections on FubuMVC basics are located.

topics
''''''

This folder is for more in depth and focused documentation on sections of
FubuMVC. For example, View Engines are discussed here, this would also be the
place to discuss topics such as Behavior Chains or Authentication.

reStructuredText editors
------------------------

reStructuredText is a simple text format that is easily editable with any text
editor, but it always helps to have syntax highlighting. Here are a few text
editors that can handle the reStructuredText file format.

    * Vim__ - So far, this is the best free text editor that supports the
      reStructuredText syntax. While it is a bit difficult to work with at
      first, Vim is a great text editor.

    * Emacs__ - Another text editor that supports syntax highlighting. More
      friendly to get started with than Vim and just as powerful when you get to
      know it.

__ http://vim.org
__ http://www.gnu.org/software/emacs/windows/Getting-Emacs.html#Getting-Emacs
