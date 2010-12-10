@SETLOCAL
@set rakeversion=""
@FOR /f "tokens=2" %%a in ('rake --version') do @set rakeversion=%%a 
@IF /I NOT "%rakeversion%" EQU "version " (
@ENDLOCAL
@ECHO *** Installing Rake
@call gem install rake --no-rdoc --no-ri
)
@ENDLOCAL

@ECHO *** Installing RubyZip
@call gem install rubyzip --no-rdoc --no-ri

@ECHO *** Installing Albacore (build support tools)
@call gem install albacore --no-rdoc --no-ri
