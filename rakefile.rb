require 'bundler/setup'
require 'fuburake'

@solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuMVC.sln'
	}
				 
	sln.assembly_info = {
		:product_name => "FubuMVC",
		:copyright => 'Copyright 2008-2014 Jeremy D. Miller, Joshua Arnold, Corey Kaylor, Joshua Flanagan, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = false
	sln.bottles_enabled = false # has to be all special in FubuMVC because of the zip package testing
	
	sln.assembly_bottle 'FubuMVC.Diagnostics'
	sln.assembly_bottle 'FubuMVC.StructureMap'
	
	sln.integration_test = ['FubuMVC.IntegrationTesting']
	sln.ci_steps = [:integration_test, :archive_gem]
	
	sln.options[:nuget_publish_folder] = 'nupkgs'
	sln.options[:nuget_publish_url] = 'https://www.myget.org/F/fubumvc-edge/'
end

add_dependency 'ripple:publish', :integration_test


desc "Unit and Integration Tests"
task :full => [:default, :integration_test]

desc "Target used for CI on Mono"
task :mono_ci => [:compile, :unit_test, :integration_test]

desc "Replaces the existing installed gem with the new version for local testing"
task :local_gem => [:create_gem] do
	sh 'gem uninstall fubu -a -x'
	Dir.chdir 'pkg' do
	    sh 'gem install fubu'
    end
end

desc "Moves the gem to the archive folder"
task :archive_gem => [:create_gem] do
	copyOutputFiles "pkg", "*.gem", "artifacts"
end

desc "Outputs the command line usage"
task :dump_usages => [:compile] do
  sh "src/Fubu/bin/#{@solution.compilemode}/fubu.exe dump-usages fubu src/Fubu.Docs/fubu.cli.xml"
end

desc "Creates the gem for fubu.exe"
task :create_gem => [:compile] do
    require "rubygems/package"
	cleanDirectory 'bin';	
	cleanDirectory 'pkg'
	
	Dir.mkdir 'artifacts' unless Dir.exists?('artifacts')
	Dir.mkdir 'bin' unless Dir.exists?('bin')
	Dir.mkdir 'pkg' unless Dir.exists?('pkg')
	
	copyOutputFiles "src/Fubu/bin/#{@solution.compilemode}", '*.dll', 'bin'
	copyOutputFiles "src/Fubu/bin/#{@solution.compilemode}", 'Fubu.exe', 'bin'
	copyOutputFiles "src/Fubu/bin/#{@solution.compilemode}", 'chromedriver.exe', 'bin'
	FileUtils.cp_r 'templates', 'bin'
	
	FileUtils.copy 'fubu', 'bin'


	spec = Gem::Specification.new do |s|
	  s.platform    = Gem::Platform::RUBY
	  s.name        = 'fubu'
	  s.version     = @solution.options[:build_number] + '.alpha'
	  s.files = Dir['bin/**/*']
	  s.bindir = 'bin'
	  s.executables << 'fubu'
	  
	  s.summary     = 'Command line tools for FubuMVC development'
	  s.description = 'Command line tools for FubuMVC development'
	  
	  s.add_runtime_dependency "rake",["~>10.0"]
	  s.add_runtime_dependency "bundler",[">=1.3.5"]
	  
	  s.authors           = ['Jeremy D. Miller', 'Josh Arnold', 'Chad Myers', 'Joshua Flanagan']
	  s.email             = 'fubumvc-devel@googlegroups.com'
	  s.homepage          = 'http://fubu-project.org'
	  s.rubyforge_project = 'fubu'
	end   
    puts "ON THE FLY SPEC FILES"
    puts spec.files
    puts "=========="

    Gem::Package::build spec, true
	
	FileUtils.mv "fubu-#{@solution.options[:build_number]}.alpha.gem", "pkg/fubu-#{@solution.options[:build_number]}.alpha.gem"
	
end
