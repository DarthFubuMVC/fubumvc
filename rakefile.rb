begin
  require 'bundler/setup'
  require 'fuburake'
rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end


@solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuMVC.sln'
	}
				 
	sln.assembly_info = {
		:product_name => "FubuMVC",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, Joshua Arnold, Corey Kaylor, Joshua Flanagan, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
	sln.bottles_enabled = false # has to be all special in FubuMVC because of the zip package testing
	
	sln.integration_test = ['FubuMVC.IntegrationTesting']
	sln.ci_steps = [:integration_test]
	
	sln.defaults = [:ilrepack]
end


desc "Unit and Integration Tests"
task :full => [:default, :integration_test]

desc "Target used for CI on Mono"
task :mono_ci => [:compile, :unit_test, :integration_test]

require_relative 'ILRepack'

desc "Ilmerge the appropriate OWIN and Katana assemblies"
task :ilrepack do
	merge_owin_types("src/FubuMVC.OwinHost/bin/#{@solution.compilemode}", 'FubuMVC.OwinHost.dll')
	#merge_owin_listener("src/FubuMVC.Katana/bin/#{@solution.compilemode}", "Microsoft.Owin.dll")
	
	#merge_katana("src/FubuMVC.Katana/bin/#{@solution.compilemode}", 'FubuMVC.Katana.dll')
	
	owin_file = "src/FubuMVC.OwinHost/bin/#{@solution.compilemode}/FubuMVC.OwinHost.dll"
	katana_file = "src/FubuMVC.Katana/bin/#{@solution.compilemode}/FubuMVC.Katana.dll"
	
	
	
	FileUtils.cp owin_file,  "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}"
	#FileUtils.cp katana_file,  "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}"
	
	#FileUtils.remove_file "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}/Microsoft.Owin.dll"
	#FileUtils.remove_file "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}/Microsoft.Owin.Hosting.dll"
	#FileUtils.remove_file "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}/Microsoft.Owin.Host.HttpListener.dll"
	FileUtils.remove_file "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}/Owin.Types.dll"
	FileUtils.remove_file "src/FubuMVC.IntegrationTesting/bin/#{@solution.compilemode}/Owin.Extensions.dll"
end

Rake::Task[:integration_test].enhance [:ilrepack]

def merge_owin_types(dir, assembly)
	output = File.join(dir, assembly)
	packer = ILRepack.new :out => output, :lib => dir
	packer.merge :lib => dir, :refs => [assembly, 'Owin.Types.dll'], :clrversion => @solution.options[:clrversion]
	packer.merge :lib => dir, :refs => [assembly, 'Owin.Extensions.dll'], :clrversion => @solution.options[:clrversion]
end

def merge_owin_listener(dir, assembly)
	output = File.join(dir, assembly)
	packer = ILRepack.new :out => output, :lib => dir
	
	packer.merge :lib => dir, :refs => [assembly, 'Microsoft.Owin.Host.HttpListener.dll', 'Microsoft.Owin.Hosting.dll'], :clrversion => @solution.options[:clrversion]
	#packer.merge :lib => dir, :refs => [assembly, ], :clrversion => @solution.options[:clrversion]
end

def merge_katana(dir, assembly)
	output = File.join(dir, assembly)
	packer = ILRepack.new :out => output, :lib => dir
	packer.merge :lib => dir, :refs => [assembly, 'Microsoft.Owin.dll'], :clrversion => @solution.options[:clrversion]
end
