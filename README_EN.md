<div align="center">
  <img src="Assets/bo3_sound_suite.png" alt="BO3 Sound Suite logo" width="120" />

[🇫🇷 Français](README.md) · 🇬🇧 **English** · [🇪🇸 Español](README_ES.md) · [🇩🇪 Deutsch](README_DE.md) · [🇮🇹 Italiano](README_IT.md)

# BO3 Sound Suite

### Audio for your Black Ops III maps, from source file to SZC

Convert sounds to BO3 WAV, create sound alias CSV files, and add the CSV to your map from one Windows application.

[![Latest release](https://img.shields.io/github/v/release/GaLeX-Le-Penguin/BO3-Sound-Suite?label=latest%20release)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/GaLeX-Le-Penguin/BO3-Sound-Suite/total?label=downloads)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases)
![Windows x64](https://img.shields.io/badge/Windows-x64-0078D4?logo=windows)
![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)

### [⬇️ Download BO3 Sound Suite](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)

[All releases](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases) · [Report an issue](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues)
</div>

## About

BO3 Sound Suite brings together the three audio preparation steps for **Call of Duty: Black Ops III Mod Tools**. The application interface is available in French and English and opens on the audio converter.

```text
Audio files  →  BO3 WAV  →  Sound alias CSV  →  Map SZC file
```

## Features

### 🔊 Audio conversion

- Select or drag and drop WAV, MP3, AIFF, AIF, and WMA files.
- Convert to **48 kHz / 16-bit PCM** BO3 WAV.
- Save converted files by default in **the same folder as each original sound**, including files outside `sound_assets`.
- Change the output path per file and convert multiple sounds in one batch.
- Optionally back up an existing WAV before replacement; backups are off by default.
- Detect missing inputs and output conflicts, with a color-coded conversion log.

### 📋 Sound alias CSV

- Add, remove, and duplicate sound entries.
- Use presets for 2D/3D effects, voice, music, ambience, and interface sounds.
- Edit aliases, volume, looping, and BO3 CSV fields.
- Inspect WAV files and preview the CSV before saving.
- Open BO3 `sound_assets` and `share/raw/sound/aliases` folders quickly when detected.

### 🗺️ SZC editor

- Open an SZC file through the picker or drag and drop.
- Add or replace the `ALIAS` block that references your map's CSV.
- Edit `Name` and `Filename`, or work directly with the raw SZC content.
- Save to the map folder, with an optional backup.

## Installation

1. Open the [latest release](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest).
2. Download **`BO3-Sound-Suite-v1.1.13-win-x64.zip`** from *Assets*.
3. Extract the archive and run **`BO3 Sound Suite.exe`**.

The distributed version is a self-contained **Windows x64** executable. It does not require a separate .NET installation or ffmpeg.

> [!NOTE]
> Converting a WAV in its original folder replaces that file. Enable the backup option in the application if you want to keep the previous version.

## Build from source

The source code is in this repository. Open `BO3SoundSuite.sln` in Visual Studio with the .NET 8 SDK, restore NuGet packages, and build in Release mode. The project uses NAudio 2.2.1 and generates a self-contained executable in `dist` during a normal build.

Technical module details and version history are in [docs/DETAILS.md](docs/DETAILS.md) (French).

## Report an issue

Open a [GitHub issue](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues) with the application version, affected audio format, steps to reproduce, and the message shown in the log.

---

<div align="center">
  Developed by <strong>GLX</strong> for Black Ops III map creators.<br />
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest">⬇️ Download</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases">Releases</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues">Issues</a>
</div>
