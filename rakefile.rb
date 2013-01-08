# require 'bundler/setup'

COMPILE_TARGET = ENV['config'].nil? ? "Debug" : ENV['config'] # Keep this in sync w/ VS settings since Mono is case-sensitive
CLR_TOOLS_VERSION = "v4.0.30319"

buildsupportfiles = Dir["#{File.dirname(__FILE__)}/buildsupport/*.rb"]

if( ! buildsupportfiles.any? )
  # no buildsupport, let's go get it for them.
  sh 'git submodule update --init' unless buildsupportfiles.any?
  buildsupportfiles = Dir["#{File.dirname(__FILE__)}/buildsupport/*.rb"]
end

# nope, we still don't have buildsupport. Something went wrong.
raise "Run `git submodule update --init` to populate your buildsupport folder." unless buildsupportfiles.any?

buildsupportfiles.each { |ext| load ext }

include FileTest
require 'albacore'
load "VERSION.txt"

RESULTS_DIR = "results"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2008-2010 Chad Myers, Jeremy D. Miller, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
BUILD_DIR = File.expand_path("build")
ARTIFACTS = File.expand_path("artifacts")

@teamcity_build_id = "bt24"
tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
BUILD_NUMBER = "#{BUILD_VERSION}.#{build_revision}"

props = { :stage => BUILD_DIR, :artifacts => ARTIFACTS }

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Unit and Integration Tests"
task :full => [:default, :integration_test]

desc "Target used for the CI server"
task :ci => [:update_all_dependencies, :default, :integration_test, :history, :package]

desc "Target used for CI on Mono"
task :mono_ci => [:update_all_dependencies, :compile, :mono_unit_test, :integration_test]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_NUMBER
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{BUILD_NUMBER}']" unless tc_build_number.nil?
  puts "Version: #{BUILD_NUMBER}" if tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = PRODUCT
  asm.description = BUILD_NUMBER
  asm.version = asm_version
  asm.file_version = BUILD_NUMBER
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

desc "Prepares the working directory for a new build"
task :clean => [:update_buildsupport] do
	
	FileUtils.rm_rf props[:stage]
    # work around nasty latency issue where folder still exists for a short while after it is removed
    waitfor { !exists?(props[:stage]) }
	Dir.mkdir props[:stage]
    
	Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])
end

def waitfor(&block)
  checks = 0
  until block.call || checks >10 
    sleep 0.5
    checks += 1
  end
  raise 'waitfor timeout expired' if checks > 10
end

desc "Compiles the app"
task :compile => [:restore_if_missing, :clean, :version, "docs:bottle"] do


  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_TOOLS_VERSION

  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "Bottles*.{dll,pdb,exe}", props[:stage]
  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "fubu", props[:stage]

  target = COMPILE_TARGET.downcase
end

def copyOutputFiles(fromDir, filePattern, outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file| 		
	copy(file, outDir, :preserve => true) if File.file?(file)
  } 
end

desc "Runs unit tests"
task :test => [:unit_test]

desc "Runs unit tests"
task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuMVC.Tests', 'FubuMVC.SelfHost.Testing', 'FubuMVC.StructureMap.Testing', 'FubuMVC.Autofac.Testing', 'FubuMVC.OwinHost.Testing']
end

desc "Runs some of the unit tests for Mono"
task :mono_unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuMVC.Tests', 'FubuMVC.StructureMap.Testing', 'FubuMVC.Autofac.Testing', 'FubuMVC.OwinHost.Testing']
end

desc "Runs the integration tests"
task :integration_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuMVC.IntegrationTesting']
end

desc "ZIPs up the build results"
zip :package do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = 'fubumvc.zip'
	zip.output_path = [props[:artifacts]]
end




def self.bottles(args)
  bottles = Platform.runtime(Nuget.tool("Bottles", "BottleRunner.exe"))
  sh "#{bottles} #{args}"
end


