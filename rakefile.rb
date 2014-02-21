require 'bundler/setup'
require 'fuburake'

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
end


desc "Unit and Integration Tests"
task :full => [:default, :integration_test]

desc "Target used for CI on Mono"
task :mono_ci => [:compile, :unit_test, :integration_test]


