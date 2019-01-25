nuget install nunit.runners -Version 2.6.3 -o ./packages -FallbackSource 'https://api.nuget.org/v3/index.json'

$nunit = ".\packages\nunit.runners.2.6.3\tools\nunit-console.exe"

& $nunit /noshadow /process="Multiple" /domain="Multiple" /framework:"net-4.0" /xml:Tests.nunit.xml ".\validationweb.tests\bin\Release\ValidationWeb.Tests.dll"