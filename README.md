# MustardBlack

[![Build status](https://ci.appveyor.com/api/projects/status/u4uo9h4i6eg5smuq?svg=true)](https://ci.appveyor.com/project/trullock/mustardblack)

## Install dependencies

```
.paket\paket.exe restore
```

## Building

After building the solution in `Release` configuration, use the command below to create NuGet packages:

```
.paket\paket pack . --version <VERSION_NUMBER>
```
