COMPILE_TARGET = ENV['config'].nil? ? "Debug" : ENV['config'] # Keep this in sync w/ VS settings since Mono is case-sensitive
CLR_TOOLS_VERSION = "v4.0.30319"

buildsupportfiles = Dir["#{File.dirname(__FILE__)}/buildsupport/*.rb"]
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
task :default => [:compile, :unit_test, "template:build"]

desc "Target used for the CI server"
task :ci => [:update_all_dependencies, :default, "template:build", :history, :publish]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_VERSION + ".0"
  
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
task :compile => [:restore_if_missing, :clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuMVC.sln', :clrversion => CLR_TOOLS_VERSION
  #AspNetCompilerRunner.compile :webPhysDir => "src/FubuMVC.HelloWorld", :webVirDir => "localhost/xyzzyplugh"

  
  
  #copyOutputFiles "src/FubuMVC.StructureMap/bin/#{COMPILE_TARGET}", "*.{dll,pdb}", props[:stage]
  #copyOutputFiles "src/FubuMVC.GettingStarted/bin/#{COMPILE_TARGET}", "*.{dll,pdb}", props[:stage]

  #copyOutputFiles "src/FubuLocalization/bin/#{COMPILE_TARGET}", "FubuLocalization.{dll,pdb}", props[:stage]
  #copyOutputFiles "src/FubuMVC.WebForms/bin/#{COMPILE_TARGET}", "FubuMVC.WebForms.{dll,pdb}", props[:stage]
  #copyOutputFiles "src/FubuMVC.Spark/bin/#{COMPILE_TARGET}", "*Spark*.{dll,pdb}", props[:stage]
  #copyOutputFiles "src/FubuMVC.Deployers/bin/#{COMPILE_TARGET}", "*Deployers*.{dll,pdb}", props[:stage]

  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "fubu.exe", props[:stage]
  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "Bottles*.{dll,pdb,exe}", props[:stage]
  copyOutputFiles "src/fubu/bin/#{COMPILE_TARGET}", "fubu", props[:stage]


  target = COMPILE_TARGET.downcase
  bottles("create-pak src/FubuMVC.Deployers build/fubumvc-deployers.zip -target #{target}")
  
  outputDir = "src/FubuMVC.Diagnostics/bin"
  packer = ILRepack.new :out => "src/FubuMVC.Diagnostics/bin/FubuMVC.Diagnostics.dll", :lib => outputDir
  packer.merge :lib => outputDir, :refs => ['FubuMVC.Diagnostics.dll', 'Newtonsoft.Json.dll']
  bottles("create-pak src/FubuMVC.Diagnostics build/fubumvc-diagnostics.zip -target #{target}")
  
  bottles("create-pak src/FubuMVC.GettingStarted build/basic-getting-started.zip -target #{target}")
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
  runner.executeTests ['FubuMVC.Tests', 'FubuMVC.WebForms.Testing', 'FubuMVC.Spark.Tests']
end


desc "Runs the StoryTeller suite of end to end tests.  IIS must be running first"
task :storyteller => [:compile] do
  storyteller("Storyteller.xml output/st-results.htm")
end

desc "Set up the virtual directories for the HelloWorld applications"
task :virtual_dir => [:compile] do
  fubu("createvdir src/FubuMVC.HelloWorld helloworld")
  fubu("createvdir src/FubuMVC.HelloFubuSpark hellofubuspark")
end

desc "ZIPs up the build results"
zip :package do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = 'fubumvc.zip'
	zip.output_path = [props[:artifacts]]
end
desc "Bundles up the packaged content in FubuFastPack"
task :bundle_getting_started do
  fubu("assembly-pak src/FubuMVC.GettingStarted -projfile FubuMVC.GettingStarted.csproj")
end

def self.bottles(args)
  bottles = Platform.runtime(Nuget.tool("Bottles.Tools", "BottleRunner.exe"))
  sh "#{bottles} #{args}"
end

def self.storyteller(args)
st = Platform.runtime(Nuget.tool("Storyteller", "StorytellerRunner.exe")) 
sh "#{st} #{args}"
end

desc "Runs the StoryTeller UI"
task :run_st do
  st = Platform.runtime(Nuget.tool("Storyteller", "StorytellerUI.exe"))
  sh st 
end

def self.fubu(args)
  fubu = Platform.runtime("src/fubu/bin/#{COMPILE_TARGET}/fubu.exe")
  sh "#{fubu} #{args}" 
end

FUBUTEMPLATE_DIR = 'fubuTemplate'
namespace :template do

  desc "Cleans, Updates, and Zips fubuTemplate"
  task :build => ["template:clean", "template:update", "template:zip"]

  desc "Updates and zips default FubuTemplate"
  zip :zip do |zip|
    zip.directories_to_zip = [FUBUTEMPLATE_DIR]
    zip.output_file = 'fubuTemplate.zip'
    zip.output_path = ['build']
  end

  desc "Update fubuTemplate dependencies"
  task :update do
    path = File.join(FUBUTEMPLATE_DIR, "/fubuDependencies.txt")
    dependencies_dir = File.join(FUBUTEMPLATE_DIR, 'lib/FubuMVC')
    unless File.exists?(path)
      puts "No fubuDependencies.txt file"
      return
    end
    mkdir_p(dependencies_dir)
    IO.readlines(path).each do |l|
      cp "build/#{l.chomp}", "#{dependencies_dir}/#{l.chomp}"
    end
  end

  desc "Cleans matching files in fubuTemplateIgnore.txt"
  task :clean, [:dry_run] do |t, args|
    Dir.chdir(FUBUTEMPLATE_DIR) do
      files = IO.readlines('fubuTemplateIgnore.txt')
              .map{|l| Dir["**/#{l.chomp}"]}
              .flatten
              .uniq
      args[:dry_run] ? puts(files) : rm_r(files)
    end
  end
end

