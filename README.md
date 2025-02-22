# EpicLib
A cross platform library for reading and writing data from Super Mario Kart based on EpicEdit intended to make development easier.

This code is essentially a port from EpicEdit's rom folder with minor changes to bring it up to date with .net 8.0 and removing Windows-only features. Little to no error checking has been done on this library, so many things may be broken. Please create an issue if you find any bugs.

## Basic Usage
```cs
using EpicLib;
// Loading
var game = new Game("path.sfc");

// Saving
game.SaveRom("path.sfc");
```
