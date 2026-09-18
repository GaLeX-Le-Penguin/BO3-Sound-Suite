<div align="center">
  <img src="Assets/bo3_sound_suite.png" alt="Logotipo de BO3 Sound Suite" width="120" />

[🇫🇷 Français](README.md) · [🇬🇧 English](README_EN.md) · 🇪🇸 **Español** · [🇩🇪 Deutsch](README_DE.md) · [🇮🇹 Italiano](README_IT.md)

# BO3 Sound Suite

### El audio de tus mapas de Black Ops III, del archivo original al SZC

Convierte sonidos a WAV de BO3, crea archivos CSV de alias de sonido y añade el CSV a tu mapa desde una sola aplicación para Windows.

[![Última versión](https://img.shields.io/github/v/release/GaLeX-Le-Penguin/BO3-Sound-Suite?label=%C3%BAltima%20versi%C3%B3n)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)
[![Descargas](https://img.shields.io/github/downloads/GaLeX-Le-Penguin/BO3-Sound-Suite/total?label=descargas)](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases)
![Windows x64](https://img.shields.io/badge/Windows-x64-0078D4?logo=windows)
![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)

### [⬇️ Descargar BO3 Sound Suite](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest)

[Todas las versiones](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases) · [Informar de un problema](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues)
</div>

## Acerca de

BO3 Sound Suite reúne los tres pasos de preparación de audio para **Call of Duty: Black Ops III Mod Tools**. La interfaz de la aplicación está disponible en francés e inglés y se abre en el conversor de audio.

```text
Archivos de audio  →  WAV de BO3  →  CSV de alias  →  Archivo SZC del mapa
```

## Funciones

### 🔊 Conversión de audio

- Selecciona o arrastra archivos WAV, MP3, AIFF, AIF y WMA.
- Convierte a WAV de BO3 de **48 kHz / PCM de 16 bits**.
- Guarda los archivos convertidos, de forma predeterminada, en **la misma carpeta que cada sonido original**, incluso fuera de `sound_assets`.
- Cambia la ruta de salida de cada archivo y convierte varios sonidos a la vez.
- Permite crear una copia de seguridad antes de reemplazar un WAV existente; esta opción está desactivada por defecto.
- Detecta archivos de entrada ausentes y conflictos de salida, con un registro de conversión en colores.

### 📋 CSV de alias de sonido

- Añade, elimina y duplica entradas de sonido.
- Usa ajustes predefinidos para efectos 2D/3D, voces, música, ambiente e interfaz.
- Edita alias, volumen, repetición y campos CSV de BO3.
- Inspecciona archivos WAV y previsualiza el CSV antes de guardarlo.
- Abre rápidamente `sound_assets` y `share/raw/sound/aliases` cuando se detectan las carpetas de BO3.

### 🗺️ Editor SZC

- Abre un SZC con el selector o arrastrándolo a la aplicación.
- Añade o sustituye el bloque `ALIAS` que hace referencia al CSV del mapa.
- Edita `Name` y `Filename`, o modifica directamente el contenido SZC.
- Guarda en la carpeta del mapa, con una copia de seguridad opcional.

## Instalación

1. Abre la [última versión](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest).
2. Descarga **`BO3-Sound-Suite-v1.1.13-win-x64.zip`** de *Assets*.
3. Extrae el archivo y ejecuta **`BO3 Sound Suite.exe`**.

La versión distribuida es un ejecutable autónomo para **Windows x64**. No requiere instalar .NET por separado ni ffmpeg.

> [!NOTE]
> Convertir un WAV en su carpeta original reemplaza ese archivo. Activa la opción de copia de seguridad en la aplicación si quieres conservar la versión anterior.

## Compilar el proyecto

El código fuente está en este repositorio. Abre `BO3SoundSuite.sln` en Visual Studio con el SDK de .NET 8, restaura los paquetes NuGet y compila en modo Release. El proyecto usa NAudio 2.2.1 y genera un ejecutable autónomo en `dist` durante la compilación normal.

Los detalles técnicos y el historial de versiones están en [docs/DETAILS.md](docs/DETAILS.md) (en francés).

## Informar de un problema

Abre una [incidencia en GitHub](https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues) e indica la versión de la aplicación, el formato de audio afectado, los pasos para reproducir el problema y el mensaje del registro.

---

<div align="center">
  Desarrollado por <strong>GLX</strong> para creadores de mapas de Black Ops III.<br />
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases/latest">⬇️ Descargar</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/releases">Versiones</a> ·
  <a href="https://github.com/GaLeX-Le-Penguin/BO3-Sound-Suite/issues">Incidencias</a>
</div>
