PLog &ndash; a tool for viewing Android log and more
===

## Setup
* Visual Studio 2022 Community or Rider with F# support.
* .NET Core SDK 6.0.

## Build
* Open `PLog.sln` with your Visual Studio.
* To build macOS version:
    * Must be built on macOS.
    * Build project `PLog.Mac`.
    * Alternatively, you can run `build.mac.sh`.
* To build Windows version:
    * Must be built on Windows.
    * .NET Framework 4.6.2 and Visual Studio 2022 Community are required.
    * Build the project `PLog.Win` in Visual Studio.
    * Alternatively, you can run `build.win.bat`, which automatically sets up the Visual Studio 2022 environment.

## Pack
* Make sure the two projects above have been built.
* Run `./pack.sh [version]` to copy all output files/folders to the `build` folder. For example: `./pack.sh 8.5`.
* See the artifacts in the `build` folder.
