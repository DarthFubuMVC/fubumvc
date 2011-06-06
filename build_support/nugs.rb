namespace :nug do
	@nuget = "lib/nuget.exe"
	@nugroot = File.expand_path("/nugs")
	@dependencies = ['FubuCore','Bottles', 'FubuLocalization']
	
	desc "Build the nuget package"
	task :build do
		FileList["packaging/nuget/*.nuspec"].each do |spec|
		  sh "#{@nuget} pack #{spec} -o #{ARTIFACTS} -Version #{BUILD_NUMBER} -Symbols"
		end
	end
	
	desc "pulls new NuGet updates from your local machine"
	task :pull, [:location] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
		
		@dependencies.each do |f|
			sh "#{@nuget} install #{f} /Source #{@nugroot} /ExcludeVersion /OutputDirectory .\\lib"
		end
	end
		
	desc "pushes new NuGet udates to your local machine"
	task :push, [:location] => [:build] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
		
		FileUtils.makedirs(@nugroot)
		
		Dir["#{ARTIFACTS}/*.nupkg"].each do |fn|
			puts "Copying package #{fn} to #{@nugroot}"
			FileUtils.cp fn, @nugroot
		end
	end

	def unzip_file (file, destination)
	  require 'rubygems'
	  require 'zip/zip'
	  Zip::ZipFile.open(file) { |zip_file|
	   zip_file.each { |f|
		 f_path=File.join(destination, f.name)
		 FileUtils.mkdir_p(File.dirname(f_path))
		 zip_file.extract(f, f_path) unless File.exist?(f_path)
	   }
	  }
	end

	desc "Pushes nuget packages to the official feed"
		task :release do
		require 'open-uri'

		mkdir_p 'packaging/release'
		rm_r Dir.glob("packaging/release/*.*")

		artifact_url = "http://teamcity.codebetter.com/guestAuth/repository/downloadAll/#{@teamcity_build_id}/.lastSuccessful/artifacts.zip"
		artifact = open(artifact_url)
		unzip_file artifact.path, "packaging/release"
		FileList['packaging/release/*.nupkg'].exclude(".symbols.nupkg").each do |nupkg|
		  sh "#{@nuget} push #{nupkg}" do |ok, res|
			puts "May not have published #{nupkg}" unless ok
		  end
		end
	end
end
