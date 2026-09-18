# BO3 Sound Suite v1.1.13

Application Windows unique regroupant les trois outils du workflow audio Black Ops III Mod Tools.

## Workflow

1. **Audio**
2. **CSV**
3. **SZC**

L'application s'ouvre sur le module Audio.

## Audio

- Conversion WAV / MP3 / AIFF / AIF / WMA vers WAV BO3 48 kHz / PCM 16 bits.
- Toutes les options de backup sont **désactivées par défaut**.
- Tout fichier ajouté par sélection ou glisser-déposer produit par défaut son WAV converti dans le même dossier que le fichier d'origine.
- Si le fichier d'origine est déjà un WAV, la conversion le remplace dans ce dossier. Activez l'option de sauvegarde pour conserver une copie de l'ancien fichier.
- Le bouton **Ajouter des sons** ouvre le dossier du son déjà présent/sélectionné dans le convertisseur. Si la liste est vide, il ouvre **Téléchargements**.
- Le chemin de sortie de chaque ligne reste modifiable manuellement.
- Conversion de plusieurs fichiers en une seule opération.
- Aucun préfixe ajouté au nom du fichier.
- **Console log** réintégrée : ajout des fichiers, démarrage, succès, échecs et résumé de conversion.
- Les fichiers d'entrée supprimés ou déplacés sont détectés avant conversion.
- Les sorties dupliquées entre plusieurs lignes activées sont bloquées pour empêcher un écrasement accidentel.
- Le remplacement du WAV utilise un fichier temporaire dans le dossier cible puis un remplacement direct, sans suppression préalable de la sortie.

## CSV / Alias CSV

Le module CSV conserve la configuration complète des sons :

- Liste des sons du CSV.
- Ajout / suppression / duplication de sons.
- **Importer CSV** ouvre directement `Call of Duty Black Ops III\share\raw\sound\aliases` quand l'installation BO3 est détectée.
- **Enregistrer CSV** ouvre le dossier du CSV importé ; sans CSV importé, il ouvre `share\raw\sound\aliases`.
- **Ajouter WAV** et le navigateur WAV ouvrent directement `Call of Duty Black Ops III\sound_assets`.
- Alias personnalisable.
- Chemin BO3 relatif à `sound_assets`.
- Presets : Auto, Effet 3D, Effet 2D, Voix 3D, Musique 2D, Ambiance 3D, Interface 2D.
- Volume : Auto, 25, 50, 75, 90, 100 ou personnalisé.
- Looping configurable.
- Inspection du WAV et indication 48 kHz / 16 bits / PCM.
- Détection automatique du preset.
- Aperçu CSV complet.
- Onglet **Paramètres** permettant de modifier directement toutes les colonnes BO3 de la ligne sélectionnée.
- Conservation des valeurs importées qui diffèrent du preset automatique.
- Contrôle de `PriorityMin/PriorityMax` quand `LimitType` ou `EntityLimitType` vaut `PRIORITY`.

## SZC

- **Ouvrir SZC** ouvre directement `Call of Duty Black Ops III\usermaps` quand l'installation BO3 est détectée.
- Ouverture également disponible par drag & drop.
- Détection du premier bloc `Type : ALIAS`.
- Modification en temps réel de `Name` et `Filename`.
- Remplacement du premier ALIAS ou ajout d'un nouvel ALIAS.
- Édition brute du contenu SZC.
- **Enregistrer** ouvre toujours la boîte de sauvegarde : dans le dossier du SZC importé, ou dans `Call of Duty Black Ops III\usermaps` pour un SZC créé dans l'application.
- Répartition des éditeurs : bloc généré 42 %, contenu SZC 58 %.
- Backup SZC disponible avant sauvegarde, **désactivé par défaut**.
- Le repérage du bloc ALIAS a été renforcé : il recherche maintenant les accolades équilibrées au lieu de couper au premier `}` rencontré.

## Détection des dossiers BO3

La v1.1.6 ajoute un service commun de détection des chemins :

- Steam enregistré dans le registre Windows.
- Bibliothèques Steam supplémentaires via `steamapps\libraryfolders.vdf`.
- Chemins standards `SteamLibrary` / `Steam` sur les lecteurs disponibles en secours.
- Dossier Téléchargements Windows redirigé/localisé via la Known Folder Registry quand disponible.

## Corrections issues de l'audit v1.1.6

- Correction d'un risque de faux chemin relatif lorsque l'installation BO3 n'était pas détectée.
- Correction du risque de perte d'un WAV existant entre `File.Delete` et `File.Move` pendant un remplacement.
- Backups horodatés rendus uniques jusque dans la milliseconde avec suffixe de secours, afin d'éviter une collision lors de sauvegardes rapides.
- Détection des conflits de sortie Audio avant conversion.
- Validation de l'existence du fichier d'entrée avant lancement d'une conversion.
- Drag & drop Audio n'annonce plus une copie valide quand aucun fichier déposé n'est compatible.
- Signature nullable de la résolution des en-têtes CSV corrigée.
- Analyse SZC renforcée pour les blocs contenant des structures imbriquées.

## Interface

- Navigation **Audio → CSV → SZC**.
- Interface sombre, boutons/champs/panneaux arrondis.
- Menu de langue personnalisé et arrondi.
- Logo GLX dans la barre latérale.
- Icône `bo3_sound_suite.ico` intégrée à la fenêtre et à l'exécutable.
- Interrupteurs Audio modernes.
- Scrollbars personnalisées.
- Français / English / Auto.

## Compilation Visual Studio

Prérequis : Visual Studio 2022 ou plus récent avec **Desktop development with .NET** et le SDK .NET 8.

1. Ouvrir `BO3SoundSuite.csproj`.
2. Restaurer les packages NuGet.
3. Compiler en `Release | x64`.

Publication autonome :

```bat
build_publish.bat
```

Résultat :

`bin\Release\net8.0-windows\win-x64\publish\`

## Dépendance

Le projet utilise `NAudio 2.2.1`. La publication `self-contained` embarque le runtime .NET. Aucun ffmpeg externe n'est requis.


## v1.1.7 — Console log colorée

La console du convertisseur Audio utilise maintenant un code couleur :

- bleu : informations, ajout de fichiers, chemins et conversion en cours ;
- vert : conversion réussie ;
- jaune : avertissement, suppression, aucune ligne active ou résultat partiellement réussi ;
- rouge : erreur, fichier introuvable, conflit de sortie ou conversion échouée ;
- gris : horodatage.

Le nettoyage automatique des anciens logs conserve désormais la mise en forme colorée.


## v1.1.8 — Documentation des paramètres CSV

- Dans l'onglet **CSV > Paramètres**, laisser la souris environ 1 seconde sur le nom ou la valeur d'un paramètre affiche maintenant une description contextuelle.
- Les descriptions connues sont des reformulations courtes de documentation BO3 Sound Aliases et de références communautaires UGX/Modme.
- Les champs dont le rôle n'est pas documenté publiquement ne reçoivent pas d'explication inventée : l'info-bulle indique explicitement que leur comportement n'a pas pu être confirmé.
- Les champs de priorité ont une aide spécifique, notamment pour `LimitType/EntityLimitType = priority` et la contrainte `PriorityMin != PriorityMax` déjà contrôlée par l'application.
- Les info-bulles respectent la langue FR/EN de l'application et utilisent le thème sombre existant.

Références consultées :

- BO3 Sound Aliases documentation : https://ren.gay/bo3/docs/sound_alias
- UGX-Mods Wiki — Creating/Modifying Soundaliases & Converting Sounds : https://wiki.ugx-mods.com/Modding/Black-Ops-3-Modtools/Sounds/Creating-Modifying-Soundaliases-and-Converting-Sounds.html
- UGX-Mods Wiki — Adding custom sounds : https://github.com/UGX-Mods/community-wiki/blob/main/Modding/Black-Ops-3-Modtools/Asset-Conversion/Adding-custom-sounds.html
- Archives Modme/UGX contenant des alias et `template_mod.csv` BO3 réels.


## v1.1.9

- Logo de l'application avec coins arrondis renforcés sur les quatre côtés.
- ICO multi-résolution régénéré de 16 à 256 px.
- Chargement de l'icône de fenêtre/barre des tâches renforcé via l'icône compilée de l'exécutable, avec fallback sur la ressource embarquée.

## v1.1.10

- Nouveau logo BO3 Sound Suite intégré comme icône Windows de l'exécutable.
- ICO multi-résolution 16 à 256 px régénéré depuis le nouveau logo.
- Même icône utilisée dans la barre de titre et la barre des tâches via l'icône compilée dans l'EXE.

## v1.1.11 — publication en un seul EXE

La publication Windows x64 est maintenant forcée en **single-file self-contained**. Les assemblies .NET et les DLL managées, dont NAudio, sont intégrées au bundle de l'exécutable. Les bibliothèques natives publiables sont également embarquées avec `IncludeNativeLibrariesForSelfExtract`.

Exécuter `build_publish.bat`. Le résultat final est volontairement copié dans :

`dist\BO3 Sound Suite.exe`

Le script vérifie que `dist` ne contient qu'un seul fichier et refuse le résultat si une DLL ou un PDB y reste. `PublishTrimmed` reste désactivé afin d'éviter les régressions WinForms/NAudio liées au trimming.


## v1.1.12 - EXE unique automatique

Le dossier `bin` est le dossier de compilation interne de .NET et peut contenir des DLL, des JSON et des ressources. Il n'est pas le fichier à distribuer.

À chaque Build Visual Studio réel, le projet lance aussi une publication single-file et écrit le fichier distribuable dans :

`dist\BO3 Sound Suite.exe`

Le dossier `dist` est recréé et ne contient qu'un seul fichier. `build_publish.bat` effectue la même publication en Release, vérifie qu'il n'existe ni DLL, ni JSON, ni PDB, ni sous-dossier, puis ouvre automatiquement `dist`.
