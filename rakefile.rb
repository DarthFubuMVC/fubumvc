COMPILE_TARGET = "debug"
require "build_support/BuildUtils.rb"

include FileTest
require 'albacore'

RESULTS_DIR = "results"
BUILD_NUMBER_BASE = "0.1.0"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2008 Chad Myers, Jeremy D. Miller, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

props = { :archive => "build" }

desc "Compiles, unit tests, generates the database"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_NUMBER_BASE + ".0"
  
  begin
	gittag = `git describe --long`.chomp 	# looks something like v0.1.0-63-g92228f4
    gitnumberpart = /-(\d+)-/.match(gittag)
    gitnumber = gitnumberpart.nil? ? '0' : gitnumberpart[1]
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
    gitnumber = "0"
  end
  build_number = "#{BUILD_NUMBER_BASE}.#{gitnumber}"
  tc_build_number = ENV["BUILD_NUMBER"]
  puts "##teamcity[buildNumber '#{build_number}-#{tc_build_number}']" unless tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = "#{PRODUCT} #{gittag}"
  asm.description = build_number
  asm.version = asm_version
  asm.file_version = build_number
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	Dir.mkdir props[:archive] unless exists?(props[:archive])
end

desc "Compiles the app"
task :compile => [:clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_TOOLS_VERSION
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloWorld", :webVirDir => "localhost/xyzzyplugh"
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloSpark", :webVirDir => "localhost/xyzzyplugh"
  
  copyOutputFiles "src/FubuMVC.StructureMap/bin/#{COMPILE_TARGET}", "*.{dll,pdb}", props[:archive]
  copyOutputFiles "src/FubuMVC.View.Spark/bin/#{COMPILE_TARGET}", "*Spark.{dll.pdb}", props[:archive]
  copyOutputFiles "src/FubuMVC.UI/bin/#{COMPILE_TARGET}", "FubuMVC.UI.{dll,pdb}", props[:archive]
  copyOutputFiles "src/Spark.Web.FubuMVC/bin/#{COMPILE_TARGET}", "*Spark*.{dll,pdb}", props[:archive]
end

def copyOutputFiles(fromDir, filePattern, outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file| 		
	copy(file, outDir) if File.file?(file)
  } 
end

desc "Runs unit tests"
task :test => [:unit_test]

desc "Runs unit tests"
task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuMVC.Tests', 'FubuCore.Testing', 'HtmlTags.Testing', 'Spark.Web.FubuMVC.Tests']
end

desc "Target used for the CI server"
task :ci => [:unit_test,:zip]

desc "ZIPs up the build results"
zip do |zip|
	zip.directories_to_zip = [props[:archive]]
	zip.output_file = 'fubumvc.zip'
	zip.output_path = 'build'
end
