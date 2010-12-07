COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
require "build_support/BuildUtils.rb"

include FileTest
require 'albacore'

RESULTS_DIR = "results"
BUILD_NUMBER_BASE = "0.4.0"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2008 Chad Myers, Jeremy D. Miller, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

props = { :stage => "build", :stage35 => "build35", :artifacts => "artifacts" }

desc "Compiles, unit tests, generates the database"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test, :compile35]

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
	Dir.mkdir props[:stage] unless exists?(props[:stage])
	Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])
end

desc "Compiles the app"
task :compile => [:clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_TOOLS_VERSION
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloWorld", :webVirDir => "localhost/xyzzyplugh"
  AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloSpark", :webVirDir => "localhost/xyzzyplugh"
  
  copyOutputFiles "src/FubuMVC.StructureMap/bin/#{COMPILE_TARGET}", "*.{dll,pdb}", props[:stage]
  copyOutputFiles "src/FubuMVC.UI/bin/#{COMPILE_TARGET}", "FubuMVC.UI.{dll,pdb}", props[:stage]
  copyOutputFiles "src/Spark.Web.FubuMVC/bin/#{COMPILE_TARGET}", "*Spark*.{dll,pdb}", props[:stage]
  copyOutputFiles "src/FubuLocalization/#{COMPILE_TARGET}", "FubuLocalization.{dll,pdb}", props[:stage]
end

desc "Compiles the app for .NET Framework 3.5"
task :compile35 do
  output = "bin\\#{COMPILE_TARGET}35\\"
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.Fx35.sln', :clrversion => CLR_TOOLS_VERSION,
   :properties=>[
     "TargetFrameworkVersion=v3.5",
     "OutDir=#{output}",
     "DefineConstants=\"LEGACY;TRACE\""
     ]

  Dir.mkdir props[:stage35] unless exists?(props[:stage35])
  output_nix = output.gsub('\\', '/')
  copyOutputFiles "src/FubuMVC.StructureMap/#{output_nix}", "*.{dll,pdb}", props[:stage35]
  copyOutputFiles "src/FubuMVC.UI/#{output_nix}", "FubuMVC.UI.{dll,pdb}", props[:stage35]  
  copyOutputFiles "src/FubuLocalization/#{output_nix}", "FubuLocalization.{dll,pdb}", props[:stage35]
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
  runner.executeTests ['FubuMVC.Tests', 'FubuCore.Testing', 'FubuLocalization.Tests', 'HtmlTags.Testing', 'Spark.Web.FubuMVC.Tests', 'FubuValidation.Tests', 'FubuFastPack.Testing']
end

desc "Target used for the CI server"
task :ci => [:default,:package,:package35]

desc "ZIPs up the build results"
zip :package do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = 'fubumvc_net40.zip'
	zip.output_path = [props[:artifacts]]
end

zip :package35 do |zip|
	zip.directories_to_zip = [props[:stage35]]
	zip.output_file = 'fubumvc_net35.zip'
	zip.output_path = [props[:artifacts]]
end
