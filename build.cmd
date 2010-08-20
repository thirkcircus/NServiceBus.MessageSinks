@echo off
SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\V3.5;

if not exist output ( mkdir output )

echo Compiling
msbuild /nologo /verbosity:quiet src/NServiceBus.MessageSinks.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/NServiceBus.MessageSinks.sln /p:Configuration=Release

echo Copying
copy .\src\proj\NServiceBus.MessageSinks\bin\Release\NServiceBus.MessageSinks.* output
copy .\src\proj\NServiceBus.MessageSinks.AutofacConfiguration\bin\Release\NServiceBus.MessageSinks.AutofacConfiguration.* output

echo Cleaning
msbuild /nologo /verbosity:quiet src/NServiceBus.MessageSinks.sln /p:Configuration=Release /t:Clean

echo Done