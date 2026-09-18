<div align="center">
  <img src="Assets/bo3_sound_suite.png" alt="Logo BO3 Sound Suite" width="120" />

# BO3 Sound Suite

### Les sons de vos maps Black Ops III, du fichier audio au SZC

Convertissez vos sons au format WAV BO3, créez leurs alias CSV et ajoutez le CSV à votre map depuis une seule application Windows.

[![Dernière version](https://img.shields.io/github/v/release/GaLeX-Le-Penguin/BO3-Sound-Suite?label=derni%C3%A8re%20version)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)
[![Téléchargements](https://img.shields.io/github/downloads/GaLeX-Le-Penguin/BO3-Sound-Suite/total?label=t%C3%A9l%C3%A9chargements)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases)
![Windows x64](https://img.shields.io/badge/Windows-x64-0078D4?logo=windows)
![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)

### [⬇️ Télécharger BO3 Sound Suite](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)

[Versions disponibles](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases) · [Signaler un problème](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues)
</div>

## À propos

BO3 Sound Suite réunit les trois étapes de préparation des sons pour **Call of Duty: Black Ops III Mod Tools**. L'interface est disponible en français et en anglais et s'ouvre directement sur le convertisseur audio.

```text
Fichiers audio  →  WAV BO3  →  Sound aliases CSV  →  Fichier SZC de la map
```

## Fonctionnalités

### 🔊 Conversion audio

- Import par sélection ou glisser-déposer de fichiers WAV, MP3, AIFF, AIF et WMA.
- Conversion en WAV BO3 **48 kHz / PCM 16 bits**.
- Sortie par défaut dans **le même dossier que le son d'origine**, y compris hors de `sound_assets`.
- Chemin de sortie modifiable pour chaque fichier et conversion de plusieurs sons en une opération.
- Option de sauvegarde avant remplacement d'un WAV existant, désactivée par défaut.
- Détection des fichiers manquants et des conflits de sortie, avec journal de conversion coloré.

### 📋 Sound aliases CSV

- Ajout, suppression et duplication des entrées de sons.
- Presets pour effets 2D/3D, voix, musique, ambiance et interface.
- Réglage de l'alias, du volume, de la boucle et des colonnes BO3.
- Inspection des WAV et aperçu du CSV avant enregistrement.
- Ouverture rapide des dossiers BO3 `sound_assets` et `share/raw/sound/aliases` lorsqu'ils sont détectés.

### 🗺️ Éditeur SZC

- Ouverture d'un SZC par sélection ou glisser-déposer.
- Ajout ou remplacement du bloc `ALIAS` qui référence le CSV de la map.
- Modification de `Name` et `Filename`, avec accès au contenu brut.
- Sauvegarde dans le dossier de la map et option de copie de sauvegarde.

## Installation

1. Ouvrez la [dernière release](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest).
2. Téléchargez **`BO3-Sound-Suite-v1.1.13-win-x64.zip`** dans la section *Assets*.
3. Extrayez l'archive et lancez **`BO3 Sound Suite.exe`**.

La version distribuée est un exécutable autonome pour **Windows x64**. Elle ne nécessite pas d'installation séparée de .NET ni de ffmpeg.

> [!NOTE]
> Convertir un WAV dans son dossier d'origine remplace ce fichier. Activez l'option de sauvegarde dans l'application si vous souhaitez conserver l'ancienne version.

## Compiler le projet

Le code source est disponible directement dans ce dépôt. Ouvrez `BO3SoundSuite.sln` avec Visual Studio et le SDK .NET 8, restaurez les packages NuGet, puis compilez en Release. Le projet utilise NAudio 2.2.1 et génère un exécutable autonome dans `dist` lors d'une compilation normale.

Les détails des modules et l'historique technique se trouvent dans [docs/DETAILS.md](docs/DETAILS.md).

## Signaler un problème

Ouvrez une [issue GitHub](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues) en indiquant la version de l'application, le format du fichier audio concerné, les étapes pour reproduire le problème et le message affiché dans le journal.

---

<div align="center">
  Développé par <strong>GLX</strong> pour les créateurs de maps Black Ops III.<br />
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest">⬇️ Télécharger</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases">Releases</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues">Issues</a>
</div>
