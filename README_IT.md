<div align="center">
  <img src="Assets/bo3_sound_suite.png" alt="Logo BO3 Sound Suite" width="120" />

[🇫🇷 Français](README.md) · [🇬🇧 English](README_EN.md) · [🇪🇸 Español](README_ES.md) · [🇩🇪 Deutsch](README_DE.md) · 🇮🇹 **Italiano**

# BO3 Sound Suite

### L'audio delle tue mappe di Black Ops III, dal file originale allo SZC

Converti i suoni in WAV per BO3, crea file CSV con gli alias audio e aggiungi il CSV alla tua mappa da un'unica applicazione Windows.

[![Ultima versione](https://img.shields.io/github/v/release/GaLeX-Le-Penguin/BO3-Sound-Suite?label=ultima%20versione)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)
[![Scarica](https://img.shields.io/badge/Scarica-GitHub-2ea44f?logo=github)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)
![Windows x64](https://img.shields.io/badge/Windows-x64-0078D4?logo=windows)
![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)

### [⬇️ Scarica BO3 Sound Suite](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)

[Tutte le versioni](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases) · [Segnala un problema](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues)
</div>

## Informazioni

BO3 Sound Suite riunisce i tre passaggi per preparare l'audio per **Call of Duty: Black Ops III Mod Tools**. L'interfaccia dell'applicazione è disponibile in francese e inglese e si apre nel convertitore audio.

```text
File audio  →  WAV per BO3  →  CSV degli alias  →  File SZC della mappa
```

## Funzionalità

### 🔊 Conversione audio

- Seleziona o trascina file WAV, MP3, AIFF, AIF e WMA.
- Converte in WAV per BO3 a **48 kHz / PCM a 16 bit**.
- Salva i file convertiti, per impostazione predefinita, **nella stessa cartella del suono originale**, anche fuori da `sound_assets`.
- Modifica il percorso di output per ogni file e converti più suoni in un'unica operazione.
- Copia di sicurezza facoltativa prima di sostituire un WAV esistente; disattivata per impostazione predefinita.
- Rileva file di input mancanti e conflitti nei percorsi di output, con un registro di conversione a colori.

### 📋 CSV degli alias audio

- Aggiungi, elimina e duplica le voci audio.
- Usa preset per effetti 2D/3D, voce, musica, ambiente e interfaccia.
- Modifica alias, volume, ripetizione e campi CSV di BO3.
- Analizza i WAV e visualizza l'anteprima del CSV prima del salvataggio.
- Apri rapidamente le cartelle BO3 `sound_assets` e `share/raw/sound/aliases` quando vengono rilevate.

### 🗺️ Editor SZC

- Apri un file SZC tramite selezione o trascinamento.
- Aggiungi o sostituisci il blocco `ALIAS` che fa riferimento al CSV della mappa.
- Modifica `Name` e `Filename` oppure il contenuto SZC grezzo.
- Salva nella cartella della mappa, con una copia di sicurezza facoltativa.

## Installazione

1. Apri l'[ultima versione](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest).
2. Scarica **`BO3-Sound-Suite-v1.1.13-win-x64.zip`** dalla sezione *Assets*.
3. Estrai l'archivio e avvia **`BO3 Sound Suite.exe`**.

La versione distribuita è un eseguibile autonomo per **Windows x64**. Non richiede l'installazione separata di .NET o ffmpeg.

> [!NOTE]
> La conversione di un WAV nella cartella originale sostituisce quel file. Attiva l'opzione di copia di sicurezza nell'applicazione per conservare la versione precedente.

## Compilare il progetto

Il codice sorgente si trova in questo repository. Apri `BO3SoundSuite.sln` in Visual Studio con l'SDK .NET 8, ripristina i pacchetti NuGet e compila in modalità Release. Il progetto usa NAudio 2.2.1 e genera un eseguibile autonomo in `dist` durante una normale compilazione.

Dettagli tecnici e cronologia delle versioni si trovano in [docs/DETAILS.md](docs/DETAILS.md) (in francese).

## Segnalare un problema

Apri una [segnalazione su GitHub](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues) indicando la versione dell'applicazione, il formato audio interessato, i passaggi per riprodurre il problema e il messaggio mostrato nel registro.

## Licenza

Il progetto è distribuito sotto la [licenza MIT](LICENSE). Le informazioni su NAudio e sugli altri componenti di terze parti si trovano in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

## Copyright

Copyright © 2026 Galex (GLX). BO3 Sound Suite e la sua identità visiva sono sviluppati da GLX. I componenti di terze parti mantengono i rispettivi diritti d'autore e le proprie licenze.
---

<div align="center">
  Sviluppato da <strong>GLX</strong> per i creatori di mappe di Black Ops III.<br />
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest">⬇️ Scarica</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases">Versioni</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues">Segnalazioni</a>
</div>
