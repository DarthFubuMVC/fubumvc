namespace :nug do
	@nuget = "lib/nuget.exe"
	
	@packname = 'fubumvc'
	@files = ['Fubu','FubuCore','FubuLocalization','FubuMVC.Core','FubuMVC.StructureMap','FubuMVC.WebForms'].map {|name| name + ".dll"}
	@nugroot = File.expand_path("/nugs")
	
	desc "Build the nuget package"
	task :build do
		sh "#{@nuget} pack packaging/nuget/fubumvc.nuspec -o #{ARTIFACTS} -Version #{BUILD_NUMBER}"
	end

	desc "pulls new NuGet updates from your local machine"
	task :pull, [:location] => [:build] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
		
		@files.each do |f|
			#copy to 
		end
	end
		
	desc "pushes new NuGet udates to your local machine"
	task :push, [:location] => [:build] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
			
		Dir["#{ARTIFACTS}/*.nupkg"].each do |fn|		
			FileUtils.cp fn, @nugroot
		end
	end
	
end