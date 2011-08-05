namespace :docs do

  DOC_DIR="docs"
  DOCS_BUILD_DIR="#{DOC_DIR}/build"
  DOC_SOURCE="#{DOC_DIR}/source"
  SPHINXOPTS=ENV['SPHINXOPTS']
  ALLSPHINXOPTS="#{SPHINXOPTS} -d #{DOCS_BUILD_DIR}/doctrees #{DOC_SOURCE}"

  def run_sphinx(args)
    sh %{sphinx-build #{args}} do |ok, res|
      if ! ok
        p res
      end
    end
  end

  desc "to make standalone HTML files"
  task :html do
    run_sphinx "-b html #{ALLSPHINXOPTS} #{DOCS_BUILD_DIR}/html"
  end

  desc "to make HTML files named index.html in directories"
  task :dirhtml do
  end

  desc "to make a single large HTML file"
  task :singlehtml do
  end

  desc "to make pickle files"
  task :pickle do
  end

  desc "to make JSON files"
  task :json do
  end

  desc "to make HTML files and a HTML help project"
  task :htmlhelp do
  end

  desc "to make HTML files and a qthelp project"
  task :qthelp do
  end

  desc "to make HTML files and a Devhelp project"
  task :devhelp do
  end

  desc "to make an epub"
  task :epub do
  end

  desc "to make LaTeX files, you can set PAPER=a4 or PAPER=letter"
  task :latex do
  end

  desc "to make LaTeX files and run them through pdflatex"
  task :latexpdf do
  end

  desc "to make text files"
  task :text do
  end

  desc "to make manual pages"
  task :man do
  end

  desc "to make an overview of all changed/added/deprecated items"
  task :changes do
  end

  desc "to check all external links for integrity"
  task :linkcheck do
  end

  desc "to run all doctests embedded in the documentation (if enabled)"
  task :doctest do
  end
end
