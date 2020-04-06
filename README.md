# MDMProject
Maska dla medyka - https://www.maskadlamedyka.pl/

Aby uruchmić projekt musimy pobrać/zainstalować ms visual studio 2019
Następnie skopiować link z repozytorium do visual studio, musimy zrobić clone. 
Wybieramy opcję "Clone or check out code"

Następnie trzeba uruchomić plik MDMsolution.sln

Jeśli po uruchmieniu poajwi się błąd:
Could not find a part of the path … bin\roslyn\csc.exe

proszę wykononać

run this in the Package Manager Console:
Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r

następnie uruchomić
