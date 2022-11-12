# dotnet-compression-bug

Simple app and tests that demostrate a potential .net compression bug introduced in .net 6+

There are 3 identical tests, each targeting a different .net framework. dotnet 5 passes and while dotnet 6 and 7 will fail.

## running the app

```pws

git clone https://github.com/dale-personal/dotnet-compression-bug.git

cd dotnet-compression-bug

dotnet build

dotnet test

```
