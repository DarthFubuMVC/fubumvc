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
multiple target formats such as HTML, PDF, ePub, and many other formats while
the source documentation is easily stored in version control.

__ http://sphinx.pocoo.org

Installing Sphinx on Windows
''''''''''''''''''''''''''''

The Sphinx documentation system is built in Python, so to get Sphinx running on
Windows there are a few steps that need to be taken.

    #. Install Python 2.7 from python.org__

    #. Add the Python directory to the :envvar:`PATH` (i.e., ``C:\Python27``)

    #. Add the Python /Scripts directory to the :envvar:`PATH` (i.e.,
       ``C:\Python27\Scripts``)

    #. From the command prompt type ``C:\> easy_install -U Sphinx``

__ http://python.org/download

.. warning::

    On 64-bit Windows, follow `these instructions to get easy_install intalled
    <http://pypi.python.org/pypi/setuptools#windows>`_ then run ``easy_install 
    -U Sphinx``

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

ref
'''

Reference documentation. This is the in-depth details of features of FubuMVC.
The documentation for :doc:`/ref/fubuexe` is here. This would also be a great
place to document what is provided by StructureMap to the FubuRegistry.

internals
'''''''''

This is where documentation on how FubuMVC's internals work. This is targeted at
developers looking to contribute to FubuMVC itself as well as existing users
looking to see how FubuMVC works. For example, this is where documentation for
how to add support for additional view engines or containers would go.

Example
-------

So how does all of this work together? Lets take a look with a quick example:

First, the ``ref/fubuexe.rst`` document could have a layout like this:

.. code-block:: rst

    ========
    Fubu.exe
    ========

    ...
    This is some documentation that does not go past 80 characters wide so that
    the text is more readable when viewed in a text editor. It also uses spaces
    instead of tabs to keep things consistent as well.
    ...

    .. _available-commands:

    Available commands
    ==================

    .. _deprecated-commands:

    Deprecated commands
    ===================

    ...

Next, the ``topics/fubuexe.rst`` could contain something like this:

.. code-block:: rst

    You can access a :ref:`listing of all available commands
    <available-commands>``. For a list of deprecated commands see
    :ref:`deprecated-commands`.

    You can find both in the :doc:`fubu.exe reference document </ref/fubuexe>`.

This shows the usage of the Sphinx :rst:role:`doc` cross reference element. This
is when we want to reference another document as a whole as opposed to the
:rst:role:`ref` element which is for when we want to link to an arbitrary
location in a document.

For more examples of how to structure the documentation and how to format the
text, please feel free to look at the source of any of the other guides in the
documentation.

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
