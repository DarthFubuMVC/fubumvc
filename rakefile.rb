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
task :default => [:compile, :unit_test]

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

desc "Packages the Serenity bottle files"
task :bottle_serenity do
  bottles("assembly-pak src/Serenity")
end

desc "Compiles the app"
task :compile => [:restore_if_missing, :clean, :version, :bottle_serenity] do
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

desc "Runs the unit tests for Serenity"
task :serenity_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['Serenity.Testing']
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

FUBUTEMPLATE_DIR = 'fubuTemplates'
namespace :template do

  desc 'Cleans, Updates, and Zips Fubu templates'
  task :build => ['template:clean', 'template:update', 'template:slimdown_nuget_pkgs', 'template:zip_all', 'template:build_artifacts_cleanup']
  nuget = 'buildsupport/nuget.exe'


  desc 'Updates and zips default Fubu templates'
  task :zip_all do
    # first clear out old templates
    Dir['build/*Template.zip'].each {|zip_file| rm_rf zip_file}

    Dir["#{FUBUTEMPLATE_DIR}/*/"].each do |template_dir|
      Rake::Task['template:zip'].execute template_dir
    end
  end

  zip :zip do |zip, subdir|
    folder_name = File.basename subdir
    zip.directories_to_zip = [subdir]
    zip.output_file = "#{folder_name}Template.zip"
    zip.output_path = ['build']
  end

  desc 'Update Fubu templates dependencies'
  task :update do
    Dir["#{FUBUTEMPLATE_DIR}/*/"].each do |template_dir|
      packages_config = File.join(template_dir, 'src/FUBUPROJECTNAME/packages.config')
      dependencies_dir = File.join(template_dir, 'src/packages')
      unless File.exists?(packages_config)
        puts "No packages.config file for brining in dependencies into template #{template_dir}"
        return
      end
      mkdir_p(dependencies_dir)
      # Run nuget here
      sh "#{nuget} install #{packages_config} -o #{dependencies_dir} -ExcludeVersion"
      cp_r "#{template_dir}src/packages/FubuMVC/content/fubu-content/", "#{template_dir}src/FUBUPROJECTNAME/fubu-content"
    end
  end

  desc 'Cleans nuget install downloading source'
  task :slimdown_nuget_pkgs, [:dry_run] do |t, args|
    Dir.chdir(FUBUTEMPLATE_DIR) do
      files = Dir.glob('*/src/packages/**/src')
      nupkg = Dir.glob('*/src/packages/**/*.nupkg')
      files << nupkg
      args[:dry_run] ? puts(files) : rm_r(files)
    end
  end

  desc 'Cleans matching files in fubuTemplateIgnore.txt'
  task :clean, [:dry_run] do |t, args|
    Dir.chdir(FUBUTEMPLATE_DIR) do
      files = IO.readlines('fubuTemplateIgnore.txt').map{|l| Dir["**/#{l.chomp}"]}.flatten.uniq
      args[:dry_run] ? puts(files) : rm_r(files)
    end
  end

  task :build_artifacts_cleanup do
    Dir["#{FUBUTEMPLATE_DIR}/*/src/"].each do |template_dir|
      nuget_pkg_dirs = File.join(template_dir, 'packages/*/')
      fubu_content_dir = File.join(template_dir, 'FUBUPROJECTNAME/fubu-content')

      Dir[nuget_pkg_dirs].each {|nuget_pkg_dir| rm_rf nuget_pkg_dir}
      rm_rf fubu_content_dir
    end
  end
end

