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
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, Joshua Arnold, Joshua Flanagan, Chad Myers, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
	
	sln.integration_test = ['FubuMVC.IntegrationTesting']
	sln.ci_steps = [:integration_test]
end


desc "Unit and Integration Tests"
task :full => [:default, :integration_test]

desc "Target used for CI on Mono"
task :mono_ci => [:compile, unit_test, :integration_test]


