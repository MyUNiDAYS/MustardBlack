# MustardBlack

[![Build status](https://ci.appveyor.com/api/projects/status/okhpmk0tob5b2dix?svg=true)](https://ci.appveyor.com/project/nullseed/mustardblack)

## Install dependencies

```
.paket\paket.exe restore
```

## Building

After building the solution in `Release` configuration, use the command below to create NuGet packages:

```
.paket\paket pack . --version <VERSION_NUMBER>
```
