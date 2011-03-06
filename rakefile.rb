COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
require File.dirname(__FILE__) + "/build_support/BuildUtils.rb"

include FileTest
require 'albacore'

RESULTS_DIR = "results"
BUILD_NUMBER_BASE = "0.4.0"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2008-2010 Chad Myers, Jeremy D. Miller, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

props = { :stage => File.expand_path("build"), :artifacts => File.expand_path("artifacts") }

desc "Displays a list of tasks"
task :help do
  taskHash = Hash[*(`rake.bat -T`.split(/\n/).collect { |l| l.match(/rake (\S+)\s+\#\s(.+)/).to_a }.collect { |l| [l[1], l[2]] }).flatten] 
 
  indent = "                          "
  
  puts "rake #{indent}#Runs the 'default' task"
  
  taskHash.each_pair do |key, value|
    if key.nil?  
      next
    end
    puts "rake #{key}#{indent.slice(0, indent.length - key.length)}##{value}"
  end
end

desc "Compiles, unit tests, generates the database"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_NUMBER_BASE + ".0"
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  build_number = getBuildNumber
  tc_build_number = ENV["BUILD_NUMBER"]
  puts "##teamcity[buildNumber '#{build_number}-#{tc_build_number}']" unless tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = PRODUCT
  asm.description = build_number
  asm.version = asm_version
  asm.file_version = build_number
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

def getBuildNumber
  "#{BUILD_NUMBER_BASE}.#{Date.today.strftime('%y%j')}"
end

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
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
task :compile => [:clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_TOOLS_VERSION
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloWorld", :webVirDir => "localhost/xyzzyplugh"
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloSpark", :webVirDir => "localhost/xyzzyplugh"
  
  copyOutputFiles "src/FubuMVC.StructureMap/bin/#{COMPILE_TARGET}", "*.{dll,pdb}", props[:stage]
  copyOutputFiles "src/Spark.Web.FubuMVC/bin/#{COMPILE_TARGET}", "*Spark*.{dll,pdb}", props[:stage]
  copyOutputFiles "src/FubuLocalization/bin/#{COMPILE_TARGET}", "FubuLocalization.{dll,pdb}", props[:stage]
  copyOutputFiles "src/FubuValidation/bin/#{COMPILE_TARGET}", "FubuValidation.{dll,pdb}", props[:stage]
  copyOutputFiles "src/FubuMVC.Validation/bin", "FubuMVC.Validation.{dll,pdb}", props[:stage]

  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "fubu.exe", props[:stage]
  copyOutputFiles "src/FubuFastPack/bin/#{COMPILE_TARGET}", "FubuFastPack.{dll,pdb}", props[:stage]
end

desc "Bundles up the packaged content in FubuFastPack"
task :bundle_fast_pack => [:compile] do
  sh "src/fubu/bin/#{COMPILE_TARGET}/fubu.exe assembly-pak src/FubuFastPack -projfile FubuFastPack.csproj"
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
  runner.executeTests ['FubuMVC.Tests', 'FubuCore.Testing', 'FubuLocalization.Tests', 'Spark.Web.FubuMVC.Tests', 'FubuValidation.Tests', 'FubuFastPack.Testing', 'FubuMVC.Validation.Tests']
end


desc "Runs the StoryTeller suite of end to end tests.  IIS must be running first"
task :storyteller => [:compile] do
  sh "lib/storyteller/StoryTellerRunner Storyteller.xml output/st-results.htm"
end

desc "Set up the virtual directories for the HelloWorld applications"
task :virtual_dir => [:compile] do
  sh "src/fubu/bin/#{COMPILE_TARGET}/fubu.exe createvdir src/FubuMVC.HelloWorld helloworld"
  sh "src/fubu/bin/#{COMPILE_TARGET}/fubu.exe createvdir src/FubuMVC.HelloSpark hellospark"
end

desc "Target used for the CI server"
task :ci => [:default,:package,:nuget]

desc "ZIPs up the build results"
zip :package do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = 'fubumvc_net40.zip'
	zip.output_path = [props[:artifacts]]
end


desc "Build the nuget package"
task :nuget do
	sh "lib/nuget.exe pack packaging/nuget/fubumvc.nuspec -o artifacts -Version #{getBuildNumber}"
	sh "lib/nuget.exe pack packaging/nuget/spark.web.fubumvc.nuspec -o artifacts -Version #{getBuildNumber}"
end
