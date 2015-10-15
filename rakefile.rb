COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
RESULTS_DIR = "results"
BUILD_VERSION = '3.0.0'

NUGET_KEY = ENV['api_key']

include FileUtils
include FileTest

tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number 

task :ci => [:default, :integration_test, :publish]

task :default => [:test]

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf RESULTS_DIR
	FileUtils.rm_rf 'artifacts'
	Dir.mkdir 'artifacts'

	Dir.mkdir RESULTS_DIR
end

desc "Update the version information for the build"
task :version do
  asm_version = build_number
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?
  
  options = {
	:description => 'The .Net web framework and service bus that gets out of your way',
	:product_name => 'StructureMap',
	:copyright => 'Copyright 2009-2015 Jeremy D. Miller, Corey Kaylor, Joshua Arnold, et al. All rights reserved.',
	:trademark => commit,
	:version => asm_version,
	:file_version => build_number,
	:informational_version => asm_version
	
  }
  
  puts "Writing src/CommonAssemblyInfo.cs..."
	File.open('src/CommonAssemblyInfo.cs', 'w') do |file|
		file.write "using System.Reflection;\n"
		file.write "using System.Runtime.InteropServices;\n"
		file.write "[assembly: AssemblyDescription(\"#{options[:description]}\")]\n"
		file.write "[assembly: AssemblyProduct(\"#{options[:product_name]}\")]\n"
		file.write "[assembly: AssemblyCopyright(\"#{options[:copyright]}\")]\n"
		file.write "[assembly: AssemblyTrademark(\"#{options[:trademark]}\")]\n"
		file.write "[assembly: AssemblyVersion(\"#{options[:version]}\")]\n"
		file.write "[assembly: AssemblyFileVersion(\"#{options[:file_version]}\")]\n"
		file.write "[assembly: AssemblyInformationalVersion(\"#{options[:informational_version]}\")]\n"
	end
end

desc 'Compile the code'
task :compile => [:npm, :clean, :version] do
	sh "paket.exe install"
	sh "C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe src/FubuMVC.sln   /property:Configuration=#{COMPILE_TARGET} /v:m /t:rebuild /nr:False /maxcpucount:2"
end

desc 'Run the unit tests'
task :test => [:compile] do
	sh "packages/Fixie/lib/net45/Fixie.Console.exe src/FubuMVC.Tests/bin/#{COMPILE_TARGET}/FubuMVC.Tests.dll --NUnitXml results/TestResult.xml"
end

desc 'Run the integration tests'
task :integration_test => [:compile] do
    sh "packages/Fixie/lib/net45/Fixie.Console.exe src/FubuMVC.IntegrationTesting/bin/#{COMPILE_TARGET}/FubuMVC.IntegrationTesting.dll --NUnitXml results/IntegrationTestResult.xml"
    sh "packages/Fixie/lib/net45/Fixie.Console.exe src/FubuMVC.RavenDb.Tests/bin/#{COMPILE_TARGET}/FubuMVC.RavenDb.Tests.dll --NUnitXml results/RavenDbTestResult.xml"
    sh "packages/Fixie/lib/net45/Fixie.Console.exe src/FubuMVC.LightningQueues.Testing/bin/#{COMPILE_TARGET}/FubuMVC.LightningQueues.Testing.dll --NUnitXml results/LQTestResult.xml"
end

desc 'Build Nuspec packages'
task :pack => [:compile] do
	sh "nuget.exe pack packaging/nuget/fubumvc.aspnet.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/fubumvc.core.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/fubumvc.lightningqueues.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/fubumvc.ravendb.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/fubumvc.razor.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/fubumvc.spark.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/serenity.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	sh "nuget.exe pack packaging/nuget/jasperservice.nuspec -VERSION #{build_number}-alpha -OutputDirectory artifacts"
	


	
end

task :publish => [:pack] do
	sh "nuget.exe push artifacts/FubuMVC.Core.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/FubuMVC.AspNet.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/FubuMVC.LightningQueues.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/FubuMVC.RavenDb.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/FubuMVC.Razor.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/FubuMVC.Spark.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/Serenity.#{build_number}-alpha.nupkg #{NUGET_KEY} "
	sh "nuget.exe push artifacts/JasperService.#{build_number}-alpha.nupkg #{NUGET_KEY} "
end

desc "Launches the diagnostics harness for client side development"
task :diagnostics => [:compile] do
	sh 'start npm run watch'
	sh "src/Fubu/bin/#{COMPILE_TARGET}/fubu.exe run --directory src/DiagnosticsHarness -o -w --mode diagnostics"
end

desc "Unit and Integration Tests"
task :full => [:default, :integration_test]


desc "Delegates to npm install and builds the javascript for diagnostics"
task :npm do
	sh 'npm install'
	sh 'npm run build'
end


desc "Replaces the existing installed gem with the new version for local testing"
task :local_gem => [:create_gem] do
	sh 'gem uninstall fubu -a -x'
	Dir.chdir 'pkg' do
	    sh 'gem install fubu'
    end
end

desc "Moves the gem to the archive folder"
task :archive_gem => [:create_gem] do
	copyOutputFiles "pkg", "*.gem", "artifacts"
end


# 'https://www.myget.org/F/fubumvc-edge/'
# TODO -- need to add a set of tasks to test JasperService

desc "Run the storyteller specifications"
task :storyteller => [:compile] do
	sh "packages/Storyteller/tools/st.exe run src/FubuMVC.IntegrationTesting --retries 3 --results-path artifacts/stresults.htm --build #{COMPILE_TARGET}"
end

desc "Run the storyteller specifications"
task :open_st => [:compile] do
	sh "packages/Storyteller/tools/st.exe open src/FubuMVC.IntegrationTesting"
end

desc "Outputs the command line usage"
task :dump_usages => [:compile] do
  sh "src/Fubu/bin/#{COMPILE_TARGET}/fubu.exe dump-usages fubu src/Fubu.Docs/fubu.cli.xml"
end

desc "Creates the gem for fubu.exe"
task :create_gem => [:compile] do
    require "rubygems/package"
	cleanDirectory 'bin';	
	cleanDirectory 'pkg'
	
	Dir.mkdir 'artifacts' unless Dir.exists?('artifacts')
	Dir.mkdir 'bin' unless Dir.exists?('bin')
	Dir.mkdir 'pkg' unless Dir.exists?('pkg')
	
	copyOutputFiles "src/Fubu/bin/#{COMPILE_TARGET}", '*.dll', 'bin'
	copyOutputFiles "src/Fubu/bin/#{COMPILE_TARGET}", 'Fubu.exe', 'bin'
	copyOutputFiles "src/Fubu/bin/#{COMPILE_TARGET}", 'chromedriver.exe', 'bin'
	FileUtils.cp_r 'templates', 'bin'
	
	FileUtils.copy 'fubu', 'bin'


	spec = Gem::Specification.new do |s|
	  s.platform    = Gem::Platform::RUBY
	  s.name        = 'fubu'
	  s.version     = @solution.options[:build_number] + '.alpha'
	  s.files = Dir['bin/**/*']
	  s.bindir = 'bin'
	  s.executables << 'fubu'
	  
	  s.summary     = 'Command line tools for FubuMVC development'
	  s.description = 'Command line tools for FubuMVC development'
	  
	  s.add_runtime_dependency "rake",["~>10.0"]
	  s.add_runtime_dependency "bundler",[">=1.3.5"]
	  
	  s.authors           = ['Jeremy D. Miller', 'Josh Arnold', 'Chad Myers', 'Joshua Flanagan']
	  s.email             = 'fubumvc-devel@googlegroups.com'
	  s.homepage          = 'http://fubu-project.org'
	  s.rubyforge_project = 'fubu'
	end   
    puts "ON THE FLY SPEC FILES"
    puts spec.files
    puts "=========="

    Gem::Package::build spec, true
	
	FileUtils.mv "fubu-#{build_number}.alpha.gem", "pkg/fubu-#{build_number}.alpha.gem"
	
end

desc "Launches VS to the FubuMVC solution file"
task :sln do
	sh "start src/FubuMVC.sln"
end

def copyOutputFiles(fromDir, filePattern, outDir)
	Dir.glob(File.join(fromDir, filePattern)){|file|		
		copy(file, outDir) if File.file?(file)
	} 
end

def waitfor(&block)
	checks = 0
	until block.call || checks >10 
		sleep 0.5
		checks += 1
	end
	raise 'waitfor timeout expired' if checks > 10
end

def cleanDirectory(dir)
	if exists?(dir)
		puts 'Cleaning directory ' + dir
		FileUtils.rm_rf dir;
		waitfor { !exists?(dir) }
	end
	
	if dir == 'artifacts'
		Dir.mkdir 'artifacts'
	end
end

def cleanFile(file)
	File.delete file unless !File.exist?(file)
end
