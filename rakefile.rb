COMPILE_TARGET = "debug"
require "build_support/BuildUtils.rb"

include FileTest
require 'albacore'

RESULTS_DIR = "results"
BUILD_NUMBER = "0.1.0."  + (ENV["BUILD_NUMBER"].nil? ? '0' : ENV["BUILD_NUMBER"].to_s)
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2008 Chad Myers, Jeremy D. Miller, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_VERSION = "v3.5"

versionNumber = ENV["BUILD_NUMBER"].nil? ? 0 : ENV["BUILD_NUMBER"]

props = { :archive => "build" }

desc "Compiles, unit tests, generates the database"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Update the version information for the build"
assemblyinfotask :version do |asm|
  asm.version = BUILD_NUMBER
  asm.custom_attributes :InformationalVersion => BUILD_NUMBER
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO

  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  asm.description = "Commit: " + commit
end

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	Dir.mkdir props[:archive] unless exists?(props[:archive])
end

desc "Compiles the app"
task :compile => [:clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_VERSION
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloWorld", :webVirDir => "localhost/xyzzyplugh"
    
  outDir = "src/FubuMVC.StructureMap/bin/#{COMPILE_TARGET}"
    
  Dir.glob(File.join(outDir, "*.{dll,pdb}")){|file| 		
	copy(file, props[:archive]) if File.file?(file)
  }
end

desc "Runs unit tests"
task :test => [:unit_test]

desc "Runs unit tests"
task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuMVC.Tests', 'HtmlTags.Testing']  
end

desc "Target used for the CI server"
task :ci => [:unit_test,:zip]

desc "ZIPs up the build results"
ziptask do |zip|
	zip.directories_to_zip = ["build"]
	zip.output_file = 'fubumvc.zip'
	zip.output_path = 'build'
end